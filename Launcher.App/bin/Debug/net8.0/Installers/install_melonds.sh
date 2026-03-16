#!/usr/bin/env bash
set -euo pipefail

echo "🟢 Installing melonDS (package-based)..."

if command -v melonds >/dev/null 2>&1; then
    echo "✅ melonDS already installed: $(command -v melonds)"
    exit 0
fi

EMUDIR="$HOME/.local/share/emulators/melonds"
BINDIR="$HOME/.local/bin"
rm -rf "$EMUDIR" "$BINDIR/melonds"

echo "🔄 Trying to install melonDS from known sources..."

# Try multiple install methods in order
METHODS=(
    "flatpak"
    "snap"
    "apt"
)

for METHOD in "${METHODS[@]}"; do
    case "$METHOD" in
        flatpak)
            if command -v flatpak >/dev/null 2>&1; then
                echo "🌐 Trying Flatpak (net.kuribo64.melonDS)..."
                if flatpak install --user -y flathub net.kuribo64.melonDS >/dev/null 2>&1; then
                    # Create convenience wrapper
                    mkdir -p "$BINDIR"
                    cat > "$BINDIR/melonds" << EOF
#!/usr/bin/env bash
exec flatpak run net.kuribo64.melonDS "\$@"
EOF
                    chmod +x "$BINDIR/melonds"
                    echo "✅ SUCCESS: Installed via Flatpak"
                    break
                fi
            fi
            ;;
        snap)
            if command -v snap >/dev/null 2>&1; then
                echo "🌐 Trying Snap (melonds)..."
                if sudo snap install melonds >/dev/null 2>&1; then  # main snap name is 'melonds' [web:12][web:24]
                    mkdir -p "$BINDIR"
                    cat > "$BINDIR/melonds" << EOF
#!/usr/bin/env bash
exec snap run melonds "\$@"
EOF
                    chmod +x "$BINDIR/melonds"
                    echo "✅ SUCCESS: Installed via Snap"
                    break
                fi
            fi
            ;;
        apt)
            if command -v apt >/dev/null 2>&1; then
                echo "🌐 Trying system package (apt)..."
                # Some distros ship 'melonds' directly, others via third-party repos. [web:26]
                if sudo apt update >/dev/null 2>&1 && sudo apt install -y melonds >/dev/null 2>&1; then
                    echo "✅ SUCCESS: Installed via apt"
                    break
                fi
            fi
            ;;
    esac
done

# Verify installation (any method)
if command -v melonds >/dev/null 2>&1; then
    echo "🎉 SUCCESS: melonDS installed!"
    echo "📍 Binary: $(command -v melonds)"
    echo "🧪 Test: melonds --help"
    exit 0
fi

# Check wrapper if flatpak/snap path
if [[ -x "$BINDIR/melonds" ]]; then
    echo "🎉 SUCCESS: melonDS launcher installed at $BINDIR/melonds"
    echo "🧪 Test: $BINDIR/melonds --help"
    exit 0
fi

cat << EOF
❌ melonDS installation failed!

Manual alternatives:
1. Flatpak: flatpak install --user flathub net.kuribo64.melonDS
2. Snap:    sudo snap install melonds
3. Native:  Check your distro's package manager or https://melonds.kuribo64.net

EOF
echo "❌ All install methods failed!"
exit 1
