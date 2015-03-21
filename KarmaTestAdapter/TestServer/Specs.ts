import TestContext = require('./TestContext');

export interface StackInfo {
    skip?: number;
    skipFunctions?: string;
    stack?: string;
    stacktrace?: string;
    'opera#sourceloc'?: any;
}

export interface Context {
    name: string;
    testContext?: TestContext;
}

export interface SpecData {
    id?: string;
    description?: string;
    log?: string[];
    skipped?: boolean;
    success?: boolean;
    suite?: string[];
    time?: number;
    startTime?: number;
    endTime?: number;
    uniqueName?: string;
    failures?: Failure[];
    source?: StackInfo;
}

export interface Failure {
    message?: string;
    stack?: StackInfo;
    passed?: boolean;
}

export interface Spec {
    id: string;
    description: string;
    uniqueName?: string;
    suite: string[];
    source: Source;
    results?: SpecResult[];
}

export interface SpecResult {
    name: string;
    success: boolean;
    skipped: boolean;
    output: string;
    time: number;
    startTime: number;
    endTime: number;
    log?: string[];
    failures: Expectation[];
}

export interface Source {
    functionName?: string;
    fileName: string;
    lineNumber: number;
    columnNumber: number;
    source?: Source;
}

export interface Expectation {
    message: string;
    stack: string[];
    passed: boolean;
}
