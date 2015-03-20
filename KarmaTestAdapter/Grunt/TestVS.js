var util = require('util');
var path = require('path');
var cp = require('child_process');
var Q = require('q');
var format = require("string-template");
var regedit = require('regedit');
function compareVsVersions(v1, v2) {
    var v1Parts = v1.split(/\./g);
    var v2Parts = v2.split(/\./g);
    var len = Math.min(v1Parts.length, v2Parts.length);
    for (var i = 0; i < len; i++) {
        var v1Part = Number(v1Parts[i]);
        var v2Part = Number(v2Parts[i]);
        if (v1Part < v2Part) {
            return -1;
        }
        if (v1Part > v2Part) {
            return 1;
        }
    }
    if (v1Parts.length === v2Parts.length) {
        return 0;
    }
    if (v1Parts.length < v2Parts.length) {
        return -1;
    }
    else {
        return 1;
    }
}
function getRegistryKey(key, arch) {
    var list;
    switch (arch) {
        case 32:
            list = regedit.arch.list32;
            break;
        case 64:
            list = regedit.arch.list64;
            break;
        default:
            list = regedit.list;
            break;
    }
    var deferred = Q.defer();
    try {
        list([key], function (err, result) {
            if (err)
                return deferred.reject(err);
            deferred.resolve(result[key]);
        });
    }
    catch (err) {
        deferred.reject(err);
    }
    return deferred.promise;
}
exports.getRegistryKey = getRegistryKey;
function getInstallations() {
    var deferred = Q.defer();
    try {
        getRegistryKey('HKCU\\Software\\Microsoft\\VisualStudio').then(function (regKey) {
            var versions = regKey.keys.filter(function (k) { return /^\d+\.\d+$/.test(k); }).filter(function (k) { return compareVsVersions(k, '12.0') >= 0; }).filter(function (k) { return regKey.keys.indexOf(k + '_Config') >= 0; }).sort(function (k1, k2) { return compareVsVersions(k2, k1); });
            Q.all(versions.map(function (v) { return getRegistryKey('HKLM\\Software\\Microsoft\\VisualStudio\\' + v + '\\Setup\\VS', 32).then(function (k) { return {
                version: v,
                productDir: path.resolve(k.values['ProductDir'].value),
                devenv: path.resolve(k.values['EnvironmentPath'].value)
            }; }); })).then(function (installations) { return deferred.resolve(installations); });
        }, function (err) { return deferred.reject(err); });
    }
    catch (err) {
        deferred.reject(err);
    }
    return deferred.promise;
}
exports.getInstallations = getInstallations;
function getCurrentInstallation(version) {
    var deferred = Q.defer();
    try {
        getInstallations().then(function (installations) {
            if (installations.length <= 0) {
                deferred.reject(new Error('No Visual Studio installations found'));
            }
            deferred.resolve(installations.filter(function (i) { return i.version === version; })[0] || installations[0]);
        }, function (err) { return deferred.reject(err); });
    }
    catch (err) {
        deferred.reject(err);
    }
    return deferred.promise;
}
exports.getCurrentInstallation = getCurrentInstallation;
function runCommand(grunt, command) {
    var deferred = Q.defer();
    grunt.verbose.subhead(command);
    var childProcess = cp.exec(command, {
        shell: 'cmd.exe'
    }, function () {
    });
    childProcess.stdout.on('data', function (d) { return grunt.log.write(d); });
    childProcess.stderr.on('data', function (d) { return grunt.log.error(d); });
    childProcess.on('error', function (err) {
        grunt.log.error(err);
        deferred.reject(err);
    });
    childProcess.on('exit', function (code) {
        if (code !== 0) {
            grunt.log.error(util.format('Exited with code: %d.', code));
            deferred.reject(new Error(util.format('Exited with code: %d.', code)));
        }
        grunt.verbose.ok(util.format('Exited with code: %d.', code));
        deferred.resolve(undefined);
    });
    return deferred.promise;
}
exports.runCommand = runCommand;
function cleanVsInstance(grunt, options) {
    return getCurrentInstallation(options.version).then(function (installation) {
        return format('"{CreateExpInstance}" /Clean /VSInstance={VSVersion} /RootSuffix={RootSuffix}', {
            CreateExpInstance: path.resolve(installation.productDir, 'VSSDK/VisualStudioIntegration/Tools/Bin/CreateExpInstance.exe'),
            VSVersion: installation.version,
            RootSuffix: options.rootSuffix
        });
    }).then(function (command) { return runCommand(grunt, command); });
}
exports.cleanVsInstance = cleanVsInstance;
function installVsix(grunt, options) {
    return getCurrentInstallation(options.version).then(function (installation) {
        return format('"{InstallVsix}" "{VsixFile}" "{VSExecutable}" {RootSuffix}', {
            InstallVsix: path.resolve(options.toolsDir, 'InstallVsix/InstallVsix.exe'),
            VsixFile: options.vsixFile,
            VSExecutable: installation.devenv,
            RootSuffix: options.rootSuffix
        });
    }).then(function (command) { return runCommand(grunt, command); });
}
exports.installVsix = installVsix;
function reset(grunt, options) {
    return cleanVsInstance(grunt, options).then(function () { return installVsix(grunt, options); });
}
exports.reset = reset;
function run(grunt, options) {
    return getCurrentInstallation(options.version).then(function (installation) {
        var command = format('"{VSExecutable}"', { VSExecutable: installation.devenv });
        if (options.testProject) {
            command += format(' "{TestProject}"', { TestProject: options.testProject });
        }
        command += format(' /RootSuffix {RootSuffix}', { RootSuffix: options.rootSuffix });
        return command;
    }).then(function (command) { return runCommand(grunt, command); });
}
exports.run = run;
//# sourceMappingURL=TestVS.js.map