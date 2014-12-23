import Util = require('./Util');
import Globals = require('./Globals');
import VsConfig = require('./VsConfig');
import Javascript = require('./Javascript');
import JasminePreprocessor = require('./JasminePreprocessor');
import util = require('util');

var Preprocessor: any = function (config, logger, helper) {
    Util.baseDir = config.basePath;
    var log = Util.createLogger(logger);
    var jasminePreprocessor = new JasminePreprocessor();

    return function (content: string, file, done: (content: string) => void) {
        var testFile = Globals.vsConfig.getFile(file.path);
        if (testFile && testFile.hasTests()) {
            var jsFile = new Javascript.Program({ path: file.path, content: content });
            content = jasminePreprocessor.preprocess(jsFile, testFile);
        }
        done(content);
    };
};

Preprocessor.$inject = ['config', 'logger', 'helper'];

export = Preprocessor;
