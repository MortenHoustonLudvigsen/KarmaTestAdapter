var Util = require('./Util');

var Test = require('./Test');

var path = require('path');
var parseFiles = require('./ParseFiles');
var _ = require('lodash');
var extend = require('extend');

var Commands;
(function (Commands) {
    function init(configFile) {
        Util.writeConfigFile(configFile);
    }
    Commands.init = init;

    function getKarmaConfig(config) {
        var logger = require("karma/lib/logger");
        var cfg = require('karma/lib/config');
        var karmaConfigFile = path.resolve(config.karmaConfigFile);

        // Config
        logger.setup('INFO', false);
        var log = Util.createLogger(logger);

        var karmaConfig = {
            singleRun: true,
            browsers: [],
            reporters: [],
            colors: false,
            logLevel: 'INFO'
        };

        if (_.isObject(config.config)) {
            karmaConfig = extend(karmaConfig, config.config);
        }

        return cfg.parseConfig(karmaConfigFile, karmaConfig);
    }

    function getConfig(config, outputFile) {
        var karmaConfig = getKarmaConfig(config);
        Util.writeFile(outputFile, JSON.stringify(karmaConfig, undefined, 4));
    }
    Commands.getConfig = getConfig;

    function discover(config, outputFile) {
        var di = require('karma/node_modules/di');
        var logger = require("karma/lib/logger");
        var preprocessor = require('karma/lib/preprocessor');
        var fileList = require('karma/lib/file_list').List;
        var emitter = require('karma/lib/events').EventEmitter;
        var karmaConfig = getKarmaConfig(config);

        var modules = [{
                logger: ['value', logger],
                emitter: ['type', emitter],
                config: ['value', karmaConfig],
                preprocess: ['factory', preprocessor.createPreprocessor],
                fileList: ['type', fileList]
            }];

        var discoverTests = function (fileList, logger, config) {
            var log = Util.createLogger(logger);
            try  {
                Util.baseDir = config.basePath;
                var karma = new Test.Karma();

                karma.add(new Test.KarmaConfig(karmaConfig));

                fileList.refresh().then(function (files) {
                    try  {
                        parseFiles(karma, files, log);
                        var xml = karma.toXml();
                        Util.writeFile(outputFile, karma.toXml());
                    } catch (e) {
                        log.error(e);
                    }
                });
            } catch (e) {
                log.error(e);
            }
        };
        discoverTests.$inject = ['fileList', 'logger', 'config'];

        new di.Injector(modules).invoke(discoverTests);
    }
    Commands.discover = discover;

    function run(config, outputFile, vsConfig, port) {
        var origConfig = getKarmaConfig(config);
        var karmaConfig = {
            configFile: path.resolve(config.karmaConfigFile),
            reporters: ['progress', 'vs'],
            singleRun: true,
            colors: false,
            vsReporter: {
                outputFile: outputFile,
                vsConfig: vsConfig
            }
        };

        if (_.isObject(config.config)) {
            karmaConfig = extend(karmaConfig, config.config);
        }

        if (vsConfig.hasFiles()) {
            karmaConfig.files = vsConfig.files.map(function (f) {
                return {
                    pattern: f.path,
                    watched: false,
                    included: f.included,
                    served: f.served
                };
            });

            karmaConfig.preprocessors = origConfig.preprocessors || {};
            vsConfig.files.filter(function (f) {
                return f.hasTests();
            }).forEach(function (f) {
                karmaConfig.preprocessors[f.path] = ['vs'];
            });
        }

        if (port) {
            karmaConfig.port = port;
        }

        require('karma').server.start(karmaConfig, function (exitCode) {
            process.exit(exitCode);
        });
    }
    Commands.run = run;
})(Commands || (Commands = {}));

module.exports = Commands;
//# sourceMappingURL=Commands.js.map
