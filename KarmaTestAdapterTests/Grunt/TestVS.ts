import util = require('util');
import path = require('path');
import cp = require('child_process');
import Q = require('q');
var format = require("string-template");
var regedit = require('regedit');

function compareVsVersions(v1: string, v2: string): number {
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
    } else {
        return 1;
    }
}

type RegistryKey = {
    keys: string[];
    values: {
        [name: string]: {
            value: any;
            type: string;
        }
    };
};

export function getRegistryKey(key: string, arch?: number): Q.Promise<RegistryKey> {
    var list: Function;

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

    var deferred = Q.defer<RegistryKey>();
    try {
        list([key],(err, result) => {
            if (err) return deferred.reject(err);
            deferred.resolve(result[key]);
        });
    } catch (err) {
        deferred.reject(err);
    }
    return deferred.promise;
}

type Installation = {
    version: string;
    productDir: string;
    devenv: string;
};

export function getInstallations(): Q.Promise<Installation[]> {
    var deferred = Q.defer<Installation[]>();
    try {
        getRegistryKey('HKCU\\Software\\Microsoft\\VisualStudio').then(regKey => {
            var versions = regKey.keys
                .filter(k => /^\d+\.\d+$/.test(k))
                .filter(k => compareVsVersions(k, '12.0') >= 0)
                .filter(k => regKey.keys.indexOf(k + '_Config') >= 0)
                .sort((k1, k2) => compareVsVersions(k2, k1));

            Q.all(versions.map(v => getRegistryKey('HKLM\\Software\\Microsoft\\VisualStudio\\' + v + '\\Setup\\VS', 32).then(k => <Installation>{
                version: v,
                productDir: path.resolve(k.values['ProductDir'].value),
                devenv: path.resolve(k.values['EnvironmentPath'].value)
            }))).then(installations => deferred.resolve(installations));
        }, err => deferred.reject(err));
    } catch (err) {
        deferred.reject(err);
    }
    return deferred.promise;
}

export function getCurrentInstallation(version?: string): Q.Promise<Installation> {
    var deferred = Q.defer<Installation>();
    try {
        getInstallations().then(installations => {
            if (installations.length <= 0) {
                deferred.reject(new Error('No Visual Studio installations found'));
            }
            deferred.resolve(installations.filter(i => i.version === version)[0] || installations[0]);
        }, err => deferred.reject(err));
    } catch (err) {
        deferred.reject(err);
    }
    return deferred.promise;
}

type Options = {
    version?: string;
    toolsDir?: string;
    rootSuffix: string;
    vsixFile?: string;
    testProject?: string;
};

export function runCommand(grunt: any, command: string): Q.Promise<void> {
    var deferred = Q.defer<void>();

    grunt.verbose.subhead(command);

    var childProcess = cp.exec(command, {
        shell: 'cmd.exe'
    },() => { });

    childProcess.stdout.on('data', d => grunt.log.write(d));
    childProcess.stderr.on('data', d => grunt.log.error(d));

    childProcess.on('error', err => {
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

export function cleanVsInstance(grunt: any, options: Options): Q.Promise<void> {
    return getCurrentInstallation(options.version).then(installation => {
        return format('"{CreateExpInstance}" /Clean /VSInstance={VSVersion} /RootSuffix={RootSuffix}', {
            CreateExpInstance: path.resolve(installation.productDir, 'VSSDK/VisualStudioIntegration/Tools/Bin/CreateExpInstance.exe'),
            VSVersion: installation.version,
            RootSuffix: options.rootSuffix
        });
    }).then(command => runCommand(grunt, command));
}

export function installVsix(grunt: any, options: Options): Q.Promise<void> {
    return getCurrentInstallation(options.version).then(installation => {
        return format('"{InstallVsix}" "{VsixFile}" "{VSExecutable}" {RootSuffix}', {
            InstallVsix: path.resolve(options.toolsDir, 'InstallVsix/InstallVsix.exe'),
            VsixFile: options.vsixFile,
            VSExecutable: installation.devenv,
            RootSuffix: options.rootSuffix
        });
    }).then(command => runCommand(grunt, command));
}

export function reset(grunt: any, options: Options): Q.Promise<void> {
    return cleanVsInstance(grunt, options).then(() => installVsix(grunt, options));
}

export function run(grunt: any, options: Options): Q.Promise<void> {
    return getCurrentInstallation(options.version).then(installation => {
        var command = format('"{VSExecutable}"', { VSExecutable: installation.devenv });
        if (options.testProject) {
            command += format(' "{TestProject}"', { TestProject: options.testProject });
        }
        command += format(' /RootSuffix {RootSuffix}', { RootSuffix: options.rootSuffix });
        return command;
    }).then(command => runCommand(grunt, command));
}
