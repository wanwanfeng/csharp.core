@echo off
setlocal enabledelayedexpansion

set key=%~1
set c=%~2
set d=%~3
set md5_txt=tools\md5_encrypt_list.txt
set aes_txt=tools\aes_encrypt_list.txt

echo %key%%c%%d%

if not %d%=="" ( set md5_txt=%d% )
echo %md5_txt%

if not exist %md5_txt% ( 
	echo %md5_txt% is not exist
	pause
	exit
 )
 
if exist %aes_txt% ( del %aes_txt%)
 
for /f "delims=" %%i in ( %md5_txt% ) do (
	echo %%i
	call encrypt aes %%i %key% %c%
	call :getSize %%i size
	echo !size!,%%i >>%aes_txt%
	REM pause
)
pause 
exit

REM echo -------------获取文件大小---------------
:getSize 
for %%i in (%~1) do set "%2=%%~zi"
goto :eof


