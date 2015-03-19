import Q = require('q');

export var karma = {
    Constants: <KarmaConstants>require('karma/lib/constants'),
    Logger: <LoggerModule>require('karma/lib/logger'),
    Config: <Config>require('karma/lib/config'),
    Server: <Server>require('karma').server
};

export interface Config {
    parseConfig(configFilePath: any, cliOptions: any): any;
}

export interface KarmaConfig {
    basePath: string;
    urlRoot: string;
}

export interface Server {
    start(cliOptions: any, done: (exitCode: number) => void): void;
}

export interface LoggerModule {
    setup(level: string, colors: boolean, appenders: any[]): void;
    create(name: string, level: string): Logger;
}

export interface Logger {
    setLevel(level: string): void;
    setLevel(level: Level): void;

    isLevelEnabled(level: Level): boolean;
    isTraceEnabled(): boolean;
    isDebugEnabled(): boolean;
    isInfoEnabled(): boolean;
    isWarnEnabled(): boolean;
    isErrorEnabled(): boolean;
    isFatalEnabled(): boolean;

    trace(message: string, ...args: any[]): void;
    debug(message: string, ...args: any[]): void;
    info(message: string, ...args: any[]): void;
    warn(message: string, ...args: any[]): void;
    error(message: string, ...args: any[]): void;
    fatal(message: string, ...args: any[]): void;

    appenders?: any;
    setup(): void;
}

export interface Level {
    isEqualTo(other: string): boolean;
    isEqualTo(otherLevel: Level): boolean;
    isLessThanOrEqualTo(other: string): boolean;
    isLessThanOrEqualTo(otherLevel: Level): boolean;
    isGreaterThanOrEqualTo(other: string): boolean;
    isGreaterThanOrEqualTo(otherLevel: Level): boolean;
}

export interface KarmaConstants {
    LOG_DISABLE: string;
    LOG_ERROR: string;
    LOG_WARN: string;
    LOG_INFO: string;
    LOG_DEBUG: string;
    COLOR_PATTERN: string;
    NO_COLOR_PATTERN: string;
    CONSOLE_APPENDER: any;
}

export interface EventEmitter {
    bind(object, context): void;
    emitAsync(name: string): Q.Promise<any>;
}

