#!/usr/bin/env python3
"""Bake PNG/JPG to Khronos .pkm (ETC2_RGBA8) for GunMobile Unity runtime.

Preferred: Unity Editor → GunMobile → Bake PKM (ETC2) from StreamingAssets PNG
(requires Unity + EditorUtility.CompressTexture).

This script uses etc2comp when installed (Khronos):
  git clone https://github.com/KhronosGroup/ETC2Comp etc2comp && cd etc2comp && cmake . && make
  export PATH="$PWD/bin:$PATH"

Usage:
  python3 tools/png_to_pkm.py [--root legacy/unpacked/Resource/image] [--limit 100]
"""

from __future__ import annotations

import argparse
import shutil
import struct
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PKM_HEADER = 16


def write_pkm_header(width: int, height: int, etc2_format: int = 3) -> bytes:
    encoded_w = (width + 3) // 4
    encoded_h = (height + 3) // 4
    header = bytearray(PKM_HEADER)
    header[0:4] = b"PKM "
    header[4:6] = b"20"
    header[6:8] = b"\r\n"
    struct.pack_into(">HH", header, 8, width, height)
    header[12] = encoded_w & 0xFF
    header[13] = encoded_h & 0xFF
    header[14] = 0
    header[15] = etc2_format & 0xFF
    return bytes(header)


def find_etc2comp() -> str | None:
    for name in ("etc2comp", "ETC2Comp"):
        path = shutil.which(name)
        if path:
            return path
    return None


def bake_with_etc2comp(comp: str, src: Path, dst: Path) -> bool:
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        out_kmg = tmp / "out.pkm"
        cmd = [
            comp,
            "-i",
            str(src),
            "-o",
            str(out_kmg),
            "-f",
            "ETC2_RGBA",
        ]
        try:
            subprocess.run(cmd, check=True, capture_output=True, timeout=120)
        except (subprocess.CalledProcessError, FileNotFoundError, subprocess.TimeoutExpired):
            return False

        if not out_kmg.exists():
            return False

        data = out_kmg.read_bytes()
        if len(data) <= PKM_HEADER or not data.startswith(b"PKM "):
            return False

        dst.parent.mkdir(parents=True, exist_ok=True)
        dst.write_bytes(data)
        return True


def bake_with_pil_pad_only(src: Path, dst: Path) -> bool:
    """Fallback: copy PNG path list only — real ETC2 needs Unity Editor or etc2comp."""
    return False


def iter_images(root: Path, limit: int) -> list[Path]:
    files: list[Path] = []
    for ext in ("*.png", "*.jpg", "*.jpeg"):
        files.extend(root.rglob(ext))
    files.sort()
    if limit > 0:
        files = files[:limit]
    return files


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--root", default=str(ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData" / "Resource" / "image"))
    parser.add_argument("--limit", type=int, default=0, help="Max files (0 = all)")
    parser.add_argument("--force", action="store_true")
    args = parser.parse_args()

    root = Path(args.root)
    if not root.is_dir():
        print(f"missing root: {root}", file=sys.stderr)
        print("Run GunMobile → Bake PKM in Unity Editor, or unpack PC dump first.", file=sys.stderr)
        return 1

    comp = find_etc2comp()
    if not comp:
        print("etc2comp not found in PATH.", file=sys.stderr)
        print("Use Unity Editor: GunMobile → Bake PKM (ETC2) from StreamingAssets PNG", file=sys.stderr)
        return 2

    ok = 0
    skip = 0
    for src in iter_images(root, args.limit):
        dst = src.with_suffix(".pkm")
        if dst.exists() and not args.force:
            skip += 1
            continue
        if bake_with_etc2comp(comp, src, dst):
            ok += 1
            if ok % 50 == 0:
                print(f"  {ok}…", flush=True)
        else:
            skip += 1

    print(f"PKM bake: {ok} written, {skip} skipped (root={root})")
    return 0 if ok > 0 or skip > 0 else 1


if __name__ == "__main__":
    raise SystemExit(main())
