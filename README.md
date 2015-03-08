A Visual Studio test explorer adapter for Karma

This extension integrates [Karma - Spectacular Test Runner for Javascript](http://karma-runner.github.io/) with the test explorer in Visual Studio 2013 and Visual Studio 2015 Preview / CTP.

# Features

* Runs tests when files change.

* Registers the file and position of each test, so that the test explorer in Visual Studio can link to the source code for the test.

* Source maps in test files are used to find the position of tests. So if a test is written in Typescript and the compiled javascript file contains a source map the test explorer will link to the typescript file.

# Prerequisites

* Install [NodeJS](http://nodejs.org/)

* Install [Karma](http://karma-runner.github.io/) in your project:
`npm install karma --save-dev`

# Installation

Download and install from the Visual Studio Gallery here: [Karma Test Adapter](http://visualstudiogallery.msdn.microsoft.com/4cd59e4a-82e8-4b4e-8302-d102fc81b090)

# Configuration

Set up Karma normally (as described here: [Installation](http://karma-runner.github.io/0.12/intro/installation.html) and here: [Configuration](http://karma-runner.github.io/0.12/intro/configuration.html)).

Install this extension.

Start testing!

If you want Visual Studio to work differently from how Karma is configured (if you for example only want to run PhantomJS from VS),
you can create a JSON settings file called `KarmaTestAdapter.json`.

Example:

```json
{
    "$schema": "http://MortenHoustonLudvigsen.github.io/KarmaTestAdapter/KarmaTestAdapter.schema.json",
    "KarmaConfigFile": "karma.conf.test.js",
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

# Caveats

At the moment the adapter only works properly with [Jasmine](http://jasmine.github.io/) tests. It should be relatively easy to add other frameworks. Pull requests are welcome.

# Changes

See <http://mortenhoustonludvigsen.github.io/KarmaTestAdapter/changes/>

