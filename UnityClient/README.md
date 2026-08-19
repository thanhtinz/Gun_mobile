# Unity client (Android + iOS)

Open **this folder** (`UnityClient/`) in Unity **6.3 LTS** (`6000.3.22f1`).

## Run
1. Install Unity 6.3 LTS with **Android Build Support** (SDK/NDK/OpenJDK) and **iOS Build Support**.
2. File → Open Project → `Gun_mobile/UnityClient`.
3. Press Play. `GameApp` boots from `Assets/Scenes/Boot.unity`.
4. Login dùng `Flash/1.png`, Hall dùng Starling `hall_scene`, Room dùng `samll_map.png` của từng map. Không vẽ UI mới.
   - **开战** — mọi map packed vs bot
   - **副本 / NPC 狩猎 / 迷宫 / 世界BOSS** — PVE, stat NPC từ `NPCInfoList` (Blood khổng lồ được scale cho mobile)
   - **商城 / VIP / 抽奖 / 拍卖** — mua, gacha, bán
   - **背包 / 铁匠铺** — mặc đồ, cường hóa +0…+15
   - **宠物 / 卡片 / 称号 / 图腾 / 坐骑 / 精灵 / 战魂 / 修炼** — cộng stat
   - **任务 / 签到 / 弹王盟约 / 农场 / 公会 / 排行 / 好友 / 邮件 / 聊天**
   - **炮弹 / 炸弹配置** — chọn ball từ BallList / bombconfig
   - **角色 / 设置**
5. Battle: left walk, right aim+release, trajectory dots, living948 + SWF living/bomb JPEG, crater PNG. Physics is the Fight `game.logic.dll` 25fps loop (gravity **0.7 px/frame**). End screen + quest rewards on win.
6. **LAN 2 điện thoại:** Hall → 开战 → máy host **开房 Fight**, máy kia gõ IP → **加入**, host chọn map. Socket native cổng 4396/1910 (không phải SQL Road.Service).

## Build on GitHub (APK + IPA)

See `docs/GITHUB_BUILD.md`. After `UNITY_LICENSE` is in repo secrets, run **Actions → Build APK and IPA**.

## Build locally
Menu **GunMobile**:
- Unpack Full PC Dump
- Pack StreamingAssets PcData
- Apply Android + iOS Player Settings
- Build Android APK → `UnityClient/Builds/GunMobile.apk`
- Build iOS Xcode Project → `UnityClient/Builds/ios` (archive in Xcode on macOS)

Bundle id: `com.gunmobile.client`  
Orientation: landscape  
Android min SDK 23 / target 35 / ARM64 IL2CPP  
iOS 13+ IL2CPP

This environment has no Unity Editor, so the APK/IPA must be built on a machine with Unity 6.3 LTS, or via GitHub Actions.

## PC data
`Assets/StreamingAssets/PcData` is packed from the Ok dump (templates, shop, quests, balls, **all maps with fore.png+fore.map**, bombs, hall/game atlases).

Full extra art (`Resource/image/equip`, etc.) is not all shipped in the APK. Unpack:

```bash
python3 tools/unpack_pc_dump.py
```

Unity ExtraRoots then sees `legacy/unpacked/` so character icons and extra maps resolve in the Editor. Copy that tree into `persistentDataPath/PcData` on a device for the same layout.
