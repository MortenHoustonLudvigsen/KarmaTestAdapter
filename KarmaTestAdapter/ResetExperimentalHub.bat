@echo on

setlocal

set VSVersion=14.0

set CreateExpInstance="C:\Program Files (x86)\Microsoft Visual Studio %VSVersion%\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe"
set RootVSIX=..\Tools\Root-VSIX-%VSVersion%\Root-VSIX.exe

%CreateExpInstance% /Clean /VSInstance=%VSVersion% /RootSuffix=karma
%RootVSIX% %VSVersion% karma dist/KarmaTestAdapter.vsix
