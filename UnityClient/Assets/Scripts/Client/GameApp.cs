using System.Collections;
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
                case "godcard":
                    GodCardScreen.Show(_safe, this);
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
                case "title":
                    TitleScreen.Show(_safe, this);
                    return;
                case "totem":
                    TotemScreen.Show(_safe, this);
                    return;
                case "horse":
                    MountScreen.Show(_safe, this);
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
                    case PhoneMsg.StockResult:
                    case PhoneMsg.StrengthenResult:
                    case PhoneMsg.GuildResult:
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
                            Profile.ChatLog.Add(from + ": " + cm);
                            if (Profile.ChatLog.Count > 100) Profile.ChatLog.RemoveAt(0);
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
            Profile.GodCardEquipId = JsonInt(json, "godCardEquipId", Profile.GodCardEquipId);
            Profile.EngraveSetId = JsonInt(json, "engraveSetId", Profile.EngraveSetId);
            string consortia = JsonStr(json, "consortiaName", null);
            if (consortia != null) Profile.ConsortiaName = consortia;
            ParseBagFromServer(json);
            ParseGodCardsFromServer(json);
            ParseStockFromServer(json);
            ParseFriendsFromServer(json);
            ParseFightSpiritsFromServer(json);
            ParseMagicStonesFromServer(json);
            Profile.Save();
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
                if (id > 0)
                {
                    list.Add(new GodCardSlot { Id = id, Count = count });
                }

                pos = cb + 1;
            }

            if (list.Count > 0)
            {
                Profile.GodCards = list;
            }
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
