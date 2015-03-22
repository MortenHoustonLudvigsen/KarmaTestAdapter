import Specs = require('./Specs');
import TestContext = require('./TestContext');
import SourceUtils = require('./SourceUtils');
import Logger = require('./Logger');

class TestReporter {
    constructor(private server: Specs.Server, private basePath: string, private logger: Logger, private resolveFilePath: (fileName: string) => string) {
    }

    private specMap: { [id: string]: Specs.Spec } = {};
    private specs: Specs.Spec[] = [];
    private output: string[] = [];
    private sourceUtils: SourceUtils;

    getSpec(context: Specs.Context, spec: Specs.SpecData): Specs.Spec {
        var existingSpec: Specs.Spec;
        if (existingSpec = this.specMap[spec.id]) {
            existingSpec.source = existingSpec.source || this.sourceUtils.getSource(spec.source);
        } else {
            existingSpec = this.specMap[spec.id] = {
                id: spec.id,
                description: spec.description,
                suite: spec.suite,
                source: this.sourceUtils.getSource(spec.source),
                results: []
            };
            existingSpec.fullyQualifiedName = context.testContext.getFullyQualifiedName(existingSpec);
            existingSpec.displayName = context.testContext.getDisplayName(existingSpec);
            existingSpec.traits = context.testContext.getTraits(existingSpec);
            this.specs.push(existingSpec);
        }
        return existingSpec;
    }

    onTestRunStart(): void {
        this.server.testRunStarted();
        this.specMap = {};
        this.specs = [];
        this.output = [];
        this.sourceUtils = new SourceUtils(this.basePath, this.logger, this.resolveFilePath);
    }

    onTestRunComplete(): void {
        this.server.testRunCompleted(this.specs);
    }

    onContextStart(context: Specs.Context): void {
        this.output = [];
        context.testContext = new TestContext(this.server);
    }

    onContextDone(context: Specs.Context): void {
        this.output = [];
        context.testContext = context.testContext || new TestContext(this.server);
        context.testContext.adjustResults();
    }

    onError(context: Specs.Context, error: any): void {
        context.testContext = context.testContext || new TestContext(this.server);

        if (error) {
            var failure: Specs.Failure = { passed: false };
            var source: Specs.StackInfo;
            var log: string[];

            if (typeof error === 'string') {
                log = [error];
                if (this.sourceUtils.parseStack({ stack: error }, false)) {
                    source = { stack: error };
                    failure.message = error.split(/(\r\n|\n|\r)/g)[0];
                    failure.stack = source;
                } else if (this.sourceUtils.parseStack({ stacktrace: error }, false)) {
                    source = { stacktrace: error };
                    failure.message = error.split(/(\r\n|\n|\r)/g)[0];
                    failure.stack = source;
                } else {
                    failure.message = error;
                }
            } else if (typeof error === 'object') {
                failure.message = error.message || 'Uncaught error';
                failure.stack = error;
            } else {
                failure.message = 'Uncaught error';
            }

            var id = context.testContext.getNewErrorId();

            this.onSpecDone(context, {
                id: id,
                description: 'Uncaught error',
                log: log,
                skipped: false,
                success: false,
                suite: [],
                time: 0,
                startTime: undefined,
                endTime: undefined,
                source: source,
                failures: [failure]
            });
        }
        this.output = [];
    }

    onOutput(context: Specs.Context, message: string): void {
        this.output.push(message);
    }

    onSuiteStart(context: Specs.Context) {
        this.output = [];
    }

    onSuiteDone(context: Specs.Context) {
        this.output = [];
    }

    onSpecStart(context: Specs.Context, specData: Specs.SpecData) {
        this.output = [];
    }

    onSpecDone(context: Specs.Context, specData: Specs.SpecData) {
        var spec = this.getSpec(context, specData);
        var result: Specs.SpecResult = {
            name: context.name,
            success: specData.success,
            skipped: specData.skipped,
            output: this.output.join('\n'),
            time: specData.time,
            startTime: specData.startTime,
            endTime: specData.endTime,
            log: specData.log,
            failures: specData.failures ? specData.failures.map(exp => <Specs.Expectation>{
                message: exp.message,
                stack: this.sourceUtils.normalizeStack(exp.stack),
                passed: exp.passed
            }) : undefined
        };
        context.testContext.addResult(spec, result);
        spec.results.push(result);
        this.output = [];
    }
}

export = TestReporter;