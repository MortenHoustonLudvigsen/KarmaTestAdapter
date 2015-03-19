set VSVersion=%1
IF [%1] == [] set VSVersion=14.0

set VSDir=C:\Program Files (x86)\Microsoft Visual Studio %VSVersion%
set CreateExpInstance="%VSDir%\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe"
set RootVSIX=..\Tools\Root-VSIX-%VSVersion%\Root-VSIX.exe
set DevEnv="%VSDir%\Common7\IDE\devenv.exe"
