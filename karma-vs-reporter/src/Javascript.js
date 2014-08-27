var esprima = require('esprima');
var sourceMap = require('source-map');
var Util = require("./Util");

var sourceMapRe = /^\s*[#@]\s*sourceMappingURL\s*=\s*(\S.*\.map)\s*$/;

var Javascript;
(function (Javascript) {
    var Program = (function () {
        function Program(options) {
            var _this = this;
            this.options = options;
            if (!options.path && options.content === undefined) {
                throw new Error("path or content must be defined");
            }

            if (options.content === undefined) {
                options.content = Util.readFile(options.path);
            }

            options.baseDir = options.baseDir || process.cwd();

            this.path = options.path;
            this.content = options.content;

            this.ast = Util.Try(function () {
                return esprima.parse(_this.content, { loc: true, range: true, comment: true });
            });
            this.smc = this.getSourceMap();
            this.tokens = this.tokenize();
        }
        Program.prototype.getOriginalPosition = function (position) {
            if (this.smc) {
                var pos = this.smc.originalPositionFor(position);
                if (pos.source && typeof pos.line === 'number' && typeof pos.column === 'number') {
                    pos.source = Util.resolvePath(pos.source, this.path);
                    return pos;
                }
            }
        };

        Program.prototype.lexeme = function (range) {
            return this.content.substring(range[0], range[1]);
        };

        Program.prototype.getSourceMapFromComment = function (comment) {
            var _this = this;
            return Util.ifMatch(sourceMapRe, comment, function (match) {
                return Util.readJsonFile(match[1], _this.path);
            });
        };

        Program.prototype.getSourceMap = function () {
            var _this = this;
            if (this.ast && this.ast.comments) {
                var mappings = this.ast.comments.filter(function (c) {
                    return c.type === 'Line';
                }).map(function (c) {
                    return _this.getSourceMapFromComment(c.value);
                }).filter(function (m) {
                    return m;
                }).map(function (m) {
                    return new sourceMap.SourceMapConsumer(m);
                });
                return mappings[0];
            }
        };

        Program.prototype.tokenize = function () {
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
            return tokens.map(function (t) {
                return t;
            });
        };
        return Program;
    })();
    Javascript.Program = Program;
})(Javascript || (Javascript = {}));

module.exports = Javascript;
//# sourceMappingURL=Javascript.js.map
