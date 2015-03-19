import path = require('path');

function getContentTypes(grunt: any, directory: string): string {
    var extensions = {
        'vsixmanifest': 'text/xml',
        'xml': 'text/xml',
        'js': 'application/javascript',
        'json': 'application/json',
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
        } else {
            file = '/' + file;
            if (!overrides[file]) {
                overrides[file] = 'application/octet-stream';
            }
        }
    });

    var contentTypes = [];

    for (var extension in extensions) {
        if (extensions.hasOwnProperty(extension)) {
            contentTypes.push('<Default Extension="' + extension + '" ContentType="' + extensions[extension] + '"/>');
        }
    }

    for (var override in overrides) {
        if (overrides.hasOwnProperty(override)) {
            contentTypes.push('<Override PartName="' + override + '" ContentType="' + overrides[override] + '"/>');
        }
    }

    return contentTypes.join('\n');
}

export = getContentTypes;
