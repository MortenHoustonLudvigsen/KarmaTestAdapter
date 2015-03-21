var fs = require('fs');
var iconv = require('iconv-lite');
function read(path) {
    var buffer = fs.readFileSync(path);
    var encoding = detectBom(buffer) || 'utf8';
    return iconv.decode(stripBom(buffer), encoding);
}
exports.read = read;
function readJson(path, reviver) {
    return JSON.parse(read(path), reviver);
}
exports.readJson = readJson;
var boms = {
    utf8: [0xEF, 0xBB, 0xBF],
    utf16le: [0xFF, 0xFE],
    utf16be: [0xFE, 0xFF],
    utf32le: [0xFF, 0xFE, 0x00, 0x00],
    utf32be: [0x00, 0x00, 0xFE, 0xFF]
};
function detectBom(buffer, defaultValue) {
    for (var encoding in boms) {
        if (boms.hasOwnProperty(encoding)) {
            var bom = boms[encoding];
            if (hasBom(buffer, bom)) {
                return encoding;
            }
        }
    }
    return defaultValue;
}
exports.detectBom = detectBom;
function stripBom(buffer) {
    var encoding = detectBom(buffer);
    if (encoding) {
        return buffer.slice(boms[encoding].length);
    }
    return buffer;
}
exports.stripBom = stripBom;
function hasBom(buffer, bom) {
    for (var i = 0; i < bom.length; i++) {
        if (buffer[i] !== bom[i]) {
            return false;
        }
    }
    return true;
}
//# sourceMappingURL=TextFile.js.map