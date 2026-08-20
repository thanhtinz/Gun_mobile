using System.Collections;
using System.Collections.Generic;
using GunMobile.Core;
using GunMobile.Net;
using GunMobile.Res;
using GunMobile.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GunMobile.Client
{
    public enum AppState
    {
        Boot,
        Login,
        Hall,
        Room,
        Battle,
        Module
    }

    public sealed class GameApp : MonoBehaviour
    {
        public static GameApp I { get; private set; }

        public ResLoader Loader { get; private set; }
        public FlashConfig Config { get; private set; }
        public PlayerProfile Profile { get; private set; }
        public AppState State { get; private set; }

        Canvas _canvas;
        RectTransform _safe;
        Text _status;
        public RectTransform SafeArea => _safe;
        public GameDatabase Database { get; private set; }
        string _currentModuleId = "";
        int _pendingBattleMapId;
        int _pendingBattleNpcId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBoot()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            if (FindFirstObjectByType<GameApp>() != null)
            {
                return;
            }

            var go = new GameObject("GameApp");
            DontDestroyOnLoad(go);
            go.AddComponent<GameApp>();
        }

        void Awake()
        {
            if (I != null && I != this)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
            DontDestroyOnLoad(gameObject);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            EnsureCamera();
            EnsureEventSystem();
            Loader = PcContent.CreateLoader();
            PcSkin.Warm(Loader);
            Profile = PlayerProfile.Load();
            _canvas = MobileUiBootstrap.CreateRoot(transform);
            _safe = _canvas.transform.Find("SafeArea") as RectTransform;
        }

        void Start()
        {
            StartCoroutine(BootRoutine());
        }

        IEnumerator BootRoutine()
        {
            ShowStatus("Loading PC data for Android / iOS…");
            yield return PcContent.Install(Loader, ShowStatus);
            PcSkin.Warm(Loader);
            TryLoadConfig();
            ShowStatus("Loading PC tables…");
            yield return null;
            Database = GameDatabase.Load(Loader) ?? new GameDatabase();
            Profile.EnsureStarterBag();
            Profile.RecalcStats(Database);
            PhoneNet.Boot(Database, Loader);
            ShowLogin();
        }

        void TryLoadConfig()
        {
            if (Loader.TryReadBytes(GamePaths.ConfigXml, out byte[] bytes) ||
                Loader.TryReadBytes("Flash/config.xml", out bytes) ||
                Loader.TryReadBytes("Config/config.xml", out bytes))
            {
                Config = FlashConfig.Load(ZlibXml.Load(bytes));
            }
        }

        public void ShowLogin()
        {
            State = AppState.Login;
            LoginScreen.Show(_safe, this);
        }

        public void ShowHall()
        {
            Profile.Save();
            PhoneNet.EnsureConnected(Profile.Nick);
            State = AppState.Hall;
            HallScreen.Show(_safe, this);
        }

        public void ShowRoom()
        {
            State = AppState.Room;
            RoomScreen.Show(_safe, this);
        }

        public void ShowBattle(int mapId, int npcId = 0, string fightStartJson = null)
        {
            Profile.MapId = mapId;
            Profile.Save();
            State = AppState.Battle;
            BattleRuntime.Show(_safe, this, mapId, npcId, fightStartJson);
        }

        public void ShowModule(ModuleDef module)
        {
            if (module.OpensBattle)
            {
                ShowRoom();
                return;
            }

            State = AppState.Module;
            _currentModuleId = module.Id;
            switch (module.Id)
            {
                case "shop":
                    ShopScreen.Show(_safe, this);
                    return;
                case "bag":
                    BagScreen.Show(_safe, this);
                    return;
                case "quest":
                    QuestScreen.Show(_safe, this);
                    return;
                case "character":
                    CharacterScreen.Show(_safe, this);
                    return;
                case "signin":
                case "church":
                    SignInScreen.Show(_safe, this);
                    return;
                case "calendar":
                    CalendarScreen.Show(_safe, this);
                    return;
                case "quiz":
                    ExtraModulesScreens.QuizScreen(_safe, this);
                    return;
                case "oneyuan":
                    ExtraModulesScreens.OneYuanScreen(_safe, this);
                    return;
                case "godcard":
                    GodCardScreen.Show(_safe, this);
                    return;
                case "godcardraise":
                    GodCardRaiseScreen.Show(_safe, this);
                    return;
                case "engrave":
                    EngraveScreen.Show(_safe, this);
                    return;
                case "stock":
                    StockScreen.Show(_safe, this);
                    return;
                case "setting":
                    SettingsScreen.Show(_safe, this);
                    return;
                case "pet":
                    PetScreen.Show(_safe, this);
                    return;
                case "card":
                    CardScreen.Show(_safe, this);
                    return;
                case "jamps":
                    JampsScreen.Show(_safe, this);
                    return;
                case "title":
                    TitleScreen.Show(_safe, this);
                    return;
                case "totem":
                    TotemScreen.Show(_safe, this);
                    return;
                case "horse":
                    MountScreen.Show(_safe, this);
                    return;
                case "achievement":
                    AchievementScreen.Show(_safe, this);
                    return;
                case "linkpal":
                    LinkPalScreen.Show(_safe, this);
                    return;
                case "elf":
                    ElfScreen.Show(_safe, this);
                    return;
                case "farm":
                    FarmScreen.Show(_safe, this);
                    return;
                case "consortia":
                    ConsortiaScreen.Show(_safe, this);
                    return;
                case "rank":
                    RankScreen.Show(_safe, this);
                    return;
                case "auction":
                    AuctionScreen.Show(_safe, this);
                    return;
                case "vip":
                    VipScreen.Show(_safe, this);
                    return;
                case "lottery":
                    LotteryScreen.Show(_safe, this);
                    return;
                case "labyrinth":
                    LabyrinthScreen.Show(_safe, this);
                    return;
                case "worldboss":
                    WorldBossScreen.Show(_safe, this);
                    return;
                case "dungeon":
                    DungeonScreen.Show(_safe, this);
                    return;
                case "npc":
                    NpcHuntScreen.Show(_safe, this);
                    return;
                case "store":
                    ForgeScreen.Show(_safe, this);
                    return;
                case "texp":
                    TexpScreen.Show(_safe, this);
                    return;
                case "gemstone":
                    GemScreen.Show(_safe, this);
                    return;
                case "kingbless":
                    KingBlessScreen.Show(_safe, this);
                    return;
                case "friend":
                    FriendScreen.Show(_safe, this);
                    return;
                case "mail":
                    MailInboxScreen.Show(_safe, this);
                    return;
                case "im":
                    ChatScreen.Show(_safe, this);
                    return;
                case "ball":
                    BallPickScreen.Show(_safe, this);
                    return;
                case "bomb":
                    BombConfigScreen.Show(_safe, this);
                    return;
                case "magicstone":
                    ExtraModulesScreens.MagicStoneScreen(_safe, this);
                    return;
                case "enchant":
                    ExtraModulesScreens.EnchantScreen(_safe, this);
                    return;
                case "teamdungeon":
                    ExtraModulesScreens.TeamDungeonScreen(_safe, this);
                    return;
                case "carnival":
                    ExtraModulesScreens.CarnivalScreen(_safe, this);
                    return;
                case "bank":
                    ExtraModulesScreens.BankScreen(_safe, this);
                    return;
                case "mines":
                    ExtraModulesScreens.MinesScreen(_safe, this);
                    return;
                case "auditorium":
                    ExtraModulesScreens.AuditoriumScreen(_safe, this);
                    return;
                case "treasure":
                    ExtraModulesScreens.TreasureScreen(_safe, this);
                    return;
                case "peakbattle":
                    ExtraModulesScreens.PeakBattleScreen(_safe, this);
                    return;
                case "necklace":
                    ExtraModulesScreens.NecklaceScreen(_safe, this);
                    return;
                case "devilturn":
                    ExtraModulesScreens.DevilTurnScreen(_safe, this);
                    return;
                case "redpacket":
                    ExtraModulesScreens.RedPacketScreen(_safe, this);
                    return;
                case "homeTemple":
                    ExtraModulesScreens.HomeTempleScreen(_safe, this);
                    return;
                case "sweep":
                    ExtraModulesScreens.SweepScreen(_safe, this);
                    return;
                case "emblem":
                    ExtraModulesScreens.EmblemScreen(_safe, this);
                    return;
                case "soulmark":
                    ExtraModulesScreens.SoulMarkScreen(_safe, this);
                    return;
                case "magicwardrobe":
                    ExtraModulesScreens.MagicWardrobeScreen(_safe, this);
                    return;
                case "honorhall":
                    ExtraModulesScreens.HonorHallScreen(_safe, this);
                    return;
                case "glory":
                    ExtraModulesScreens.GloryScreen(_safe, this);
                    return;
                case "sigil":
                    ExtraModulesScreens.SigilScreen(_safe, this);
                    return;
                case "jade":
                    ExtraModulesScreens.JadeScreen(_safe, this);
                    return;
                case "rune":
                    ExtraModulesScreens.RuneScreen(_safe, this);
                    return;
                case "horseamulet":
                    ExtraModulesScreens.HorseAmuletScreen(_safe, this);
                    return;
                case "dreamland":
                    ExtraModulesScreens.DreamlandScreen(_safe, this);
                    return;
                case "darkboundary":
                    ExtraModulesScreens.DarkBoundaryScreen(_safe, this);
                    return;
                case "firstrecharge":
                    ExtraModulesScreens.FirstRechargeScreen(_safe, this);
                    return;
                case "forcesbattle":
                    ExtraModulesScreens.ForcesBattleScreen(_safe, this);
                    return;
                case "culture":
                    ExtraModulesScreens.CultureScreen(_safe, this);
                    return;
                case "labyrinthgame":
                    ExtraModulesScreens.LabyrinthGameScreen(_safe, this);
                    return;
                case "treasureroom":
                    ExtraModulesScreens.TreasureRoomScreen(_safe, this);
                    return;
                case "christmas":
                    ExtraModulesScreens.ChristmasScreen(_safe, this);
                    return;
                case "newyear":
                    ExtraModulesScreens.NewYearScreen(_safe, this);
                    return;
                case "worshipthemoon":
                    ExtraModulesScreens.WorshipMoonScreen(_safe, this);
                    return;
                case "jigsaw":
                    ExtraModulesScreens.JigsawScreen(_safe, this);
                    return;
                case "bible":
                    ExtraModulesScreens.BibleScreen(_safe, this);
                    return;
                case "carnivalSuperLucker":
                    ExtraModulesScreens.CarnivalSuperLuckerScreen(_safe, this);
                    return;
                case "boguadventure":
                    ExtraModulesScreens.BoguAdventureScreen(_safe, this);
                    return;
                default:
                    if (!string.IsNullOrEmpty(module.MornUiFile))
                    {
                        ExtraModulesScreens.ShowMornModule(_safe, this, module, module.MornUiFile);
                        return;
                    }

                    DataBrowserScreen.Show(_safe, this, module);
                    return;
            }
        }

        void RefreshCurrentModule()
        {
            if (string.IsNullOrEmpty(_currentModuleId))
            {
                return;
            }

            ShowModule(new ModuleDef(_currentModuleId, _currentModuleId));
        }

        public void ShowStatus(string msg)
        {
            if (_status == null)
            {
                UiKit.ClearChildren(_safe);
                var panel = UiKit.Panel(_safe, "Boot", new Color(0.05f, 0.07f, 0.1f, 1f));
                _status = UiKit.Label(panel.transform, "Status", msg, 32, Color.white, TextAnchor.MiddleCenter);
                UiKit.Stretch(_status.gameObject);
            }
            else
            {
                _status.text = msg;
            }
        }

        static void EnsureCamera()
        {
            if (Camera.main != null)
            {
                return;
            }

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.07f, 0.1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.nearClipPlane = -10f;
            cam.farClipPlane = 100f;
            DontDestroyOnLoad(camGo);
        }

        static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(es);
        }

        void Update()
        {
            PhoneNet.TickKeepAlive(Time.deltaTime);
            PumpRoad();
            PumpFight();
        }

        void PumpRoad()
        {
            if (PhoneNet.Road == null) return;
            while (PhoneNet.Road.TryDequeue(out var msg))
            {
                switch (msg.Id)
                {
                    case PhoneMsg.LoginOk:
                        PhoneNet.PlayerId = JsonInt(msg.Json, "playerId", PhoneNet.PlayerId);
                        break;
                    case PhoneMsg.ProfileData:
                    case PhoneMsg.StatResult:
                    case PhoneMsg.QuestResult:
                    case PhoneMsg.ShopResult:
                    case PhoneMsg.EquipResult:
                    case PhoneMsg.SignInResult:
                    case PhoneMsg.LotteryResult:
                    case PhoneMsg.GodCardResult:
                    case PhoneMsg.GodCardRaise:
                    case PhoneMsg.GodCardPointClaim:
                    case PhoneMsg.StockResult:
                    case PhoneMsg.StrengthenResult:
                    case PhoneMsg.GuildResult:
                    case PhoneMsg.WardrobeUpgrade:
                    case PhoneMsg.HonorSystemAction:
                    case PhoneMsg.HonorSystemClaim:
                    case PhoneMsg.ForcesRelicUpgrade:
                    case PhoneMsg.CultureResult:
                    case PhoneMsg.JampsUpgrade:
                    case PhoneMsg.JampsClaimPage:
                    case PhoneMsg.CardMainUpgrade:
                    case PhoneMsg.ElfIntimacyAction:
                    case PhoneMsg.GuildUpgrade:
                    case PhoneMsg.ConsortiaBossStart:
                    case PhoneMsg.PetStarUpgrade:
                    case PhoneMsg.MountTalismanEquip:
                    case PhoneMsg.ManorUpgrade:
                    case PhoneMsg.GoldEquipUpgrade:
                    case PhoneMsg.GloryUpgrade:
                    case PhoneMsg.SigilRoll:
                    case PhoneMsg.MountSkillUnlock:
                    case PhoneMsg.AchievementClaim:
                    case PhoneMsg.LinkPalAction:
                    case PhoneMsg.JadeEquip:
                    case PhoneMsg.RuneEquip:
                    case PhoneMsg.HorseAmuletUpgrade:
                    case PhoneMsg.CardBookletClaim:
                    case PhoneMsg.StrengthenGoodsMap:
                    case PhoneMsg.BoxOpen:
                    case PhoneMsg.ItemFusion:
                        if (msg.Id == PhoneMsg.MountSkillUnlock) PhoneNet.LastMountSkillJson = msg.Json;
                        if (msg.Id == PhoneMsg.AchievementClaim) PhoneNet.LastAchievementJson = msg.Json;
                        if (msg.Id == PhoneMsg.LinkPalAction) PhoneNet.LastLinkPalJson = msg.Json;
                        PhoneNet.LastGuildJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && !string.IsNullOrEmpty(_currentModuleId))
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.MailResult:
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && !string.IsNullOrEmpty(_currentModuleId))
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.MailListData:
                        PhoneNet.LastMailListJson = msg.Json;
                        if (State == AppState.Module && _currentModuleId == "mail")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.FriendResult:
                        PhoneNet.LastFriendListJson = msg.Json;
                        ParseFriendsFromServer(msg.Json);
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && !string.IsNullOrEmpty(_currentModuleId))
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.AuctionListData:
                        PhoneNet.LastAuctionListJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "auction")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.PveResult:
                        Profile.PendingReward = JsonInt(msg.Json, "reward", Profile.PendingReward);
                        PhoneNet.PendingPveNpcId = JsonInt(msg.Json, "npcId", PhoneNet.PendingPveNpcId);
                        PhoneNet.PendingPveMapId = JsonInt(msg.Json, "map", PhoneNet.PendingPveMapId);
                        ApplyProfileFromServer(msg.Json);
                        break;
                    case PhoneMsg.DreamlandClaim:
                    case PhoneMsg.WarriorFamClaim:
                    case PhoneMsg.SweepLabyrinth:
                    case PhoneMsg.SweepMission:
                    case PhoneMsg.HomeTemplePractice:
                    case PhoneMsg.HomeTempleAdvance:
                    case PhoneMsg.BankDeposit:
                    case PhoneMsg.FirstRechargeClaim:
                    case PhoneMsg.FirstRechargeShop:
                    case PhoneMsg.MailSend:
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && !string.IsNullOrEmpty(_currentModuleId))
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.SpaRoomStart:
                    case PhoneMsg.SpaRoomBomb:
                        PhoneNet.LastSpaRoomJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "labyrinthgame")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.TreasureRoomResult:
                        PhoneNet.LastTreasureRoomJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "treasureroom")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.ChristmasClaim:
                        PhoneNet.LastChristmasJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "christmas")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.NewYearClaim:
                        PhoneNet.LastNewYearJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "newyear")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.WorshipMoonClaim:
                        PhoneNet.LastWorshipMoonJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "worshipthemoon")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.SuperLuckerDraw:
                        PhoneNet.LastSuperLuckerJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "carnivalSuperLucker") RefreshCurrentModule();
                        break;
                    case PhoneMsg.CalendarClaim:
                        PhoneNet.LastCalendarJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "calendar") RefreshCurrentModule();
                        break;
                    case PhoneMsg.QuizAnswer:
                        PhoneNet.LastQuizJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "quiz") RefreshCurrentModule();
                        break;
                    case PhoneMsg.OneYuanBuy:
                        PhoneNet.LastOneYuanJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && (_currentModuleId == "oneyuan" || _currentModuleId == "shop")) RefreshCurrentModule();
                        break;
                    case PhoneMsg.ActivityQuestClaim:
                        PhoneNet.LastActivityQuestJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && (_currentModuleId == "quest" || _currentModuleId == "activityquest")) RefreshCurrentModule();
                        break;
                    case PhoneMsg.SwornAction:
                        PhoneNet.LastSwornJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && (_currentModuleId == "auction" || _currentModuleId == "friend")) RefreshCurrentModule();
                        break;
                    case PhoneMsg.VipStoreBuy:
                        PhoneNet.LastVipStoreJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "vip") RefreshCurrentModule();
                        break;
                    case PhoneMsg.AuditoriumAction:
                        PhoneNet.LastAuditoriumJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "auditorium") RefreshCurrentModule();
                        break;
                    case PhoneMsg.BoguAdventureAction:
                        PhoneNet.LastBoguAdventureJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "boguadventure") RefreshCurrentModule();
                        break;
                    case PhoneMsg.JigsawAction:
                        PhoneNet.LastJigsawJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "jigsaw")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.BibleAction:
                        PhoneNet.LastBibleJson = msg.Json;
                        ApplyProfileFromServer(msg.Json);
                        if (State == AppState.Module && _currentModuleId == "bible")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.Error:
                    {
                        string err = JsonStr(msg.Json, "err", "error");
                        ShowStatus("Server: " + err);
                        break;
                    }
                    case PhoneMsg.ChatBroadcast:
                        string from = JsonStr(msg.Json, "from", "?");
                        string cm = JsonStr(msg.Json, "msg", "");
                        if (!string.IsNullOrEmpty(cm))
                        {
                            bool whisper = (msg.Json ?? "").IndexOf("\"whisper\":true", StringComparison.Ordinal) >= 0;
                            Profile.ChatLog.Add(whisper ? ("[密] " + from + ": " + cm) : (from + ": " + cm));
                            if (Profile.ChatLog.Count > 100) Profile.ChatLog.RemoveAt(0);
                        }
                        if (State == AppState.Module && _currentModuleId == "im")
                        {
                            RefreshCurrentModule();
                        }
                        break;
                    case PhoneMsg.RoomCreated:
                        PhoneNet.RoomId = JsonInt(msg.Json, "roomId", -1);
                        PhoneNet.Seat = JsonInt(msg.Json, "seat", 0);
                        break;
                    case PhoneMsg.RoomOk:
                        PhoneNet.RoomId = JsonInt(msg.Json, "roomId", PhoneNet.RoomId);
                        PhoneNet.Seat = JsonInt(msg.Json, "seat", PhoneNet.Seat);
                        break;
                    case PhoneMsg.RankData:
                        PhoneNet.LastRankJson = msg.Json;
                        break;
                    case PhoneMsg.RoomListData:
                        PhoneNet.LastRoomListJson = msg.Json;
                        break;
                    case PhoneMsg.RoomState:
                        PhoneNet.LastRoomStateJson = msg.Json;
                        break;
                }
            }
        }

        void PumpFight()
        {
            if (PhoneNet.Fight == null) return;
            while (PhoneNet.Fight.TryDequeue(out var msg))
            {
                if (msg.Id == PhoneMsg.FightStart)
                {
                    int mapId = JsonInt(msg.Json, "map", PhoneNet.PendingPveMapId > 0 ? PhoneNet.PendingPveMapId : 1056);
                    int seed = JsonInt(msg.Json, "seed", 0);
                    if (seed != 0) PhoneNet.BattleSeed = seed;
                    PhoneNet.NetBattle = true;
                    int npcId = PhoneNet.PendingPveNpcId;
                    PhoneNet.PendingPveMapId = 0;
                    PhoneNet.PendingPveNpcId = 0;
                    ShowBattle(mapId, npcId, msg.Json);
                }
            }
        }

        void ApplyProfileFromServer(string json)
        {
            if (string.IsNullOrEmpty(json) || Profile == null) return;
            Profile.Gold = JsonInt(json, "gold", Profile.Gold);
            Profile.Gift = JsonInt(json, "gift", Profile.Gift);
            Profile.Gp = JsonInt(json, "gp", Profile.Gp);
            Profile.Level = JsonInt(json, "level", Profile.Level);
            string nick = JsonStr(json, "nick", null);
            if (nick != null) Profile.Nick = nick;
            Profile.Attack = JsonInt(json, "attack", Profile.Attack);
            Profile.Defence = JsonInt(json, "defence", Profile.Defence);
            Profile.Agility = JsonInt(json, "agility", Profile.Agility);
            Profile.Luck = JsonInt(json, "luck", Profile.Luck);
            Profile.Hp = JsonInt(json, "hp", Profile.Hp);
            Profile.Win = JsonInt(json, "win", Profile.Win);
            Profile.Lose = JsonInt(json, "lose", Profile.Lose);
            Profile.WeaponId = JsonInt(json, "weaponId", Profile.WeaponId);
            Profile.EquipHead = JsonInt(json, "equipHead", Profile.EquipHead);
            Profile.EquipHair = JsonInt(json, "equipHair", Profile.EquipHair);
            Profile.EquipFace = JsonInt(json, "equipFace", Profile.EquipFace);
            Profile.EquipCloth = JsonInt(json, "equipCloth", Profile.EquipCloth);
            Profile.EquipGlass = JsonInt(json, "equipGlass", Profile.EquipGlass);
            Profile.EquipWeapon = JsonInt(json, "equipWeapon", Profile.EquipWeapon);
            Profile.PetId = JsonInt(json, "petId", Profile.PetId);
            Profile.CardId = JsonInt(json, "cardId", Profile.CardId);
            Profile.TitleId = JsonInt(json, "titleId", Profile.TitleId);
            Profile.TotemId = JsonInt(json, "totemId", Profile.TotemId);
            Profile.MountGrade = JsonInt(json, "mountGrade", Profile.MountGrade);
            Profile.MountTalismanId = JsonInt(json, "mountTalismanId", Profile.MountTalismanId);
            Profile.EnsureMountSkills();
            ParseIntListFromServer(json, "mountSkillIds", Profile.MountSkillIds);
            Profile.ManorGrade = JsonInt(json, "manorGrade", Profile.ManorGrade);
            Profile.LinkPalId = JsonInt(json, "linkPalId", Profile.LinkPalId);
            Profile.AchievementPoints = JsonInt(json, "achievementPoints", Profile.AchievementPoints);
            Profile.EnsureAchievements();
            ParseIntListFromServer(json, "completedAchievements", Profile.CompletedAchievements);
            ParseIntListFromServer(json, "claimedAchievements", Profile.ClaimedAchievements);
            Profile.GoldEquipId = JsonInt(json, "goldEquipId", Profile.GoldEquipId);
            Profile.GloryTemplateId = JsonInt(json, "gloryTemplateId", Profile.GloryTemplateId);
            Profile.SigilQuality = JsonInt(json, "sigilQuality", Profile.SigilQuality);
            Profile.SigilProType = JsonInt(json, "sigilProType", Profile.SigilProType);
            Profile.SigilProValue = JsonInt(json, "sigilProValue", Profile.SigilProValue);
            Profile.JadeEquipId = JsonInt(json, "jadeEquipId", Profile.JadeEquipId);
            Profile.RuneTemplateId = JsonInt(json, "runeTemplateId", Profile.RuneTemplateId);
            Profile.HorseAmuletLevel = JsonInt(json, "horseAmuletLevel", Profile.HorseAmuletLevel);
            Profile.HorseAmuletGrade = JsonInt(json, "horseAmuletGrade", Profile.HorseAmuletGrade);
            Profile.HorseAmuletPhase = JsonInt(json, "horseAmuletPhase", Profile.HorseAmuletPhase);
            Profile.VipLevel = JsonInt(json, "vipLevel", Profile.VipLevel);
            Profile.Honor = JsonInt(json, "honor", Profile.Honor);
            Profile.Texp = JsonInt(json, "texp", Profile.Texp);
            Profile.PreferredBallId = JsonInt(json, "preferredBallId", Profile.PreferredBallId);
            Profile.LastSignDay = JsonInt(json, "lastSignDay", Profile.LastSignDay);
            Profile.SignIndex = JsonInt(json, "signIndex", Profile.SignIndex);
            Profile.LabyrinthFloor = JsonInt(json, "labyrinthFloor", Profile.LabyrinthFloor);
            Profile.ElfId = JsonInt(json, "elfId", Profile.ElfId);
            Profile.GemLevel = JsonInt(json, "gemLevel", Profile.GemLevel);
            Profile.KingBlessDay = JsonInt(json, "kingBlessDay", Profile.KingBlessDay);
            Profile.FarmHarvests = JsonInt(json, "farmHarvests", Profile.FarmHarvests);
            Profile.FusionKeys = JsonInt(json, "fusionKeys", Profile.FusionKeys);
            Profile.BankGold = JsonInt(json, "bankGold", Profile.BankGold);
            Profile.MineDigs = JsonInt(json, "mineDigs", Profile.MineDigs);
            Profile.WorldBossHits = JsonInt(json, "worldBossHits", Profile.WorldBossHits);
            Profile.GuildLevel = JsonInt(json, "guildLevel", Profile.GuildLevel);
            Profile.ConsortiaBossHits = JsonInt(json, "consortiaBossHits", Profile.ConsortiaBossHits);
            Profile.NecklaceLevel = JsonInt(json, "necklaceLevel", Profile.NecklaceLevel);
            Profile.HomeTempleLevel = JsonInt(json, "homeTempleLevel", Profile.HomeTempleLevel);
            Profile.HomeTemplePracticeLevel = JsonInt(json, "homeTemplePracticeLevel", Profile.HomeTemplePracticeLevel);
            Profile.HomeTempleAdvanceLevel = JsonInt(json, "homeTempleAdvanceLevel", Profile.HomeTempleAdvanceLevel);
            Profile.WardrobeClothId = JsonInt(json, "wardrobeClothId", Profile.WardrobeClothId);
            Profile.HonorSystemExp = JsonInt(json, "honorSystemExp", Profile.HonorSystemExp);
            Profile.HonorSystemLevel = JsonInt(json, "honorSystemLevel", Profile.HonorSystemLevel);
            Profile.RedPacketClaims = JsonInt(json, "redPacketClaims", Profile.RedPacketClaims);
            Profile.DevilTurnSpins = JsonInt(json, "devilTurnSpins", Profile.DevilTurnSpins);
            Profile.DevilTurnPoints = JsonInt(json, "devilTurnPoints", Profile.DevilTurnPoints);
            Profile.SpaRoomDayScore = JsonInt(json, "spaRoomDayScore", Profile.SpaRoomDayScore);
            ParseDevilTreasPointClaimedFromServer(json);
            ParseQuestsFromServer(json);
            Profile.TreasureRoomDraws = JsonInt(json, "treasureRoomDraws", Profile.TreasureRoomDraws);
            Profile.ChristmasClaims = JsonInt(json, "christmasClaims", Profile.ChristmasClaims);
            Profile.NewYearPoints = JsonInt(json, "newYearPoints", Profile.NewYearPoints);
            Profile.NewYearFreeUsed = JsonInt(json, "newYearFreeUsed", Profile.NewYearFreeUsed);
            Profile.WorshipMoonDraws = JsonInt(json, "worshipMoonDraws", Profile.WorshipMoonDraws);
            Profile.SuperLuckerDraws = JsonInt(json, "superLuckerDraws", Profile.SuperLuckerDraws);
            Profile.JigsawClaims = JsonInt(json, "jigsawClaims", Profile.JigsawClaims);
            Profile.BibleClaims = JsonInt(json, "bibleClaims", Profile.BibleClaims);
            Profile.SweepCount = JsonInt(json, "sweepCount", Profile.SweepCount);
            Profile.FirstRechargeClaimed = JsonInt(json, "firstRechargeClaimed", Profile.FirstRechargeClaimed ? 1 : 0) != 0;
            Profile.DreamlandChapter = JsonInt(json, "dreamlandChapter", Profile.DreamlandChapter);
            Profile.DreamlandSection = JsonInt(json, "dreamlandSection", Profile.DreamlandSection);
            Profile.DreamlandClearedSection = JsonInt(json, "dreamlandClearedSection", Profile.DreamlandClearedSection);
            Profile.DreamlandAttempts = JsonInt(json, "dreamlandAttempts", Profile.DreamlandAttempts);
            Profile.WarriorFamHardType = JsonInt(json, "warriorFamHardType", Profile.WarriorFamHardType);
            Profile.WarriorFamLevel = JsonInt(json, "warriorFamLevel", Profile.WarriorFamLevel);
            Profile.WarriorFamClearedLevel = JsonInt(json, "warriorFamClearedLevel", Profile.WarriorFamClearedLevel);
            Profile.WarriorFamAttempts = JsonInt(json, "warriorFamAttempts", Profile.WarriorFamAttempts);
            Profile.ForcesBattleScore = JsonInt(json, "forcesBattleScore", Profile.ForcesBattleScore);
            Profile.ForcesBattleAttempts = JsonInt(json, "forcesBattleAttempts", Profile.ForcesBattleAttempts);
            Profile.CultureGrade = JsonInt(json, "cultureGrade", Profile.CultureGrade);
            Profile.CultureAtk = JsonInt(json, "cultureAtk", Profile.CultureAtk);
            Profile.CultureDef = JsonInt(json, "cultureDef", Profile.CultureDef);
            Profile.CultureAgi = JsonInt(json, "cultureAgi", Profile.CultureAgi);
            Profile.CultureLuck = JsonInt(json, "cultureLuck", Profile.CultureLuck);
            Profile.JampsManualLevel = JsonInt(json, "jampsManualLevel", Profile.JampsManualLevel);
            Profile.CardMainLevel = JsonInt(json, "cardMainLevel", Profile.CardMainLevel);
            Profile.ElfIntimacyExp = JsonInt(json, "elfIntimacyExp", Profile.ElfIntimacyExp);
            Profile.ElfIntimacyLevel = JsonInt(json, "elfIntimacyLevel", Profile.ElfIntimacyLevel);
            Profile.ElfIntimacyActions = JsonInt(json, "elfIntimacyActions", Profile.ElfIntimacyActions);
            Profile.CalendarMonth = JsonInt(json, "calendarMonth", Profile.CalendarMonth);
            Profile.AuditoriumActions = JsonInt(json, "auditoriumActions", Profile.AuditoriumActions);
            Profile.BoguAdventureActions = JsonInt(json, "boguAdventureActions", Profile.BoguAdventureActions);
            Profile.QuizAttempts = JsonInt(json, "quizAttempts", Profile.QuizAttempts);
            Profile.GodCardEquipId = JsonInt(json, "godCardEquipId", Profile.GodCardEquipId);
            Profile.GodCardPoints = JsonInt(json, "godCardPoints", Profile.GodCardPoints);
            Profile.EngraveSetId = JsonInt(json, "engraveSetId", Profile.EngraveSetId);
            string consortia = JsonStr(json, "consortiaName", null);
            if (consortia != null) Profile.ConsortiaName = consortia;
            ParseBagFromServer(json);
            ParseGodCardsFromServer(json);
            ParseGodCardPointClaimedFromServer(json);
            ParseStockFromServer(json);
            ParseFriendsFromServer(json);
            ParseFightSpiritsFromServer(json);
            ParseMagicStonesFromServer(json);
            ParseEmblemsFromServer(json);
            ParseSoulStampsFromServer(json);
            ParseRelicsFromServer(json);
            ParseWardrobeFromServer(json);
            ParseHonorSystemFromServer(json);
            ParseNewYearClaimedFromServer(json);
            ParseIntListFromServer(json, "calendarClaimedDays", Profile.CalendarClaimedDays ?? (Profile.CalendarClaimedDays = new List<int>()));
            ParseIntListFromServer(json, "oneYuanBought", Profile.OneYuanBought ?? (Profile.OneYuanBought = new List<int>()));
            ParseIntListFromServer(json, "activityQuestClaimed", Profile.ActivityQuestClaimed ?? (Profile.ActivityQuestClaimed = new List<int>()));
            ParseIntListFromServer(json, "activityQuestCompleted", Profile.ActivityQuestCompleted ?? (Profile.ActivityQuestCompleted = new List<int>()));
            ParseIntListFromServer(json, "vipStoreBought", Profile.VipStoreBought ?? (Profile.VipStoreBought = new List<int>()));
            Profile.ActivityQuestPeriod = JsonInt(json, "activityQuestPeriod", Profile.ActivityQuestPeriod);
            string swornNick = JsonStr(json, "swornNick", null);
            if (swornNick != null) Profile.SwornNick = swornNick;
            Profile.SwornLevel = JsonInt(json, "swornLevel", Profile.SwornLevel);
            Profile.SwornGp = JsonInt(json, "swornGp", Profile.SwornGp);
            ParseBankDepositsFromServer(json);
            ParseSweepMissionClearsFromServer(json);
            ParseIntListFromServer(json, "jampsDebrisOwned", Profile.JampsDebrisOwned ?? (Profile.JampsDebrisOwned = new List<int>()));
            ParseIntListFromServer(json, "jampsPagesCollected", Profile.JampsPagesCollected ?? (Profile.JampsPagesCollected = new List<int>()));
            ParseIntListFromServer(json, "jampsPagesActivated", Profile.JampsPagesActivated ?? (Profile.JampsPagesActivated = new List<int>()));
            ParseIntListFromServer(json, "ownedCardTemplateIds", Profile.OwnedCardTemplateIds ?? (Profile.OwnedCardTemplateIds = new List<int>()));
            ParseIntListFromServer(json, "cardBookletProfiles", Profile.CardBookletProfiles ?? (Profile.CardBookletProfiles = new List<int>()));
            ParseIntListFromServer(json, "cardBookletClaimed", Profile.CardBookletClaimed ?? (Profile.CardBookletClaimed = new List<int>()));
            Profile.CardSoul = JsonInt(json, "cardSoul", Profile.CardSoul);
            Profile.Save();
        }

        void ParseBankDepositsFromServer(string json)
        {
            int idx = json.IndexOf("\"bankDeposits\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 15;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.EnsureBankDeposits();
            Profile.BankDeposits.Clear();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = body.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = body.Substring(ob, cb - ob + 1);
                Profile.BankDeposits.Add(new BankTermDeposit
                {
                    TemplateId = JsonInt(entry, "templateId", 0),
                    Amount = JsonInt(entry, "amount", 0),
                    DepositDay = JsonInt(entry, "depositDay", 0)
                });
                pos = cb + 1;
            }
        }

        void ParseSweepMissionClearsFromServer(string json)
        {
            int idx = json.IndexOf("\"sweepMissionClears\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 21;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.EnsureSweepMissionClears();
            Profile.SweepMissionClears.Clear();
            string chunk = json.Substring(start, end - start);
            if (string.IsNullOrWhiteSpace(chunk)) return;
            foreach (string part in chunk.Split(','))
                if (int.TryParse(part.Trim(), out int id) && id > 0) Profile.SweepMissionClears.Add(id);
        }

        static int FindMatchingJsonBrace(string json, int openBrace)
        {
            if (json == null || openBrace < 0 || openBrace >= json.Length || json[openBrace] != '{')
            {
                return -1;
            }

            int depth = 0;
            for (int i = openBrace; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '{') depth++;
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }

            return -1;
        }

        void ParseIntListFromServer(string json, string key, List<int> target)
        {
            string needle = "\"" + key + "\":[";
            int idx = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + needle.Length;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            target.Clear();
            string chunk = json.Substring(start, end - start);
            if (string.IsNullOrWhiteSpace(chunk)) return;
            foreach (string part in chunk.Split(','))
                if (int.TryParse(part.Trim(), out int id)) target.Add(id);
        }

        void ParseNewYearClaimedFromServer(string json)

        {
            int idx = json.IndexOf("\"newYearPointClaimed\":[", System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return;
            }

            int start = idx + 22;
            int end = json.IndexOf(']', start);
            if (end <= start)
            {
                return;
            }

            Profile.NewYearPointClaimed = Profile.NewYearPointClaimed ?? new List<int>();
            Profile.NewYearPointClaimed.Clear();
            string chunk = json.Substring(start, end - start);
            if (string.IsNullOrWhiteSpace(chunk))
            {
                return;
            }

            string[] parts = chunk.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i].Trim(), out int id) && id > 0)
                {
                    Profile.NewYearPointClaimed.Add(id);
                }
            }
        }

        void ParseDevilTreasPointClaimedFromServer(string json)
        {
            int idx = json.IndexOf("\"devilTreasPointClaimed\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 26;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.DevilTreasPointClaimed = Profile.DevilTreasPointClaimed ?? new List<int>();
            Profile.DevilTreasPointClaimed.Clear();
            string chunk = json.Substring(start, end - start);
            if (string.IsNullOrWhiteSpace(chunk)) return;
            foreach (string part in chunk.Split(','))
            {
                if (int.TryParse(part.Trim(), out int id) && id > 0)
                    Profile.DevilTreasPointClaimed.Add(id);
            }
        }

        void ParseQuestsFromServer(string json)
        {
            Profile.AcceptedQuests = Profile.AcceptedQuests ?? new List<int>();
            Profile.CompletedQuests = Profile.CompletedQuests ?? new List<int>();
            Profile.QuestProgress = Profile.QuestProgress ?? new Dictionary<int, List<int>>();
            ParseIntListFromServer(json, "acceptedQuests", Profile.AcceptedQuests);
            ParseIntListFromServer(json, "completedQuests", Profile.CompletedQuests);
            int idx = json.IndexOf("\"questProgress\":{", System.StringComparison.Ordinal);
            if (idx >= 0)
            {
                int openBrace = idx + 17;
                int end = FindMatchingJsonBrace(json, openBrace);
                if (end > openBrace)
                {
                    Profile.QuestProgress.Clear();
                    string body = json.Substring(openBrace + 1, end - openBrace - 1);
                    int pos = 0;
                    while (pos < body.Length)
                    {
                        int qk = body.IndexOf('"', pos);
                        if (qk < 0) break;
                        int qk2 = body.IndexOf('"', qk + 1);
                        if (qk2 < 0) break;
                        if (!int.TryParse(body.Substring(qk + 1, qk2 - qk - 1), out int questId) || questId <= 0)
                        {
                            pos = qk2 + 1;
                            continue;
                        }
                        int arrStart = body.IndexOf('[', qk2);
                        int arrEnd = arrStart >= 0 ? body.IndexOf(']', arrStart) : -1;
                        if (arrStart < 0 || arrEnd <= arrStart) break;
                        var prog = new List<int>();
                        string arr = body.Substring(arrStart + 1, arrEnd - arrStart - 1);
                        if (!string.IsNullOrWhiteSpace(arr))
                        {
                            foreach (string part in arr.Split(','))
                            {
                                if (int.TryParse(part.Trim(), out int v)) prog.Add(v);
                            }
                        }
                        Profile.QuestProgress[questId] = prog;
                        pos = arrEnd + 1;
                    }
                }
            }

            PruneStaleQuestProgress();
        }

        void PruneStaleQuestProgress()
        {
            if (Profile.QuestProgress == null || Profile.QuestProgress.Count == 0)
            {
                return;
            }

            var remove = new List<int>();
            foreach (int qid in Profile.QuestProgress.Keys)
            {
                if (Profile.CompletedQuests.Contains(qid) || !Profile.AcceptedQuests.Contains(qid))
                {
                    remove.Add(qid);
                }
            }

            for (int i = 0; i < remove.Count; i++)
            {
                Profile.QuestProgress.Remove(remove[i]);
            }
        }

        void ParseRelicsFromServer(string json)
        {
            int idx = json.IndexOf("\"relics\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 9;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.EnsureRelics();
            Profile.Relics.Clear();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = body.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = body.Substring(ob, cb - ob + 1);
                Profile.Relics.Add(new RelicSlot
                {
                    RelicId = JsonInt(entry, "relicId", 1),
                    UpgradeLevel = JsonInt(entry, "upgradeLevel", 0)
                });
                pos = cb + 1;
            }
        }

        void ParseFriendsFromServer(string json)
        {
            int idx = json.IndexOf("\"friends\":[", System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return;
            }

            int start = idx + 10;
            int end = json.IndexOf(']', start);
            if (end <= start)
            {
                return;
            }

            var names = new System.Collections.Generic.List<string>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob >= 0)
                {
                    int cb = body.IndexOf('}', ob);
                    if (cb < 0)
                    {
                        break;
                    }

                    string entry = body.Substring(ob, cb - ob + 1);
                    string nick = JsonStr(entry, "nick", null);
                    if (!string.IsNullOrEmpty(nick))
                    {
                        names.Add(nick);
                    }

                    pos = cb + 1;
                    continue;
                }

                int q1 = body.IndexOf('"', pos);
                if (q1 < 0)
                {
                    break;
                }

                int q2 = body.IndexOf('"', q1 + 1);
                if (q2 < 0)
                {
                    break;
                }

                names.Add(body.Substring(q1 + 1, q2 - q1 - 1));
                pos = q2 + 1;
            }

            if (names.Count > 0)
            {
                Profile.Friends = names;
            }
        }

        void ParseFightSpiritsFromServer(string json)
        {
            int idx = json.IndexOf("\"fightSpirits\":[", System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return;
            }

            int start = idx + 15;
            int end = json.IndexOf(']', start);
            if (end <= start)
            {
                return;
            }

            var list = new System.Collections.Generic.List<FightSpiritSlot>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0)
                {
                    break;
                }

                int cb = body.IndexOf('}', ob);
                if (cb < 0)
                {
                    break;
                }

                string entry = body.Substring(ob, cb - ob + 1);
                int spiritId = JsonInt(entry, "spiritId", 0);
                int level = JsonInt(entry, "level", 0);
                if (spiritId > 0)
                {
                    list.Add(new FightSpiritSlot { SpiritId = spiritId, Level = level });
                }

                pos = cb + 1;
            }

            if (list.Count > 0)
            {
                Profile.FightSpirits = list;
                Profile.EnsureFightSpirits();
            }
        }

        void ParseMagicStonesFromServer(string json)
        {
            int idx = json.IndexOf("\"magicStones\":[", System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return;
            }

            int start = idx + 14;
            int end = json.IndexOf(']', start);
            if (end <= start)
            {
                return;
            }

            var list = new System.Collections.Generic.List<MagicStoneSlot>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0)
                {
                    break;
                }

                int cb = body.IndexOf('}', ob);
                if (cb < 0)
                {
                    break;
                }

                string entry = body.Substring(ob, cb - ob + 1);
                int templateId = JsonInt(entry, "templateId", 0);
                int level = JsonInt(entry, "level", 0);
                if (templateId > 0)
                {
                    list.Add(new MagicStoneSlot { TemplateId = templateId, Level = level });
                }

                pos = cb + 1;
            }

            if (list.Count > 0)
            {
                Profile.MagicStones = list;
                Profile.EnsureMagicStones();
            }
        }


        void ParseEmblemsFromServer(string json)
        {
            int idx = json.IndexOf("\"emblems\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 10;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            var list = new System.Collections.Generic.List<EmblemSlot>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = body.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = body.Substring(ob, cb - ob + 1);
                list.Add(new EmblemSlot { Id = JsonInt(entry, "id", 0), TemplateId = JsonInt(entry, "templateId", 0), Types = JsonInt(entry, "types", 0), Profile = JsonInt(entry, "profile", 0), MainType = JsonInt(entry, "mainType", 0), MainValue = JsonInt(entry, "mainValue", 0), SubValue = JsonInt(entry, "subValue", 0), SkillId = JsonInt(entry, "skillId", 0), Equipped = JsonInt(entry, "equipped", 0) });
                pos = cb + 1;
            }
            if (list.Count > 0) { Profile.Emblems = list; Profile.EnsureEmblems(); }
        }

        void ParseSoulStampsFromServer(string json)
        {
            int idx = json.IndexOf("\"soulStamps\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 13;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            var list = new System.Collections.Generic.List<SoulStampSlot>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = body.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = body.Substring(ob, cb - ob + 1);
                list.Add(new SoulStampSlot { Id = JsonInt(entry, "id", 0), TempId = JsonInt(entry, "tempId", 0), Type = JsonInt(entry, "type", 0), Quality = JsonInt(entry, "quality", 0), Grade = JsonInt(entry, "grade", 0), ProType = JsonInt(entry, "proType", 0), ProValue = JsonInt(entry, "proValue", 0), SkillId = JsonInt(entry, "skillId", 0), Equipped = JsonInt(entry, "equipped", 0) });
                pos = cb + 1;
            }
            if (list.Count > 0) { Profile.SoulStamps = list; Profile.EnsureSoulStamps(); }
        }

        void ParseWardrobeFromServer(string json)
        {
            int idx = json.IndexOf("\"wardrobeProperties\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 21; int end = json.IndexOf(']', start);
            if (end <= start) return;
            var list = new System.Collections.Generic.List<int>();
            string body = json.Substring(start, end - start);
            int pos = 0;
            while (pos < body.Length)
            {
                while (pos < body.Length && (body[pos] == ' ' || body[pos] == ',')) pos++;
                int ns = pos;
                while (pos < body.Length && body[pos] >= '0' && body[pos] <= '9') pos++;
                if (pos > ns && int.TryParse(body.Substring(ns, pos - ns), out int id) && id > 0) list.Add(id);
            }
            Profile.WardrobeProperties = list; Profile.EnsureWardrobeProperties();
        }

        void ParseHonorSystemFromServer(string json)
        {
            int idx = json.IndexOf("\"honorSystemClaimed\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 21; int end = json.IndexOf(']', start);
            if (end <= start) return;
            var list = new System.Collections.Generic.List<int>();
            string body = json.Substring(start, end - start);
            int pos = 0;
            while (pos < body.Length)
            {
                while (pos < body.Length && (body[pos] == ' ' || body[pos] == ',')) pos++;
                int ns = pos;
                while (pos < body.Length && body[pos] >= '0' && body[pos] <= '9') pos++;
                if (pos > ns && int.TryParse(body.Substring(ns, pos - ns), out int lv) && lv > 0) list.Add(lv);
            }
            Profile.HonorSystemClaimed = list;
        }

        void ParseGodCardsFromServer(string json)
        {
            int idx = json.IndexOf("\"godCards\":[", System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return;
            }

            int start = idx + 11;
            int end = json.IndexOf(']', start);
            if (end <= start)
            {
                return;
            }

            var list = new System.Collections.Generic.List<GodCardSlot>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0)
                {
                    break;
                }

                int cb = body.IndexOf('}', ob);
                if (cb < 0)
                {
                    break;
                }

                string entry = body.Substring(ob, cb - ob + 1);
                int id = JsonInt(entry, "id", 0);
                int count = JsonInt(entry, "count", 1);
                int grooveLevel = JsonInt(entry, "grooveLevel", 0);
                int grooveExp = JsonInt(entry, "grooveExp", 0);
                if (id > 0)
                {
                    list.Add(new GodCardSlot { Id = id, Count = count, GrooveLevel = grooveLevel, GrooveExp = grooveExp });
                }

                pos = cb + 1;
            }

            if (list.Count > 0)
            {
                Profile.GodCards = list;
            }
        }

        void ParseGodCardPointClaimedFromServer(string json)
        {
            int idx = json.IndexOf("\"godCardPointClaimed\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 22;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            var list = new System.Collections.Generic.List<int>();
            string body = json.Substring(start, end - start);
            int pos = 0;
            while (pos < body.Length)
            {
                while (pos < body.Length && (body[pos] == ' ' || body[pos] == ',')) pos++;
                int ns = pos;
                while (pos < body.Length && body[pos] >= '0' && body[pos] <= '9') pos++;
                if (pos > ns && int.TryParse(body.Substring(ns, pos - ns), out int rid) && rid > 0) list.Add(rid);
            }
            Profile.GodCardPointClaimed = list;
        }

        void ParseStockFromServer(string json)
        {
            int idx = json.IndexOf("\"stockHoldings\":[", System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return;
            }

            int start = idx + 16;
            int end = json.IndexOf(']', start);
            if (end <= start)
            {
                return;
            }

            var list = new System.Collections.Generic.List<StockSlot>();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0)
                {
                    break;
                }

                int cb = body.IndexOf('}', ob);
                if (cb < 0)
                {
                    break;
                }

                string entry = body.Substring(ob, cb - ob + 1);
                int stockId = JsonInt(entry, "stockId", 0);
                int shares = JsonInt(entry, "shares", 0);
                int avg = JsonInt(entry, "avgPrice", 0);
                if (stockId > 0 && shares > 0)
                {
                    list.Add(new StockSlot { StockId = stockId, Shares = shares, AvgPrice = avg });
                }

                pos = cb + 1;
            }

            Profile.StockHoldings = list;
        }

        void ParseBagFromServer(string json)
        {
            int bagIdx = json.IndexOf("\"bag\":[", System.StringComparison.Ordinal);
            if (bagIdx < 0) return;
            int start = bagIdx + 6;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            string bagStr = json.Substring(start, end - start);
            var newBag = new System.Collections.Generic.List<BagItem>();
            int pos = 0;
            while (pos < bagStr.Length)
            {
                int ob = bagStr.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = bagStr.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = bagStr.Substring(ob, cb - ob + 1);
                int t = JsonInt(entry, "t", 0);
                int c = JsonInt(entry, "c", 1);
                int s = JsonInt(entry, "s", 0);
                if (t > 0) newBag.Add(new BagItem { TemplateId = t, Count = c, Strengthen = s });
                pos = cb + 1;
            }
            if (newBag.Count > 0) Profile.Bag = newBag;
        }

        public static int JsonInt(string json, string key, int fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (i < 0) return fallback;
            string raw = json.Substring(i + needle.Length).TrimStart().Split(',', '}', ' ', '"')[0];
            return int.TryParse(raw, out int n) ? n : fallback;
        }

        public static string JsonStr(string json, string key, string fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":\"";
            int i = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (i < 0) return fallback;
            int s = i + needle.Length;
            int e = json.IndexOf('"', s);
            return e > s ? json.Substring(s, e - s) : fallback;
        }
    }
}
