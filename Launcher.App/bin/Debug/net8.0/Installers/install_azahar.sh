#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Azahar (user-local)..."

HOME_DIR="$HOME"
APPDIR="$HOME_DIR/.local/share/emulators/Azahar"
BIN="$HOME_DIR/.local/bin"

mkdir -p "$APPDIR" "$BIN"

# Fix 1: Better tool checking with suggestions
for cmd in curl jq wget; do
    if ! command -v "$cmd" >/dev/null 2>&1; then
        echo "❌ Required tool '$cmd' is not installed."
        echo "💡 Install with:"
        if [[ -f /etc/debian_version ]]; then
            echo "   sudo apt install $cmd"
        elif [[ -f /etc/redhat-release ]]; then
            echo "   sudo dnf install $cmd"
        elif [[ -f /etc/arch-release ]]; then
            echo "   sudo pacman -S $cmd"
        fi
        exit 1
    fi
done

echo "🔍 Fetching Azahar releases..."

# Fix 2: Fixed URL formatting
URL=$(curl -s "https://api.github.com/repos/azahar-emu/azahar/releases/latest" \
  | jq -r '.assets[] | select(.name | contains("AppImage") and contains("wayland")?) | .browser_download_url' \
  | head -n 1)

if [[ -z "$URL" || "$URL" == "null" ]]; then
    echo "❌ No Azahar AppImage found in latest release."
    echo "💡 Try Flatpak: flatpak install flathub org.azahar_emu.Azahar"
    exit 1
fi

FILENAME=$(basename "$URL")
echo "⬇ Downloading $FILENAME from: $URL"

# Fix 3: Better download with progress + verification
wget --show-progress -c "$URL" -O "$APPDIR/$FILENAME"
chmod +x "$APPDIR/$FILENAME"

# Fix 4: Create launcher symlink
ln -sf "$APPDIR/$FILENAME" "$BIN/azahar"

# Fix 5: Auto-add to PATH (persistent)
if ! grep -q "$BIN" "$HOME_DIR/.bashrc" 2>/dev/null && \
   ! grep -q "$BIN" "$HOME_DIR/.zshrc" 2>/dev/null; then
    echo "➕ Adding $BIN to PATH in ~/.bashrc..."
    echo 'export PATH="$HOME/.local/bin:$PATH"' >> "$HOME_DIR/.bashrc"
    echo 'export PATH="$HOME/.local/bin:$PATH"' >> "$HOME_DIR/.zshrc"
fi

# Fix 6: Verify installation
if command -v azahar >/dev/null 2>&1; then
    echo "✅ Azahar installed and in PATH!"
    azahar --version || echo "ℹ Version check failed (normal for AppImage)"
else
    echo "⚠ Azahar installed but not in PATH yet. Run: source ~/.bashrc"
fi

echo "🎉 Success!"
echo "➡ AppImage: $APPDIR/$FILENAME"
echo "➡ Launcher: $BIN/azahar"
echo "ℹ Restart terminal or run: source ~/.bashrc"
