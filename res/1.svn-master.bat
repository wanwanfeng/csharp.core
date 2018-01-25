@echo off
setlocal enabledelayedexpansion

REM cd %cd%\..\

if exist temp.txt	del temp.txt

echo ----------------------------------------
echo ------------- 获取SVN版本----------------
echo ----------------------------------------

svn --version --quiet >temp.txt
set /p version=<temp.txt
echo SVN版本：%version%

echo= 
echo ----------------------------------------
echo ------------- 获取库地址----------------
echo ----------------------------------------

set /p fo=请输入目标目录，然后回车：
if not defined fo (
	echo 未输入！
	pause
	exit
)

if not exist %fo% (
	echo 目标地址不存在！
	pause
	exit
)

del temp.txt
cd %fo%

svn info --show-item url >temp.txt
set /p url=<temp.txt
echo 获取库地址：%url%

echo=    
echo ----------------------------------------
echo ----------- 获取最高版本号--------------
echo ----------------------------------------

svn info --show-item last-changed-revision >temp.txt
set /p hv=<temp.txt
echo 最高版本号：%hv%

del temp.txt

echo=    
echo ----------------------------------------
echo --------- 获取目标版本号 -----------
echo ----------------------------------------

set /p sv=请输入目标版本号(输入数字)，然后回车：
if not defined sv (
	echo 未输入，已取最高版本号！！！
	set /a sv=%hv%
)
echo 目标版本号是：%sv% 
echo 正在获取目标版本号文件详细信息...
echo 请稍等...

svn list -r %sv% %url%@%sv% -R -v >temp.txt

set master=svn-master
if exist %master%.txt del %master%.txt
echo %version% >>%master%.txt	
echo %sv% >>%master%.txt	
for /f "tokens=1,2,3,4,5,6* delims= " %%i in ( temp.txt ) do (
	REM echo %%i,%%j,%%k,%%l,%%m,%%n,%%o
	if not "%%o"=="" (
		 ( echo %%i,%%k,%%o)>>%master%.txt	
	)
)
del temp.txt

REM echo=    
REM echo ----------------------------------------
REM echo ------- 导出目标版本号干净工程 ---------
REM echo ----------------------------------------

set /p yes=是否导出目标版本号文件（y/n），然后回车：

if not defined yes ( exit )
if not %yes%==y ( exit )

call :getCdName foder
echo !foder! 版本 %sv%
echo 正在导出中... %url%
echo 根据项目大时间长短不定，请耐心等待...
echo=
REM pause

set dff=svn-!foder!-0-%sv%-master
set df=%dff%\!foder!
if exist "%df%" rd /s/q "%df%" mkdir "%df%"
svn export -r %sv% %url%@%sv% %df%

pause 
REM echo=    
REM echo ----------------------------------------
REM echo ---------------- 追加MD5----------------
REM echo ----------------------------------------
rename %master%.txt temp.txt
set master=svn-master
if exist %master%.txt del %master%.txt
echo %version% >>%master%.txt	
echo %sv% >>%master%.txt	
for /f "skip=2 tokens=1,2,3 delims=," %%i in ( temp.txt ) do (
	call :getMD5 "%df%/%%k" md5	
	( echo %%i,%%j,!md5!,%%k) >>%master%.txt
)
del temp.txt

rem 移动
move %master%.txt %dff%
if exist "%cd%/../%dff%" rd /s/q "%cd%/../%dff%"
move %dff% %cd%/../
pause
exit

REM echo ----------获取当前文件夹名字------------
:getCdName 
for %%i in ("%cd%") do set "%1=%%~ni"
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