import Util = require('./Util');
import _ = require("lodash");
import util = require('util');

module VsConfig {
    export class Test {
        public name: string;
        public index: number;

        constructor(config: any) {
            if (_.isObject(config)) {
                this.name = config.name;
                this.index = config.index;
            }
        }
    }

    export class File {
        public path: string;
        public served: boolean = true;
        public included: boolean = true;
        public tests: Test[] = [];

        constructor(config: any) {
            if (_.isObject(config)) {
                this.path = config.path;
                this.served = typeof config.served === 'boolean' ? config.served : this.served;
                this.included = typeof config.included === 'boolean' ? config.included : this.included;

                if (_.isArray(config.tests)) {
                    this.tests = config.tests.map(t => new Test(t));
                }
            }
        }

        private _testMap: { [index: number]: Test };

        public hasTests() {
            return this.tests.length > 0;
        }

        public getTest(index: number): Test {
            if (!this._testMap) {
                this._testMap = {};
                this.tests.forEach(t => this._testMap[t.index] = t);
            }
            return this._testMap[index];
        }
    }

    export class Config {
        public files: File[] = [];
        private _fileMap: { [path: string]: File };

        constructor(config) {
            if (_.isObject(config) && _.isArray(config.files)) {
                this.files = config.files.map(f => new File(f));
            }
        }

        public hasFiles() {
            return this.files.length > 0;
        }

        public getFile(path: string): File {
            if (!this._fileMap) {
                this._fileMap = {};
                this.files.forEach(f => this._fileMap[Util.resolvePath(f.path).toLowerCase()] = f);
            }
            return this._fileMap[Util.resolvePath(path).toLowerCase()];
        }
    }

    export function load(file: string): Config {
        return new Config(file ? Util.Try(() => Util.readJsonFile(Util.resolvePath(file))) || {} : {});
    }
}

export = VsConfig;