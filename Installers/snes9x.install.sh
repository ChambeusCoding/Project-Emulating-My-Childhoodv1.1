#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing SNES9x (user-safe installer)..."

if command -v snes9x >/dev/null 2>&1; then
    echo "✅ SNES9x already installed: $(command -v snes9x)"
    exit 0
fi

INSTALL_DIR="$HOME/.local/bin"
mkdir -p "$INSTALL_DIR"

if command -v flatpak >/dev/null 2>&1; then
    echo "🔄 Trying Flatpak..."
    flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo || true
    flatpak install --user -y flathub org.snes9x.Snes9x || true
    if flatpak list | grep -qi snes9x; then
        echo "🎉 SUCCESS: Flatpak (org.snes9x.Snes9x)"
        echo "📝 Run with: flatpak run org.snes9x.Snes9x"
        exit 0
    fi
fi

echo "🔄 Trying AppImage..."
cd /tmp
wget -q https://github.com/snes9xgit/snes9x/releases/latest/download/snes9x.AppImage -O snes9x.AppImage || true
if [[ -f snes9x.AppImage ]]; then
    chmod +x snes9x.AppImage
    mv snes9x.AppImage "$INSTALL_DIR/snes9x"
    echo "🎉 SUCCESS: AppImage installed in $INSTALL_DIR"
    echo "📝 Run with: $INSTALL_DIR/snes9x"
    exit 0
fi

echo "🔄 Trying GitHub prebuilt release..."
cd /tmp
wget -q https://github.com/snes9xgit/snes9x/releases/download/1.63/snes9x-1.63-linux.tar.gz -O snes9x.tar.gz || true
if [[ -f snes9x.tar.gz ]]; then
    tar xzf snes9x.tar.gz
    cp snes9x-*/snes9x "$INSTALL_DIR/snes9x" || true
    if [[ -x "$INSTALL_DIR/snes9x" ]]; then
        echo "🎉 SUCCESS: Installed from GitHub prebuilt"
        echo "📝 Run with: $INSTALL_DIR/snes9x"
        exit 0
    fi
fi

echo "❌ No installation method succeeded."
echo "💡 Manual install guide: https://github.com/snes9xgit/snes9x/releases"
exit 1
