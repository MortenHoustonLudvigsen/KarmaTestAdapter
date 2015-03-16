/**
 * Jasmine 2.0 standalone `boot.js` modified for Karma.
 * This file is registered in `index.js`. This version
 * does not include `HtmlReporter` setup.
 */
var KarmaTestAdapter;
(function (KarmaTestAdapter) {
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
    KarmaTestAdapter.wrapFunctions(jasmineEnv);
    /**
     * Obtain the public Jasmine API.
     */
    var jasmineInterface = jasmineRequire.interface(jasmine, jasmineEnv);
    /**
     * Setting up timing functions to be able to be overridden.
     * Certain browsers (Safari, IE 8, PhantomJS) require this hack.
     */
    window.setTimeout = window.setTimeout;
    window.setInterval = window.setInterval;
    window.clearTimeout = window.clearTimeout;
    window.clearInterval = window.clearInterval;
    for (var property in jasmineInterface) {
        if (jasmineInterface.hasOwnProperty(property)) {
            window[property] = jasmineInterface[property];
        }
    }
})(KarmaTestAdapter || (KarmaTestAdapter = {}));
//# sourceMappingURL=boot.js.map