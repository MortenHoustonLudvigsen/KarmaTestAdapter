import path = require('path');
import extend = require('extend');
import getNodeModules = require('./GetNodeModules');
import createContentTypes = require('./CreateContentTypes');
import TestVS = require('./TestVS');
var flatten = require('flatten-packages');

type Options = {
    name?: string;
    packagePath?: string;
    serverPath?: string;
    build?: string;
    dist?: string;
    output?: string;
    rootSuffix: string;
    testProject?: string;
    visualStudioVersion: string;
};

var defaultOptions: Options = {
    packagePath: 'package.json',
    serverPath: '.',
    build: 'build',
    dist: 'dist',
    output: 'bin',
    lib: 'lib',
    rootSuffix: 'JsTestAdapter',
    visualStudioVersion: process.env.VisualStudioVersion
};

export function config(grunt: any, options: Options): void {
    options = extend({}, defaultOptions, options);

    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-compress');
    grunt.loadNpmTasks('grunt-xmlpoke');

    grunt.config.merge({
        // read in the project settings from the package.json file into the pkg property
        pkg: grunt.file.readJSON(options.packagePath),
        JsTestAdapterPackage: grunt.file.readJSON('JsTestAdapter.json'),
        JsTestAdapterOptions: options,
        JsTestAdapterValues: {
            vsixFile: '<%= JsTestAdapterOptions.dist %>/<%= JsTestAdapterOptions.name %>.vsix'
        },

        clean: {
            JsTestAdapter: [options.build, options.dist]
        },

        copy: {
            JsTestAdapter: {
                files: [
                    // Node modules
                    { expand: true, cwd: options.serverPath, src: getNodeModules(grunt, options.serverPath), dest: options.build },
                    // Lib
                    { expand: true, cwd: options.serverPath, src: ['<%= JsTestAdapterOptions.lib %>/**/*.*'], dest: options.build },
                    // TestServer
                    { expand: true, cwd: options.serverPath, src: ['TestServer/**/*.*'], dest: options.build },
                    // Binaries
                    { expand: true, cwd: options.output, src: ['**', '!*.xml'], dest: options.build },
                    // LICENSE
                    { expand: true, cwd: path.join(options.serverPath, '..'), src: ['LICENSE'], dest: options.build }
                ]
            }
        },

        xmlpoke: {
            'JsTestAdapter-vsix': {
                options: {
                    replacements: [
                        {
                            xpath: '/PackageManifest/Metadata/Identity/@Version',
                            value: '<%= pkg.version %>'
                        }
                    ]
                },
                files: {
                    '<%= JsTestAdapterOptions.build %>/extension.vsixmanifest': 'source.extension.vsixmanifest'
                }
            }
        },

        compress: {
            JsTestAdapter: {
                options: {
                    level: 9,
                    mode: 'zip',
                    archive: '<%= JsTestAdapterValues.vsixFile %>'
                },
                files: [
                    { expand: true, cwd: options.build, src: ['**/*'], dest: '/' }
                ]
            }
        }
    });

    grunt.registerTask('JsTestAdapter-CreateContentTypes', function () {
        createContentTypes(grunt, options.build, path.join(options.build, '[Content_Types].xml'));
    });

    grunt.registerTask('JsTestAdapter-flatten-packages', function () {
        var done = this.async();
        flatten(options.build, {},(err, res) => {
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

    grunt.registerTask('JsTestAdapter-ResetVisualStudio', function () {
        var done = this.async();
        TestVS.reset(grunt, {
            version: options.visualStudioVersion,
            rootSuffix: options.rootSuffix,
            toolsDir: grunt.config('JsTestAdapterPackage').ToolsPath,
            vsixFile: grunt.config('JsTestAdapterValues').vsixFile
        }).then(() => done(), err => done(err));
    });

    grunt.registerTask('JsTestAdapter-RunVisualStudio', function () {
        var done = this.async();
        TestVS.run(grunt, {
            version: options.visualStudioVersion,
            testProject: options.testProject,
            rootSuffix: options.rootSuffix
        }).then(() => done(), err => done(err));
    });
}

