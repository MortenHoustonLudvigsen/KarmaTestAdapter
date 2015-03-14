describe('Adder', function () {
    describe('add', function () {
        [
            { a: 3, b: 3, result: 6 }
        ].forEach(function (t) { return it('(' + t.a + ', ' + t.b + ') should return ' + t.result, function () { return expect(Adder.add(t.a, t.b)).toEqual(t.result); }); });
    });
});
//# sourceMappingURL=AdderSpec.js.map