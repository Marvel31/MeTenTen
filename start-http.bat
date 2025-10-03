@echo off
echo Starting MeTenTen Development Environment (HTTP Only)...
echo.

REM Start API Server in a new command prompt
echo Starting API Server (HTTP)...
start cmd /k "cd MeTenTenAPI && dotnet run --launch-profile http"

REM Start Blazor App in another new command prompt
echo Starting Blazor App (HTTP)...
start cmd /k "cd MeTenTenBlazor && dotnet run --launch-profile http"

echo.
echo Both applications are starting...
echo API will be available at: http://localhost:5170
echo Blazor app will be available at: http://localhost:5283
echo.
echo Press any key to exit...
pause > nul
