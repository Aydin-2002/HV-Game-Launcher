@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET 8 SDK was not found. Install it from https://dotnet.microsoft.com/download/dotnet/8.0
  pause
  exit /b 1
)

echo Restoring project for win-x64...
dotnet restore "%~dp0Launcher.csproj" -r win-x64 --force
if errorlevel 1 (
  echo Restore failed.
  pause
  exit /b 1
)

echo Publishing project...
dotnet publish "%~dp0Launcher.csproj" -c Release -r win-x64 --self-contained false --no-restore
if errorlevel 1 (
  echo Build failed.
  pause
  exit /b 1
)

echo.
echo Build complete. Output:
echo bin\Release\net8.0-windows\win-x64\publish
pause
