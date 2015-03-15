var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var boms = require('./boms');
var Logger = require('./Logger');
var Bombom = (function (_super) {
    __extends(Bombom, _super);
    function Bombom() {
        _super.apply(this, arguments);
    }
    Bombom.prototype.register = function (type, signature) {
        if (this.isRegistered(type)) {
            this.warn('BOM type "' + type + '" is already registered' + ' and will be overridden');
        }
        boms[type] = new Buffer(signature);
    };
    Bombom.prototype.unregister = function (type) {
        if (!this.isRegistered(type)) {
            this.warn('No BOM type "' + type + '" to unregister');
        }
        else {
            delete boms[type];
        }
    };
    Bombom.prototype.isRegistered = function (type) {
        return typeof boms[type] !== 'undefined';
    };
    Bombom.prototype.enforce = function (buffer, type) {
        if (!this.ensureRegisteredType(type)) {
            return buffer;
        }
        var detectedType = this.detect(buffer);
        if (detectedType === type) {
            return buffer;
        }
        var detectedSignature = boms[detectedType] || new Buffer(0);
        return this.replaceSignature(buffer, detectedSignature, boms[type]);
    };
    Bombom.prototype.ensureRegisteredType = function (type) {
        if (this.isRegistered(type)) {
            return true;
        }
        this.error(new Error('BOM type "' + type + '" is not registered'));
        return false;
    };
    Bombom.prototype.detect = function (buffer) {
        var types = Object.keys(boms);
        for (var i = 0; i < types.length; i++) {
            var type = types[i];
            if (this.isSignedWithType(buffer, type)) {
                return type;
            }
        }
        return void (0);
    };
    Bombom.prototype.isSignedWithType = function (buffer, type) {
        if (!this.ensureRegisteredType(type)) {
            return void (0);
        }
        var signature = boms[type];
        for (var i = 0; i < signature.length; i++) {
            var hex = signature[i];
            if (buffer[i] !== hex) {
                return false;
            }
        }
        return true;
    };
    Bombom.prototype.replaceSignature = function (buffer, oldSig, newSig) {
        var result = new Buffer(buffer.length - oldSig.length + newSig.length);
        newSig.copy(result);
        buffer.copy(result, newSig.length);
        return result;
    };
    Bombom.prototype.strip = function (buffer, type) {
        var signature = null;
        if (!type) {
            signature = boms[this.detect(buffer)];
        }
        else if (this.isSigned(buffer, type)) {
            signature = boms[type];
        }
        if (!signature) {
            return buffer;
        }
        return buffer.slice(signature.length);
    };
    Bombom.prototype.isSigned = function (buffer, type) {
        if (type && this.ensureRegisteredType(type)) {
            return this.isSignedWithType(buffer, type);
        }
        var keys = Object.keys(boms);
        for (var i = 0; i < keys.length; i++) {
            if (this.isSignedWithType(buffer, keys[i])) {
                return true;
            }
        }
        return false;
    };
    return Bombom;
})(Logger);
module.exports = Bombom;
