#!/bin/bash
#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


APPDIR="$HOME/.local/share/emulators/Azahar"

echo "Installing Azahar emulator..."

# Create install directory
mkdir -p "$APPDIR"
cd "$APPDIR"

RELEASES_JSON=$(curl -s "https://api.github.com/repos/azahar-emu/azahar/releases")
URL=$(echo "$RELEASES_JSON" \
    | grep browser_download_url \
    | grep -i AppImage \
    | head -n 1 \
    | cut -d '"' -f 4)


if [ -z "$URL" ]; then
    echo "❌ No AppImage release found!"
    exit 1
fi

echo "Found latest Azahar AppImage: $URL"

# Download
FILENAME=$(basename "$URL")
echo "Downloading $FILENAME..."
wget -q "$URL" -O "$FILENAME"

# Make executable
chmod +x "$FILENAME"

echo "Azahar AppImage downloaded and made executable at $APPDIR/$FILENAME"

# Optional: create a symlink in ~/.local/bin
if [ -d "$HOME/.local/bin" ]; then
    ln -sf "$APPDIR/$FILENAME" "$HOME/.local/bin/azahar"
    echo "Symlink created: azahar -> ~/.local/bin"
else
    echo "Note: ~/.local/bin not found. You can run Azahar directly from $APPDIR"
fi

echo "Azahar installation completed!"
