var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var events = require('events');
var Q = require('q');
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
var JsonConnection = (function (_super) {
    __extends(JsonConnection, _super);
    function JsonConnection(socket, connected) {
        var _this = this;
        _super.call(this);
        this.socket = socket;
        this._buffer = new Buffer(0);
        this.id = JsonConnection.nextId++;
        this.name = "Connection";
        this.connected = connected;
        this.socket.on('connect', function () { return _this.onConnect(); });
        this.socket.on('data', function (data) { return _this.onData(data); });
        this.socket.on('error', function (error) { return _this.emit('error', error); });
        this.socket.on('close', function (had_error) { return _this.onClose(had_error); });
    }
    JsonConnection.prototype.onConnect = function () {
        this.connected = true;
        this.emit('connect');
    };
    JsonConnection.prototype.onData = function (data) {
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
    JsonConnection.prototype.onClose = function (had_error) {
        if (this._buffer.length > 0) {
            console.log("Writing buffer");
            this.emitMessage(this._buffer.toString('utf8'));
        }
        this.connected = false;
        this.emit('close', had_error);
    };
    JsonConnection.prototype.emitMessage = function (message) {
        try {
            this.emit('message-raw', message);
            this.emit('message', this.parse(message));
        }
        catch (e) {
            this.emit('error', e);
            this.end();
        }
    };
    JsonConnection.prototype.parse = function (message) {
        try {
            return JSON.parse(message);
        }
        catch (e) {
            throw JsonParseError(e, message);
        }
    };
    JsonConnection.prototype.stringify = function (value) {
        try {
            return JSON.stringify(value, null, '');
        }
        catch (e) {
            throw JsonStringifyError(e, value);
        }
    };
    JsonConnection.prototype.write = function (value) {
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
    JsonConnection.prototype.end = function () {
        this.socket.end();
    };
    JsonConnection.nextId = 1;
    return JsonConnection;
})(events.EventEmitter);
module.exports = JsonConnection;
//# sourceMappingURL=JsonConnection.js.map