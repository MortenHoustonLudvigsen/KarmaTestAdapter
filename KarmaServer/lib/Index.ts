module.exports = {
    'karma-vs-server': ['type', require('./Server')],
    'reporter:vs': ['type', require('./Reporter')],
    'framework:vs-jasmine': ['factory', require('./karma-vs-jasmine/Init')],
};

 
