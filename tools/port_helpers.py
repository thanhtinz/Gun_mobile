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


def load_xml(data: bytes) -> ET.Element:
    text = decode_text(data).lstrip("\ufeff\0")
    idx = text.find("<")
    if idx < 0:
        raise ValueError("no xml")
    return ET.fromstring(text[idx:])


def parse_result_table(root: ET.Element) -> List[Dict[str, str]]:
    rows = []
    for child in list(root):
        row = dict(child.attrib)
        for nested in list(child):
            row.setdefault(nested.tag, nested.text or "")
        rows.append(row)
    return rows


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

    @classmethod
    def load(cls, data: bytes) -> "MapCollision":
        width, height = struct.unpack_from("<ii", data, 0)
        bits = data[8:]
        stride = (width + 7) // 8
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


def launch(x: float, y: float, angle_deg: float, power: float, facing: int = 1, speed_scale: float = 5.5) -> Projectile:
    p = max(1.0, min(100.0, power))
    rad = math.radians(angle_deg)
    speed = p * speed_scale
    direction = 1 if facing >= 0 else -1
    return Projectile(x, y, math.cos(rad) * speed * direction, math.sin(rad) * speed)


def step(p: Projectile, wind: float, dt: float = FRAME_DT, gravity: float = 175.0, wind_scale: float = 1.15) -> Projectile:
    if not p.alive:
        return p
    return Projectile(
        p.x + p.vx * dt,
        p.y + p.vy * dt,
        p.vx + wind * wind_scale * dt,
        p.vy - gravity * dt,
        p.t + dt,
        True,
    )


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
