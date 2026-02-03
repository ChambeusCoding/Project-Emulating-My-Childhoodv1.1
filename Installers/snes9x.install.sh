#!/usr/bin/env bash
set -e

echo "🟢 Installing SNES9x..."

if command -v snes9x >/dev/null 2>&1; then
  echo "✅ SNES9x already installed"
  exit 0
fi

sudo apt update
sudo apt install -y snes9x

echo "🎉 SNES9x installed"
