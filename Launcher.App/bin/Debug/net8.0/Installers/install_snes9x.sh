#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing SNES9x (fixed installer 2026)..."

# Check ALL snes9x variants first (libretro, standalone, snaps)
if command -v snes9x >/dev/null 2>&1 || command -v snes9x-gtk >/dev/null 2>&1; then
    echo "✅ Standalone SNES9x found!"
    exit 0
fi

if command -v snap >/dev/null 2>&1 && snap list snes9x-gtk >/dev/null 2>&1; then
    echo "✅ Snap SNES9x-gtk detected!"
    echo "🔧 Run: sudo snap connect snes9x-gtk:joystick removable-media"
    exit 0
fi

if dpkg -l | grep -q libretro-snes9x; then
    echo "ℹ️  Libretro SNES9x core found (needs RetroArch frontend)"
    echo "🎮 For RetroRunner: Use libretro core instead of snes9x-gtk"
    exit 0
fi

INSTALL_DIR="$HOME/.local/bin"
mkdir -p "$INSTALL_DIR"

echo "🔄 Trying reliable methods..."

# 1. Snap INSTALL (most reliable - needs sudo but works everywhere)
if command -v snap >/dev/null 2>&1; then
    echo "🌐 Snap (RECOMMENDED)..."
    if sudo snap install snes9x-gtk 2>/dev/null && command -v snes9x-gtk >/dev/null 2>&1; then
        echo "✅ SNAP SUCCESS! Run: snes9x-gtk"
        echo "🔧 Permissions: sudo snap connect snes9x-gtk:joystick removable-media"
        exit 0
    fi
fi

# 2. Flatpak (user install - no sudo)
if command -v flatpak >/dev/null 2>&1; then
    echo "🌐 Flatpak..."
    flatpak remote-add --user --if-not-exists flathub https://dl.flathub.org/repo/flathub.flatpakrepo 2>/dev/null || true
    if flatpak install --user -y flathub org.snes9x.Snes9x 2>/dev/null && flatpak list | grep -q org.snes9x.Snes9x; then
        echo "✅ Flatpak SUCCESS! Run: flatpak run org.snes9x.Snes9x"
        exit 0
    fi
fi

# 3. AppImage (direct version URL)
echo "🌐 AppImage..."
cd /tmp
if wget -T 15 --no-verbose "https://github.com/snes9xgit/snes9x/releases/download/1.63.7/snes9x-1.63.7-x64.AppImage"; then
    chmod +x snes9x-1.63.7-x64.AppImage
    mv snes9x-1.63.7-x64.AppImage "$INSTALL_DIR/snes9x"
    ln -sf "$INSTALL_DIR/snes9x" "$INSTALL_DIR/snes9x-gtk"
    echo "✅ AppImage SUCCESS! Run: $INSTALL_DIR/snes9x"
    echo "export PATH=\"\$HOME/.local/bin:\$PATH\"" >> ~/.bashrc
    exit 0
fi

cat << EOF
❌ Auto-install failed!

🚀 MANUAL OPTIONS (Copy-paste these):

1. **SNAP (BEST - already half-working on your system)**:
   sudo snap connect snes9x-gtk:joystick
   sudo snap connect snes9x-gtk:removable-media
   snes9x-gtk "~/Documents/ROMs/Super Punch-Out!! (USA).sfc"

2. **RetroRunner + Libretro** (you already have this):
   Plugin: Use libretro-snes9x core (/usr/lib/libretro/snes9x_libretro.so)
   
3. **AppImage**:
   cd ~/Documents/ROMs && wget https://github.com/snes9xgit/snes9x/releases/download/1.63.7/snes9x-1.63.7-x64.AppImage
   chmod +x snes9x-1.63.7-x64.AppImage

📝 PATH: echo 'export PATH="/snap/bin:\$HOME/.local/bin:\$PATH"' >> ~/.bashrc
EOF

exit 1
