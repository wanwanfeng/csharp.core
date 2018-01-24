@echo off
setlocal enabledelayedexpansion
( call encrypt.exe md5 image -e)>xx.txt
pause
