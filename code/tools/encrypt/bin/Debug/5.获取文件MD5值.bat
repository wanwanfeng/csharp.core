@echo off
(
	REM echo test
	echo image
	REM echo test2
	REM echo test3
	REM echo test1
) > temp.txt
REM 密钥  "-e":末尾追加密钥 "-h":开始追加密钥
call tools\md5_encrypt.bat "dsffdstdghfghfg" "-e"
pause