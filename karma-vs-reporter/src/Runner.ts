import Globals = require('./Globals');
import Util = require('./Util');
import VsConfig = require('./VsConfig');
import http = require('http');
import Url = require('url');
import querystring = require('querystring');
import util = require('util');
import path = require('path');
import _ = require('lodash');
var extend = require('extend');
var runnerMiddleware = require('karma/lib/middleware/runner');
var runnerMiddlewareCreate = runnerMiddleware.create;
var helper = require('karma/lib/helper');
var constant = require('karma/lib/constants');
var json = require('connect').json();

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

        log.debug('Execution (fired by runner)');
        response.writeHead(200);

        if (!capturedBrowsers.length) {
            var url = 'http://' + hostname + ':' + port + urlRoot;

            return response.end('No captured browser, open ' + url + '\n');
        }

        json(request, response, function () {
            if (!capturedBrowsers.areAllReady([])) {
                response.write('Waiting for previous execution...\n');
            }

            emitter.once('run_start', function () {
                var responseWrite = response.write.bind(response);

                reporter.addAdapter(responseWrite);

                // clean up, close runner response
                emitter.once('run_complete', function (browsers, results) {
                    reporter.removeAdapter(responseWrite);
                    response.end(constant.EXIT_CODE + results.exitCode);
                });
            });

            var data = request.body;
            log.debug('Setting client.args to ', data.args);
            config.client.args = data.args;

            fileList.refresh().then(files => {
                executor.schedule();
            });
        });
    };
}

export function replaceRunner() {
    runnerMiddleware.create = create;
}




