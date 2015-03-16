var KarmaTestAdapter;
(function (KarmaTestAdapter) {
    function wrapFunctions(env) {
        ['describe', 'xdescribe', 'fdescribe', 'it', 'xit', 'fit'].forEach(function (functionName) {
            var oldFunction = env[functionName];
            if (typeof oldFunction !== 'function' || oldFunction.__source_wrapped) {
                return;
            }
            var wrapped = {};
            wrapped[functionName] = function (description, done) {
                var item = oldFunction.apply(this, Array.prototype.slice.call(arguments));
                try {
                    throw new Error();
                }
                catch (error) {
                    item.result.sourceStack = {
                        skip: 2,
                        skipFunctions: "^(jasmineInterface|Env)\.",
                        stack: error.stack,
                        stacktrace: error.stacktrace,
                        'opera#sourceloc': error['opera#sourceloc']
                    };
                }
                return item;
            };
            wrapped[functionName].__source_wrapped = true;
            env[functionName] = wrapped[functionName];
        });
    }
    KarmaTestAdapter.wrapFunctions = wrapFunctions;
})(KarmaTestAdapter || (KarmaTestAdapter = {}));
//# sourceMappingURL=instumentation.js.map