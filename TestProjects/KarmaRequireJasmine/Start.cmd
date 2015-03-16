@echo OFF
@setlocal
@cls

@pushd %~dp0..\..
@set SolutionDir=%CD%
@popd
@cd %~dp0
@set CurrentDir=%CD%
@set TestProjectsDir=%SolutionDir%\TestProjects
@cd %CurrentDir%\ui

@set NODE_PATH=%CurrentDir%\ui\node_modules;%TestProjectsDir%\node_modules;%AppData%\Roaming\npm\node_modules
@echo NODE_PATH=%NODE_PATH%
@node %SolutionDir%\KarmaServer\lib\Start.js --karma karma.conf.src.js --settings KarmaTestAdapter.json --singleRun true
