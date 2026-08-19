#!/usr/bin/env python3
"""Extract and decompress DDTank legacy configs from the Ok release zips."""

from __future__ import annotations

import argparse
import os
import re
import sys
import zipfile
import zlib
from pathlib import Path

ZLIB_MAGICS = (b"\x78\x01", b"\x78\x9c", b"\x78\xda")
SKIP_NAME_PARTS = (
    "__MACOSX/",
    "/logs/",
    "/obj/",
    "/Bin/",
    "/.vs/",
    "EntityFramework",
    "Newtonsoft.Json",
    "log4net",
    "protobuf-net",
    "一些备份",
    "Service References",
)
SKIP_SUFFIXES = (".exe", ".dll", ".pdb", ".swf", ".png", ".jpg", ".jpeg", ".zip", ".rar", ".7z", ".bak")


def is_zlib(data: bytes) -> bool:
    return len(data) >= 2 and data[:2] in ZLIB_MAGICS


def maybe_decompress(data: bytes) -> bytes:
    if is_zlib(data):
        return zlib.decompress(data)
    return data


def should_skip(name: str) -> bool:
    if name.endswith("/"):
        return True
    if any(part in name for part in SKIP_NAME_PARTS):
        return True
    if " - " in name or "Copia" in name:
        return True
    lower = name.lower()
    if lower.endswith(SKIP_SUFFIXES):
        return True
    return False


def extract_from_zip(zip_path: Path, dest: Path, prefixes: tuple[str, ...], max_bytes: int) -> int:
    count = 0
    with zipfile.ZipFile(zip_path) as zf:
        for info in zf.infolist():
            name = info.filename.replace("\\", "/")
            if should_skip(name):
                continue
            if not name.startswith(prefixes):
                continue
            if info.file_size > max_bytes:
                continue
            raw = zf.read(info)
            data = maybe_decompress(raw)
            out = dest / name
            out.parent.mkdir(parents=True, exist_ok=True)
            out.write_bytes(data)
            count += 1
    return count


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--src",
        default=str(Path(__file__).resolve().parents[1] / "legacy" / "releases" / "Ok"),
    )
    parser.add_argument(
        "--dst",
        default=str(Path(__file__).resolve().parents[1] / "legacy" / "data"),
    )
    parser.add_argument("--max-bytes", type=int, default=2_000_000)
    args = parser.parse_args()
    src = Path(args.src)
    dst = Path(args.dst)
    dst.mkdir(parents=True, exist_ok=True)

    jobs = [
        (src / "Archive.2.zip", ("Flash/",), args.max_bytes),
        (src / "Archive.3.zip", ("Request/", "Service/Road/xml/", "Service/Fight/xml/", "Service/Road/battle.xml"), args.max_bytes),
    ]
    total = 0
    for zip_path, prefixes, limit in jobs:
        if not zip_path.exists():
            print(f"missing {zip_path}", file=sys.stderr)
            continue
        n = extract_from_zip(zip_path, dst, prefixes, limit)
        print(f"{zip_path.name}: extracted {n} files")
        total += n
    print(f"done, {total} files -> {dst}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
