var Adder;
(function (Adder) {
    function add(a, b) {
        expect(1).toBe(2);
        return a + b;
    }
    Adder.add = add;
})(Adder || (Adder = {}));
//# sourceMappingURL=Adder.js.map