@echo off
echo Building Video Downloader C# Version (Release Mode)...
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

REM Build the project in Release mode
echo Building project in Release mode...
dotnet build -c Release

REM Publish as single file with optimizations
echo Publishing as single executable (optimized)...
dotnet publish -c Release -o publish --self-contained true -r win-x64 ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:DebugType=none ^
    /p:DebugSymbols=false ^
    /p:EnableCompressionInSingleFile=true ^
    /p:OptimizationPreference=Size

if errorlevel 1 (
    echo.
    echo Build failed! Please check the errors above.
    pause
    exit /b 1
)

REM Create publish directory if it doesn't exist
if not exist publish mkdir publish

REM Download yt-dlp if not present
if not exist "publish\yt-dlp.exe" (
    echo.
    echo Downloading yt-dlp.exe...
    powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; try { Invoke-WebRequest -Uri 'https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe' -OutFile 'publish\yt-dlp.exe' } catch { Write-Host 'Failed to download yt-dlp.exe. Please download manually from: https://github.com/yt-dlp/yt-dlp/releases' }"
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
echo ============================================
echo Build complete! (Release Mode - Optimized)
echo ============================================
echo.
echo Output files are in: publish\
echo - VideoDownloader.exe (Main application)
echo - yt-dlp.exe (Download engine)
echo - README.txt (Instructions)
echo.

REM Display file sizes
echo File sizes:
for %%F in (publish\VideoDownloader.exe) do echo VideoDownloader.exe: %%~zF bytes
for %%F in (publish\yt-dlp.exe) do echo yt-dlp.exe: %%~zF bytes

echo.
echo IMPORTANT: The investigator must also have ffmpeg installed!
echo.
pause