mkdir C:\java\github\tello-test
mkdir C:\java\github\tello-test\tello
cd /d C:\java\github\tello-test
xcopy C:\java\github\tello-link\tello-emu\bin\Release\net7.0-windows\*.* . /s /y
xcopy C:\java\github\tello-link\tello-link\bin\Release\net7.0-windows\*.* . /s /y
xcopy C:\java\github\tello-link\tello-html\*.* tello\ /s /y
pause
