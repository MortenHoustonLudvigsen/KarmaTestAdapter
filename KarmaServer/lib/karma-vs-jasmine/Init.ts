import path = require('path');

type Pattern = {
    pattern: string;
    included: boolean;
    served: boolean;
    watched: boolean;
};

function createPattern(path: string): Pattern {
    return {
        included: !/\.(ts|map)$/.test(path),
        served: true,
        watched: false,
        pattern: path
    };
}

class InitJasmine {
    static $inject = ['config.files'];
    constructor(files: Pattern[]) {
        [
            path.join(path.dirname(require.resolve('jasmine-core')), 'jasmine-core/jasmine.js'),
            path.join(__dirname, 'Instumentation.js'),
            path.join(__dirname, 'Adapter.js'),
            path.join(__dirname, 'Boot.js'),
            path.join(__dirname, '*.ts'),
            path.join(__dirname, '*.js.map')
        ].map(createPattern).reverse().forEach(f => files.unshift(f));
    }
}

export = InitJasmine;
