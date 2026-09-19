@echo off
setlocal

where butler >nul 2>&1
if errorlevel 1 (
    echo Butler was not found on PATH.
    echo Install it or add butler.exe to PATH:
    echo https://itch.io/docs/butler/installing.html
    pause
    exit /b 1
)

if not exist "%~dp0Builds\web\index.html" (
    echo Web build not found:
    echo %~dp0Builds\web
    echo.
    echo Build the WebGL player from Unity first.
    pause
    exit /b 1
)

pushd "%~dp0Builds"

butler push web archimagus/the-orion-trail:web
set "RESULT=%ERRORLEVEL%"

popd

if not "%RESULT%"=="0" (
    echo.
    echo Upload failed.
    pause
    exit /b %RESULT%
)

echo.
echo Upload complete.
pause