var VsConfig = require('./VsConfig');

var Globals;
(function (Globals) {
    Globals.origConfig;
    Globals.outputFile;
    Globals.vsConfig = new VsConfig.Config({});
    Globals.logTests = true;

    function Configure(config) {
        Globals.outputFile = config.outputFile;
        Globals.vsConfig = config.vsConfig;
    }
    Globals.Configure = Configure;
})(Globals || (Globals = {}));

module.exports = Globals;
//# sourceMappingURL=Globals.js.map
