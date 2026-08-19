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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBoot()
        {
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
                case "calendar":
                    SignInScreen.Show(_safe, this);
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
                default:
                    DataBrowserScreen.Show(_safe, this, module);
                    return;
            }
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
                        ApplyProfileFromServer(msg.Json);
                        break;
                    case PhoneMsg.ShopResult:
                    case PhoneMsg.EquipResult:
                    case PhoneMsg.QuestResult:
                    case PhoneMsg.StatResult:
                    case PhoneMsg.SignInResult:
                    case PhoneMsg.LotteryResult:
                    case PhoneMsg.StrengthenResult:
                    case PhoneMsg.GuildResult:
                    case PhoneMsg.FriendResult:
                    case PhoneMsg.MailResult:
                        break;
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
                }
            }
        }

        void PumpFight()
        {
            if (PhoneNet.Fight == null || State == AppState.Battle) return;
            while (PhoneNet.Fight.TryDequeue(out var msg))
            {
                if (msg.Id == PhoneMsg.FightStart)
                {
                    int mapId = JsonInt(msg.Json, "map", 1056);
                    int seed = JsonInt(msg.Json, "seed", 0);
                    if (seed != 0) PhoneNet.BattleSeed = seed;
                    PhoneNet.NetBattle = true;
                    // Keep PhoneNet.Seat already set by RoomScreen (host=0, join=1).
                    ShowBattle(mapId, 0, msg.Json);
                }
            }
        }

        void ApplyProfileFromServer(string json)
        {
            if (string.IsNullOrEmpty(json) || Profile == null) return;
            Profile.Gold = JsonInt(json, "gold", Profile.Gold);
            Profile.Gift = JsonInt(json, "gift", Profile.Gift);
            Profile.Level = JsonInt(json, "level", Profile.Level);
            Profile.Attack = JsonInt(json, "attack", Profile.Attack);
            Profile.Defence = JsonInt(json, "defence", Profile.Defence);
            Profile.Agility = JsonInt(json, "agility", Profile.Agility);
            Profile.Luck = JsonInt(json, "luck", Profile.Luck);
            Profile.Hp = JsonInt(json, "hp", Profile.Hp);
            Profile.Win = JsonInt(json, "win", Profile.Win);
            Profile.Lose = JsonInt(json, "lose", Profile.Lose);
            Profile.WeaponId = JsonInt(json, "weaponId", Profile.WeaponId);
            Profile.EquipHead = JsonInt(json, "equipHead", Profile.EquipHead);
            Profile.EquipCloth = JsonInt(json, "equipCloth", Profile.EquipCloth);
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
            string consortia = JsonStr(json, "consortiaName", null);
            if (consortia != null) Profile.ConsortiaName = consortia;
            ParseBagFromServer(json);
            Profile.Save();
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

        static int JsonInt(string json, string key, int fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (i < 0) return fallback;
            string raw = json.Substring(i + needle.Length).TrimStart().Split(',', '}', ' ', '"')[0];
            return int.TryParse(raw, out int n) ? n : fallback;
        }

        static string JsonStr(string json, string key, string fallback)
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
