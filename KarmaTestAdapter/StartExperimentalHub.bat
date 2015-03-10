@echo on

setlocal

set VSVersion=%1
IF [%1] == [] set VSVersion=14.0

set CreateExpInstance="C:\Program Files (x86)\Microsoft Visual Studio %VSVersion%\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe"
set DevEnv="C:\Program Files (x86)\Microsoft Visual Studio %VSVersion%\Common7\IDE\devenv.exe"
set RootVSIX=..\Tools\Root-VSIX-%VSVersion%\Root-VSIX.exe

%CreateExpInstance% /Clean /VSInstance=%VSVersion% /RootSuffix=karma
%RootVSIX% %VSVersion% karma dist/KarmaTestAdapter.%VSVersion%.vsix
%DevEnv% ..\TestProjects\TestProjects.sln /RootSuffix karma

