# Unity client (Android + iOS)

Open **this folder** (`UnityClient/`) in Unity **2021.3 LTS**.

## Run
1. Install Unity 2021.3 with **Android Build Support** (SDK/NDK/OpenJDK) and **iOS Build Support**.
2. File → Open Project → `Gun_mobile/UnityClient`.
3. Press Play. `GameApp` boots from `Assets/Scenes/Boot.unity`.
4. Login → Hall:
   - **开战 / 副本** — every packed map vs bot (PC `fore.map` + art)
   - **商城** — buy from `ShopItemList` (gold / 点券)
   - **背包** — equip items, stats from `TemplateAlllist`
   - **任务** — accept / claim `QuestList`
   - **角色 / 签到 / 设置**
5. Battle: left walk, right aim+release. Physics is the Fight `game.logic.dll` 25fps loop (gravity **0.7 px/frame**).

## Build
Menu **GunMobile**:
- Unpack Full PC Dump
- Pack StreamingAssets PcData
- Apply Android + iOS Player Settings
- Build Android APK → `UnityClient/Builds/GunMobile.apk`
- Build iOS Xcode Project → `UnityClient/Builds/ios` (archive in Xcode on macOS)

Bundle id: `com.gunmobile.client`  
Orientation: landscape  
Android min SDK 23 / ARM64 IL2CPP  
iOS 12+ IL2CPP

This environment has no Unity Editor, so the APK/IPA must be built on a machine with Unity 2021.3.

## PC data
`Assets/StreamingAssets/PcData` is packed from the Ok dump (templates, shop, quests, balls, **all maps with fore.png+fore.map**, bombs, hall/game atlases).

Full extra art (`Resource/image/equip`, etc.) is not all shipped in the APK. Unpack:

```bash
python3 tools/unpack_pc_dump.py
```

Unity ExtraRoots then sees `legacy/unpacked/` so character icons and extra maps resolve in the Editor. Copy that tree into `persistentDataPath/PcData` on a device for the same layout.
