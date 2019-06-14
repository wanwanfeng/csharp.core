echo off
echo “使用方法：将meta文件夹下某个.proto文件拖到此文件上，会在csharp文件夹下自动生成对应的.cs文件。注，请勿直接打开此文件。”
protogen.exe -i:%cd%\%~n1.proto -o:%cd%\%~n1.cs
pause