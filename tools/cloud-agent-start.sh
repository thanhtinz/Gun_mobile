#!/usr/bin/env bash
set -euo pipefail

# Ensure session dbus for Unity Hub GUI on desktop (:1)
if [[ -z "${DBUS_SESSION_BUS_ADDRESS:-}" ]]; then
  eval "$(dbus-launch --sh-syntax)" || true
fi

export DISPLAY="${DISPLAY:-:1}"

echo "Gun Mobile environment ready. DISPLAY=$DISPLAY"
echo "Unity Hub: unityhub   |   Test compile: bash tools/unity_test.sh"
