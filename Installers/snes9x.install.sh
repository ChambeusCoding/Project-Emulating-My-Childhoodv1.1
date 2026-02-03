#!/usr/bin/env bash
#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


echo "🟢 Installing SNES9x..."

if command -v snes9x >/dev/null 2>&1; then
  echo "✅ SNES9x already installed"
  exit 0
fi

sudo apt update
sudo apt install -y snes9x

echo "🎉 SNES9x installed"
