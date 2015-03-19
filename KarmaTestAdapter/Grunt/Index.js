var path = require('path');
var getNodeModules = require('./GetNodeModules');
var getContentTypes = require('./GetContentTypes');
var extend = require('extend');
var flatten = require('flatten-packages');
var defaultOptions = {
    packagePath: 'package.json',
    serverPath: '.',
    build: 'build',
    dist: 'dist',
    output: 'bin',
    lib: 'lib'
};
function config(grunt, options) {
    options = extend({}, defaultOptions, options);
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-compress');
    grunt.loadNpmTasks('grunt-xmlpoke');
    grunt.config.merge({
        // read in the project settings from the package.json file into the pkg property
        pkg: grunt.file.readJSON(options.packagePath),
        JsTestAdapterOptions: options,
        clean: {
            JsTestAdapter: [options.build, options.dist]
        },
        copy: {
            JsTestAdapter: {
                files: [
                    { expand: true, cwd: options.serverPath, src: getNodeModules(grunt, options.serverPath), dest: options.build },
                    { expand: true, cwd: options.serverPath, src: ['<%= JsTestAdapterOptions.lib %>/**/*.*'], dest: options.build },
                    { expand: true, cwd: options.serverPath, src: ['TestServer/**/*.*'], dest: options.build },
                    { expand: true, cwd: options.output, src: ['**', '!*.xml'], dest: options.build },
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
                    '<%= JsTestAdapterOptions.build %>/extension.vsixmanifest': 'Vsix/source.extension.vsixmanifest'
                }
            },
            'JsTestAdapter-contentTypes': {
                options: {
                    xpath: '/Types',
                    valueType: 'append',
                    value: function (node) {
                        return getContentTypes(grunt, options.build);
                    }
                },
                files: {
                    '<%= JsTestAdapterOptions.build %>/[Content_Types].xml': 'Vsix/Content_Types.xml'
                }
            }
        },
        compress: {
            JsTestAdapter: {
                options: {
                    level: 9,
                    mode: 'zip',
                    archive: '<%= JsTestAdapterOptions.dist %>/<%= JsTestAdapterOptions.name %>.vsix'
                },
                files: [
                    { expand: true, cwd: options.build, src: ['**/*'], dest: '/' }
                ]
            }
        }
    });
    grunt.registerTask('JsTestAdapter-flatten-packages', function () {
        var done = this.async();
        flatten(options.build, {}, function (err, res) {
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
    grunt.registerTask('JsTestAdapter', [
        'clean:JsTestAdapter',
        'copy:JsTestAdapter',
        'JsTestAdapter-flatten-packages',
        'xmlpoke:JsTestAdapter-vsix',
        'xmlpoke:JsTestAdapter-contentTypes',
        'compress:JsTestAdapter'
    ]);
}
exports.config = config;
//# sourceMappingURL=Index.js.map