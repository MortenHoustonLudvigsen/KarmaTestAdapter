var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
/// <reference path="../bower_components/dt-node/node.d.ts" />
var events = require('events');
var Logger = (function (_super) {
    __extends(Logger, _super);
    function Logger() {
        _super.apply(this, arguments);
    }
    Logger.prototype.info = function (message) {
        this.emit('info', message);
    };
    Logger.prototype.debug = function (message) {
        this.emit('debug', message);
    };
    Logger.prototype.warn = function (message) {
        this.emit('warn', message);
    };
    Logger.prototype.error = function (err) {
        this.emit('error', err);
    };
    return Logger;
})(events.EventEmitter);
module.exports = Logger;
