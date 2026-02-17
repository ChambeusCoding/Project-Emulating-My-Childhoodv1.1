#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Azahar (user-local)..."

HOME_DIR="$HOME"
APPDIR="$HOME_DIR/.local/share/emulators/Azahar"
BIN="$HOME_DIR/.local/bin"

mkdir -p "$APPDIR" "$BIN"
cd "$APPDIR"

for cmd in curl jq wget; do
    if ! command -v "$cmd" >/dev/null 2>&1; then
        echo "❌ Required tool '$cmd' is not installed. Please install it with your package manager and re-run."
        exit 1
    fi
done

echo "🔍 Fetching Azahar releases..."

URL=$(curl -s https://api.github.com/repos/azahar-emu/azahar/releases \
  | jq -r '.[].assets[].browser_download_url' \
  | grep -i appimage \
  | head -n 1)

if [[ -z "$URL" || "$URL" == "null" ]]; then
    echo "❌ No Azahar AppImage found in releases."
    exit 1
fi

FILENAME=$(basename "$URL")

echo "⬇ Downloading $FILENAME"
wget -q "$URL" -O "$FILENAME"
chmod +x "$FILENAME"

ln -sf "$APPDIR/$FILENAME" "$BIN/azahar"

echo "✅ Azahar installed"
echo "➡ Launcher: $BIN/azahar"
echo "ℹ Make sure ~/.local/bin is in your PATH:"
echo "   export PATH=\"\$HOME/.local/bin:\$PATH\""
