var Util = require('../Util');

function run(config, outputFile) {
    Util.writeFile(outputFile, JSON.stringify(Util.getKarmaConfig(config), undefined, 4));
}
exports.run = run;
//# sourceMappingURL=GetConfig.js.map
