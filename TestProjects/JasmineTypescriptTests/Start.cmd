@echo off
setlocal
cls
: set NODE_PATH=C:\Git\Spikes\NodeCSharpCommunication\TestProjects\node_modules
: node ..\..\KarmaTestAdapter\lib\Start.js -c karma.conf.js
set NODE_PATH=C:\Git\Spikes\NodeCSharpCommunication\TestProjects\node_modules;C:\Git\Spikes\node_modules
node ..\..\KarmaTestAdapter\lib\Start.js --karma karma.conf.js --settings KarmaTestAdapter.json