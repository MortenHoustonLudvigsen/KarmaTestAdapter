import GlobalLog = require('./GlobalLog');

GlobalLog.info('starting karma server');

import path = require('path');
import Karma = require('./Karma');
import freePort = require('./FreePort');
var argv = require('yargs').argv;

try {
    var karmaConfigFile = path.resolve(argv.karma);
    var karmaConfig = Karma.karma.Config.parseConfig(karmaConfigFile, {
        colors: false,
        singleRun: false,
        autoWatch: true,
        loggers: GlobalLog.appenders
    });
    //GlobalLog.setup();

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
            try {
                throw new Error();
            } catch (e) {
                GlobalLog.info(e.stack);
            }
            process.exit(exitCode);
        });
    });
}
catch (e) {
    GlobalLog.info(e);
}


