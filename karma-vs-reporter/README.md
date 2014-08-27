karma-vs-reporter
================

Report test results from [Karma - Spectacular Test Runner for Javascript](http://karma-runner.github.io/) in xml format for the [Karma Visual Studio test explorer adapter](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter).

# Installation

```bash
npm install karma-vs-reporter --save-dev
```

The adapter for test explorer in Visual Studio can be installed from [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/4cd59e4a-82e8-4b4e-8302-d102fc81b090). 

# Features
For features and usage see [Karma Visual Studio test explorer adapter](https://github.com/MortenHoustonLudvigsen/KarmaTestAdapter).

# Caveats

At the moment the reporter only works properly with [Jasmine](http://jasmine.github.io/) tests. It should be relatively easy to add other frameworks. Pull requests are welcome.