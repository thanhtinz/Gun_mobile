#!/usr/bin/env python3
"""Unpack the Ok PC dump into legacy/unpacked (gitignored) for the Unity ExtraRoots."""

from __future__ import annotations

import argparse
import sys
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ZIPS = ROOT / "legacy" / "releases" / "Ok"
OUT = ROOT / "legacy" / "unpacked"

SKIP_PARTS = ("__MACOSX/", "/logs/", "/obj/", "/Bin/", "/.vs/", "一些备份")
SKIP_SUFFIXES = (
    ".exe",
    ".dll",
    ".pdb",
    ".swf",
    ".bak",
    ".cs",
    ".ashx",
    ".aspx",
    ".config",
    ".sln",
    ".csproj",
)


def skip(name: str) -> bool:
    n = name.replace("\\", "/")
    if n.endswith("/") or any(p in n for p in SKIP_PARTS):
        return True
    low = n.lower()
    return low.endswith(SKIP_SUFFIXES)


def extract_zip(zip_path: Path, dest: Path, prefixes: tuple[str, ...], suffixes: tuple[str, ...] | None = None) -> int:
    n = 0
    with zipfile.ZipFile(zip_path) as zf:
        for info in zf.infolist():
            name = info.filename.replace("\\", "/")
            if skip(name):
                continue
            if prefixes and not name.startswith(prefixes):
                continue
            if suffixes and not name.lower().endswith(suffixes):
                continue
            dest_path = dest / name
            dest_path.parent.mkdir(parents=True, exist_ok=True)
            dest_path.write_bytes(zf.read(info))
            n += 1
            if n % 500 == 0:
                print(f"  {zip_path.name}: {n} files…", flush=True)
    return n


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--dst", default=str(OUT))
    args = parser.parse_args()
    dest = Path(args.dst)
    dest.mkdir(parents=True, exist_ok=True)

    z2 = ZIPS / "Archive.2.zip"
    z3 = ZIPS / "Archive.3.zip"
    total = 0
    if z2.exists():
        n = extract_zip(
            z2,
            dest,
            ("Flash/",),
            (".xml", ".txt", ".png", ".jpg", ".ui"),
        )
        print(f"Archive.2.zip Flash: {n}")
        total += n
    else:
        print("missing Archive.2.zip", file=sys.stderr)

    if z3.exists():
        n = extract_zip(z3, dest, ("Request/",), (".xml",))
        print(f"Archive.3.zip Request xml: {n}")
        total += n
        n = extract_zip(z3, dest, ("Resource/image/map/",), (".png", ".jpg", ".jpeg"))
        print(f"Archive.3.zip maps: {n}")
        total += n
        n = extract_zip(z3, dest, ("Service/Road/map/",), (".map",))
        print(f"Archive.3.zip collision: {n}")
        total += n
        n = extract_zip(z3, dest, ("Resource/image/bomb/",), (".png",))
        print(f"Archive.3.zip bomb png: {n}")
        total += n
        n = extract_zip(z3, dest, ("Resource/image/equip/",), (".png",))
        print(f"Archive.3.zip equip png: {n}")
        total += n
        n = extract_zip(z3, dest, ("Resource/image/arm/",), (".png",))
        print(f"Archive.3.zip arm png: {n}")
        total += n
        n = extract_zip(z3, dest, ("Resource/image/game/",), (".png", ".jpg"))
        print(f"Archive.3.zip game png: {n}")
        total += n
        n = extract_zip(z3, dest, ("Resource/image/scene/",), (".png", ".jpg"))
        print(f"Archive.3.zip scene: {n}")
        total += n
    else:
        print("missing Archive.3.zip", file=sys.stderr)

    marker = dest / ".unpacked"
    marker.write_text(f"files={total}\n", encoding="utf-8")
    print(f"done, {total} files -> {dest}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
