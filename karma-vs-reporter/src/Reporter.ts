import Util = require('./Util');
import Commands = require('./Commands');
import Test = require('./Test');
import q = require('q');
import path = require('path');
import fs = require('fs');
import parseFiles = require('./ParseFiles');
import _ = require("lodash");
import util = require('util');

var Reporter: any = function Reporter(baseReporterDecorator, config, fileList, helper, logger, formatError, emitter) {
    baseReporterDecorator(this);
    Util.baseDir = config.basePath;

    var log = Util.createLogger(logger);
    var outputFile = config.vsReporter.outputFile || helper.normalizeWinPath(path.resolve(config.basePath, Util.outputFile));
    var filesPromise = fileList.refresh();

    emitter.on('file_list_modified', function (emittedFilesPromise) {
        filesPromise = emittedFilesPromise;
    });

    var filesParsed;
    var browserResults;
    var karma: Test.Karma;

    this.onRunStart = function () {
        karma = new Test.Karma();
        karma.add(new Test.KarmaConfig(config));
        browserResults = {};
        filesParsed = q.defer();
        filesPromise.then(function (files) {
            parseFiles(karma, files, log);
            filesParsed.resolve();
        });
    };

    this.onBrowserStart = function (browser) {
    };

    this.onBrowserComplete = function (browser) {
    };

    this.onSpecComplete = function (browser, result) {
        browserResults[browser.name] = browserResults[browser.name] || {
            browser: browser,
            results: []
        };

        browserResults[browser.name].results.push(result);
    };

    this.onRunComplete = function () {
        filesParsed.promise.then(function () {
            _.forIn(browserResults, browserResult => {
                var browser = karma.results().add(new Test.Browser(browserResult.browser.name));

                browserResult.results.forEach(result => {
                    var suite = browser.startSuite(result.suite);
                    var testResult = suite.add(new Test.TestResult(result.description));
                    testResult.id = result.id;
                    testResult.time = result.time;
                    testResult.outcome = getOutcome(result);

                    result.log.forEach(function (logItem) {
                        testResult.log.push(formatError(logItem).replace(/\s+$/, ''));
                    });
                });
            });

            try {
                Util.writeFile(outputFile, karma.toXml());
            } catch (e) {
                log.error(e);
            }
        });
    };

    function getOutcome(result): Test.Outcome {
        if (result.success) return Test.Outcome.Success;
        if (result.skipped) return Test.Outcome.Skipped;
        return Test.Outcome.Failed;
    }
}

Reporter.$inject = ['baseReporterDecorator', 'config', 'fileList', 'helper', 'logger', 'formatError', 'emitter'];

export = Reporter;
