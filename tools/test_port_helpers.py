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


if __name__ == "__main__":
    unittest.main(verbosity=2)
