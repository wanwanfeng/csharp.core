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

set df=svn-master
if exist %df%.txt del %df%.txt
echo %version% >>%df%.txt	
echo %sv% >>%df%.txt	
for /f "tokens=1,2,3,4,5,6* delims= " %%i in ( temp.txt ) do (
	REM echo %%i,%%j,%%k,%%l,%%m,%%n,%%o
	if not "%%o"=="" (
		echo %%i,%%k,%%o>>%df%.txt	
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
echo 正在导出中...
echo 根据项目大时间长短不定，请耐心等待...
echo=
REM pause

set df=svn-master
svn export -r %sv% %url%@%sv%

if exist %df% rd /s/q %df%
rename %cd%\!foder! %df%

pause
exit

REM echo ----------获取当前文件夹名字------------
:getCdName 
for %%i in ("%cd%") do set "%1=%%~ni"
goto :eof