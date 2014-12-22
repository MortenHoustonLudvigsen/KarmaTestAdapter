import Globals = require('./Globals');
import Util = require('./Util');
import VsConfig = require('./VsConfig');
import http = require('http');
import Url = require('url');
import querystring = require('querystring');
import util = require('util');
var extend = require('extend');
var runnerMiddleware = require('karma/lib/middleware/runner');
var runnerMiddlewareCreate = runnerMiddleware.create;

interface IQueryString {
    outputFile?: string;
    vsConfig?: string;
}

function removeQueryString(url: string): string {
    var parsedUrl = Url.parse(url);
    parsedUrl.search = undefined;
    parsedUrl.query = undefined;
    return Url.format(parsedUrl);
}

function getQuery(url: string): IQueryString {
    var parsedUrl = Url.parse(url);
    return querystring.parse(parsedUrl.query);
}

function create(emitter, fileList, capturedBrowsers, reporter, executor,
    /* config.hostname */ hostname, /* config.port */ port, /* config.urlRoot */ urlRoot, config) {

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

export function replaceRunner() {
    runnerMiddleware.create = create;
}
