import GlobalLog = require('./GlobalLog');

GlobalLog.info('starting karma server');

import path = require('path');
import Karma = require('./Karma');
import freePort = require('../node_modules/JsTestAdapter/FreePort');
import TextFile = require('../node_modules/JsTestAdapter/TextFile');
var argv = require('yargs').argv;
var extend = require('extend');

try {
    var karmaConfigFile = path.resolve(argv.karma);

    var config: any = {};

    if (argv.settings) {
        var settings = TextFile.readJson(argv.settings);
        if (settings.config) {
            extend(config, settings.config);
        }
    }

    extend(config, {
        reporters: ['vs'],
        colors: false,
        singleRun: argv.singleRun === 'true',
        autoWatch: true,
        loggers: GlobalLog.appenders
    });

    var karmaConfig = Karma.karma.Config.parseConfig(karmaConfigFile, config);

    karmaConfig.plugins.push(require('./Index'));

    karmaConfig.frameworks = karmaConfig.frameworks.map(framework => {
        switch (framework) {
            case 'jasmine':
                return 'vs-jasmine';
            default:
                return framework;
        }
    });

    freePort().then(port => {
        karmaConfig.port = port;
    }).then(() => {
        return freePort(karmaConfig.port + 1).then(p => karmaConfig.vsServerPort = p);
    }).then(() => {
        Karma.karma.Server.start(karmaConfig, function (exitCode) {
            GlobalLog.info('exitCode: ' + exitCode);
            process.exit(exitCode);
        });
    });
}
catch (e) {
    GlobalLog.info(e);
}


