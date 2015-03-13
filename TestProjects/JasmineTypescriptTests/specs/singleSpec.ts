module SingleSpec {
    describe('Suite 1',() => {
        describe('Suite 2',() => {
            it('Spec 1',() => {
                console.log("Spec 1 message");
                expect(2).toBe(2);
            });
            it('Spec 2',() => expect(2).toBe(2));
            it('Spec 3',() => expect(2).toBe(3));
            it('Spec 4',() => expect(undefined).toBeDefined());
        });
    });
    describe('Suite 3',() => {
        for (var i = 1; i <= 10; i++) {
            it('test no ' + i,() => expect(i).toBe(i));
        }
    });
}