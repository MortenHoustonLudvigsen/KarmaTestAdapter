var path = require('path');
var InitJasmine = (function () {
    function InitJasmine(files) {
        var jasminePath = path.dirname(require.resolve('jasmine-core'));
        [
            [require.resolve('stackframe')],
            [require.resolve('error-stack-parser')],
            [jasminePath, 'jasmine-core/jasmine.js'],
            [__dirname, 'Instumentation.js'],
            [__dirname, 'Boot.js'],
            [__dirname, 'Adapter.js']
        ].map(function (paths) { return {
            pattern: path.join.apply(path, paths),
            included: true,
            served: true,
            watched: true
        }; }).reverse().forEach(function (f) { return files.unshift(f); });
    }
    InitJasmine.$inject = ['config.files'];
    return InitJasmine;
})();
module.exports = InitJasmine;
//# sourceMappingURL=Init.js.map