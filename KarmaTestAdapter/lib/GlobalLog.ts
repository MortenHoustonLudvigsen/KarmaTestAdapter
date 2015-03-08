import fs = require('fs');
import path = require('path');
import Karma = require('./Karma');

var logFile = path.join(process.env.LOCALAPPDATA, '2PS/KarmaTestAdapter/KarmaServer.log');
var appenders = [
    Karma.karma.Constants.CONSOLE_APPENDER,
    { type: 'file', filename: logFile }
];

fs.writeFileSync(logFile, "", { encoding: 'utf8' });

Karma.karma.Logger.setup('INFO', false, appenders);

var logger = Karma.karma.Logger.create('', Karma.karma.Constants.LOG_DEBUG);

logger.appenders = appenders;
logger.setup = () => Karma.karma.Logger.setup('INFO', false, appenders);

export = logger;
