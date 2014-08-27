import Test = require('./Test');
import Javascript = require('./Javascript');
import JasmineParser = require('./JasmineParser');

function parseFiles(karma: Test.Karma, files, log) {
    var jasmineParser = new JasmineParser();
    files.served.forEach(function (file) {
        try {
            var testFile = karma.files().getFile(file.path);
            testFile.served = true;
            var jsFile = new Javascript.Program({ path: file.path, content: file.content });
            jasmineParser.parse(jsFile, testFile);
        } catch (e) {
            log.error(e);
        }
    });
    files.included.forEach(function (file) {
        try {
            karma.files().getFile(file.path).included = true;
        } catch (e) {
            log.error(e);
        }
    });
}

export = parseFiles;