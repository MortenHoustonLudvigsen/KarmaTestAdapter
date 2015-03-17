import JsonServer = require('../lib/JsonServer');
import Specs = require('./Specs');
import Karma = require('./Karma');
import Q = require('q');

class Server extends JsonServer.Server {
    static $inject = ['config', 'emitter', 'logger'];

    constructor(private config: any, private emitter: Karma.EventEmitter, logger: Karma.LoggerModule) {
        super();
        this.port = config.vsServerPort || 0;
        this.logger = logger.create('VS Server', Karma.karma.Constants.LOG_DEBUG);
        this.on('listening',() => this.logger.info('Started - port: ' + this.address.port));
        this.start();
    }

    logger: Karma.Logger;
    events: Q.Deferred<void> = Q.defer<void>();
    specs: Q.Deferred<Specs.Spec[]> = Q.defer<Specs.Spec[]>();

    onError(error: any, connection: JsonServer.Connection) {
        this.logger.error(error);
    }

    onClose(had_error: boolean, connection: JsonServer.Connection) {
    }

    karmaStart(): void {
        if (this.specs.promise.isFulfilled()) {
            this.specs = Q.defer<Specs.Spec[]>();
        }
        this.logger.info('Test run start');
        this.events.notify('Test run start');
    }

    karmaEnd(specs: Specs.Spec[]): void {
        this.logger.info('Test run complete');
        this.events.notify('Test run complete');
        this.specs.resolve(specs);
    }

    event = this.addCommand('event',(command, message, connection) => {
        this.events.promise.progress(event => {
            var message = <any>event;
            if (typeof event === 'string') {
                message = { event: event };
            }
            connection.write(message);
        });
        this.events.notify('Connected');
        return this.events.promise;
    });

    stop = this.addCommand('stop',(command, message, connection) => {
        this.events.resolve(undefined);
        var requests: Q.Promise<void>[] = Array.prototype.concat.call(
            this.event.getRequests(),
            this.discover.getRequests()
        );
        return Q.all(requests)
            .then(() => process.exit(0));
    });

    discover = this.addCommand('discover',(command, message, connection) => {
        return this.specs.promise.then(specs => {
            var promise = Q.resolve<void>(undefined);
            specs.forEach(spec => {
                promise = promise.then(() => {
                    if (connection.connected) {
                        return connection.write(spec);
                    }
                    throw new Error("Connection closed");
                });
            });
            return promise.then(() => this.events.notify('Karma discovered'));
        });
    });

    requestRun = this.addCommand('requestRun',(command, message, connection) => {
        this.events.notify({
            event: 'Test run requested',
            tests: message.tests
        });
        return Q.resolve<void>(undefined);
    });
}

export = Server;
