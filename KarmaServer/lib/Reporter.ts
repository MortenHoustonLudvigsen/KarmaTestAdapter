import fs = require('fs');
import util = require('util');
import url = require('url');
import path = require('path');
import Specs = require('./Specs');
import Server = require('./Server');
import Karma = require('./Karma');

var errorStackParser = require('error-stack-parser');

import SourceMap = require("source-map");
var SourceMapResolve = require("source-map-resolve");

function pad(value: string | number, count: number): string {
    if (typeof value === 'number') {
        return pad(value.toString(10), count);
    }
    if (typeof value === 'string') {
        return (value + Array(count + 1).join(' ')).slice(0, count - 1);
    }
    return pad('', count);
}

interface ValueLogger {
    (name: string, status: string, value?: any): void;
    enabled?: boolean;
}

type KarmaConfig = {
    basePath: string;
    urlRoot: string;
};

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
}

interface Failure {
    message: string;
    stack: string;
    passed: boolean;
}

class Reporter {
    static name = 'karma-vs-reporter';
    static $inject = ['config', 'karma-vs-server', 'logger'];

    constructor(private config: KarmaConfig, private server: Server, logger: Karma.LoggerModule) {
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

    logValue: ValueLogger = (name: string, status: string, value?: any) => {
        if (this.logValue.enabled) {
            var message = pad(name, 20) + " " + status;
            if (typeof value !== 'undefined') {
                message += '\n' + util.inspect(value, { depth: null });
            }
            this.logger.debug(message);
        }
    }

    private logger: Karma.Logger;
    private urlRoot = this.config.urlRoot || '/';
    private urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
    private urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
    private basePath = this.config.basePath;
    private specMap: { [id: string]: Specs.Spec } = {};
    private specs: Specs.Spec[] = [];

    private output: string[] = [];

    private sourceMapConsumers: { [filePath: string]: SourceMap.SourceMapConsumer } = {};
    private getSourceMapConsumer(filePath: string): SourceMap.SourceMapConsumer {
        if (filePath in this.sourceMapConsumers) {
            return this.sourceMapConsumers[filePath];
        }
        var content = fs.readFileSync(filePath).toString();
        var sourceMap = SourceMapResolve.resolveSync(content, filePath, fs.readFileSync);
        var consumer = sourceMap ? new SourceMap.SourceMapConsumer(sourceMap.map) : null;
        if (consumer) {
            consumer['resolvePath'] = (filePath: string) => path.resolve(path.dirname(sourceMap.sourcesRelativeTo), filePath);
        }
        return consumer;
    }

    resolveSource(source: Specs.Source): Specs.Source {
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
    }

    getRealSource(source: Specs.Source, relative: boolean): Specs.Source {
        if (source) {
            if (source.source) {
                return this.getRealSource(source.source, relative);
            }
            if (relative && source.fileName) {
                source.fileName = path.relative(this.basePath, source.fileName);
            }
        }
        return source;
    }

    getFilePath(fileName: string): string {
        if (typeof fileName === 'string') {
            var filePath = url.parse(fileName).pathname;
            if (filePath.indexOf(this.urlBase) === 0) {
                return path.join(this.basePath, filePath.substring(this.urlBase.length));
            } else if (filePath.indexOf(this.urlAbsoluteBase) === 0) {
                return filePath.substring(this.urlAbsoluteBase.length);
            }
        }
        return fileName;
    }

    parseStack(stack: string, relative: boolean): Specs.Source[]{
        var reporter = this;

        try {
            return errorStackParser.parse({ stack: stack })
                .map(frame => getSource(frame))
                .map(frame => this.resolveSource(frame))
                .map(frame => this.getRealSource(frame, relative));
        } catch (e) {
            this.logger.debug(e);
            return;
        }

        function getSource(frame: any): Specs.Source {
            return {
                functionName: frame.functionName,
                fileName: reporter.getFilePath(frame.fileName),
                lineNumber: frame.lineNumber,
                columnNumber: frame.columnNumber
            };
        }
    }

    normalizeStack(stack: string): string[] {
        var relative = false;
        var basePath = this.basePath;

        var stackFrames = this.parseStack(stack, relative);
        if (stackFrames) {
            return stackFrames.map(frame => formatFrame(frame));
        } else {
            return stack.split(/\r\n|\n/g);
        }

        function formatFrame(frame: Specs.Source): string {
            var result = '    at ';
            result += frame.functionName || '<anonymous>';
            result += ' in ';
            result += frame.fileName;
            if (typeof frame.lineNumber === 'number' && frame.lineNumber >= 0) {
                result += ':line ' + frame.lineNumber.toString(10);
            }
            return result;
        }
    }

    getSpec(browser: Browser, spec: Event): Specs.Spec {
        var existingSpec: Specs.Spec;
        if (existingSpec = this.specMap[spec.id]) {
            existingSpec.source = existingSpec.source || this.getRealSource(spec.source, false);
        } else {
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
    }

    onRunStart(data): void {
        this.server.karmaStart();
        this.specMap = {};
        this.specs = [];
        this.sourceMapConsumers = {};
        this.output = [];
    }

    onRunComplete(browsers, results): void {
        this.server.karmaEnd(this.specs);
        this.logValue("Karma", "Done", this.specs);
    }

    onBrowserStart(browser: Browser): void {
        this.output = [];
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);
    }

    onBrowserError(browser: Browser, error: string): void {
        browser.vsBrowser = browser.vsBrowser || new VsBrowser(browser);

        var failures: Failure[];
        var source: Specs.Source;
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
            browser: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(failure => <Specs.Expectation>{
                message: failure.message,
                stack: this.normalizeStack(failure.stack),
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
            browser: browser.name,
            success: event.success,
            skipped: event.skipped,
            output: this.output.join('\n'),
            time: event.time,
            startTime: event.startTime,
            endTime: event.endTime,
            log: event.log,
            failures: event.failures ? event.failures.map(exp => <Specs.Expectation>{
                message: exp.message,
                stack: this.normalizeStack(exp.stack),
                passed: exp.passed
            }) : undefined
        };
        browser.vsBrowser.addResult(spec, result);
        spec.results.push(result);

        this.output = [];
    }
}

export = Reporter;

