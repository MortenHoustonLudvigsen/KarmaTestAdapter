var Extensions = (function () {
    function Extensions() {
        this.traitGetters = [];
    }
    Extensions.prototype.load = function (extensionsModule) {
        function ifFn(fn, defaultFn) {
            return typeof fn === 'function' ? fn : defaultFn;
        }
        var extensions;
        if (typeof extensionsModule === 'string') {
            extensions = require(extensionsModule);
        }
        else {
            extensions = extensionsModule;
        }
        if (extensions) {
            this.getDisplayName = ifFn(extensions.getDisplayName, this.getDisplayName);
            this.getFullyQualifiedName = ifFn(extensions.getFullyQualifiedName, this.getFullyQualifiedName);
            if (extensions.getTraits) {
                this.traitGetters.push(extensions.getTraits);
            }
        }
    };
    Extensions.prototype.getDisplayName = function (spec, server) {
        var parts = spec.suite.slice(0);
        parts.push(spec.description);
        return parts.filter(function (p) { return !!p; }).join(' ');
    };
    Extensions.prototype.getFullyQualifiedName = function (spec, server) {
        var parts = [];
        if (server.testContainerName) {
            parts.push(server.testContainerName);
        }
        var suite = spec.suite.slice(0);
        parts.push(suite.shift());
        suite.push(spec.description);
        parts.push(suite.join(' '));
        return parts.filter(function (p) { return !!p; }).map(function (s) { return s.replace(/\./g, '-'); }).join('.');
    };
    Extensions.prototype.getTraits = function (spec, server) {
        spec.traits = [];
        this.traitGetters.forEach(function (getTrait) { return spec.traits = getTrait(spec, server); });
        return spec.traits;
    };
    return Extensions;
})();
module.exports = Extensions;
//# sourceMappingURL=Extensions.js.map