var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Parser = require('./Parser');
var Test = require('./Test');

var TestFileParser = (function (_super) {
    __extends(TestFileParser, _super);
    function TestFileParser() {
        _super.apply(this, arguments);
        this.hasTests = false;
    }
    TestFileParser.prototype.parse = function (jsFile, file) {
        this.items = file;
        this.parents = [];
        this.hasTests = false;
        this.traverse(jsFile);
        return this.hasTests;
    };

    TestFileParser.prototype.StartSuite = function (name, framework) {
        var suite = new Test.Suite(name);
        suite.framework = framework;
        suite.position = this.getPosition();
        suite.originalPosition = this.getOriginalPosition();
        this.items.children.push(suite);
        this.parents.push(this.items);
        this.items = suite;
        return suite;
    };

    TestFileParser.prototype.EndSuite = function () {
        if (this.parents.length > 0) {
            this.items = this.parents.pop();
        }
    };

    TestFileParser.prototype.RegisterTest = function (name, framework) {
        this.hasTests = true;
        var test = new Test.Test(name);
        test.framework = framework;
        test.position = this.getPosition();
        test.originalPosition = this.getOriginalPosition();
        this.items.children.push(test);
        return test;
    };
    return TestFileParser;
})(Parser);

module.exports = TestFileParser;
//# sourceMappingURL=TestFileParser.js.map
