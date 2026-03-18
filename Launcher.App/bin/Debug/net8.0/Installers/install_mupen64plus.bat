@echo off
setx PATH "%PATH%;C:\Users\code\AppData\Local\Mupen64Plus\Release" /M >nul 2>&1

echo 🟢 Installing Mupen64Plus (manual bundle 2.6.0)...

:: Check if already installed
where mupen64plus.exe >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    for /f "usebackq delims=" %%i in (`where mupen64plus.exe`) do set M64P_BIN=%%i
    echo ✅ Mupen64Plus already installed: %M64P_BIN%
    exit /b 0
)

set M64P_ROOT=%LOCALAPPDATA%\Mupen64Plus
set ZIP_URL=https://github.com/mupen64plus/mupen64plus-core/releases/download/2.6.0/mupen64plus-bundle-win64-2.6.0.zip
set ZIP_FILE=%TEMP%\mupen64plus-2.6.0.zip

echo 🔄 Manual installation to %M64P_ROOT%...

:: Clean previous attempt
if exist "%M64P_ROOT%" rmdir /s /q "%M64P_ROOT%" 2>nul
if exist "%ZIP_FILE%" del /q "%ZIP_FILE%" 2>nul

:: Download ZIP (bitsadmin works on all Windows versions)
echo 📥 Downloading...
bitsadmin /transfer "Mupen64Plus" "%ZIP_URL%" "%ZIP_FILE%"

if not exist "%ZIP_FILE%" (
    echo ❌ Download failed!
    exit /b 1
)

:: Extract using PowerShell (Windows 7+)
powershell -command "Expand-Archive -Path '%ZIP_FILE%' -DestinationPath '%M64P_ROOT%' -Force"

:: Find the actual exe (handles nested folder structure)
:: Replace the for loop with this (picks FIRST working exe):
for /r "%M64P_ROOT%" %%i in (mupen64plus.exe) do (
    "%%i" --help >nul 2>&1
    if !ERRORLEVEL! EQU 0 (
        set "M64P_BIN=%%i"
        goto :found_exe
    )
)
:found_exe

if not defined M64P_BIN (
    echo ❌ Extraction failed - mupen64plus.exe not found!
    exit /b 1
)

:: Add to PATH permanently
setx MUPEN64PLUS_BIN "%M64P_BIN%" /M >nul 2>&1
setx PATH "%PATH%;%M64P_ROOT%" /M >nul 2>&1

echo 🎉 SUCCESS: Mupen64Plus 2.6.0 installed!
echo 📍 Binary: %M64P_BIN%
echo 📁 Config: %USERPROFILE%\AppData\Roaming\Mupen64Plus\
echo 🔄 Restart terminal for PATH changes to take effect
echo 🧪 Test: "%M64P_BIN%" --help

"%M64P_BIN%" --help >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ✅ Binary verified working!
) else (
    echo ⚠️  Binary exists but --help failed
)
goto cleanup


cleanup:
if exist "%ZIP_FILE%" del /q "%ZIP_FILE%"
exit /b 0
