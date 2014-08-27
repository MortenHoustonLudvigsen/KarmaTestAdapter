import Util = require('./Util');
import Javascript = require('./Javascript');
import Test = require('./Test');
import JasmineParser = require('./JasmineParser');

class TestResults {
    constructor(public outputFile: string) {
    }
}

export = TestResults;