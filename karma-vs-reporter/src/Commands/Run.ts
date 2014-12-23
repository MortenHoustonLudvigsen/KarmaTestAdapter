import Globals = require('../Globals');
import Util = require('../Util');
import VsConfig = require('../VsConfig');
import path = require('path');
var extend = require('extend');

export function run(config: Util.Config, outputFile: string, vsConfig: VsConfig.Config, port?) {
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
 