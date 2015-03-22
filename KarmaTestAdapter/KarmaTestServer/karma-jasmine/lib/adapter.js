var KarmaTestAdapter;
(function (KarmaTestAdapter) {
    var now = (function () {
        if ("performance" in window && "now" in window.performance) {
            return function () { return window.performance.now(); };
        }
        else if ("now" in Date) {
            return function () { return Date.now(); };
        }
        else {
            return function () { return new Date().getTime(); };
        }
    })();
    function map(items, cb) {
        var result = [];
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
    function isRelevantStackEntry(entry) {
        // discard empty and falsy entries:
        return (entry ? true : false) && !/[\/\\](jasmine-core)[\/\\]/.test(entry) && !/[\/\\]karma-jasmine/.test(entry) && !/[\/\\](karma.js|context.html):/.test(entry);
    }
    /**
     * Returns relevant stack entries.
     * @param  {String} stack Complete error stack trace.
     * @return {Array}        A list of relevant stack entries.
     */
    function getRelevantStackFrom(stack) {
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
    function getFailure(failure) {
        var expectation = {
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
        }
        else if (failure.stacktrace) {
            var stack = getRelevantStackFrom(failure.stacktrace).slice(messageLines.length);
            stack.unshift(messageLines[0] || '');
            expectation.stack = { stacktrace: stack.join('\n') };
        }
        return expectation;
    }
    function skipLines(str, count) {
        return str.split(/\r\n|\n/).slice(count).join('\n');
    }
    function formatFailure(failure) {
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
    /**
     * Very simple reporter for Jasmine.
     */
    var KarmaReporter = (function () {
        function KarmaReporter(karma, jasmineEnv) {
            this.karma = karma;
            this.jasmineEnv = jasmineEnv;
        }
        KarmaReporter.prototype.getSuiteList = function (result) {
            if (result.root || result.suite.root) {
                return [];
            }
            var suites = this.getSuiteList(result.suite);
            suites.push(result.suite.description);
            return suites;
        };
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
        KarmaReporter.prototype.jasmineStarted = function (suiteInfo) {
            // TODO(vojta): Do not send spec names when polling.
            this.currentSuite = {
                root: true,
                isSuite: true
            };
            this.karma.info({
                total: suiteInfo.totalSpecsDefined,
            });
        };
        KarmaReporter.prototype.jasmineDone = function () {
            this.karma.complete({
                coverage: window.__coverage__
            });
        };
        KarmaReporter.prototype.suiteStarted = function (suite) {
            if (!isTopLevelSuite(suite)) {
                suite.isSuite = true;
                suite.suite = this.currentSuite;
                suite.startTime = now();
                this.currentSuite = suite;
                this.karma.result({
                    event: 'suite-start',
                    description: suite.description,
                    id: suite.id,
                    startTime: suite.startTime
                });
            }
        };
        KarmaReporter.prototype.suiteDone = function (suite) {
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
                    source: suite.source
                });
            }
        };
        KarmaReporter.prototype.specStarted = function (spec) {
            spec.suite = this.currentSuite;
            spec.startTime = now();
            this.karma.result({
                event: 'spec-start',
                description: spec.description,
                id: spec.id
            });
        };
        KarmaReporter.prototype.specDone = function (spec) {
            spec.endTime = now();
            var failures = map(spec.failedExpectations, function (exp) { return getFailure(exp); });
            this.karma.result({
                event: 'spec-done',
                description: spec.description,
                id: spec.id,
                log: map(failures, function (failure) { return formatFailure(failure); }),
                skipped: spec.status === 'disabled' || spec.status === 'pending',
                success: spec.failedExpectations.length === 0,
                suite: this.getSuiteList(spec),
                time: spec.endTime - spec.startTime,
                startTime: spec.startTime,
                endTime: spec.endTime,
                source: spec.source,
                sourceStack: spec.sourceStack,
                failures: failures
            });
        };
        return KarmaReporter;
    })();
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
    function createStartFn(karma, jasmineEnv) {
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
})(KarmaTestAdapter || (KarmaTestAdapter = {}));
//# sourceMappingURL=adapter.js.map