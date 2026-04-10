#!/usr/bin/env bash
set -e

echo "🟢 Installing Cemu (package-based)..."

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi

if command -v cemu >/dev/null 2>&1; then
    echo "✅ melonDS already installed: $(command -v cemu)"
    exit 0
fi

EMULATOR_NAME="Cemu"
INSTALL_DIR="$HOME/.local/share/emulators/Cemu"
BIN_DIR="$HOME/.local/bin"
APPIMAGE_NAME="Cemu-2.6-x86_64.AppImage"

CEMU_URL="https://github.com/cemu-project/Cemu/releases/download/v2.6/Cemu-2.6-x86_64.AppImage"

echo "🟢 Installing Cemu..."

if [ -f "$BIN_DIR/cemu" ]; then
  echo "✅ Cemu already installed"
  exit 0
fi

mkdir -p "$INSTALL_DIR"
mkdir -p "$BIN_DIR"

echo "⬇️ Downloading Cemu from official GitHub release..."
curl -L "$CEMU_URL" -o "$INSTALL_DIR/$APPIMAGE_NAME" [web:11]

chmod +x "$INSTALL_DIR/$APPIMAGE_NAME" [web:3]

echo "🔗 Creating symlink..."
ln -s "$INSTALL_DIR/$APPIMAGE_NAME" "$BIN_DIR/cemu"

echo "📁 Creating MLC directory for Wii U filesystem (required for saves/updates)..."
mkdir -p "$HOME/.local/share/cemu/mlc01" [web:3]

echo "🎉 Cemu installed successfully!"
echo "Run with: cemu"
echo "Config and mlc01 at: $HOME/.local/share/cemu"