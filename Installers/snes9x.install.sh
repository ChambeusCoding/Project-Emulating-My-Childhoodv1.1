#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi

echo "🟢 Installing SNES9x (multi-fallback installer)..."

# 0. Check if already working
if command -v snes9x >/dev/null 2>&1 || command -v snes9x-gtk >/dev/null 2>&1; then
    echo "✅ SNES9x already installed: $(command -v snes9x 2>/dev/null || command -v snes9x-gtk)"
    exit 0
fi

# 1. SNAP (snes9x-gtk exists)
echo "🔄 [1/7] Trying Snap..."
if command -v snap >/dev/null 2>&1; then
    sudo snap install snes9x-gtk 2>/dev/null || true
    if command -v snes9x-gtk >/dev/null 2>&1; then
        echo "🎉 SUCCESS: Snap (snes9x-gtk)"
        echo "📝 Plugin: snes9x-gtk"
        exit 0
    fi
fi

# 2. FLATPAK
echo "🔄 [2/7] Trying Flatpak..."
if command -v flatpak >/dev/null 2>&1; then
    flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo 2>/dev/null || true
    flatpak install flathub org.snes9x.Snes9x -y 2>/dev/null || true
    if flatpak list | grep -q snes9x; then
        echo "🎉 SUCCESS: Flatpak (org.snes9x.Snes9x)"
        echo "📝 Plugin: flatpak run org.snes9x.Snes9x"
        exit 0
    fi
fi

# 3. GitHub Release (most reliable)
echo "🔄 [3/7] Downloading from GitHub..."
cd /tmp
wget -q https://github.com/snes9xgit/snes9x/releases/download/1.63/snes9x-1.63-linux.tar.gz || \
wget -q https://github.com/snes9xgit/snes9x/releases/download/1.62.3/snes9x-1.62.3-linux.tar.gz || true
if [[ -f snes9x-1.63-linux.tar.gz ]]; then
    tar xzf snes9x-1.63-linux.tar.gz || true
    sudo install -m 755 snes9x-1.*/snes9x /usr/local/bin/snes9x 2>/dev/null || true
elif [[ -f snes9x-1.62.3-linux.tar.gz ]]; then
    tar xzf snes9x-1.62.3-linux.tar.gz || true
    sudo install -m 755 snes9x-1.62.3-linux/snes9x /usr/local/bin/snes9x 2>/dev/null || true
fi
if command -v snes9x >/dev/null 2>&1; then
    echo "🎉 SUCCESS: GitHub (snes9x)"
    echo "📝 Plugin: snes9x"
    exit 0
fi

# 4. Ubuntu Multiverse (libretro-snes9x)
echo "🔄 [4/7] Ubuntu Multiverse..."
sudo add-apt-repository multiverse -y
sudo apt update
sudo apt install -y libretro-snes9x snes9x 2>/dev/null || true
if command -v snes9x >/dev/null 2>&1; then
    echo "🎉 SUCCESS: Ubuntu (snes9x)"
    echo "📝 Plugin: snes9x"
    exit 0
fi

# 5. RetroArch cores (snes9x core)
echo "🔄 [5/7] RetroArch + SNES9x core..."
sudo apt install -y retroarch libretro-snes9x 2>/dev/null || true
if command -v retroarch >/dev/null 2>&1; then
    echo "🎉 SUCCESS: RetroArch core"
    echo "📝 Plugin: retroarch -L /usr/lib/libretro/snes9x_libretro.so"
    exit 0
fi

# 6. AppImage (direct download)
echo "🔄 [6/7] AppImage..."
cd /tmp
wget -q https://github.com/snes9xgit/snes9x/releases/latest/download/snes9x.AppImage || true
if [[ -f snes9x.AppImage ]]; then
    sudo install -m 755 snes9x.AppImage /usr/local/bin/snes9x
    sudo chmod +x /usr/local/bin/snes9x
    if command -v snes9x >/dev/null 2>&1; then
        echo "🎉 SUCCESS: AppImage"
        echo "📝 Plugin: snes9x"
        exit 0
    fi
fi

# 7. Build from source (last resort)
echo "🔄 [7/7] Building from source..."
cd /tmp
git clone https://github.com/snes9xgit/snes9x.git || \
wget -qO- https://github.com/snes9xgit/snes9x/archive/refs/tags/1.63.tar.gz | tar xz
cd snes9x* && ./autogen.sh && ./configure && make -j$(nproc) && sudo make install
if command -v snes9x >/dev/null 2>&1; then
    echo "🎉 SUCCESS: Built from source"
    echo "📝 Plugin: snes9x"
    exit 0
fi

echo "❌ All 7 methods failed"
echo "💡 Manual: https://github.com/snes9xgit/snes9x/releases"
exit 1
