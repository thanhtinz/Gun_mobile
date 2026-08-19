#!/usr/bin/env python3
"""Pack a playable mobile subset of the PC dump into Unity StreamingAssets.

Includes every map that has both fore.png and fore.map, plus the large Request
tables (templates / shop / quests / balls). Full ~2GB Resource/image stays in
the Ok zips; unpack with tools/unpack_pc_dump.py for ExtraRoots.
"""

from __future__ import annotations

import json
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"
Z2 = ROOT / "legacy" / "releases" / "Ok" / "Archive.2.zip"
Z3 = ROOT / "legacy" / "releases" / "Ok" / "Archive.3.zip"

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
    "Flash/ui/cn_trad/starling/hall_scene/hall_newyear_scene_build.png",
    "Flash/ui/cn_trad/starling/hall_scene/hall_newyear_scene_build.xml",
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

STARTER_ART = (
    "Resource/image/equip/m/head/head1/icon_1.png",
    "Resource/image/equip/f/head/head1/icon_1.png",
    "Resource/image/equip/m/head/head2/icon_1.png",
    "Resource/image/equip/f/head/head2/icon_1.png",
    "Resource/image/equip/m/cloth/cloth1/icon_1.png",
    "Resource/image/equip/f/cloth/cloth1/icon_1.png",
    "Resource/image/equip/m/cloth/cloth2/icon_1.png",
    "Resource/image/equip/f/cloth/cloth2/icon_1.png",
    "Resource/image/arm/axe/00.png",
    "Resource/image/arm/axe/1/icon.png",
    "Resource/image/arm/axe/1/1/game.png",
    "Resource/image/arm/bow/00.png",
    "Resource/image/arm/bow/1/icon.png",
    "Resource/image/arm/gun/00.png",
    "Resource/image/arm/gun/1/icon.png",
)

REQUEST_KEEP = (
    "TemplateAlllist.xml",
    "ShopItemList.xml",
    "shopitemlist_out.xml",
    "QuestList.xml",
    "BallList.xml",
    "bombconfig.xml",
    "LoadMapsItems.xml",
    "NPCInfoList.xml",
    "VipStoreList.xml",
    "TS_EveryDaySignIn.xml",
    "petskillinfo.xml",
    "pettemplateinfo.xml",
    "cardtemplateinfo.xml",
    "newtitleinfo.xml",
    "toteminfo.xml",
    "mounttemplateOUT.xml",
    "SpiritInfoList.xml",
    "foodcomposelist.xml",
    "newlotteryitem.xml",
    "LoadPVEItems.xml",
    "ItemStrengthenList.xml",
    "TS_ElfTemplate.xml",
    "CelebByDayGPList.xml",
    "CelebByConsortiaRiches.xml",
    "fightlabdropitemlist.xml",
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


def copy_request(src: Path) -> int:
    n = 0
    if not src.exists():
        return 0
    keep = {k.lower() for k in REQUEST_KEEP}
    for path in src.rglob("*"):
        if not path.is_file():
            continue
        if path.suffix.lower() not in (".xml", ".txt"):
            continue
        rel = path.relative_to(src).as_posix()
        # Always keep the named tables; also keep other xml under 2MB.
        if path.name.lower() not in keep and path.stat().st_size > 2_000_000:
            continue
        write_bytes("Request/" + rel, path.read_bytes())
        n += 1
    return n


def copy_flash_data(src: Path) -> int:
    n = 0
    if not src.exists():
        return 0
    for path in src.rglob("*"):
        if not path.is_file() or path.stat().st_size > 2_000_000:
            continue
        if path.suffix.lower() not in (".xml", ".txt", ".ui"):
            continue
        rel = "Flash/" + path.relative_to(src).as_posix()
        write_bytes(rel, path.read_bytes())
        n += 1
    return n


def discover_playable_maps(z3: zipfile.ZipFile) -> list[str]:
    art: set[str] = set()
    col: set[str] = set()
    for name in z3.namelist():
        n = name.replace("\\", "/")
        if n.startswith("Resource/image/map/") and n.lower().endswith("fore.png"):
            parts = n.split("/")
            if len(parts) >= 4:
                art.add(parts[3])
        if n.startswith("Service/Road/map/") and n.endswith("/fore.map"):
            parts = n.split("/")
            if len(parts) >= 4:
                col.add(parts[3])
    ids = sorted(art & col, key=lambda x: (0, int(x)) if x.isdigit() else (1, x))
    return ids


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)

    with zipfile.ZipFile(Z2) as z2:
        n_flash = extract_named(z2, FLASH_FILES + STARLING_PREFIXES)
        n_morn = extract_prefix(z2, "Flash/ui/cn_trad/morn/ui/", (".ui",))
        n_uixml = extract_prefix(z2, "Flash/ui/cn_trad/xml/", (".xml",))

    map_ids: list[str] = []
    with zipfile.ZipFile(Z3) as z3:
        n_bomb = extract_prefix(z3, "Resource/image/bomb/", (".png",))
        n_game = extract_prefix(z3, "Resource/image/game/", (".png", ".jpg"))
        n_scene = extract_prefix(z3, "Resource/image/scene/", (".png", ".jpg"))
        n_starter = extract_named(z3, STARTER_ART)
        map_ids = discover_playable_maps(z3)
        n_map = 0
        for mid in map_ids:
            n_map += extract_prefix(z3, f"Resource/image/map/{mid}/")
            n_map += extract_prefix(z3, f"Service/Road/map/{mid}/", (".map",))

    n_req = copy_request(ROOT / "legacy" / "data" / "Request")
    n_flash_data = copy_flash_data(ROOT / "legacy" / "data" / "Flash")

    swf_ok = 0
    try:
        from pack_swf_sprites import pack as pack_swf

        swf_ok = pack_swf().get("ok", 0)
        print("swf extracted", swf_ok)
    except Exception as e:
        print("swf pack skipped:", e)

    shop_ok = {}
    try:
        from pack_shop_icons import pack as pack_shop

        shop_ok = pack_shop()
        print("shop icons", shop_ok)
    except Exception as e:
        print("shop icon pack skipped:", e)

    try:
        from pack_pet_title_art import pack as pack_pet_title

        print("pet/title art", pack_pet_title())
    except Exception as e:
        print("pet/title pack skipped:", e)

    files = [p.relative_to(OUT).as_posix() for p in OUT.rglob("*") if p.is_file() and p.name != "content_index.json"]
    files.sort()
    index = {
        "maps": map_ids,
        "files": files,
        "counts": {
            "flashNamed": n_flash,
            "morn": n_morn,
            "uiXml": n_uixml,
            "bomb": n_bomb,
            "game": n_game,
            "scene": n_scene,
            "starterArt": n_starter,
            "map": n_map,
            "mapIds": len(map_ids),
            "requestCopied": n_req,
            "flashCopied": n_flash_data,
            "swfExtracted": swf_ok,
        },
    }
    (OUT / "content_index.json").write_text(json.dumps(index, indent=2), encoding="utf-8")
    print(json.dumps(index["counts"], indent=2))
    print("files", len(files), "bytes", sum((OUT / f).stat().st_size for f in files))
    print("maps", len(map_ids))


if __name__ == "__main__":
    main()
