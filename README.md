Karma Test Adapter
================

A Visual Studio test explorer adapter for Karma

This extension integrates [Karma - Spectacular Test Runner for Javascript](http://karma-runner.github.io/) with the test explorer in Visual Studio 2013.

# Features

* Scans the files from the karma configuration to discover tests.

* Registers the file and position of each test, so that the test explorer in Visual Studio can link to the source code for the test.

* Source maps in test files are used to find the position of tests. So if a test is written in Typescript and the compiled javascript file contains a source map the test explorer will link to the typescript file.

* Can run selected tests

# Prerequisites

* Install [NodeJS](http://nodejs.org/)

* Install [Karma](http://karma-runner.github.io/) in your project:
`npm install karma --save-dev`

* Install the [Karma Visual Studio Reporter](https://github.com/MortenHoustonLudvigsen/karma-vs-reporter) in your project (at least version 0.5.3):
`npm install karma-vs-reporter --save-dev`

# Installation

Download and install from the Visual Studio Gallery here: [Karma Test Adapter](http://visualstudiogallery.msdn.microsoft.com/4cd59e4a-82e8-4b4e-8302-d102fc81b090)

# Configuration

Set up Karma normally (as described here: [Installation](http://karma-runner.github.io/0.12/intro/installation.html) and here: [Configuration](http://karma-runner.github.io/0.12/intro/configuration.html)).

Install this extension.

Start testing!

If you want Visual Studio to work differently from how Karma is configured (if you f.ex. only want to run PhantomJS from VS), you can create a JSON settings file called `karma-vs-reporter.json`. F.ex.:

```json
{
    "karmaConfigFile": "karma.conf.test.js",
    "config": {
        "browsers": [
            "PhantomJS"
        ]
    }
}
```

There are two properties:
* `karmaConfigFile` Use this if you want to use a karma configuration file not named `karma.conf.js`.

* `config` This property overwrites any configurations from the karma configuration file.

# Caveats

At the moment the adapter only works properly with [Jasmine](http://jasmine.github.io/) tests. It should be relatively easy to add other frameworks. This should be done in [karma-vs-reporter](https://github.com/MortenHoustonLudvigsen/karma-vs-reporter). Pull requests are welcome.