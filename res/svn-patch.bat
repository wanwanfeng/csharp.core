@echo off
setlocal enabledelayedexpansion

REM cd %cd%\..\

if exist temp.txt	del temp.txt

echo ----------------------------------------
echo ------------- 获取SVN版本----------------
echo ----------------------------------------
echo= 

svn --version --quiet >temp.txt
set /p version=<temp.txt
echo SVN版本：%version%

echo= 
echo ----------------------------------------
echo ------------- 获取库地址----------------
echo ----------------------------------------
echo= 
svn info --show-item url > temp.txt
set /p url=<temp.txt
echo 获取库地址：%url%

set /p fo=请输入目标目录，然后回车：
if not defined fo (
	echo 未输入！
	pause
	exit
)
set url=%url%/%fo%
echo 获取目标地址：%url%

echo=    
echo ----------------------------------------
echo ----------- 获取最高版本号--------------
echo ----------------------------------------
echo= 
svn info --show-item last-changed-revision > temp.txt
set /p hv=<temp.txt
echo 最高版本号：%hv%

del temp.txt

echo=    
echo ----------------------------------------
echo --------获取开始与结束版本号 -----------
echo ----------------------------------------

set /p sv=请输入起始版本号(输入数字)，然后回车：
if not defined sv (
	echo 未输入，已取最高版本号-1！！！
	set /a sv=%hv%-1
)
echo 起始版本号是：%sv% 
echo= 

set /p ev=请输入结束版本号(输入数字)，然后回车：
if not defined ev (
	echo 未输入，已取最高版本号！！！
	set ev=%hv%
) 
if %ev% gtr %hv% (
	echo 输入版本号大于开始版本号，已取最高版本号！！！
	set ev=%hv%
) else if %ev% lss %sv% (
	echo 输入版本号小于开始版本号，已取最高版本号！！！
	set ev=%hv%
) else (
	set ev=%ev%
)
echo 结束版本号是：%ev%

if %sv% equ %ev% (
	echo 起始版本号与结束版本号相等，不符合diff条件，任意键退出
	pause
	exit
) 

echo= 
echo ----------------------------------------
echo ---------- 获取版本差异列表 ------------
echo ----------------------------------------

svn diff -r %sv%:%ev% --summarize > diff_patch.txt

echo= 
echo ----------------------------------------
echo -----------获取每个差异文件-------------
echo ----------------------------------------

REM pause
set patch=svn-patch
if exist %patch% rd /s/q %patch%

if exist  diff_list.txt del diff_list.txt
for /f "tokens=1* delims= " %%i in (diff_patch.txt) do (
	REM echo %%i--%%j--%%~zj
	REM echo %%~dj%%~pj%%~nj%%~xj
	REM echo f | xcopy "%cd%\%%j" "%cd%\diffback\%%j" /e /f /y
	call :getFile "%%j" %%i
)

echo=
echo ----------------------------------------
echo ------获取每个差异文本的文件大小--------
echo ----------------------------------------

REM pause
if exist %patch%.txt del %patch%.txt

echo %version% >> %patch%.txt
echo %sv% >> %patch%.txt
echo %ev% >> %patch%.txt

for /f "tokens=1,2 delims=," %%i in (diff_list.txt) do (
	call :getSize %cd%\%patch%\%%j size	
	if %%i==A call :getVersion "%%j" vv
	if %%i==M call :getVersion "%%j" vv	
	echo !vv!,%%i,!size!,%%j >> %patch%.txt
	REM pause
)
pause

del diff_patch.txt
del diff_log.txt
del diff_list.txt

goto :endLast
exit

REM echo ------拉取结束版本文件--------
:getFile
set aaa=%~1
set bbb=%~2
REM 路径内\替换成/
set aaa=!aaa:\=/!
REM 创建父目录
set df=%patch%\%~1
if not exist !df! (	mkdir !df!&&rd /s/q !df! )
echo "%url%/!aaa!@%ev% -> !df!"
if %bbb%==D (
	echo %bbb%,!aaa! >>diff_list.txt
)else (
	svn cat -r %ev% "%url%/!aaa!@%ev%" >!df! && ( echo %bbb%,!aaa!>>diff_list.txt )
)
REM pause
goto :eof

REM echo ------根据日志获取文件低于限定的最高版本号---------
:getVersion
set aaa=%~1
REM 路径内\替换成/
set aaa=!aaa:\=/!
REM 去除最后一个空格
set aaa=%aaa:~0,-1%
echo log "%url%/!aaa!@%ev%"
svn log -r %ev%:0 "%url%/!aaa!@%ev%" -q -l1 --stop-on-copy >diff_log.txt
for /f "skip=1 eol=- tokens=1 delims=|" %%i in ( diff_log.txt ) do (
	set vv=%%i
	set vv=!vv:r=!
	set vv=!vv: =!
	echo "%url%/%aaa%@%ev%->!vv!"
	set "%2=!vv!"
)
REM pause
goto :eof

REM echo ------获取文件大小--------
:getSize 
for %%i in (%~1) do set "%2=%%~zi"
goto :eof


REM echo ------排除文件操作--------
:endLast

find /i /v ".jpg" %patch%.txt >%patch%1.txt
goto :eof




