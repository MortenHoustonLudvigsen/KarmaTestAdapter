import net = require('net');
import Q = require('q');

function freePort(startPort: number = 0): Q.Promise<number> {
    var result = Q.defer<number>();
    var server = net.createServer();
    var port: number;

    server.on('listening',() => {
        port = server.address().port;
        server.close();
    });

    server.on('close',() => {
        result.resolve(port);
    });

    server.on('error',() => {
        freePort(startPort + 1).then(p => result.resolve(p));
    });

    server.listen(startPort, '127.0.0.1');

    return result.promise;
}

export = freePort;
