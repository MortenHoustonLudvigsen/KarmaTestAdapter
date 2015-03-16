requirejs.config(requireCommonConfig); //Kinda hacky, but needed for gulp config. 

require(['common/config',
    'home/home-config'
    ], function (commonConfig, homeConfig) {

    requirejs.config(eval(homeConfig + '; requireHomeConfig;')); //Kinda hacky, but needed for gulp config. 

    require([
        'require',
        'home/viewmodels/shell',
        'q', 
    ],

    function (require, homeVM, Q) {
        //console.log('!!! Begun Main');
        window.Q = Q;

        Q.longStackSupport = true;
    });

})