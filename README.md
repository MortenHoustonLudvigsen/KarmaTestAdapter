---
layout: page
title: Karma Test Adapter
permalink: /
---

Karma Test Adapter
================

A Visual Studio test explorer adapter for Karma

This extension integrates [Karma - Spectacular Test Runner for Javascript](http://karma-runner.github.io/) with the test explorer in Visual Studio 2013 and Visual Studio 2015 Preview / CTP.

# Demo

[See a demo video](http://youtu.be/T9wqxOX3OX0)

A very simple demo is available here: [Demo](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter-Demo) 

# Features

* Scans the files from the karma configuration to discover tests.

* Registers the file and position of each test, so that the test explorer in Visual Studio can link to the source code for the test.

* Source maps in test files are used to find the position of tests. So if a test is written in Typescript and the compiled javascript file contains a source map the test explorer will link to the typescript file.

* Can run selected tests

# Prerequisites

* Install [NodeJS](http://nodejs.org/)

* Install [Karma](http://karma-runner.github.io/) in your project:
`npm install karma --save-dev`

* Install the Karma Visual Studio Reporter in your project (at least version 0.7.0):
`npm install karma-vs-reporter --save-dev`

# Installation

Download and install from the Visual Studio Gallery here: [Karma Test Adapter](http://visualstudiogallery.msdn.microsoft.com/4cd59e4a-82e8-4b4e-8302-d102fc81b090)

# Configuration

Set up Karma normally (as described here: [Installation](http://karma-runner.github.io/0.12/intro/installation.html) and here: [Configuration](http://karma-runner.github.io/0.12/intro/configuration.html)).

Install this extension.

Start testing!

If you want Visual Studio to work differently from how Karma is configured (if you for example only want to run PhantomJS from VS),
you can create a JSON settings file called `karma-vs-reporter.json`.

Example:

```json
{
    "$schema": "http://MortenHoustonLudvigsen.github.io/KarmaTestAdapter/karma-vs-reporter.schema.json",
    "karmaConfigFile": "karma.conf.test.js",
    "ServerMode": true,
    "ServerPort": 3535,
    "LogToFile": true,
    "TestFiles": [ "test/**/*test.js" ],
    "LogDirectory": "KarmaTestAdapter",
    "OutputDirectory": "KarmaTestAdapter/Output",
    "config": {
        "browsers": [
            "PhantomJS"
        ]
    }
}
```

These are the possible properties (all properties are optional):

* `$schema` Set to "<http://MortenHoustonLudvigsen.github.io/KarmaTestAdapter/karma-vs-reporter.schema.json>" to get
  intellisense for `karma-vs-reporter.json`.

* `karmaConfigFile` Use this if you want to use a karma configuration file not named `karma.conf.js`.

* `ServerMode` Set to true, if you want Karma to always run in the background. This will normally give a
  significant performance improvement. Karma will be started in the background if `ServerMode` is true **and**
  `ServerPort` has a value.

* `ServerPort` TCP port, that Karma should listen to when running in the background (when `ServerMode` is true).
   This should be different from the port configured in the Karma configuration file (`karma.conf.js`).
   
* `TestFiles` By default the adapter watches all files in the karma configuration to get the list of tests. To avoid
  running test discovery when files, that do not contain tests, are changed specify files with tests in the `TestFiles`
  property. Specify `TestFiles` as an array of file patterns, or omit it to use the file specifications in the karma
  configuration.

* `LogToFile` Set to true, if you want the adapter to write log statements to a log file (named KarmaTestAdapter.log)

* `LogDirectory` Where the log file should be saved (if LogToFile is true). If this property is not specified the
  directory in which karma-vs-reporter.json resides is used.

* `OutputDirectory` Normally the adapter communicates with Karma using temporary files. These files are deleted immediately.
  If you want to see these files, you can specify an OutputDirectory, in which case the files will not be deleted.

* `config` This property overwrites any configurations from the karma configuration file.

# Caveats

At the moment the adapter only works properly with [Jasmine](http://jasmine.github.io/) tests. It should be relatively easy to add other frameworks. Pull requests are welcome.

# Changes

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
