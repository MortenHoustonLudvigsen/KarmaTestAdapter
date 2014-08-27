import Javascript = require('./Javascript');
import Test = require('./Test');
import TestFileParser = require('./TestFileParser');

var framework = 'jasmine';

class JasmineParser extends TestFileParser {
    constructor() {
        super();
    }

    public enterNode(node: any): void {
        if (this.nodeIsCall(node, 'describe', 2)) {
            this.StartSuite(node.arguments[0].value, framework);
            this.addCallback(node, function () { this.EndSuite(); }, this);
        } else if (this.nodeIsCall(node, 'it', 2)) {
            this.RegisterTest(node.arguments[0].value, framework);
        }
    }
}

export = JasmineParser;