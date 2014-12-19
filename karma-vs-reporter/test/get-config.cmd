@echo off
setlocal
cd /d %~dp0

call init.cmd
del karma-vs-reporter.config.json
call node_modules\.bin\karma-vs-reporter get-config -c karma-vs-reporter.test.json
dir /b karma-vs-reporter.config.json
