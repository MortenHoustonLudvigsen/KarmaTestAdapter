import Specs = require('./Specs');
import Extensions = require('./Extensions');
import JsonClient = require('./JsonClient');
import Q = require('q');

class TestNetServer implements Specs.Server {
    constructor(public testContainerName: string, public port: number, public host?: string) {
    }

    extensions = new Extensions();

    private testRunStartedClient = new JsonClient('test run started', this.port, this.host);
    private testRunCompletedClient = new JsonClient('test run completed', this.port, this.host);

    testRunStarted(): Q.Promise<void> {
        return this.testRunStartedClient.run();
    }

    testRunCompleted(specs: Specs.Spec[]): Q.Promise<void> {
        return this.testRunCompletedClient.run({
            specs: specs
        });
    }
}

export = TestNetServer; 