import Javascript = require('./Javascript');
import Util = require("./Util");
import _ = require("lodash");
import util = require("util");

var xmlbuilder = require('xmlbuilder');

module Test {
    export enum Outcome {
        Success,
        Skipped,
        Failed
    }

    export class Item {
        public parent: Item;
        public children: Array<Item> = [];

        constructor() {
        }

        public add<T extends Item>(item: T): T {
            this.children.push(item);
            item.parent = this;
            return item;
        }

        public toXml(parentElement): any { }
    }

    export class Karma extends Item {
        public end: Date;

        constructor(public start: Date) {
            super();
            this.end = start;
        }

        public toXml(parentElement?): any {
            var element = xmlbuilder.create('Karma',
                { version: '1.0', encoding: 'UTF-8', standalone: true },
                { pubID: null, sysID: null },
                {
                    allowSurrogateChars: false, skipNullAttributes: true,
                    headless: false, ignoreDecorators: false, stringify: {}
                }
                );

            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element.end({ pretty: true });
        }

        public files(): Files {
            var files = this.add(new Files());
            this.files = function () {
                return files;
            };
            return this.files();
        }

        public results(): Item {
            var results = this.add(new Results(this));
            this.results = function () {
                return results;
            };
            return this.results();
        }
    }

    export class KarmaConfig extends Item {
        constructor(public config: any) {
            super();
        }

        public toXml(parentElement): any {
            return this.valueToXml(parentElement, 'Config', this.config);
        }

        private valueToXml(parentElement, name: string, value) {
            if (value === null || value === undefined) {
                return undefined;
            } else if (_.isString(value) || _.isBoolean(value) || _.isDate(value) || _.isNumber(value)) {
                return this.scalarToXml(parentElement, name, value);
            } else if (_.isArray(value)) {
                return this.arrayToXml(parentElement, name, value);
            } else if (_.isObject(value)) {
                return this.objectToXml(parentElement, name, value);
            }
        }

        private objectToXml(parentElement, name: string, object) {
            var element = this.createElement(parentElement, name);
            _.forIn(object, (value, property) => {
                this.valueToXml(element, property, value);
            });
            return element;
        }

        private scalarToXml(parentElement, name: string, value) {
            return this.createElement(parentElement, name, value);
        }

        private arrayToXml(parentElement, name: string, value) {
            var element = this.createElement(parentElement, name);
            _.forEach(value, item => {
                this.valueToXml(element, 'item', item);
            });
            return element;
        }

        private createElement(parentElement, name: string, value?) {
            if (/\W/.test(name)) {
                return parentElement.ele('item', { name: name }, value);
            } else {
                return parentElement.ele(name, value);
            }
        }
    }

    export class Container extends Item {
        public attributes = {};

        constructor(private name: string) {
            super();
        }

        public toXml(parentElement): any {
            if (this.children.length > 0) {
                var element = parentElement.ele(this.name, this.attributes);
                this.children.forEach(function (child) {
                    child.toXml(element);
                });
                return element;
            }
        }
    }

    export class Results extends Container {
        constructor(private karma: Karma) {
            super('Results');
        }

        public toXml(parentElement): any {
            this.attributes['start'] = this.karma.start.toISOString();
            this.attributes['end'] = this.karma.end.toISOString();
            return super.toXml(parentElement);
        }
    }

    export class Files extends Container {
        constructor() {
            super('Files');
        }

        private _files: { [path: string]: File; } = {};

        public add(file: File): File {
            file = super.add(file);
            this._files[Util.resolvePath(file.path)] = file;
            return file;
        }

        public getFile(path: string): File {
            return this._files[Util.resolvePath(path)] || this.add(new File(path));
        }
    }

    export class File extends Item {
        public served: boolean = false;
        public included: boolean = false;

        constructor(public path: string) {
            super();
            this.path = Util.resolvePath(this.path);
        }

        public toXml(parentElement): any {
            var element = parentElement.ele('File', {
                Path: this.path,
                Served: this.served,
                Included: this.included
            });
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        }
    }

    export class Suite extends Item {
        public framework: string;
        public position: Javascript.Position;
        public originalPosition: Javascript.MappedPosition;

        constructor(public name: string) {
            super();
        }

        public toXml(parentElement): any {
            var attributes: any = {
                Name: this.name,
                Framework: this.framework
            }

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
        }
    }

    export class Test extends Item {
        public framework: string;
        public position: Javascript.Position;
        public originalPosition: Javascript.MappedPosition;
        public index: number;

        constructor(public name: string) {
            super();
        }

        public toXml(parentElement): any {
            var attributes: any = {
                Name: this.name,
                Framework: this.framework
            }

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
        }
    }

    export class ResultContainer extends Item {
        constructor() {
            super();
        }

        public parentResultContainer(): ResultContainer {
            return <ResultContainer>this.parent;
        }

        public isSuite(suite: string): boolean {
            return false;
        }

        public getSuites(): SuiteResult[] {
            return [];
        }
    }

    export class Browser extends ResultContainer {
        private _currentSuite: ResultContainer;
        constructor(public name: string) {
            super();
            this._currentSuite = this;
        }

        public startSuite(suite: string[]): ResultContainer {
            var currentSuites = this._currentSuite.getSuites();
            var currentSuite: ResultContainer = this;
            var match = true;

            suite.forEach(s => {
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
        }

        public toXml(parentElement): any {
            var attributes: any = {
                Name: this.name
            }
            var element = parentElement.ele('Browser', attributes);
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        }
    }

    export class SuiteResult extends ResultContainer {
        constructor(public name: string) {
            super();
        }

        public getSuites(): SuiteResult[] {
            var result = this.parentResultContainer().getSuites();
            result.push(this);
            return result;
        }

        public isSuite(suite: string): boolean {
            return suite === this.name;
        }

        public toXml(parentElement): any {
            var attributes: any = {
                Name: this.name
            }
            var element = parentElement.ele('SuiteResult', attributes);
            this.children.forEach(function (child) {
                child.toXml(element);
            });
            return element;
        }
    }

    export class TestResult extends Item {
        public id: string;
        public time: number;
        public outcome: Outcome;
        public log: Array<string> = [];

        constructor(public name: string) {
            super();
        }

        public toXml(parentElement): any {
            var attributes: any = {
                Name: this.name,
                Id: this.id,
                Time: this.time,
                Outcome: this.outcome !== undefined ? Outcome[this.outcome] : undefined
            }

            var element = parentElement.ele('TestResult', attributes);
            this.log.forEach(function (line) {
                element.ele('Log', line);
            });
            return element;
        }
    }

    function SourceToXml(parentElement, source: Javascript.MappedPosition): any {
        if (source) {
            return parentElement.ele('Source', {
                Path: source.source,
                Line: source.line,
                Column: source.column
            });
        }
    }
}

export = Test;