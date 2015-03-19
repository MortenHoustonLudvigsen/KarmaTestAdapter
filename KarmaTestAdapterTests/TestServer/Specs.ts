export interface Spec {
    id: string;
    description: string;
    uniqueName: string;
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
