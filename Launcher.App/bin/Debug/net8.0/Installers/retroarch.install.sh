#!/bin/bash
#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


echo "Installing RetroArch via APT..."
sudo apt update
sudo apt install -y retroarch retroarch-assets

echo "Creating emulator core directories..."
EMU_CORE_DIR="$HOME/.local/share/emulators/RetroArch/cores"
mkdir -p "$EMU_CORE_DIR"

echo "Done! Launch RetroArch and use its Online Updater to install the Citra core."
echo "RetroArch installed as: $(which retroarch)"
echo "Cores folder created at: $EMU_CORE_DIR"
echo "Now open RetroArch → Online Updater → Core Downloader → Download 'citra' core."

