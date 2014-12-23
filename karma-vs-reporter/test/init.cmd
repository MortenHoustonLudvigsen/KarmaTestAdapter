@echo off
setlocal
cd /d %~dp0

rmdir node_modules\karma-vs-reporter /s /q
del /q node_modules\.bin\karma-vs-reporter*.*
rem call npm install
call npm install ..
