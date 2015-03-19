import util = require('util');
import path = require('path');
import url = require('url');
import Specs = require('../TestServer/Specs');
import SourceUtils = require('../TestServer/SourceUtils');
import Server = require('./Server');
import Karma = require('./Karma');

class VsBrowser {
    constructor(public browser: Browser) {
    }

    results: Specs.SpecResult[] = [];
    uniqueNames: { [name: string]: boolean } = {};
    totalTime: number = 0;
    startTime: number;
    endTime: number;
    timesValid = true;

    addResult(spec: Specs.Spec, result: Specs.SpecResult): void {
        this.results.push(result);

        if (this.timesValid && typeof result.startTime === 'number' && typeof result.endTime === 'number') {
            this.totalTime += result.time;

            this.startTime = typeof this.startTime === 'number'
                ? Math.min(this.startTime, result.startTime) : result.startTime;

            this.endTime = typeof this.endTime === 'number'
                ? Math.max(this.endTime, result.endTime) : result.endTime;
        } else {
            this.timesValid = false;
        }
    }

    getUniqueName(suite: string[], description: string): string;
    getUniqueName(event: Event): string;
    getUniqueName(eventOrSuite: Event | string[], description?: string): string {
        if (eventOrSuite instanceof Array) {
            var suite = <string[]>eventOrSuite;
            var uniqueName = suite.map(name => {
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
        } else {
            var event = <Event>eventOrSuite;
            return this.getUniqueName(event.suite, event.description);
        }
    }

    adjustResults() {
        this.adjustTimes();
    }

    adjustTimes() {
        if (this.timesValid && typeof this.startTime === 'number' && typeof this.endTime === 'number') {
            var diff = ((this.endTime - this.startTime) - this.totalTime) / this.results.length;
            this.results.forEach(result => result.time = Math.max(0.01, result.time + diff));
        }
    }
}

interface Browser {
    name: string;
    vsBrowser: VsBrowser
}

interface Event {
    event: string;
    description: string;
    id: string;
    log: string[];
    skipped: boolean;
    success: boolean;
    suite: string[];
    time: number;
    startTime: number;
    endTime: number;
    uniqueName: string;
    source: Specs.Source;
    failures: Failure[];
    sourceStack?: any;
}

interface Failure {
    message: string;
    stack: string;
    passed: boolean;
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

    private logger: Karma.Logger;
    private specMap: { [id: string]: Specs.Spec } = {};
    private specs: Specs.Spec[] = [];
    private sourceUtils: SourceUtils;

    private output: string[] = [];

    getSpec(browser: Browser, spec: Event): Specs.Spec {
        var existingSpec: Specs.Spec;
        if (existingSpec = this.specMap[spec.id]) {
            existingSpec.source = existingSpec.source || this.sourceUtils.getSource(spec.sourceStack);
        } else {
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
    }

    onRunStart(data): void {
        this.server.testRunStarted();
        this.specMap = {};
        this.specs = [];
        this.output = [];
        this.sourceUtils = new SourceUtils(this.basePath, this.logger, fileName => {
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
    }

    onRunComplete(browsers, results): void {
        this.server.testRunCompleted(this.specs);
    }

    onBrowserStart(browser: Browser): void {
        this.output = [];
        browser.vsBrowser = new VsBrowser(browser);
    }

    onBrowserError(browser: Browser, error: string): void {
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);

        var failures: Failure[];
        var source: Specs.Source;
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

        var event = <Event>{
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

        var result: Specs.SpecResult = {
            name: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(failure => <Specs.Expectation>{
                message: failure.message,
                stack: this.sourceUtils.normalizeStack(failure.stack),
                passed: failure.passed
            }) : undefined
        };
        browser.vsBrowser.addResult(spec, result);
        spec.results.push(result);

        this.output = [];
    }

    onBrowserLog(browser: Browser, message: string, type): void {
        if (typeof message === 'string') {
            message = message.replace(/^'(.*)'$/, '$1');
        }
        this.output.push(message);
    }

    onBrowserComplete(browser: Browser): void {
        this.output = [];
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);
        browser.vsBrowser.adjustResults();
    }

    onSpecComplete(browser: Browser, result: Event): void {
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
    }

    onSuiteStart(browser: Browser, event: Event) {
        this.output = [];
    }

    onSuiteDone(browser: Browser, event: Event) {
        this.output = [];
    }

    onSpecStart(browser: Browser, event: Event) {
        this.output = [];
    }

    onSpecDone(browser: Browser, event: Event) {
        var spec = this.getSpec(browser, event);
        var result: Specs.SpecResult = {
            name: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(exp => <Specs.Expectation>{
                message: exp.message,
                stack: this.sourceUtils.normalizeStack(exp.stack),
                passed: exp.passed
            }) : undefined
        };
        browser.vsBrowser.addResult(spec, result);
        spec.results.push(result);

        this.output = [];
    }
}

export = Reporter;

