var requireCommonConfig = {
//requirejs.config({
    
    baseUrl: 'ui/src',
    waitSeconds: 15, //Default: 7
    urlArgs: 'r1',
    map: {
        '*': {
            //'css': '../bower_modules/require-css/css',
            'jquery.ui.sortable': 'jquery-ui/sortable' //Specifically added due to knockout-sortable function needing that folder path
        }
    },
    paths: {
        //Folder Paths:

        //jQuery
        'jquery': '../bower_modules/jquery/dist/jquery',
        'jquery-ui': '../bower_modules/jquery-ui/ui',


        //knockout
        'knockout':     '../bower_modules/knockout/dist/knockout',
        'koAmdHelpers': '../bower_modules/knockout-amd-helpers/build/knockout-amd-helpers.min',

        'q':            '../bower_modules/q/q',
        'text':         '../bower_modules/requirejs-text/text',

        'css':          '../bower_modules/require-css/css',
        'css-builder':  '../bower_modules/require-css/css-builder',
        'normalize':    '../bower_modules/require-css/normalize',

    },

    shim: {
        'jquery': { exports: '$' },

        'jquery-ui': { deps: ['jquery'] },

        'q': { exports: 'Q' }
    }
};
