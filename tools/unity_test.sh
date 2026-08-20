#!/usr/bin/env bash
# Compile/test UnityClient from CLI (needs activated Unity license).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_VERSION="${UNITY_VERSION:-6000.3.22f1}"
UNITY_BIN="${UNITY_BIN:-$HOME/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity}"
LOG="${LOG:-/tmp/unity-test.log}"

if [[ ! -x "$UNITY_BIN" ]]; then
  echo "Unity not found: $UNITY_BIN" >&2
  echo "Run: unityhub --headless install --version $UNITY_VERSION --changeset 1c726e1fb402 --module linux-il2cpp android --childModules" >&2
  exit 1
fi

"$UNITY_BIN" \
  -batchmode -nographics -quit \
  -projectPath "$ROOT/UnityClient" \
  -executeMethod GunMobile.EditorTools.MobileBuildMenu.ApplyPlayerSettings \
  -logFile "$LOG"

echo "Log: $LOG"
tail -30 "$LOG"
