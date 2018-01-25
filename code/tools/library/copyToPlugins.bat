@echo off
setlocal enabledelayedexpansion

cd %cd%\bin\
set ha=%cd:~,-18%\source\Assets\Plugins
echo %cd%
echo %ha%
copy Debug %ha%
pause