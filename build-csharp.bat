@echo off
echo Building Video Downloader C# Version...
echo.

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK is not installed!
    echo Please download and install from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Clean previous builds
echo Cleaning previous builds...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
if exist publish rmdir /s /q publish

REM Restore packages
echo Restoring packages...
dotnet restore

REM Build the project
echo Building project...
dotnet build -c Release

REM Publish as single file
echo Publishing as single executable...
dotnet publish -c Release -o publish

REM Download yt-dlp if not present
if not exist "publish\yt-dlp.exe" (
    echo.
    echo Downloading yt-dlp.exe...
    powershell -Command "Invoke-WebRequest -Uri 'https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe' -OutFile 'publish\yt-dlp.exe'"
)

REM Create a README for the investigator
echo Creating README...
(
echo Video Downloader - Instructions
echo ==============================
echo.
echo 1. Make sure ffmpeg is installed:
echo    - Download from: https://www.gyan.dev/ffmpeg/builds/
echo    - Extract ffmpeg.exe to this folder or add to PATH
echo.
echo 2. Run VideoDownloader.exe
echo.
echo 3. The application includes:
echo    - VideoDownloader.exe - Main application
echo    - yt-dlp.exe - Video download engine
echo.
echo 4. First time setup:
echo    - Go to Settings tab
echo    - Enter credentials if needed for private videos
echo    - Save settings
echo.
echo 5. To download a video:
echo    - Paste the video URL
echo    - Select quality
echo    - Click Download Video
echo.
echo Note: Downloaded videos will be saved to the folder you specify.
) > publish\README.txt

echo.
echo Build complete!
echo.
echo Output files are in: publish\
echo - VideoDownloader.exe (Main application)
echo - yt-dlp.exe (Download engine)
echo - README.txt (Instructions)
echo.
echo IMPORTANT: The investigator must also have ffmpeg installed!
echo.
pause