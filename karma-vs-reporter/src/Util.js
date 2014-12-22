var _ = require('lodash');
var fs = require('fs');
var path = require('path');
var extend = require('extend');
var stripBom = require('strip-bom');

var Util;
(function (Util) {
    Util.configFile = path.resolve('karma-vs-reporter.json');
    Util.baseDir = process.cwd();
    Util.outputFile = 'karma-vs-reporter.xml';
    Util.configOutputFile = 'karma-vs-reporter.config.json';
    Util.config = readConfigFile(Util.configFile);

    function readConfigFile(configFile) {
        var config = extend({
            karmaConfigFile: 'karma.conf.js',
            LogToFile: false,
            LogDirectory: '',
            OutputDirectory: '',
            config: {}
        }, Try(function () {
            return readJsonFile(configFile);
        }));
        return config;
    }
    Util.readConfigFile = readConfigFile;

    function writeConfigFile(configFile) {
        writeFile(configFile, JSON.stringify(Util.config, null, 4));
    }
    Util.writeConfigFile = writeConfigFile;

    function resolvePath(filepath, relativeTo) {
        if (_.isUndefined(filepath)) {
            return '';
        }
        if (!_.isUndefined(relativeTo)) {
            relativeTo = resolvePath(relativeTo);
            if (!fs.lstatSync(relativeTo).isDirectory()) {
                relativeTo = path.dirname(relativeTo);
            }
            filepath = path.resolve(relativeTo, filepath);
        }
        if (_.isUndefined(Util.baseDir)) {
            throw new Error("baseDir undefined");
            return filepath;
        }
        return path.relative(Util.baseDir, filepath).replace(/\\/g, '/').toLowerCase();
    }
    Util.resolvePath = resolvePath;

    function absolutePath(filepath, relativeTo) {
        return path.resolve(Util.baseDir, resolvePath(filepath, relativeTo));
    }
    Util.absolutePath = absolutePath;

    function readFile(filepath, relativeTo) {
        return stripBom(fs.readFileSync(absolutePath(filepath, relativeTo), 'utf8'));
    }
    Util.readFile = readFile;

    function readJsonFile(filepath, relativeTo) {
        return JSON.parse(readFile(filepath, relativeTo));
    }
    Util.readJsonFile = readJsonFile;

    function writeFile(filepath, data, relativeTo) {
        return fs.writeFileSync(absolutePath(filepath, relativeTo), data, { encoding: 'utf8' });
    }
    Util.writeFile = writeFile;

    function ifTruthy(value, action) {
        return value ? action(value) : undefined;
    }
    Util.ifTruthy = ifTruthy;

    function ifMatch(re, str, onMatch) {
        return ifTruthy(re.exec(str), function (m) {
            return onMatch(m);
        });
    }
    Util.ifMatch = ifMatch;

    function Try(action, defaultResult) {
        try  {
            return action();
        } catch (e) {
            return defaultResult;
        }
    }
    Util.Try = Try;

    function createLogger(logger) {
        return logger.create('karma-vs-reporter');
    }
    Util.createLogger = createLogger;
})(Util || (Util = {}));

module.exports = Util;
//# sourceMappingURL=Util.js.map
