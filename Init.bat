@echo off
setlocal

: Cleans the solution of all files and directories ignored by git, and installs npm packages

set SolutionDir=%~dp0

echo Clean %SolutionDir%
cd %SolutionDir%
git clean -fdX
echo.
echo.

echo Install npm packages in %SolutionDir%\TestProjects
cd %SolutionDir%\TestProjects
call npm install
echo.
echo.

echo Install npm packages in %SolutionDir%\KarmaTestAdapter
cd %SolutionDir%\KarmaTestAdapter
call npm install
echo.
echo.

echo FINISHED
