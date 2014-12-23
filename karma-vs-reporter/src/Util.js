var Globals = require('./Globals');

var _ = require('lodash');
var fs = require('fs');
var path = require('path');
var extend = require('extend');
var stripBom = require('strip-bom');

function readConfigFile(configFile) {
    var config = extend({
        karmaConfigFile: 'karma.conf.js',
        LogToFile: false,
        LogDirectory: '',
        OutputDirectory: '',
        config: {}
    }, exports.Try(function () {
        return exports.readJsonFile(configFile);
    }));
    return config;
}
exports.readConfigFile = readConfigFile;

function writeConfigFile(configFile) {
    exports.writeFile(configFile, JSON.stringify(exports.config, null, 4));
}
exports.writeConfigFile = writeConfigFile;

function resolvePath(filepath, relativeTo) {
    if (_.isUndefined(filepath)) {
        return '';
    }
    if (!_.isUndefined(relativeTo)) {
        relativeTo = exports.resolvePath(relativeTo);
        if (!fs.lstatSync(relativeTo).isDirectory()) {
            relativeTo = path.dirname(relativeTo);
        }
        filepath = path.resolve(relativeTo, filepath);
    }
    if (_.isUndefined(exports.baseDir)) {
        throw new Error("baseDir undefined");
        return filepath.replace(/\\/g, '/');
    }
    return path.resolve(exports.baseDir, filepath).replace(/\\/g, '/');
}
exports.resolvePath = resolvePath;

function absolutePath(filepath, relativeTo) {
    return path.resolve(exports.baseDir, exports.resolvePath(filepath, relativeTo));
}
exports.absolutePath = absolutePath;

function readFile(filepath, relativeTo) {
    return stripBom(fs.readFileSync(exports.absolutePath(filepath, relativeTo), 'utf8'));
}
exports.readFile = readFile;

function readJsonFile(filepath, relativeTo) {
    return JSON.parse(exports.readFile(filepath, relativeTo));
}
exports.readJsonFile = readJsonFile;

function writeFile(filepath, data, relativeTo) {
    return fs.writeFileSync(exports.absolutePath(filepath, relativeTo), data, { encoding: 'utf8' });
}
exports.writeFile = writeFile;

function ifTruthy(value, action) {
    return value ? action(value) : undefined;
}
exports.ifTruthy = ifTruthy;

function ifMatch(re, str, onMatch) {
    return exports.ifTruthy(re.exec(str), function (m) {
        return onMatch(m);
    });
}
exports.ifMatch = ifMatch;

function Try(action, defaultResult) {
    try  {
        return action();
    } catch (e) {
        return defaultResult;
    }
}
exports.Try = Try;

function createLogger(logger) {
    logger = logger || require("karma/lib/logger");
    return logger.create('vs');
}
exports.createLogger = createLogger;

function getKarmaConfig(config, extensions) {
    var karmaConfigFile = path.resolve(config.karmaConfigFile);

    var extend = require('extend');
    var logger = require("karma/lib/logger");
    var cfg = require('karma/lib/config');

    logger.setup('INFO', false);

    var karmaConfig = {
        configFile: karmaConfigFile,
        singleRun: true,
        browsers: [],
        reporters: [],
        colors: false,
        logLevel: 'INFO'
    };

    if (_.isObject(config.config)) {
        karmaConfig = extend(karmaConfig, config.config);
    }

    Globals.origConfig = cfg.parseConfig(karmaConfigFile, karmaConfig);
    karmaConfig = extend({}, Globals.origConfig, extensions);
    exports.baseDir = karmaConfig.basePath;
    return karmaConfig;
}
exports.getKarmaConfig = getKarmaConfig;

function setupLogger() {
    require("karma/lib/logger").setup('INFO', false);
}
exports.setupLogger = setupLogger;

exports.configFile = path.resolve('karma-vs-reporter.json');
exports.baseDir = process.cwd();
exports.outputFile = 'karma-vs-reporter.xml';
exports.configOutputFile = 'karma-vs-reporter.config.json';
exports.config = exports.readConfigFile(exports.configFile);
//# sourceMappingURL=Util.js.map
