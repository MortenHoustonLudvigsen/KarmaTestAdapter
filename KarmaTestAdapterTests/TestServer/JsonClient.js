var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var net = require('net');
var events = require('events');
var Q = require('q');
var JsonConnection = require('./JsonConnection');
var JsonClient = (function (_super) {
    __extends(JsonClient, _super);
    function JsonClient(commandName, port, host) {
        _super.call(this);
        this.commandName = commandName;
        this.port = port;
        this.host = host;
    }
    JsonClient.prototype.run = function (request, onMessage) {
        var _this = this;
        request = request || {};
        request.command = this.commandName;
        var deferred = Q.defer();
        var socket = new net.Socket();
        var connection = new JsonConnection(socket, false);
        connection.on('message', function (message) {
            _this.onMessage(message, connection);
            if (message && message['FINISHED']) {
                socket.end();
            }
            else if (onMessage) {
                onMessage(message);
            }
        });
        connection.on('error', function (error) {
            deferred.reject(error);
            _this.onError(error, connection);
        });
        connection.on('close', function (data) {
            deferred.resolve(undefined);
            _this.onClose(data, connection);
        });
        socket.connect(this.port, this.host, function () {
            connection.write(request);
        });
        return deferred.promise;
    };
    JsonClient.prototype.onMessage = function (message, connection) {
        this.emit('message', message, connection);
    };
    JsonClient.prototype.onError = function (error, connection) {
        this.emit('error', error, connection);
    };
    JsonClient.prototype.onClose = function (had_error, connection) {
        this.emit('close', had_error, connection);
    };
    return JsonClient;
})(events.EventEmitter);
module.exports = JsonClient;
//# sourceMappingURL=JsonClient.js.map