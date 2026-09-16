#!/usr/bin/env python3
"""Pure-python mirrors of the Unity helpers, used to validate the Ok dump."""

from __future__ import annotations

import math
import struct
import zlib
import xml.etree.ElementTree as ET
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, Iterable, List, Optional, Tuple

ZLIB_MAGICS = (b"\x78\x01", b"\x78\x9c", b"\x78\xda")
FRAME_DT = 1.0 / 25.0


def is_zlib(data: bytes) -> bool:
    return len(data) >= 2 and data[:2] in ZLIB_MAGICS


def decode_bytes(data: bytes) -> bytes:
    return zlib.decompress(data) if is_zlib(data) else data


def decode_text(data: bytes) -> str:
    raw = decode_bytes(data)
    if raw.startswith(b"\xef\xbb\xbf"):
        raw = raw[3:]
    return raw.decode("utf-8")


OBFUSCATION_PREFIX = b"\x00\x03\x5e\x5f\x5e"
OBFUSCATED_BYTE = 16


def deobfuscate(data: bytes) -> bytes:
    """Strip the resource obfuscation the PC build applies to some assets.

    1177 files under ``Resource/image`` are obfuscated, and the scheme is two
    steps, not one: a five byte prefix (``00 03 5E 5F 5E``) is put in front of
    the payload, and the byte at offset 16 of the payload is bitwise
    complemented. Undoing only the prefix leaves a file no decoder accepts.

    Offset 16 is format-agnostic — the packer does not look at what it is
    wrapping — so the damage lands somewhere different in each format and
    looks like a different bug every time:

    * PNG (1104 files): the high byte of the IHDR width, ``00`` becoming
      ``FF``, which fails the IHDR CRC.
    * CWS/SWF (63 files): six bytes into the deflate stream, which fails to
      inflate at all ("invalid code lengths set").
    * JPEG (2 files): the high byte of the EXIF IFD offset.
    * ZIP (8 files): a byte of the local header CRC, which readers ignore in
      favour of the central directory, so these happened to survive.

    Complementing that one byte back is exact rather than a heuristic: every
    one of the 1104 PNGs then passes its IHDR CRC and every one of the 63 SWFs
    then inflates, while none of the 16929 unobfuscated PNGs in the dump is
    damaged in the first place. The prefix is therefore the whole signal, and
    anything without it is returned untouched.
    """
    if not data.startswith(OBFUSCATION_PREFIX):
        return data
    body = bytearray(data[len(OBFUSCATION_PREFIX):])
    if len(body) > OBFUSCATED_BYTE:
        body[OBFUSCATED_BYTE] ^= 0xFF
    return bytes(body)


def write_asset(dest, data: bytes) -> int:
    """Write one packed asset, undoing the PC build's obfuscation on the way.

    Every packer that copies bytes out of the Ok archives into StreamingAssets
    must go through here. Writing ``dest.write_bytes(zf.read(...))`` directly
    ships files the client cannot decode, and the packers are separate modules,
    so the only way that stays fixed is for them to share one writer.

    Returns the number of bytes written, which is what the callers report.
    """
    clean = deobfuscate(data)
    dest.parent.mkdir(parents=True, exist_ok=True)
    dest.write_bytes(clean)
    return len(clean)


def load_xml(data: bytes) -> ET.Element:
    text = decode_text(data).lstrip("\ufeff\0")
    idx = text.find("<")
    if idx < 0:
        raise ValueError("no xml")
    return ET.fromstring(text[idx:])


def parse_result_table(root: ET.Element) -> List[Dict[str, str]]:
    return parse_nested_items(root)


class Amf3Error(ValueError):
    """The bytes are not the AMF3 object a .ui bundle wraps."""


class _Amf3Bundle:
    """The slice of AMF3 a Morn ``.ui`` bundle uses.

    One dynamic object: string keys are view paths, values are XML markup.
    Markers outside that slice are rejected rather than guessed at.
    """

    OBJECT, XML, XML_DOC, STRING, INTEGER, DOUBLE = 10, 11, 7, 6, 4, 5
    UNDEFINED, NULL, FALSE, TRUE = 0, 1, 2, 3

    def __init__(self, data: bytes):
        self.data = data
        self.pos = 0
        self.strings: List[str] = []

    def _byte(self) -> int:
        if self.pos >= len(self.data):
            raise Amf3Error("AMF3 stream ended early")
        value = self.data[self.pos]
        self.pos += 1
        return value

    def _u29(self) -> int:
        """Variable-length integer: up to three continuation bytes, then a full byte."""
        value = 0
        for _ in range(3):
            byte = self._byte()
            value = (value << 7) | (byte & 0x7F)
            if not byte & 0x80:
                return value
        return (value << 8) | self._byte()

    def _string(self) -> str:
        head = self._u29()
        if not head & 1:
            return self.strings[head >> 1]
        length = head >> 1
        text = self.data[self.pos : self.pos + length].decode("utf-8")
        self.pos += length
        # The empty string is never table-referenced; adding it would shift
        # every later reference by one.
        if text:
            self.strings.append(text)
        return text

    def _value(self):
        marker = self._byte()
        if marker in (self.UNDEFINED, self.NULL):
            return None
        if marker == self.FALSE:
            return False
        if marker == self.TRUE:
            return True
        if marker == self.INTEGER:
            n = self._u29()
            return n - 0x20000000 if n & 0x10000000 else n
        if marker == self.DOUBLE:
            value = struct.unpack_from(">d", self.data, self.pos)[0]
            self.pos += 8
            return value
        if marker == self.STRING:
            return self._string()
        if marker in (self.XML, self.XML_DOC):
            head = self._u29()
            if not head & 1:
                raise Amf3Error("xml references are not supported")
            length = head >> 1
            text = self.data[self.pos : self.pos + length].decode("utf-8")
            self.pos += length
            return text
        raise Amf3Error(f"unsupported AMF3 marker 0x{marker:02x}")

    def views(self) -> List[Tuple[str, str]]:
        if self._byte() != self.OBJECT:
            raise Amf3Error("a .ui bundle starts with an AMF3 object")
        traits = self._u29()
        if not traits & 1 or not traits & 2 or traits & 4:
            raise Amf3Error("object/trait references and externalizables are not bundles")
        dynamic = bool(traits & 8)
        sealed_count = traits >> 4

        self._string()  # class name, always empty here
        names = [self._string() for _ in range(sealed_count)]
        out: List[Tuple[str, str]] = []
        for name in names:
            value = self._value()
            if isinstance(value, str):
                out.append((name, value))
        if dynamic:
            while True:
                key = self._string()
                if not key:
                    break
                value = self._value()
                if isinstance(value, str):
                    out.append((key, value))
        return out


def parse_morn_views(data: bytes) -> List[Tuple[str, int, int]]:
    """Read a Morn ``.ui`` bundle: (view path, width, height) in stored order.

    This reads the AMF3 container rather than scanning the decoded bytes for
    ``<View>`` … ``</View>`` pairs. The scan was wrong three ways on the real
    data: a self-closing ``<View …/>`` has no closing tag so the scan ran past
    it and produced a chunk with two roots and raw length bytes between them
    (not well-formed, so parsing threw); names walked backwards from ``.xml``
    over letters/digits/``_/-.`` and so swallowed the AMF3 length prefix
    whenever that byte was one of those, which hit 580 of the 950 shipped
    views; and a view holding a nested ``<View>`` closed early.
    """
    views: List[Tuple[str, int, int]] = []
    for name, markup in _Amf3Bundle(decode_bytes(data)).views():
        try:
            el = ET.fromstring(markup)
        except ET.ParseError:
            # One unreadable view must not cost the caller the whole bundle.
            continue
        views.append((name, int(el.get("width", 0)), int(el.get("height", 0))))
    return views


@dataclass
class MapCollision:
    width: int
    height: int
    stride: int
    bits: bytes

    @staticmethod
    def stride_for(width: int) -> int:
        """Bytes per packed row.

        Not ``ceil(width / 8)``: the original writer always emits one extra byte,
        so a width that is an exact multiple of eight still gets a trailing byte
        (2000 px -> 251 bytes, not 250). Checked against every mask in the dump --
        ``(width >> 3) + 1`` matches the file size on all 4685 of them while
        ``ceil(width / 8)`` matches only 3755.
        """
        return (width >> 3) + 1

    @classmethod
    def load(cls, data: bytes) -> "MapCollision":
        width, height = struct.unpack_from("<ii", data, 0)
        bits = data[8:]
        stride = cls.stride_for(width)
        expected = stride * height
        if len(bits) != expected:
            raise ValueError(
                f"map payload is {len(bits)} bytes, expected {expected} "
                f"for {width}x{height} (stride {stride})"
            )
        return cls(width, height, stride, bits)

    def is_solid(self, x: int, y: int) -> bool:
        if x < 0 or y < 0 or x >= self.width or y >= self.height:
            return False
        index = y * self.stride + (x >> 3)
        mask = 0x80 >> (x & 7)
        return (self.bits[index] & mask) != 0

    def solid_count(self, step: int = 8) -> int:
        n = 0
        for y in range(0, self.height, step):
            for x in range(0, self.width, step):
                if self.is_solid(x, y):
                    n += 1
        return n


@dataclass
class Projectile:
    x: float
    y: float
    vx: float
    vy: float
    t: float = 0.0
    alive: bool = True


def launch(x: float, y: float, angle_deg: float, power: float, facing: int = 1, speed_scale: float = 1.0) -> Projectile:
    p = max(1.0, min(100.0, power))
    rad = math.radians(angle_deg)
    speed = p * speed_scale
    direction = 1 if facing >= 0 else -1
    return Projectile(x, y, math.cos(rad) * speed * direction, math.sin(rad) * speed)


def step(
    p: Projectile,
    wind: float,
    dt: float = FRAME_DT,
    gravity: float = 0.7,
    wind_scale: float = 0.04,
    gravity_factor: float = 1.0,
    wind_factor: float = 1.0,
) -> Projectile:
    """One PC 40ms frame. Velocities are pixels/frame (game.logic.dll Physics)."""
    if not p.alive:
        return p
    return Projectile(
        p.x + p.vx,
        p.y + p.vy,
        p.vx + wind * wind_scale * wind_factor,
        p.vy - gravity * gravity_factor,
        p.t + FRAME_DT,
        True,
    )


class Row(Dict[str, str]):
    """One row's attributes, with its repeated child elements kept aside.

    A row is a flat attribute map, which cannot hold a child element that appears
    more than once — and 1971 such children sit in 13 of the shipped
    ``Request/*.xml`` files. ``QuestList.xml`` alone carries 1067 ``Item_Good``
    (the quest's item rewards) and 317 ``Item_Condiction``. The flat map kept the
    first of each and stored its *text*, which is empty because the data is in the
    attributes, so ``row["Item_Good"]`` read ``""`` with no way to reach the rest.

    This subclasses ``dict``, so every existing caller keeps working unchanged and
    ``row.children["Item_Good"]`` reaches what the map cannot hold.
    """

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.children: Dict[str, List["Row"]] = {}


def _row_from_element(el: ET.Element) -> Row:
    row = Row(el.attrib)
    for nested in list(el):
        row.setdefault(nested.tag, nested.text or "")
        if nested.attrib:
            # A text-only child is already in the map under its own tag.
            row.children.setdefault(nested.tag, []).append(_row_from_element(nested))
    return row


def parse_nested_items(root: ET.Element) -> List[Row]:
    rows: List[Row] = []
    for child in list(root):
        if len(list(child)) and child.attrib == {}:
            for nested in list(child):
                row = _row_from_element(nested)
                if row:
                    rows.append(row)
            continue
        row = _row_from_element(child)
        if row:
            rows.append(row)
    return rows


def fly_until_map(p: Projectile, wind: float, m: MapCollision, max_time: float = 12.0) -> Projectile:
    while p.alive and p.t < max_time:
        nxt = step(p, wind)
        mx, my = int(round(nxt.x)), m.height - int(round(nxt.y))
        if nxt.x < -40 or nxt.x > m.width + 40 or nxt.y < -40:
            nxt.alive = False
            return nxt
        if m.is_solid(mx, my):
            nxt.alive = False
            return nxt
        p = nxt
    p.alive = False
    return p
