/// <vs AfterBuild='default' />

import jsTestAdapter = require('./Grunt/Index');

function config(grunt) {
    var buildConfig = grunt.file.readJSON('./BuildConfig.json');

    grunt.initConfig({
        buildConfig: buildConfig,
        exec: {
            resetTestVS: {
                cmd: 'ResetTestVS.bat <%= buildConfig.VisualStudioVersion %>'
            },
            startTestVS: {
                cmd: 'StartTestVS.bat <%= buildConfig.VisualStudioVersion %>'
            }
        }
    });

    jsTestAdapter.config(grunt, {
        name: 'KarmaTestAdapter',
        lib: 'KarmaTestServer'
    });

    if (buildConfig.VisualStudioVersion === "12.0") {
        grunt.registerTask('default', [
            'JsTestAdapter'
        ]);
    } else {
        grunt.registerTask('default', [
            'JsTestAdapter',
            'exec:resetTestVS'
        ]);
    }

    grunt.registerTask('ResetTestVS', [
        'exec:resetTestVS'
    ]);

    grunt.registerTask('StartTestVS', [
        'exec:startTestVS'
    ]);

    grunt.loadNpmTasks('grunt-exec');
}

export = config;
