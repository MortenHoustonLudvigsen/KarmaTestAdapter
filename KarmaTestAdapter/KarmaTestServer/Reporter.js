var path = require('path');
var url = require('url');
var SourceUtils = require('../TestServer/SourceUtils');
var Karma = require('./Karma');
var VsBrowser = (function () {
    function VsBrowser(browser) {
        this.browser = browser;
        this.results = [];
        this.uniqueNames = {};
        this.totalTime = 0;
        this.timesValid = true;
    }
    VsBrowser.prototype.addResult = function (spec, result) {
        this.results.push(result);
        if (this.timesValid && typeof result.startTime === 'number' && typeof result.endTime === 'number') {
            this.totalTime += result.time;
            this.startTime = typeof this.startTime === 'number' ? Math.min(this.startTime, result.startTime) : result.startTime;
            this.endTime = typeof this.endTime === 'number' ? Math.max(this.endTime, result.endTime) : result.endTime;
        }
        else {
            this.timesValid = false;
        }
    };
    VsBrowser.prototype.getUniqueName = function (eventOrSuite, description) {
        if (eventOrSuite instanceof Array) {
            var suite = eventOrSuite;
            var uniqueName = suite.map(function (name) {
                name = name.replace(/\./g, '-');
            }).join(' / ') + '.' + description;
            if (this.uniqueNames[uniqueName]) {
                var no = 2;
                while (this.uniqueNames[uniqueName + '-' + no]) {
                    no += 1;
                }
                uniqueName = uniqueName + '-' + no;
            }
            return uniqueName;
        }
        else {
            var event = eventOrSuite;
            return this.getUniqueName(event.suite, event.description);
        }
    };
    VsBrowser.prototype.adjustResults = function () {
        this.adjustTimes();
    };
    VsBrowser.prototype.adjustTimes = function () {
        if (this.timesValid && typeof this.startTime === 'number' && typeof this.endTime === 'number') {
            var diff = ((this.endTime - this.startTime) - this.totalTime) / this.results.length;
            this.results.forEach(function (result) { return result.time = Math.max(0.01, result.time + diff); });
        }
    };
    return VsBrowser;
})();
var Reporter = (function () {
    function Reporter(config, server, logger) {
        this.config = config;
        this.server = server;
        this.urlRoot = this.config.urlRoot || '/';
        this.urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
        this.urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
        this.basePath = this.config.basePath;
        this.specMap = {};
        this.specs = [];
        this.output = [];
        this.logger = logger.create('VS Reporter', Karma.karma.Constants.LOG_DEBUG);
        this.logger.info("Created");
    }
    Reporter.prototype.getSpec = function (browser, spec) {
        var existingSpec;
        if (existingSpec = this.specMap[spec.id]) {
            existingSpec.source = existingSpec.source || this.sourceUtils.getSource(spec.sourceStack);
        }
        else {
            existingSpec = this.specMap[spec.id] = {
                id: spec.id,
                description: spec.description,
                uniqueName: spec.uniqueName || browser.vsBrowser.getUniqueName(spec),
                suite: spec.suite,
                source: this.sourceUtils.getSource(spec.sourceStack),
                results: []
            };
            this.specs.push(existingSpec);
        }
        return existingSpec;
    };
    Reporter.prototype.onRunStart = function (data) {
        var _this = this;
        this.server.testRunStarted();
        this.specMap = {};
        this.specs = [];
        this.output = [];
        this.sourceUtils = new SourceUtils(this.basePath, this.logger, function (fileName) {
            if (typeof fileName === 'string') {
                var filePath = url.parse(fileName).pathname;
                if (filePath.indexOf(_this.urlBase) === 0) {
                    return path.join(_this.basePath, filePath.substring(_this.urlBase.length));
                }
                else if (filePath.indexOf(_this.urlAbsoluteBase) === 0) {
                    return filePath.substring(_this.urlAbsoluteBase.length);
                }
            }
            return fileName;
        });
    };
    Reporter.prototype.onRunComplete = function (browsers, results) {
        this.server.testRunCompleted(this.specs);
    };
    Reporter.prototype.onBrowserStart = function (browser) {
        this.output = [];
        browser.vsBrowser = new VsBrowser(browser);
    };
    Reporter.prototype.onBrowserError = function (browser, error) {
        var _this = this;
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);
        var failures;
        var source;
        var stackFrames = this.sourceUtils.parseStack({ stack: error }, false);
        if (stackFrames) {
            source = stackFrames[0];
            failures = [{
                message: error.split(/(\r\n|\n|\r)/g)[0],
                passed: false,
                stack: error
            }];
        }
        var id = browser.vsBrowser.getUniqueName([], "Uncaught error");
        var event = {
            description: "Uncaught error",
            id: id,
            log: [error],
            skipped: false,
            success: false,
            suite: [],
            time: 0,
            startTime: undefined,
            endTime: undefined,
            uniqueName: id,
            source: source,
            failures: failures
        };
        var spec = this.getSpec(browser, event);
        var result = {
            name: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(function (failure) { return {
                message: failure.message,
                stack: _this.sourceUtils.normalizeStack(failure.stack),
                passed: failure.passed
            }; }) : undefined
        };
        browser.vsBrowser.addResult(spec, result);
        spec.results.push(result);
        this.output = [];
    };
    Reporter.prototype.onBrowserLog = function (browser, message, type) {
        if (typeof message === 'string') {
            message = message.replace(/^'(.*)'$/, '$1');
        }
        this.output.push(message);
    };
    Reporter.prototype.onBrowserComplete = function (browser) {
        this.output = [];
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);
        browser.vsBrowser.adjustResults();
    };
    Reporter.prototype.onSpecComplete = function (browser, result) {
        result.source = this.sourceUtils.resolveSource(result.source);
        switch (result.event) {
            case 'suite-start':
                this.onSuiteStart(browser, result);
                break;
            case 'suite-done':
                this.onSuiteDone(browser, result);
                break;
            case 'spec-start':
                this.onSpecStart(browser, result);
                break;
            case 'spec-done':
                this.onSpecDone(browser, result);
                break;
            default:
                this.onSpecDone(browser, result);
                break;
        }
    };
    Reporter.prototype.onSuiteStart = function (browser, event) {
        this.output = [];
    };
    Reporter.prototype.onSuiteDone = function (browser, event) {
        this.output = [];
    };
    Reporter.prototype.onSpecStart = function (browser, event) {
        this.output = [];
    };
    Reporter.prototype.onSpecDone = function (browser, event) {
        var _this = this;
        var spec = this.getSpec(browser, event);
        var result = {
            name: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(function (exp) { return {
                message: exp.message,
                stack: _this.sourceUtils.normalizeStack(exp.stack),
                passed: exp.passed
            }; }) : undefined
        };
        browser.vsBrowser.addResult(spec, result);
        spec.results.push(result);
        this.output = [];
    };
    Reporter.name = 'karma-vs-reporter';
    Reporter.$inject = ['config', 'karma-vs-server', 'logger'];
    return Reporter;
})();
module.exports = Reporter;
//# sourceMappingURL=Reporter.js.map