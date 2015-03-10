var net = require('net');
var Q = require('q');
function freePort(startPort) {
    if (startPort === void 0) { startPort = 0; }
    var result = Q.defer();
    var server = net.createServer();
    var port;
    server.on('listening', function () {
        port = server.address().port;
        server.close();
    });
    server.on('close', function () {
        result.resolve(port);
    });
    server.on('error', function () {
        freePort(startPort + 1).then(function (p) { return result.resolve(p); });
    });
    server.listen(startPort, '127.0.0.1');
    return result.promise;
}
module.exports = freePort;
//# sourceMappingURL=FreePort.js.map