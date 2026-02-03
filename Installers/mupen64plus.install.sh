#!/usr/bin/env bash
#!/usr/bin/env bash
set -e

if [[ "$EUID" -ne 0 ]]; then
    echo "Re-launching installer with admin privileges..."
    exec pkexec bash "$0" "$@"
fi


echo "🟢 Installing Mupen64Plus..."

if command -v mupen64plus >/dev/null 2>&1; then
  echo "✅ Mupen64Plus already installed"
  exit 0
fi

echo "📦 Installing via apt (requires sudo)..."
sudo apt update
sudo apt install -y mupen64plus

echo "🎉 Mupen64Plus installed!"
