var Extensions = require('./Extensions');
var JsonClient = require('./JsonClient');
var TestNetServer = (function () {
    function TestNetServer(testContainerName, port, host) {
        this.testContainerName = testContainerName;
        this.port = port;
        this.host = host;
        this.extensions = new Extensions();
        this.testRunStartedClient = new JsonClient('test run started', this.port, this.host);
        this.testRunCompletedClient = new JsonClient('test run completed', this.port, this.host);
    }
    TestNetServer.prototype.testRunStarted = function () {
        return this.testRunStartedClient.run();
    };
    TestNetServer.prototype.testRunCompleted = function (specs) {
        return this.testRunCompletedClient.run({
            specs: specs
        });
    };
    return TestNetServer;
})();
module.exports = TestNetServer;
//# sourceMappingURL=TestNetServer.js.map