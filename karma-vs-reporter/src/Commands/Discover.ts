import Globals = require('../Globals');
import Util = require('../Util');
import VsConfig = require('../VsConfig');
import Test = require('../Test');
import parseFiles = require('../ParseFiles');
import _ = require('lodash');

var discoverTests: any = function (fileList, config) {
    var log = Util.createLogger();
    try {
        fileList.refresh().then(function (files) {
            try {
                var karma = new Test.Karma(new Date());
                karma.add(new Test.KarmaConfig(config));
                parseFiles(karma, files, log);
                karma.end = new Date();
                var xml = karma.toXml();
                Util.writeFile(Globals.outputFile, karma.toXml());
            } catch (e) {
                log.error(e);
            }
        });
    } catch (e) {
        log.error(e);
    }
}
discoverTests.$inject = ['fileList', 'config'];

export function run(config: Util.Config, outputFile: string) {
    Globals.Configure({ outputFile: outputFile });
    var di = require('di');
    new di.Injector([{
        emitter: ['type', require('karma/lib/events').EventEmitter],
        config: ['value', Util.getKarmaConfig(config)],
        preprocess: ['factory', require('karma/lib/preprocessor').createPreprocessor],
        fileList: ['type', require('karma/lib/file_list').List]
    }]).invoke(discoverTests);
}

