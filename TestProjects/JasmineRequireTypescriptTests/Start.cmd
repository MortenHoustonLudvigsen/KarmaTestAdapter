@echo off
setlocal
cls

pushd %~dp0..\..
set SolutionDir=%CD%
popd
cd %~dp0
set CurrentDir=%CD%
set TestProjectsDir=%SolutionDir%\TestProjects
set NODE_PATH=%CurrentDir%\node_modules;%TestProjectsDir%\node_modules;%SolutionDir%\node_modules;%AppData%\Roaming\npm\node_modules
node %SolutionDir%\KarmaServer\lib\Start.js --karma karma.conf.js --settings KarmaTestAdapter.json
