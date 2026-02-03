#!/usr/bin/env bash
set -e

NAME="Citra3DS"
INSTALL_DIR="$HOME/.local/share/emulators/Citra3DS"
BIN="$HOME/.local/bin/citra"
APPIMAGE="citra.AppImage"

# 🔴 Replace with real URL
URL="https://example.com/citra.AppImage"

echo "🟢 Installing Citra..."

if [ -f "$BIN" ]; then
  echo "✅ Citra already installed"
  exit 0
fi

mkdir -p "$INSTALL_DIR" "$HOME/.local/bin"

curl -L "$URL" -o "$INSTALL_DIR/$APPIMAGE"
chmod +x "$INSTALL_DIR/$APPIMAGE"

ln -s "$INSTALL_DIR/$APPIMAGE" "$BIN"

echo "🎉 Citra installed"
