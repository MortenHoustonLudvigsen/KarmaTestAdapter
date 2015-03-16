requirejs(['jquery', 'common/config', 'home/home-config'], function ($) {
    var tests = [];

    for (var file in window.__karma__.files) {
        if (window.__karma__.files.hasOwnProperty(file)) {
            if (/test\/(common|home)\/.*\.js$/.test(file)) {
                console.log('File: ' + file);
                tests.push(file);
            }
        } 
    }

    console.log('Timestamp: ' + (new Date).toLocaleTimeString());

    //brought both variables in from includs in karma
    var paths = requireCommonConfig.paths, //Kinda hacky, but needed for gulp/karma config. Setting up base require.config before getting the paths for the home configs setup (require common paths)
        homePaths = requireHomeConfig.paths; //Kinda hacky, but needed for gulp/karma config. 

    $.extend(paths, homePaths);

    requirejs.config({
        baseUrl: '/base/src',

        paths: paths,
        deps: tests,
        map: {
            '*': {
                'home/main': 'common/blankModule',//Hacky way of stopping the main file from executing in the bundle
                'home/viewmodels/shell': 'common/blankModule'
            }
        },
        callback: window.__karma__.start,
    });
});