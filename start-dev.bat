@echo off
echo Starting MeTenTen Development Environment...
echo.

echo Starting API Server...
start "MeTenTen API" cmd /k "dotnet run --project MeTenTenAPI"

timeout /t 3 /nobreak > nul

echo Starting Blazor App...
start "MeTenTen Blazor" cmd /k "dotnet run --project MeTenTenBlazor"

echo.
echo Both applications are starting...
echo API will be available at: https://localhost:5170
echo Blazor app will be available at: https://localhost:7257
echo.
echo Press any key to exit...
pause > nul
