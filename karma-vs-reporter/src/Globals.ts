import VsConfig = require('./VsConfig');

module Globals {
    export var origConfig: any;
    export var outputFile: string;
    export var vsConfig = new VsConfig.Config({});
    export var logTests = true;

    export function Configure(config: { outputFile?: string; vsConfig?: VsConfig.Config }): void {
        outputFile = config.outputFile;
        vsConfig = config.vsConfig;
    }
}

export = Globals;
