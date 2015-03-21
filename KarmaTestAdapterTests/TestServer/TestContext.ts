import Specs = require('./Specs');

type Event = {
    description?: string;
    suite?: string[];
};

class TestContext {
    constructor() {
    }

    results: Specs.SpecResult[] = [];
    uniqueNames: { [name: string]: boolean } = {};
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

    getUniqueName(suite: string[], description: string): string;
    getUniqueName(event: Event): string;
    getUniqueName(eventOrSuite: Event | string[], description?: string): string {
        if (eventOrSuite instanceof Array) {
            var suite = <string[]>eventOrSuite;
            var uniqueName = suite.map(name => {
                name = name.replace(/\./g, '-');
            }).join(' / ') + '.' + description;

            if (this.uniqueNames[uniqueName]) {
                var no = 2;
                while (this.uniqueNames[uniqueName + '-' + no]) {
                    no += 1;
                }
                uniqueName = uniqueName + '-' + no;
            }

            return uniqueName;
        } else {
            var event = <Event>eventOrSuite;
            return this.getUniqueName(event.suite, event.description);
        }
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