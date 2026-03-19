#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


echo "🟢 Installing Dolphin (Flatpak)..."

if flatpak list | grep -q org.DolphinEmu.dolphin-emu; then
  echo "✅ Dolphin already installed"
  exit 0
fi

flatpak install -y flathub org.DolphinEmu.dolphin-emu

echo "🎉 Dolphin installed"
