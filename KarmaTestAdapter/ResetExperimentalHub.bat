@echo off
setlocal
call %~dp0\ExperimentalHubVars.bat %1

@echo on
%CreateExpInstance% /Clean /VSInstance=%VSVersion% /RootSuffix=karma
%RootVSIX% %VSVersion% karma dist/KarmaTestAdapter.vsix
