Karma Test Adapter
================

A Visual Studio test explorer adapter for Karma

This extension integrates [Karma - Spectacular Test Runner for Javascript](http://karma-runner.github.io/) with the test explorer in Visual Studio 2013.

# Features

* Scans the files from the karma configuration to discover tests.

* Registers the file and position of each test, so that the link from the test explorer in Visual Studio and the test file works.

* Source maps in test files are used to find the position of tests. So if a test is written in Typescript and the compiled javascript file contains a source map the test explorer will link to the typescript file.  

# Prerequisites

* Install [NodeJS](http://nodejs.org/)

* Install [Karma](http://karma-runner.github.io/) in your product
`npm install karma --save-dev`

* Install the [Karma Visual Studio Reporter](https://github.com/MortenHoustonLudvigsen/karma-vs-reporter):
`npm install karma-vs-reporter`

# Configuration

Set up Karma normally (as described here: [Installation](http://karma-runner.github.io/0.12/intro/installation.html) and here: [Configuration](http://karma-runner.github.io/0.12/intro/configuration.html)).

Install this extension.

Start testing!

If you want Visual Studio to work differently from how Karma is configured (if yoy f.ex. only want to run PhantomJS from VS), you can create a JSON settings file called `karma-vs-reporter.json`. F.ex.:

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

