var path = require('path');
var url = require('url');
var TestReporter = require('../TestServer/TestReporter');
var Karma = require('./Karma');
var Reporter = (function () {
    function Reporter(config, server, logger) {
        this.config = config;
        this.server = server;
        this.urlRoot = this.config.urlRoot || '/';
        this.urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
        this.urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
        this.basePath = this.config.basePath;
        this.logger = logger.create('VS Reporter', Karma.karma.Constants.LOG_DEBUG);
        this.logger.info("Created");
    }
    Reporter.prototype.onRunStart = function (data) {
        var _this = this;
        this.testReporter = new TestReporter(this.server, this.basePath, this.logger, function (fileName) {
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
        this.testReporter.onTestRunStart();
    };
    Reporter.prototype.onRunComplete = function (browsers, results) {
        this.testReporter.onTestRunComplete();
    };
    Reporter.prototype.onBrowserStart = function (browser) {
        this.testReporter.onContextStart(browser);
    };
    Reporter.prototype.onBrowserError = function (browser, error) {
        this.testReporter.onError(browser, error);
    };
    Reporter.prototype.onBrowserLog = function (browser, message, type) {
        if (typeof message === 'string') {
            this.testReporter.onOutput(browser, message.replace(/^'(.*)'$/, '$1'));
        }
    };
    Reporter.prototype.onBrowserComplete = function (browser) {
        this.testReporter.onContextDone(browser);
    };
    Reporter.prototype.onSpecComplete = function (browser, result) {
        switch (result.event) {
            case 'suite-start':
                this.testReporter.onSuiteStart(browser);
                break;
            case 'suite-done':
                this.testReporter.onSuiteDone(browser);
                break;
            case 'spec-start':
                this.testReporter.onSpecStart(browser, result);
                break;
            case 'spec-done':
            default:
                this.testReporter.onSpecDone(browser, result);
                break;
        }
    };
    Reporter.name = 'karma-vs-reporter';
    Reporter.$inject = ['config', 'karma-vs-server', 'logger'];
    return Reporter;
})();
module.exports = Reporter;
//# sourceMappingURL=Reporter.js.map