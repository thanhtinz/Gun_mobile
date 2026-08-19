#!/usr/bin/env python3
"""Copy shop/bag item icons from Archive.3.zip into StreamingAssets.

Does not re-pack maps. Paths stay identical to the PC Resource/image layout.
"""

from __future__ import annotations

import json
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"
Z3 = ROOT / "legacy" / "releases" / "Ok" / "Archive.3.zip"
DATA = ROOT / "legacy" / "data" / "Request"

STARTER_IDS = (7001, 1102, 1103, 5102, 5103)

EQUIP_SLOTS = {
    1: "head",
    2: "glass",
    3: "hair",
    4: "eff",
    5: "cloth",
    6: "face",
    8: "armlet",
    9: "ring",
    13: "suits",
    14: "necklace",
    15: "wing",
    16: "offhand",
    17: "offhand",
}


def _load_table(name: str) -> list[dict[str, str]]:
    from port_helpers import load_xml, parse_result_table

    path = DATA / name
    if not path.exists():
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
            low = rest.lower()
            if low not in out:
                out.append(low)
        if p.startswith("s") and len(p) > 1:
            rest = p[1:]
            if rest not in out:
                out.append(rest)
    return out


def _candidates(cat: int, pic: str) -> list[str]:
    paths: list[str] = []
    slot = EQUIP_SLOTS.get(cat, "")
    for p in _pic_variants(pic):
        if slot in ("head", "glass", "hair", "eff", "cloth", "face", "suits"):
            for sex in ("m", "f"):
                paths.append(f"Resource/image/equip/{sex}/{slot}/{p}/icon_1.png")
        elif slot:
            paths.append(f"Resource/image/equip/{slot}/{p}/icon_1.png")
            paths.append(f"Resource/image/equip/{slot}/{p}/icon.png")
        if cat == 7:
            paths.append(f"Resource/image/arm/{p}/1/icon.png")
            paths.append(f"Resource/image/arm/{p}/icon.png")
            paths.append(f"Resource/image/arm/{p}/00.png")
        if cat == 12:
            paths.append(f"Resource/image/task/{p}/icon.png")
        if cat in (16,):
            paths.append(f"Resource/image/specialprop/chatball/{p.lower()}/icon.png")
        if cat == 32:
            paths.append(f"Resource/image/farm/crops/{p}/seed.png")
            paths.append(f"Resource/image/farm/crops/{p.lower()}/seed.png")
        paths.append(f"Resource/image/unfrightprop/{p}/icon.png")
        paths.append(f"Resource/image/prop/{p}/icon.png")
        paths.append(f"Resource/image/gift/{p}/icon.png")
        paths.append(f"Resource/image/gift/{p.lower()}/icon.png")
        paths.append(f"Resource/image/buff/{p}/icon.png")
        paths.append(f"Resource/image/pet/{p}/icon1.png")
        paths.append(f"Resource/image/elf/{p}/icon.png")
    small: list[str] = []
    huge: list[str] = []
    seen: set[str] = set()
    for path in paths:
        if path in seen:
            continue
        seen.add(path)
        if path.endswith("00.png"):
            huge.append(path)
        else:
            small.append(path)
    return small + huge


def pack() -> dict:
    import sys

    sys.path.insert(0, str(ROOT / "tools"))

    templates = {int(r["TemplateID"]): r for r in _load_table("TemplateAlllist.xml") if r.get("TemplateID", "").isdigit()}
    shop = _load_table("shopitemlist_out.xml") or _load_table("ShopItemList.xml")
    wanted: dict[tuple[int, str], tuple[int, str]] = {}
    for offer in shop:
        tid = offer.get("TemplateID", "")
        if not tid.isdigit() or int(tid) not in templates:
            continue
        row = templates[int(tid)]
        cat = int(row.get("CategoryID") or 0)
        pic = (row.get("Pic") or "").strip()
        wanted[(cat, pic.lower())] = (cat, pic)
    for tid in STARTER_IDS:
        if tid not in templates:
            continue
        row = templates[tid]
        cat = int(row.get("CategoryID") or 0)
        pic = (row.get("Pic") or "").strip()
        wanted[(cat, pic.lower())] = (cat, pic)

    copied = 0
    skipped = 0
    missing = 0
    bytes_out = 0
    files: list[str] = []
    with zipfile.ZipFile(Z3) as zf:
        lower = {}
        for name in zf.namelist():
            n = name.replace("\\", "/")
            if "__MACOSX" in n or n.endswith("/"):
                continue
            lower.setdefault(n.lower(), n)
        for cat, pic in wanted.values():
            hit = None
            for rel in _candidates(cat, pic):
                actual = lower.get(rel.lower())
                if actual is None:
                    continue
                if actual.endswith("00.png") and zf.getinfo(actual).file_size > 80_000:
                    continue
                hit = actual
                break
            if hit is None:
                missing += 1
                continue
            dest = OUT / hit
            if dest.exists():
                skipped += 1
                files.append(hit)
                continue
            dest.parent.mkdir(parents=True, exist_ok=True)
            data = zf.read(hit)
            dest.write_bytes(data)
            copied += 1
            bytes_out += len(data)
            files.append(hit)

    idx_path = OUT / "content_index.json"
    if idx_path.exists():
        content = json.loads(idx_path.read_text(encoding="utf-8"))
        have = set(content.get("files") or [])
        for rel in files:
            if rel not in have:
                have.add(rel)
        listing = sorted(have)
        content["files"] = listing
        counts = content.setdefault("counts", {})
        counts["shopIconsPacked"] = copied + skipped
        counts["shopIconsCopied"] = copied
        counts["shopIconsSkipped"] = skipped
        counts["shopIconsMissing"] = missing
        idx_path.write_text(json.dumps(content, indent=2), encoding="utf-8")

    return {
        "wanted": len(wanted),
        "copied": copied,
        "skipped": skipped,
        "missing": missing,
        "bytes": bytes_out,
        "files": len(files),
    }


if __name__ == "__main__":
    print(json.dumps(pack(), indent=2))
