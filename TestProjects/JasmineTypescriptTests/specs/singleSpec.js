var SingleSpec;
(function (SingleSpec) {
    describe('Suite 1', function () {
        describe('Suite 2', function () {
            it('Spec 1', function () {
                console.log("Spec 1 message");
                expect(2).toBe(2);
            });
            it('Spec 2', function () { return expect(2).toBe(2); });
            it('Spec 3', function () { return expect(2).toBe(3); });
            it('Spec 4', function () { return expect(undefined).toBeDefined(); });
        });
    });
    describe('Suite 3', function () {
        for (var i = 1; i <= 10; i++) {
            it('test no ' + i, function () { return expect(i).toBe(i); });
        }
    });
})(SingleSpec || (SingleSpec = {}));
//# sourceMappingURL=singleSpec.js.map