@echo off
if "%1" == "" goto missing
if not exist %1.log goto badfile
logiccom -l %1.log -g %2 %3
rem echo errorlevel
if errorlevel 1 goto badcompile
type %1.cod
goto stop
:badcompile
type listing.txt
goto stop
:missing
echo No source file specified - use test3 srce
goto stop
:badfile
echo Cannot find %1.log
:stop
