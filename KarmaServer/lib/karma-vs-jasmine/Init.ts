import path = require('path');

type File = {
    pattern: string;
    included: boolean;
    served: boolean;
    watched: boolean;
};

class InitJasmine {
    static $inject = ['config.files'];
    constructor(files: File[]) {
        var jasminePath = path.dirname(require.resolve('jasmine-core'));
        [
            [require.resolve('stackframe')],
            [require.resolve('error-stack-parser')],
            [jasminePath, 'jasmine-core/jasmine.js'],
            [__dirname, 'Instumentation.js'],
            [__dirname, 'Boot.js'],
            [__dirname, 'Adapter.js']
        ].map(paths => <File>{
            pattern: path.join.apply(path, paths),
            included: true,
            served: true,
            watched: true
        }).reverse().forEach(f => files.unshift(f));
    }
}

export = InitJasmine;
