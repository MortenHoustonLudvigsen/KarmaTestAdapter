@echo off
setlocal
cd /d %~dp0

call init.cmd
del testserve.xml
call node_modules\.bin\karma-vs-reporter serve -p 53983 -c karma-vs-reporter.test.json -o testserve.xml
dir /b testserve.xml