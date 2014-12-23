import util = require('util');
import path = require('path');
import Util = require('./Util');
import VsConfig = require('./VsConfig');

Util.setupLogger();

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
            require('./Commands/Init').run(args.configFile);
            break;
        case "get-config":
            require('./Commands/GetConfig').run(args.config, argv.o || Util.configOutputFile);
            break;
        case "discover":
            require('./Commands/Discover').run(args.config, args.outputFile);
            break;
        case "run":
            require('./Commands/Run').run(args.config, args.outputFile, args.vsConfig, args.port);
            break;
        case "serve":
            require('./Commands/Serve').run(args.config, args.port);
            break;
        case "served-run":
            require('./Commands/ServedRun').run(args.config, args.outputFile, argv.v, args.port);
            break;
        case "args":
            console.log(util.inspect(args, { depth: null }));
            break;
        default:
            var log = Util.createLogger();
            log.error("Command " + JSON.stringify(args.command) + " not recognized");
            process.exit(1);
            break;
    }
}
