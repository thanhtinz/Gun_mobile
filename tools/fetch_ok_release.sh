#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEST="${1:-$ROOT/legacy/releases/Ok}"
REPO="${GUNMOBILE_RELEASE_REPO:-thanhtinz/Gun_mobile}"
TAG="${GUNMOBILE_RELEASE_TAG:-Ok}"
mkdir -p "$DEST"

echo "Downloading release ${TAG} from ${REPO} -> ${DEST}"
gh release download "$TAG" --repo "$REPO" --dir "$DEST" --skip-existing

SUMS="${DEST}/SHA256SUMS.txt"
if [[ -f "$SUMS" ]]; then
  echo "Verifying SHA256…"
  (cd "$DEST" && sha256sum -c SHA256SUMS.txt) || {
    echo "SHA256 mismatch — delete bad zips and re-run." >&2
    exit 1
  }
fi

for z in Archive.zip Archive.2.zip Archive.3.zip; do
  f="${DEST}/${z}"
  if [[ ! -f "$f" ]] || [[ "$(stat -c%s "$f" 2>/dev/null || echo 0)" -lt 10000 ]]; then
    echo "Missing or stub archive: $f" >&2
    exit 1
  fi
  echo "  $(ls -lh "$f" | awk '{print $5, $9}')"
done

echo "Release ${TAG} ready under ${DEST}"
