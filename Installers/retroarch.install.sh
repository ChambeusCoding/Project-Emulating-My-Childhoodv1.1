#!/bin/bash
set -e

echo "Installing RetroArch and cores..."

# Update repositories
sudo apt update

# Install RetroArch and dependencies
sudo apt install -y retroarch retroarch-assets \
    libretro-bsnes-mercury-performance \
    libretro-bsnes-mercury-balanced \
    libretro-beetle-psx libretro-beetle-pce-fast libretro-beetle-vb \
    libretro-genesisplusgx libretro-gambatte \
    libretro-nestopia libretro-snes9x libretro-mgba libretro-desmume \
    libretro-beetle-wswan libretro-bsnes-mercury-accuracy \
    libretro-core-info

# Create emulator directories
EMULATORS_DIR="$HOME/.local/share/emulators"
mkdir -p "$EMULATORS_DIR/RetroArch/cores"

# Download latest Citra libretro core
CITRA_URL="https://buildbot.libretro.com/stable/cores/citra_libretro_linux_x86_64.so"
CITRA_DEST="$EMULATORS_DIR/RetroArch/cores/citra_libretro.so"

echo "Downloading Citra core..."
curl -L -o "$CITRA_DEST" "$CITRA_URL" || {
    echo "Failed to download Citra core. You may need to manually download it from https://docs.libretro.com/library/citra/"
}

echo "RetroArch + cores installation complete."
echo "Cores folder: $EMULATORS_DIR/RetroArch/cores"
