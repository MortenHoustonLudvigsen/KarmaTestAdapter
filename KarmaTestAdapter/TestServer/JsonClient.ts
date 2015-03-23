import net = require('net');
import events = require('events');
import util = require('util');
import Q = require('q');
import JsonConnection = require('./JsonConnection');

class JsonClient extends events.EventEmitter {
    constructor(public commandName: string, public port: number, public host?: string) {
        super();
    }

    run(request?: any, onMessage?: (message: any) => void): Q.Promise<void> {
        request = request || {};
        request.command = this.commandName;

        var deferred = Q.defer<void>();
        var socket = new net.Socket();
        var connection = new JsonConnection(socket, false);

        connection.on('message', message => {
            this.onMessage(message, connection);
            if (message && message['FINISHED']) {
                socket.end();
            } else if (onMessage) {
                onMessage(message);
            }
        });

        connection.on('error', error => {
            deferred.reject(error);
            this.onError(error, connection);
        });

        connection.on('close', data => {
            deferred.resolve(undefined);
            this.onClose(data, connection);
        });

        socket.connect(this.port, this.host,() => {
            connection.write(request);
        });

        return deferred.promise;
    }

    onMessage(message: any, connection: JsonConnection) {
        this.emit('message', message, connection);
    }

    onError(error: any, connection: JsonConnection) {
        this.emit('error', error, connection);
    }

    onClose(had_error: boolean, connection: JsonConnection) {
        this.emit('close', had_error, connection);
    }
}

export = JsonClient;