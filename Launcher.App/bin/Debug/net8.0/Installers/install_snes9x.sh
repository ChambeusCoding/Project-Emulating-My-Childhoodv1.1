#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing SNES9x (user-safe installer)..."

if command -v snes9x >/dev/null 2>&1; then
    echo "✅ SNES9x already installed: $(command -v snes9x)"
    exit 0
fi

INSTALL_DIR="$HOME/.local/bin"
mkdir -p "$INSTALL_DIR"

echo "🔄 Downloading SNES9x (live progress)..."

# Try multiple installation methods
METHODS=(
    "Flatpak: org.snes9x.Snes9x"
    "AppImage: https://github.com/snes9xgit/snes9x/releases/latest/download/snes9x.AppImage"
    "Prebuilt: https://github.com/snes9xgit/snes9x/releases/download/1.63/snes9x-1.63-linux.tar.gz"
)

for METHOD in "${METHODS[@]}"; do
    echo "🌐 Trying: $METHOD"
    
    case "$METHOD" in
        "Flatpak: "*)
            if command -v flatpak >/dev/null 2>&1; then
                flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo || true
                if flatpak install --user -y flathub org.snes9x.Snes9x 2>/dev/null | grep -q "installed"; then
                    echo "✅ SUCCESS: $METHOD"
                    echo "📝 Run with: flatpak run org.snes9x.Snes9x"
                    exit 0
                fi
            fi
            ;;
        "AppImage: "*)
            URL="${METHOD#AppImage: }"
            if wget --spider --timeout=10 "$URL" 2>/dev/null; then
                echo "🔗 Downloading AppImage: $URL"
                if wget --timeout=60 --show-progress --progress=bar:force "$URL" -O "/tmp/snes9x.AppImage"; then
                    chmod +x "/tmp/snes9x.AppImage"
                    mv "/tmp/snes9x.AppImage" "$INSTALL_DIR/snes9x"
                    ln -sf "$INSTALL_DIR/snes9x" "$INSTALL_DIR/snes9x-gtk"
                    echo "✅ SUCCESS: $METHOD"
                    echo "📍 Ready: $INSTALL_DIR/snes9x"
                    exit 0
                fi
            fi
            ;;
        "Prebuilt: "*)
            URL="${METHOD#Prebuilt: }"
            if wget --spider --timeout=10 "$URL" 2>/dev/null; then
                echo "🔗 Downloading prebuilt: $URL"
                if wget --timeout=60 --show-progress --progress=bar:force "$URL" -O "/tmp/snes9x.tar.gz"; then
                    tar xzf "/tmp/snes9x.tar.gz" -C /tmp
                    cp /tmp/snes9x-*/snes9x "$INSTALL_DIR/snes9x" 2>/dev/null || cp /tmp/snes9x-1.*/snes9x "$INSTALL_DIR/snes9x"
                    chmod +x "$INSTALL_DIR/snes9x"
                    echo "✅ SUCCESS: $METHOD"
                    echo "📍 Ready: $INSTALL_DIR/snes9x"
                    exit 0
                fi
            fi
            ;;
    esac
done

cat << EOF
❌ SNES9x installation failed!

Manual alternatives:
1. Flatpak: flatpak install --user flathub org.snes9x.Snes9x
2. AppImage: https://github.com/snes9xgit/snes9x/releases/latest
3. System package: sudo apt install snes9x-gtk
4. Build from source: https://github.com/snes9xgit/snes9x

EOF
echo "❌ All install methods failed!"
exit 1
