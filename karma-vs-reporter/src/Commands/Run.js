var Globals = require('../Globals');
var Util = require('../Util');

var path = require('path');
var extend = require('extend');

function run(config, outputFile, vsConfig, port) {
    Globals.Configure({
        outputFile: outputFile,
        vsConfig: vsConfig
    });

    var karmaConfig = Util.getKarmaConfig(config, {
        configFile: path.resolve(config.karmaConfigFile),
        reporters: ['vs'],
        singleRun: true,
        colors: false,
        port: port
    });

    extend(karmaConfig.preprocessors, { '**/*.js': ['vs'] });

    require('karma').server.start(karmaConfig, function (exitCode) {
        process.exit(exitCode);
    });
}
exports.run = run;
//# sourceMappingURL=Run.js.map
