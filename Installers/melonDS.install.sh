#!/usr/bin/env bash
#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


INSTALL_DIR="$HOME/.local/share/emulators/MelonDS"
BIN_DIR="$HOME/.local/bin"
APPIMAGE="melonDS.AppImage"
URL="https://example.com/melonDS.AppImage"

echo "🟢 Installing melonDS..."

if [ -f "$BIN_DIR/melonds" ]; then
  echo "✅ melonDS already installed"
  exit 0
fi

mkdir -p "$INSTALL_DIR" "$BIN_DIR"

curl -L "$URL" -o "$INSTALL_DIR/$APPIMAGE"
chmod +x "$INSTALL_DIR/$APPIMAGE"

ln -s "$INSTALL_DIR/$APPIMAGE" "$BIN_DIR/melonds"

echo "🎉 melonDS installed!"
