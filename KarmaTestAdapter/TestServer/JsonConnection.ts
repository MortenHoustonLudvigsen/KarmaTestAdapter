import net = require('net');
import events = require('events');
import Q = require('q');

interface JsonParseError extends Error {
    error: Error;
    text: string;
}

function JsonParseError(err: Error, text: string): JsonParseError {
    var error = <JsonParseError>new Error("Could not parse JSON");
    error.error = err;
    error.text = text;
    return error;
}

interface JsonStringifyError extends Error {
    error: Error;
    value: any;
}

function JsonStringifyError(err: Error, value: any): JsonStringifyError {
    var error = <JsonStringifyError>new Error("Could not stringify to JSON");
    error.error = err;
    error.value = value;
    return error;
}

class JsonConnection extends events.EventEmitter {
    private static nextId = 1;

    constructor(public socket: net.Socket, connected: boolean) {
        super();
        this.connected = connected;
        this.socket.on('connect',() => this.onConnect());
        this.socket.on('data',(data) => this.onData(data));
        this.socket.on('error',(error) => this.emit('error', error));
        this.socket.on('close',(had_error) => this.onClose(had_error));
    }

    private _buffer = new Buffer(0);
    public id = JsonConnection.nextId++;
    public connected: boolean;
    public name = "Connection";

    private onConnect(): void {
        this.connected = true;
        this.emit('connect');
    }

    private onData(data: Buffer): void {
        var buffer = this._buffer;
        var start = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i] == 0) {
                if (i > start) {
                    buffer = Buffer.concat([buffer, data.slice(start, i)]);
                }
                if (buffer.length > 0) {
                    this.emitMessage(buffer.toString('utf8'));
                }
                buffer = new Buffer(0);
                start = i + 1;
            }
        }
        this._buffer = Buffer.concat([buffer, data.slice(start)]);
    }

    private onClose(had_error: boolean): void {
        if (this._buffer.length > 0) {
            console.log("Writing buffer");
            this.emitMessage(this._buffer.toString('utf8'));
        }
        this.connected = false;
        this.emit('close', had_error);
    }

    private emitMessage(message: string): void {
        try {
            this.emit('message-raw', message);
            this.emit('message', this.parse(message));
        } catch (e) {
            this.emit('error', e);
            this.end();
        }
    }

    private parse(message: string): any {
        try {
            return JSON.parse(message);
        } catch (e) {
            throw JsonParseError(e, message);
        }
    }

    private stringify(value: any): string {
        try {
            return JSON.stringify(value, null, '');
        } catch (e) {
            throw JsonStringifyError(e, value);
        }
    }

    write(value: any): Q.Promise<void> {
        if (!this.connected) {
            throw new Error("The connection is closed");
        }
        var deferred = Q.defer<void>();
        try {
            function errorListener(e) {
                deferred.reject(e);
            }
            this.socket.on('error', errorListener);
            this.socket.write(new Buffer(this.stringify(value) + '\0', 'utf8'),() => {
                this.socket.removeListener('error', errorListener);
                deferred.resolve(undefined);
            });
            return deferred.promise;
        } catch (e) {
            this.emit('error', e);
            this.end();
        }
    }

    end(): void {
        this.socket.end();
    }
}

export = JsonConnection;
