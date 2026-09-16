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
PNG_MAGIC = b"\x89PNG\r\n\x1a\x08"[:4] + b"\r\n\x1a\n"
PNG_WIDTH_HIGH_BYTE = 16


def deobfuscate(data: bytes) -> bytes:
    """Strip the resource obfuscation the PC build applies to some assets.

    1177 files under ``Resource/image`` carry a five byte prefix
    (``00 03 5E 5F 5E``) ahead of the real payload, and the PNGs among them are
    additionally damaged: the high byte of the IHDR width is overwritten with
    ``0xFF``. Stripping the prefix alone is not enough — the IHDR CRC still fails
    and the image will not decode. Resetting that byte to zero restores a valid
    IHDR CRC on every affected file, so the repair is exact, not a heuristic.

    Anything that is not obfuscated is returned untouched.
    """
    if not data.startswith(OBFUSCATION_PREFIX):
        return data
    body = data[len(OBFUSCATION_PREFIX):]
    if body.startswith(PNG_MAGIC) and len(body) > PNG_WIDTH_HIGH_BYTE and body[PNG_WIDTH_HIGH_BYTE] == 0xFF:
        body = body[:PNG_WIDTH_HIGH_BYTE] + b"\x00" + body[PNG_WIDTH_HIGH_BYTE + 1:]
    return body


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


def parse_morn_views(data: bytes) -> List[Tuple[str, int, int]]:
    text = decode_bytes(data).decode("utf-8", errors="replace")
    views = []
    search = 0
    while True:
        start = text.find("<View", search)
        if start < 0:
            break
        end = text.find("</View>", start)
        if end < 0:
            break
        end += len("</View>")
        xml = text[start:end]
        el = ET.fromstring(xml)
        name = "View"
        xml_ext = text.rfind(".xml", 0, start)
        if xml_ext >= 0:
            s = xml_ext
            while s > 0 and (text[s - 1].isalnum() or text[s - 1] in "_/.-"):
                s -= 1
            name = text[s : xml_ext + 4]
        views.append((name, int(el.get("width", 0)), int(el.get("height", 0))))
        search = end
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


def parse_nested_items(root: ET.Element) -> List[Dict[str, str]]:
    rows: List[Dict[str, str]] = []
    for child in list(root):
        if len(list(child)) and child.attrib == {}:
            for nested in list(child):
                row = dict(nested.attrib)
                for inner in list(nested):
                    row.setdefault(inner.tag, inner.text or "")
                if row:
                    rows.append(row)
            continue
        row = dict(child.attrib)
        for nested in list(child):
            row.setdefault(nested.tag, nested.text or "")
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
