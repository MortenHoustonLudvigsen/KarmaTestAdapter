@echo off
setlocal
call %~dp0\ExperimentalHubVars.bat %1
call %~dp0\ResetExperimentalHub.bat %VSVersion%
@echo on
%DevEnv% ..\TestProjects\TestProjects.sln /RootSuffix karma
