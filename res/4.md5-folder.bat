@echo off
setlocal enabledelayedexpansion

(
	REM echo test
	echo image
	REM echo test2
	REM echo test3
	REM echo test1
)> temp.txt
REM 密钥  "-e":末尾追加密钥 "-h":开始追加密钥
call :encryptMd5 "" "-e"
pause 
exit

:encryptMd5
set key=%~1
set isHead=%~2
if exist xx.txt ( del xx.txt )
if exist md5_list.txt ( del md5_list.txt )
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
	
	echo !filePath!
	REM ::相对路径
	if %isHead%==-e	( 
		( set /p a=!filePath!%key%<nul)>xx.txt
	) else (
		( set /p a=%key%!filePath!<nul)>xx.txt 
	)
	call :getMD5 xx.txt  md5
	set filePathMd5=temp\!md5!
	REM echo !filePathMd5!
	if not exist "!filePathMd5!" ( mkdir "!filePathMd5!")		
	REM pause
	
	echo !fileName!
	REM ::文件名
	if %isHead%==-e	( 
		( set /p a=!fileName!%key%<nul)>xx.txt
	) else (
		( set /p a=%key%!fileName!<nul)>xx.txt 
	)
	call :getMD5 xx.txt  md5
	set fileNameMd5=!md5!
	rem %%~xi
	echo !filePathMd5!\!fileNameMd5!
	if exist "!filePathMd5!!fileNameMd5!" ( del "!filePathMd5!!fileNameMd5!" )
	REM pause

	echo !filePathMd5!\!fileNameMd5! >> md5_list.txt
	echo f | xcopy "%%i" "%cd%\!filePathMd5!\!fileNameMd5!" /y/s/q
	REM pause
)
goto :eof

:getFilePath
set folder=%~1
set "%~2=!folder:%cd%\=!"
goto :eof

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
		REM echo %%i
		del %md5txt%
		goto :eof
	)
) || ( set "%2=" )

goto :eof
