describe('Simple tests 2', function () {
    it('should be, that 3 + 2 = 4', function () {
        expect(3 + 2).toBe(4);
    });
    xit('should be skipped', function () {
        expect(true).toBe(false);
    });
    describe('Nested tests', function () {
        it('should be, that 3 + 12 = 23', function () {
            expect(3 + 12).toBe(23);
        });
    });
    it('should be, that 3 + 12 = 23', function () {
        expect(3 + 12).toBe(23);
    });
});
