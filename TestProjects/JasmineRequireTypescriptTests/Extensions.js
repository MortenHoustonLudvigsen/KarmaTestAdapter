exports.getDisplayName = function (spec, server) {
    var parts = spec.suite.slice(0);
    parts.push(spec.description);
    return parts.
        filter(function (p) { return !!p; }).
        join(' ');
};

//exports.getFullyQualifiedName = function (spec, server) {
//    var classNameParts = [];
//    var parts = spec.suite.slice(0);
//    if (parts.length > 0) {
//        classNameParts.push(parts.shift());
//    }
//    parts.push(spec.description);
//    return [classNameParts.join(' / '), parts.join(' ')].
//        filter(function (p) { return !!p; }).
//        map(function (s) { return s.replace(/\./g, '-'); }).
//        join('.');
//};

exports.getTraits = function (spec, server) {
    return [ { Name: 'Slam', Value: 'Lalleglad' } ];
};
