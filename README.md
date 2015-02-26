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
    "KarmaConfigFile": "karma.conf.test.js",
    "ServerMode": true,
    "ServerPort": 3535,
    "LogToFile": true,
    "TestFiles": [ "test/**/*test.js" ],
    "LogDirectory": "TestResults/Karma",
    "OutputDirectory": "TestResults/Karma/Output",
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

See <http://mortenhoustonludvigsen.github.io/KarmaTestAdapter/changes/>

