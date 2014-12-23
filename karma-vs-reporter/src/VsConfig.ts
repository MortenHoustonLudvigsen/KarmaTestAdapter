import Globals = require('./Globals');
import Util = require('./Util');
import _ = require("lodash");
import util = require('util');
import path = require('path');
var extend = require('extend');
var helper = require('karma/lib/helper');

function basePathResolve(basePath: string, relativePath: string): string {
    if (helper.isUrlAbsolute(relativePath)) {
        return relativePath;
    }

    if (!helper.isDefined(basePath) || !helper.isDefined(relativePath)) {
        return '';
    }
    return path.resolve(basePath, relativePath);
}

export class Test {
    public name: string;
    public index: number;
    public include: boolean;

    constructor(config: any) {
        if (_.isObject(config)) {
            this.name = config.name;
            this.index = config.index;
            this.include = config.include;
        }
    }
}

export class File {
    public path: string;
    public served: boolean = true;
    public included: boolean = true;
    public tests: Test[] = [];
    public finished: boolean = false;

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

    public getFiles(basePath: string): any[] {
        return this.files.map(f => <any>{
            "pattern": basePathResolve(basePath, f.path),
            "served": f.served,
            "included": f.included,
            "watched": false
        });
    }
}

export function load(file: string): Config {
    return new Config(file ? Util.Try(() => Util.readJsonFile(Util.resolvePath(file))) || {} : {});
}

export function loadFiles(files: { served?: { path: string }[]; included?: { path: string }[] }): File[]{
    var result: { [path: string]: File } = {};
    if (_.isArray(files.served)) {
        files.served.map(f => new File({ path: f.path, served: true, included: false })).forEach(f => {
            result[f.path] = f;
        });
    }
    if (_.isArray(files.included)) {
        files.included.forEach(f => {
            var file = result[f.path] || new File({ path: f.path, served: false, included: true });
            file.included = true;
            result[f.path] = file;
        });
    }
    return _.values(result);
}