#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing Mupen64Plus 2.6.0 (NO WRAPPER - direct binary)..."

# 1) Already working?
if command -v mupen64plus >/dev/null 2>&1; then
    echo "✅ Mupen64Plus already installed: $(command -v mupen64plus)"
    exit 0
fi

EMUDIR="$HOME/.local/share/emulators/mupen64plus"
BINDIR="$HOME/.local/bin"
mkdir -p "$BINDIR" "$EMUDIR"

# 2) Download official bundle
echo "🔄 Downloading Mupen64Plus 2.6.0 bundle..."
cd /tmp
wget -q "https://github.com/mupen64plus/mupen64plus-core/releases/download/2.6.0/mupen64plus-bundle-linux64-2.6.0.tar.gz" -O mupen.tar.gz || {
    echo "❌ Download failed. Install system packages:"
    echo "sudo apt install -y mupen64plus-common"
    exit 1
}

tar xzf mupen.tar.gz
BundleDir=$(ls -d mupen64plus-* 2>/dev/null | head -1)

if [[ -d "$BundleDir" ]]; then
    # 3) Copy EVERYTHING to EMUDIR (plugins + libs + binary)
    cp -r "$BundleDir"/* "$EMUDIR/"
    chmod +x "$EMUDIR/mupen64plus"
    
    # 4) Create DIRECT symlink to binary (NO WRAPPER SCRIPT)
    ln -sf "$EMUDIR/mupen64plus" "$BINDIR/mupen64plus"
    
    # 5) Force Rice plugin + disable capture via environment (no wrapper needed)
    cat > "$EMUDIR/mupen64plus-env" << 'EOF'
export LD_LIBRARY_PATH="$HOME/.local/share/emulators/mupen64plus:$LD_LIBRARY_PATH"
export MUPEN64PLUS_VIDEO_PLUGIN="$HOME/.local/share/emulators/mupen64plus/mupen64plus-video-rice.so"
export GST_PLUGIN_PATH=""
export OPENCV_VIDEOIO_PRIORITY_GSTREAMER=0
EOF
    
    # 6) PATH setup
    if ! echo "$PATH" | grep -q "$BINDIR"; then
        echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.bashrc
        export PATH="$HOME/.local/bin:$PATH"
    fi
    
    rm -rf mupen64plus-* mupen.tar.gz
    echo "🎉 SUCCESS: Mupen64Plus 2.6.0 installed (NO WRAPPER)"
    echo "📍 Binary: $(which mupen64plus)"
    echo "📁 EmuDir: $EMUDIR"
    echo "🧪 Test: mupen64plus --help"
    exit 0
fi

echo "❌ Install failed."
exit 1
