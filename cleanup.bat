@echo off
rem cleans up all the bak and generated files so that you can
rem do a fresh build of a system
rem
rem for example    cleanup boolcalc

del *.cod
del parva.cs
del parser.cs
del scanner.cs
del %1.exe
del %1.cs
del errors.txt
del listing.txt
del *.bak
del *.old
del %1\*.bak
