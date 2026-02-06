#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    exec pkexec bash "$0" "$@"
fi

REAL_USER_HOME=$(getent passwd "$SUDO_USER" | cut -d: -f6)
APPDIR="$REAL_USER_HOME/.local/share/emulators/Azahar"
BIN="$REAL_USER_HOME/.local/bin"

echo "🟢 Installing Azahar..."

apt update
apt install -y curl wget jq fuse libfuse2 || true

mkdir -p "$APPDIR"
cd "$APPDIR"

echo "🔍 Fetching Azahar releases..."

URL=$(curl -s https://api.github.com/repos/azahar-emu/azahar/releases \
  | jq -r '.[].assets[].browser_download_url' \
  | grep -i appimage \
  | head -n 1)

if [[ -z "$URL" ]]; then
    echo "❌ No Azahar AppImage found"
    exit 1
fi

FILENAME=$(basename "$URL")

echo "⬇ Downloading $FILENAME"
wget -q "$URL" -O "$FILENAME"
chmod +x "$FILENAME"

mkdir -p "$BIN"
ln -sf "$APPDIR/$FILENAME" "$BIN/azahar"

echo "✅ Azahar installed"
echo "➡ $BIN/azahar"
