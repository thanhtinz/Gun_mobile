#!/usr/bin/env python3
"""Pull living + bullet bitmaps out of Archive.3.zip SWF into StreamingAssets."""

from __future__ import annotations

import json
import sys
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))
from swf_extract import write_largest  # noqa: E402

Z3 = ROOT / "legacy" / "releases" / "Ok" / "Archive.3.zip"
OUT = ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"


def _stem(name: str) -> str:
    return Path(name).stem


def pack() -> dict:
    living_dir = OUT / "Resource" / "image" / "game" / "living" / "extracted"
    bullet_dir = OUT / "Resource" / "image" / "bomb" / "bullet" / "extracted"
    blast_dir = OUT / "Resource" / "image" / "bomb" / "blastout" / "extracted"
    living_dir.mkdir(parents=True, exist_ok=True)
    bullet_dir.mkdir(parents=True, exist_ok=True)
    blast_dir.mkdir(parents=True, exist_ok=True)

    index = {"living": {}, "bullet": {}, "blastout": {}}
    n_ok = n_fail = 0
    with zipfile.ZipFile(Z3) as zf:
        names = [n.replace("\\", "/") for n in zf.namelist() if "__MACOSX" not in n]
        livings = [n for n in names if n.startswith("Resource/image/game/living/") and n.endswith(".swf")]
        bullets = [n for n in names if n.startswith("Resource/image/bomb/bullet/") and n.endswith(".swf")]
        blasts = [n for n in names if n.startswith("Resource/image/bomb/blastout/") and n.endswith(".swf")]
        want_ids = set()
        try:
            from port_helpers import load_xml, parse_result_table

            ball_xml = ROOT / "legacy" / "data" / "Request" / "BallList.xml"
            if ball_xml.exists():
                for row in parse_result_table(load_xml(ball_xml.read_bytes())):
                    pid = (row.get("BombPartical") or "").strip()
                    if pid.isdigit() and 0 < int(pid) <= 80:
                        want_ids.add(int(pid))
        except Exception:
            want_ids = set()
        want_ids.update((1, 3, 4, 6, 7, 8, 9, 10, 65))
        prefer = []
        for bid in sorted(want_ids):
            prefer.extend([n for n in blasts if n.endswith(f"/blastout{bid}.swf")])
        extra = [n for n in blasts if n not in prefer][:8]
        selected_blasts = prefer + extra

        for group, paths, dest, key in (
            ("living", livings, living_dir, "living"),
            ("bullet", bullets, bullet_dir, "bullet"),
            ("blastout", selected_blasts, blast_dir, "blastout"),
        ):
            for path in paths:
                dest_stem = dest / _stem(path)
                existing = None
                for ext in (".png", ".jpg"):
                    cand = dest_stem.with_suffix(ext)
                    if cand.exists():
                        existing = cand
                        break
                if existing is not None:
                    rel = existing.relative_to(OUT).as_posix()
                    index[key][_stem(path).lower()] = rel
                    n_ok += 1
                    continue
                try:
                    written = write_largest(zf.read(path), dest / _stem(path))
                except Exception as e:
                    print("fail", path, e)
                    n_fail += 1
                    continue
                if written is None:
                    n_fail += 1
                    continue
                rel = written.relative_to(OUT).as_posix()
                index[key][_stem(path).lower()] = rel
                n_ok += 1

    (OUT / "Resource" / "image" / "swf_extract_index.json").write_text(
        json.dumps(index, indent=2), encoding="utf-8"
    )

    idx_path = OUT / "content_index.json"
    if idx_path.exists():
        content = json.loads(idx_path.read_text(encoding="utf-8"))
        files = content.get("files") or []
        have = set(files)
        extra_files = [index[k][s] for k in index for s in index[k]]
        extra_files.append("Resource/image/swf_extract_index.json")
        for rel in extra_files:
            if rel not in have:
                files.append(rel)
        files.sort()
        content["files"] = files
        content.setdefault("counts", {})["swfExtracted"] = n_ok
        idx_path.write_text(json.dumps(content, indent=2), encoding="utf-8")

    return {"ok": n_ok, "fail": n_fail, "living": len(index["living"]), "bullet": len(index["bullet"]), "blast": len(index["blastout"])}


if __name__ == "__main__":
    print(json.dumps(pack(), indent=2))
