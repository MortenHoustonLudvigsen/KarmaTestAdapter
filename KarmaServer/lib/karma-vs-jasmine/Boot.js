/**
 * Jasmine 2.0 standalone `boot.js` modified for Karma.
 * This file is registered in `index.js`. This version
 * does not include `HtmlReporter` setup.
 */
(function (global) {
    /*global jasmineRequire */
    'use strict';
    /**
     * Require Jasmine's core files. Specifically, this requires and
     * attaches all of Jasmine's code to the `jasmine` reference.
     */
    var jasmine = jasmineRequire.core(jasmineRequire);
    /**
     * Obtain the Jasmine environment.
     */
    var jasmineEnv = jasmine.getEnv();
    /**
     * Instrument jasmine functions.
     */
    JasmineInstumentation.wrapFunctions(jasmineEnv);
    /**
     * Obtain the public Jasmine API.
     */
    var jasmineInterface = jasmineRequire.interface(jasmine, jasmineEnv);
    /**
     * Setting up timing functions to be able to be overridden.
     * Certain browsers (Safari, IE 8, PhantomJS) require this hack.
     */
    global.setTimeout = global.setTimeout;
    global.setInterval = global.setInterval;
    global.clearTimeout = global.clearTimeout;
    global.clearInterval = global.clearInterval;
    for (var property in jasmineInterface) {
        if (jasmineInterface.hasOwnProperty(property)) {
            global[property] = jasmineInterface[property];
        }
    }
}(window));
//# sourceMappingURL=Boot.js.map