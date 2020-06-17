@echo off

set PATH="C:\Windows\System32\WindowsPowerShell\v1.0\;"

msbuild>NUL && (
    echo MSBuild already in PATH
 ) || (
    for /F "tokens=* USEBACKQ" %%F in (`Powershell.exe -executionpolicy remotesigned -File  .\tools\findmsbuild.ps1`) DO (
        set msbuildpath=%%F
    )
    echo MSBuild path Found: %msbuildpath%
    set PATH="%PATH%;%msbuildpath%;"
    
 )

echo Building Project
msbuild