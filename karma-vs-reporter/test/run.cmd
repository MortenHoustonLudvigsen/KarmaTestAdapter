@echo off
setlocal
cd /d %~dp0

call init.cmd
del testrun.xml
call node_modules\.bin\karma-vs-reporter run -o testrun.xml -p 53983 -c karma-vs-reporter.test.json -v VsConfig.json
dir /b testrun.xml