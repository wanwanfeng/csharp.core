@echo off
setlocal enabledelayedexpansion

REM dir /b/ad >patch-list.txt

set list=patch-list.txt
if exist %list% del %list%

for /f "tokens=1 delims=" %%i in ( ' dir /ad/b "svn*master" "svn*patch" ') do (
	echo %%i
	if exist %%i.zip (
		call :getSize "%cd%\%%i.zip" size	
		echo !size!,%%i.zip>>%list%	
	) else (
		echo %%i>>%list%
	)
)

pause
exit

REM echo ------获取文件大小--------
:getSize 
for %%i in (%~1) do set "%2=%%~zi"
goto :eof

::find /i /v ".jpg" svn-patch.txt >svn-patch11111.txt

pause
