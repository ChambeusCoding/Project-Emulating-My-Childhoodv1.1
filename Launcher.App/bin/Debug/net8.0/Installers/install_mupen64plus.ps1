param(
    [string]$Version = "2.6.0"
)

$ErrorActionPreference = "Stop"

$M64P_ROOT = Join-Path $env:LOCALAPPDATA "Mupen64Plus"
$ZIP_URL = "https://github.com/mupen64plus/mupen64plus-core/releases/download/$Version/mupen64plus-bundle-win64-$Version.zip"
$ZIP_FILE = Join-Path $env:TEMP "mupen64plus-$Version.zip"

Write-Host "Installing Mupen64Plus $Version..."

$existing = Get-Command mupen64plus.exe -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Already installed: $($existing.Source)"
    exit 0
}

if (Test-Path $M64P_ROOT) {
    Remove-Item $M64P_ROOT -Recurse -Force
}
if (Test-Path $ZIP_FILE) {
    Remove-Item $ZIP_FILE -Force
}

Write-Host "Downloading..."
Invoke-WebRequest -Uri $ZIP_URL -OutFile $ZIP_FILE

Write-Host "Extracting..."
Expand-Archive -LiteralPath $ZIP_FILE -DestinationPath $M64P_ROOT -Force

$M64P_BIN = Get-ChildItem $M64P_ROOT -Recurse -Filter "mupen64plus.exe" | Select-Object -First 1

if (-not $M64P_BIN) {
    throw "mupen64plus.exe not found after extraction."
}

Write-Host "Installed: $($M64P_BIN.FullName)"

[Environment]::SetEnvironmentVariable("MUPEN64PLUS_BIN", $M64P_BIN.FullName, "Machine")

$machinePath = [Environment]::GetEnvironmentVariable("Path", "Machine")
if ($machinePath -notlike "*$M64P_ROOT*") {
    [Environment]::SetEnvironmentVariable("Path", "$machinePath;$M64P_ROOT", "Machine")
}

try {
    & $M64P_BIN.FullName --help | Out-Null
    Write-Host "Binary verified successfully."
} catch {
    Write-Host "Warning: binary exists but --help failed."
}

Remove-Item $ZIP_FILE -Force -ErrorAction SilentlyContinue
Write-Host "Done."