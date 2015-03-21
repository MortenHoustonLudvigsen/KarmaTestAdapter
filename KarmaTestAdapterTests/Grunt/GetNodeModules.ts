import path = require('path');

function getDependencies(grunt: any, serverPath: string): string[] {
    var pkg = grunt.file.readJSON(path.join(serverPath, 'package.json'));
    var result = [];
    for (var dependency in pkg.dependencies) {
        if (pkg.dependencies.hasOwnProperty(dependency)) {
            result.push(dependency);
        }
    }
    return result;
}

function getNodeModules(grunt: any, serverPath: string): string[] {
    var result: string[] = [];
    result.push('node_modules/JsTestAdapter/**');
    getDependencies(grunt, serverPath).forEach(function (dependency) {
        result.push('node_modules/' + dependency + '/**');
    });
    result.push('!node_modules/source-map/bench/**');
    result.push('!node_modules/source-map/build/**');
    result.push('!node_modules/source-map/test/**');
    return result;
}

export = getNodeModules;
