import util = require('util');
import path = require('path');
import url = require('url');
import Specs = require('../TestServer/Specs');
import TestReporter = require('../TestServer/TestReporter');
import Server = require('./Server');
import Karma = require('./Karma');

interface Browser {
    name: string;
}

interface Result extends Specs.SpecData {
    event: string;
}

class Reporter {
    static name = 'karma-vs-reporter';
    static $inject = ['config', 'karma-vs-server', 'logger'];

    constructor(private config: Karma.KarmaConfig, private server: Server, logger: Karma.LoggerModule) {
        this.logger = logger.create('VS Reporter', Karma.karma.Constants.LOG_DEBUG);
        this.logger.info("Created");
    }

    private urlRoot = this.config.urlRoot || '/';
    private urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
    private urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
    private basePath = this.config.basePath;

    private testReporter: TestReporter;
    private logger: Karma.Logger;

    onRunStart(data): void {
        this.testReporter = new TestReporter(this.server, this.basePath, this.logger, fileName => {
            if (typeof fileName === 'string') {
                var filePath = url.parse(fileName).pathname;
                if (filePath.indexOf(this.urlBase) === 0) {
                    return path.join(this.basePath, filePath.substring(this.urlBase.length));
                } else if (filePath.indexOf(this.urlAbsoluteBase) === 0) {
                    return filePath.substring(this.urlAbsoluteBase.length);
                }
            }
            return fileName;
        });
        this.testReporter.onTestRunStart();
    }

    onRunComplete(browsers, results): void {
        this.testReporter.onTestRunComplete();
    }

    onBrowserStart(browser: Browser): void {
        this.testReporter.onContextStart(browser);
    }

    onBrowserError(browser: Browser, error: any): void {
        this.testReporter.onError(browser, error);
    }

    onBrowserLog(browser: Browser, message: string, type): void {
        if (typeof message === 'string') {
            this.testReporter.onOutput(browser, message.replace(/^'(.*)'$/, '$1'));
        }
    }

    onBrowserComplete(browser: Browser): void {
        this.testReporter.onContextDone(browser);
    }

    onSpecComplete(browser: Browser, result: Result): void {
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
    }
}

export = Reporter;

