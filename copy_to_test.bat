cd /d %~dp0
mkdir bin
mkdir bin\tello
cd bin
xcopy ..\tello-emu\bin\Release\net7.0-windows\*.* . /s /y
xcopy ..\tello-link\bin\Release\net7.0-windows\*.* . /s /y
xcopy ..\tello-link\tello-html\*.* tello\ /s /y
pause
