@echo off
setlocal enabledelayedexpansion

REM cd %cd%\..\

if exist temp.txt	del temp.txt

echo ----------------------------------------
echo ------------- ��ȡSVN�汾----------------
echo ----------------------------------------

svn --version --quiet >temp.txt
set /p version=<temp.txt
echo SVN�汾��%version%

echo= 
echo ----------------------------------------
echo ------------- ��ȡ���ַ----------------
echo ----------------------------------------

svn info --show-item url >temp.txt
set /p url=<temp.txt
echo ��ȡ���ַ��%url%

echo=    
echo ----------------------------------------
echo ----------- ��ȡ��߰汾��--------------
echo ----------------------------------------

svn info --show-item last-changed-revision >temp.txt
set /p hv=<temp.txt
echo ��߰汾�ţ�%hv%

del temp.txt

echo=    
echo ----------------------------------------
echo --------- ��ȡĿ��汾�� -----------
echo ----------------------------------------

set /p sv=������Ŀ��汾��(��������)��Ȼ��س���
if not defined sv (
	echo δ���룬��ȡ��߰汾�ţ�����
	set /a sv=%hv%
)
echo Ŀ��汾���ǣ�%sv% 
echo ���ڻ�ȡĿ��汾���ļ���ϸ��Ϣ...
echo ���Ե�...

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
REM echo ------- ����Ŀ��汾�Ÿɾ����� ---------
REM echo ----------------------------------------

set /p yes=�Ƿ񵼳�Ŀ��汾���ļ���y/n����Ȼ��س���

if not defined yes ( exit )
if not %yes%==y ( exit )

call :getCdName foder
echo !foder! �汾 %sv%
echo ���ڵ�����...
echo ������Ŀ��ʱ�䳤�̲����������ĵȴ�...
echo=
REM pause

set df=svn-master
svn export -r %sv% %url%@%sv%

if exist %df% rd /s/q %df%
rename %cd%\!foder! %df%

pause
exit

REM echo ----------��ȡ��ǰ�ļ�������------------
:getCdName 
for %%i in ("%cd%") do set "%1=%%~ni"
goto :eof