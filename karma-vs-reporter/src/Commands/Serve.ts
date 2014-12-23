import Util = require('../Util');
import Runner = require('../Runner');
import path = require('path');
var extend = require('extend');

export function run(config: Util.Config, port?) {
    var karmaConfig = Util.getKarmaConfig(config, {
        configFile: path.resolve(config.karmaConfigFile),
        reporters: ['vs'],
        singleRun: false,
        autoWatch: false,
        colors: false,
        port: port
    });

    extend(karmaConfig.preprocessors, { '**/*.js': ['vs'] });

    Runner.replaceRunner();

    require('karma').server.start(karmaConfig, function (exitCode) {
        process.exit(exitCode);
    });
}
 