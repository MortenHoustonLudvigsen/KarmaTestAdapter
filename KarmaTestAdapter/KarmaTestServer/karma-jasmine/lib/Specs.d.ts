declare module Specs {
    export interface StackInfo {
        skip?: number;
        skipFunctions?: string;
        stack?: string;
        stacktrace?: string;
        'opera#sourceloc'?: any;
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
}