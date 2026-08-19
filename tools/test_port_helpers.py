#!/usr/bin/env python3
"""Validate resource helpers against the Ok dump and extracted configs."""

from __future__ import annotations

import hashlib
import json
import sys
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

from port_helpers import (  # noqa: E402
    MapCollision,
    fly_until_map,
    is_zlib,
    launch,
    load_xml,
    parse_morn_views,
    parse_result_table,
)

DATA = ROOT / "legacy" / "data"
RELEASE = ROOT / "legacy" / "releases" / "Ok"
SAMPLES = ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Samples~" / "StreamingAssets"
PCDATA = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"


def sha256_file(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


class Checksums(unittest.TestCase):
    def test_release_hashes(self):
        expected = {
            "Archive.2.zip": "83764cf7c01b93c3f3371abbce1bea3064a5518f2437b3d1f4a6da0090e922e3",
            "Archive.3.zip": "fe6f77910190c22c3e40ef7892a3df7153cda46690007e47dc4181fe7dc083c9",
            "Archive.zip": "41d3bee0e4330ba3f7af26bdac9c4a82c6e7d0fe23f9a89f90a3f791f57092ba",
        }
        for name, digest in expected.items():
            path = RELEASE / name
            self.assertTrue(path.exists(), name)
            self.assertEqual(sha256_file(path), digest, name)


class XmlHelpers(unittest.TestCase):
    def test_config_language(self):
        root = load_xml((DATA / "Flash" / "config.xml").read_bytes())
        lang = root.find("config").find("LANGUAGE").get("value")
        self.assertEqual(lang, "cn_trad")
        frame = root.find("config").find("GAME_FRAME_CONFIG").find("FRAME_TIME_OVER_TAG").get("value")
        self.assertEqual(frame, "67")

    def test_bombconfig_table(self):
        raw = (DATA / "Request" / "bombconfig.xml").read_bytes()
        self.assertTrue(raw.startswith(b"<Result") or is_zlib(raw))
        root = load_xml(raw)
        rows = parse_result_table(root)
        self.assertGreater(len(rows), 10)
        self.assertIn("TemplateID", rows[0])
        self.assertIn("Common", rows[0])

    def test_nested_shop_and_templates(self):
        shop = parse_result_table(load_xml((DATA / "Request" / "shopitemlist_out.xml").read_bytes()))
        self.assertGreater(len(shop), 50)
        self.assertIn("TemplateID", shop[0])
        self.assertIn("AValue1", shop[0])
        templates = parse_result_table(load_xml((DATA / "Request" / "TemplateAlllist.xml").read_bytes()))
        self.assertGreater(len(templates), 100)
        self.assertIn("TemplateID", templates[0])
        self.assertIn("Name", templates[0])
        quests = parse_result_table(load_xml((DATA / "Request" / "QuestList.xml").read_bytes()))
        self.assertGreater(len(quests), 100)
        maps = parse_result_table(load_xml((DATA / "Request" / "LoadMapsItems.xml").read_bytes()))
        self.assertGreater(len(maps), 50)
        balls = parse_result_table(load_xml((DATA / "Request" / "BallList.xml").read_bytes()))
        self.assertIn("Mass", balls[0])
        self.assertIn("Wind", balls[0])

    def test_character_define_actions(self):
        root = load_xml((DATA / "Flash" / "characterdefine.xml").read_bytes())
        names = [a.get("name") for a in root.find("actionSet").findall("action")]
        self.assertIn("stand", names)
        self.assertIn("walk", names)

    def test_morn_setting_views(self):
        path = DATA / "Flash" / "ui" / "cn_trad" / "morn" / "ui" / "setting.ui"
        views = parse_morn_views(path.read_bytes())
        self.assertGreaterEqual(len(views), 3)
        names = [v[0] for v in views]
        self.assertTrue(any("Setting" in n for n in names))

    def test_starling_atlas(self):
        path = DATA / "Flash" / "ui" / "cn_trad" / "starling" / "game" / "game.xml"
        root = load_xml(path.read_bytes())
        self.assertEqual(root.get("imagePath"), "game.png")
        self.assertGreater(len(list(root)), 20)


class MapAndBallistics(unittest.TestCase):
    def test_fore_map_header(self):
        data = (SAMPLES / "Maps" / "1056" / "fore.map").read_bytes()
        m = MapCollision.load(data)
        self.assertEqual(m.width, 1250)
        self.assertEqual(m.height, 942)
        self.assertEqual(m.stride, 157)
        self.assertGreater(m.solid_count(), 50)

    def test_shot_is_deterministic(self):
        data = (SAMPLES / "Maps" / "1056" / "fore.map").read_bytes()
        m = MapCollision.load(data)
        a = fly_until_map(launch(120, 800, 55, 70), wind=0, m=m)
        b = fly_until_map(launch(120, 800, 55, 70), wind=0, m=m)
        self.assertAlmostEqual(a.x, b.x, places=5)
        self.assertAlmostEqual(a.y, b.y, places=5)
        self.assertGreater(a.t, 0.5)


class MobilePack(unittest.TestCase):
    def test_unity_project_exists(self):
        self.assertTrue((ROOT / "UnityClient" / "ProjectSettings" / "ProjectVersion.txt").exists())
        version = (ROOT / "UnityClient" / "ProjectSettings" / "ProjectVersion.txt").read_text(encoding="utf-8")
        self.assertIn("6000.3.22f1", version)
        self.assertTrue((ROOT / "UnityClient" / "Assets" / "Scenes" / "Boot.unity").exists())
        self.assertTrue((ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "GameApp.cs").exists())

    def test_packed_maps_and_index(self):
        index = PCDATA / "content_index.json"
        self.assertTrue(index.exists())
        for mid in ("1056", "2001", "1005", "1010", "1029", "1048"):
            self.assertTrue((PCDATA / "Service" / "Road" / "map" / mid / "fore.map").exists(), mid)
            self.assertTrue((PCDATA / "Resource" / "image" / "map" / mid / "fore.png").exists(), mid)
        self.assertTrue((PCDATA / "Flash" / "config.xml").exists())
        self.assertTrue((PCDATA / "Request" / "bombconfig.xml").exists())
        self.assertTrue((PCDATA / "Request" / "TemplateAlllist.xml").exists() or (PCDATA / "Request" / "shopitemlist_out.xml").exists())
        self.assertTrue((PCDATA / "Flash" / "ui" / "cn_trad" / "starling" / "hall_scene" / "hall_scene.png").exists())
        index = json.loads((PCDATA / "content_index.json").read_text(encoding="utf-8"))
        self.assertGreaterEqual(len(index.get("maps", [])), 100)
        self.assertTrue((ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "GameplayScreens.cs").exists())
        self.assertTrue((ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Res" / "GameDatabase.cs").exists())
        self.assertTrue((ROOT / ".github" / "workflows" / "build-mobile.yml").exists())
        self.assertTrue((ROOT / "UnityClient" / "ci" / "android-debug.keystore").exists())
        self.assertTrue((ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "SystemsScreens.cs").exists())
        for table in (
            "pettemplateinfo.xml",
            "cardtemplateinfo.xml",
            "newtitleinfo.xml",
            "toteminfo.xml",
            "newlotteryitem.xml",
            "LoadPVEItems.xml",
            "NPCInfoList.xml",
            "TS_ElfTemplate.xml",
            "SpiritInfoList.xml",
            "foodcomposelist.xml",
            "CelebByDayGPList.xml",
        ):
            self.assertTrue((PCDATA / "Request" / table).exists(), table)

    def test_hall_systems_tables(self):
        pets = parse_result_table(load_xml((DATA / "Request" / "pettemplateinfo.xml").read_bytes()))
        self.assertGreater(len(pets), 50)
        self.assertIn("HighAttack", pets[0])
        pve = parse_result_table(load_xml((DATA / "Request" / "LoadPVEItems.xml").read_bytes()))
        self.assertGreaterEqual(len(pve), 10)
        npcs = parse_result_table(load_xml((DATA / "Request" / "NPCInfoList.xml").read_bytes()))
        self.assertGreater(len(npcs), 100)
        titles = parse_result_table(load_xml((DATA / "Request" / "newtitleinfo.xml").read_bytes()))
        self.assertIn("Name", titles[0])
        app = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "GameApp.cs").read_text(encoding="utf-8")
        for module in (
            "PetScreen",
            "DungeonScreen",
            "NpcHuntScreen",
            "ForgeScreen",
            "LotteryScreen",
            "WorldBossScreen",
            "ConsortiaScreen",
            "ChatScreen",
        ):
            self.assertIn(module, app)
        self.assertIn("ShowBattle(int mapId, int npcId", app)

    def test_battle_art_and_signin(self):
        living = PCDATA / "Resource" / "image" / "game" / "living" / "living948.png"
        self.assertTrue(living.exists())
        raw = living.read_bytes()
        self.assertEqual(raw[:2], b"PK")
        import zipfile, io, struct
        zf = zipfile.ZipFile(io.BytesIO(raw))
        names = zf.namelist()
        self.assertTrue(any(n.lower().endswith(".png") for n in names))
        self.assertTrue(any(n.lower().endswith(".xml") for n in names))
        png = zf.read(next(n for n in names if n.lower().endswith(".png")))
        self.assertEqual(png[:8], b"\x89PNG\r\n\x1a\n")
        w, h = struct.unpack(">II", png[16:24])
        self.assertGreaterEqual(w, 256)
        self.assertGreaterEqual(h, 256)
        crater = PCDATA / "Resource" / "image" / "bomb" / "crater" / "65" / "crater1.png"
        self.assertTrue(crater.exists())
        self.assertEqual(crater.read_bytes()[:8], b"\x89PNG\r\n\x1a\n")
        self.assertTrue((PCDATA / "Resource" / "image" / "arm" / "axe" / "00.png").exists())
        self.assertTrue((PCDATA / "Resource" / "image" / "equip" / "m" / "head" / "head1" / "icon_1.png").exists())
        self.assertTrue((ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Res" / "SpriteSheet.cs").exists())
        gp = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "GameplayScreens.cs").read_text(encoding="utf-8")
        self.assertIn("BattleResultScreen", gp)
        self.assertIn("TS_EveryDaySignIn", gp)
        sign = parse_result_table(load_xml((DATA / "Request" / "TS_EveryDaySignIn.xml").read_bytes()))
        self.assertEqual(len(sign), 28)

    def test_swf_extract_and_phone_packet(self):
        from swf_extract import largest_image, write_largest
        import zipfile, struct
        z3 = RELEASE / "Archive.3.zip"
        with zipfile.ZipFile(z3) as zf:
            data = zf.read("Resource/image/game/living/living094.swf")
        hit = largest_image(data)
        self.assertIsNotNone(hit)
        ext, blob = hit
        self.assertIn(ext, (".jpg", ".png"))
        self.assertGreater(len(blob), 500)
        if ext == ".jpg":
            self.assertEqual(blob[:2], b"\xff\xd8")
        extracted = PCDATA / "Resource" / "image" / "game" / "living" / "extracted"
        self.assertTrue(extracted.exists())
        self.assertGreaterEqual(len(list(extracted.glob("*"))), 50)
        self.assertTrue((PCDATA / "Resource" / "image" / "swf_extract_index.json").exists())
        self.assertTrue((ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Net" / "PhonePacket.cs").exists())
        magic = 0x7D01
        body = b'{"ok":true}'
        pkt = struct.pack("<IHH", 4 + len(body), magic, 2) + body
        payload, mag, mid = struct.unpack_from("<IHH", pkt)
        self.assertEqual(mag, magic)
        self.assertEqual(mid, 2)
        self.assertEqual(pkt[8:], body)
        src = (ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Net" / "PhonePacket.cs").read_text(encoding="utf-8")
        self.assertIn("0x7D01", src)
        self.assertIn("4396", src)
        self.assertIn("1910", src)
        living = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PcArt.cs").read_text(encoding="utf-8")
        self.assertIn("NpcLiving", living)
        self.assertIn("extracted", living)
        net = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PhoneNet.cs").read_text(encoding="utf-8")
        self.assertIn("ConnectFight", net)
        self.assertIn("SendFire", net)
        self.assertTrue((ROOT / "UnityClient" / "Assets" / "Plugins" / "Android" / "AndroidManifest.xml").exists())
        manifest = (ROOT / "UnityClient" / "Assets" / "Plugins" / "Android" / "AndroidManifest.xml").read_text(encoding="utf-8")
        self.assertIn("android.permission.INTERNET", manifest)
        bullets = PCDATA / "Resource" / "image" / "bomb" / "bullet" / "extracted"
        self.assertTrue(bullets.exists())
        self.assertGreaterEqual(len(list(bullets.glob("*"))), 20)


if __name__ == "__main__":
    unittest.main(verbosity=2)
