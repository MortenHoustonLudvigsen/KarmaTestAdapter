import fs = require('fs');
import path = require('path');
import url = require('url');
import Karma = require('./Karma');
import Specs = require('./Specs');
import SourceMap = require("source-map");
var SourceMapResolve = require("source-map-resolve");
var errorStackParser = require('error-stack-parser');

type ErrorWithStack = {
    skip?: number;
    skipFunctions?: string;
    stack?: string;
    stacktrace?: string;
    'opera#sourceloc'?: any;
};

class StackTrace {
    constructor(private config: Karma.KarmaConfig, private logger: Karma.Logger) {
    }

    private urlRoot = this.config.urlRoot || '/';
    private urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
    private urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
    private basePath = this.config.basePath;
    private sourceMapConsumers: { [filePath: string]: SourceMap.SourceMapConsumer } = {};

    private getSourceMapConsumer(filePath: string): SourceMap.SourceMapConsumer {
        if (filePath in this.sourceMapConsumers) {
            return this.sourceMapConsumers[filePath];
        }
        try {
            var content = fs.readFileSync(filePath).toString();
            var sourceMap = SourceMapResolve.resolveSync(content, filePath, fs.readFileSync);
            var consumer = sourceMap ? new SourceMap.SourceMapConsumer(sourceMap.map) : null;
            if (consumer) {
                consumer['resolvePath'] = (filePath: string) => path.resolve(path.dirname(sourceMap.sourcesRelativeTo), filePath);
            }
            this.sourceMapConsumers[filePath] = consumer;
            return consumer;
        } catch (e) {
            this.sourceMapConsumers[filePath] = undefined;
        }
    }

    getFilePath(fileName: string): string {
        if (typeof fileName === 'string') {
            var filePath = url.parse(fileName).pathname;
            if (filePath.indexOf(this.urlBase) === 0) {
                return path.join(this.basePath, filePath.substring(this.urlBase.length));
            } else if (filePath.indexOf(this.urlAbsoluteBase) === 0) {
                return filePath.substring(this.urlAbsoluteBase.length);
            }
        }
        return fileName;
    }

    resolveSource(source: Specs.Source): Specs.Source {
        if (source && source.fileName) {
            source.fileName = this.getFilePath(source.fileName);
            var consumer = this.getSourceMapConsumer(source.fileName);
            if (consumer) {
                var position = {
                    line: Math.max(source.lineNumber || 1, 1),
                    column: Math.max(source.columnNumber || 1, 1) - 1,
                    bias: SourceMap.SourceMapConsumer.GREATEST_LOWER_BOUND
                };
                var orig = consumer.originalPositionFor(position);
                if (!orig.source) {
                    position.bias = SourceMap.SourceMapConsumer.LEAST_UPPER_BOUND;
                    orig = consumer.originalPositionFor(position);
                }
                if (orig.source) {
                    source.source = this.resolveSource({
                        functionName: source.functionName,
                        fileName: consumer['resolvePath'](orig.source),
                        lineNumber: orig.line,
                        columnNumber: orig.column + 1
                    });
                }
            }
        }
        return source;
    }

    getRealSource(source: Specs.Source, relative: boolean): Specs.Source {
        if (source) {
            if (source.source) {
                return this.getRealSource(source.source, relative);
            }
            if (relative && source.fileName) {
                source.fileName = path.relative(this.basePath, source.fileName);
            }
        }
        return source;
    }

    getSource(error: ErrorWithStack): Specs.Source {
        if (!error) return;
        var stack = this.parseStack(error, false);
        if (!stack) return;
        if (typeof error.skip === 'number' && error.skip > 0) {
            stack = stack.slice(error.skip);
        }
        if (typeof error.skipFunctions === 'string' && error.skipFunctions) {
            var re = new RegExp(error.skipFunctions);
            while (stack.length > 0 && re.test(stack[0].functionName)) {
                stack.shift();
            }
        }
        return stack[0];
    }

    parseStack(error: ErrorWithStack, relative: boolean): Specs.Source[] {
        var self = this;

        try {
            return errorStackParser.parse(error)
                .map(frame => getSource(frame))
                .map(frame => this.resolveSource(frame))
                .map(frame => this.getRealSource(frame, relative));
        } catch (e) {
            this.logger.debug(e);
            return;
        }

        function getSource(frame: any): Specs.Source {
            return {
                functionName: frame.functionName,
                fileName: self.getFilePath(frame.fileName),
                lineNumber: frame.lineNumber,
                columnNumber: frame.columnNumber
            };
        }
    }

    normalizeStack(stack: string): string[] {
        var relative = false;
        var basePath = this.basePath;

        var stackFrames = this.parseStack({ stack: stack }, relative);
        if (stackFrames) {
            return stackFrames.map(frame => formatFrame(frame));
        } else {
            return stack.split(/\r\n|\n/g);
        }

        function formatFrame(frame: Specs.Source): string {
            var result = '    at ';
            result += frame.functionName || '<anonymous>';
            result += ' in ';
            result += frame.fileName;
            if (typeof frame.lineNumber === 'number' && frame.lineNumber >= 0) {
                result += ':line ' + frame.lineNumber.toString(10);
            }
            return result;
        }
    }
}

export = StackTrace;
