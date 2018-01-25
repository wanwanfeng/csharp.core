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

set /p fo=������Ŀ��Ŀ¼��Ȼ��س���
if not defined fo (
	echo δ���룡
	pause
	exit
)

if not exist %fo% (
	echo Ŀ���ַ�����ڣ�
	pause
	exit
)

del temp.txt
cd %fo%

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
REM echo ------- ����Ŀ��汾�Ÿɾ����� ---------
REM echo ----------------------------------------

set /p yes=�Ƿ񵼳�Ŀ��汾���ļ���y/n����Ȼ��س���

if not defined yes ( exit )
if not %yes%==y ( exit )

call :getCdName foder
echo !foder! �汾 %sv%
echo ���ڵ�����... %url%
echo ������Ŀ��ʱ�䳤�̲����������ĵȴ�...
echo=
REM pause

set dff=svn-!foder!-0-%sv%-master
set df=%dff%\!foder!
if exist "%df%" rd /s/q "%df%" mkdir "%df%"
svn export -r %sv% %url%@%sv% %df%

pause 
REM echo=    
REM echo ----------------------------------------
REM echo ---------------- ׷��MD5----------------
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

rem �ƶ�
move %master%.txt %dff%
if exist "%cd%/../%dff%" rd /s/q "%cd%/../%dff%"
move %dff% %cd%/../
pause
exit

REM echo ----------��ȡ��ǰ�ļ�������------------
:getCdName 
for %%i in ("%cd%") do set "%1=%%~ni"
goto :eof

REM echo ------��ȡ�ļ�MD5--------
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