var util = require('util');
var TestContext = (function () {
    function TestContext(server) {
        this.server = server;
        this.results = [];
        this.nextErrorId = 0;
        this.fullyQualifiedNames = {};
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
    TestContext.prototype.getNewErrorId = function () {
        return util.format('-error-%d', this.nextErrorId++);
    };
    TestContext.prototype.getFullyQualifiedName = function (spec) {
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
    };
    TestContext.prototype.getDisplayName = function (spec) {
        return this.server.extensions.getDisplayName(spec, this.server);
    };
    TestContext.prototype.getTraits = function (spec) {
        return this.server.extensions.getTraits(spec, this.server);
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