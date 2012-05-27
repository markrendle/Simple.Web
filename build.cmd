@echo off

SET TARGET="Default"

IF NOT [%1]==[] (set TARGET="%1")
  
"packages\FAKE.1.64.6\tools\Fake.exe" "build.fsx" "target=%TARGET%"

exit /b %errorlevel%
