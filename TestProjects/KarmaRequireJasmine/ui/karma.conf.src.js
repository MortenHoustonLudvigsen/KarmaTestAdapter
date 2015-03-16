module.exports = function (config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: './',

    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['requirejs', 'jasmine-jquery', 'jasmine', 'jasmine-matchers'],

    // list of files / patterns to load in the browser
    files: [
        'bower_modules/jquery/dist/jquery.js',

        { pattern: 'bower_modules/**/*.js', included: false },
        { pattern: 'bower_modules/**/*.html', included: false},

        { pattern: 'src/**/*.js', included: false },
        { pattern: 'src/**/*.html', included: false },
        { pattern: 'test/**/*Test.js', included: false }, //Test files are excluded here, but included in test/test-main.js and test/test-main.src.js 

        'src/common/config.js',
        'src/home/home-config.js',

        'test/test-main-src.js'
    ],

    // list of files to exclude
    exclude: [
        'src/home/main.js'
    ],

    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress'],
    //reporters: ['xml'],

    // web server port
    port: 9876,

    // enable / disable colors in the output (reporters and logs)
    colors: true,

    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,

    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,

    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Chrome'],

    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: false
  });
};
