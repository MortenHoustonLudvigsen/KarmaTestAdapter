module KarmaTestAdapter {
    interface WrappedFunction extends Function {
        __karma_test_adapter_source_wrapped?: boolean;
    }

    export function wrapFunctions(env: any): void {
        ['describe', 'xdescribe', 'fdescribe', 'it', 'xit', 'fit'].forEach(functionName => {
            var oldFunction: WrappedFunction = env[functionName];

            if (typeof oldFunction !== 'function' || oldFunction.__karma_test_adapter_source_wrapped) {
                return;
            }

            var wrapped: { [name: string]: WrappedFunction } = {};
            wrapped[functionName] = function (description: string, done: Function): void {
                var item = oldFunction.apply(this, Array.prototype.slice.call(arguments));
                try {
                    // Error must be thrown to get stack in IE
                    throw new Error();
                } catch (error) {
                    item.result.source = <Specs.StackInfo>{
                        skip: 2,
                        skipFunctions: "^(jasmineInterface|Env)\.",
                        stack: error.stack,
                        stacktrace: error.stacktrace,
                        'opera#sourceloc': error['opera#sourceloc']
                    };
                }
                return item;
            };
            wrapped[functionName].__karma_test_adapter_source_wrapped = true;
            env[functionName] = wrapped[functionName];
        });
    }
}
