import jsTestAdapter = require('./Grunt/Index');

function config(grunt) {
    grunt.initConfig({
    });

    jsTestAdapter.config(grunt, {
        name: 'KarmaTestAdapterTests'
    });

    grunt.registerTask('default', []);
}

export = config;
