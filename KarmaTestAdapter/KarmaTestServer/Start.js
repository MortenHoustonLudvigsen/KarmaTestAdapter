var GlobalLog = require('./GlobalLog');
GlobalLog.info('starting karma server');
var path = require('path');
var Karma = require('./Karma');
var freePort = require('../TestServer/FreePort');
var TextFile = require('../TestServer/TextFile');
var argv = require('yargs').argv;
var extend = require('extend');
try {
    var karmaConfigFile = path.resolve(argv.karma);
    var config = {};
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
            process.exit(exitCode);
        });
    });
}
catch (e) {
    GlobalLog.info(e);
}
//# sourceMappingURL=Start.js.map