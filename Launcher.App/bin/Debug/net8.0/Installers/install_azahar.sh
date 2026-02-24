echo "🟢 Installing Azahar (package-first)..."

APPDIR="$HOME/.local/share/emulators/Azahar"
BIN="$HOME/.local/bin"

mkdir -p "$APPDIR" "$BIN"

# TRY FLATPAK FIRST (like apt - cached/fast)
if command -v flatpak >/dev/null 2>&1; then
    echo "🔄 Trying Flatpak (fast)..."
    if flatpak install flathub org.azahar_emu.Azahar -y --noninteractive 2>/dev/null | grep -q installed; then
        echo "✅ Flatpak installed! Run: flatpak run org.azahar_emu.Azahar"
        exit 0
    fi
fi

# FALLBACK: AppImage (backgrounded like my last script)
echo "🔄 Flatpak failed, launching AppImage download in background..."
(
    URL=$(curl -s --max-time 10 "https://api.github.com/repos/azahar-emu/azahar/releases/latest" \
        | jq -r '.assets[] | select(.name | contains("AppImage")) | .browser_download_url' | head -n1)
    [[ -n "$URL" ]] && wget -q --timeout=30 "$URL" -O "$APPDIR/Azahar.AppImage" && \
        chmod +x "$APPDIR/Azahar.AppImage" && ln -sf "$APPDIR/Azahar.AppImage" "$BIN/azahar"
) > "$HOME/azahar-install.log" 2>&1 &

echo "🎉 UI SAFE! Background install: tail -f ~/azahar-install.log"
echo "➡ Or use: flatpak run org.azahar_emu.Azahar"
