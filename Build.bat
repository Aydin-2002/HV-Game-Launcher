@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET 8 SDK was not found.
  echo Install the .NET 8 SDK, then run Build.bat again.
  pause
  exit /b 1
)

echo Building standalone Windows x64 launcher...
dotnet publish "Launcher.csproj" -c Release -r win-x64 --self-contained true ^
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true -o "Publish"

if errorlevel 1 (
  echo Build failed.
  pause
  exit /b 1
)

copy /Y "config.ini" "Publish\config.ini" >nul
if not exist "Publish\Assets" mkdir "Publish\Assets"
echo.
echo Done: Publish\Launcher.exe
pause
