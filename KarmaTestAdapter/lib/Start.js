var GlobalLog = require('./GlobalLog');
GlobalLog.info('starting karma server');
var path = require('path');
var Karma = require('./Karma');
var freePort = require('./FreePort');
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
    karmaConfig.frameworks = karmaConfig.frameworks.map(function (framework) {
        switch (framework) {
            case 'jasmine':
                return 'vs-jasmine';
            default:
                return framework;
        }
    });
    freePort().then(function (port) {
        karmaConfig.port = port;
    }).then(function () {
        return freePort(karmaConfig.port + 1).then(function (p) { return karmaConfig.vsServerPort = p; });
    }).then(function () {
        Karma.karma.Server.start(karmaConfig, function (exitCode) {
            GlobalLog.info('exitCode: ' + exitCode);
            try {
                throw new Error();
            }
            catch (e) {
                GlobalLog.info(e.stack);
            }
            process.exit(exitCode);
        });
    });
}
catch (e) {
    GlobalLog.info(e);
}
//# sourceMappingURL=Start.js.map