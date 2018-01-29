@echo off
setlocal enabledelayedexpansion

REM cd %cd%\..\

if exist temp.txt	del temp.txt

echo ----------------------------------------
echo ------------- ��ȡSVN�汾----------------
echo ----------------------------------------
echo= 

svn --version --quiet >temp.txt
set /p version=<temp.txt
echo SVN�汾��%version%

echo= 
echo ----------------------------------------
echo ------------- ��ȡ���ַ----------------
echo ----------------------------------------
echo= 

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

svn info --show-item url > temp.txt
set /p url=<temp.txt
echo ��ȡ���ַ��%url%

echo=    
echo ----------------------------------------
echo ----------- ��ȡ��߰汾��--------------
echo ----------------------------------------
echo= 
svn info --show-item last-changed-revision > temp.txt
set /p hv=<temp.txt
echo ��߰汾�ţ�%hv%

del temp.txt

echo=    
echo ----------------------------------------
echo --------��ȡ��ʼ������汾�� -----------
echo ----------------------------------------

set /p sv=��������ʼ�汾��(��������)��Ȼ��س���
if not defined sv (
	echo δ���룬��ȡ��߰汾��-1������
	set /a sv=%hv%-1
)
echo ��ʼ�汾���ǣ�%sv% 
echo= 

set /p ev=����������汾��(��������)��Ȼ��س���
if not defined ev (
	echo δ���룬��ȡ��߰汾�ţ�����
	set ev=%hv%
) 
if %ev% gtr %hv% (
	echo ����汾�Ŵ��ڿ�ʼ�汾�ţ���ȡ��߰汾�ţ�����
	set ev=%hv%
) else if %ev% lss %sv% (
	echo ����汾��С�ڿ�ʼ�汾�ţ���ȡ��߰汾�ţ�����
	set ev=%hv%
) else (
	set ev=%ev%
)
echo �����汾���ǣ�%ev%

if %sv% equ %ev% (
	echo ��ʼ�汾��������汾����ȣ�������diff������������˳�
	pause
	exit
) 

echo= 
echo ----------------------------------------
echo ---------- ��ȡ�汾�����б� ------------
echo ----------------------------------------

( svn diff -r %sv%:%ev% --summarize) > diff_patch.txt
echo= 
echo ----------------------------------------
echo -----------��ȡÿ�������ļ�-------------
echo ----------------------------------------

REM pause
set patch=svn-patch
if exist %patch% rd /s/q %patch%

if exist  diff_list.txt del diff_list.txt
for /f "tokens=1* delims= " %%i in (diff_patch.txt) do (
	REM echo %%i--%%j--%%~zj
	REM echo %%~dj%%~pj%%~nj%%~xj
	REM echo f | xcopy "%cd%\%%j" "%cd%\diffback\%%j" /e /f /y
	call :getFile "%%j" "%%i"
)

echo=
echo ----------------------------------------
echo ----��ȡÿ�������ı����ļ���С��MD5-----
echo ----------------------------------------

REM pause
if exist %patch%.txt del %patch%.txt
echo %version% >> %patch%.txt
echo %sv% >> %patch%.txt
echo %ev% >> %patch%.txt
for /f "tokens=1,2 delims=," %%i in (diff_list.txt) do (
	call :getMD5 "%cd%\%patch%\%%j" md5
	call :getSize "%cd%\%patch%\%%j" size	
	if %%i==A call :getVersion "%%j" vv
	if %%i==M call :getVersion "%%j" vv	
	( echo !vv!,%%i,!size!,!md5!,%%j) >> %patch%.txt
)
rem pause

del diff_patch.txt
del diff_log.txt
del diff_list.txt

rem move %patch%.txt %patch%

call :getCdName foder
set fName=svn-!foder!-%sv%-%ev%-patch
if exist  "%cd%\%fName%" rd /s/q "%cd%\%fName%"
mkdir "%cd%\%fName%"

rename %patch%.txt %fName%.txt
move %fName%.txt %cd%/../

rename "%patch%" "!foder!"
move !foder! "%fName%"

if exist  "%cd%/../%fName%" rd /s/q "%cd%/../%fName%"
move "%fName%" %cd%/../
pause

goto :endLast
exit

REM echo ------��ȡ�����汾�ļ�--------
:getFile
set aaa=%~1
set bbb=%~2
REM ·����\�滻��/
set aaa=!aaa:\=/!
REM ������Ŀ¼
set df=%patch%\%~1
if %bbb%==D (
	echo %bbb%,!aaa! >>diff_list.txt
)else (	
	if not exist "!df!" (	mkdir "!df!" && rd /s/q "!df!" )
	echo "%url%/!aaa!@%ev% -> !df!"
	svn cat -r %ev% "%url%/!aaa!@%ev%" >!df! && ( ( echo %bbb%,!aaa!)>>diff_list.txt ) || echo "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
)
::pause
goto :eof

REM echo ------������־��ȡ�ļ������޶�����߰汾��---------
:getVersion
set aaa=%~1
REM ·����\�滻��/
set aaa=!aaa:\=/!
rem echo log "%url%/!aaa!@%ev%"
svn log -r %ev%:0 "%url%/!aaa!@%ev%" -q -l1 --stop-on-copy >diff_log.txt
for /f "skip=1 eol=- tokens=1 delims=|" %%i in ( diff_log.txt ) do (
	set vv=%%i
	set vv=!vv:r=!
	set vv=!vv: =!
	echo "%url%/%aaa%@%ev%->!vv!"
	set "%2=!vv!"
	goto :eof
)
REM pause
goto :eof

REM echo ----------��ȡ��ǰ�ļ�������------------
:getCdName 
for %%i in ("%cd%") do set "%1=%%~ni"
goto :eof

REM echo ------��ȡ�ļ���С--------
:getSize 
for %%i in ("%~1") do set "%2=%%~zi"
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

REM echo ------�ų��ļ�����--------
:endLast

REM find /i /v ".jpg" %patch%.txt >%patch%1.txt
goto :eof



REM @echo off
REM setlocal enabledelayedexpansion

REM find /i /v ".jpg" svn-patch.txt >svn-patch11111.txt

REM pause

