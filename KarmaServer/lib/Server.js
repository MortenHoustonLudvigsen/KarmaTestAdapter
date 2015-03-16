var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var JsonServer = require('../lib/JsonServer');
var Karma = require('./Karma');
var Q = require('q');
var Server = (function (_super) {
    __extends(Server, _super);
    function Server(config, emitter, logger) {
        var _this = this;
        _super.call(this);
        this.config = config;
        this.emitter = emitter;
        this.events = Q.defer();
        this.specs = Q.defer();
        this.event = this.addCommand('event', function (command, message, connection) {
            _this.events.promise.progress(function (event) {
                var message = event;
                if (typeof event === 'string') {
                    message = { event: event };
                }
                connection.write(message);
            });
            _this.events.notify('Connected');
            return _this.events.promise;
        });
        this.stop = this.addCommand('stop', function (command, message, connection) {
            _this.events.resolve(undefined);
            var requests = Array.prototype.concat.call(_this.event.getRequests(), _this.discover.getRequests());
            return Q.all(requests).then(function () { return process.exit(0); });
        });
        this.discover = this.addCommand('discover', function (command, message, connection) {
            return _this.specs.promise.then(function (specs) {
                var promise = Q.resolve(undefined);
                specs.forEach(function (spec) {
                    promise = promise.then(function () {
                        if (connection.connected) {
                            return connection.write(spec);
                        }
                        throw new Error("Connection closed");
                    });
                });
                return promise.then(function () { return _this.events.notify('Karma discovered'); });
            });
        });
        this.requestRun = this.addCommand('requestRun', function (command, message, connection) {
            _this.events.notify({
                event: 'Karma run requested',
                tests: message.tests
            });
            return Q.resolve(undefined);
        });
        this.port = config.vsServerPort || 0;
        this.logger = logger.create('VS Server', Karma.karma.Constants.LOG_DEBUG);
        this.on('listening', function () { return _this.logger.info('Started - port: ' + _this.address.port); });
        this.start();
    }
    Server.prototype.onError = function (error, connection) {
        this.logger.error(error);
    };
    Server.prototype.onClose = function (had_error, connection) {
        //try {
        //    throw new Error();
        //} catch (e) {
        //    this.logger.info('Connection ' + connection.name + ' closed: ' + (had_error ? "Had error" : "Without error") + '\n' + e.stack.replace(/^[^\n]*\n/, ''));
        //}
        //this.logger.info('Connection ' + connection.name + ' closed: ' + (had_error ? "Had error" : "Without error"));
    };
    Server.prototype.karmaStart = function () {
        if (this.specs.promise.isFulfilled()) {
            this.specs = Q.defer();
        }
        this.logger.info('Karma run start');
        this.events.notify('Karma run start');
    };
    Server.prototype.karmaEnd = function (specs) {
        this.logger.info('Karma run complete:\n', specs);
        this.events.notify('Karma run complete');
        this.specs.resolve(specs);
    };
    Server.$inject = ['config', 'emitter', 'logger'];
    return Server;
})(JsonServer.Server);
module.exports = Server;
//# sourceMappingURL=Server.js.map