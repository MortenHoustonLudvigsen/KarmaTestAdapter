/*global module */

module.exports = function (grunt) {
    'use strict';

    grunt.initConfig({
        dts_bundle: {
            build: {
                options: {
                    name: 'JsTestAdapter',
                    main: 'TestServer/Index.d.ts',
                    baseDir: 'TestServer/',
                }
            }
        }
    });

    grunt.registerTask('default', [
        'dts_bundle:build'
    ]);

    // Add all plugins that your project needs here
    grunt.loadNpmTasks('grunt-dts-bundle');
};