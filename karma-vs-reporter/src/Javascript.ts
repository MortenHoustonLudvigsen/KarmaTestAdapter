import path = require('path');
import fs = require('fs');
import esprima = require('esprima');
import sourceMap = require('source-map');
import Util = require("./Util");

var sourceMapRe = /^\s*[#@]\s*sourceMappingURL\s*=\s*(\S.*\.map)\s*$/;

module Javascript {
    export interface Position {
        line: number;
        column: number;
        index?: number;
    }

    export interface MappedPosition extends Position {
        source: string;
        name?: string;
    }

    export interface Token {
        type: string;
        value: string;
        range?: Array<number>;
        loc?: { start: Position; end: Position };
        originalLoc?: MappedPosition;
    }

    export class Program {
        public path: string;
        public content: string;
        public ast: esprima.Syntax.Program;
        public tokens: Array<esprima.Token>;
        private smc: sourceMap.SourceMapConsumer;

        constructor(private options: { path?: string; content?: string; baseDir?: string }) {
            if (!options.path && options.content === undefined) {
                throw new Error("path or content must be defined");
            }

            if (options.content === undefined) {
                options.content = Util.readFile(options.path);
            }

            options.baseDir = options.baseDir || process.cwd();

            this.path = options.path;
            this.content = options.content;

            this.ast = Util.Try(() => esprima.parse(this.content, { loc: true, range: true, comment: true }));
            this.smc = this.getSourceMap();
            this.tokens = this.tokenize();
        }

        public getOriginalPosition(position: Position): MappedPosition {
            if (this.smc) {
                var pos = this.smc.originalPositionFor(position);
                if (pos.source && typeof pos.line === 'number' && typeof pos.column === 'number') {
                    pos.source = Util.resolvePath(pos.source, this.path);
                    return pos;
                }
            }
        }

        public lexeme(range: Array<number>): string {
            return this.content.substring(range[0], range[1]);
        }

        private getSourceMapFromComment(comment): any {
            return Util.ifMatch(sourceMapRe, comment, match => Util.readJsonFile(match[1], this.path));
        }

        private getSourceMap(): sourceMap.SourceMapConsumer {
            if (this.ast && this.ast.comments) {
                var mappings = this.ast.comments
                    .filter(c => c.type === 'Line')
                    .map(c => this.getSourceMapFromComment(c.value))
                    .filter(m => m)
                    .map(m => new sourceMap.SourceMapConsumer(m));
                return mappings[0];
            }
        }

        private tokenize(): Array<Token> {
            if (!this.ast) {
                return [];
            }

            // Get all significant tokens
            var tokens = esprima.tokenize(this.content, {
                //loc: true,
                range: true,
                comment: true
            });

            if (tokens === null) {
                return [];
            }

            tokens.comments = tokens.comments || [];

            // Add comments
            tokens.comments.forEach(function (token) {
                if (token.type === 'Line') {
                    token.type = 'Comment';
                    token.value = '//' + token.value;
                } else if (token.type === 'Block') {
                    token.type = 'Comment';
                    token.value = '/*' + token.value + '*/';
                }
                tokens.push(token);
            });

            // Sort by index - original order
            tokens.sort(function (token1, token2) {
                return token1.range[0] - token2.range[0];
            });

            // Fill in "holes" whith white space from original content
            var lastIndex = 0;
            var spaces = [];
            tokens.forEach(function (token) {
                if (token.range[0] > lastIndex) {
                    spaces.push({
                        type: 'Space',
                        value: this.lexeme([lastIndex, token.range[0]]),
                        range: [lastIndex, token.range[0]]
                    });
                }
                lastIndex = token.range[1];
            }, this);
            spaces.push({
                type: 'Space',
                value: this.lexeme([lastIndex, this.content.length]),
                range: [lastIndex, this.content.length]
            });
            Array.prototype.push.apply(tokens, spaces);

            // Sort by index - original order
            tokens.sort(function (token1, token2) {
                return token1.range[0] - token2.range[0];
            });

            //return this.getOriginalTokenPositions(tokens);
            return tokens.map(t => t);
        }
    }
}

export = Javascript;
