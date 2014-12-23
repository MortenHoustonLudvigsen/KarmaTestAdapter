var Util = require('./Util');
var _ = require("lodash");

var path = require('path');
var extend = require('extend');
var helper = require('karma/lib/helper');

function basePathResolve(basePath, relativePath) {
    if (helper.isUrlAbsolute(relativePath)) {
        return relativePath;
    }

    if (!helper.isDefined(basePath) || !helper.isDefined(relativePath)) {
        return '';
    }
    return path.resolve(basePath, relativePath);
}

var Test = (function () {
    function Test(config) {
        if (_.isObject(config)) {
            this.name = config.name;
            this.index = config.index;
            this.include = config.include;
        }
    }
    return Test;
})();
exports.Test = Test;

var File = (function () {
    function File(config) {
        this.served = true;
        this.included = true;
        this.tests = [];
        this.finished = false;
        if (_.isObject(config)) {
            this.path = config.path;
            this.served = typeof config.served === 'boolean' ? config.served : this.served;
            this.included = typeof config.included === 'boolean' ? config.included : this.included;

            if (_.isArray(config.tests)) {
                this.tests = config.tests.map(function (t) {
                    return new Test(t);
                });
            }
        }
    }
    File.prototype.hasTests = function () {
        return this.tests.length > 0;
    };

    File.prototype.getTest = function (index) {
        var _this = this;
        if (!this._testMap) {
            this._testMap = {};
            this.tests.forEach(function (t) {
                return _this._testMap[t.index] = t;
            });
        }
        return this._testMap[index];
    };
    return File;
})();
exports.File = File;

var Config = (function () {
    function Config(config) {
        this.files = [];
        if (_.isObject(config) && _.isArray(config.files)) {
            this.files = config.files.map(function (f) {
                return new File(f);
            });
        }
    }
    Config.prototype.hasFiles = function () {
        return this.files.length > 0;
    };

    Config.prototype.getFile = function (path) {
        var _this = this;
        if (!this._fileMap) {
            this._fileMap = {};
            this.files.forEach(function (f) {
                return _this._fileMap[Util.resolvePath(f.path).toLowerCase()] = f;
            });
        }
        return this._fileMap[Util.resolvePath(path).toLowerCase()];
    };

    Config.prototype.getFiles = function (basePath) {
        return this.files.map(function (f) {
            return {
                "pattern": basePathResolve(basePath, f.path),
                "served": f.served,
                "included": f.included,
                "watched": false
            };
        });
    };
    return Config;
})();
exports.Config = Config;

function load(file) {
    return new Config(file ? Util.Try(function () {
        return Util.readJsonFile(Util.resolvePath(file));
    }) || {} : {});
}
exports.load = load;

function loadFiles(files) {
    var result = {};
    if (_.isArray(files.served)) {
        files.served.map(function (f) {
            return new File({ path: f.path, served: true, included: false });
        }).forEach(function (f) {
            result[f.path] = f;
        });
    }
    if (_.isArray(files.included)) {
        files.included.forEach(function (f) {
            var file = result[f.path] || new File({ path: f.path, served: false, included: true });
            file.included = true;
            result[f.path] = file;
        });
    }
    return _.values(result);
}
exports.loadFiles = loadFiles;
//# sourceMappingURL=VsConfig.js.map
