#!/usr/bin/env bash
#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


EMULATOR_NAME="Cemu"
INSTALL_DIR="$HOME/.local/share/emulators/Cemu"
BIN_DIR="$HOME/.local/bin"
APPIMAGE_NAME="Cemu.AppImage"


CEMU_URL="https://example.com/Cemu.AppImage"

echo "🟢 Installing Cemu..."


if [ -f "$BIN_DIR/cemu" ]; then
  echo "✅ Cemu already installed"
  exit 0
fi

mkdir -p "$INSTALL_DIR"
mkdir -p "$BIN_DIR"

echo "⬇️ Downloading Cemu..."
curl -L "$CEMU_URL" -o "$INSTALL_DIR/$APPIMAGE_NAME"

chmod +x "$INSTALL_DIR/$APPIMAGE_NAME"

echo "🔗 Creating symlink..."
ln -s "$INSTALL_DIR/$APPIMAGE_NAME" "$BIN_DIR/cemu"

echo "🎉 Cemu installed successfully!"
