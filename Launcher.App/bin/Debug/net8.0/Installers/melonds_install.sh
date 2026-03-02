#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing melonDS (user-safe)..."

if command -v melonds >/dev/null 2>&1; then
    echo "✅ melonDS already installed: $(command -v melonds)"
    exit 0
fi

APPDIR="$HOME/.local/share/emulators/melonds"
BINDIR="$HOME/.local/bin"
mkdir -p "$APPDIR" "$BINDIR"

if command -v flatpak >/dev/null 2>&1; then
    echo "🔄 Trying Flatpak (recommended)..."

    flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo || true

    if flatpak install --user -y flathub net.kuribo64.melonDS; then
        echo "🎉 SUCCESS: Installed melonDS via Flatpak"
        echo "📝 Run with: flatpak run net.kuribo64.melonDS"
        exit 0
    fi
fi

echo "🔄 Flatpak unavailable/failed, trying AppImage from melonDS downloads..."

LOG="$HOME/melonds-install.log"

(

    DOWNLOAD_PAGE="https://melonds.kuribo64.net/downloads.php"

    URL=$(curl -s --max-time 15 "$DOWNLOAD_PAGE" \
        | grep -Eo 'https?://[^"]+AppImage' \
        | head -n1)

    if [[ -z "$URL" ]]; then
        echo "❌ Could not auto-detect AppImage URL from $DOWNLOAD_PAGE"
        exit 1
    fi

    echo "🔗 Downloading AppImage: $URL"
    wget -q --timeout=60 "$URL" -O "$APPDIR/melonds.AppImage"

    chmod +x "$APPDIR/melonds.AppImage"
    ln -sf "$APPDIR/melonds.AppImage" "$BINDIR/melonds"

    echo "🎉 SUCCESS: melonDS AppImage installed to $APPDIR"
    echo "📍 Binary symlink: $BINDIR/melonds"
    echo "📝 Run with: $BINDIR/melonds"
) >"$LOG" 2>&1 &

echo "🎉 UI SAFE! Background install running. Log: tail -f \"$LOG\""
echo "➡ Once done, run: $BINDIR/melonds  (or: melonds if ~/.local/bin is in PATH)"
echo "💡 Alternative: flatpak install --user flathub net.kuribo64.melonDS && flatpak run net.kuribo64.melonDS"
