declare var jasmineRequire: any;


/**
 * Jasmine 2.0 standalone `boot.js` modified for Karma.
 * This file is registered in `index.js`. This version
 * does not include `HtmlReporter` setup.
 */
module KarmaTestAdapter {
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
    wrapFunctions(jasmineEnv);

    /**
     * Obtain the public Jasmine API.
     */
    var jasmineInterface = jasmineRequire.interface(jasmine, jasmineEnv);

    /**
     * Add reporter
     */
    jasmineEnv.addReporter(new KarmaReporter(window.__karma__, jasmineEnv));

    /**
     * Setting up timing functions to be able to be overridden.
     * Certain browsers (Safari, IE 8, PhantomJS) require this hack.
     */
    window.setTimeout = window.setTimeout;
    window.setInterval = window.setInterval;
    window.clearTimeout = window.clearTimeout;
    window.clearInterval = window.clearInterval;

    /**
     * Add all of the Jasmine global/public interface to the proper
     * global, so a project can use the public interface directly.
     * For example, calling `describe` in specs instead of
     * `jasmine.getEnv().describe`.
     */
    for (var property in jasmineInterface) {
        if (jasmineInterface.hasOwnProperty(property)) {
            window[property] = jasmineInterface[property];
        }
    }

    createSpecFilter(window.__karma__.config, jasmineEnv);
    window.__karma__.start = createStartFn(window.__karma__, jasmineEnv);
}
