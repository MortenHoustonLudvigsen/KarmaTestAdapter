describe('Adder',() => {
    describe('add',() => {
        [
            { a: 3, b: 3, result: 6 }
        ].forEach(t => it('(' + t.a + ', ' + t.b + ') should return ' + t.result,() => expect(Adder.add(t.a, t.b)).toEqual(t.result)));
    });
}); 