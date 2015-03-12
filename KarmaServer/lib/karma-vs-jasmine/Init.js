var path = require('path');
function createPattern(path) {
    return {
        included: !/\.(ts|map)$/.test(path),
        served: true,
        watched: false,
        pattern: path
    };
}
var InitJasmine = (function () {
    function InitJasmine(files) {
        [
            path.join(path.dirname(require.resolve('jasmine-core')), 'jasmine-core/jasmine.js'),
            path.join(__dirname, 'Instumentation.js'),
            path.join(__dirname, 'Adapter.js'),
            path.join(__dirname, 'Boot.js'),
            path.join(__dirname, '*.ts'),
            path.join(__dirname, '*.js.map')
        ].map(createPattern).reverse().forEach(function (f) { return files.unshift(f); });
    }
    InitJasmine.$inject = ['config.files'];
    return InitJasmine;
})();
module.exports = InitJasmine;
//# sourceMappingURL=Init.js.map