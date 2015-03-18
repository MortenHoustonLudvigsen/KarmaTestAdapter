var fs = require('fs');
var path = require('path');
var TextFile = require('node_modules/JsTestAdapter/TextFile');
describe('TextFile', function () {
    var expected = '{ "Some Danish letters for good measure": "æøåÆØÅ" }';
    var expectedJson = JSON.parse(expected);
    var fixturesDir = '../KarmaTestAdapterTests/PathUtils/ReadFileTextFixtures';
    var fixtures = fs.readdirSync(fixturesDir).map(function (f) { return path.join(fixturesDir, f); });
    fixtures.forEach(function (fixture) {
        it("read('" + fixture + "') should read file", function () {
            expect(TextFile.read(fixture)).toEqual(expected);
        });
    });
    fixtures.forEach(function (fixture) {
        it("readJson('" + fixture + "') should read JSON from file", function () {
            expect(TextFile.readJson(fixture)).toEqual(expectedJson);
        });
    });
});
//# sourceMappingURL=TextFileSpec.js.map