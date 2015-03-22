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
        var classNameParts = [];
        if (server.testContainerName) {
            classNameParts.push(server.testContainerName);
        }
        var parts = spec.suite.slice(0);
        if (parts.length > 0) {
            classNameParts.push(parts.shift());
        }
        parts.push(spec.description);
        return [classNameParts.join('/'), parts.join(' ')].filter(function (p) { return !!p; }).map(function (s) { return s.replace(/\./g, '-'); }).join('.');
    };
    Extensions.prototype.getTraits = function (spec, server) {
        return this.traitGetters.map(function (getTrait) { return getTrait(spec, server); }).reduce(function (previousTraits, currentTraits) { return previousTraits.concat(currentTraits); }, []);
    };
    return Extensions;
})();
module.exports = Extensions;
//# sourceMappingURL=Extensions.js.map