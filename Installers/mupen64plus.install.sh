#!/usr/bin/env bash
set -e

REAL_USER_HOME=$(getent passwd "$SUDO_USER" | cut -d: -f6)
EMUDIR="$REAL_USER_HOME/.local/share/emulators/Mupen64Plus"

LAUNCHER="$EMUDIR/mupen64plus.sh"

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi

echo "🟢 Installing Mupen64Plus..."

if ! command -v mupen64plus >/dev/null 2>&1; then
    echo "📦 Installing via apt..."
    apt update
    apt install -y mupen64plus
else
    echo "✅ Mupen64Plus already installed"
fi

echo "📁 Setting up emulator directory..."
mkdir -p "$EMUDIR"

echo "🚀 Creating launcher wrapper..."
cat <<EOF > "$LAUNCHER"
#!/usr/bin/env bash
exec /usr/bin/mupen64plus "\$@"
EOF

chmod +x "$LAUNCHER"

echo "🎉 Mupen64Plus ready!"
echo "➡ Launcher path: $LAUNCHER"
