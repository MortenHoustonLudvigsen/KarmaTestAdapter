describe('Simple tests', function () {
    it('should be, that 1 + 1 = 2', function () {
        expect(1 + 1).toBe(2);
    });

    it("should fail", function (done) {
        setTimeout(function () {
            expect(1).toEqual(10);
            done();
        });
    });

    it("should also fail", function (done) {
        setTimeout(function () {
            expect(1).toEqual(10);
            done();
        });
    });

    describe('Lal', function () {
        it('should succeed', function () {
            console.log("Where do I go?");
            expect(1).toEqual(10);
        });
    });
});
//# sourceMappingURL=TestFile1.js.map
