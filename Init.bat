@echo off
setlocal

: Cleans the solution of all files and directories ignored by git

set SolutionDir=%~dp0

echo ====================================================================================
echo Clean %SolutionDir%
echo ------------------------------------------------------------------------------------
cd %SolutionDir%
git clean -f -x -d -e KarmaTestAdapter/BuildConfig.json -e .vs/ -e *.suo
echo.
echo.

echo ====================================================================================
echo FINISHED
echo ====================================================================================

