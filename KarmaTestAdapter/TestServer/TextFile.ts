import fs = require('fs');
var iconv = require('iconv-lite');

export function read(path: string): string {
    var buffer = fs.readFileSync(path);
    var encoding = detectBom(buffer) || 'utf8';
    return iconv.decode(stripBom(buffer), encoding);
}

export function readJson(path: string, reviver?: (key: any, value: any) => any): any {
    return JSON.parse(read(path), reviver);
}

var boms = <Object>{
    utf8: [0xEF, 0xBB, 0xBF],
    utf16le: [0xFF, 0xFE],
    utf16be: [0xFE, 0xFF],
    utf32le: [0xFF, 0xFE, 0x00, 0x00],
    utf32be: [0x00, 0x00, 0xFE, 0xFF]
};

export function detectBom(buffer: Buffer, defaultValue?: string): string {
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

export function stripBom(buffer: Buffer) {
    var encoding = detectBom(buffer);
    if (encoding) {
        return buffer.slice(boms[encoding].length);
    }
    return buffer;
}

function hasBom(buffer: Buffer, bom: number[]): boolean {
    for (var i = 0; i < bom.length; i++) {
        if (buffer[i] !== bom[i]) {
            return false;
        }
    }
    return true;
}