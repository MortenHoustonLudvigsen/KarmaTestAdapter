var Util = require('./Util');
var Globals = require('./Globals');

var Test = require('./Test');
var q = require('q');
var path = require('path');

var parseFiles = require('./ParseFiles');
var _ = require("lodash");

var Reporter = function Reporter(baseReporterDecorator, config, fileList, helper, logger, formatError, emitter) {
    baseReporterDecorator(this);
    Util.baseDir = config.basePath;

    var log = Util.createLogger(logger);
    var filesPromise = fileList.refresh();

    emitter.on('file_list_modified', function (emittedFilesPromise) {
        filesPromise = emittedFilesPromise;
    });

    var filesParsed;
    var browserResults;
    var karma;

    this.onRunStart = function () {
        karma = new Test.Karma(new Date());
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
        karma.end = new Date();
        filesParsed.promise.then(function () {
            _.forIn(browserResults, function (browserResult) {
                var browser = karma.results().add(new Test.Browser(browserResult.browser.name));

                browserResult.results.forEach(function (result) {
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

            var outputFile = Globals.outputFile || helper.normalizeWinPath(path.resolve(config.basePath, Util.outputFile));
            try  {
                Util.writeFile(outputFile, karma.toXml());
            } catch (e) {
                log.error(e);
            }
        });
    };

    function getOutcome(result) {
        if (result.success)
            return 0 /* Success */;
        if (result.skipped)
            return 1 /* Skipped */;
        return 2 /* Failed */;
    }
};

Reporter.$inject = ['baseReporterDecorator', 'config', 'fileList', 'helper', 'logger', 'formatError', 'emitter'];
Reporter.name = 'karma-vs-reporter';

module.exports = Reporter;
//# sourceMappingURL=Reporter.js.map
