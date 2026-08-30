# Kế hoạch port Gun / 弹弹堂 PC → Unity mobile

Dump gốc nằm ở `legacy/releases/Ok/` (Release [Ok](https://github.com/thanhtinz/Gun_mobile/releases/tag/Ok)). Đây **không** phải Unity project: client Flash (Pickgliss + Morn UI + Starling), server .NET `Road` / `Fight` / `Center`, XML request, SQL Server.

Helper Unity: package `UnityClient/Packages/com.gunmobile.port`.

## Hiện trạng dump

| Archive | Nội dung |
| --- | --- |
| `Archive.zip` | Backup SQL (`Db-Tank`, `Db-Tank-All`, `Db-Membership`…), NewPanel.zip, NpcEditor |
| `Archive.2.zip` | `Flash/` (SWF, `config.xml`, UI Morn/Starling), `data/*.tank`, NewPanel, NpcEditor |
| `Archive.3.zip` | `Request/` XML + ashx, `Service/{Road,Fight,Center}`, `Resource/image`, `WebSite/` |

Luồng PC:

1. Web login (`WebSite`) → Flash loader đọc `Flash/config.xml`
2. Flash tải `Resource/image/**` và `Request/*.xml` (nhiều file zlib)
3. Hall socket tới **Road** (`IP:4396` trong exe.config)
4. Vào trận socket tới **Fight** (`battle.xml` port `1910`)

Tọa độ map: bitmap Y đi xuống. Unity 2D: Y đi lên — helper collision đã đổi trục khi test đạn.

## Mục tiêu mobile

- Unity **6.3 LTS**, **landscape** 16:9 / 20:9, Android trước, iOS sau.
- Giữ luật chơi PC: turn-based, góc + lực, gió, địa hình phá được, bomb template.
- Không nhúng Flash/SWF. Sprite PNG/atlas + logic C#.
- Server: giữ Road/Fight nếu cần online; hoặc replay offline 1-2 người cho milestone 1.

## Phase 1 — Nền (đã làm)

- [x] Import 3 zip Ok lên git (LFS)
- [x] Extract XML/UI đã giải zlib → `legacy/data/`
- [x] Helper zlib / map / atlas / Morn / đạn
- [x] Unity 6.3 LTS project Android + iOS (`UnityClient/`, bundle `com.gunmobile.client`)
- [x] Client: login → hall (mọi module PC: shop/bag/quest/pet/card/title/totem/mount/elf/farm/guild/rank/auction/vip/lottery/labyrinth/worldboss/dungeon/NPC/forge/texp/gem/mail/chat/friends) → **all packed maps** → trận vs bot **và PVE vs NPC** (PC gravity 0.7/frame)
- [x] `GameDatabase` loads TemplateAlllist / Shop / Quest / Ball / Map / NPC / Pet / Card / Title / Totem / Mount / Lottery / VIP / PVE / Spirit / Elf / Farm
- [x] SWF living/bomb → JPEG/PNG (`tools/swf_extract.py`, runtime `SwfImage`)
- [x] Phone Road/Fight TCP trên cổng **4396 / 1910** (không SQL; magic `0x7D01`) — LAN 2 máy hoặc loopback
- [x] Shop/bag icon từ `Resource/image/equip|arm|unfrightprop` (dump PC, không vẽ mới)
- [x] HUD trận `gameprop.png` + sảnh podium `hall_new_rankbg`
- [x] LAN đồng bộ đi bộ `FightWalk` (92) + mộ `game_tombAsset` + pet/title PNG PC
- [x] Multi-shot (Amount>1) + damage popup + equip layer preview + arm/equip game.png
- [x] `MobileGameServer` — thay thế Road/Fight (hall server-authoritative); battle server-authoritative (server mô phỏng đường đạn, cắt địa hình, tự tính damage)
- [x] Client wired: every screen sends PhoneMsg → server validates → ProfileData sync back
- [x] VIP/Texp/Gem/KingBless/Mail/Auction all server-notified
- [x] Server bag sync (ProfileData→Bag), room create/join, turn advancement
- [x] Auto-reconnect, damage clamping, server turn broadcast

### PhoneMsg 233–269 — nốt các bảng PC còn lại

Đợt port cuối gom hết bảng `Request/*.xml` còn dùng được thành module có server validate + màn hình client:

| Msg | Hệ thống | Bảng PC |
| --- | --- | --- |
| 233–234 | Trang viên: gieo/thu hoạch/phụ giúp, nhiệm vụ | `templatemanorlist`, `ts_manortask` |
| 235 | Thành tựu thẻ bài (cộng chỉ số vĩnh viễn) | `cardachievement` |
| 236 | Thủ hộ hạch nhân: cấp + kỹ năng | `guardcoretemplate`, `guardcoreleveltemplate` |
| 237 | Đèn lồng đố chữ | `lightriddlequest` |
| 238–239 | Công bằng chiến: học kỹ năng, quân hàm, thưởng tuần | `fairbattleskillgettemplate` (+material), `fairbattlerewardtemp`, `fairbatttleweeklyawardtemp` |
| 240 | Trang bị online 5 ô + đào mỏ | `onlinearmlevelinfo`, `onlinearmdropitem` |
| 241–243 | Phó vũ khí tiến hoá, tình lữ, thần thụ | `subweaponevolutiontemplate`, `lovelevelist`, `treetemplatelist` |
| 244–245 | Hoạt động ngày, thưởng đăng nhập | `everydayactive*`, `loginawarditemtemplate` |
| 246–247 | Vương giả chi lộ, đổi vật phẩm hoạt động | `kingofroadquestinfolist`, `activeconvertiteminfo` |
| 248–249 | Shop mini game, phế liệu | `minigameshoptemplate`, `WasteRecycle_Award` |
| 250–252 | Nuôi bộ trang bị, tinh/tôi luyện khắc ấn, rương thành trưởng | `setsbuildtemp` (+`suittemplateinfolist`), `engraverefinery/temperconfiginfo`, `loaduserbox` |
| 253–256 | Hoạt động toàn server, sưu tầm, trợ chiến | `communalactive*`, `goodscollect`, `helpgamereward` |
| 257–259 | Pet biến hình/manh thú, rune tiến giai, mốc nạp | `loadpetformdata`/`loadpetmoeproperty`/`petlevelinfo`, `runeadvancetemplatelist`, `loadchargeactivetemplate` + `chargespendreward*` + `txplayerawardtemplatelist` |
| 260–262 | Tam thanh, xúc xắc, câu cá + thẻ tháng | `threecleanpointaward`, `dicegameawarditem`, `homefishinfo`, `monthcardgoodinfo` |
| 263–265 | Trang bị nại khoái, bốc quà hoạt động/sự kiện | `TS_NaiKuaiEquip`, `activitysystemitems`(+rate), `eventrewarditemlist` |
| 266–268 | Buff bộ thẻ, tầm bảo, đột phá cấp | `cardbufflist`+`cardinfolist`, `searchgoodstemp`+`lotteryshowtemplate`, `maxleveltemplate` |
| 269 | Cường hoá theo exp (không hên xui) | `itemstrengthendata`, `loadstrengthexp` |

Bảng chỉ để tra cứu (không có msg riêng, hiện trong màn **游戏资料**): `ranktemplateall`, `ActiveList`, `mapserverlist`, `ts_versionnotice`, `loaditemscategory`, `bufftemplateinfo`, `TS_Relic_AdvanceValue`, `missionenergyprice`.

Chỗ dump thiếu dữ liệu nên server phải giả định (đã ghi chú ngay trong code):

- `lightriddlequest` không có cột đáp án → coi `Option1` là đúng, đổi bằng ServerConfig `LightRiddleAnswerIndex`.
- `onlinearmdropitem` chỉ có `Rate1..11` mà không kèm danh sách vật phẩm → quy bậc rớt ra vàng/exp thay vì bịa template id.
- `templatemanorlist` giữ format ItemTemplate: `Property2` số lượng thu, `Property3` phút chín, `Property4` item thu, `Property6/7` exp trang viên/phụ giúp.
- `cardachievement.RequireType` 1/2/3 = kim/ngân/mọi thẻ, ngưỡng `Quality` lấy từ ServerConfig `CardGoldQuality` / `CardSilverQuality`.
- Không có cổng thanh toán nên "nạp" (msg 259) tiêu vàng để đổi ra điểm nạp.

Chưa port (không phải bảng gameplay): backup `TemplateAlllistbkap`, biến thể trùng dữ liệu (`*_out`, `balllist_b`, `Totem_Info`, `exerciseinfolist1 (2)`), bảng xếp hạng chụp từ server PC (`celeb*`, `areaceleb*`, `manorwealth`, `user_lotteryrank`) và file cấu hình client Flash (`config`, `buttonconfig`, `debugConfig`, `crossdomain`).


### Kiểm tra biên dịch không cần Unity

`tools/compilecheck/compile_check.sh` chỉ cần **.NET 8 runtime**: script tải Roslyn (`csc.dll`) từ NuGet vào thư mục tạm rồi biên dịch

1. server standalone (`Server/GunMobile.Standalone` + `Runtime/` của package), và
2. **toàn bộ script client**, dùng `tools/compilecheck/UnityCompileStubs.cs` thay cho `UnityEngine` / `UnityEngine.UI`.

Stub chỉ mô phỏng API Unity đủ để bắt lỗi cú pháp, sai tên, sai kiểu, delegate lệch — build APK/IPA thật vẫn phải chạy Unity. Cách chạy:

```bash
bash tools/compilecheck/compile_check.sh     # in "OK: server + client compile clean"
```

Muốn chạy thử server với bảng PC thật (không cần đóng gói StreamingAssets):

```bash
GUNMOBILE_PC_DATA=legacy/data GUNMOBILE_DATA=/tmp/gm dotnet /tmp/gunmobile-compilecheck/GunMobileServer.dll
```

Server in ra số dòng đã nạp của từng bảng rồi lắng nghe 4396/1910; save người chơi nằm ở `$GUNMOBILE_DATA/server_players/*.json`.
### SWF living / bomb trên điện thoại

Unity **không** chạy Flash. `tools/swf_extract.py` lấy JPEG/PNG lớn nhất trong tag `DefineBitsJPEG3` / `DefineBitsLossless2` từ SWF living + bullet (+ vài blastout) vào:

`UnityClient/Assets/StreamingAssets/PcData/Resource/image/{game/living,bomb/bullet,bomb/blastout}/extracted/`

Runtime `SwfImage` / `PcArt` đọc file đã extract, hoặc JPEG trong SWF nếu ExtraRoots còn `.swf`.

### LAN 2 điện thoại (PhoneRoad)

Không phải `Road.Service.exe` + SQL. Mỗi máy chạy `TcpListener` native:

1. Máy A: Hall → 开战 → **开房 Fight** (status hiện IP LAN).
2. Máy B: gõ IP của A → **加入** (Road 4396 + Fight 1910).
3. Máy A chọn map → server broadcast `FightStart`. Cả Host và Joiner **đều chờ `FightStart`** (map + stats/weapon/preferredBallId cho 2 ghế).
4. Tới lượt mình thì kéo ngắm / bắn; `FightFire` JSON đồng bộ góc/lực/facing. Đi bộ gửi `FightWalk` (msg 92, ~8Hz). Server nhận `FightFire`, tự mô phỏng đường đạn rồi broadcast `FightShotResult` / `FightCrater` / `FightDamage` — client chỉ vẽ lại.

Nếu bạn muốn chơi **online qua VPS** (server chạy độc lập headless trên VPS):
- Build **Dedicated Server → Linux** trong Unity hoặc dùng CI workflow (`GunMobileServer-Linux` artifact)
- `DedicatedServerBootstrap` tự khởi động khi chạy `-batchmode` → `MobileGameServer` listen TCP **4396 / 1910**
- Mở firewall port **4396** và **1910** trên VPS
- Trên client, nhập **public IP của VPS** vào ô IP → bấm Host/Join
- Xem chi tiết: [`docs/VPS_SERVER_GUIDE.md`](VPS_SERVER_GUIDE.md)

Cổng giống PC (4396 / 1910) nhưng magic packet `0x7D01` — client Flash PC **không** nói chuyện được với PhoneRoad. RSA/login 7road chưa làm.

Android: `INTERNET` + `ACCESS_WIFI_STATE`. iOS: `NSLocalNetworkUsageDescription` (Info.plist khi build).

Mở `UnityClient/` bằng Unity 6.3 LTS (`6000.3.22f1`), Play hoặc menu **GunMobile / Build Android APK** / **Build iOS Xcode Project**.

Chạy kiểm tra:

```bash
python3 tools/test_port_helpers.py
```

## Phase 2 — Import art

1. Unzip `Archive.3.zip` → `Resource/image` vào `Application.persistentDataPath` (đừng nhét hết ~2GB PNG vào repo).
2. Map folder:
   - `image/map/{id}/fore.png` + `back.jpg` + `samll_map.png` (đúng chính tả PC)
   - `image/bomb/{id}/` viên đạn, `image/bomb/crater/{id}/` hố
   - `image/equip/{slot}/{key}/` đồ, `image/arm/` súng
3. Atlas Starling `Flash/ui/cn_trad/starling/{game,hall_scene,default}/*.xml` + PNG → `TextureAtlasParser`.
4. Nén sprite (ASTC/ETC2), max 2048 atlas, tắt mip map UI.
5. SWF (`login.swf`, `shape.swf`…) **không** load được trong Unity — export lại bằng JPEXS / animate, hoặc vẽ UI uGUI từ Morn XML (`MornUiBuilder`).

`ResLoader` tìm file theo thứ tự: `persistentDataPath` rồi `StreamingAssets`. Giữ nguyên path PC (`Flash/…`, `Resource/image/…`, `Request/…`).

## Phase 3 — UI mobile

PC ~1000×600, chuột, rất nhiều cửa sổ. Mobile:

| PC | Mobile |
| --- | --- |
| Bàn phím góc/lực | `TouchAimController` (kéo tay phải) |
| A/D đi | `TouchMoveController` (tay trái) |
| Hall icon dày | 5–7 nút thumb + sheet “thêm” |
| Chat/full HUD | Safe area, chữ ≥ 28px physical, hitbox ≥ 88dp |
| Mỗi popup instantiate | `UiObjectPool` |

`MobileUiBootstrap.CreateRoot()` tạo Canvas `ScaleWithScreenSize` match 0.5, child `SafeAreaFitter`.

Ưu tiên màn: Login (`Flash/1.png`) → Hall PC `hall_newyear_scene_build` (开战/副本/公会/迷宫/排行 podium) → Room (`samll_map.png`). Shop/bag hiện `icon_1.png` / `unfrightprop/*/icon.png` / `arm/*/1/icon.png`. HUD trận dùng `gameprop.xml` (`game_prop_1`…). Living/bomb từ SWF PC.

Morn `.ui`: zlib + vài `<View>`. Builder map `Image/Button/CheckBox/Label` → uGUI. Skin `asset.*` cần bảng lookup sprite (sau khi convert SWF/atlas).

## Phase 4 — Logic trận

1. Load `fore.map` + `fore.png`. Vẽ terrain bằng `SpriteMask` hoặc texture CPU update sau `CutCircle`.
2. `ProjectileSimulator.Launch(angle, power, facing)` rồi `FlyUntil` với `MapCollision.IsSolid`.
3. Gió `BattleLoop.Wind` (bội số 10, −30…30), gravity **0.7 px/frame** từ `game.logic.dll` (`PcPhysics`). `SpeedScale=1` (px/frame / power). Wind scale **0.04**. BallList `Weight`/`Wind` điều chỉnh factor.
4. `bombconfig.xml` → `BombTable` (Common / Special ball id).
5. Nổ: `MapCollision.CutCircle` + PNG crater (`Resource/image/bomb/crater/{id}`). Living: zip-atlas `living948.png` (TexturePacker). Aim dots + màn kết trận.
6. `DamageCalculator` (atk/def/luck) — tinh chỉnh theo công thức server khi decompile `Fight`.
7. Turn 20s, `SUCIDE_TIME` 120s từ `config.xml`.

Trục: Unity `y` lên; map bit `y` xuống. Bootstrap demo: `IsSolid(x, map.Height - y)`.

## Phase 5 — Nhân vật & item

- `characterdefine.xml`: action `stand/walk/inhale*` layer `head/body/effect`.
- Item template: `Request/TemplateAllList_out.xml` (lớn, không extract mặc định nếu >2MB — lấy từ zip).
- Equip path `Resource/image/equip/{slot}/{id}/`.

## Phase 6 — Mạng

`MobileGameServer` thay thế `Road.Service.exe` + `Fight.Service.exe` + SQL Server:

- **Cổng giống PC**: Road 4396, Fight 1910, magic `0x7D01`
- **Auth**: nick-based login, server tạo player profile, JSON persistence
- **Hall systems server-authoritative**: shop buy, equip, quest, pet/card/title/totem/mount select, sign-in, lottery, forge, guild, friends, mail, chat broadcast
- **Room/matchmaking**: create/join room, room list
- **Battle (server-authoritative)**: server broadcast `FightStart` (map + per-seat stats + `weaponId`/`preferredBallId`), relay `FightWalk`, và với mỗi `FightFire` (góc/lực/facing) thì `ServerSimulateFire()` chạy `ProjectileSimulator` + `MapCollision` ngay trên server: dò va chạm, `CutCircle` địa hình, `DamageCalculator` tính damage/heal/chí mạng, rồi broadcast `FightShotResult` (kèm path mẫu) + `FightCrater` + `FightDamage`. `FightDamage` do client gửi lên bị **bỏ qua**. Khi trận kết, client gửi `FightOver` → server award gold/exp và gửi `FightReward` + `ProfileData`.
- **Persistence**: JSON save per player in `persistentDataPath/server_players/`

Không dùng SQL Server / RSA / LoginKey PC. Client gửi PhoneMsg, server validate và reply.

`Flash/config.xml` và `Road.Service.exe.config` trong zip chứa IP/password SQL — **đổi secret**, đừng dùng production.

## Cấu trúc helper

```
UnityClient/Packages/com.gunmobile.port/Runtime/
  Core/     ZlibXml, GamePaths, XmlResultTable, PackedMornUi
  Res/      ResLoader, TextureAtlasParser, SpriteSheet, SwfImage, FlashConfig, MapCollision, CharacterDefine
  UI/       MobileUiBootstrap, SafeAreaFitter, UiObjectPool, MornUiBuilder, TouchAim/Move
  Logic/    ProjectileSimulator, BattleLoop, BombTable, DamageCalculator
  Net/      PhonePacket, PhoneRoadServer, PhoneRoadClient, MobileGameServer (LAN 4396/1910)
  GunMobileBootstrap.cs
```

Kéo package vào Unity (`manifest.json` file: path) rồi add `GunMobileBootstrap` vào scene. Copy `Samples~/StreamingAssets` vào `Assets/StreamingAssets`.

## Rủi ro

- SWF không port máy móc — UI phải dựng lại.
- `fore.map` bit order giả định MSB-left (khớp stride 1250→157). Nếu terrain lệch, đảo mask `0x80 >>` thành `1 << (x & 7)`.
- Physics: `game.logic.dll` `Physics`/`SimpleBomb` — gravity 0.7/frame, wind 0.04/frame. Chưa binary-identical với mọi bomb script PVE. Hai điểm cần soi lại khi có bản PC để đối chiếu:
  * Gió đổi mỗi lượt trong khoảng ±30, nhân 0.04 thành ±1.2 px/frame² — lớn hơn cả trọng lực 0.7, nên gió mạnh đủ để đẩy ngược một phát bắn (đo được bằng `tools/compilecheck/smoke_test.sh`).
  * Nòng súng lấy `mapH - standY + 18`; nếu đổi dấu thành `- 18` thì đạn sinh ra ~17px dưới mặt đất và nổ ngay dưới chân — client lẫn server phải dùng chung dấu.
- Anti-cheat (server): damage và địa hình do server mô phỏng nên client không tự khai được sát thương. Còn hở: vị trí đi bộ (`FightWalk`) vẫn tin client, và hằng số vật lý phải khớp `game.logic.dll` thì đường đạn server mới trùng với client.
- Resource ~2GB: APK chứa **mọi map playable** + XML; equip PNG unpack local (`legacy/unpacked`).
- Online PC Road/Fight (RSA + SQL Server) không chạy trên điện thoại. Thay bằng **PhoneRoad** TCP cổng 4396/1910, magic 0x7D01, JSON bắn đồng bộ LAN.
- Dump có `__MACOSX`, file tên Trung + backup — `extract_legacy.py` đã bỏ png/swf/exe và thư mục backup.
