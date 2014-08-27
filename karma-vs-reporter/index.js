var Reporter = require('./src/Reporter');
var Preprocessor = require('./src/Preprocessor');

// PUBLISH DI MODULE
module.exports = {
    'reporter:vs': ['type', Reporter],
    'preprocessor:vs': ['factory', Preprocessor]
};
//# sourceMappingURL=index.js.map
