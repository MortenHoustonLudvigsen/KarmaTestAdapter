var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var TestFileParser = require('./TestFileParser');

var framework = 'jasmine';

var JasmineParser = (function (_super) {
    __extends(JasmineParser, _super);
    function JasmineParser() {
        _super.call(this);
    }
    JasmineParser.prototype.enterNode = function (node) {
        if (this.nodeIsCall(node, 'describe', 2)) {
            this.StartSuite(node.arguments[0].value, framework);
            this.addCallback(node, function () {
                this.EndSuite();
            }, this);
        } else if (this.nodeIsCall(node, 'it', 2)) {
            this.RegisterTest(node.arguments[0].value, framework);
        }
    };
    return JasmineParser;
})(TestFileParser);

module.exports = JasmineParser;
//# sourceMappingURL=JasmineParser.js.map
