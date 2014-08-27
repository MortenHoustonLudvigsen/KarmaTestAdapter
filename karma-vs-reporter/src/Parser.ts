import Javascript = require('./Javascript');
import traverse = require("ast-traverse");

class Parser {
    public node;
    public program: Javascript.Program;

    public traverse(program: Javascript.Program) {
        if (program.ast) {
            this.program = program;
            this.node = undefined;
            try {
                traverse(program.ast, {
                    pre: node => {
                        this.node = node;
                        this.node.callbacks = [];
                        this.enterNode(this.node);
                    },
                    post: node => {
                        this.node = node;
                        try {
                            this.node.callbacks.forEach(function (callback) {
                                callback.action.call(callback.target, this.node);
                            }, this);
                            this.exitNode(this.node);
                        } finally {
                            delete this.node.callbacks;
                        }
                    }
                });
            } finally {
                this.program = undefined;
                this.node = undefined;
            }
        }
    }

    public enterNode(node: any): void { }
    public exitNode(node: any): void { }

    public addCallback(node: any, callback: (n: any) => void, target?: any) {
        node.callbacks = node.callbacks || [];
        node.callbacks.push({ action: callback, target: target });
    }

    public lexeme(range: Array<number>): string {
        return this.program.lexeme(range);
    }

    public nodeIsCall(node: any, callee?: string, minArgs: number = 0): boolean {
        var result = node.type === 'CallExpression';

        if (callee) {
            result = result && callee === this.lexeme(node.callee.range);
        }

        if (minArgs > 0) {
            result = result && node.arguments && node.arguments.length >= minArgs;
        }

        return result;
    }

    public getPosition() {
        return {
            line: this.node.loc.start.line,
            column: this.node.loc.start.column,
            index: this.node.range[0]
        };
    }

    public getOriginalPosition() {
        return this.program.getOriginalPosition(this.getPosition());
    }
}

export = Parser;