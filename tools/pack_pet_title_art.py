#!/usr/bin/env python3
"""Copy PC pet icons and title banners into StreamingAssets (no map re-pack)."""

from __future__ import annotations

import json
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"
Z3 = ROOT / "legacy" / "releases" / "Ok" / "Archive.3.zip"
DATA = ROOT / "legacy" / "data" / "Request"


def _table(name: str) -> list[dict[str, str]]:
    from port_helpers import load_xml, parse_result_table

    path = DATA / name
    if not path.exists():
        return []
    return parse_result_table(load_xml(path.read_bytes()))


def _append_index(files: list[str], copied: int, skipped: int, missing: int) -> None:
    idx_path = OUT / "content_index.json"
    if not idx_path.exists():
        return
    content = json.loads(idx_path.read_text(encoding="utf-8"))
    have = set(content.get("files") or [])
    for rel in files:
        have.add(rel)
    content["files"] = sorted(have)
    counts = content.setdefault("counts", {})
    counts["petTitlePacked"] = copied + skipped
    counts["petTitleCopied"] = copied
    counts["petTitleMissing"] = missing
    idx_path.write_text(json.dumps(content, indent=2), encoding="utf-8")


def pack() -> dict:
    import sys

    sys.path.insert(0, str(ROOT / "tools"))

    wanted: list[str] = []
    for row in _table("pettemplateinfo.xml"):
        pic = (row.get("Pic") or "").strip()
        if pic:
            wanted.append(f"Resource/image/pet/{pic}/icon1.png")
            wanted.append(f"Resource/image/pet/{pic}/icon2.png")
            wanted.append(f"Resource/image/pet/{pic}/icon.png")
    for row in _table("newtitleinfo.xml"):
        pic = (row.get("Pic") or "").strip()
        if not pic or pic == "0":
            continue
        wanted.append(f"Resource/image/title/image_title_{pic}.png")
        wanted.append(f"Resource/image/title/{pic}/icon.png")

    copied = skipped = missing = 0
    bytes_out = 0
    files: list[str] = []
    seen_rel: set[str] = set()
    with zipfile.ZipFile(Z3) as zf:
        lower = {}
        for name in zf.namelist():
            n = name.replace("\\", "/")
            if "__MACOSX" in n or n.endswith("/"):
                continue
            lower.setdefault(n.lower(), n)
        for rel in wanted:
            actual = lower.get(rel.lower())
            if actual is None:
                missing += 1
                continue
            if actual in seen_rel:
                continue
            seen_rel.add(actual)
            dest = OUT / actual
            if dest.exists():
                skipped += 1
                files.append(actual)
                continue
            dest.parent.mkdir(parents=True, exist_ok=True)
            data = zf.read(actual)
            dest.write_bytes(data)
            copied += 1
            bytes_out += len(data)
            files.append(actual)

    _append_index(files, copied, skipped, missing)
    return {
        "copied": copied,
        "skipped": skipped,
        "missing": missing,
        "bytes": bytes_out,
        "files": len(files),
    }


if __name__ == "__main__":
    print(json.dumps(pack(), indent=2))
