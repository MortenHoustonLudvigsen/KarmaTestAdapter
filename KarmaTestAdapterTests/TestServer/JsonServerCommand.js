var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var events = require('events');
var Q = require('q');
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
var JsonServerCommand = (function (_super) {
    __extends(JsonServerCommand, _super);
    function JsonServerCommand(command, onMessageRecieved) {
        _super.call(this);
        this.command = command;
        this.connections = new Dictionary();
        this.requests = new Dictionary();
        this._messageListener = this._messageListener.bind(this);
        if (onMessageRecieved) {
            this.onMessageRecieved = onMessageRecieved;
        }
    }
    JsonServerCommand.prototype._messageListener = function (message, connection, handled) {
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
    JsonServerCommand.prototype.attachTo = function (server) {
        server.on('message', this._messageListener);
    };
    JsonServerCommand.prototype.detachFrom = function (server) {
        server.removeListener('message', this._messageListener);
    };
    JsonServerCommand.prototype.onMessageRecieved = function (command, message, connection) {
        return Q.resolve(undefined);
    };
    JsonServerCommand.prototype.end = function (connection) {
        if (connection.connected) {
            return connection.write({ FINISHED: true }).thenResolve(undefined);
        }
        return Q.resolve(undefined);
    };
    JsonServerCommand.prototype.getRequests = function () {
        return this.requests.items();
    };
    return JsonServerCommand;
})(events.EventEmitter);
module.exports = JsonServerCommand;
//# sourceMappingURL=JsonServerCommand.js.map