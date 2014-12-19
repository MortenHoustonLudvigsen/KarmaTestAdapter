import util = require('util');
import Util = require('./Util');
import Javascript = require('./Javascript');
import Test = require('./Test');
import VsConfig = require('./VsConfig');
import JasmineParser = require('./JasmineParser');
import path = require('path');
import parseFiles = require('./ParseFiles');
import _ = require('lodash');
var extend = require('extend');

module Commands {
    export function init(configFile) {
        Util.writeConfigFile(configFile);
    }

    function getKarmaConfig(config: Util.Config) {
        var logger = require("karma/lib/logger");
        var cfg = require('karma/lib/config');
        var karmaConfigFile = path.resolve(config.karmaConfigFile);

        // Config
        logger.setup('INFO', false);
        var log = Util.createLogger(logger);

        var karmaConfig: any = {
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

    export function getConfig(config: Util.Config, outputFile: string) {
        var karmaConfig = getKarmaConfig(config);
        Util.writeFile(outputFile, JSON.stringify(karmaConfig, undefined, 4));
    }

    export function discover(config: Util.Config, outputFile: string) {
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

        var discoverTests: any = function (fileList, logger, config) {
            var log = Util.createLogger(logger);
            try {
                Util.baseDir = config.basePath;
                var karma = new Test.Karma();

                karma.add(new Test.KarmaConfig(karmaConfig));

                fileList.refresh().then(function (files) {
                    try {
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
        }
        discoverTests.$inject = ['fileList', 'logger', 'config'];

        new di.Injector(modules).invoke(discoverTests);
    }

    export function run(config: Util.Config, outputFile: string, vsConfig: VsConfig.Config, port?) {
        var origConfig = getKarmaConfig(config);
        var karmaConfig: any = {
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
            karmaConfig.files = vsConfig.files.map(f => <any>{
                pattern: f.path,
                watched: false,
                included: f.included,
                served: f.served
            });

            karmaConfig.preprocessors = origConfig.preprocessors || {};
            vsConfig.files.filter(f => f.hasTests()).forEach(f => {
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
}

export = Commands;