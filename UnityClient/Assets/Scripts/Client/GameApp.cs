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
            PhoneNet.Boot();
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
            State = AppState.Hall;
            HallScreen.Show(_safe, this);
        }

        public void ShowRoom()
        {
            State = AppState.Room;
            RoomScreen.Show(_safe, this);
        }

        public void ShowBattle(int mapId, int npcId = 0)
        {
            Profile.MapId = mapId;
            Profile.Save();
            State = AppState.Battle;
            BattleRuntime.Show(_safe, this, mapId, npcId);
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
            if (PhoneNet.Fight == null || State == AppState.Battle)
            {
                return;
            }

            while (PhoneNet.Fight.TryDequeue(out var msg))
            {
                if (msg.Id != PhoneMsg.FightStart)
                {
                    continue;
                }

                int mapId = JsonInt(msg.Json, "map", 1056);
                int seed = JsonInt(msg.Json, "seed", 0);
                if (seed != 0)
                {
                    PhoneNet.BattleSeed = seed;
                }

                PhoneNet.NetBattle = true;
                PhoneNet.Seat = 1;
                ShowBattle(mapId);
            }
        }

        static int JsonInt(string json, string key, int fallback)
        {
            if (string.IsNullOrEmpty(json))
            {
                return fallback;
            }

            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (i < 0)
            {
                return fallback;
            }

            string raw = json.Substring(i + needle.Length).TrimStart().Split(',', '}', ' ')[0];
            return int.TryParse(raw, out int n) ? n : fallback;
        }
    }
}
