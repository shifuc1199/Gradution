cd %~dp0
rd /s /q output\unpack_excel\
mkdir output\unpack_excel\
cd output\unpack_excel\
mkdir %~dp0\output\unpack_excel\test
cd %~dp0\output\unpack_excel\test
%~dp0\7z.exe x F:\UnityProject\GraduationProjectConfig\excel\test.xlsx
