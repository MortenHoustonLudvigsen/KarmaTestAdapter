import Util = require('../Util');

export function run(config: Util.Config, outputFile: string) {
    Util.writeFile(outputFile, JSON.stringify(Util.getKarmaConfig(config), undefined, 4));
}
