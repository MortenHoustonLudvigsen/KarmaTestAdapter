var fs = require('fs');
var path = require('path');
var url = require('url');
var SourceMap = require("source-map");
var SourceMapResolve = require("source-map-resolve");
var errorStackParser = require('error-stack-parser');
var StackTrace = (function () {
    function StackTrace(config, logger) {
        this.config = config;
        this.logger = logger;
        this.urlRoot = this.config.urlRoot || '/';
        this.urlBase = url.parse(path.join(this.urlRoot, 'base')).pathname;
        this.urlAbsoluteBase = url.parse(path.join(this.urlRoot, 'absolute')).pathname;
        this.basePath = this.config.basePath;
        this.sourceMapConsumers = {};
    }
    StackTrace.prototype.getSourceMapConsumer = function (filePath) {
        if (filePath in this.sourceMapConsumers) {
            return this.sourceMapConsumers[filePath];
        }
        var content = fs.readFileSync(filePath).toString();
        var sourceMap = SourceMapResolve.resolveSync(content, filePath, fs.readFileSync);
        var consumer = sourceMap ? new SourceMap.SourceMapConsumer(sourceMap.map) : null;
        if (consumer) {
            consumer['resolvePath'] = function (filePath) { return path.resolve(path.dirname(sourceMap.sourcesRelativeTo), filePath); };
        }
        return consumer;
    };
    StackTrace.prototype.getFilePath = function (fileName) {
        if (typeof fileName === 'string') {
            var filePath = url.parse(fileName).pathname;
            if (filePath.indexOf(this.urlBase) === 0) {
                return path.join(this.basePath, filePath.substring(this.urlBase.length));
            }
            else if (filePath.indexOf(this.urlAbsoluteBase) === 0) {
                return filePath.substring(this.urlAbsoluteBase.length);
            }
        }
        return fileName;
    };
    StackTrace.prototype.resolveSource = function (source) {
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
    };
    StackTrace.prototype.getRealSource = function (source, relative) {
        if (source) {
            if (source.source) {
                return this.getRealSource(source.source, relative);
            }
            if (relative && source.fileName) {
                source.fileName = path.relative(this.basePath, source.fileName);
            }
        }
        return source;
    };
    StackTrace.prototype.getSource = function (error) {
        if (!error)
            return;
        var stack = this.parseStack(error, false);
        if (!stack)
            return;
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
    };
    StackTrace.prototype.parseStack = function (error, relative) {
        var _this = this;
        var self = this;
        try {
            return errorStackParser.parse(error).map(function (frame) { return getSource(frame); }).map(function (frame) { return _this.resolveSource(frame); }).map(function (frame) { return _this.getRealSource(frame, relative); });
        }
        catch (e) {
            this.logger.debug(e);
            return;
        }
        function getSource(frame) {
            return {
                functionName: frame.functionName,
                fileName: self.getFilePath(frame.fileName),
                lineNumber: frame.lineNumber,
                columnNumber: frame.columnNumber
            };
        }
    };
    StackTrace.prototype.normalizeStack = function (stack) {
        var relative = false;
        var basePath = this.basePath;
        var stackFrames = this.parseStack({ stack: stack }, relative);
        if (stackFrames) {
            return stackFrames.map(function (frame) { return formatFrame(frame); });
        }
        else {
            return stack.split(/\r\n|\n/g);
        }
        function formatFrame(frame) {
            var result = '    at ';
            result += frame.functionName || '<anonymous>';
            result += ' in ';
            result += frame.fileName;
            if (typeof frame.lineNumber === 'number' && frame.lineNumber >= 0) {
                result += ':line ' + frame.lineNumber.toString(10);
            }
            return result;
        }
    };
    return StackTrace;
})();
module.exports = StackTrace;
//# sourceMappingURL=StackTrace.js.map