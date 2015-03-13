import net = require('net');
import events = require('events');
import util = require('util');
import Q = require('q');

function stringify(value: any): string {
    return util.inspect(value, { depth: null });
}

function handledTracker(): (handled?: boolean) => boolean {
    var isHandled = false;
    return (handled?: boolean) => {
        if (handled) {
            isHandled = true;
        }
        return isHandled;
    };
}

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

export class Server extends events.EventEmitter {
    constructor(public port: number = 0, public host?: string) {
        super();
        this._server = net.createServer((socket) => this.newSocket(socket));
        this._server.on('listening',() => {
            this.address = this._server.address();
            this.emit('listening');
        });
    }

    private _server: net.Server;
    private _commands: Command[] = [];
    address: { port: number; family: string; address: string; };

    private newSocket(socket: net.Socket): void {
        var connection = new Connection(socket);
        connection.on('message', message => this.onMessage(message, connection));
        connection.on('message-raw', message => this.emit('message-raw', message, connection));
        connection.on('error', error => this.onError(error, connection));
        connection.on('close', data => this.onClose(data, connection));
        this.emit('connected', connection);
    }

    public start() {
        this._server.listen(this.port, this.host);
    }

    public addCommand(command: string, onMessageRecieved: (command: Command, message: any, connection: Connection) => Q.Promise<void>): Command;
    public addCommand(command: Command): Command;
    public addCommand(command: string | Command, onMessageRecieved?: (command: Command, message: any, connection: Connection) => Q.Promise<void>): Command {
        if (typeof command === 'string') {
            return this.addCommand(new Command(command, onMessageRecieved));
        } else {
            this._commands.push(command);
            command.attachTo(this);
            return command;
        }
    }

    private handledEmit(event: string, args: any[], unhandled: () => void) {
        var isHandled = handledTracker();
        args.unshift(event);
        args.push(isHandled);
        this.emit.apply(this, args);
        if (!isHandled()) {
            unhandled();
        }
    }

    public onMessage(message: any, connection: Connection) {
        this.handledEmit('message', [message, connection],() => {
            //connection.end();
            this.onError(new Error("Unhandled message: " + stringify(message)), connection);
        });
    }

    public onError(error: any, connection: Connection) {
        this.handledEmit('message', [error, connection],() => {
            console.log('Error: ' + stringify(error));
        });
    }

    public onClose(had_error: boolean, connection: Connection) {
        this.handledEmit('close', [had_error, connection],() => {
            console.log('Connection closed: ' + (had_error ? "Had error" : "Without error"));
        });
    }
}

export class Connection extends events.EventEmitter {
    private static nextId = 1;

    constructor(public socket: net.Socket) {
        super();
        this.socket.on('data',(data) => this.onData(data));
        this.socket.on('error',(error) => this.emit('error', error));
        this.socket.on('close',(had_error) => this.onClose(had_error));
    }

    private _buffer = new Buffer(0);
    public id = Connection.nextId++;
    public connected = true;
    public name = "Connection";

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

class Dictionary<TItem> {
    static _nextId = 1;

    private _items: { id: number; item: TItem }[] = [];

    add(item: TItem): number {
        var id = Dictionary._nextId++;
        this._items.push({ id: id, item: item });
        return id;
    }

    remove(id: number): void {
        this._items = this._items.filter(item => item.id !== id);
    }

    items(): TItem[] {
        return this._items.map(item => item.item);
    }
}

export class Command extends events.EventEmitter {
    constructor(public command: string, onMessageRecieved?: (command: Command, message: any, connection: Connection) => Q.Promise<void>) {
        super();
        this._messageListener = this._messageListener.bind(this);
        if (onMessageRecieved) {
            this.onMessageRecieved = onMessageRecieved;
        }
    }

    private connections = new Dictionary<Connection>();
    private requests = new Dictionary<Q.Promise<void>>();

    private _messageListener(message: any, connection: Connection, handled: (handled?: boolean) => void): void {
        if (message.command === this.command) {
            handled(true);
            connection.name = this.command;
            var connectionId = this.connections.add(connection);
            var request = this.onMessageRecieved(this, message, connection);
            var requestId = this.requests.add(request);
            request
                .then(() => this.end(connection))
                .finally(() => this.requests.remove(requestId))
                .finally(() => this.connections.remove(connectionId));
        }
    }

    attachTo(server: Server): void {
        server.on('message', this._messageListener);
    }

    detachFrom(server: Server): void {
        server.removeListener('message', this._messageListener);
    }

    onMessageRecieved(command: Command, message: any, connection: Connection): Q.Promise<void> {
        return Q.resolve<void>(undefined);
    }

    end(connection: Connection): Q.Promise<void> {
        if (connection.connected) {
            return connection.write({ FINISHED: true }).thenResolve<void>(undefined);
        }
        return Q.resolve<void>(undefined);
    }

    getRequests(): Q.Promise<void>[] {
        return this.requests.items();
    }
}
