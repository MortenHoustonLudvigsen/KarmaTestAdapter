var fs = require('fs');
var util = require('util');
var url = require('url');
var path = require('path');
var Karma = require('./Karma');
var errorStackParser = require('error-stack-parser');
var SourceMap = require("source-map");
var SourceMapResolve = require("source-map-resolve");
function pad(value, count) {
    if (typeof value === 'number') {
        return pad(value.toString(10), count);
    }
    if (typeof value === 'string') {
        return (value + Array(count + 1).join(' ')).slice(0, count - 1);
    }
    return pad('', count);
}
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
        var _this = this;
        this.config = config;
        this.server = server;
        this.logValue = function (name, status, value) {
            if (_this.logValue.enabled) {
                var message = pad(name, 20) + " " + status;
                if (typeof value !== 'undefined') {
                    message += '\n' + util.inspect(value, { depth: null });
                }
                _this.logger.debug(message);
            }
        };
        this.urlRoot = this.config.urlRoot || '/';
        this.urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
        this.urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
        this.basePath = this.config.basePath;
        this.specMap = {};
        this.specs = [];
        this.output = [];
        this.sourceMapConsumers = {};
        this.logger = logger.create('VS Reporter', Karma.karma.Constants.LOG_DEBUG);
        this.logger.info("Created");
        //this.logValue.enabled = true;
        this.logValue("Reporter", "Created", {
            basePath: this.basePath,
            urlRoot: this.urlRoot,
            urlBase: this.urlBase,
            urlAbsoluteBase: this.urlAbsoluteBase,
            logger: this.logger
        });
    }
    Reporter.prototype.getSourceMapConsumer = function (filePath) {
        if (filePath in this.sourceMapConsumers) {
            return this.sourceMapConsumers[filePath];
        }
        var content = fs.readFileSync(filePath).toString();
        var sourceMap = SourceMapResolve.resolveSync(content, filePath, fs.readFileSync);
        var consumer = sourceMap ? new SourceMap.SourceMapConsumer(sourceMap.map) : null;
        if (consumer) {
            consumer['resolvePath'] = function (filePath) { return path.resolve(path.dirname(sourceMap.sourcesRelativeTo), filePath); };
        }
        return consumer;
    };
    Reporter.prototype.resolveSource = function (source) {
        if (source && source.fileName) {
            source.fileName = this.getFilePath(source.fileName);
            var consumer = this.getSourceMapConsumer(source.fileName);
            if (consumer) {
                var position = {
                    line: Math.max(source.lineNumber || 1, 1),
                    column: Math.max(source.columnNumber || 1, 1) - 1,
                    bias: SourceMap.SourceMapConsumer.GREATEST_LOWER_BOUND
                };
                var orig = consumer.originalPositionFor(position);
                if (!orig.source) {
                    position.bias = SourceMap.SourceMapConsumer.LEAST_UPPER_BOUND;
                    orig = consumer.originalPositionFor(position);
                }
                if (orig.source) {
                    source.source = this.resolveSource({
                        functionName: source.functionName,
                        fileName: consumer['resolvePath'](orig.source),
                        lineNumber: orig.line,
                        columnNumber: orig.column + 1
                    });
                }
            }
        }
        return source;
    };
    Reporter.prototype.getRealSource = function (source, relative) {
        if (source) {
            if (source.source) {
                return this.getRealSource(source.source, relative);
            }
            if (relative && source.fileName) {
                source.fileName = path.relative(this.basePath, source.fileName);
            }
        }
        return source;
    };
    Reporter.prototype.getFilePath = function (fileName) {
        if (typeof fileName === 'string') {
            var filePath = url.parse(fileName).pathname;
            if (filePath.indexOf(this.urlBase) === 0) {
                return path.join(this.basePath, filePath.substring(this.urlBase.length));
            }
            else if (filePath.indexOf(this.urlAbsoluteBase) === 0) {
                return filePath.substring(this.urlAbsoluteBase.length);
            }
        }
        return fileName;
    };
    Reporter.prototype.parseStack = function (stack, relative) {
        var _this = this;
        var reporter = this;
        try {
            return errorStackParser.parse({ stack: stack }).map(function (frame) { return getSource(frame); }).map(function (frame) { return _this.resolveSource(frame); }).map(function (frame) { return _this.getRealSource(frame, relative); });
        }
        catch (e) {
            this.logger.debug(e);
            return;
        }
        function getSource(frame) {
            return {
                functionName: frame.functionName,
                fileName: reporter.getFilePath(frame.fileName),
                lineNumber: frame.lineNumber,
                columnNumber: frame.columnNumber
            };
        }
    };
    Reporter.prototype.normalizeStack = function (stack) {
        var relative = false;
        var basePath = this.basePath;
        var stackFrames = this.parseStack(stack, relative);
        if (stackFrames) {
            return stackFrames.map(function (frame) { return formatFrame(frame); });
        }
        else {
            return stack.split(/\r\n|\n/g);
        }
        function formatFrame(frame) {
            var result = '    at ';
            result += frame.functionName || '<anonymous>';
            result += ' in ';
            result += frame.fileName;
            if (typeof frame.lineNumber === 'number' && frame.lineNumber >= 0) {
                result += ':line ' + frame.lineNumber.toString(10);
            }
            return result;
        }
    };
    Reporter.prototype.getSpec = function (browser, spec) {
        var existingSpec;
        if (existingSpec = this.specMap[spec.id]) {
            existingSpec.source = existingSpec.source || this.getRealSource(spec.source, false);
        }
        else {
            existingSpec = this.specMap[spec.id] = {
                id: spec.id,
                description: spec.description,
                uniqueName: spec.uniqueName || browser.vsBrowser.getUniqueName(spec),
                suite: spec.suite,
                source: this.getRealSource(spec.source, false),
                results: []
            };
            this.specs.push(existingSpec);
        }
        return existingSpec;
    };
    Reporter.prototype.onRunStart = function (data) {
        this.server.karmaStart();
        this.specMap = {};
        this.specs = [];
        this.sourceMapConsumers = {};
        this.output = [];
    };
    Reporter.prototype.onRunComplete = function (browsers, results) {
        this.server.karmaEnd(this.specs);
        this.logValue("Karma", "Done", this.specs);
    };
    Reporter.prototype.onBrowserStart = function (browser) {
        this.output = [];
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);
    };
    Reporter.prototype.onBrowserError = function (browser, error) {
        var _this = this;
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);
        var failures;
        var source;
        var stackFrames = this.parseStack(error, false);
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
            browser: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(function (failure) { return {
                message: failure.message,
                stack: _this.normalizeStack(failure.stack),
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
        result.source = this.resolveSource(result.source);
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
            browser: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(function (exp) { return {
                message: exp.message,
                stack: _this.normalizeStack(exp.stack),
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