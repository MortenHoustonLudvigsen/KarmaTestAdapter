exports.getDisplayName = function (spec, server) {
    return spec.suite.join('.') + ' ' + spec.description;
};

exports.getFullyQualifiedName = function (spec, server) {
    var parts = [];
    // Add server.testContainerName to ensure uniqueness
    if (server.testContainerName) {
        parts.push(server.testContainerName);
    }
    // To ensure that the test explorer sees all suites as one identifier, we
    // separate suites with '-', not '.'
    parts.push(spec.suite.join('-'));
    // Add the test description
    parts.push(spec.description);
    // The fully qualified name is the parts separated by '.'. We make sure that
    // the parts do not contain '.', as this would confuse the test explorer
    return parts.map(function (s) { return s.replace(/\./g, '-'); }).join('.');
};

exports.getTraits = function (spec, server) {
    var traits = spec.traits;
    //traits = [];
    var outerSuite = spec.suite[0];
    if (outerSuite) {
        traits.push({
            name: 'The Suite',
            value: outerSuite
        });
    }
    return traits;
};
