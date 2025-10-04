Write-Host "Starting MeTenTen Development Environment..." -ForegroundColor Green
Write-Host ""

Write-Host "Starting API Server..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "MeTenTenAPI", "--launch-profile", "http" -WindowStyle Normal

Start-Sleep -Seconds 3

Write-Host "Starting Blazor App..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "MeTenTenBlazor", "--launch-profile", "http" -WindowStyle Normal

Write-Host ""
Write-Host "Both applications are starting..." -ForegroundColor Green
Write-Host "API will be available at: http://0.0.0.0:5170" -ForegroundColor Cyan
Write-Host "Blazor app will be available at: http://0.0.0.0:5283" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor White
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
