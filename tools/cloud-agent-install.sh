#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

UNITY_VERSION="${UNITY_VERSION:-6000.3.22f1}"
UNITY_CHANGESET="${UNITY_CHANGESET:-1c726e1fb402}"
UNITY_BIN="$HOME/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity"

echo "== Gun Mobile cloud install =="

if ! command -v unityhub >/dev/null 2>&1; then
  echo "unityhub missing — install Unity Hub in Dockerfile or apt." >&2
  exit 1
fi

if [[ ! -x "$UNITY_BIN" ]]; then
  echo "Installing Unity Editor ${UNITY_VERSION} (this may take 10–30 min)…"
  unityhub --headless install \
    --version "$UNITY_VERSION" \
    --changeset "$UNITY_CHANGESET" \
    --module linux-il2cpp android \
    --childModules
fi

if [[ ! -x "$UNITY_BIN" ]]; then
  echo "Unity Editor not found at $UNITY_BIN" >&2
  exit 1
fi

echo "Unity: $("$UNITY_BIN" -version 2>/dev/null || true)"

if [[ -f legacy/releases/Ok/Archive.3.zip ]] && [[ "$(stat -c%s legacy/releases/Ok/Archive.3.zip)" -lt 10000 ]]; then
  echo "Fetching PC Ok release archives…"
  bash tools/fetch_ok_release.sh || true
fi

if [[ ! -f legacy/unpacked/.unpacked ]]; then
  if [[ -f legacy/releases/Ok/Archive.3.zip ]] && [[ "$(stat -c%s legacy/releases/Ok/Archive.3.zip)" -gt 10000 ]]; then
    python3 tools/unpack_pc_dump.py || true
  fi
fi

python3 tools/bootstrap_pc_assets.py --skip-fetch --skip-unpack --skip-pack 2>/dev/null || true

if ! command -v dotnet >/dev/null 2>&1; then
  echo "Installing .NET 8 SDK for standalone server…"
  curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0
  export PATH="$HOME/.dotnet:$PATH"
fi

echo "Cloud install done."
