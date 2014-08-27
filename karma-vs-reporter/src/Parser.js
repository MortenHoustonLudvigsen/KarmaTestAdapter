var traverse = require("ast-traverse");

var Parser = (function () {
    function Parser() {
    }
    Parser.prototype.traverse = function (program) {
        var _this = this;
        if (program.ast) {
            this.program = program;
            this.node = undefined;
            try  {
                traverse(program.ast, {
                    pre: function (node) {
                        _this.node = node;
                        _this.node.callbacks = [];
                        _this.enterNode(_this.node);
                    },
                    post: function (node) {
                        _this.node = node;
                        try  {
                            _this.node.callbacks.forEach(function (callback) {
                                callback.action.call(callback.target, this.node);
                            }, _this);
                            _this.exitNode(_this.node);
                        } finally {
                            delete _this.node.callbacks;
                        }
                    }
                });
            } finally {
                this.program = undefined;
                this.node = undefined;
            }
        }
    };

    Parser.prototype.enterNode = function (node) {
    };
    Parser.prototype.exitNode = function (node) {
    };

    Parser.prototype.addCallback = function (node, callback, target) {
        node.callbacks = node.callbacks || [];
        node.callbacks.push({ action: callback, target: target });
    };

    Parser.prototype.lexeme = function (range) {
        return this.program.lexeme(range);
    };

    Parser.prototype.nodeIsCall = function (node, callee, minArgs) {
        if (typeof minArgs === "undefined") { minArgs = 0; }
        var result = node.type === 'CallExpression';

        if (callee) {
            result = result && callee === this.lexeme(node.callee.range);
        }

        if (minArgs > 0) {
            result = result && node.arguments && node.arguments.length >= minArgs;
        }

        return result;
    };

    Parser.prototype.getPosition = function () {
        return {
            line: this.node.loc.start.line,
            column: this.node.loc.start.column,
            index: this.node.range[0]
        };
    };

    Parser.prototype.getOriginalPosition = function () {
        return this.program.getOriginalPosition(this.getPosition());
    };
    return Parser;
})();

module.exports = Parser;
//# sourceMappingURL=Parser.js.map
