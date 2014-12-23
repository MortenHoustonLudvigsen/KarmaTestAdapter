var Util = require('../Util');
var Runner = require('../Runner');
var path = require('path');
var extend = require('extend');

function run(config, port) {
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
exports.run = run;
//# sourceMappingURL=Serve.js.map
