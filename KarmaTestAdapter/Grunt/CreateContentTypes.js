var path = require('path');
var builder = require('xmlbuilder');
function createContentTypes(grunt, directory, fileName) {
    var types = require('xmlbuilder').create('Types', { version: '1.0', encoding: 'utf-8' });
    types.att('xmlns', 'http://schemas.openxmlformats.org/package/2006/content-types');
    var extensions = {
        'vsixmanifest': 'text/xml',
        'xml': 'text/xml',
        'js': 'application/javascript',
        'jsm': 'application/javascript',
        'ts': 'application/typescript',
        'json': 'application/json',
        'json5': 'application/json',
        'hbs': 'text/plain',
        'map': 'text/plain',
        'md': 'text/plain',
        'markdown': 'text/plain',
        'txt': 'text/plain'
    };
    var overrides = {};
    grunt.file.expand({ filter: 'isFile', cwd: directory }, ['**']).forEach(function (file) {
        var extension = path.extname(file);
        if (extension) {
            extension = extension.replace(/^\./, '');
            if (!extensions[extension]) {
                extensions[extension] = 'application/octet-stream';
            }
        }
        else {
            file = '/' + file;
            if (!overrides[file]) {
                overrides[file] = 'application/octet-stream';
            }
        }
    });
    for (var extension in extensions) {
        if (extensions.hasOwnProperty(extension)) {
            types.ele('Default', {
                'Extension': extension,
                'ContentType': extensions[extension]
            });
        }
    }
    for (var override in overrides) {
        if (overrides.hasOwnProperty(override)) {
            types.ele('Override', {
                'PartName': override,
                'ContentType': overrides[override]
            });
        }
    }
    grunt.file.write(fileName, types.end({
        pretty: true,
        indent: '    ',
        newline: '\n'
    }));
}
module.exports = createContentTypes;
//# sourceMappingURL=CreateContentTypes.js.map