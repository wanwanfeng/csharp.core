echo off
echo ��ʹ�÷�������meta�ļ�����ĳ��.proto�ļ��ϵ����ļ��ϣ�����csharp�ļ������Զ����ɶ�Ӧ��.cs�ļ���ע������ֱ�Ӵ򿪴��ļ�����
protogen.exe -i:%cd%\%~n1.proto -o:%cd%\%~n1.cs
pause