import util = require('util');
import Specs = require('./Specs');

type Event = {
    description?: string;
    suite?: string[];
};

class TestContext {
    constructor(private server: Specs.Server) {
    }

    results: Specs.SpecResult[] = [];
    nextErrorId = 0;
    fullyQualifiedNames: { [name: string]: boolean } = {};
    totalTime: number = 0;
    startTime: number;
    endTime: number;
    timesValid = true;

    addResult(spec: Specs.Spec, result: Specs.SpecResult): void {
        this.results.push(result);

        if (this.timesValid && typeof result.startTime === 'number' && typeof result.endTime === 'number') {
            this.totalTime += result.time;

            this.startTime = typeof this.startTime === 'number'
                ? Math.min(this.startTime, result.startTime) : result.startTime;

            this.endTime = typeof this.endTime === 'number'
                ? Math.max(this.endTime, result.endTime) : result.endTime;
        } else {
            this.timesValid = false;
        }
    }

    getNewErrorId(): string {
        return util.format('-error-%d', this.nextErrorId++);
    }

    getFullyQualifiedName(spec: Specs.Spec): string {
        var fullyQualifiedName = this.server.extensions.getFullyQualifiedName(spec, this.server);
        if (this.fullyQualifiedNames[fullyQualifiedName]) {
            var no = 2;
            while (this.fullyQualifiedNames[fullyQualifiedName + '-' + no]) {
                no += 1;
            }
            fullyQualifiedName = fullyQualifiedName + '-' + no;
            this.fullyQualifiedNames[fullyQualifiedName] = true;
        }
        return fullyQualifiedName;
    }

    getDisplayName(spec: Specs.Spec): string {
        return this.server.extensions.getDisplayName(spec, this.server);
    }

    getTraits(spec: Specs.Spec): Specs.Trait[] {
        return this.server.extensions.getTraits(spec, this.server);
    }

    adjustResults() {
        this.adjustTimes();
    }

    adjustTimes() {
        if (this.timesValid && typeof this.startTime === 'number' && typeof this.endTime === 'number') {
            var diff = ((this.endTime - this.startTime) - this.totalTime) / this.results.length;
            this.results.forEach(result => result.time = Math.max(0.01, result.time + diff));
        }
    }
}

export = TestContext;