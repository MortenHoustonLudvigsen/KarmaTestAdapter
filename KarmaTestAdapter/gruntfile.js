/// <vs AfterBuild='default' />
/*global module */

var path = require('path');
var fs = require('fs');
var flatten = require('flatten-packages');

module.exports = function (grunt) {
    'use strict';

    var pkg = grunt.file.readJSON('package.json');

    function getDependencies() {
        var result = [];
        for (var dependency in pkg.dependencies) {
            if (pkg.dependencies.hasOwnProperty(dependency)) {
                result.push(dependency);
            }
        }
        return result;
    }

    function getNodeModules() {
        var result = [];
        getDependencies().forEach(function (dependency) {
            result.push('node_modules/' + dependency + '/**');
        });
        //result.push('!node_modules/source-map/bench/**');
        //result.push('!node_modules/source-map/build/**');
        //result.push('!node_modules/source-map/test/**');
        return result;
    }

    function getContentTypes() {
        var extensions = {
            'vsixmanifest': 'text/xml',
            'xml': 'text/xml',
            'js': 'application/javascript',
            'json': 'application/json',
            'md': 'text/plain',
            'markdown': 'text/plain',
            'txt': 'text/plain'
        };
        var overrides = {};

        grunt.file.expand({ filter: 'isFile', cwd: 'build' }, ['**']).forEach(function (file) {
            var extension = path.extname(file);
            if (extension) {
                extension = extension.replace(/^\./, '');
                if (!extensions[extension]) {
                    extensions[extension] = 'application/octet-stream';
                }
            } else {
                file = '/' + file;
                if (!overrides[file]) {
                    overrides[file] = 'application/octet-stream';
                }
            }
        });

        var contentTypes = [];

        for (var extension in extensions) {
            if (extensions.hasOwnProperty(extension)) {
                contentTypes.push('<Default Extension="' + extension + '" ContentType="' + extensions[extension] + '"/>');
            }
        }

        for (var override in overrides) {
            if (overrides.hasOwnProperty(override)) {
                contentTypes.push('<Override PartName="' + override + '" ContentType="' + overrides[override] + '"/>');
            }
        }

        return contentTypes.join('\n');
    }

    grunt.initConfig({
        // read in the project settings from the package.json file into the pkg property
        pkg: grunt.file.readJSON('package.json'),

        clean: {
            dist: ['build', 'dist']
        },

        copy: {
            dist: {
                files: [
                    // Node modules
                    { expand: true, src: getNodeModules(), dest: 'build/' },
                    // Lib
                    { expand: true, src: ['lib/**/*.js'], dest: 'build/' },
                    // Binaries
                    { expand: true, cwd: 'bin/', src: ['**'], dest: 'build/' },
                    // LICENSE
                    { expand: true, src: ['LICENSE'], dest: 'build/' }
                ]
            }
        },

        xmlpoke: {
            updateVersion: {
                options: {
                    xpath: '/PackageManifest/Metadata/Identity/@Version',
                    value: '<%= pkg.version %>'
                },
                files: {
                    'build/extension.vsixmanifest': 'source.extension.vsixmanifest'
                }
            },
            contentTypes: {
                options: {
                    xpath: '/Types',
                    valueType: 'append',
                    value: function (node) {
                        return getContentTypes();
                    }
                },
                files: {
                    'build/[Content_Types].xml': 'Vsix/Content_Types.xml'
                }
            }
        },

        compress: {
            dist: {
                options: {
                    level: 9,
                    mode: 'zip',
                    archive: 'dist/KarmaTestAdapter.vsix'
                },
                files: [
                    { expand: true, cwd: 'build/', src: ['**/*'], dest: '/' }
                ]
            }
        },

        commands: {
            resetExperimentalHub: {
                cmd: 'ResetExperimentalHub.bat'
            },
            startExperimentalHub: {
                cmd: 'StartExperimentalHub.bat'
            }
        }
    });

    grunt.registerTask('build-done', function () {
        grunt.file.write('build/build-done', '');
    });

    grunt.registerTask('flatten-packages', function () {
        var done = this.async();
        flatten('build', {}, function (err, res) {
            if (err) {
                grunt.log.error(err);
                done(false);
            }
            if (res) {
                grunt.log.writeln(res);
                done();
            }
        });
    });

    grunt.registerTask('default', [
        'clean:dist',
        'copy:dist',
        'flatten-packages',
        'xmlpoke:updateVersion',
        'xmlpoke:contentTypes',
        'compress:dist',
        'build-done'
    ]);

    grunt.registerTask('resetExperimentalHub', [
        'commands:resetExperimentalHub'
    ]);

    grunt.registerTask('startExperimentalHub', [
        'commands:startExperimentalHub'
    ]);

    // Add all plugins that your project needs here
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-compress');
    grunt.loadNpmTasks('grunt-xmlpoke');
    grunt.loadNpmTasks('grunt-commands');
};