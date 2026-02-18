#!/usr/bin/env bash
set -euo pipefail

EMUDIR="$HOME/.local/share/emulators/mupen64plus"
LAUNCHER="$EMUDIR/mupen64plus"

mkdir -p "$EMUDIR"

# Use RetroPie/AlbertByte's battle-tested standalone build
cd "$EMUDIR"
wget -q "https://github.com/albertbyte/mupen64plus-standalone/releases/download/0.998/mupen64plus-standalone-linux-x86_64-v0.998.tar.gz" -O mupen.tar.gz || {
    echo "❌ Download failed. Try: sudo apt install mupen64plus"
    exit 1
}

tar xzf mupen.tar.gz
cp mupen64plus-v0.998-x86_64/mupen64plus mupen64plus.bin
chmod +x mupen64plus.bin
rm -rf mupen64plus-v0.998-x86_64 mupen.tar.gz

cat > "$LAUNCHER" <<EOF
#!/usr/bin/env bash
cd "$EMUDIR"
exec ./mupen64plus.bin "\$@"
EOF
chmod +x "$LAUNCHER"

echo "✅ SUCCESS! Test with: $LAUNCHER /path/to/rom.z64"
