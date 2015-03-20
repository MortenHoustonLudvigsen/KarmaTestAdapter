var TestContext = (function () {
    function TestContext(context) {
        this.context = context;
        this.results = [];
        this.uniqueNames = {};
        this.totalTime = 0;
        this.timesValid = true;
    }
    TestContext.prototype.addResult = function (spec, result) {
        this.results.push(result);
        if (this.timesValid && typeof result.startTime === 'number' && typeof result.endTime === 'number') {
            this.totalTime += result.time;
            this.startTime = typeof this.startTime === 'number' ? Math.min(this.startTime, result.startTime) : result.startTime;
            this.endTime = typeof this.endTime === 'number' ? Math.max(this.endTime, result.endTime) : result.endTime;
        }
        else {
            this.timesValid = false;
        }
    };
    TestContext.prototype.getUniqueName = function (eventOrSuite, description) {
        if (eventOrSuite instanceof Array) {
            var suite = eventOrSuite;
            var uniqueName = suite.map(function (name) {
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
        }
        else {
            var event = eventOrSuite;
            return this.getUniqueName(event.suite, event.description);
        }
    };
    TestContext.prototype.adjustResults = function () {
        this.adjustTimes();
    };
    TestContext.prototype.adjustTimes = function () {
        if (this.timesValid && typeof this.startTime === 'number' && typeof this.endTime === 'number') {
            var diff = ((this.endTime - this.startTime) - this.totalTime) / this.results.length;
            this.results.forEach(function (result) { return result.time = Math.max(0.01, result.time + diff); });
        }
    };
    return TestContext;
})();
module.exports = TestContext;
//# sourceMappingURL=TestContext.js.map