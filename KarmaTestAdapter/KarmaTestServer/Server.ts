import path = require('path');
import TestServer = require('../TestServer/TestServer');
import JsonServer = require('../TestServer/JsonServer');
import Specs = require('../TestServer/Specs');
import Karma = require('./Karma');
import Q = require('q');

class Server extends TestServer {
    static $inject = ['config', 'emitter', 'logger'];

    constructor(private config: any, private emitter: Karma.EventEmitter, logger: Karma.LoggerModule) {
        super(config.vs.name, config.vs.serverPort || 0);
        this.logger = logger.create('VS Server', Karma.karma.Constants.LOG_DEBUG);

        if (config.vs.traits) {
            function mapTrait(trait: string|Specs.Trait): Specs.Trait {
                if (typeof trait === 'string') {
                    return {
                        name: 'Category',
                        value: trait
                    };
                } else {
                    return trait;
                }
            }
            var traits = config.vs.traits.map(mapTrait);
            this.loadExtensions({ getTraits: (spec, server) => traits });
        }

        if (config.vs.extensions) {
            try {
                this.loadExtensions(path.resolve(config.vs.extensions));
            } catch (e) {
                this.logger.error('Failed to load extensions from ' + config.vs.extensions + ': ' + e.message);
            }
        }

        this.on('listening',() => this.logger.info('Started - port: ' + this.address.port));
        this.start();
    }

    logger: Karma.Logger;

    onError(error: any, connection: JsonServer.Connection) {
        this.logger.error(error);
    }

    testRunStarted(): void {
        this.logger.info('Test run start');
        super.testRunStarted();
    }

    testRunCompleted(specs: Specs.Spec[]): void {
        this.logger.info('Test run complete');
        super.testRunCompleted(specs);
    }
}

export = Server;
