@echo off
if "%1" == "" goto missing
if not exist %1.log goto badfile
logiccom -l %1.log -n %2 %3
if errorlevel 1 goto badcompile
type %1.cod
goto stop
:badcompile
type listing.txt
type %1.cod
goto stop
:missing
echo No source file specified - use test3 srce
goto stop
:badfile
echo Cannot find %1.log
:stop
