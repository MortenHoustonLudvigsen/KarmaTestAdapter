var Util = require('../Util');

var querystring = require('querystring');
var http = require('http');

function parseExitCode(buffer, defaultCode) {
    var constants = require('karma/lib/constants');
    var tailPos = buffer.length - Buffer.byteLength(constants.EXIT_CODE) - 1;

    if (tailPos < 0) {
        return defaultCode;
    }

    // tail buffer which might contain the message
    var tail = buffer.slice(tailPos);
    var tailStr = tail.toString();
    if (tailStr.substr(0, tailStr.length - 1) === constants.EXIT_CODE) {
        tail.fill('\x00');
        return parseInt(tailStr.substr(-1), 10);
    }

    return defaultCode;
}

//var callServer: any = function (config, outputFile, vsConfigFile) {
//    var log = Util.createLogger();
//    var exitCode = 1;
//    var options = {
//        hostname: config.hostname,
//        path: config.urlRoot + 'run?' + querystring.stringify({ outputFile: outputFile, vsConfig: vsConfigFile }),
//        port: config.port,
//        method: 'POST',
//        headers: { 'Content-Type': 'application/json' }
//    };
//    var request = http.request(options, function (response) {
//        response.on('data', function (buffer) {
//            exitCode = parseExitCode(buffer, exitCode);
//            process.stdout.write(buffer);
//        });
//        response.on('end', function () {
//            process.exit(exitCode);
//        });
//    });
//    request.on('error', function (e) {
//        if (e.code === 'ECONNREFUSED') {
//            console.error('There is no server listening on port %d', options.port);
//            process.exit(1);
//        } else {
//            throw e;
//        }
//    });
//    request.end(JSON.stringify({}));
//}
//callServer.$inject = ['config', 'outputFile', 'vsConfigFile'];
//export function run(config: Util.Config, outputFile: string, vsConfigFile: string, port?) {
//    Globals.logTests = false;
//    var di = require('karma/node_modules/di');
//    new di.Injector([{
//        config: ['value', Util.getKarmaConfig(config, { port: port })],
//        emitter: ['type', require('karma/lib/events').EventEmitter],
//        logger: ['value', require('karma/lib/logger')],
//        preprocess: ['factory', require('karma/lib/preprocessor').createPreprocessor],
//        fileList: ['type', require('karma/lib/file_list').List],
//        outputFile: ['value', outputFile],
//        vsConfigFile: ['value', vsConfigFile]
//    }]).invoke(callServer);
//}
function run(config, outputFile, vsConfigFile, port) {
    var karmaConfig = Util.getKarmaConfig(config, {
        port: port
    });

    var exitCode = 1;
    var options = {
        hostname: karmaConfig.hostname,
        path: karmaConfig.urlRoot + 'run?' + querystring.stringify({ outputFile: outputFile, vsConfig: vsConfigFile }),
        port: karmaConfig.port,
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    };

    var request = http.request(options, function (response) {
        response.on('data', function (buffer) {
            exitCode = parseExitCode(buffer, exitCode);
            process.stdout.write(buffer);
        });

        response.on('end', function () {
            process.exit(exitCode);
        });
    });

    request.on('error', function (e) {
        if (e.code === 'ECONNREFUSED') {
            console.error('There is no server listening on port %d', options.port);
            process.exit(1);
        } else {
            throw e;
        }
    });

    request.end(JSON.stringify({}));
}
exports.run = run;
//# sourceMappingURL=ServedRun.js.map
