var Util = require('./Util');
var _ = require("lodash");

var VsConfig;
(function (VsConfig) {
    var Test = (function () {
        function Test(config) {
            if (_.isObject(config)) {
                this.name = config.name;
                this.index = config.index;
            }
        }
        return Test;
    })();
    VsConfig.Test = Test;

    var File = (function () {
        function File(config) {
            this.served = true;
            this.included = true;
            this.tests = [];
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
    VsConfig.File = File;

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
        return Config;
    })();
    VsConfig.Config = Config;

    function load(file) {
        return new Config(file ? Util.Try(function () {
            return Util.readJsonFile(Util.resolvePath(file));
        }) || {} : {});
    }
    VsConfig.load = load;
})(VsConfig || (VsConfig = {}));

module.exports = VsConfig;
//# sourceMappingURL=VsConfig.js.map
