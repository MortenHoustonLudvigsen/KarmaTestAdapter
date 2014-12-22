var Util = require('./Util');
var Globals = require('./Globals');

var Javascript = require('./Javascript');
var JasminePreprocessor = require('./JasminePreprocessor');

var Preprocessor = function (config, logger, helper) {
    Util.baseDir = config.basePath;
    var log = Util.createLogger(logger);
    var jasminePreprocessor = new JasminePreprocessor();

    return function (content, file, done) {
        var fileFromConfig = Globals.vsConfig.getFile(file.path);
        if (fileFromConfig && fileFromConfig.hasTests()) {
            var jsFile = new Javascript.Program({ path: file.path, content: content });
            content = jasminePreprocessor.preprocess(jsFile, fileFromConfig);
        }
        done(content);
    };
};

Preprocessor.$inject = ['config', 'logger', 'helper'];

module.exports = Preprocessor;
//# sourceMappingURL=Preprocessor.js.map
