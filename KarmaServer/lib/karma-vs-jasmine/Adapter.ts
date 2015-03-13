type Karma = {
    start: (karma: Karma, jasmineEnv?: any) => void;
    config: any;
};

interface Window {
    __karma__?: Karma;
    __coverage__?: any;
    jasmine: any;
}

module KarmaTestAdapter {
    var now = (function () {
        if ("performance" in window && "now" in window.performance) {
            return () => window.performance.now();
        } else if ("now" in Date) {
            return () => Date.now();
        } else {
            return () => new Date().getTime();
        }
    })();

    function map<T, U>(items: T[], cb: (item: T) => U): U[] {
        var result: U[] = [];
        var length = items.length;
        for (var i = 0; i < length; i += 1) {
            result.push(cb(items[i]));
        }
        return result;
    }

    /**
     * Decision maker for whether a stack entry is considered relevant.
     * @param  {String}  entry Error stack entry.
     * @return {Boolean}       True if relevant, False otherwise.
     */
    function isRelevantStackEntry(entry: string): boolean {
        // discard empty and falsy entries:
        return (entry ? true : false) &&
            // discard entries related to jasmine:
            !/[\/\\](jasmine-core)[\/\\]/.test(entry) &&
            // discard entries related to the karma test adapter:
            !/[\/\\]karma-vs/.test(entry) &&
            // discard karma specifics, e.g. "at http://localhost:7018/karma.js:185"
            !/[\/\\](karma.js|context.html):/.test(entry);
    }

    /**
     * Returns relevant stack entries.
     * @param  {String} stack Complete error stack trace.
     * @return {Array}        A list of relevant stack entries.
     */
    function getRelevantStackFrom(stack: string): string[] {
        var relevantStack = [];

        var entries = stack.split(/\r\n|\n/);

        for (var i = 0; i < entries.length; i += 1) {
            var entry = entries[i];
            if (isRelevantStackEntry(entry)) {
                relevantStack.push(entry);
            }
        }

        return relevantStack;
    }

    /**
     * Custom formatter for a failed step.
     *
     * Different browsers report stack trace in different ways. This function
     * attempts to provide a concise, relevant error message by removing the
     * unnecessary stack traces coming from the testing framework itself as well
     * as possible repetition.
     *
     * @see    https://github.com/karma-runner/karma-jasmine/issues/60
     * @param  {Object} step Step object with stack and message properties.
     * @return {String}      Formatted step.
     */
    function formatFailedStep(step) {
        // Safari seems to have no stack trace,
        // so we just return the error message:
        if (!step.stack) { return step.message; }

        var relevantMessage = [];
        var relevantStack = [];
        var dirtyRelevantStack = getRelevantStackFrom(step.stack);

        // PhantomJS returns multiline error message for errors coming from specs
        // (for example when calling a non-existing function). This error is present
        // in both `step.message` and `step.stack` at the same time, but stack seems
        // preferable, so we iterate relevant stack, compare it to message:
        for (var i = 0; i < dirtyRelevantStack.length; i += 1) {
            if (step.message && step.message.indexOf(dirtyRelevantStack[i]) === -1) {
                // Stack entry is not in the message,
                // we consider it to be a relevant stack:
                relevantStack.push(dirtyRelevantStack[i]);
            } else {
                // Stack entry is already in the message,
                // we consider it to be a suitable message alternative:
                relevantMessage.push(dirtyRelevantStack[i]);
            }
        }

        // In most cases the above will leave us with an empty message...
        if (relevantMessage.length === 0) {
            // Let's reuse the original message:
            relevantMessage.push(step.message);

            // Now we probably have a repetition case where:
            // relevantMessage: ["Expected true to be false."]
            // relevantStack:   ["Error: Expected true to be false.", ...]
            if (relevantStack[0].indexOf(step.message) !== -1) {
                // The message seems preferable, so we remove the first value from
                // the stack to get rid of repetition :
                relevantStack.shift();
            }
        }

        // Example output:
        // --------------------
        // Chrome 40.0.2214 (Mac OS X 10.9.5) xxx should return false 1 FAILED
        //    Expected true to be false
        //    at /foo/bar/baz.spec.js:22:13
        //    at /foo/bar/baz.js:18:29
        return relevantMessage.concat(relevantStack).join('\n');
    }

    function getExpectation(step) {
        var expectation = <any>{
            message: step.message,
            passed: step.passed
        };

        if (step.stack) {
            var messageLines = step.message.split(/\r\n|\n/);
            var messageLineCount = messageLines.length;
            var stack = getRelevantStackFrom(step.stack).slice(messageLines.length);
            stack.unshift(messageLines[0] || '');
            expectation.stack = stack.join('\n');
        }

        return expectation;
    }

    /**
     * @param suite
     * @returns {boolean} Return true if it is system jasmine top level suite
     */
    function isTopLevelSuite(suite) {
        return suite.description === 'Jasmine_TopLevel_Suite';
    }

    interface SuiteInfo {
        totalSpecsDefined?: number;
    }

    interface Result {
        root?: boolean;
        id?: string;
        isSuite?: boolean;
        suite?: SuiteResult;
        source?: any;
        sourceStack?: any;
        uniqueName?: string;
        fullName?: string;
        description?: string;
        status?: string;
        startTime?: number;
        endTime?: number;
    }

    interface SuiteResult extends Result {
    }

    interface SpecResult extends Result {
        failedExpectations?: any[];
    }

    /**
     * Very simple reporter for Jasmine.
     */
    export class KarmaReporter {
        constructor(private karma, private jasmineEnv) {
        }

        private currentSuite: SuiteResult;
        private _uniqueNames: { [uniqueNames: string]: boolean } = {};

        private getUniqueName(result: Result) {
            var uniqueName = '';
            var name = result.description.replace(/\./g, '-');
            var seperator = result.isSuite ? ' / ' : '.';

            if (result.suite.root) {
                uniqueName = result.description;
            } else {
                uniqueName = result.suite.uniqueName + seperator + result.description;
            }
            if (this._uniqueNames[uniqueName]) {
                var no = 2;
                while (this._uniqueNames[uniqueName + '-' + no]) {
                    no += 1;
                }
                uniqueName = uniqueName + '-' + no;
            }
            this._uniqueNames[uniqueName] = true;
            return uniqueName;
        }

        private getSuiteList(result: Result): string[] {
            if (result.root || result.suite.root) {
                return [];
            }

            var suites = this.getSuiteList(result.suite);
            suites.push(result.suite.description);
            return suites;
        }

        /**
         * Jasmine 2.0 dispatches the following events:
         *
         *  - jasmineStarted
         *  - jasmineDone
         *  - suiteStarted
         *  - suiteDone
         *  - specStarted
         *  - specDone
         */

        jasmineStarted(suiteInfo: SuiteInfo) {
            // TODO(vojta): Do not send spec names when polling.
            this._uniqueNames = {};
            this.currentSuite = {
                root: true,
                isSuite: true
            };
            this.karma.info({
                total: suiteInfo.totalSpecsDefined,
                //specs: getAllSpecNames(this.jasmineEnv.topSuite())
            });
        }


        jasmineDone() {
            this.karma.complete({
                coverage: window.__coverage__
            });
        }


        suiteStarted(suite: SuiteResult) {
            if (!isTopLevelSuite(suite)) {
                suite.isSuite = true;
                suite.suite = this.currentSuite;
                suite.uniqueName = this.getUniqueName(suite);
                suite.startTime = now();
                this.currentSuite = suite;
                this.karma.result({
                    event: 'suite-start',
                    description: suite.description,
                    id: suite.id,
                    startTime: suite.startTime,
                    uniqueName: suite.uniqueName
                });
            }
        }


        suiteDone(suite: SuiteResult) {
            if (!isTopLevelSuite(suite)) {
                suite.endTime = now();

                // In the case of xdescribe, only "suiteDone" is fired.
                // We need to skip that.
                if (this.currentSuite === suite) {
                    this.currentSuite = suite.suite;
                }
                this.karma.result({
                    event: 'suite-done',
                    description: suite.description,
                    id: suite.id,
                    time: suite.endTime - suite.startTime,
                    startTime: suite.startTime,
                    endTime: suite.endTime,
                    uniqueName: suite.uniqueName,
                    source: suite.source
                });
            }
        }


        specStarted(spec: SpecResult) {
            spec.suite = this.currentSuite;
            spec.uniqueName = this.getUniqueName(spec);
            spec.startTime = now();

            this.karma.result({
                event: 'spec-start',
                description: spec.description,
                id: spec.id,
                uniqueName: spec.uniqueName
            });
        }


        specDone(spec: SpecResult) {
            spec.endTime = now();
            var skipped = spec.status === 'disabled' || spec.status === 'pending';
            var success = spec.failedExpectations.length === 0;
            var log = [];
            if (!success) {
                var steps = spec.failedExpectations;
                for (var i = 0, l = steps.length; i < l; i++) {
                    log.push(formatFailedStep(steps[i]));
                }
            }
            this.karma.result({
                event: 'spec-done',
                description: spec.description,
                id: spec.id,
                log: log,
                skipped: skipped,
                success: success,
                suite: this.getSuiteList(spec),
                time: spec.endTime - spec.startTime,
                startTime: spec.startTime,
                endTime: spec.endTime,
                uniqueName: spec.uniqueName,
                source: spec.source,
                sourceStack: spec.sourceStack,
                failures: map(spec.failedExpectations, exp => getExpectation(exp))
            });
        }
    }

    /**
     * Extract grep option from karma config
     * @param {[Array|string]} clientArguments The karma client arguments
     * @return {string} The value of grep option by default empty string
     */
    var getGrepOption = function (clientArguments) {
        var clientArgString = clientArguments || '';

        if (Object.prototype.toString.call(clientArguments) === '[object Array]') {
            clientArgString = clientArguments.join('=');
        }

        var match = /--grep=(.*)/.exec(clientArgString);
        return match ? match[1] : '';
    };

    /**
     * Create jasmine spec filter
     * @param {Object} options Spec filter options
     */
    var KarmaSpecFilter = function (options) {
        var filterString = options && options.filterString() && options.filterString().replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&');
        var filterPattern = new RegExp(filterString);

        this.matches = function (specName) {
            return filterPattern.test(specName);
        };
    };

    /**
     * @param {Object} config The karma config
     * @param {Object} jasmineEnv jasmine environment object
     */
    export function createSpecFilter(config, jasmineEnv) {
        var specFilter = new KarmaSpecFilter({
            filterString: function () {
                return getGrepOption(config.args);
            }
        });

        jasmineEnv.specFilter = function (spec) {
            return specFilter.matches(spec.getFullName());
        };
    }

    /**
     * Karma starter function factory.
     *
     * This function is invoked from the wrapper.
     * @see  adapter.wrapper
     *
     * @param  {Object}   karma        Karma runner instance.
     * @param  {Object}   [jasmineEnv] Optional Jasmine environment for testing.
     * @return {Function}              Karma starter function.
     */
    export function createStartFn(karma, jasmineEnv?) {
        // This function will be assigned to `window.__karma__.start`:
        return function () {
            jasmineEnv = jasmineEnv || window.jasmine.getEnv();

            //jasmineEnv.addReporter(new KarmaReporter(karma, jasmineEnv));
            jasmineEnv.execute();
        };
    }
}
