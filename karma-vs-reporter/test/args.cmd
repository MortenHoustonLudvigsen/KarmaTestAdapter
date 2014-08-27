@echo off
setlocal
cd /d %~dp0

call init.cmd
del *.xml
call node_modules\.bin\karma-vs-reporter args -c karma-vs-reporter.test.json -v VsConfig.json
dir /b *.xml