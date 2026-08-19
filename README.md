# Gun Mobile

PC dump (弹弹堂 / DDTank) from release [Ok](https://github.com/thanhtinz/Gun_mobile/releases/tag/Ok) plus a Unity **2021.3** client targeting **Android and iOS**.

## Play / build

Open folder `UnityClient/` in Unity 2021.3 LTS (Android + iOS modules). Play `Assets/Scenes/Boot.unity`, or use menu **GunMobile → Build Android APK** / **Build iOS Xcode Project**.

Details: `UnityClient/README.md`

## Repo layout

- `legacy/releases/Ok/` — original 3 zip archives (Git LFS)
- `legacy/data/` — decompressed Flash/Request XML
- `UnityClient/` — Unity project (landscape, IL2CPP, `com.gunmobile.client`)
- `UnityClient/Assets/StreamingAssets/PcData` — packed PC tables, hall/game art, 6 maps, bombs
- `UnityClient/Assets/Scripts/Client` — login, hall (all PC modules), room, battle vs bot
- `UnityClient/Packages/com.gunmobile.port` — zlib XML, map collision, physics, mobile HUD
- `docs/UNITY_MOBILE_PORT.md` — port notes

## Test

```bash
python3 tools/test_port_helpers.py
```
