using GunMobile.Core;
using GunMobile.Logic;
using GunMobile.Res;
using GunMobile.UI;
using UnityEngine;

namespace GunMobile
{
    /// <summary>
    /// Drop on an empty scene to load sample configs, spin up mobile HUD, and print a dry-run shot.
    /// Point ResLoader.PersistentRoot at the unpacked Ok dump (Flash/, Resource/, Request/).
    /// </summary>
    public sealed class GunMobileBootstrap : MonoBehaviour
    {
        [SerializeField] bool _buildHud = true;
        [SerializeField] int _demoMapId = 1056;
        [SerializeField] float _demoAngle = 55f;
        [SerializeField] float _demoPower = 70f;

        public ResLoader Loader { get; private set; }
        public FlashConfig Config { get; private set; }
        public MapCollision Map { get; private set; }
        public BattleLoop Battle { get; private set; }

        void Start()
        {
            Loader = new ResLoader();
            TryLoadConfig();
            TryLoadMap();
            Battle = new BattleLoop();
            Battle.Reset(new[]
            {
                new LivingStats { Attack = 120, Defence = 80, Luck = 40, Hp = 1000, MaxHp = 1000, Team = 1 },
                new LivingStats { Attack = 110, Defence = 90, Luck = 20, Hp = 1000, MaxHp = 1000, Team = 2 }
            });

            if (_buildHud)
            {
                Canvas canvas = MobileUiBootstrap.CreateRoot();
                Transform safe = canvas.transform.Find("SafeArea");
                MobileUiBootstrap.CreateHudLayer(safe, "MovePad", TextAnchor.LowerLeft, MobileUiBootstrap.FingerButtonSize * 3f)
                    .gameObject.AddComponent<TouchMoveController>();
                MobileUiBootstrap.CreateHudLayer(safe, "AimPad", TextAnchor.LowerRight, MobileUiBootstrap.FingerButtonSize * 3.4f)
                    .gameObject.AddComponent<TouchAimController>();
            }

            DryRunShot();
        }

        void TryLoadConfig()
        {
            if (!Loader.TryReadBytes(GamePaths.ConfigXml, out byte[] bytes) &&
                !Loader.TryReadBytes("Config/config.xml", out bytes))
            {
                Debug.LogWarning("GunMobile: config.xml not in StreamingAssets yet.");
                return;
            }

            Config = FlashConfig.Load(ZlibXml.Load(bytes));
            Debug.Log($"GunMobile language={Config.Language} request={Config.RequestPath}");
        }

        void TryLoadMap()
        {
            string relative = GamePaths.PathCombine("Maps", _demoMapId.ToString(), "fore.map");
            if (!Loader.TryReadBytes(relative, out byte[] bytes) &&
                !Loader.TryReadBytes(GamePaths.MapCollision(_demoMapId), out bytes))
            {
                return;
            }

            Map = MapCollision.Load(bytes);
            Debug.Log($"GunMobile map {_demoMapId} {Map.Width}x{Map.Height}");
        }

        void DryRunShot()
        {
            var sim = new ProjectileSimulator();
            ProjectileState shot = sim.Launch(120f, 800f, _demoAngle, _demoPower, 1);
            shot = sim.FlyUntil(
                shot,
                Battle.Wind,
                (x, y) => Map != null && Map.IsSolid(Mathf.RoundToInt(x), Map.Height - Mathf.RoundToInt(y)),
                (x, y) => Map != null && (x < -40f || x > Map.Width + 40f || y < -40f));
            Debug.Log($"GunMobile dry-run impact ({shot.X:F1},{shot.Y:F1}) t={shot.Time:F2}s wind={Battle.Wind}");
        }
    }
}
