var TestContext = require('./TestContext');
var SourceUtils = require('./SourceUtils');
var TestReporter = (function () {
    function TestReporter(server, basePath, logger, resolveFilePath) {
        this.server = server;
        this.basePath = basePath;
        this.logger = logger;
        this.resolveFilePath = resolveFilePath;
        this.specMap = {};
        this.specs = [];
        this.output = [];
    }
    TestReporter.prototype.getSpec = function (context, spec) {
        var existingSpec;
        if (existingSpec = this.specMap[spec.id]) {
            existingSpec.source = existingSpec.source || this.sourceUtils.getSource(spec.source);
        }
        else {
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
    };
    TestReporter.prototype.onTestRunStart = function () {
        this.server.testRunStarted();
        this.specMap = {};
        this.specs = [];
        this.output = [];
        this.sourceUtils = new SourceUtils(this.basePath, this.logger, this.resolveFilePath);
    };
    TestReporter.prototype.onTestRunComplete = function () {
        this.server.testRunCompleted(this.specs);
    };
    TestReporter.prototype.onContextStart = function (context) {
        this.output = [];
        context.testContext = new TestContext(this.server);
    };
    TestReporter.prototype.onContextDone = function (context) {
        this.output = [];
        context.testContext = context.testContext || new TestContext(this.server);
        context.testContext.adjustResults();
    };
    TestReporter.prototype.onError = function (context, error) {
        context.testContext = context.testContext || new TestContext(this.server);
        if (error) {
            var failure = { passed: false };
            var source;
            var log;
            if (typeof error === 'string') {
                log = [error];
                if (this.sourceUtils.parseStack({ stack: error }, false)) {
                    source = { stack: error };
                    failure.message = error.split(/(\r\n|\n|\r)/g)[0];
                    failure.stack = source;
                }
                else if (this.sourceUtils.parseStack({ stacktrace: error }, false)) {
                    source = { stacktrace: error };
                    failure.message = error.split(/(\r\n|\n|\r)/g)[0];
                    failure.stack = source;
                }
                else {
                    failure.message = error;
                }
            }
            else if (typeof error === 'object') {
                failure.message = error.message || 'Uncaught error';
                failure.stack = error;
            }
            else {
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
    };
    TestReporter.prototype.onOutput = function (context, message) {
        this.output.push(message);
    };
    TestReporter.prototype.onSuiteStart = function (context) {
        this.output = [];
    };
    TestReporter.prototype.onSuiteDone = function (context) {
        this.output = [];
    };
    TestReporter.prototype.onSpecStart = function (context, specData) {
        this.output = [];
    };
    TestReporter.prototype.onSpecDone = function (context, specData) {
        var _this = this;
        var spec = this.getSpec(context, specData);
        var result = {
            name: context.name,
            success: specData.success,
            skipped: specData.skipped,
            output: this.output.join('\n'),
            time: specData.time,
            startTime: specData.startTime,
            endTime: specData.endTime,
            log: specData.log,
            failures: specData.failures ? specData.failures.map(function (exp) { return {
                message: exp.message,
                stack: _this.sourceUtils.normalizeStack(exp.stack),
                passed: exp.passed
            }; }) : undefined
        };
        context.testContext.addResult(spec, result);
        spec.results.push(result);
        this.output = [];
    };
    return TestReporter;
})();
module.exports = TestReporter;
//# sourceMappingURL=TestReporter.js.map