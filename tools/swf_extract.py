#!/usr/bin/env python3
"""Extract bitmap tags from PC Flash SWF (living / bullet / blastout).

SWF does not run in Unity. DefineBitsJPEG3 payloads are already JPEG;
DefineBitsLossless2 is decoded to PNG. Used at pack time so the phone
loads PNG/JPEG instead of a Flash player.
"""

from __future__ import annotations

import struct
import zlib
from pathlib import Path
from typing import Iterator

JPEG_SOI = b"\xff\xd8"
PNG_SIG = b"\x89PNG"


def swf_body(data: bytes) -> bytes:
    if len(data) < 8:
        raise ValueError("too short")
    sig = data[:3]
    if sig == b"CWS":
        return zlib.decompress(data[8:])
    if sig == b"FWS":
        return data[8:]
    if sig == b"ZWS":
        raise ValueError("LZMA SWF")
    raise ValueError("not swf")


def iter_tags(body: bytes) -> Iterator[tuple[int, bytes]]:
    nbits = body[0] >> 3
    i = (5 + nbits * 4 + 7) // 8 + 4
    n = len(body)
    while i + 2 <= n:
        rec = struct.unpack_from("<H", body, i)[0]
        i += 2
        code = rec >> 6
        ln = rec & 0x3F
        if ln == 0x3F:
            if i + 4 > n:
                break
            ln = struct.unpack_from("<I", body, i)[0]
            i += 4
        payload = body[i : i + ln]
        i += ln
        yield code, payload
        if code == 0:
            break


def _png(w: int, h: int, rgba: bytes) -> bytes:
    def chunk(tag: bytes, data: bytes) -> bytes:
        crc = zlib.crc32(tag + data) & 0xFFFFFFFF
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", crc)

    rows = b"".join(b"\x00" + rgba[y * w * 4 : (y + 1) * w * 4] for y in range(h))
    ihdr = struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0)
    return PNG_SIG + b"\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", zlib.compress(rows, 9)) + chunk(b"IEND", b"")


def jpeg3_bytes(payload: bytes) -> bytes | None:
    if len(payload) < 6:
        return None
    _cid, off = struct.unpack_from("<HI", payload, 0)
    img = payload[6 : 6 + off]
    p = img.find(JPEG_SOI)
    if p < 0:
        return None
    return img[p:]


def lossless2_png(payload: bytes) -> bytes | None:
    if len(payload) < 8:
        return None
    _cid, fmt, w, h = struct.unpack_from("<HBHH", payload, 0)
    if w <= 0 or h <= 0 or w > 4096 or h > 4096:
        return None
    if fmt == 5:
        try:
            raw = zlib.decompress(payload[7:])
        except zlib.error:
            return None
        if len(raw) < w * h * 4:
            return None
        rgba = bytearray(w * h * 4)
        for i in range(w * h):
            a, r, g, b = raw[i * 4 : i * 4 + 4]
            rgba[i * 4 : i * 4 + 4] = bytes((r, g, b, a))
        return _png(w, h, bytes(rgba))
    if fmt == 3:
        color_n = payload[7] + 1
        try:
            raw = zlib.decompress(payload[8:])
        except zlib.error:
            return None
        table = raw[: color_n * 4]
        stride = (w + 3) & ~3
        idxs = raw[color_n * 4 :]
        rgba = bytearray(w * h * 4)
        for y in range(h):
            row = idxs[y * stride : y * stride + w]
            for x, pix in enumerate(row):
                o = pix * 4
                if o + 3 >= len(table):
                    continue
                r, g, b, a = table[o : o + 4]
                p = (y * w + x) * 4
                rgba[p : p + 4] = bytes((r, g, b, a))
        return _png(w, h, bytes(rgba))
    return None


def extract_images(swf: bytes) -> list[tuple[str, bytes]]:
    body = swf_body(swf)
    out: list[tuple[str, bytes]] = []
    for code, payload in iter_tags(body):
        if code == 35:
            jpg = jpeg3_bytes(payload)
            if jpg:
                out.append((".jpg", jpg))
        elif code == 36:
            png = lossless2_png(payload)
            if png:
                out.append((".png", png))
        elif code == 21:
            # DefineBitsJPEG2: UI16 id + JPEG
            if len(payload) > 4:
                img = payload[2:]
                p = img.find(JPEG_SOI)
                if p >= 0:
                    out.append((".jpg", img[p:]))
    return out


def largest_image(swf: bytes) -> tuple[str, bytes] | None:
    imgs = extract_images(swf)
    if not imgs:
        return None
    return max(imgs, key=lambda kv: len(kv[1]))


def write_largest(swf: bytes, dest_no_suffix: Path) -> Path | None:
    hit = largest_image(swf)
    if hit is None:
        return None
    ext, blob = hit
    dest = dest_no_suffix.with_suffix(ext)
    dest.parent.mkdir(parents=True, exist_ok=True)
    dest.write_bytes(blob)
    return dest
