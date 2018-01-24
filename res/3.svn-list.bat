@echo off
setlocal enabledelayedexpansion

REM dir /b/ad >patch-list.txt

set list=patch-list.txt
if exist %list% del %list%

for /f "tokens=1 delims=" %%i in ( ' dir /ad/b "svn*master" "svn*patch" ') do (
	echo %%i
	if exist %%i.zip (
		call :getMD5 "%%i.zip" md5
		call :getSize "%%i.zip" size	
		echo !size!,!md5!,%%i.zip>>%list%	
	) else (
		echo %%i>>%list%
	)
)

pause
exit

REM echo ------获取文件大小--------
:getSize 
for %%i in ("%~1") do set "%2=%%~zi"
goto :eof

REM echo ------获取文件MD5--------
:getMD5 
set md5txt=md5.txt
if exist %md5txt% del %md5txt%
certutil -hashfile "%~1" MD5 > %md5txt% && (
	for /f "skip=1 tokens=1 delims=" %%i in ( %md5txt% ) do (
		set "%2=%%i"
		echo %%i
		del %md5txt%
		goto :eof
	)
)
set "%2="
goto :eof

::find /i /v ".jpg" svn-patch.txt >svn-patch11111.txt

pause
