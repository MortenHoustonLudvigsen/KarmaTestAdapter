import fs = require('fs');
import path = require('path');
import TextFile = require('node_modules/JsTestAdapter/TextFile');

describe('TextFile',() => {
    var expected = '{ "Some Danish letters for good measure": "æøåÆØÅ" }';
    var expectedJson = JSON.parse(expected);
    var fixturesDir = '../KarmaTestAdapterTests/PathUtils/ReadFileTextFixtures';
    var fixtures = fs.readdirSync(fixturesDir).map(f => path.join(fixturesDir, f));

    fixtures.forEach(fixture => {
        it("read('" + fixture + "') should read file",() => {
            expect(TextFile.read(fixture)).toEqual(expected);
        });
    });

    fixtures.forEach(fixture => {
        it("readJson('" + fixture + "') should read JSON from file",() => {
            expect(TextFile.readJson(fixture)).toEqual(expectedJson);
        });
    });
});