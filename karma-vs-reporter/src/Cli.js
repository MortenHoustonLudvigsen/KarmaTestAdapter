var util = require('util');

var Util = require('./Util');
var Commands = require('./Commands');
var VsConfig = require('./VsConfig');

var Cli;
(function (Cli) {
    function run() {
        var argv = require('yargs').argv;

        var args = {
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
    Cli.run = run;
})(Cli || (Cli = {}));

module.exports = Cli;
//# sourceMappingURL=Cli.js.map
