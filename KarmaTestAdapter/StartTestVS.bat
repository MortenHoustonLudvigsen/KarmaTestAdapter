@echo off
setlocal
call %~dp0\TestVSVars.bat %1
call %~dp0\ResetTestVS.bat %VSVersion%
@echo on
%DevEnv% ..\TestProjects\TestProjects.sln /RootSuffix KarmaTestAdapter
