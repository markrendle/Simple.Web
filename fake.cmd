@echo off

SET TARGET="Default"

IF NOT [%1]==[] (set TARGET="%1")
  
"tools\FAKE\Fake.exe" "build.fsx" "target=%TARGET%"

exit /b %errorlevel%
