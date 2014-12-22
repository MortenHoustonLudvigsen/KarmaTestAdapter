@echo off
setlocal
cd /d %~dp0

del served-testrun.xml
call node_modules\.bin\karma-vs-reporter served-run -p 53983 -c karma-vs-reporter.test.json -v VsConfig.json -o served-testrun.xml
dir /b served-testrun.xml