@echo off
setlocal
call %~dp0\TestVSVars.bat %1

@echo on
%CreateExpInstance% /Clean /VSInstance=%VSVersion% /RootSuffix=KarmaTestAdapter
%RootVSIX% %VSVersion% KarmaTestAdapter dist/KarmaTestAdapter.vsix
