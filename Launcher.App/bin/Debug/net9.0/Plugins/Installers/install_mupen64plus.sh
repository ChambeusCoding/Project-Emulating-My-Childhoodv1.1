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

echo "🔄 Downloading Mupen64Plus packages (live progress)..."

# Try multiple package sources
SOURCES=(
    "apt: mupen64plus-ui-console mupen64plus-video-rice"
)

for SOURCE in "${SOURCES[@]}"; do
    echo "🌐 Trying: $SOURCE"
    
    if sudo apt update && sudo apt install -y mupen64plus-ui-console mupen64plus-video-rice 2>/dev/null; then
        echo "✅ SUCCESS: Mupen64Plus installed via $SOURCE"
        break
    fi
done

# Verify installation
if command -v mupen64plus >/dev/null 2>&1; then
    echo "🎉 SUCCESS: Mupen64Plus installed!"
    echo "📍 Binary: $(command -v mupen64plus)"
    echo "📁 Config: ~/.mupen64plus/"
    echo "🧪 Test: mupen64plus --help"
    exit 0
fi

cat << EOF
❌ Mupen64Plus installation failed!

Manual alternatives:
1. System package: sudo apt install mupen64plus-ui-console mupen64plus-video-rice
2. Flatpak: flatpak install --user flathub org.mupen64plus.Mupen64Plus
3. AppImage: Check https://mupen64plus.org/

EOF
echo "❌ All install methods failed!"
exit 1
