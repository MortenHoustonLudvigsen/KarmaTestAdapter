---
layout: page
title:
---

This extension integrates [Karma - Spectacular Test Runner for Javascript](http://karma-runner.github.io/) with the test explorer in Visual Studio 2013 and Visual Studio 2015 Preview / CTP.

# Features

* Karma is started in the background with `autoWatch` set to `true`, so tests are run immediately when files change.

* Once Karma has concluded a test run the results are shown in the Test Explorer in Visual Studio. There is no need to click `Run All` in the Test Explorer.

* Any output from a test (f.ex. using `console.log`) will be shown in the test result in the Test Explorer as a link: `Output`.  

* The `port` setting in the Karma configuration file (normally `karma.conf.js`) is ignored. A free port is chosen automatically.

* Karma is only started if the configuration files (`karma.conf.js` and optionally `KarmaTestAdapter.json`) are included in a Visual Studio project. Any other files, that Karma uses do not need to be included in a project.

## Features specific to [Jasmine](http://jasmine.github.io/) tests

* The file and position of each test is registered, so that the Test Explorer in Visual Studio can link to the source code for the test.

* Source maps in test files are used to find the position of tests. So if, for example, a test is written in Typescript and the compiled javascript file contains a source map the test explorer will link to the typescript file.

* The stack trace of a failed test is shown as a list of function names that link to the relevant line and file. As with the position of of tests, source maps are used to link to the original source.

# Prerequisites

* Install [NodeJS](http://nodejs.org/)

* Install [Karma](http://karma-runner.github.io/) in your project:
```
npm install karma --save-dev
```

# Installation

Download and install from the Visual Studio Gallery here: [Karma Test Adapter](http://visualstudiogallery.msdn.microsoft.com/4cd59e4a-82e8-4b4e-8302-d102fc81b090)

# Configuration

Set up Karma normally (as described here: [Installation](http://karma-runner.github.io/0.12/intro/installation.html) and here: [Configuration](http://karma-runner.github.io/0.12/intro/configuration.html)).

Start testing!

If you want Visual Studio to work differently from how Karma is configured (if you for example only want to run PhantomJS from VS), you can create a JSON settings file called `KarmaTestAdapter.json`.

Example:

```json
{
    "$schema": "http://MortenHoustonLudvigsen.github.io/KarmaTestAdapter/KarmaTestAdapter.schema.json",
    "KarmaConfigFile": "karma.conf.test.js",
    "Disabled": false,
    "LogToFile": true,
    "LogDirectory": "TestResults/Karma",
    "config": {
        "browsers": [
            "PhantomJS"
        ]
    }
}
```

These are the possible properties (all properties are optional):

* `$schema` Set to "<http://MortenHoustonLudvigsen.github.io/KarmaTestAdapter/KarmaTestAdapter.schema.json>" to get
  intellisense for `KarmaTestAdapter.json`.

* `KarmaConfigFile` Use this if you want to use a karma configuration file not named `karma.conf.js`.

* `Disabled` Set to true, if the Karma test adapter should be disabled for this karma configuration file.

* `LogToFile` Set to true, if you want the adapter to write log statements to a log file (named KarmaTestAdapter.log).

* `LogDirectory` Where the log file should be saved (if LogToFile is true). If this property is not specified the directory in which `KarmaTestAdapter.json` resides is used.

* `config` This property overwrites any configurations from the karma configuration file.


`KarmaTestAdapter.json` must be encoded in one of the following encodings:

* UTF-8
* UTF-8 with BOM / Signature
* UTF-16 Big-Endian with BOM / Signature
* UTF-16 Little-Endian with BOM / Signature

# Test containers

Test containers specify which Karma configuration files to use when running tests.

* A project can contain more than one test container.

* A directory can contain at most one test container.

* Test containers are files named `karma.conf.js` or `KarmaTestAdapter.json`.

* If a test container is named `karma.conf.js` it specifies itself as the Karma configuration file to use.

* If a test container is named `KarmaTestAdapter.json` it specifies the Karma configuration file to use in the optional `KarmaConfigFile` setting. If the `KarmaConfigFile` setting is not specified, then `karma.conf.js` in the same directory is used.

* A test container, which specifies a Karma configuration file that is not included in a project in the current solution or does not exist, will be disabled. I.e. no tests will be run for the container.

* Only test containers, that are included in a project in the current solution, are used.

* If there is a `KarmaTestAdapter.json` file in a project, then any `karma.conf.js` file in the same directory is not used as a test container.

* If there is a `KarmaTestAdapter.json` in a project in the current solution, that specifies a Karma configuration file in a different directory or project, then that Karma configuration file is not used as a test container.
