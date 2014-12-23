import Globals = require('./Globals');
import Javascript = require('./Javascript');
import Test = require('./Test');
import _ = require('lodash');
import fs = require('fs');
import path = require('path');
var extend = require('extend');
var stripBom = require('strip-bom');

export interface Config {
    karmaConfigFile: string;
    config: any;
}

export function readConfigFile(configFile): Config {
    var config: Config = extend({
        karmaConfigFile: 'karma.conf.js',
        LogToFile: false,
        LogDirectory: '',
        OutputDirectory: '',
        config: {}
    }, Try(() => readJsonFile(configFile)));
    return config;
}

export function writeConfigFile(configFile) {
    writeFile(configFile, JSON.stringify(config, null, 4));
}

export function resolvePath(filepath: string, relativeTo?: string) {
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
    if (_.isUndefined(baseDir)) {
        throw new Error("baseDir undefined");
        return filepath.replace(/\\/g, '/');
    }
    return path.resolve(baseDir, filepath).replace(/\\/g, '/');
}

export function absolutePath(filepath: string, relativeTo?: string) {
    return path.resolve(baseDir, resolvePath(filepath, relativeTo));
}

export function readFile(filepath: string, relativeTo?: string) {
    return stripBom(fs.readFileSync(absolutePath(filepath, relativeTo), 'utf8'));
}

export function readJsonFile(filepath: string, relativeTo?: string) {
    return JSON.parse(readFile(filepath, relativeTo));
}

export function writeFile(filepath: string, data: any, relativeTo?: string) {
    return fs.writeFileSync(absolutePath(filepath, relativeTo), data, { encoding: 'utf8' });
}

export function ifTruthy<V, T>(value: V, action: (value: V) => T) {
    return value ? action(value) : undefined;
}

export function ifMatch<T>(re: RegExp, str: string, onMatch: (match: RegExpExecArray) => T) {
    return ifTruthy<RegExpExecArray, T>(re.exec(str), m => onMatch(m));
}

export function Try<T>(action: () => T, defaultResult?: T) {
    try {
        return action();
    } catch (e) {
        return defaultResult;
    }
}

export function createLogger(logger?) {
    logger = logger || require("karma/lib/logger");
    return logger.create('vs');
}

export function getKarmaConfig(config: Config, extensions?) {
    var karmaConfigFile = path.resolve(config.karmaConfigFile);

    var extend = require('extend');
    var logger = require("karma/lib/logger");
    var cfg = require('karma/lib/config');

    logger.setup('INFO', false);

    var karmaConfig: any = {
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
    baseDir = karmaConfig.basePath;
    return karmaConfig;
}

export function setupLogger() {
    require("karma/lib/logger").setup('INFO', false);
}

export var configFile = path.resolve('karma-vs-reporter.json');
export var baseDir = process.cwd();
export var outputFile = 'karma-vs-reporter.xml';
export var configOutputFile = 'karma-vs-reporter.config.json';
export var config = readConfigFile(configFile);

