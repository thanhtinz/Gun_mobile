#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEST="${1:-$ROOT/legacy/releases/Ok}"
mkdir -p "$DEST"
gh release download Ok --repo thanhtinz/Gun_mobile --dir "$DEST" --skip-existing
echo "Release Ok saved under $DEST"
