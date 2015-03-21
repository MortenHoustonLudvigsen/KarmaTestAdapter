var path = require('path');
function getDependencies(grunt, serverPath) {
    var pkg = grunt.file.readJSON(path.join(serverPath, 'package.json'));
    var result = [];
    for (var dependency in pkg.dependencies) {
        if (pkg.dependencies.hasOwnProperty(dependency)) {
            result.push(dependency);
        }
    }
    return result;
}
function getNodeModules(grunt, serverPath) {
    var result = [];
    result.push('node_modules/JsTestAdapter/**');
    getDependencies(grunt, serverPath).forEach(function (dependency) {
        result.push('node_modules/' + dependency + '/**');
    });
    result.push('!node_modules/source-map/bench/**');
    result.push('!node_modules/source-map/build/**');
    result.push('!node_modules/source-map/test/**');
    return result;
}
module.exports = getNodeModules;
//# sourceMappingURL=GetNodeModules.js.map