var path = require('path');
var Karma = require('../../Karma');
function createPattern(path) {
    return {
        pattern: path.replace(/\\/g, '/'),
        included: !/\.(ts|map)$/.test(path),
        served: true,
        watched: false
    };
}
var InitJasmine = (function () {
    function InitJasmine(files, karmaLogger) {
        var logger = karmaLogger.create('VS Jasmine', Karma.karma.Constants.LOG_DEBUG);
        var patterns = [
            path.join(path.dirname(require.resolve('jasmine-core')), 'jasmine-core/jasmine.js'),
            path.join(__dirname, 'instumentation.js'),
            path.join(__dirname, 'boot.js'),
            path.join(__dirname, 'adapter.js'),
            path.join(__dirname, '*.ts'),
            path.join(__dirname, '*.js.map')
        ].map(createPattern);
        patterns.reverse().forEach(function (f) { return files.unshift(f); });
        logger.info("Ready");
    }
    InitJasmine.$inject = ['config.files', 'logger'];
    return InitJasmine;
})();
module.exports = InitJasmine;
//# sourceMappingURL=index.js.map