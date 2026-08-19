# Gun Mobile

Repo chứa dump PC (弹弹堂 / DDTank-style) từ release [Ok](https://github.com/thanhtinz/Gun_mobile/releases/tag/Ok) và package Unity để port mobile.

## Có gì trong repo

- `legacy/releases/Ok/` — 3 zip gốc (Git LFS), checksum trong `SHA256SUMS.txt`
- `legacy/data/` — XML/UI đã giải nén zlib (Flash config, Request tables, Morn, Starling atlas XML)
- `UnityClient/Packages/com.gunmobile.port/` — helper đọc res, UI phone, logic đạn/turn
- `docs/UNITY_MOBILE_PORT.md` — kế hoạch port từng phase
- `tools/extract_legacy.py` / `tools/test_port_helpers.py`

## Unity

1. Unity 2021.3 LTS, 2D.
2. `Packages/manifest.json`:

```json
"com.gunmobile.port": "file:../UnityClient/Packages/com.gunmobile.port"
```

hoặc copy folder package vào `Packages/`.

3. Copy `Samples~/StreamingAssets/*` → `Assets/StreamingAssets/`.
4. Unzip `Archive.3.zip` `Resource/` + `Archive.2.zip` `Flash/` vào `persistentDataPath` trên máy/device.
5. Scene trống + component `GunMobile.GunMobileBootstrap`.

## Kiểm tra dump + helper

```bash
python3 tools/test_port_helpers.py
```
