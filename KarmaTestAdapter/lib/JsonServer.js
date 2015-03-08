var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var net = require('net');
var events = require('events');
var util = require('util');
var Q = require('q');
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
function JsonParseError(err, text) {
    var error = new Error("Could not parse JSON");
    error.error = err;
    error.text = text;
    return error;
}
function JsonStringifyError(err, value) {
    var error = new Error("Could not stringify to JSON");
    error.error = err;
    error.value = value;
    return error;
}
var Server = (function (_super) {
    __extends(Server, _super);
    function Server(port, host) {
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
    Server.prototype.newSocket = function (socket) {
        var _this = this;
        var connection = new Connection(socket);
        connection.on('message', function (message) { return _this.onMessage(message, connection); });
        connection.on('message-raw', function (message) { return _this.emit('message-raw', message, connection); });
        connection.on('error', function (error) { return _this.onError(error, connection); });
        connection.on('close', function (data) { return _this.onClose(data, connection); });
        this.emit('connected', connection);
    };
    Server.prototype.start = function () {
        this._server.listen(this.port, this.host);
    };
    Server.prototype.addCommand = function (command, onMessageRecieved) {
        if (typeof command === 'string') {
            return this.addCommand(new Command(command, onMessageRecieved));
        }
        else {
            this._commands.push(command);
            command.attachTo(this);
            return command;
        }
    };
    Server.prototype.handledEmit = function (event, args, unhandled) {
        var isHandled = handledTracker();
        args.unshift(event);
        args.push(isHandled);
        this.emit.apply(this, args);
        if (!isHandled()) {
            unhandled();
        }
    };
    Server.prototype.onMessage = function (message, connection) {
        var _this = this;
        this.handledEmit('message', [message, connection], function () {
            //connection.end();
            _this.onError(new Error("Unhandled message: " + stringify(message)), connection);
        });
    };
    Server.prototype.onError = function (error, connection) {
        this.handledEmit('message', [error, connection], function () {
            console.log('Error: ' + stringify(error));
        });
    };
    Server.prototype.onClose = function (had_error, connection) {
        this.handledEmit('close', [had_error, connection], function () {
            console.log('Connection closed: ' + (had_error ? "Had error" : "Without error"));
        });
    };
    return Server;
})(events.EventEmitter);
exports.Server = Server;
var Connection = (function (_super) {
    __extends(Connection, _super);
    function Connection(socket) {
        var _this = this;
        _super.call(this);
        this.socket = socket;
        this._buffer = new Buffer(0);
        this.id = Connection.nextId++;
        this.connected = true;
        this.name = "Connection";
        this.socket.on('data', function (data) { return _this.onData(data); });
        this.socket.on('error', function (error) { return _this.emit('error', error); });
        this.socket.on('close', function (had_error) { return _this.onClose(had_error); });
    }
    Connection.prototype.onData = function (data) {
        var buffer = this._buffer;
        var start = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i] == 0) {
                if (i > start) {
                    buffer = Buffer.concat([buffer, data.slice(start, i)]);
                }
                if (buffer.length > 0) {
                    this.emitMessage(buffer.toString('utf8'));
                }
                buffer = new Buffer(0);
                start = i + 1;
            }
        }
        this._buffer = Buffer.concat([buffer, data.slice(start)]);
    };
    Connection.prototype.onClose = function (had_error) {
        if (this._buffer.length > 0) {
            console.log("Writing buffer");
            this.emitMessage(this._buffer.toString('utf8'));
        }
        this.connected = false;
        this.emit('close', had_error);
    };
    Connection.prototype.emitMessage = function (message) {
        try {
            this.emit('message-raw', message);
            this.emit('message', this.parse(message));
        }
        catch (e) {
            this.emit('error', e);
            this.end();
        }
    };
    Connection.prototype.parse = function (message) {
        try {
            return JSON.parse(message);
        }
        catch (e) {
            throw JsonParseError(e, message);
        }
    };
    Connection.prototype.stringify = function (value) {
        try {
            return JSON.stringify(value, null, '');
        }
        catch (e) {
            throw JsonStringifyError(e, value);
        }
    };
    Connection.prototype.write = function (value) {
        var _this = this;
        if (!this.connected) {
            throw new Error("The connection is closed");
        }
        var deferred = Q.defer();
        try {
            function errorListener(e) {
                deferred.reject(e);
            }
            this.socket.on('error', errorListener);
            this.socket.write(new Buffer(this.stringify(value) + '\0', 'utf8'), function () {
                _this.socket.removeListener('error', errorListener);
                deferred.resolve(undefined);
            });
            return deferred.promise;
        }
        catch (e) {
            this.emit('error', e);
            this.end();
        }
    };
    Connection.prototype.end = function () {
        this.socket.end();
    };
    Connection.nextId = 1;
    return Connection;
})(events.EventEmitter);
exports.Connection = Connection;
var Dictionary = (function () {
    function Dictionary() {
        this._items = [];
    }
    Dictionary.prototype.add = function (item) {
        var id = Dictionary._nextId++;
        this._items.push({ id: id, item: item });
        return id;
    };
    Dictionary.prototype.remove = function (id) {
        this._items = this._items.filter(function (item) { return item.id !== id; });
    };
    Dictionary.prototype.items = function () {
        return this._items.map(function (item) { return item.item; });
    };
    Dictionary._nextId = 1;
    return Dictionary;
})();
var Command = (function (_super) {
    __extends(Command, _super);
    function Command(command, onMessageRecieved) {
        _super.call(this);
        this.command = command;
        this.connections = new Dictionary();
        this.requests = new Dictionary();
        this._messageListener = this._messageListener.bind(this);
        if (onMessageRecieved) {
            this.onMessageRecieved = onMessageRecieved;
        }
    }
    Command.prototype._messageListener = function (message, connection, handled) {
        var _this = this;
        if (message.command === this.command) {
            handled(true);
            connection.name = this.command;
            var connectionId = this.connections.add(connection);
            var request = this.onMessageRecieved(this, message, connection);
            var requestId = this.requests.add(request);
            request.then(function () { return _this.end(connection); }).finally(function () { return _this.requests.remove(requestId); }).finally(function () { return _this.connections.remove(connectionId); });
        }
    };
    Command.prototype.attachTo = function (server) {
        server.on('message', this._messageListener);
    };
    Command.prototype.detachFrom = function (server) {
        server.removeListener('message', this._messageListener);
    };
    Command.prototype.onMessageRecieved = function (command, message, connection) {
        return Q.resolve(undefined);
    };
    Command.prototype.end = function (connection) {
        if (connection.connected) {
            return connection.write({ FINISHED: true }).thenResolve(undefined);
        }
        return Q.resolve(undefined);
    };
    Command.prototype.getRequests = function () {
        return this.requests.items();
    };
    return Command;
})(events.EventEmitter);
exports.Command = Command;
//# sourceMappingURL=JsonServer.js.map