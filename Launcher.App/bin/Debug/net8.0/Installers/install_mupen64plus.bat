@echo off
setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

echo 🟢 Installing Mupen64Plus (system package ONLY)...

:: Check if already installed
where mupen64plus.exe >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    for /f "usebackq delims=" %%i in (`where mupen64plus.exe`) do set M64P_BIN=%%i
    echo ✅ Mupen64Plus already installed: %M64P_BIN%
    exit /b 0
)

echo 🔄 Attempting to install Mupen64Plus...

:: Try winget first (Windows 10/11 with app installer)
where winget >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo 🌐 Trying: winget
    winget install --id m64p.m64p -e --source winget
)

:: If still not installed, try Chocolatey if available
where mupen64plus.exe >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    where choco >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo 🌐 Trying: choco
        choco install mupen64plus -y
    )
)

:: Verify installation
where mupen64plus.exe >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    for /f "usebackq delims=" %%i in (`where mupen64plus.exe`) do set M64P_BIN=%%i
    echo 🎉 SUCCESS: Mupen64Plus installed!
    echo 📍 Binary: %M64P_BIN%
    echo 📁 Config: %USERPROFILE%\AppData\Roaming\Mupen64Plus\
    echo 🧪 Test: "%M64P_BIN%" --help
    exit /b 0
)

echo(
echo ❌ Mupen64Plus installation failed!
echo(
echo Manual alternatives:
echo 1. Download Windows bundle from the official Mupen64Plus / m64p site and unzip.
echo 2. Use winget:   winget install m64p.m64p
echo 3. Use choco:    choco install mupen64plus
echo(
echo ❌ All install methods failed!
exit /b 1
