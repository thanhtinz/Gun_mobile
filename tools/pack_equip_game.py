#!/usr/bin/env python3
"""Copy equip/arm battle game.png sheets into StreamingAssets (PC path layout).

Uses Archive.3.zip when present, else legacy/unpacked/ as source.
Targets every TemplateAlllist item with a body slot or weapon (CategoryID 1-7, 13).
"""

from __future__ import annotations

import json
import shutil
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"
Z3 = ROOT / "legacy" / "releases" / "Ok" / "Archive.3.zip"
UNPACKED = ROOT / "legacy" / "unpacked"
DATA = ROOT / "legacy" / "data" / "Request"

EQUIP_SLOTS = {
    1: "head",
    2: "glass",
    3: "hair",
    4: "eff",
    5: "cloth",
    6: "face",
    13: "suits",
}

FRAMES = ("1", "2", "3", "a", "b")


def _load_table(name: str) -> list[dict[str, str]]:
    from port_helpers import load_xml, parse_result_table

    path = DATA / name
    if not path.exists():
        alt = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData" / "Request" / name
        if alt.exists():
            path = alt
        else:
            return []
    return parse_result_table(load_xml(path.read_bytes()))


def _pic_variants(pic: str) -> list[str]:
    raw = (pic or "").replace("\\", "/").strip()
    if not raw or raw.lower() == "default":
        return []
    out: list[str] = []
    for p in (raw, raw.lower(), raw[:1].lower() + raw[1:] if raw else raw):
        if p and p not in out:
            out.append(p)
        if p.startswith("S") and len(p) > 1:
            rest = p[1:]
            if rest not in out:
                out.append(rest)
    return out


def _game_paths(cat: int, pic: str) -> list[str]:
    paths: list[str] = []
    slot = EQUIP_SLOTS.get(cat, "")
    for p in _pic_variants(pic):
        if slot:
            for sex in ("m", "f"):
                for frame in FRAMES:
                    paths.append(f"Resource/image/equip/{sex}/{slot}/{p}/{frame}/game.png")
        if cat == 7:
            paths.append(f"Resource/image/arm/{p}/1/1/game.png")
            paths.append(f"Resource/image/arm/{p}/1/0/game.png")
            paths.append(f"Resource/image/arm/{p}/00.png")
    return paths


def _copy_one(rel: str, z3: zipfile.ZipFile | None) -> bool:
    dest = OUT / rel
    if dest.exists():
        return False
    src_unpacked = UNPACKED / rel
    if src_unpacked.exists():
        dest.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(src_unpacked, dest)
        return True
    if z3 is not None:
        try:
            data = z3.read(rel)
        except KeyError:
            return False
        dest.parent.mkdir(parents=True, exist_ok=True)
        dest.write_bytes(data)
        return True
    return False


def pack(full: bool = False) -> dict:
    templates = {
        int(r["TemplateID"]): r
        for r in _load_table("TemplateAlllist.xml")
        if r.get("TemplateID", "").isdigit()
    }
    wanted: set[str] = set()
    for tid, row in templates.items():
        cat = int(row.get("CategoryID") or "0")
        pic = row.get("Pic") or row.get("pic") or ""
        if full or cat in EQUIP_SLOTS or cat == 7:
            for rel in _game_paths(cat, pic):
                wanted.add(rel)

    z3 = None
    if Z3.exists() and Z3.stat().st_size > 1000:
        z3 = zipfile.ZipFile(Z3)

    ok = skip = 0
    for rel in sorted(wanted):
        if _copy_one(rel, z3):
            ok += 1
        else:
            skip += 1

    if z3:
        z3.close()

    index_path = OUT / "content_index.json"
    game_files = sorted(
        p.relative_to(OUT).as_posix()
        for p in OUT.rglob("game.png")
        if "Resource/image/equip/" in p.as_posix() or "Resource/image/arm/" in p.as_posix()
    )
    manifest_path = OUT / "equip_game_manifest.json"
    manifest_path.write_text(json.dumps({"files": game_files}, indent=2), encoding="utf-8")

    if index_path.exists():
        index = json.loads(index_path.read_text(encoding="utf-8"))
        files = set(index.get("files", []))
        for rel in wanted:
            if (OUT / rel).exists():
                files.add(rel)
        index["files"] = sorted(files)
        index.setdefault("counts", {})["equipGame"] = ok
        index_path.write_text(json.dumps(index, indent=2), encoding="utf-8")

    return {"wanted": len(wanted), "copied": ok, "missing": skip}


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument("--full", action="store_true", help="All template game.png (large)")
    print(json.dumps(pack(full=parser.parse_args().full), indent=2))
