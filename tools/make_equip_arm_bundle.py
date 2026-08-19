#!/usr/bin/env python3
"""Build equip_arm_bundle.zip for first-run device install (icons + all PNGs under equip/arm)."""

from __future__ import annotations

import hashlib
import json
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
UNPACKED = ROOT / "legacy" / "unpacked"
OUT_DIR = ROOT / "legacy" / "releases" / "Ok"
BUNDLE = OUT_DIR / "equip_arm_bundle.zip"
MANIFEST = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData" / "pc_asset_sources.json"

PREFIXES = (
    "Resource/image/equip/",
    "Resource/image/arm/",
)


def sha256_file(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def build(force: bool = False) -> dict:
    if not UNPACKED.is_dir():
        raise SystemExit(f"missing unpacked tree: {UNPACKED} — run tools/bootstrap_pc_assets.py")

    if BUNDLE.exists() and not force:
        info = {
            "path": str(BUNDLE),
            "sizeBytes": BUNDLE.stat().st_size,
            "sha256": sha256_file(BUNDLE),
            "files": 0,
            "reused": True,
        }
        _write_sources(info)
        return info

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    count = 0
    with zipfile.ZipFile(BUNDLE, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=6) as zf:
        for prefix in PREFIXES:
            base = UNPACKED / prefix.rstrip("/")
            if not base.is_dir():
                continue
            for path in base.rglob("*"):
                if not path.is_file() or path.suffix.lower() != ".png":
                    continue
                rel = path.relative_to(UNPACKED).as_posix()
                zf.write(path, rel)
                count += 1
                if count % 2000 == 0:
                    print(f"  bundled {count} png…", flush=True)

    info = {
        "path": str(BUNDLE),
        "sizeBytes": BUNDLE.stat().st_size,
        "sha256": sha256_file(BUNDLE),
        "files": count,
        "reused": False,
    }
    _write_sources(info)
    print(json.dumps(info, indent=2))
    return info


def _write_sources(info: dict) -> None:
    url = "https://github.com/thanhtinz/Gun_mobile/releases/download/Ok/equip_arm_bundle.zip"
    payload = {
        "equipArmBundle": {
            "url": url,
            "sha256": info["sha256"],
            "sizeBytes": info["sizeBytes"],
            "files": info.get("files", 0),
        }
    }
    MANIFEST.parent.mkdir(parents=True, exist_ok=True)
    MANIFEST.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    import argparse

    p = argparse.ArgumentParser()
    p.add_argument("--force", action="store_true")
    build(force=p.parse_args().force)
