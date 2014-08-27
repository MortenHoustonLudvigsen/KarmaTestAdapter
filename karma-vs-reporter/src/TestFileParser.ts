import Javascript = require('./Javascript');
import Parser = require('./Parser');
import Test = require('./Test');

class TestFileParser extends Parser {
    private items: Test.Item;
    private parents: Array<Test.Item>;
    private hasTests = false;

    public parse(jsFile: Javascript.Program, file: Test.File): boolean {
        this.items = file;
        this.parents = [];
        this.hasTests = false;
        this.traverse(jsFile);
        return this.hasTests;
    }

    public StartSuite(name: string, framework: string) {
        var suite = new Test.Suite(name);
        suite.framework = framework;
        suite.position = this.getPosition();
        suite.originalPosition = this.getOriginalPosition();
        this.items.children.push(suite);
        this.parents.push(this.items);
        this.items = suite;
        return suite;
    }

    public EndSuite() {
        if (this.parents.length > 0) {
            this.items = this.parents.pop();
        }
    }

    public RegisterTest(name: string, framework: string) {
        this.hasTests = true;
        var test = new Test.Test(name);
        test.framework = framework;
        test.position = this.getPosition();
        test.originalPosition = this.getOriginalPosition();
        this.items.children.push(test);
        return test;
    }
}

export = TestFileParser;