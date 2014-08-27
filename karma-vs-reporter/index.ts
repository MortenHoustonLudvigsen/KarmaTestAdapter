import Reporter = require('./src/Reporter');
import Preprocessor = require('./src/Preprocessor');

// PUBLISH DI MODULE
module.exports = {
    'reporter:vs': ['type', Reporter],
    'preprocessor:vs': ['factory', Preprocessor]
};
