Write-Host "Starting MeTenTen with API..." -ForegroundColor Green

# API 서버 시작
Write-Host "Starting API Server..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit -Command `"cd '..\MeTenTenAPI'; dotnet run`""

# 잠시 대기
Start-Sleep -Seconds 3

# Blazor 앱 시작
Write-Host "Starting Blazor App..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit -Command `"cd 'MeTenTenBlazor'; dotnet run`""

# 브라우저 열기
Start-Sleep -Seconds 5
Write-Host "Opening browser..." -ForegroundColor Green
Start-Process "https://localhost:7257"

Write-Host "Both applications are running!" -ForegroundColor Green
Write-Host "API: https://localhost:5170" -ForegroundColor Cyan
Write-Host "Blazor: https://localhost:7257" -ForegroundColor Cyan
