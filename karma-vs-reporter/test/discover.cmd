@echo off
setlocal
cd /d %~dp0

call init.cmd
del karma-vs-reporter.xml
call node_modules\.bin\karma-vs-reporter discover -c karma-vs-reporter.test.json
dir /b karma-vs-reporter.xml
