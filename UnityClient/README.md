# Unity client (Android + iOS)

Open **this folder** (`UnityClient/`) in Unity **2021.3 LTS**.

## Run
1. Install Unity 2021.3 with **Android Build Support** (SDK/NDK/OpenJDK) and **iOS Build Support**.
2. File → Open Project → `Gun_mobile/UnityClient`.
3. Press Play. `GameApp` boots from `Assets/Scenes/Boot.unity`.
4. Login → Hall (all PC modules) → 开战 → pick a packed map → vs bot.

## Build
Menu **GunMobile**:
- Apply Android + iOS Player Settings
- Build Android APK → `UnityClient/Builds/GunMobile.apk`
- Build iOS Xcode Project → `UnityClient/Builds/ios` (archive in Xcode on macOS)

Bundle id: `com.gunmobile.client`  
Orientation: landscape  
Android min SDK 23 / ARM64 IL2CPP  
iOS 12+ IL2CPP

## PC data
`Assets/StreamingAssets/PcData` is unpacked from the Ok dump (XML tables, hall/game atlases, 6 maps, bombs). Full ~2GB `Resource/image` stays in `legacy/releases/Ok` and can be unpacked later with `tools/extract_legacy.py`.
