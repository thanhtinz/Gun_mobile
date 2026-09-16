#!/usr/bin/env python3
"""Validate resource helpers against the Ok dump and extracted configs."""

from __future__ import annotations

import hashlib
import json
import struct
import sys
import tempfile
import xml.etree.ElementTree as ET
import zlib
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

from port_helpers import (  # noqa: E402
    MapCollision,
    deobfuscate,
    parse_morn_views,
    write_asset,
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


class Obfuscation(unittest.TestCase):
    """The PC build hides some art behind a prefix and a complemented byte."""

    PREFIX = b"\x00\x03\x5e\x5f\x5e"
    PREFIXED_PNG = (
        PREFIX
        + b"\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR\xff\x00\x00\x50\x00\x00\x00\x50"
    )

    def test_prefix_and_damaged_byte_are_both_repaired(self):
        clean = deobfuscate(self.PREFIXED_PNG)
        self.assertTrue(clean.startswith(b"\x89PNG\r\n\x1a\n"))
        self.assertEqual(clean[16], 0x00, "the damaged byte must be restored")
        self.assertEqual(int.from_bytes(clean[16:20], "big"), 80)

    def test_byte_sixteen_is_complemented_whatever_the_format(self):
        """PNG is not special: the packer complements offset 16 sight unseen.

        Reading it as "clear the IHDR width high byte" happens to be right for
        PNG and wrong for everything else, which is why 63 obfuscated .swf files
        were dropped by both the packer and the client.
        """
        for payload in (b"CWS\x09" + bytes(range(4, 40)), b"\xff\xd8" + b"\x5a" * 38):
            with self.subTest(head=payload[:4]):
                clean = deobfuscate(self.PREFIX + payload)
                self.assertEqual(clean[:16], payload[:16])
                self.assertEqual(clean[16], payload[16] ^ 0xFF)
                self.assertEqual(clean[17:], payload[17:])

    def test_a_short_payload_is_not_read_past_its_end(self):
        short = b"\x89PNG\r\n\x1a\n"
        self.assertEqual(deobfuscate(self.PREFIX + short), short)

    def test_clean_data_is_returned_untouched(self):
        plain = b"\x89PNG\r\n\x1a\n" + b"\x00" * 32
        self.assertIs(deobfuscate(plain), plain)

    def test_obfuscated_swf_reaches_the_extractor(self):
        """swf_body must deobfuscate before it looks at the signature."""
        import swf_extract

        body = b"\x00" * 5 + b"\x00\x00\x00\x00"  # empty RECT, rate, frame count
        swf = bytearray(b"FWS\x09" + len(body).to_bytes(4, "little") + body)
        swf[16] ^= 0xFF
        packed = self.PREFIX + bytes(swf)

        # As stored, the file looks like nothing: the signature bytes a reader
        # would test are the prefix, and the body byte is complemented. Both
        # have to be undone before the tag walk, and swf_body owns that.
        self.assertNotIn(packed[:3], (b"CWS", b"FWS"))
        self.assertEqual(swf_extract.swf_body(packed), body)

    def test_no_packed_asset_still_carries_the_prefix(self):
        """Regression: 272 crater PNGs shipped obfuscated and never decoded."""
        offenders = []
        for root in (PCDATA, SAMPLES):
            if not root.exists():
                continue
            for path in root.rglob("*"):
                if path.is_file() and path.read_bytes()[:5] == b"\x00\x03\x5e\x5f\x5e":
                    offenders.append(str(path.relative_to(ROOT)))
        self.assertEqual(offenders[:5], [], f"{len(offenders)} obfuscated assets packed")

    def test_packed_pngs_have_a_sane_header(self):
        pngs = [p for p in (PCDATA / "Resource" / "image" / "bomb").rglob("*.png")]
        self.assertGreater(len(pngs), 50, "no bomb art packed")
        for path in pngs:
            head = path.read_bytes()[:24]
            with self.subTest(png=path.name):
                self.assertTrue(head.startswith(b"\x89PNG\r\n\x1a\n"))
                width = int.from_bytes(head[16:20], "big")
                height = int.from_bytes(head[20:24], "big")
                self.assertTrue(0 < width <= 8192, f"implausible width {width}")
                self.assertTrue(0 < height <= 8192, f"implausible height {height}")


def _amf3_u29(value: int) -> bytes:
    if value < 0x80:
        return bytes([value])
    if value < 0x4000:
        return bytes([(value >> 7) | 0x80, value & 0x7F])
    if value < 0x200000:
        return bytes([(value >> 14) | 0x80, ((value >> 7) & 0x7F) | 0x80, value & 0x7F])
    return bytes([(value >> 22) | 0x80, ((value >> 15) & 0x7F) | 0x80,
                  ((value >> 8) & 0x7F) | 0x80, value & 0xFF])


def _amf3_str(text: str) -> bytes:
    raw = text.encode("utf-8")
    return _amf3_u29((len(raw) << 1) | 1) + raw


def _ui_bundle(views) -> bytes:
    """Build a .ui bundle the way the PC tooling does: zlib over one AMF3 object."""
    body = bytearray([10])           # object marker
    body += _amf3_u29(0x0B)          # inline traits, dynamic, no sealed members
    body += _amf3_str("")            # empty class name
    for key, markup in views:
        body += _amf3_str(key)
        raw = markup.encode("utf-8")
        body += bytes([11]) + _amf3_u29((len(raw) << 1) | 1) + raw
    body += _amf3_str("")            # end of the dynamic section
    return zlib.compress(bytes(body))


class RepeatedChildren(unittest.TestCase):
    """A flat attribute map cannot hold a child element that repeats."""

    SAMPLE = (
        "<Result>"
        '  <Item ID="1" Title="a">'
        '    <Item_Good RewardItemID="11" RewardItemCount1="2" />'
        '    <Item_Good RewardItemID="22" RewardItemCount1="3" />'
        "    <Note>plain text</Note>"
        "  </Item>"
        '  <Item ID="2" Title="b" />'
        "</Result>"
    )

    def rows(self):
        return parse_result_table(ET.fromstring(self.SAMPLE))

    def test_every_repeat_survives_in_document_order(self):
        rewards = self.rows()[0].children["Item_Good"]
        self.assertEqual([r["RewardItemID"] for r in rewards], ["11", "22"])
        self.assertEqual(rewards[0]["RewardItemCount1"], "2")

    def test_a_row_is_still_a_plain_attribute_map(self):
        """Every existing caller reads rows as dicts; that must not change."""
        rows = self.rows()
        self.assertIsInstance(rows[0], dict)
        self.assertEqual(rows[0]["ID"], "1")
        self.assertEqual(rows[1]["Title"], "b")
        self.assertEqual(rows[1].children, {})

    def test_text_only_children_stay_in_the_map(self):
        row = self.rows()[0]
        self.assertEqual(row["Note"], "plain text")
        self.assertNotIn("Note", row.children)

    def test_shipped_quest_rewards_are_reachable(self):
        """Regression: QuestList's rewards read as "" and all but one were lost."""
        quests = parse_result_table(load_xml((DATA / "Request" / "QuestList.xml").read_bytes()))
        goods = sum(len(q.children.get("Item_Good", ())) for q in quests)
        conditions = sum(len(q.children.get("Item_Condiction", ())) for q in quests)
        self.assertGreater(goods, 1000, "quest item rewards are unreachable")
        self.assertGreater(conditions, 1000, "quest conditions are unreachable")
        for quest in quests:
            for reward in quest.children.get("Item_Good", ()):
                self.assertIn("RewardItemID", reward)


class PkmHeaders(unittest.TestCase):
    """ETC pads to 4x4 blocks, so the authored size has to be written down."""

    def header(self, width, height, fmt=3):
        import png_to_pkm

        return png_to_pkm.write_pkm_header(width, height, fmt)

    def fields(self, header):
        self.assertEqual(header[:6], b"PKM 20")
        return struct.unpack_from(">HHHHH", header, 6)

    def test_padded_and_authored_sizes_are_both_recorded(self):
        kind, ext_w, ext_h, w, h = self.fields(self.header(90, 50))
        self.assertEqual(kind, 3)
        self.assertEqual((ext_w, ext_h), (92, 52), "payload covers whole blocks")
        self.assertEqual((w, h), (90, 50), "authored size must survive the bake")

    def test_exact_multiples_pad_to_themselves(self):
        _, ext_w, ext_h, w, h = self.fields(self.header(64, 32))
        self.assertEqual((ext_w, ext_h), (64, 32))
        self.assertEqual((w, h), (64, 32))

    def test_the_data_type_has_its_own_field(self):
        """Regression: the format was written at offset 15 and read at 14.

        Offset 14 is the authored height, so the format a reader saw was the
        image's height. Heights of 1 and 4 map to formats whose payload is a
        different size, and the texture upload fails outright.
        """
        for fmt in (1, 3, 4):
            with self.subTest(fmt=fmt):
                self.assertEqual(self.fields(self.header(40, 4, fmt))[0], fmt)

    def test_the_header_is_sixteen_bytes(self):
        self.assertEqual(len(self.header(90, 50)), 16)


class MornBundles(unittest.TestCase):
    """The .ui reader must read the AMF3 container, not scan for <View> tags.

    The scan it replaced was wrong three ways on the shipped data: a
    self-closing <View .../> has no closing tag, so the scan ran past it into
    the next view and produced a chunk with two roots and raw AMF3 length bytes
    between them — not well-formed, so parsing threw and took the whole screen
    with it (magicStone, magicStone_bk, dreamlandChallenge). Names walked
    backwards from ".xml" over letters/digits/_-. and so swallowed the AMF3
    length prefix whenever that byte was one of those, which corrupted 580 of
    the 950 shipped view names. And a view holding a nested <View> closed early.
    """

    def test_reads_names_and_sizes_in_stored_order(self):
        bundle = _ui_bundle([
            ("bank/BankMainFrame.xml", '<View width="334" height="383"/>'),
            ("view/BibleMainView.xml", '<View width="600" height="400"><Image skin="a"/></View>'),
        ])
        self.assertEqual(
            parse_morn_views(bundle),
            [("bank/BankMainFrame.xml", 334, 383), ("view/BibleMainView.xml", 600, 400)],
        )

    def test_self_closing_view_does_not_swallow_the_next_one(self):
        """magicStone.ui: MagicStoneMain is <View .../> and the scan lost two views."""
        bundle = _ui_bundle([
            ("MagicStoneMain.xml", '<View width="600" height="400"/>'),
            ("MagicJadeProTip.xml", '<View width="153" height="300"><Image skin="bg"/></View>'),
        ])
        views = parse_morn_views(bundle)
        self.assertEqual([name for name, _, _ in views],
                         ["MagicStoneMain.xml", "MagicJadeProTip.xml"])

    def test_length_prefix_never_leaks_into_the_name(self):
        """A key of 24 bytes has length byte (24<<1)|1 = 0x31 = '1', a digit.

        The old backwards walk treated that as part of the name.
        """
        key = "view/BibleStrongVie.xml"
        self.assertEqual(((len(key) << 1) | 1), ord("/"))
        name = parse_morn_views(_ui_bundle([(key, '<View width="1" height="2"/>')]))[0][0]
        self.assertEqual(name, key)

    def test_nested_view_element_stays_with_its_parent(self):
        markup = '<View width="10" height="20"><View width="5" height="5"/></View>'
        views = parse_morn_views(_ui_bundle([("outer.xml", markup)]))
        self.assertEqual(views, [("outer.xml", 10, 20)])

    def test_unreadable_view_is_skipped_not_fatal(self):
        bundle = _ui_bundle([
            ("broken.xml", "<View width='1'"),
            ("fine.xml", '<View width="7" height="8"/>'),
        ])
        self.assertEqual(parse_morn_views(bundle), [("fine.xml", 7, 8)])

    def test_non_bundle_bytes_are_rejected_loudly(self):
        with self.assertRaises(ValueError):
            parse_morn_views(zlib.compress(b"<View width='1' height='2'/>"))


class AssetWriters(unittest.TestCase):
    """Every packer must copy assets through the de-obfuscating writer.

    pack_mobile_content.main() also runs pack_shop_icons, pack_pet_title_art
    and pack_equip_game. Those wrote bytes straight out of the zip, so a
    regenerated pack re-obfuscated 252 of the 272 files the prefix fix had
    repaired (17 farm, 97 pet/title, 138 equip/arm) and broke the check below.
    """

    PACKERS = (
        "pack_mobile_content.py",
        "pack_shop_icons.py",
        "pack_pet_title_art.py",
        "pack_equip_game.py",
    )

    def test_write_asset_repairs_and_creates_parents(self):
        dest = Path(tempfile.mkdtemp()) / "nested" / "icon.png"
        raw = Obfuscation.PREFIXED_PNG
        written = write_asset(dest, raw)

        self.assertTrue(dest.exists(), "write_asset must create missing parents")
        clean = dest.read_bytes()
        self.assertEqual(clean, deobfuscate(raw))
        self.assertEqual(written, len(clean))

    def test_clean_bytes_survive_the_writer_unchanged(self):
        dest = Path(tempfile.mkdtemp()) / "plain.png"
        raw = b"\x89PNG\r\n\x1a\n" + b"\x00" * 32
        self.assertEqual(write_asset(dest, raw), len(raw))
        self.assertEqual(dest.read_bytes(), raw)

    def test_no_packer_writes_asset_bytes_directly(self):
        """Regression: a raw write here re-ships obfuscated art.

        Copying bytes with dest.write_bytes(...) or shutil.copy2(...) bypasses
        the repair, and nothing else in the suite would notice until someone
        regenerated the pack.
        """
        offenders = []
        for name in self.PACKERS:
            source = (ROOT / "tools" / name).read_text(encoding="utf-8")
            for number, text in enumerate(source.splitlines(), start=1):
                stripped = text.strip()
                if stripped.startswith("#"):
                    continue
                if ".write_bytes(" in stripped or "shutil.copy2(" in stripped:
                    offenders.append(f"{name}:{number}: {stripped}")
        self.assertEqual(offenders, [], "these must go through write_asset()")


class MapAndBallistics(unittest.TestCase):
    def test_fore_map_header(self):
        data = (SAMPLES / "Maps" / "1056" / "fore.map").read_bytes()
        m = MapCollision.load(data)
        self.assertEqual(m.width, 1250)
        self.assertEqual(m.height, 942)
        self.assertEqual(m.stride, 157)
        self.assertGreater(m.solid_count(), 50)

    def test_stride_keeps_the_writers_spare_byte(self):
        """A width divisible by eight still carries one extra byte per row.

        ``ceil(width / 8)`` looks right and is wrong for exactly those widths:
        every row then shifts one byte left and the terrain dissolves. 47 of the
        127 maps shipped in this repo have such a width.
        """
        self.assertEqual(MapCollision.stride_for(1250), 157)  # not divisible by 8
        self.assertEqual(MapCollision.stride_for(2000), 251)  # divisible; ceil gives 250
        self.assertEqual(MapCollision.stride_for(1600), 201)  # divisible; ceil gives 200

    def test_every_shipped_map_matches_the_stride(self):
        maps = sorted(PCDATA.glob("Service/Road/map/*/*.map")) + sorted(SAMPLES.glob("Maps/*/*.map"))
        self.assertGreater(len(maps), 100, "no map data found to validate")
        for path in maps:
            with self.subTest(map=path.parent.name):
                # load() now rejects any payload whose size disagrees with the
                # stride, so parsing every map is itself the assertion.
                m = MapCollision.load(path.read_bytes())
                self.assertEqual(len(m.bits), m.stride * m.height)

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
        skin = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PcSkin.cs").read_text(encoding="utf-8")
        self.assertIn("TryLoadStarling", skin)
        self.assertIn("hall_scene", skin)
        self.assertIn("Flash", skin)
        sheet = (ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Res" / "SpriteSheet.cs").read_text(encoding="utf-8")
        self.assertIn("TryLoadStarling", sheet)
        self.assertTrue((PCDATA / "Flash" / "ui" / "cn_trad" / "starling" / "hall_scene" / "hall_scene.png").exists())
        self.assertTrue((PCDATA / "Flash" / "ui" / "cn_trad" / "starling" / "game" / "game.png").exists())
        self.assertTrue((PCDATA / "Flash" / "1.png").exists())
        self.assertTrue((PCDATA / "Flash" / "ui" / "cn_trad" / "starling" / "hall_scene" / "hall_newyear_scene_build.png").exists())
        hall = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "HallScreens.cs").read_text(encoding="utf-8")
        self.assertIn("hall_new_fight", hall)
        self.assertIn("hall_new_rankbg", hall)
        self.assertIn("ItemIcon", (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PcArt.cs").read_text(encoding="utf-8"))
        art = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PcArt.cs").read_text(encoding="utf-8")
        self.assertIn("unfrightprop", art)
        self.assertIn('return "eff"', art)
        battle = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "BattleRuntime.cs").read_text(encoding="utf-8")
        self.assertIn("game_prop_", battle)
        self.assertIn("BuildPropBar", battle)
        balls = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "SystemsScreens.cs").read_text(encoding="utf-8")
        self.assertIn("PcArt.Bullet", balls)
        self.assertTrue((PCDATA / "Resource" / "image" / "unfrightprop").exists() or (PCDATA / "Resource" / "image" / "arm" / "axe" / "1" / "icon.png").exists())
        packed_icons = list((PCDATA / "Resource" / "image").rglob("icon_1.png")) + list((PCDATA / "Resource" / "image").rglob("icon.png"))
        self.assertGreaterEqual(len(packed_icons), 40)
        net = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PhoneNet.cs").read_text(encoding="utf-8")
        self.assertIn("ConnectFight", net)
        self.assertIn("SendFire", net)
        self.assertIn("SendWalk", net)
        pkt_cs = (ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Net" / "PhonePacket.cs").read_text(encoding="utf-8")
        self.assertIn("FightWalk = 92", pkt_cs)
        self.assertIn("PetIcon", living)
        self.assertIn("TitleBanner", living)
        self.assertIn("ApplyNetWalk", battle)
        self.assertIn("game_tombAsset", battle)
        self.assertIn("game_moveStripBgAsset", battle)
        self.assertIn("_shotRemaining", battle)
        self.assertIn("SpawnDmgPopup", battle)
        self.assertIn("BuildLivingPreview", (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "GameplayScreens.cs").read_text(encoding="utf-8"))
        server_cs = (ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Net" / "MobileGameServer.cs").read_text(encoding="utf-8")
        self.assertIn("MobileGameServer", server_cs)
        self.assertIn("HandleShopBuy", server_cs)
        self.assertIn("HandleFightStart", server_cs)
        self.assertIn("HandleStrengthen", server_cs)
        self.assertIn("HandleLottery", server_cs)
        self.assertIn("SavePlayer", server_cs)
        self.assertIn("BroadcastToRoom", server_cs)
        pkt_cs = (ROOT / "UnityClient" / "Packages" / "com.gunmobile.port" / "Runtime" / "Net" / "PhonePacket.cs").read_text(encoding="utf-8")
        self.assertIn("ShopBuy = 30", pkt_cs)
        self.assertIn("CreateRoom = 82", pkt_cs)
        self.assertIn("FightDamage = 94", pkt_cs)
        self.assertIn("ChatBroadcast = 78", pkt_cs)
        net_cs = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "PhoneNet.cs").read_text(encoding="utf-8")
        self.assertIn("MobileGameServer", net_cs)
        self.assertIn("RequestProfile", net_cs)
        self.assertIn("ShopBuy", net_cs)
        self.assertIn("DrawLottery", net_cs)
        self.assertIn("UpgradeVip", net_cs)
        self.assertIn("TrainTexp", net_cs)
        self.assertIn("UpgradeGem", net_cs)
        self.assertIn("HandleVipUpgrade", server_cs)
        self.assertIn("HandleTexpTrain", server_cs)
        self.assertIn("HandleGemUpgrade", server_cs)
        self.assertIn("VipUpgrade = 67", pkt_cs)
        self.assertIn("TexpTrain = 68", pkt_cs)
        self.assertIn("GemUpgrade = 69", pkt_cs)
        hall = (ROOT / "UnityClient" / "Assets" / "Scripts" / "Client" / "HallScreens.cs").read_text(encoding="utf-8")
        self.assertIn("hall_scene_build_title_roomList", hall)
        self.assertTrue((PCDATA / "Resource" / "image" / "pet").exists() or (PCDATA / "Resource" / "image" / "title").exists())
        self.assertTrue((ROOT / "UnityClient" / "Assets" / "Plugins" / "Android" / "AndroidManifest.xml").exists())
        manifest = (ROOT / "UnityClient" / "Assets" / "Plugins" / "Android" / "AndroidManifest.xml").read_text(encoding="utf-8")
        self.assertIn("android.permission.INTERNET", manifest)
        bullets = PCDATA / "Resource" / "image" / "bomb" / "bullet" / "extracted"
        self.assertTrue(bullets.exists())
        self.assertGreaterEqual(len(list(bullets.glob("*"))), 20)


if __name__ == "__main__":
    unittest.main(verbosity=2)
