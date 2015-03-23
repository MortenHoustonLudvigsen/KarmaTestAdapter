## Version 1.1.0

### Extensions and traits

* The name of the outermost suite is now used as the class name for a test. This can be customized using new properties `Name` and `Extensions` in `KarmaTestAdapter.json`.

* The display name of each test now includes the suites separated by a space. This can be customized using new properties `Name` and `Extensions` in `KarmaTestAdapter.json`.

* New properties in `KarmaTestAdapter.json`:
  * `Name` The name of the test container. Used in the default generation of the fully qualified name for each test.
  * `Traits` An array of traits to be attached to each test. A trait can be a string or an object containing properties `Name` and `Value`. For traits specified by a string the string is the trait value and the trait name is "Category".
  * `Extensions` Path to a node.js module implementing extensions.

* Logging to the output window is significantly less chatty.

* The code in the Karma Test Adapter has been refactored to enable sharing code with other test adapters. A lot of the code has moved to [JsTestAdapter](https://github.com/MortenHoustonLudvigsen/JsTestAdapter), which can be install as a NuGet package.

These changes resolve the following issues:

* [Concatenate a test's suites in front of the test description](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/issues/24).

* [configure test entries so that they behave as expected in Test Explorer's grouping and sorting](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/issues/25).

### Download

[KarmaTestAdapter.vsix](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/releases/download/v1.1.0/KarmaTestAdapter.vsix)
 
## Version 1.0.3

### Bug fixes

* Fixed: [The adapter does not work if both karma-jasmine and karma-jasmine-matchers are used ](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/issues/20).

### Download

[KarmaTestAdapter.vsix](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/releases/download/v1.0.3/KarmaTestAdapter.vsix)
 
## Version 1.0.2

### Bug fixes

* Fixed: [Can not read KarmaTestAdapter.json if it is encoded with a BOM](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/issues/12).

  From now on `KarmaTestAdapter.json` must be encoded in one of the following encodings:

  * UTF-8
  * UTF-8 with BOM / Signature
  * UTF-16 Big-Endian with BOM / Signature
  * UTF-16 Little-Endian with BOM / Signature

### Download

[KarmaTestAdapter.vsix](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/releases/download/v1.0.2/KarmaTestAdapter.vsix)
 
## Version 1.0.1

### Improved identification of test containers:

* Test containers specify which Karma configuration files to use when running tests.

* A project can contain more than one test container.

* A directory can contain at most one test container.

* Test containers are files named `karma.conf.js` or `KarmaTestAdapter.json`.

* If a test container is named `karma.conf.js` it specifies itself as the Karma configuration file to use.

* If a test container is named `KarmaTestAdapter.json` it specifies the Karma configuration file to use in the optional `KarmaConfigFile` setting. If the `KarmaConfigFile` setting is not specified, then `karma.conf.js` in the same directory is used.

* A test container, which specifies a Karma configuration file that is not included in a project in the current solution or does not exist, will be disabled. I.e. no tests will be run for the container.

* Only test containers, that are included in a project in the current solution, are used.

* If there is a `KarmaTestAdapter.json` file in a project, then any `karma.conf.js` file in the same directory is not used as a test container.

* If there is a `KarmaTestAdapter.json` in a project in the current solution, that specifies a Karma configuration file in a different directory or project, then that Karma configuration file is not used as a test container.

### Bug fixes

* Fixed: [The adapter does not work with karma configuration files not called karma.conf.js](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/issues/15).

* Fixed: [The format of the URI could not be determined](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/issues/13).

* The adapter now works with globally installed modules

### Download

[KarmaTestAdapter.vsix](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/releases/download/v1.0.1/KarmaTestAdapter.vsix)
 

## Version 1.0.0

### This is a complete rewrite of the Karma Test Adapter

Some of the major changes are:

* The node package `karma-vs-reporter` is deprecated, and is no longer used

* The settings file `karma-vs-reporter.json` is now `KarmaTestAdapter.json`

* Karma is now always run in the background, and the settings `ServerMode` and `ServerPort` are no longer used.

* Deciding when to run tests is now left up to Karma. The adapter only watches configuration files.
 
* Test results are shown in the Test Explorer as soon as Karma has completed a test run.

### Download

[KarmaTestAdapter.vsix](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/releases/download/v1.0.0/KarmaTestAdapter.vsix)
 
## Version 0.8.3

### Bug fixes

* Ignore tests with no names

### Download

[KarmaTestAdapter.Package.vsix](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter/releases/download/v0.8.3/KarmaTestAdapter.Package.vsix)

## Version 0.8.2

* Bug fixes

## Version 0.8.1

* The adapter now searches for node module karma-vs-reporter in parent directories

### Bug fixes

* Don't fail if no test files are found

## Version 0.8.0

### Bug fixes

* Use node module 'di' in stead of 'karma/node_modules/di'.

* Version 0.8.0 needs at least version 0.8.0 of karma-vs-reporter to work. To upgrade run:

```
npm install karma-vs-reporter
```

## Version 0.7.4

### Bug fixes

* The adapter now tries to detect the encoding of `karma-vs-reporter.json` falling back to UTF-8 if the encoding could not be detected.

* The adapter no longer fails if it can not get a SHA1 hash of the contents of a file.

## Version 0.7.3

### Bug fixes

* The adapter could sometimes fail with a null exception if there were errors in `karma-vs-reporter.json`. This should now be fixed.

## Version 0.7.2

### Error handling

* The adapter will now always log to `%appdata%\2PS\KarmaTestAdapter\KarmaTestAdapter.log`.

* If there are errors in `karma-vs-reporter.json` no tests will be shown for that configuration,
  and the adapter *will not* use any `karma.conf` in the same directory.

* It is no longer necessary to restart Visual Studio after correcting errors in `karma-vs-reporter.json`.

## Version 0.7.1

* `karma-vs-reporter.json` has a new optional property:

  - `TestFiles` By default the adapter watches all files in the karma configuration to get the list of tests. To avoid
    running test discovery when files, that do not contain tests, are changed specify files with tests in the `TestFiles`
    property. Specify `TestFiles` as an array of file patterns, or omit it to use the file specifications in the karma
    configuration.

* The adapter now fails if the correct version of karma-vs-reporter is not installed.

* Error handling should be a bit better in this version.

## Version 0.7.0

### Server mode

* Introduced server mode. When in server mode Karma will be started once, and run in the background.
  This means that browsers are not restarted every time tests are run, improving performance and
  making the experience much smoother.

* `karma-vs-reporter.json` has new optional properties:

  - `ServerMode` set to true, if you want Karma to always run in the background. This will normally give a
    significant performance improvement. Karma will be started in the background if `ServerMode` is true **and**
    `ServerPort` has a value.
  
  - `ServerPort` TCP port, that Karma should listen to when running in the background (when `ServerMode` is true).
     This should be different from the port configured in the Karma configuration file (`karma.conf.js`).

* Version 0.7.0 needs at least version 0.7.0 of karma-vs-reporter to work

## Version 0.6.3

### Bug fixes

* Fixed: If `LogToFile` is true and `LogDirectory` is empty in `karma-vs-reporter.json` the adapter fails with the following message: `The path is not of a legal form.`.

## Version 0.6.2

* Changes to tests files that are not included in a project will now trigger test discovery.

## Version 0.6.1

* More improvements to logging

* Tests are discovered when the karma configuration file changes (normally `karma.conf.js`), even when `karma-vs-reporter.json` is used.

## Version 0.6.0

* Version 0.6.0 needs at least version 0.6.0 of karma-vs-reporter to work

* I have made a number of tweeks to logging

* `karma-vs-reporter.json` has new optional properties:

  - `LogToFile`: set to true, if you want the adapter to write log statements to a log file (named `KarmaTestAdapter.log`)

  - `LogDirectory`: Where the log file should be saved (if `LogToFile` is true). If this property is not specified the
    directory in which `karma-vs-reporter.json` resides is used.

  - `OutputDirectory`: Normally the adapter communicates with Karma using temporary files. These files are deleted immediately.
    If you want to see these files, you can specify an `OutputDirectory`, in which case the files will not be deleted.

* The adapter should no longer try to discover tests when any file is saved. Only files that are included in the karma tests
  will trigger discovery, and only if they have actually changed (the adapter keeps track of this using a SHA1 hash for each file).
