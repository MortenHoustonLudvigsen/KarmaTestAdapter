var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Parser = require('./Parser');

var _ = require("lodash");

var JasminePreprocessor = (function (_super) {
    __extends(JasminePreprocessor, _super);
    function JasminePreprocessor() {
        _super.apply(this, arguments);
    }
    JasminePreprocessor.prototype.preprocess = function (jsFile, file) {
        var _this = this;
        this._file = file;
        this._tokens = jsFile.tokens.map(function (t) {
            return _.clone(t, true);
        });
        this._tokenMap = {};
        this._tokens.forEach(function (t) {
            return _this._tokenMap[t.range[0]] = t;
        });
        this.traverse(jsFile);
        return this._tokens.map(function (t) {
            return t.value;
        }).join('');
    };

    JasminePreprocessor.prototype.enterNode = function (node) {
        if (this.nodeIsCall(node, 'it', 2)) {
            var index = node.range[0];
            var token = this._tokenMap[index];
            var test = this._file.getTest(index);
            if (token && test && !test.include) {
                token.value = 'xit';
            }
        }
    };
    return JasminePreprocessor;
})(Parser);

module.exports = JasminePreprocessor;
//# sourceMappingURL=JasminePreprocessor.js.map
