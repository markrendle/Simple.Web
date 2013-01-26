@echo off
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy ByPass ".\setup\SetupWebsite.ps1 -url 'sandbox.simpleweb.local' -name 'SimpleWeb.Sandbox' -location '..\src\Sandbox' "

pause