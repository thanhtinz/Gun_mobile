#!/usr/bin/env bash
# End-to-end test: dựng server standalone với bảng PC thật (legacy/data), rồi
# bắn toàn bộ PhoneMsg mới qua TCP và kiểm tra phản hồi.
#
# Chỉ cần .NET 8 runtime — compile_check.sh lo phần biên dịch (tải Roslyn).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${GUNMOBILE_COMPILE_WORK:-${TMPDIR:-/tmp}/gunmobile-compilecheck}"
DATA="$WORK/smoke-data"

bash "$ROOT/tools/compilecheck/compile_check.sh"

CSC="$WORK/roslyn/tasks/netcore/bincore/csc.dll"
FW_LINE="$(dotnet --list-runtimes | grep '^Microsoft.NETCore.App 8\.' | tail -1 || true)"
FW_VER="$(printf '%s' "$FW_LINE" | awk '{print $2}')"
FW_DIR="$(printf '%s' "$FW_LINE" | sed -n 's/.*\[\(.*\)\]$/\1/p')"
if command -v cygpath >/dev/null 2>&1 && [ -n "$FW_DIR" ]; then FW_DIR="$(cygpath -u "$FW_DIR")"; fi
FW="$FW_DIR/$FW_VER"

win() { if command -v cygpath >/dev/null 2>&1; then cygpath -w "$1"; else printf '%s' "$1"; fi; }

echo "==> smoke client"
{
  echo "-nologo"; echo "-nostdlib"; echo "-langversion:12"; echo "-target:exe"
  echo "-out:\"$(win "$WORK/SmokeClient.dll")\""
  for d in "$FW"/*.dll; do
    case "$(basename "$d")" in
      *Native*|clrjit.dll|coreclr.dll|clrgc.dll|clretwrc.dll|msquic.dll|hostpolicy.dll|mscorrc.dll|mscordbi.dll|mscordaccore*|createdump*|*.resources.dll) continue ;;
    esac
    echo "-r:\"$(win "$d")\""
  done
  echo "\"$(win "$ROOT/Server/GunMobile.SmokeClient/Program.cs")\""
} > "$WORK/smoke.rsp"
dotnet "$CSC" "@$(win "$WORK/smoke.rsp")"
cp "$WORK/GunMobileServer.runtimeconfig.json" "$WORK/SmokeClient.runtimeconfig.json"

rm -rf "$DATA"
rm -f "$WORK/server.log"
mkdir -p "$DATA"

# StreamingAssets có cả Request lẫn Service/Road/map (cần map để mô phỏng đạn);
# legacy/data chỉ có bảng XML nên dùng làm phương án dự phòng.
PC_DATA="$ROOT/UnityClient/Assets/StreamingAssets/PcData"
if [ ! -d "$PC_DATA/Request" ]; then PC_DATA="$ROOT/legacy/data"; fi

echo "==> server (PC data: $PC_DATA)"
GUNMOBILE_PC_DATA="$(win "$PC_DATA")" GUNMOBILE_DATA="$(win "$DATA")" \
  dotnet "$WORK/GunMobileServer.dll" > "$WORK/server.log" 2>&1 &
SERVER_PID=$!
trap 'kill "$SERVER_PID" 2>/dev/null || true' EXIT

for _ in $(seq 1 120); do
  if grep -q "Listening Road" "$WORK/server.log" 2>/dev/null; then break; fi
  if ! kill -0 "$SERVER_PID" 2>/dev/null; then
    echo "server chết khi khởi động:" >&2
    cat "$WORK/server.log" >&2
    exit 1
  fi
  sleep 1
done

if ! grep -q "Listening Road" "$WORK/server.log"; then
  echo "server không lắng nghe sau 120s:" >&2
  tail -20 "$WORK/server.log" >&2
  exit 1
fi

grep -m1 "GunMobile DB " "$WORK/server.log" | tr ' ' '\n' | grep -E "^(items|maps|manorSeeds|riddles|rankBoards|strengthExp)=" || true

echo "==> smoke client → server"
set +e
dotnet "$WORK/SmokeClient.dll"
RC=$?
set -e

kill "$SERVER_PID" 2>/dev/null || true
exit "$RC"
