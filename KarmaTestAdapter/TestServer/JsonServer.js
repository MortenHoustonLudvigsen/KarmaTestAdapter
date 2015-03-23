var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var net = require('net');
var events = require('events');
var util = require('util');
var JsonConnection = require('./JsonConnection');
var JsonServerCommand = require('./JsonServerCommand');
function stringify(value) {
    return util.inspect(value, { depth: null });
}
function handledTracker() {
    var isHandled = false;
    return function (handled) {
        if (handled) {
            isHandled = true;
        }
        return isHandled;
    };
}
var JsonServer = (function (_super) {
    __extends(JsonServer, _super);
    function JsonServer(port, host) {
        var _this = this;
        if (port === void 0) { port = 0; }
        _super.call(this);
        this.port = port;
        this.host = host;
        this._commands = [];
        this._server = net.createServer(function (socket) { return _this.newSocket(socket); });
        this._server.on('listening', function () {
            _this.address = _this._server.address();
            _this.emit('listening');
        });
    }
    JsonServer.prototype.newSocket = function (socket) {
        var _this = this;
        var connection = new JsonConnection(socket, true);
        connection.on('message', function (message) { return _this.onMessage(message, connection); });
        connection.on('message-raw', function (message) { return _this.emit('message-raw', message, connection); });
        connection.on('error', function (error) { return _this.onError(error, connection); });
        connection.on('close', function (data) { return _this.onClose(data, connection); });
        this.emit('connected', connection);
    };
    JsonServer.prototype.start = function () {
        this._server.listen(this.port, this.host);
    };
    JsonServer.prototype.addCommand = function (command, onMessageRecieved) {
        if (typeof command === 'string') {
            return this.addCommand(new JsonServerCommand(command, onMessageRecieved));
        }
        else {
            this._commands.push(command);
            command.attachTo(this);
            return command;
        }
    };
    JsonServer.prototype.handledEmit = function (event, args, unhandled) {
        var isHandled = handledTracker();
        args.unshift(event);
        args.push(isHandled);
        this.emit.apply(this, args);
        if (!isHandled()) {
            unhandled();
        }
    };
    JsonServer.prototype.onMessage = function (message, connection) {
        var _this = this;
        this.handledEmit('message', [message, connection], function () {
            //connection.end();
            _this.onError(new Error("Unhandled message: " + stringify(message)), connection);
        });
    };
    JsonServer.prototype.onError = function (error, connection) {
        this.handledEmit('message', [error, connection], function () {
            console.log('Error: ' + stringify(error));
        });
    };
    JsonServer.prototype.onClose = function (had_error, connection) {
        this.handledEmit('close', [had_error, connection], function () {
            console.log('Connection closed: ' + (had_error ? "Had error" : "Without error"));
        });
    };
    return JsonServer;
})(events.EventEmitter);
module.exports = JsonServer;
//# sourceMappingURL=JsonServer.js.map