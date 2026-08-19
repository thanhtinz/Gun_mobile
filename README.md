# Gun Mobile

PC dump (弹弹堂 / DDTank) from release [Ok](https://github.com/thanhtinz/Gun_mobile/releases/tag/Ok) plus a Unity **2021.3** client targeting **Android and iOS**.

## Build APK / IPA trên GitHub

Workflow **Build APK and IPA** (GameCI, Unity 2021.3.33f1):

1. Thêm secrets `UNITY_LICENSE` + `UNITY_EMAIL` + `UNITY_PASSWORD` (xem `docs/GITHUB_BUILD.md`)
2. Actions → **Build APK and IPA** → Run workflow
3. Tải artifact `GunMobile.apk`. IPA cần thêm cert Apple (cùng doc)

Hoặc mở folder `UnityClient/` trong Unity Hub rồi menu **GunMobile → Build Android APK** / **Build iOS Xcode Project**.

## Repo layout

- `legacy/releases/Ok/` — original 3 zip archives (Git LFS)
- `legacy/data/` — decompressed Flash/Request XML
- `legacy/unpacked/` — full PNG/map unpack (local only)
- `UnityClient/` — Unity project (landscape, IL2CPP, `com.gunmobile.client`)
- `UnityClient/Assets/StreamingAssets/PcData` — packed PC tables, hall/game art, **all playable maps**, bombs
- `UnityClient/Assets/Scripts/Client` — login, hall, shop/bag/quest/character, room, battle vs bot
- `UnityClient/Packages/com.gunmobile.port` — zlib XML, map collision, PC 25fps physics, mobile HUD
- `docs/UNITY_MOBILE_PORT.md` — port notes

## Test

```bash
python3 tools/test_port_helpers.py
python3 tools/unpack_pc_dump.py   # optional, ~maps + equip PNG
python3 tools/pack_mobile_content.py
```
