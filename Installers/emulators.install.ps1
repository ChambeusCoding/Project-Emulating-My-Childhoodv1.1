$BaseDir = "C:\Emulators"
New-Item -ItemType Directory -Force -Path $BaseDir | Out-Null

function Download-And-Extract {
    param (
        [string]$Name,
        [string]$Url,
        [string]$ExeName
    )

    $InstallDir = Join-Path $BaseDir $Name
    $ZipPath = "$InstallDir.zip"

    Write-Host "`nInstalling $Name..."

    New-Item -ItemType Directory -Force -Path $InstallDir | Out-Null

    Invoke-WebRequest -Uri $Url -OutFile $ZipPath
    Expand-Archive -Force $ZipPath $InstallDir
    Remove-Item $ZipPath

    if (Test-Path (Join-Path $InstallDir $ExeName)) {
        Write-Host "$Name installed successfully."
    } else {
        Write-Host "⚠ $Name installed, but executable not found automatically."
    }
}


Download-And-Extract `
    -Name "RetroArch" `
    -Url "https://buildbot.libretro.com/stable/1.18.0/windows/x86_64/RetroArch.7z" `
    -ExeName "retroarch.exe"


Download-And-Extract `
    -Name "Cemu" `
    -Url "https://github.com/cemu-project/Cemu/releases/latest/download/cemu_2.0_windows.zip" `
    -ExeName "Cemu.exe"


Download-And-Extract `
    -Name "Dolphin" `
    -Url "https://dl.dolphin-emu.org/builds/dolphin-master-latest-x64.zip" `
    -ExeName "Dolphin.exe"


Download-And-Extract `
    -Name "Citra" `
    -Url "https://archive.org/download/citra-nightly-2104/citra-windows-mingw-20221004.zip" `
    -ExeName "citra-qt.exe"


Download-And-Extract `
    -Name "melonDS" `
    -Url "https://github.com/melonDS-emu/melonDS/releases/latest/download/melonDS_windows_x64.zip" `
    -ExeName "melonDS.exe"


Download-And-Extract `
    -Name "Snes9x" `
    -Url "https://github.com/snes9xgit/snes9x/releases/latest/download/snes9x-x64.zip" `
    -ExeName "snes9x-x64.exe"

Write-Host "`nAll emulators installed in C:\Emulators"
