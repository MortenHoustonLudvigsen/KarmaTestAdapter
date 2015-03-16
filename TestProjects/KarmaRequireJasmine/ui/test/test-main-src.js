requirejs(['jquery', './../base/src/common/config', './../base/src/home/home-config'], function ($) {
    var tests = [];

    for (var file in window.__karma__.files) {
        if (window.__karma__.files.hasOwnProperty(file)) {
            if (/test\/(home|common|lib)\/.*\.js$/.test(file)) {
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
        urlArgs: '',
        paths: paths,
        deps: tests,
        callback: window.__karma__.start,
    });

    require([
        'knockout'
    ],
        function (ko) {
            window.ko = ko;
        });
});