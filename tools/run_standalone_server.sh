#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PC_DATA="${GUNMOBILE_PC_DATA:-$ROOT/UnityClient/Assets/StreamingAssets/PcData}"
DATA="${GUNMOBILE_DATA:-$ROOT/.gunmobile-server-data}"

export GUNMOBILE_PC_DATA="$PC_DATA"
export GUNMOBILE_DATA="$DATA"

cd "$ROOT/Server/GunMobile.Standalone"
dotnet build -c Release -v minimal
dotnet run -c Release --no-build -- "$PC_DATA"
