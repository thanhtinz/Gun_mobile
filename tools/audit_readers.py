#!/usr/bin/env python3
"""Re-run the reader audit against an extracted PC dump.

Every bug fixed on this branch was found the same way: reimplement what the
shipped reader does, run it over every matching file in the dump beside a
parser written from the format spec, and count the disagreements. The dump is
too large to commit (4.2GB extracted, and the archives are LFS pointers), so
the checks live here instead of the findings.

    python3 tools/audit_readers.py <extracted-dump-dir>

Get a dump with ``python3 tools/bootstrap_pc_assets.py`` and unzip the
archives, or point this at any directory holding ``Resource/`` and ``Flash/``.

Exits non-zero if an invariant a fix depends on no longer holds.
"""

from __future__ import annotations

import collections
import struct
import sys
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

from port_helpers import MapCollision, deobfuscate, load_xml, parse_result_table  # noqa: E402

PREFIX = b"\x00\x03\x5e\x5f\x5e"


def walk(root, pattern="*"):
    """Files matching *pattern*, minus the junk a macOS zip carries.

    An archive zipped on macOS ships an AppleDouble shadow for every entry —
    ``__MACOSX/Service/Road/Bomb/._11295.bomb`` beside the real one, 224 bytes
    of resource-fork metadata. Plain ``unzip`` writes them out, so a naive
    rglob sees two "masks" for every real one and half of them fail every
    check. That is a defect in this script, not in the data: it reported
    9378/18756 on a freshly unzipped dump and read as a regression in the
    stride fix. The repo's own extract_legacy.py already drops these.
    """
    for path in sorted(root.rglob(pattern)):
        if not path.is_file():
            continue
        if "__MACOSX" in path.parts or path.name.startswith("._"):
            continue
        yield path


class Audit:
    def __init__(self) -> None:
        self.failures: list[str] = []

    def check(self, ok: bool, label: str, detail: str) -> None:
        print(f"  [{'ok ' if ok else 'FAIL'}] {label}: {detail}")
        if not ok:
            self.failures.append(label)


def obfuscation(dump: Path, audit: Audit) -> None:
    """The prefix implies byte 16 is complemented, in every format."""
    print("\nobfuscation — prefix plus a complemented byte at offset 16")
    kinds = collections.Counter()
    repaired = collections.Counter()
    for path in walk(dump):
        try:
            raw = path.read_bytes()
        except OSError:
            continue
        if not raw.startswith(PREFIX):
            continue
        body = deobfuscate(raw)
        if body[:8] == b"\x89PNG\r\n\x1a\n":
            kinds["png"] += 1
            crc = struct.unpack_from(">I", body, 29)[0]
            repaired["png"] += zlib.crc32(body[12:29]) == crc
        elif body[:3] == b"CWS":
            kinds["cws"] += 1
            try:
                zlib.decompressobj(-15).decompress(body[10:])
                repaired["cws"] += 1
            except zlib.error:
                pass

    audit.check(
        kinds["png"] > 0 and repaired["png"] == kinds["png"],
        "every obfuscated PNG passes its IHDR CRC after the repair",
        f"{repaired['png']}/{kinds['png']}",
    )
    audit.check(
        kinds["cws"] > 0 and repaired["cws"] == kinds["cws"],
        "every obfuscated SWF inflates after the repair",
        f"{repaired['cws']}/{kinds['cws']}",
    )

    # The prefix must be the whole signal: untouched PNGs are never damaged.
    clean = damaged = 0
    for path in walk(dump, "*.png"):
        raw = path.read_bytes()
        if raw.startswith(PREFIX) or raw[:8] != b"\x89PNG\r\n\x1a\n":
            continue
        clean += 1
        if zlib.crc32(raw[12:29]) != struct.unpack_from(">I", raw, 29)[0]:
            damaged += 1
    audit.check(
        clean > 0 and damaged == 0,
        "no unobfuscated PNG is damaged, so the prefix is the whole signal",
        f"{clean} checked, {damaged} damaged",
    )


def map_stride(dump: Path, audit: Audit) -> None:
    """Stride is (width >> 3) + 1, not ceil(width / 8)."""
    print("\nmap collision — one spare byte per packed row")
    shipped = ceil_matches = stride_matches = skipped = 0
    for path in list(walk(dump, "*.map")) + list(walk(dump, "*.bomb")):
        raw = path.read_bytes()
        payload = len(raw) - 8
        if payload <= 0:
            skipped += 1
            continue
        width, height = struct.unpack_from("<ii", raw, 0)
        # Classify by content, not by extension: NewPanel ships
        # jquery-1.11.3.min.map, a JSON source map, which is not terrain.
        # The test here is independent of which stride formula is right —
        # plausible dimensions, and rows of equal length — so it cannot
        # quietly define away a mask the formula would fail on.
        if not (0 < width <= 20000 and 0 < height <= 20000) or payload % height:
            skipped += 1
            continue
        shipped += 1
        ceil_matches += payload == ((width + 7) // 8) * height
        stride_matches += payload == MapCollision.stride_for(width) * height

    if skipped:
        print(f"  [note] {skipped} file(s) named .map/.bomb are not terrain masks")
    if not shipped:
        print("  (no terrain masks in this dump)")
        return
    audit.check(
        stride_matches == shipped,
        "(width >> 3) + 1 matches every mask's file size",
        f"{stride_matches}/{shipped}  (ceil(width/8) would match {ceil_matches})",
    )


def starling_atlases(dump: Path, audit: Audit) -> None:
    """Starling y is top-left; Unity wants bottom-left."""
    print("\nstarling atlases — the y flip, and the trim offsets nobody reads")
    frames = needs_flip = trimmed = off_centre = 0
    worst_flip = worst_trim = 0.0
    for xml in walk(dump, "*.xml"):
        png = xml.with_suffix(".png")
        if not png.exists():
            continue
        head = png.read_bytes()[:24]
        if head[:8] != b"\x89PNG\r\n\x1a\n":
            continue
        _, texture_height = struct.unpack_from(">II", head, 16)
        try:
            root = load_xml(xml.read_bytes())
        except Exception:
            continue
        for sub in root.iter("SubTexture"):
            y = float(sub.get("y", 0))
            h = float(sub.get("height", 0))
            frames += 1
            error = abs(texture_height - 2 * y - h)
            if error:
                needs_flip += 1
                worst_flip = max(worst_flip, error)
            if sub.get("frameWidth") is None:
                continue
            trimmed += 1
            dx = (float(sub.get("frameWidth", 0)) / 2 + float(sub.get("frameX", 0))
                  - float(sub.get("width", 0)) / 2)
            dy = (float(sub.get("frameHeight", 0)) / 2 + float(sub.get("frameY", 0)) - h / 2)
            if dx or dy:
                off_centre += 1
                worst_trim = max(worst_trim, abs(dx) + abs(dy))

    if not frames:
        print("  (no starling atlases in this dump)")
        return
    audit.check(
        needs_flip / frames > 0.9,
        "the y flip is not a no-op",
        f"{needs_flip}/{frames} frames move, worst {worst_flip:.0f}px",
    )
    print(f"  [note] trim offsets ignored by TextureAtlasParser: "
          f"{off_centre}/{trimmed} trimmed frames are off-centre, worst {worst_trim:.0f}px")


def repeated_children(dump: Path, audit: Audit) -> None:
    """A flat attribute map cannot hold a child element that repeats."""
    print("\nrequest tables — children a flat row cannot hold")
    files = reachable = 0
    per_file: collections.Counter[str] = collections.Counter()
    for xml in walk(dump, "Request/*.xml"):
        try:
            rows = parse_result_table(load_xml(xml.read_bytes()))
        except Exception:
            continue
        files += 1
        for row in rows:
            for name, kids in row.children.items():
                if len(kids) > 1:
                    per_file[xml.name] += len(kids) - 1
                reachable += len(kids)

    if not files:
        print("  (no Request/*.xml in this dump)")
        return
    audit.check(
        reachable > 0,
        "repeated children are reachable through row.children",
        f"{reachable} children across {files} files",
    )
    for name, lost in per_file.most_common(5):
        print(f"  [note] {name}: {lost} would be dropped by a flat map alone")


def main() -> int:
    if len(sys.argv) != 2:
        print(__doc__)
        return 2
    dump = Path(sys.argv[1])
    if not dump.is_dir():
        print(f"not a directory: {dump}", file=sys.stderr)
        return 2

    print(f"auditing {dump}")
    audit = Audit()
    obfuscation(dump, audit)
    map_stride(dump, audit)
    starling_atlases(dump, audit)
    repeated_children(dump, audit)

    print()
    if audit.failures:
        print(f"{len(audit.failures)} invariant(s) no longer hold:")
        for name in audit.failures:
            print(f"  - {name}")
        return 1
    print("every invariant the fixes rest on still holds")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
