---
layout: page
title: Changes
permalink: /changes/
---

## Version 0.8.3

### Bug fixes

* Ignore tests with no names

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
