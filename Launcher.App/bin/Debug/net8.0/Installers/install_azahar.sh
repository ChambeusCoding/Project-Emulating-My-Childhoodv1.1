#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Azahar (package-first)..."

APPDIR="$HOME/.local/share/emulators/Azahar"
BIN="$HOME/.local/bin"
mkdir -p "$APPDIR" "$BIN"

# Ensure ~/.local/bin in PATH
if ! [[ ":$PATH:" == *":$BIN:"* ]]; then
    echo "➕ Adding $BIN to PATH in ~/.bashrc"
    echo "export PATH=\"$BIN:\$PATH\"" >> ~/.bashrc
    export PATH="$BIN:$PATH"
fi

echo "🔄 Downloading Azahar (live progress)..."

# Try multiple installation methods
METHODS=(
    "Flatpak: org.azahar_emu.Azahar"
    "AppImage: GitHub latest"
)

for METHOD in "${METHODS[@]}"; do
    echo "🌐 Trying: $METHOD"
    
    case "$METHOD" in
        "Flatpak: "*)
            if command -v flatpak >/dev/null 2>&1; then
                flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo || true
                if flatpak install --user -y flathub org.azahar_emu.Azahar 2>/dev/null | grep -q "installed"; then
                    echo "✅ SUCCESS: $METHOD"
                    echo "📝 Run with: flatpak run org.azahar_emu.Azahar"
                    exit 0
                fi
            fi
            ;;
        "AppImage: "*)
            if command -v jq >/dev/null 2>&1; then
                URL=$(curl -s --max-time 10 "https://api.github.com/repos/azahar-emu/azahar/releases/latest" \
                    | jq -r '.assets[] | select(.name | contains("AppImage")) | .browser_download_url' | head -n1)
                
                if [[ -n "$URL" ]]; then
                    echo "🔗 Downloading AppImage: $URL"
                    if wget --timeout=60 --show-progress --progress=bar:force "$URL" -O "$APPDIR/Azahar.AppImage"; then
                        chmod +x "$APPDIR/Azahar.AppImage"
                        ln -sf "$APPDIR/Azahar.AppImage" "$BIN/azahar"
                        echo "✅ SUCCESS: $METHOD"
                        echo "📍 Ready: $BIN/azahar (or just 'azahar')"
                        exit 0
                    fi
                fi
            else
                echo "⚠️  jq not installed (needed for GitHub API)"
            fi
            ;;
    esac
done

cat << EOF
❌ Azahar installation failed!

Manual alternatives:
1. Flatpak: flatpak install --user flathub org.azahar_emu.Azahar
2. AppImage: https://github.com/azahar-emu/azahar/releases/latest
3. Install jq first: sudo apt install jq

EOF
echo "❌ All install methods failed!"
exit 1
