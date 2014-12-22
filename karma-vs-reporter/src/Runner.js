var Globals = require('./Globals');
var Util = require('./Util');
var VsConfig = require('./VsConfig');

var Url = require('url');
var querystring = require('querystring');

var extend = require('extend');
var runnerMiddleware = require('karma/lib/middleware/runner');
var runnerMiddlewareCreate = runnerMiddleware.create;

function removeQueryString(url) {
    var parsedUrl = Url.parse(url);
    parsedUrl.search = undefined;
    parsedUrl.query = undefined;
    return Url.format(parsedUrl);
}

function getQuery(url) {
    var parsedUrl = Url.parse(url);
    return querystring.parse(parsedUrl.query);
}

function create(emitter, fileList, capturedBrowsers, reporter, executor, /* config.hostname */ hostname, /* config.port */ port, /* config.urlRoot */ urlRoot, config) {
    var logger = require('karma/lib/logger');
    var log = Util.createLogger(logger);

    var handler = runnerMiddlewareCreate(emitter, fileList, capturedBrowsers, reporter, executor, hostname, port, urlRoot, config);

    return function (request, response, next) {
        var runUrl = removeQueryString(request.url);
        if (runUrl !== '/__run__' && runUrl !== urlRoot + 'run') {
            return next();
        }

        var query = getQuery(request.url);
        if (!query.outputFile || !query.vsConfig) {
            log.error("Missing query parameter outputFile or vsConfig");
            return next();
        }

        Globals.Configure({
            outputFile: query.outputFile,
            vsConfig: VsConfig.load(query.vsConfig)
        });

        fileList.reload(Globals.vsConfig.getFiles(config.basePath), []);
        return handler(extend({}, request, { url: runUrl }), response, next);
    };
}

function replaceRunner() {
    runnerMiddleware.create = create;
}
exports.replaceRunner = replaceRunner;
//# sourceMappingURL=Runner.js.map
