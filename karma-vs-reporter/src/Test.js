var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Util = require("./Util");
var _ = require("lodash");

var xmlbuilder = require('xmlbuilder');

var Test;
(function (_Test) {
    //function lazy<T>(init: () => T): T {
    //    var fn = function () {
    //        var
    //    }
    //}
    (function (Outcome) {
        Outcome[Outcome["Success"] = 0] = "Success";
        Outcome[Outcome["Skipped"] = 1] = "Skipped";
        Outcome[Outcome["Failed"] = 2] = "Failed";
    })(_Test.Outcome || (_Test.Outcome = {}));
    var Outcome = _Test.Outcome;

    var Item = (function () {
        function Item() {
            this.children = [];
        }
        Item.prototype.add = function (item) {
            this.children.push(item);
            item.parent = this;
            return item;
        };

        Item.prototype.toXml = function (parentElement) {
        };
        return Item;
    })();
    _Test.Item = Item;

    var Karma = (function (_super) {
        __extends(Karma, _super);
        function Karma(start) {
            _super.call(this);
            this.start = start;
            this.end = start;
        }
        Karma.prototype.toXml = function (parentElement) {
            var element = xmlbuilder.create('Karma', { version: '1.0', encoding: 'UTF-8', standalone: true }, { pubID: null, sysID: null }, {
                allowSurrogateChars: false, skipNullAttributes: true,
                headless: false, ignoreDecorators: false, stringify: {}
            });

            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element.end({ pretty: true });
        };

        Karma.prototype.files = function () {
            var files = this.add(new Files());
            this.files = function () {
                return files;
            };
            return this.files();
        };

        Karma.prototype.results = function () {
            var results = this.add(new Results(this));
            this.results = function () {
                return results;
            };
            return this.results();
        };
        return Karma;
    })(Item);
    _Test.Karma = Karma;

    var KarmaConfig = (function (_super) {
        __extends(KarmaConfig, _super);
        function KarmaConfig(config) {
            _super.call(this);
            this.config = config;
        }
        KarmaConfig.prototype.toXml = function (parentElement) {
            return this.valueToXml(parentElement, 'Config', this.config);
        };

        KarmaConfig.prototype.valueToXml = function (parentElement, name, value) {
            if (value === null || value === undefined) {
                return undefined;
            } else if (_.isString(value) || _.isBoolean(value) || _.isDate(value) || _.isNumber(value)) {
                return this.scalarToXml(parentElement, name, value);
            } else if (_.isArray(value)) {
                return this.arrayToXml(parentElement, name, value);
            } else if (_.isObject(value)) {
                return this.objectToXml(parentElement, name, value);
            }
        };

        KarmaConfig.prototype.objectToXml = function (parentElement, name, object) {
            var _this = this;
            var element = this.createElement(parentElement, name);
            _.forIn(object, function (value, property) {
                _this.valueToXml(element, property, value);
            });
            return element;
        };

        KarmaConfig.prototype.scalarToXml = function (parentElement, name, value) {
            return this.createElement(parentElement, name, value);
        };

        KarmaConfig.prototype.arrayToXml = function (parentElement, name, value) {
            var _this = this;
            var element = this.createElement(parentElement, name);
            _.forEach(value, function (item) {
                _this.valueToXml(element, 'item', item);
            });
            return element;
        };

        KarmaConfig.prototype.createElement = function (parentElement, name, value) {
            if (/\W/.test(name)) {
                return parentElement.ele('item', { name: name }, value);
            } else {
                return parentElement.ele(name, value);
            }
        };
        return KarmaConfig;
    })(Item);
    _Test.KarmaConfig = KarmaConfig;

    var Container = (function (_super) {
        __extends(Container, _super);
        function Container(name) {
            _super.call(this);
            this.name = name;
            this.attributes = {};
        }
        Container.prototype.toXml = function (parentElement) {
            if (this.children.length > 0) {
                var element = parentElement.ele(this.name, this.attributes);
                this.children.forEach(function (child) {
                    child.toXml(element);
                });
                return element;
            }
        };
        return Container;
    })(Item);
    _Test.Container = Container;

    var Results = (function (_super) {
        __extends(Results, _super);
        function Results(karma) {
            _super.call(this, 'Results');
            this.karma = karma;
        }
        Results.prototype.toXml = function (parentElement) {
            this.attributes['start'] = this.karma.start.toISOString();
            this.attributes['end'] = this.karma.end.toISOString();
            return _super.prototype.toXml.call(this, parentElement);
        };
        return Results;
    })(Container);
    _Test.Results = Results;

    var Files = (function (_super) {
        __extends(Files, _super);
        function Files() {
            _super.call(this, 'Files');
            this._files = {};
        }
        Files.prototype.add = function (file) {
            file = _super.prototype.add.call(this, file);
            this._files[Util.resolvePath(file.path)] = file;
            return file;
        };

        Files.prototype.getFile = function (path) {
            return this._files[Util.resolvePath(path)] || this.add(new File(path));
        };
        return Files;
    })(Container);
    _Test.Files = Files;

    var File = (function (_super) {
        __extends(File, _super);
        function File(path) {
            _super.call(this);
            this.path = path;
            this.served = false;
            this.included = false;
            this.path = Util.resolvePath(this.path);
        }
        File.prototype.toXml = function (parentElement) {
            var element = parentElement.ele('File', {
                Path: this.path,
                Served: this.served,
                Included: this.included
            });
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        };
        return File;
    })(Item);
    _Test.File = File;

    var Suite = (function (_super) {
        __extends(Suite, _super);
        function Suite(name) {
            _super.call(this);
            this.name = name;
        }
        Suite.prototype.toXml = function (parentElement) {
            var attributes = {
                Name: this.name,
                Framework: this.framework
            };

            if (this.position) {
                attributes.Line = this.position.line;
                attributes.Column = this.position.column;
            }

            var element = parentElement.ele('Suite', attributes);
            SourceToXml(element, this.originalPosition);
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        };
        return Suite;
    })(Item);
    _Test.Suite = Suite;

    var Test = (function (_super) {
        __extends(Test, _super);
        function Test(name) {
            _super.call(this);
            this.name = name;
        }
        Test.prototype.toXml = function (parentElement) {
            var attributes = {
                Name: this.name,
                Framework: this.framework
            };

            if (this.position) {
                attributes.Line = this.position.line;
                attributes.Column = this.position.column;
                if (_.isNumber(this.position.index)) {
                    attributes.Index = this.position.index;
                }
            }

            if (_.isNumber(this.index)) {
                attributes.index = this.index;
            }

            var element = parentElement.ele('Test', attributes);
            SourceToXml(element, this.originalPosition);
            return element;
        };
        return Test;
    })(Item);
    _Test.Test = Test;

    var ResultContainer = (function (_super) {
        __extends(ResultContainer, _super);
        function ResultContainer() {
            _super.call(this);
        }
        ResultContainer.prototype.parentResultContainer = function () {
            return this.parent;
        };

        ResultContainer.prototype.isSuite = function (suite) {
            return false;
        };

        ResultContainer.prototype.getSuites = function () {
            return [];
        };
        return ResultContainer;
    })(Item);
    _Test.ResultContainer = ResultContainer;

    var Browser = (function (_super) {
        __extends(Browser, _super);
        function Browser(name) {
            _super.call(this);
            this.name = name;
            this._currentSuite = this;
        }
        Browser.prototype.startSuite = function (suite) {
            var currentSuites = this._currentSuite.getSuites();
            var currentSuite = this;
            var match = true;

            suite.forEach(function (s) {
                if (match) {
                    var c = currentSuites.shift();
                    if (c && s === c.name) {
                        currentSuite = c;
                    } else {
                        match = false;
                        currentSuite = currentSuite.add(new SuiteResult(s));
                    }
                } else {
                    currentSuite = currentSuite.add(new SuiteResult(s));
                }
            });

            this._currentSuite = currentSuite;
            return currentSuite;
        };

        Browser.prototype.toXml = function (parentElement) {
            var attributes = {
                Name: this.name
            };
            var element = parentElement.ele('Browser', attributes);
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        };
        return Browser;
    })(ResultContainer);
    _Test.Browser = Browser;

    var SuiteResult = (function (_super) {
        __extends(SuiteResult, _super);
        function SuiteResult(name) {
            _super.call(this);
            this.name = name;
        }
        SuiteResult.prototype.getSuites = function () {
            var result = this.parentResultContainer().getSuites();
            result.push(this);
            return result;
        };

        SuiteResult.prototype.isSuite = function (suite) {
            return suite === this.name;
        };

        SuiteResult.prototype.toXml = function (parentElement) {
            var attributes = {
                Name: this.name
            };
            var element = parentElement.ele('SuiteResult', attributes);
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        };
        return SuiteResult;
    })(ResultContainer);
    _Test.SuiteResult = SuiteResult;

    var TestResult = (function (_super) {
        __extends(TestResult, _super);
        function TestResult(name) {
            _super.call(this);
            this.name = name;
            this.log = [];
        }
        TestResult.prototype.toXml = function (parentElement) {
            var attributes = {
                Name: this.name,
                Id: this.id,
                Time: this.time,
                Outcome: this.outcome !== undefined ? Outcome[this.outcome] : undefined
            };

            var element = parentElement.ele('TestResult', attributes);
            this.log.forEach(function (line) {
                element.ele('Log', line);
            });
            return element;
        };
        return TestResult;
    })(Item);
    _Test.TestResult = TestResult;

    function SourceToXml(parentElement, source) {
        if (source) {
            return parentElement.ele('Source', {
                Path: source.source,
                Line: source.line,
                Column: source.column
            });
        }
    }
})(Test || (Test = {}));

module.exports = Test;
//# sourceMappingURL=Test.js.map
