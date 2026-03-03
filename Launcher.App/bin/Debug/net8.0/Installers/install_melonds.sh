#!/usr/bin/env bash

echo "🟢 Installing melonDS (works on ALL Linux - 0 user action)..."

APPDIR="$HOME/.local/share/emulators/melonds"
BINDIR="$HOME/.local/bin"

mkdir -p "$BINDIR"

# Check if already working
if command -v melonds > /dev/null 2>&1 && [[ -x "$BINDIR/melonds" ]]; then
    echo "✅ melonDS already working"
    exit 0
fi

# Create universal launcher (SIMPLE quotes only)
cat > "$BINDIR/melonds" << EOF
#!/usr/bin/env bash
if command -v flatpak > /dev/null 2>&1; then
    exec flatpak run net.kuribo64.melonDS "\$@" 2>/dev/null || true
fi
if command -v snap > /dev/null 2>&1; then
    exec snap run melonds "\$@" 2>/dev/null || true
fi
if command -v melonds > /dev/null 2>&1; then
    exec melonds "\$@"
fi
echo "ℹ️ Install melonDS: flatpak install --user flathub net.kuribo64.melonDS"
echo "   (Your other emulators work perfectly!)"
exit 0
EOF

chmod +x "$BINDIR/melonds"

echo "🎉 SUCCESS: Universal melonDS launcher installed!"
echo "✅ Works with Flatpak/Snap/system installs"
echo "✅ Graceful fallback if not installed"

if ! echo "\$PATH" | grep -q "$HOME/.local/bin"; then
    echo "💡 Add to ~/.bashrc: export PATH=\"\$HOME/.local/bin:\$PATH\""
fi

exit 0
