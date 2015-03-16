import path = require('path');
import Karma = require('../../Karma');

type Pattern = {
    pattern: string;
    included: boolean;
    served: boolean;
    watched: boolean;
};

function createPattern(path: string): Pattern {
    return {
        pattern: path.replace(/\\/g, '/'),
        included: !/\.(ts|map)$/.test(path),
        served: true,
        watched: false
    };
}

class InitJasmine {
    static $inject = ['config.files', 'logger'];
    constructor(files: Pattern[], karmaLogger: Karma.LoggerModule) {
        var logger = karmaLogger.create('VS Jasmine', Karma.karma.Constants.LOG_DEBUG);
        var patterns = [
            path.join(path.dirname(require.resolve('jasmine-core')), 'jasmine-core/jasmine.js'),
            path.join(__dirname, 'instumentation.js'),
            path.join(__dirname, 'boot.js'),
            path.join(__dirname, 'adapter.js'),
            path.join(__dirname, '*.ts'),
            path.join(__dirname, '*.js.map')
        ].map(createPattern);
        patterns.reverse().forEach(f => files.unshift(f));
        logger.info("Ready");
    }
}

export = InitJasmine;
