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
    interface KarmaResult extends Specs.SpecData {
        event: string;
    }

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
            !/[\/\\]karma-jasmine/.test(entry) &&
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

    function getFailure(failure: any): Specs.Failure {
        var expectation = <Specs.Failure>{
            message: failure.message,
            passed: failure.passed,
            stack: {}
        };

        var messageLines = failure.message.split(/\r\n|\n/);
        var messageLineCount = messageLines.length;

        if (failure.stack) {
            var stack = getRelevantStackFrom(failure.stack).slice(messageLines.length);
            stack.unshift(messageLines[0] || '');
            expectation.stack = { stack: stack.join('\n') };
        } else if (failure.stacktrace) {
            var stack = getRelevantStackFrom(failure.stacktrace).slice(messageLines.length);
            stack.unshift(messageLines[0] || '');
            expectation.stack = { stacktrace: stack.join('\n') };
        }

        return expectation;
    }

    function skipLines(str: string, count: number): string {
        return str.split(/\r\n|\n/).slice(count).join('\n');
    }

    function formatFailure(failure: Specs.Failure): string {
        var result = failure.message;
        if (failure.stack && failure.stack.stack) {
            result += '\n' + skipLines(failure.stack.stack, 1);
        }
        if (failure.stack && failure.stack.stacktrace) {
            result += '\n' + skipLines(failure.stack.stacktrace, 1);
        }
        return result;
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
    class KarmaReporter {
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
                this.karma.result(<KarmaResult>{
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
                this.karma.result(<KarmaResult>{
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

            this.karma.result(<KarmaResult>{
                event: 'spec-start',
                description: spec.description,
                id: spec.id,
                uniqueName: spec.uniqueName
            });
        }


        specDone(spec: SpecResult) {
            spec.endTime = now();
            var failures = map(spec.failedExpectations, exp => getFailure(exp));
            this.karma.result(<KarmaResult>{
                event: 'spec-done',
                description: spec.description,
                id: spec.id,
                log: map(failures, failure => formatFailure(failure)),
                skipped: spec.status === 'disabled' || spec.status === 'pending',
                success: spec.failedExpectations.length === 0,
                suite: this.getSuiteList(spec),
                time: spec.endTime - spec.startTime,
                startTime: spec.startTime,
                endTime: spec.endTime,
                uniqueName: spec.uniqueName,
                source: spec.source,
                sourceStack: spec.sourceStack,
                failures: failures
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
    function createSpecFilter(config, jasmineEnv) {
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
    function createStartFn(karma, jasmineEnv?) {
        // This function will be assigned to `window.__karma__.start`:
        return function () {
            jasmineEnv = jasmineEnv || window.jasmine.getEnv();
            jasmineEnv.execute();
        };
    }

    /**
     * Obtain the Jasmine environment.
     */
    var jasmineEnv = window.jasmine.getEnv();

    /**
     * Add reporter
     */
    jasmineEnv.addReporter(new KarmaReporter(window.__karma__, jasmineEnv));

    createSpecFilter(window.__karma__.config, jasmineEnv);
    window.__karma__.start = createStartFn(window.__karma__, jasmineEnv);
}
