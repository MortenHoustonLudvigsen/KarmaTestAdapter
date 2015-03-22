import GlobalLog = require('./GlobalLog');

GlobalLog.info('starting karma server');

import util = require('util');
import path = require('path');
import Karma = require('./Karma');
import freePort = require('../TestServer/FreePort');
import TextFile = require('../TestServer/TextFile');
var argv = require('yargs').argv;
var extend = require('extend');

try {
    var karmaConfigFile = path.resolve(argv.karma);

    var config: any = {};
    var settings: any = {};

    if (argv.settings) {
        settings = TextFile.readJson(argv.settings);
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

    karmaConfig.vs = {
        name: argv.name,
        traits: settings.Traits,
        extensions: settings.Extensions
    };

    freePort().then(port => {
        karmaConfig.port = port;
    }).then(() => {
        return freePort(karmaConfig.port + 1).then(p => karmaConfig.vs.serverPort = p);
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


