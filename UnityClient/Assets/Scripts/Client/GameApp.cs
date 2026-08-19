using System.Collections;
using GunMobile.Core;
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

        public void ShowBattle(int mapId)
        {
            Profile.MapId = mapId;
            Profile.Save();
            State = AppState.Battle;
            BattleRuntime.Show(_safe, this, mapId);
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
                case "mail":
                    MailScreen.Show(_safe, this, "邮件", "Offline client — no Road mailbox. Rewards from 签到 / 任务.");
                    return;
                case "friend":
                case "im":
                    MailScreen.Show(_safe, this, module.Title, "Online Road socket is not wired. Chat/friends stay on the PC server.");
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
    }
}
