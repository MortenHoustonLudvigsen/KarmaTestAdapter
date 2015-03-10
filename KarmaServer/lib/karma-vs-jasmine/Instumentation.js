var JasmineInstumentation;
(function (JasmineInstumentation) {
    //var ErrorStackParser = require('error-stack-parser');
    var skipMethodsRe = /^[fx]?(?:describe|it)$/;
    var skipFunctionsRe = /^(?:jasmineInterface|Env)\./;
    function getStackTrace(error) {
        try {
            return ErrorStackParser.parse(error);
        }
        catch (e) {
            return [];
        }
    }
    function getSource(error) {
        var stackframes = getStackTrace(error);
        for (var i = 2 /* Skip the two first stack frames */; i < stackframes.length; i++) {
            var frame = stackframes[i];
            if (!skipFunctionsRe.test(frame.functionName)) {
                return {
                    functionName: frame.functionName,
                    fileName: frame.fileName,
                    lineNumber: frame.lineNumber,
                    columnNumber: frame.columnNumber
                };
            }
        }
        return;
    }
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
                    item.result.source = getSource(error);
                }
                //item.result.sourcePromise = StackTrace.get().then(stackframes => getSource(stackframes));
                return item;
            };
            wrapped[functionName].__source_wrapped = true;
            env[functionName] = wrapped[functionName];
        });
    }
    JasmineInstumentation.wrapFunctions = wrapFunctions;
    if (typeof global === 'object') {
        global.JasmineInstumentation = JasmineInstumentation;
    }
})(JasmineInstumentation || (JasmineInstumentation = {}));
//# sourceMappingURL=Instumentation.js.map