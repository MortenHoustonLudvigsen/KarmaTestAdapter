import events = require('events');
import JsonConnection = require('./JsonConnection');
import JsonServer = require('./JsonServer');
import Q = require('q');

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

class JsonServerCommand extends events.EventEmitter {
    constructor(public command: string, onMessageRecieved?: (command: JsonServerCommand, message: any, connection: JsonConnection) => Q.Promise<void>) {
        super();
        this._messageListener = this._messageListener.bind(this);
        if (onMessageRecieved) {
            this.onMessageRecieved = onMessageRecieved;
        }
    }

    private connections = new Dictionary<JsonConnection>();
    private requests = new Dictionary<Q.Promise<void>>();

    private _messageListener(message: any, connection: JsonConnection, handled: (handled?: boolean) => void): void {
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

    attachTo(server: JsonServer): void {
        server.on('message', this._messageListener);
    }

    detachFrom(server: JsonServer): void {
        server.removeListener('message', this._messageListener);
    }

    onMessageRecieved(command: JsonServerCommand, message: any, connection: JsonConnection): Q.Promise<void> {
        return Q.resolve<void>(undefined);
    }

    end(connection: JsonConnection): Q.Promise<void> {
        if (connection.connected) {
            return connection.write({ FINISHED: true }).thenResolve<void>(undefined);
        }
        return Q.resolve<void>(undefined);
    }

    getRequests(): Q.Promise<void>[] {
        return this.requests.items();
    }
}

export = JsonServerCommand;
