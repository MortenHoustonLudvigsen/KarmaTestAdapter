import Specs = require('./Specs');

class Extensions implements Specs.Extensions {
    load(extensionsModule: string|Specs.Extensions) {
        function ifFn(fn: any, defaultFn: any): any {
            return typeof fn === 'function' ? fn : defaultFn;
        }

        var extensions: Specs.Extensions;

        if (typeof extensionsModule === 'string') {
            extensions = require(extensionsModule);
        } else {
            extensions = extensionsModule;
        }

        if (extensions) {
            this.getDisplayName = ifFn(extensions.getDisplayName, this.getDisplayName);
            this.getFullyQualifiedName = ifFn(extensions.getFullyQualifiedName, this.getFullyQualifiedName);
            if (extensions.getTraits) {
                this.traitGetters.push(extensions.getTraits);
            }
        }
    }

    getDisplayName(spec: Specs.Spec, server: Specs.Server): string {
        var parts = spec.suite.slice(0);
        parts.push(spec.description);
        return parts
            .filter(p => !!p)
            .join(' ');
    }

    getFullyQualifiedName(spec: Specs.Spec, server: Specs.Server): string {
        var parts = [];
        if (server.testContainerName) {
            parts.push(server.testContainerName);
        }
        var suite = spec.suite.slice(0);
        parts.push(suite.shift());
        suite.push(spec.description);
        parts.push(suite.join(' '));
        return parts
            .filter(p => !!p)
            .map(s => s.replace(/\./g, '-'))
            .join('.');
    }

    private traitGetters: Specs.TraitGetter[] = [];
    getTraits(spec: Specs.Spec, server: Specs.Server): Specs.Trait[]{
        spec.traits = [];
        this.traitGetters.forEach(getTrait => spec.traits = getTrait(spec, server));
        return spec.traits;
    }
} 

export = Extensions;
