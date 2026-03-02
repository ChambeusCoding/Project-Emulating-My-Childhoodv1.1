#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Mupen64Plus (system package ONLY)..."

if command -v mupen64plus >/dev/null 2>&1; then
    echo "✅ Mupen64Plus already installed: $(command -v mupen64plus)"
    exit 0
fi

EMUDIR="$HOME/.local/share/emulators/mupen64plus"
BINDIR="$HOME/.local/bin"
rm -rf "$EMUDIR" "$BINDIR/mupen64plus"

echo "🔄 Installing Mupen64Plus via apt (20MB, completely safe)..."
sudo apt update
sudo apt install -y mupen64plus-ui-console mupen64plus-video-rice

if command -v mupen64plus >/dev/null 2>&1; then
    echo "🎉 SUCCESS: Mupen64Plus installed"
    echo "📍 Binary: $(command -v mupen64plus)"
    echo "📁 Config: ~/.mupen64plus/"
    echo "🧪 Test: mupen64plus --help"
    exit 0
fi

echo "❌ Installation failed"
exit 1
