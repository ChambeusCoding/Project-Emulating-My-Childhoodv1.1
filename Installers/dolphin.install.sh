#!/usr/bin/env bash
set -e

echo "🟢 Installing Dolphin (Flatpak)..."

if flatpak list | grep -q org.DolphinEmu.dolphin-emu; then
  echo "✅ Dolphin already installed"
  exit 0
fi

flatpak install -y flathub org.DolphinEmu.dolphin-emu

echo "🎉 Dolphin installed"
