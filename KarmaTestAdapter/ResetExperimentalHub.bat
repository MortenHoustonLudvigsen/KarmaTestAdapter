@echo off

setlocal

set CreateExpInstance="C:\Program Files (x86)\Microsoft Visual Studio 12.0\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe"
set DevEnv="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"

%CreateExpInstance% /Clean /VSInstance=12.0 /RootSuffix=karma
..\Tools\Root-VSIX\Root-VSIX.exe 12.0 karma dist/KarmaTestAdapter.vsix

: %DevEnv% ..\TestProjects\TestProjects.sln /RootSuffix karma

: grunt dist
