var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var TestServer = require('../node_modules/JsTestAdapter/TestServer');
var Karma = require('./Karma');
var Server = (function (_super) {
    __extends(Server, _super);
    function Server(config, emitter, logger) {
        var _this = this;
        _super.call(this, config.vsServerPort || 0);
        this.config = config;
        this.emitter = emitter;
        this.logger = logger.create('VS Server', Karma.karma.Constants.LOG_DEBUG);
        this.on('listening', function () { return _this.logger.info('Started - port: ' + _this.address.port); });
        this.start();
    }
    Server.prototype.onError = function (error, connection) {
        this.logger.error(error);
    };
    Server.prototype.testRunStarted = function () {
        this.logger.info('Test run start');
        _super.prototype.testRunStarted.call(this);
    };
    Server.prototype.testRunCompleted = function (specs) {
        this.logger.info('Test run complete');
        _super.prototype.testRunCompleted.call(this, specs);
    };
    Server.$inject = ['config', 'emitter', 'logger'];
    return Server;
})(TestServer);
module.exports = Server;
//# sourceMappingURL=Server.js.map