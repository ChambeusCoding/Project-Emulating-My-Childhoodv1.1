#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Mupen64Plus (user-local)..."

HOME_DIR="${HOME}"
EMUDIR="$HOME_DIR/.local/share/emulators/mupen64plus"
LAUNCHER="$EMUDIR/mupen64plus"

mkdir -p "$EMUDIR"

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

if wget -q https://github.com/mupen64plus/mupen64plus-core/releases/latest/download/mupen64plus.AppImage -O mupen64plus.AppImage; then
    chmod +x mupen64plus.AppImage
    mv mupen64plus.AppImage "$INSTALL_BIN"
    echo "✅ Downloaded AppImage to $INSTALL_BIN"
else
    echo "⚠ AppImage download failed, trying generic prebuilt tarball..."
    if wget -q https://github.com/mupen64plus/mupen64plus-core/releases/latest/download/mupen64plus-linux.tar.gz -O mupen64plus.tar.gz; then
        tar xzf mupen64plus.tar.gz
        FOUND_BIN="$(find . -type f -name 'mupen64plus' | head -n 1 || true)"
        if [[ -n "$FOUND_BIN" ]]; then
            cp "$FOUND_BIN" "$INSTALL_BIN"
            chmod +x "$INSTALL_BIN"
            echo "✅ Installed prebuilt binary to $INSTALL_BIN"
        else
            echo "❌ Could not locate mupen64plus binary in archive."
        fi
    else
        echo "❌ Prebuilt tarball download failed."
    fi
fi

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

echo "❌ Failed to obtain Mupen64Plus without root."
echo "💡 You can manually install via your package manager, then re-run this script to just create the wrapper."
exit 1
