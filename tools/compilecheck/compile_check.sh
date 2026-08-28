#!/usr/bin/env bash
# Compile-check GunMobile without Unity or the .NET SDK.
#
# Chỉ cần .NET 8 runtime (dotnet --list-runtimes). Script tải Roslyn (csc.dll) từ
# NuGet vào thư mục tạm rồi biên dịch hai thứ:
#   1. server standalone (Server/GunMobile.Standalone + package Runtime)
#   2. toàn bộ script client, dùng UnityCompileStubs.cs thay cho UnityEngine
#
# Stub chỉ đủ để bắt lỗi cú pháp/tên/kiểu — build thật vẫn phải chạy trong Unity.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${GUNMOBILE_COMPILE_WORK:-${TMPDIR:-/tmp}/gunmobile-compilecheck}"
ROSLYN_VERSION="${ROSLYN_VERSION:-4.9.2}"
mkdir -p "$WORK"

CSC="$WORK/roslyn/tasks/netcore/bincore/csc.dll"
if [ ! -f "$CSC" ]; then
  echo "==> downloading Roslyn $ROSLYN_VERSION"
  curl -sSL --max-time 300 -o "$WORK/roslyn.nupkg" \
    "https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers.toolset/$ROSLYN_VERSION/microsoft.net.compilers.toolset.$ROSLYN_VERSION.nupkg"
  unzip -q -o "$WORK/roslyn.nupkg" -d "$WORK/roslyn"
fi

FW_LINE="$(dotnet --list-runtimes | grep '^Microsoft.NETCore.App 8\.' | tail -1)"
FW_VER="$(printf '%s' "$FW_LINE" | awk '{print $2}')"
FW_DIR="$(printf '%s' "$FW_LINE" | sed -n 's/.*\[\(.*\)\]$/\1/p')"
if command -v cygpath >/dev/null 2>&1 && [ -n "$FW_DIR" ]; then FW_DIR="$(cygpath -u "$FW_DIR")"; fi
FW="$FW_DIR/$FW_VER"
if [ -z "$FW_VER" ] || [ ! -d "$FW" ]; then
  echo "Không tìm thấy .NET 8 runtime. Cài từ https://dotnet.microsoft.com/download" >&2
  exit 1
fi

win() { if command -v cygpath >/dev/null 2>&1; then cygpath -w "$1"; else printf '%s' "$1"; fi; }

refs() {
  for d in "$FW"/*.dll; do
    case "$(basename "$d")" in
      *Native*|clrjit.dll|coreclr.dll|clrgc.dll|clretwrc.dll|msquic.dll|hostpolicy.dll|mscorrc.dll|mscordbi.dll|mscordaccore*|createdump*|*.resources.dll) continue ;;
    esac
    echo "-r:\"$(win "$d")\""
  done
}

PORT="$ROOT/UnityClient/Packages/com.gunmobile.port/Runtime"

echo "==> server (GUNMOBILE_STANDALONE)"
{
  echo "-nologo"; echo "-nostdlib"; echo "-langversion:12"
  echo "-define:GUNMOBILE_STANDALONE"; echo "-target:exe"
  echo "-out:\"$(win "$WORK/GunMobileServer.dll")\""
  refs
  for f in "$ROOT"/Server/GunMobile.Standalone/Program.cs \
           "$ROOT"/Server/GunMobile.Standalone/UnityEngine/*.cs \
           "$PORT"/Core/*.cs \
           "$PORT"/Logic/BattleEffects.cs "$PORT"/Logic/BattleLoop.cs \
           "$PORT"/Logic/PcPhysics.cs "$PORT"/Logic/ProjectileSimulator.cs \
           "$PORT"/Res/FlashConfig.cs "$PORT"/Res/GameDatabase.cs \
           "$PORT"/Res/MapCollision.cs "$PORT"/Res/ResLoader.cs \
           "$PORT"/Net/MobileGameServer.cs "$PORT"/Net/PhonePacket.cs "$PORT"/Net/PlayerExtras.cs; do
    echo "\"$(win "$f")\""
  done
} > "$WORK/server.rsp"
dotnet "$CSC" "@$(win "$WORK/server.rsp")"

echo "==> client + package (UnityEngine stubs)"
{
  echo "-nologo"; echo "-nostdlib"; echo "-langversion:12"; echo "-target:library"
  echo "-out:\"$(win "$WORK/client.dll")\""
  refs
  echo "\"$(win "$ROOT/tools/compilecheck/UnityCompileStubs.cs")\""
  for f in "$ROOT"/UnityClient/Assets/Scripts/Client/*.cs \
           "$PORT"/Core/*.cs "$PORT"/Logic/*.cs "$PORT"/Res/*.cs \
           "$PORT"/Net/*.cs "$PORT"/UI/*.cs "$PORT"/*.cs; do
    echo "\"$(win "$f")\""
  done
} > "$WORK/client.rsp"
dotnet "$CSC" "@$(win "$WORK/client.rsp")"

echo "OK: server + client compile clean"
