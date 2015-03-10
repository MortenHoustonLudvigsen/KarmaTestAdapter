//export var Constants: KarmaConstants = require('karma/lib/constants');
//export var KarmaLogger: LoggerModule = require('karma/lib/logger');
exports.karma = {
    Constants: require('karma/lib/constants'),
    Logger: require('karma/lib/logger'),
    Config: require('karma/lib/config'),
    Server: require('karma').server
};
//# sourceMappingURL=Karma.js.map