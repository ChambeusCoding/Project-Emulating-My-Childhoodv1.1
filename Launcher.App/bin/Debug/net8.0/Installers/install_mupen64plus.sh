#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Mupen64Plus (user-local)..."

HOME_DIR="${HOME}"
EMUDIR="$HOME_DIR/.local/share/emulators/mupen64plus"
LAUNCHER="$EMUDIR/mupen64plus"

mkdir -p "$EMUDIR"

# Check if already installed via package manager
if command -v mupen64plus >/dev/null 2>&1; then
    REAL_EXE="$(command -v mupen64plus)"
    echo "✅ Found Mupen64Plus on PATH at: $REAL_EXE"
    
    cat > "$LAUNCHER" <<EOF
#!/usr/bin/env bash
exec "$REAL_EXE" "\$@"
EOF
    chmod +x "$LAUNCHER"
    echo "🎉 Mupen64Plus wrapper created at: $LAUNCHER"
    echo "📝 Plugin Executable: $LAUNCHER"
    exit 0
fi

INSTALL_BIN="$EMUDIR/mupen64plus.bin"

echo "🔄 Attempting AppImage download..."
TMPDIR="$(mktemp -d)"
cd "$TMPDIR"

# Fix 1: Use curl (more reliable) with proper GitHub release URL
if command -v curl >/dev/null 2>&1; then
    if curl -L -s -o mupen64plus.AppImage \
        "https://github.com/mupen64plus/mupen64plus-core/releases/latest/download/mupen64plus.AppImage"; then
        chmod +x mupen64plus.AppImage
        mv mupen64plus.AppImage "$INSTALL_BIN"
        echo "✅ Downloaded AppImage to $INSTALL_BIN"
    else
        echo "⚠ AppImage download failed"
        DOWNLOAD_SUCCESS=false
    fi
else
    echo "⚠ curl not found, trying wget..."
    if wget -q "https://github.com/mupen64plus/mupen64plus-core/releases/latest/download/mupen64plus.AppImage" -O mupen64plus.AppImage; then
        chmod +x mupen64plus.AppImage
        mv mupen64plus.AppImage "$INSTALL_BIN"
        echo "✅ Downloaded AppImage to $INSTALL_BIN"
    else
        echo "⚠ AppImage download failed"
        DOWNLOAD_SUCCESS=false
    fi
fi

# Fix 2: Try Qt GUI version if core fails
if [[ ! -f "$INSTALL_BIN" ]]; then
    echo "🔄 Trying Mupen64Plus-Qt AppImage..."
    if command -v curl >/dev/null 2>&1 && \
       curl -L -s -o mupen64plus-qt.AppImage \
           "https://github.com/dh4/mupen64plus-qt/releases/latest/download/Mupen64Plus-Qt.AppImage"; then
        chmod +x mupen64plus-qt.AppImage
        mv mupen64plus-qt.AppImage "$INSTALL_BIN"
        echo "✅ Downloaded Mupen64Plus-Qt to $INSTALL_BIN"
    fi
fi

# Fix 3: Better cleanup
cd /
rm -rf "$TMPDIR"

if [[ -x "$INSTALL_BIN" ]]; then
    cat > "$LAUNCHER" <<EOF
#!/usr/bin/env bash
exec "$INSTALL_BIN" "\$@"
EOF
    chmod +x "$LAUNCHER"
    echo "🎉 Mupen64Plus installed locally!"
    echo "➡ Binary: $INSTALL_BIN"
    echo "➡ Launcher (used by plugin): $LAUNCHER"
    exit 0
fi

# Fix 4: Better distro-specific instructions
echo "❌ Failed to download prebuilt binaries."
echo "💡 Install via package manager, then re-run this script:"
echo ""
if [[ -f /etc/debian_version ]]; then
    echo "   sudo apt update && sudo apt install mupen64plus mupen64plus-qt"
elif [[ -f /etc/redhat-release ]] || [[ -f /etc/fedora-release ]]; then
    echo "   sudo dnf install mupen64plus-qt"
elif [[ -f /etc/arch-release ]] || [[ -d /etc/pacman.d ]]; then
    echo "   sudo pacman -S mupen64plus-qt"
elif command -v apk >/dev/null 2>&1; then
    echo "   sudo apk add mupen64plus"
else
    echo "   Search your package manager for 'mupen64plus'"
fi
echo ""
echo "💡 After installing, re-run: $(basename "$0")"
exit 1
