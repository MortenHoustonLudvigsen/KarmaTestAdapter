declare var global: any;

module JasmineInstumentation {
    declare var ErrorStackParser: any;
    //var ErrorStackParser = require('error-stack-parser');

    var skipMethodsRe = /^[fx]?(?:describe|it)$/;
    var skipFunctionsRe = /^(?:jasmineInterface|Env)\./;

    interface Source {
        functionName: string;
        fileName: string;
        lineNumber: number;
        columnNumber: number;
    }

    interface WrappedFunction extends Function {
        __source_wrapped?: boolean;
    }

    function getStackTrace(error): any[] {
        try {
            return ErrorStackParser.parse(error);
        } catch (e) {
            return [];
        }
    }

    function getSource(error): Source {
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

    export function wrapFunctions(env: any): void {
        ['describe', 'xdescribe', 'fdescribe', 'it', 'xit', 'fit'].forEach(functionName => {
            var oldFunction: WrappedFunction = env[functionName];

            if (typeof oldFunction !== 'function' || oldFunction.__source_wrapped) {
                return;
            }

            var wrapped: { [name: string]: WrappedFunction } = {};
            wrapped[functionName] = function (description: string, done: Function): void {
                var item = oldFunction.apply(this, Array.prototype.slice.call(arguments));
                try {
                    // Error must be thrown to get stack in IE
                    throw new Error();
                } catch (error) {
                    item.result.source = getSource(error);
                    //item.result.stacktrace = getStackTrace(error);
                }
            
                //item.result.sourcePromise = StackTrace.get().then(stackframes => getSource(stackframes));
                return item;
            };
            wrapped[functionName].__source_wrapped = true;
            env[functionName] = wrapped[functionName];
        });
    }

    if (typeof global === 'object') {
        global.JasmineInstumentation = JasmineInstumentation;
    }
}
