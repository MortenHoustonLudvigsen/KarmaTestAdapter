interface Options {
    pre?: (node: any, parent?: any, prop?: any, idx?: number) => any;
    post?: (node: any, parent?: any, prop?: any, idx?: number) => any;
    skipProperty?: (prop: any, node: any) => boolean;
}

declare function traverse(ast: any, options: Options): void;

declare module 'ast-traverse' {
    export = traverse;
}
