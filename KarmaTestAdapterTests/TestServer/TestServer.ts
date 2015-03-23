import JsonConnection = require('./JsonConnection');
import JsonServer = require('./JsonServer');
import Specs = require('./Specs');
import Extensions = require('./Extensions');
import Q = require('q');

class TestServer extends JsonServer implements Specs.Server {
    constructor(public testContainerName: string, public port: number = 0, public host?: string) {
        super(port, host);
    }

    extensions = new Extensions();
    events: Q.Deferred<void> = Q.defer<void>();
    specs: Q.Deferred<Specs.Spec[]> = Q.defer<Specs.Spec[]>();

    loadExtensions(extensionsModule: string|Specs.Extensions) {
        this.extensions.load(extensionsModule);
    }

    onError(error: any, connection: JsonConnection) {
    }

    onClose(had_error: boolean, connection: JsonConnection) {
    }

    testRunStarted(): void {
        if (this.specs.promise.isFulfilled()) {
            this.specs = Q.defer<Specs.Spec[]>();
        }
        this.events.notify('Test run start');
    }

    testRunCompleted(specs: Specs.Spec[]): void {
        this.events.notify('Test run complete');
        this.specs.resolve(specs);
    }

    testRunStartedCommand = this.addCommand('test run started',(command, message, connection) => {
        this.testRunStarted();
        return Q.resolve<void>(undefined);
    });

    testRunCompletedCommand = this.addCommand('test run completed',(command, message, connection) => {
        this.testRunCompleted(message.specs);
        return Q.resolve<void>(undefined);
    });

    eventCommand = this.addCommand('event',(command, message, connection) => {
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

    stopCommand = this.addCommand('stop',(command, message, connection) => {
        this.events.resolve(undefined);
        var requests: Q.Promise<void>[] = Array.prototype.concat.call(
            this.eventCommand.getRequests(),
            this.discoverCommand.getRequests()
            );
        return Q.all(requests)
            .then(() => process.exit(0));
    });

    discoverCommand = this.addCommand('discover',(command, message, connection) => {
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
            return promise.then(() => this.events.notify('Tests discovered'));
        });
    });

    requestRunCommand = this.addCommand('requestRun',(command, message, connection) => {
        this.events.notify({
            event: 'Test run requested',
            tests: message.tests
        });
        return Q.resolve<void>(undefined);
    });
}

export = TestServer;