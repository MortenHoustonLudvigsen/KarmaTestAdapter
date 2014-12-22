import util = require('util');
import Util = require('./Util');
import Javascript = require('./Javascript');
import Test = require('./Test');
import VsConfig = require('./VsConfig');
import JasmineParser = require('./JasmineParser');
import path = require('path');
import parseFiles = require('./ParseFiles');
import Globals = require('./Globals');
import Runner = require('./Runner');
import _ = require('lodash');
import http = require('http');
import querystring = require('querystring');
var extend = require('extend');

module Commands {
    export function init(configFile) {
        Util.writeConfigFile(configFile);
    }

    function getKarmaConfig(config: Util.Config, extensions?) {
        var logger = require("karma/lib/logger");
        var cfg = require('karma/lib/config');
        var karmaConfigFile = path.resolve(config.karmaConfigFile);

        // Config
        logger.setup('INFO', false);
        var log = Util.createLogger(logger);

        var karmaConfig: any = {
            configFile: karmaConfigFile,
            singleRun: true,
            browsers: [],
            reporters: [],
            colors: false,
            logLevel: 'INFO'
        };

        if (_.isObject(config.config)) {
            karmaConfig = extend(karmaConfig, config.config);
        }

        Globals.origConfig = cfg.parseConfig(karmaConfigFile, karmaConfig);
        karmaConfig = extend({}, Globals.origConfig, extensions);
        Util.baseDir = karmaConfig.basePath;
        return karmaConfig;
    }

    export function getConfig(config: Util.Config, outputFile: string) {
        Util.writeFile(outputFile, JSON.stringify(getKarmaConfig(config), undefined, 4));
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

        Globals.Configure({ outputFile: outputFile });

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
                        Util.writeFile(Globals.outputFile, karma.toXml());
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
        var karmaConfig = getKarmaConfig(config, {
            configFile: path.resolve(config.karmaConfigFile),
            reporters: ['vs'],
            singleRun: true,
            colors: false,
            port: port
        });

        Globals.Configure({
            outputFile: outputFile,
            vsConfig: vsConfig
        });

        vsConfig.processKarmaConfig(karmaConfig);

        require('karma').server.start(karmaConfig, function (exitCode) {
            process.exit(exitCode);
        });
    }

    export function serve(config: Util.Config, port?) {
        var karmaConfig = getKarmaConfig(config, {
            configFile: path.resolve(config.karmaConfigFile),
            reporters: ['progress', 'vs'],
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

    export function servedRun(config: Util.Config, outputFile: string, vsConfigFile: string, port?) {
        var karmaConfig = getKarmaConfig(config, {
            port: port
        });

        var exitCode = 1;
        var options = {
            hostname: karmaConfig.hostname,
            path: karmaConfig.urlRoot + 'run?' + querystring.stringify({ outputFile: outputFile, vsConfig: vsConfigFile }),
            port: karmaConfig.port,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        };

        var request = http.request(options, function (response) {
            function parseExitCode(buffer, defaultCode) {
                var constants = require('karma/lib/constants');
                var tailPos = buffer.length - Buffer.byteLength(constants.EXIT_CODE) - 1;

                if (tailPos < 0) {
                    return defaultCode;
                }

                // tail buffer which might contain the message
                var tail = buffer.slice(tailPos);
                var tailStr = tail.toString();
                if (tailStr.substr(0, tailStr.length - 1) === constants.EXIT_CODE) {
                    tail.fill('\x00');
                    return parseInt(tailStr.substr(-1), 10);
                }

                return defaultCode;
            }

            response.on('data', function (buffer) {
                exitCode = parseExitCode(buffer, exitCode);
                process.stdout.write(buffer);
            });

            response.on('end', function () {
                process.exit(exitCode);
            });
        });

        request.on('error', function (e) {
            if (e.code === 'ECONNREFUSED') {
                console.error('There is no server listening on port %d', options.port);
                process.exit(1);
            } else {
                throw e;
            }
        });

        request.end(JSON.stringify({ refresh: false }));
    }
}

export = Commands;