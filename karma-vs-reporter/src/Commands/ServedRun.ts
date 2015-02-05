import Util = require('../Util');
import Globals = require('../Globals');
import VsConfig = require('../VsConfig');
import querystring = require('querystring');
import http = require('http');
import _ = require('lodash');

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

export function run(config: Util.Config, outputFile: string, vsConfigFile: string, port?) {
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
