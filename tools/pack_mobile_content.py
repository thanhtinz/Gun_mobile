#!/usr/bin/env python3
"""Pack a playable mobile subset of the PC dump into Unity StreamingAssets."""

from __future__ import annotations

import hashlib
import json
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"
Z2 = ROOT / "legacy" / "releases" / "Ok" / "Archive.2.zip"
Z3 = ROOT / "legacy" / "releases" / "Ok" / "Archive.3.zip"

MAP_IDS = ("1056", "2001", "1005", "1010", "1029", "1048")
STARLING_PREFIXES = (
    "Flash/ui/cn_trad/starling/hall_scene/hall_scene.png",
    "Flash/ui/cn_trad/starling/hall_scene/hall_scene.xml",
    "Flash/ui/cn_trad/starling/hall_scene/hall_scene2.png",
    "Flash/ui/cn_trad/starling/hall_scene/hall_scene2.xml",
    "Flash/ui/cn_trad/starling/default/default_resource.png",
    "Flash/ui/cn_trad/starling/default/default_resource.xml",
    "Flash/ui/cn_trad/starling/game/game.png",
    "Flash/ui/cn_trad/starling/game/game.xml",
    "Flash/ui/cn_trad/starling/game/gameprop.png",
    "Flash/ui/cn_trad/starling/game/gameprop.xml",
)
FLASH_FILES = (
    "Flash/config.xml",
    "Flash/characterdefine.xml",
    "Flash/ui/cn_trad/language.txt",
    "Flash/ui/cn_trad/language.png",
    "Flash/1.png",
    "Flash/2.png",
    "Flash/3.png",
    "Flash/4.png",
)


def write_bytes(rel: str, data: bytes) -> None:
    dest = OUT / rel
    dest.parent.mkdir(parents=True, exist_ok=True)
    dest.write_bytes(data)


def extract_named(zf: zipfile.ZipFile, names: tuple[str, ...]) -> int:
    n = 0
    available = set(zf.namelist())
    for name in names:
        if name in available:
            write_bytes(name, zf.read(name))
            n += 1
    return n


def extract_prefix(zf: zipfile.ZipFile, prefix: str, suffixes: tuple[str, ...] | None = None) -> int:
    n = 0
    for info in zf.infolist():
        name = info.filename.replace("\\", "/")
        if "__MACOSX" in name or name.endswith("/"):
            continue
        if not name.startswith(prefix):
            continue
        if suffixes and not name.lower().endswith(suffixes):
            continue
        write_bytes(name, zf.read(info))
        n += 1
    return n


def copy_tree(src: Path, dest_prefix: str, max_bytes: int = 2_000_000) -> int:
    n = 0
    if not src.exists():
        return 0
    for path in src.rglob("*"):
        if not path.is_file():
            continue
        if path.stat().st_size > max_bytes:
            continue
        rel = dest_prefix + "/" + path.relative_to(src).as_posix()
        write_bytes(rel, path.read_bytes())
        n += 1
    return n


def main() -> None:
    if OUT.exists():
        # keep folder, overwrite files
        pass
    OUT.mkdir(parents=True, exist_ok=True)

    with zipfile.ZipFile(Z2) as z2:
        n_flash = extract_named(z2, FLASH_FILES + STARLING_PREFIXES)
        n_morn = extract_prefix(z2, "Flash/ui/cn_trad/morn/ui/", (".ui",))
        n_uixml = extract_prefix(z2, "Flash/ui/cn_trad/xml/", (".xml",))

    with zipfile.ZipFile(Z3) as z3:
        n_bomb = extract_prefix(z3, "Resource/image/bomb/", (".png",))
        n_game = extract_prefix(z3, "Resource/image/game/", (".png", ".jpg"))
        n_scene = extract_prefix(z3, "Resource/image/scene/", (".png", ".jpg"))
        n_map = 0
        for mid in MAP_IDS:
            n_map += extract_prefix(z3, f"Resource/image/map/{mid}/")
            n_map += extract_prefix(z3, f"Service/Road/map/{mid}/", (".map",))

    n_req = copy_tree(ROOT / "legacy" / "data" / "Request", "Request", max_bytes=20_000_000)
    n_flash_data = copy_tree(ROOT / "legacy" / "data" / "Flash", "Flash", max_bytes=2_000_000)

    files = [p.relative_to(OUT).as_posix() for p in OUT.rglob("*") if p.is_file()]
    files.sort()
    index = {
        "maps": list(MAP_IDS),
        "files": files,
        "counts": {
            "flashNamed": n_flash,
            "morn": n_morn,
            "uiXml": n_uixml,
            "bomb": n_bomb,
            "game": n_game,
            "scene": n_scene,
            "map": n_map,
            "requestCopied": n_req,
            "flashCopied": n_flash_data,
        },
    }
    (OUT / "content_index.json").write_text(json.dumps(index, indent=2), encoding="utf-8")
    print(json.dumps(index["counts"], indent=2))
    print("files", len(files), "bytes", sum((OUT / f).stat().st_size for f in files))


if __name__ == "__main__":
    main()
