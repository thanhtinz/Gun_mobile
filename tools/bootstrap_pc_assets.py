#!/usr/bin/env python3
"""One-shot PC asset pipeline: fetch Ok release → unpack → pack StreamingAssets → equip bundle.

Run from repo root (CI / cloud agent / dev machine):

    python3 tools/bootstrap_pc_assets.py

Optional:
    --skip-fetch    use existing zips under legacy/releases/Ok/
    --skip-unpack   keep legacy/unpacked/
    --skip-pack     skip StreamingAssets pack
    --upload        gh release upload equip_arm_bundle.zip to Ok
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OK = ROOT / "legacy" / "releases" / "Ok"
UNPACKED = ROOT / "legacy" / "unpacked"


def run(cmd: list[str], **kw) -> None:
    print("+", " ".join(cmd), flush=True)
    subprocess.run(cmd, cwd=ROOT, check=True, **kw)


def zip_ok(name: str) -> bool:
    p = OK / name
    return p.is_file() and p.stat().st_size > 10_000


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--skip-fetch", action="store_true")
    parser.add_argument("--skip-unpack", action="store_true")
    parser.add_argument("--skip-pack", action="store_true")
    parser.add_argument("--upload", action="store_true", help="Upload equip_arm_bundle.zip to GitHub release Ok")
    args = parser.parse_args()

    if not args.skip_fetch:
        if not all(zip_ok(z) for z in ("Archive.2.zip", "Archive.3.zip")):
            run(["bash", "tools/fetch_ok_release.sh"])

    if not args.skip_unpack:
        run([sys.executable, "tools/unpack_pc_dump.py"])

    if not args.skip_pack:
        run([sys.executable, "tools/pack_mobile_content.py"])
        run([sys.executable, "tools/pack_equip_game.py", "--full"])

    sys.path.insert(0, str(ROOT / "tools"))
    from make_equip_arm_bundle import build

    bundle_info = build(force=True)

    pc_data = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"
    total_bytes = sum(f.stat().st_size for f in pc_data.rglob("*") if f.is_file())
    equip_n = len(list((UNPACKED / "Resource/image/equip").rglob("*.png"))) if (UNPACKED / "Resource/image/equip").is_dir() else 0
    arm_n = len(list((UNPACKED / "Resource/image/arm").rglob("*.png"))) if (UNPACKED / "Resource/image/arm").is_dir() else 0

    summary = {
        "streamingAssetsPcDataBytes": total_bytes,
        "unpackedEquipPng": equip_n,
        "unpackedArmPng": arm_n,
        "equipArmBundle": bundle_info,
    }
    print(json.dumps(summary, indent=2))

    if args.upload:
        bundle = Path(bundle_info["path"])
        run(
            [
                "gh",
                "release",
                "upload",
                "Ok",
                str(bundle),
                "--repo",
                "thanhtinz/Gun_mobile",
                "--clobber",
            ]
        )
        print("Uploaded equip_arm_bundle.zip to release Ok")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
