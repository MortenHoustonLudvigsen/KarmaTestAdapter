var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var JsonServer = require('./JsonServer');
var Extensions = require('./Extensions');
var Q = require('q');
var TestServer = (function (_super) {
    __extends(TestServer, _super);
    function TestServer(testContainerName, port, host) {
        var _this = this;
        if (port === void 0) { port = 0; }
        _super.call(this, port, host);
        this.testContainerName = testContainerName;
        this.port = port;
        this.host = host;
        this.extensions = new Extensions();
        this.events = Q.defer();
        this.specs = Q.defer();
        this.testRunStartedCommand = this.addCommand('test run started', function (command, message, connection) {
            _this.testRunStarted();
            return Q.resolve(undefined);
        });
        this.testRunCompletedCommand = this.addCommand('test run completed', function (command, message, connection) {
            _this.testRunCompleted(message.specs);
            return Q.resolve(undefined);
        });
        this.eventCommand = this.addCommand('event', function (command, message, connection) {
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
        this.stopCommand = this.addCommand('stop', function (command, message, connection) {
            _this.events.resolve(undefined);
            var requests = Array.prototype.concat.call(_this.eventCommand.getRequests(), _this.discoverCommand.getRequests());
            return Q.all(requests).then(function () { return process.exit(0); });
        });
        this.discoverCommand = this.addCommand('discover', function (command, message, connection) {
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
                return promise.then(function () { return _this.events.notify('Tests discovered'); });
            });
        });
        this.requestRunCommand = this.addCommand('requestRun', function (command, message, connection) {
            _this.events.notify({
                event: 'Test run requested',
                tests: message.tests
            });
            return Q.resolve(undefined);
        });
    }
    TestServer.prototype.loadExtensions = function (extensionsModule) {
        this.extensions.load(extensionsModule);
    };
    TestServer.prototype.onError = function (error, connection) {
    };
    TestServer.prototype.onClose = function (had_error, connection) {
    };
    TestServer.prototype.testRunStarted = function () {
        if (this.specs.promise.isFulfilled()) {
            this.specs = Q.defer();
        }
        this.events.notify('Test run start');
    };
    TestServer.prototype.testRunCompleted = function (specs) {
        this.events.notify('Test run complete');
        this.specs.resolve(specs);
    };
    return TestServer;
})(JsonServer);
module.exports = TestServer;
//# sourceMappingURL=TestServer.js.map