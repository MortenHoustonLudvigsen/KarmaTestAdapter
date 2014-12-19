import util = require('util');
import path = require('path');
import Util = require('./Util');
import Commands = require('./Commands');
import VsConfig = require('./VsConfig');

module Cli {
    export function run() {
        var argv = require('yargs').argv;

        var args: any = {
            outputFile: argv.o || Util.outputFile,
            configFile: argv.c || Util.configFile,
            port: argv.p,
            command: argv._[0] || '',
            baseDir: Util.baseDir
        };

        args.config = Util.readConfigFile(argv.c || Util.configFile);
        args.vsConfig = VsConfig.load(argv.v);

        switch (args.command.toLowerCase()) {
            case "init":
                Commands.init(args.configFile);
                break;
            case "get-config":
                Commands.getConfig(args.config, argv.o || Util.configOutputFile);
                break;
            case "discover":
                Commands.discover(args.config, args.outputFile);
                break;
            case "run":
                Commands.run(args.config, args.outputFile, args.vsConfig, args.port);
                break;
            case "args":
                console.log(util.inspect(args, { depth: null }));
                break;
            default:
                console.error("Command " + JSON.stringify(args.command) + " not recognized");
                process.exit(1);
                break;
        }
    }
}

export = Cli;