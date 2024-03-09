@echo off
@REM pushd %~dp0\..\
call scripts\premake\premake5.exe vs2022
PAUSE