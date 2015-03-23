import net = require('net');
import events = require('events');
import util = require('util');
import Q = require('q');
import JsonConnection = require('./JsonConnection');
import JsonServerCommand = require('./JsonServerCommand');

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

class JsonServer extends events.EventEmitter {
    constructor(public port: number = 0, public host?: string) {
        super();
        this._server = net.createServer((socket) => this.newSocket(socket));
        this._server.on('listening',() => {
            this.address = this._server.address();
            this.emit('listening');
        });
    }

    private _server: net.Server;
    private _commands: JsonServerCommand[] = [];
    address: { port: number; family: string; address: string; };

    private newSocket(socket: net.Socket): void {
        var connection = new JsonConnection(socket, true);
        connection.on('message', message => this.onMessage(message, connection));
        connection.on('message-raw', message => this.emit('message-raw', message, connection));
        connection.on('error', error => this.onError(error, connection));
        connection.on('close', data => this.onClose(data, connection));
        this.emit('connected', connection);
    }

    public start() {
        this._server.listen(this.port, this.host);
    }

    public addCommand(command: string, onMessageRecieved: (command: JsonServerCommand, message: any, connection: JsonConnection) => Q.Promise<void>): JsonServerCommand;
    public addCommand(command: JsonServerCommand): JsonServerCommand;
    public addCommand(command: string | JsonServerCommand, onMessageRecieved?: (command: JsonServerCommand, message: any, connection: JsonConnection) => Q.Promise<void>): JsonServerCommand {
        if (typeof command === 'string') {
            return this.addCommand(new JsonServerCommand(command, onMessageRecieved));
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

    public onMessage(message: any, connection: JsonConnection) {
        this.handledEmit('message', [message, connection],() => {
            //connection.end();
            this.onError(new Error("Unhandled message: " + stringify(message)), connection);
        });
    }

    public onError(error: any, connection: JsonConnection) {
        this.handledEmit('message', [error, connection],() => {
            console.log('Error: ' + stringify(error));
        });
    }

    public onClose(had_error: boolean, connection: JsonConnection) {
        this.handledEmit('close', [had_error, connection],() => {
            console.log('Connection closed: ' + (had_error ? "Had error" : "Without error"));
        });
    }
}

export = JsonServer;
