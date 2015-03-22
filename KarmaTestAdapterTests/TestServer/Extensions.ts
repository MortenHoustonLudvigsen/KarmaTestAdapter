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
        var classNameParts = [];
        if (server.testContainerName) {
            classNameParts.push(server.testContainerName);
        }
        var parts = spec.suite.slice(0);
        if (parts.length > 0) {
            classNameParts.push(parts.shift());
        }
        parts.push(spec.description);
        return [classNameParts.join('/'), parts.join(' ')]
            .filter(p => !!p)
            .map(s => s.replace(/\./g, '-'))
            .join('.');
    }

    private traitGetters: Specs.TraitGetter[] = [];
    getTraits(spec: Specs.Spec, server: Specs.Server): Specs.Trait[] {
        return this.traitGetters
            .map(getTrait => getTrait(spec, server))
            .reduce((previousTraits, currentTraits) => previousTraits.concat(currentTraits), []);
    }
} 

export = Extensions;
