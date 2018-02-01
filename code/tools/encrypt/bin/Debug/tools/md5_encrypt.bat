﻿@echo off
setlocal enabledelayedexpansion

set key=%~1
set isHead=%~2
if exist xx.txt ( del xx.txt )
if exist tools\md5_encrypt_list.txt ( del tools\md5_encrypt_list.txt )
for /f "delims=" %%i in (temp.txt) do (
	call :encryptPath %%i
)
if exist xx.txt ( del xx.txt )
if exist temp.txt ( del temp.txt )
pause 
exit

:encryptPath
set folder=%~1
REM echo %folder%
for /f "delims=" %%i in (' dir %cd%\%folder% /b/a-d/s ') do (
	call :getFilePath %%~dpi filePath
	set filePath=!filePath:\=/!
	REM 去除末尾/
	set filePath=!filePath:~0,-1!
	set fileName=%%~nxi
	echo !filePath!/!fileName!
	
	REM ::相对路径
	call encrypt md5 "!filePath!" %key% %isHead% >xx.txt
	set /p filePathMd5=<xx.txt
	set filePathMd5=temp\!filePathMd5!
	REM echo !filePathMd5!
	if not exist "!filePathMd5!" ( mkdir "!filePathMd5!")
	
	REM ::文件名
	call encrypt md5 "!fileName!" %key% %isHead% >xx.txt
	set /p fileNameMd5=<xx.txt
	set fileNameMd5=!fileNameMd5!%%~xi
	echo !filePathMd5!\!fileNameMd5!
	if exist "!filePathMd5!!fileNameMd5!" ( del "!filePathMd5!!fileNameMd5!" )

	REM echo "%%i"
	echo !filePathMd5!\!fileNameMd5! >> tools\md5_encrypt_list.txt
	echo f | xcopy "%%i" "%cd%\!filePathMd5!\!fileNameMd5!" /y/s/q
	REM pause
)
goto :eof

:getFilePath
set folder=%~1
set "%~2=!folder:%cd%\=!"
goto :eof

pause 