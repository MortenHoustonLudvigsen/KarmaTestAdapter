@echo off
setlocal

: Cleans the solution of all files and directories ignored by git, and installs npm packages

set SolutionDir=%~dp0

echo ====================================================================================
echo Clean %SolutionDir%
echo ------------------------------------------------------------------------------------
cd %SolutionDir%
git clean -f -x -d -e KarmaTestAdapter/BuildConfig.json -e .vs/ -e *.suo
echo.
echo.

echo ====================================================================================
echo Install npm packages in %SolutionDir%\KarmaTestAdapter
echo ------------------------------------------------------------------------------------
cd %SolutionDir%\KarmaTestAdapter
call npm install
echo.
echo.

echo ====================================================================================
echo Install npm packages in %SolutionDir%\KarmaServer
echo ------------------------------------------------------------------------------------
cd %SolutionDir%\KarmaServer
call npm install
echo.
echo.

echo ====================================================================================
echo Install npm packages in %SolutionDir%\TestProjects
echo ------------------------------------------------------------------------------------
cd %SolutionDir%\TestProjects
call npm install
echo.
echo.

echo ====================================================================================
echo Create debug settings
echo ------------------------------------------------------------------------------------
cd %SolutionDir%\KarmaTestAdapter
grunt xmlpoke:debugSettings
echo.
echo.

echo ====================================================================================
echo FINISHED
echo ====================================================================================

