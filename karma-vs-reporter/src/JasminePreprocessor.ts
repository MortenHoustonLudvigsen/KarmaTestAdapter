import Javascript = require('./Javascript');
import Parser = require('./Parser');
import VsConfig = require('./VsConfig');
import _ = require("lodash");
import util = require("util");

class JasminePreprocessor extends Parser {
    private _file: VsConfig.File;
    private _tokens: esprima.Token[];
    private _tokenMap: { [index: number]: esprima.Token };

    public preprocess(jsFile: Javascript.Program, file: VsConfig.File): string {
        this._file = file;
        this._tokens = jsFile.tokens.map(t => _.clone(t, true));
        this._tokenMap = {};
        this._tokens.forEach(t => this._tokenMap[t.range[0]] = t);
        this.traverse(jsFile);
        return this._tokens.map(t => t.value).join('');
    }

    public enterNode(node: any): void {
        if (this.nodeIsCall(node, 'it', 2)) {
            var index = node.range[0];
            var token = this._tokenMap[index];
            var test = this._file.getTest(index);
            if (token && !test) {
                token.value = 'xit';
            }
        }
    }
}

export = JasminePreprocessor;