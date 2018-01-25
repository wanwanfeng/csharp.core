@echo off
setlocal enabledelayedexpansion

REM dir /b/ad >patch-list.txt

set list=patch-list.txt
if exist %list% del %list%

for /f "tokens=1 delims=" %%i in ( ' dir /ad /b /od "svn*master" "svn*patch" ') do (
	echo %%i
	if exist %%i\svn-patch.txt ( set ha=%%i\svn-patch.txt)
	if exist %%i\svn-master.txt ( set ha=%%i\svn-master.txt) 
	if not exist %%i.zip (
		call :getMD5 "!ha!" md5
		call :getSize "!ha!" size
		echo !size!,!md5!,%%i>>%list%
	)else (
		call :getMD5 "!ha!" md5
		call :getSize "!ha!" size
		call :getMD5 "%%i.zip" md52
		call :getSize "%%i.zip" size2
		echo !size!,!md5!,!size2!,!md52!,%%i.zip>>%list%
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
		set aaa=%%i
		set aaa=!aaa: =!
		set "%2=!aaa!"
		echo !aaa!
		del %md5txt%
		goto :eof
	)
) || ( echo "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" )
set "%2="
goto :eof

::find /i /v ".jpg" svn-patch.txt >svn-patch11111.txt

pause
