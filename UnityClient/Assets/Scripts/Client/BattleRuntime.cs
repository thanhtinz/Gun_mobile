using System.Collections;
using System.Collections.Generic;
using GunMobile.Core;
using GunMobile.Logic;
using GunMobile.Net;
using GunMobile.Res;
using GunMobile.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GunMobile.Client
{
    public static class DataBrowserScreen
    {
        public static void Show(RectTransform safe, GameApp app, ModuleDef module)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.PcPanel(safe, "Module");
            var back = UiKit.Button(bg.transform, "Back", "← 大厅", app.ShowHall, new Vector2(160f, 56f));
            back.GetComponent<RectTransform>().anchorMin = back.GetComponent<RectTransform>().anchorMax = new Vector2(0.08f, 0.93f);

            UiKit.Label(bg.transform, "Title", module.Title, 34, new Color(1f, 0.9f, 0.5f), TextAnchor.MiddleCenter)
                .rectTransform.anchorMin = new Vector2(0.2f, 0.88f);
            bg.transform.Find("Title").GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.98f);
            bg.transform.Find("Title").GetComponent<RectTransform>().offsetMin = Vector2.zero;
            bg.transform.Find("Title").GetComponent<RectTransform>().offsetMax = Vector2.zero;

            var scroll = UiKit.Scroll(bg.transform, "Rows");
            var srt = scroll.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.03f, 0.04f);
            srt.anchorMax = new Vector2(0.97f, 0.86f);

            if (string.IsNullOrEmpty(module.TablePath) || !app.Loader.TryReadBytes(module.TablePath, out byte[] bytes))
            {
                AddLine(scroll.content, "PC table not packed, or this module is UI-only in Flash. Data still lives in the Ok dump.");
                return;
            }

            try
            {
                if (module.TablePath.EndsWith(".ui"))
                {
                    var views = PackedMornUi.Parse(bytes);
                    AddLine(scroll.content, $"Morn views: {views.Count}");
                    foreach (var v in views)
                    {
                        AddLine(scroll.content, $"{v.Name}  {v.Width}x{v.Height}");
                    }

                    return;
                }

                var table = XmlResultTable.LoadBytes(bytes);
                AddLine(scroll.content, $"{table.RowName}  ok={table.Ok}  rows={table.Rows.Count}  {table.Message}");
                int limit = Mathf.Min(table.Rows.Count, 250);
                for (int i = 0; i < limit; i++)
                {
                    AddLine(scroll.content, FormatRow(table.Rows[i]));
                }

                if (table.Rows.Count > limit)
                {
                    AddLine(scroll.content, $"… {table.Rows.Count - limit} more (same as PC XML)");
                }
            }
            catch (System.Exception e)
            {
                AddLine(scroll.content, e.Message);
            }
        }

        static void AddLine(Transform parent, string text)
        {
            var label = UiKit.Label(parent, "row", text, 22, Color.white);
            var le = label.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 44f;
            le.flexibleWidth = 1f;
        }

        static string FormatRow(System.Collections.Generic.IReadOnlyDictionary<string, string> row)
        {
            var parts = new System.Collections.Generic.List<string>();
            foreach (var kv in row)
            {
                string v = kv.Value ?? "";
                if (v.Length > 40)
                {
                    v = v.Substring(0, 40) + "…";
                }

                parts.Add(kv.Key + "=" + v);
                if (parts.Count >= 6)
                {
                    break;
                }
            }

            return string.Join("  |  ", parts);
        }
    }

    public static class BattleRuntime
    {
        public static void Show(RectTransform safe, GameApp app, int mapId, int npcId = 0, string fightStartJson = null)
        {
            var host = safe.GetComponent<BattleHost>();
            if (host == null)
            {
                host = safe.gameObject.AddComponent<BattleHost>();
            }

            host.Run(app, mapId, npcId, fightStartJson);
        }
    }

    public sealed class BattleHost : MonoBehaviour
    {
        struct SeatLook
        {
            public int Sex;
            public int Level;
            public int EquipHead;
            public int EquipHair;
            public int EquipFace;
            public int EquipCloth;
            public int EquipGlass;
            public int EquipWeapon;
            public int PetId;
            public int TitleId;
            public int NpcId;
        }

        GameApp _app;
        string _fightStartJson;
        MapCollision _map;
        Texture2D _foreTex;
        RawImage _fore;
        RawImage _back;
        RectTransform _world;
        Text _hud;
        TouchAimController _aim;
        TouchMoveController _move;
        ProjectileSimulator _sim = new ProjectileSimulator();
        BattleLoop _loop = new BattleLoop();
        BallPhysics _ball = BallPhysics.Default;
        BallPhysics[] _ballsByLiving;
        Vector2[] _pos;
        int[] _facing;
        ProjectileState _shot;
        bool _flying;
        int _shotRemaining;
        bool _shotFromNet;
        bool _botQueued;
        float _botDelay;
        int _mapId;
        int _npcId;
        string _foeName = "Bot";
        string[] _playerNames;
        SeatLook[] _seatLooks;
        RawImage[] _petImgs;
        RawImage[] _titleImgs;
        int _serverRewardGold;
        bool _serverRewardWin;
        int _serverQuestGold;
        bool _serverRewardReady;
        bool _pendingMatchOver;
        int _propAvailableMask;
        Dictionary<int, Button> _propButtons = new Dictionary<int, Button>();
        SpriteSheet _livingSheet;
        List<SheetFrame> _walkFrames = new List<SheetFrame>();
        List<SheetFrame> _atkFrames = new List<SheetFrame>();
        RawImage[] _livingImg;
        RawImage[] _hpFill;
        RawImage _shotImg;
        RawImage[] _dots;
        Texture2D _craterTex;
        Texture2D _npcSprite;
        Texture2D _blastTex;
        RawImage _blastImg;
        float _blastT;
        float _animT;
        bool _resultOpen;
        List<DmgPopup> _dmgPopups = new List<DmgPopup>();
        int _propId;
        float _propPower;
        float _propDmg = 1f;
        float _propRadius = 1f;
        bool _propCrit;
        float _walkSendT;
        int _lastWalkDir;
        float _nextFightReconnectAt;
        float _battleStartTime;
        int[] _seatPetIds;
        int[] _weaponIds;
        int[] _preferredBallIds;
        int _lastShooter;
        bool _specialNextShot;

        public void Run(GameApp app, int mapId, int npcId = 0, string fightStartJson = null)
        {
            _app = app;
            _mapId = mapId;
            _npcId = npcId;
            _fightStartJson = fightStartJson;
            _resultOpen = false;
            _serverRewardGold = 0;
            _serverRewardWin = false;
            _serverQuestGold = 0;
            _serverRewardReady = false;
            _pendingMatchOver = false;
            ClearProp();
            StopAllCoroutines();
            UiKit.ClearChildren(app.transform.Find("GunMobileCanvas/SafeArea") ?? transform);
            StartCoroutine(LoadAndPlay());
        }

        IEnumerator LoadAndPlay()
        {
            RectTransform safe = transform as RectTransform;
            UiKit.ClearChildren(safe);
            var root = UiKit.Panel(safe, "Battle", Color.black);

            _world = new GameObject("World", typeof(RectTransform)).GetComponent<RectTransform>();
            _world.SetParent(root.transform, false);
            UiKit.Stretch(_world.gameObject);

            if (!TryLoadMap())
            {
                UiKit.Label(root.transform, "err", "Missing map " + _mapId, 32, Color.red, TextAnchor.MiddleCenter);
                UiKit.Stretch(root.transform.Find("err").gameObject);
                yield break;
            }

            BuildWorld(root.transform);
            BuildHud(root.transform);

            int playerCount;
            LivingStats[] allLivings;

            if (PhoneNet.NetBattle && !string.IsNullOrEmpty(_fightStartJson))
            {
                playerCount = JsonInt(_fightStartJson, "playerCount", 2);
                allLivings = new LivingStats[playerCount];
                _playerNames = new string[playerCount];
                for (int i = 0; i < playerCount; i++)
                {
                    string px = "p" + i + "_";
                    _playerNames[i] = JsonStr(_fightStartJson, px + "nick", "P" + (i + 1));
                    allLivings[i] = new LivingStats
                    {
                        Attack = JsonInt(_fightStartJson, px + "atk", 110),
                        Defence = JsonInt(_fightStartJson, px + "def", 85),
                        Agility = JsonInt(_fightStartJson, px + "agi", 70),
                        Luck = JsonInt(_fightStartJson, px + "luck", 40),
                        Hp = JsonInt(_fightStartJson, px + "hp", 1200),
                        MaxHp = JsonInt(_fightStartJson, px + "maxhp", 1200),
                        Team = JsonInt(_fightStartJson, px + "team", (i % 2) + 1)
                    };
                }
                int me = MeSeat();
                if (playerCount == 2)
                {
                    _foeName = _playerNames[1 - me];
                }
                else
                {
                    var foes = new List<string>();
                    int myTeam = allLivings[me].Team;
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (i != me && allLivings[i].Team != myTeam)
                        {
                            foes.Add(_playerNames[i]);
                        }
                    }
                    _foeName = foes.Count > 0 ? string.Join(", ", foes) : "Team";
                }

                _seatLooks = new SeatLook[playerCount];
                for (int i = 0; i < playerCount; i++)
                {
                    string px = "p" + i + "_";
                    _seatLooks[i] = new SeatLook
                    {
                        Sex = JsonInt(_fightStartJson, px + "sex", 1),
                        Level = JsonInt(_fightStartJson, px + "level", 20),
                        EquipHead = JsonInt(_fightStartJson, px + "equipHead", 0),
                        EquipHair = JsonInt(_fightStartJson, px + "equipHair", 0),
                        EquipFace = JsonInt(_fightStartJson, px + "equipFace", 0),
                        EquipCloth = JsonInt(_fightStartJson, px + "equipCloth", 0),
                        EquipGlass = JsonInt(_fightStartJson, px + "equipGlass", 0),
                        EquipWeapon = JsonInt(_fightStartJson, px + "equipWeapon", JsonInt(_fightStartJson, px + "weaponId", 7001)),
                        PetId = JsonInt(_fightStartJson, px + "petId", 0),
                        TitleId = JsonInt(_fightStartJson, px + "titleId", 0),
                        NpcId = JsonInt(_fightStartJson, px + "npcId", 0)
                    };
                }
            }
            else
            {
                playerCount = 2;
                var player = new LivingStats
                {
                    Attack = _app.Profile.Attack,
                    Defence = _app.Profile.Defence,
                    Agility = _app.Profile.Agility,
                    Luck = _app.Profile.Luck,
                    Hp = _app.Profile.Hp,
                    MaxHp = _app.Profile.Hp,
                    Team = 1
                };

                LivingStats bot;
                NpcInfo npc = _npcId != 0 && _app.Database != null ? _app.Database.GetNpc(_npcId) : null;
                if (npc != null)
                {
                    bot = _app.Database.MakeNpcLiving(_npcId);
                    _foeName = npc.Name;
                }
                else
                {
                    bot = new LivingStats
                    {
                        Attack = 110, Defence = 85, Agility = 70, Luck = 40,
                        Hp = 1200, MaxHp = 1200, Team = 2
                    };
                    _foeName = PhoneNet.NetBattle ? "P2" : "Bot";
                }
                allLivings = new[] { player, bot };
            }

            int seed = PhoneNet.NetBattle && PhoneNet.BattleSeed != 0 ? PhoneNet.BattleSeed : 0;
            if (PhoneNet.NetBattle && !string.IsNullOrEmpty(_fightStartJson))
            {
                int serverSeed = JsonInt(_fightStartJson, "seed", seed);
                if (serverSeed != 0) seed = serverSeed;
            }

            float turnSec = 20f;
            if (_app.Config != null && _app.Config.FightTurnSeconds >= 5)
            {
                turnSec = _app.Config.FightTurnSeconds;
            }
            else if (_app.Database != null)
            {
                int dbSec = _app.Database.BattleTurnSeconds();
                if (dbSec >= 5)
                {
                    turnSec = dbSec;
                }
            }

            _loop.Reset(allLivings, turnSec, seed);
            _battleStartTime = Time.time;
            _ballsByLiving = new BallPhysics[playerCount];
            _seatPetIds = new int[playerCount];
            _weaponIds = new int[playerCount];
            _preferredBallIds = new int[playerCount];
            for (int i = 0; i < playerCount; i++) _ballsByLiving[i] = BallPhysics.Default;
            if (_app.Database != null)
            {
                if (PhoneNet.NetBattle && !string.IsNullOrEmpty(_fightStartJson))
                {
                    for (int i = 0; i < playerCount; i++)
                    {
                        string px = "p" + i + "_";
                        _weaponIds[i] = JsonInt(_fightStartJson, px + "weaponId", _app.Profile.WeaponId);
                        _preferredBallIds[i] = JsonInt(_fightStartJson, px + "preferredBallId", 0);
                        _seatPetIds[i] = JsonInt(_fightStartJson, px + "petId", 0);
                        _ballsByLiving[i] = _app.Database.ResolveBall(_weaponIds[i], _preferredBallIds[i]);
                    }
                }
                else
                {
                    _weaponIds[0] = _app.Profile.WeaponId;
                    _preferredBallIds[0] = _app.Profile.PreferredBallId;
                    _seatPetIds[0] = _app.Profile.PetId;
                    _ballsByLiving[0] = _app.Database.ResolveBall(_weaponIds[0], _preferredBallIds[0]);
                    for (int i = 1; i < playerCount; i++)
                    {
                        _weaponIds[i] = 7001;
                        _ballsByLiving[i] = _app.Database.ResolveBall(7001);
                    }
                }
            }

            _ball = _ballsByLiving[0];
            _sim.ApplyBall(_ball);
            _pos = new Vector2[playerCount];
            _facing = new int[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                float frac = playerCount <= 1 ? 0.1f : (float)i / (playerCount - 1);
                _pos[i] = new Vector2(Mathf.Lerp(140f, _map.Width - 160f, frac), 0f);
                _facing[i] = frac < 0.5f ? 1 : -1;
                PlaceOnGround(i);
            }
            BuildActors(root.transform);
            yield return null;
        }

        bool TryLoadMap()
        {
            string col = GamePaths.PathCombine("Service", "Road", "map", _mapId.ToString(), "fore.map");
            if (!_app.Loader.TryReadBytes(col, out byte[] mapBytes))
            {
                return false;
            }

            _map = MapCollision.Load(mapBytes);
            return true;
        }

        void BuildWorld(Transform parent)
        {
            _back = MakeLayer(parent, "Back");
            _fore = MakeLayer(parent, "Fore");
            TryAssign(_back, GamePaths.PathCombine("Resource", "image", "map", _mapId.ToString(), "back.jpg"));
            if (_app.Loader.TryReadBytes(GamePaths.PathCombine("Resource", "image", "map", _mapId.ToString(), "fore.png"), out byte[] foreBytes))
            {
                _foreTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                _foreTex.LoadImage(foreBytes, false);
                _foreTex.filterMode = FilterMode.Point;
                _fore.texture = _foreTex;
            }
        }

        RawImage MakeLayer(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            UiKit.Stretch(go);
            var raw = go.GetComponent<RawImage>();
            raw.raycastTarget = false;
            raw.color = Color.white;
            return raw;
        }

        void TryAssign(RawImage img, string path)
        {
            if (!_app.Loader.TryReadBytes(path, out byte[] bytes))
            {
                return;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            if (tex.LoadImage(bytes))
            {
                img.texture = tex;
            }
        }

        void BuildHud(Transform parent)
        {
            PcSkin.Warm(_app.Loader);
            var bar = new GameObject("Hud", typeof(RectTransform), typeof(Image));
            bar.transform.SetParent(parent, false);
            var rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.86f);
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            PcSkin.Chrome(bar.GetComponent<Image>(), PcSkin.Game, "game_blood_RBg");
            _hud = UiKit.Label(bar.transform, "HudText", "", 24, Color.white, TextAnchor.MiddleLeft);
            UiKit.Stretch(_hud.gameObject).offsetMin = new Vector2(20f, 0f);

            var exit = UiKit.Button(bar.transform, "Exit", "退出", () =>
            {
                _app.ShowHall();
            }, new Vector2(140f, 50f));
            exit.GetComponent<RectTransform>().anchorMin = exit.GetComponent<RectTransform>().anchorMax = new Vector2(0.93f, 0.5f);

            if (PhoneNet.NetBattle)
            {
                var surrender = UiKit.Button(bar.transform, "Surrender", "投降", () =>
                {
                    PhoneNet.Fight?.Send(PhoneMsg.FightSurrender, "{}");
                }, new Vector2(140f, 50f));
                surrender.GetComponent<RectTransform>().anchorMin = surrender.GetComponent<RectTransform>().anchorMax = new Vector2(0.82f, 0.5f);
            }

            if (_app.Database != null && _app.Database.Bombs.TryGetValue(_app.Profile.WeaponId, out BombInfo weaponBomb) &&
                weaponBomb.Special > 0 && weaponBomb.Special != weaponBomb.Common)
            {
                var special = UiKit.Button(bar.transform, "Special", "必杀", () => { _specialNextShot = true; }, new Vector2(140f, 50f));
                special.GetComponent<RectTransform>().anchorMin = special.GetComponent<RectTransform>().anchorMax = new Vector2(0.71f, 0.5f);
            }

            var move = MobileUiBootstrap.CreateHudLayer(parent as RectTransform, "Move", TextAnchor.LowerLeft, MobileUiBootstrap.FingerButtonSize * 3f);
            var moveImg = move.gameObject.AddComponent<Image>();
            PcSkin.Chrome(moveImg, PcSkin.Game, "game_moveStripBgAsset");
            if (moveImg.sprite == null)
            {
                PcSkin.Chrome(moveImg, PcSkin.Game, "game_takeAimAssetBG");
            }
            if (moveImg.sprite == null)
            {
                moveImg.color = new Color(1f, 1f, 1f, 0.12f);
            }

            _move = move.gameObject.AddComponent<TouchMoveController>();

            var aim = MobileUiBootstrap.CreateHudLayer(parent as RectTransform, "Aim", TextAnchor.LowerRight, MobileUiBootstrap.FingerButtonSize * 3.4f);
            var aimImg = aim.gameObject.AddComponent<Image>();
            PcSkin.Chrome(aimImg, PcSkin.Game, "game_takeAimAsset");
            if (aimImg.sprite == null)
            {
                aimImg.color = new Color(1f, 0.8f, 0.2f, 0.16f);
            }

            _aim = aim.gameObject.AddComponent<TouchAimController>();
            BuildPropBar(parent);
        }

        void BuildPropBar(Transform parent)
        {
            if (PcSkin.GameProp == null)
            {
                return;
            }

            var bar = new GameObject("Props", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            bar.transform.SetParent(parent, false);
            var rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.26f, 0.015f);
            rt.anchorMax = new Vector2(0.74f, 0.13f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var layout = bar.GetComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 8f;
            layout.padding = new RectOffset(8, 8, 4, 4);
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;
            int[] ids = { 1, 2, 4, 5, 6, 7 };
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                var btn = UiKit.Button(bar.transform, "prop" + id, "", () => UseProp(id), new Vector2(56f, 56f));
                _propButtons[id] = btn;
                btn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.04f);
                RawImage art = PcSkin.Slice(btn.transform, "Art", PcSkin.GameProp, "game_prop_" + id, true);
                if (art != null)
                {
                    art.raycastTarget = false;
                }
            }

            RefreshPropButtons();
        }

        static int PropBitIndex(int propId)
        {
            // propIds order must match server:
            // [1,2,4,5,6,7] -> bits [0..5]
            switch (propId)
            {
                case 1: return 0;
                case 2: return 1;
                case 4: return 2;
                case 5: return 3;
                case 6: return 4;
                case 7: return 5;
                default: return -1;
            }
        }

        bool IsPropAvailable(int propId)
        {
            if (propId == 0) return true;
            int bit = PropBitIndex(propId);
            if (bit < 0) return false;
            return (_propAvailableMask & (1 << bit)) != 0;
        }

        void RefreshPropButtons()
        {
            // Single-player: always enable.
            if (!PhoneNet.NetBattle)
            {
                foreach (var kv in _propButtons)
                {
                    if (kv.Value == null) continue;
                    kv.Value.interactable = true;
                    kv.Value.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);
                }
                return;
            }

            foreach (var kv in _propButtons)
            {
                int propId = kv.Key;
                Button b = kv.Value;
                if (b == null) continue;
                bool enable = IsPropAvailable(propId);
                b.interactable = enable;
                b.GetComponent<Image>().color = enable
                    ? new Color(1f, 1f, 1f, 0.4f)
                    : new Color(1f, 1f, 1f, 0.04f);
            }
        }

        void UseProp(int id)
        {
            if (_loop == null || _loop.Phase != BattlePhase.Aiming || _loop.CurrentLiving != MeSeat())
            {
                return;
            }

            if (PhoneNet.NetBattle && !IsPropAvailable(id))
            {
                return; // server says this prop isn't available this turn
            }

            _propId = id;
            _propPower = 0f;
            _propDmg = 1f;
            _propRadius = 1f;
            _propCrit = false;
            if (_app?.Database != null)
            {
                _app.Database.ApplyFightProp(id, out _propDmg, out _propRadius, out _propPower, out _propCrit);
                return;
            }

            switch (id)
            {
                case 1:
                    _propDmg = 1.25f;
                    _propRadius = 1.35f;
                    break;
                case 2:
                    _propDmg = 1.2f;
                    break;
                case 5:
                    _propPower = 12f;
                    break;
                case 6:
                    _propDmg = 1.4f;
                    break;
                case 7:
                    _propCrit = true;
                    break;
            }
        }

        void Update()
        {
            if (_map == null || _loop == null || _pos == null)
            {
                return;
            }

            if (!PhoneNet.NetBattle)
            {
                _loop.TickClock(Time.deltaTime);
            }
            else
            {
                _loop.TickClockDisplay(Time.deltaTime);
            }

            PumpNet();
            int cur = _loop.CurrentLiving;
            int me = MeSeat();
            if (_loop.Phase == BattlePhase.Aiming && cur == me && _aim != null)
            {
                int walk = _move != null ? _move.Direction : 0;
                if (walk != 0)
                {
                    _facing[me] = walk;
                    _pos[me].x = Mathf.Clamp(_pos[me].x + walk * 80f * Time.deltaTime, 20f, _map.Width - 20f);
                    PlaceOnGround(me);
                    _aim.SetFacing(_facing[me]);
                }

                if (PhoneNet.NetBattle)
                {
                    _walkSendT -= Time.deltaTime;
                    if (walk != 0 && _walkSendT <= 0f)
                    {
                        _walkSendT = 0.12f;
                        PhoneNet.SendWalk(me, _pos[me].x, _facing[me]);
                    }
                    else if (walk == 0 && _lastWalkDir != 0)
                    {
                        PhoneNet.SendWalk(me, _pos[me].x, _facing[me]);
                    }

                    _lastWalkDir = walk;
                }

                if (_aim.FireReleased)
                {
                    _aim.ConsumeFire();
            Fire(me, _aim.AngleDeg, Mathf.Clamp(_aim.Power + _propPower, 1f, 100f), false);
                }
            }
            else if (!PhoneNet.NetBattle && _loop.Phase == BattlePhase.Aiming && cur == 1 && !_flying && !_botQueued)
            {
                _botQueued = true;
                _botDelay = 0.7f;
            }

            if (_botQueued && _loop.Phase == BattlePhase.Aiming && cur == 1)
            {
                _botDelay -= Time.deltaTime;
                if (_botDelay <= 0f)
                {
                    _botQueued = false;
                    FireBot();
                }
            }

            if (_flying)
            {
                StepShot();
            }

            DrawHud();
            UpdateActors();
            TickDmgPopups();
            TickSuicideTimer();

            if (_loop.Phase == BattlePhase.MatchOver)
            {
                TryFinishMatch();
            }
        }

        void FireBot()
        {
            float bestA = 50f;
            float bestP = 70f;
            float bestD = 1e9f;
            Vector2 target = LivingUnity(_pos[0]);
            for (int a = 28; a <= 72; a += 4)
            {
                for (int p = 35; p <= 95; p += 6)
                {
                    ProjectileState s = _sim.Launch(_pos[1].x, _map.Height - _pos[1].y - 18f, a, p, _facing[1]);
                    s = _sim.FlyUntil(
                        s,
                        _loop.Wind,
                        (x, y) => _map.IsSolid(Mathf.RoundToInt(x), _map.Height - Mathf.RoundToInt(y)),
                        (x, y) => x < -40f || x > _map.Width + 40f || y < -40f,
                        8f);
                    float d = Vector2.Distance(new Vector2(s.X, s.Y), target);
                    if (d < bestD)
                    {
                        bestD = d;
                        bestA = a;
                        bestP = p;
                    }
                }
            }

            Fire(1, bestA, bestP, false);
        }

        public void ApplyNetFire(int who, float angle, float power)
        {
            if (_flying || _map == null)
            {
                return;
            }

            Fire(who, angle, power, true);
        }

        public void ApplyNetWalk(int who, float x, int facing)
        {
            if (_map == null || _pos == null || who < 0 || who >= _pos.Length || who == MeSeat())
            {
                return;
            }

            if (x >= 0f)
            {
                _pos[who].x = Mathf.Clamp(x, 20f, _map.Width - 20f);
            }

            _facing[who] = facing >= 0 ? 1 : -1;
            PlaceOnGround(who);
        }

        Texture2D TryLoadCraterTexture(int craterId)
        {
            string[] craterPaths =
            {
                GamePaths.PathCombine(GamePaths.BombCrater(craterId), "crater.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(craterId), "crater1.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(craterId), "Crater.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(65), "crater1.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(65), "Crater.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(65), "crater.png")
            };

            foreach (string path in craterPaths)
            {
                if (_app.Loader.TryReadBytes(path, out byte[] bytes))
                {
                    var tex = SpriteSheet.LoadTexture(bytes, true);
                    if (tex != null)
                    {
                        return tex;
                    }
                }
            }

            return _craterTex;
        }

        void ApplyShotVisuals()
        {
            if (_shotImg != null)
            {
                Texture2D bullet = PcArt.Bullet(_app.Loader, _ball.Id > 0 ? _ball.Id : _ball.FlyingPartical);
                if (bullet != null)
                {
                    _shotImg.texture = bullet;
                }
            }

            if (_blastImg != null)
            {
                Texture2D blast = PcArt.Blast(_app.Loader, _ball.BombPartical > 0 ? _ball.BombPartical : _ball.Id);
                if (blast != null)
                {
                    _blastImg.texture = blast;
                }
            }

            if (_ball != null)
            {
                int craterId = _ball.Crater > 0 ? _ball.Crater : 65;
                _craterTex = TryLoadCraterTexture(craterId);
            }
        }

        void Fire(int who, float angle, float power, bool fromNet)
        {
            _lastShooter = who;
            _shotFromNet = fromNet;
            _loop.BeginShot();
            _aim?.SetFacing(_facing[who]);
            bool specialShot = false;
            if (_app?.Database != null && _weaponIds != null && who >= 0 && who < _weaponIds.Length)
            {
                int wid = _weaponIds[who];
                int pref = _preferredBallIds != null && who < _preferredBallIds.Length ? _preferredBallIds[who] : 0;
                int propForShot = who == MeSeat() ? _propId : 0;
                specialShot = who == MeSeat() && _specialNextShot;
                _specialNextShot = false;
                _ball = specialShot
                    ? _app.Database.ResolveSpecialBall(wid)
                    : _app.Database.ResolveBallForShot(wid, pref, propForShot);
                if (_ballsByLiving != null && who < _ballsByLiving.Length)
                {
                    _ballsByLiving[who] = _ball;
                }
            }
            else if (_ballsByLiving != null && who >= 0 && who < _ballsByLiving.Length && _ballsByLiving[who] != null)
            {
                _ball = _ballsByLiving[who];
            }

            _sim.ApplyBall(_ball);
            ApplyShotVisuals();
            Vector2 p = _pos[who];
            float unityY = _map.Height - p.y - 18f;
            _shot = _sim.Launch(p.x, unityY, angle, power, _facing[who]);
            _flying = true;
            _shotRemaining = Mathf.Max(0, _ball.Amount - 1);
            if (!fromNet && PhoneNet.NetBattle)
            {
                PhoneNet.SendFire(who, angle, power, _facing[who], _propId, specialShot);
            }
        }

        void PumpNet()
        {
            if (!PhoneNet.NetBattle || PhoneNet.Fight == null)
            {
                return;
            }

            // Lightweight auto-reconnect for online battle.
            // This allows the new server "reconnect grace" logic to be exercised.
            if (!PhoneNet.Fight.Connected)
            {
                if (PhoneNet.PlayerId > 0 && Time.realtimeSinceStartup >= _nextFightReconnectAt)
                {
                    _nextFightReconnectAt = Time.realtimeSinceStartup + 3f;
                    PhoneNet.ConnectFight(PhoneNet.PeerHost);
                }
                return;
            }

            while (PhoneNet.Fight.TryDequeue(out var msg))
            {
                if (msg.Id == PhoneMsg.FightTurn)
                {
                    int turn = JsonInt(msg.Json, "turn", _loop.TurnIndex);
                    int player = JsonInt(msg.Json, "player", _loop.CurrentLiving);
                    float wind = JsonFloat(msg.Json, "wind", _loop.Wind);
                    float timeLeft = JsonFloat(msg.Json, "timeLeft", 20f);
                    // If we were mid-flight during reconnect, still resync when turn index changes.
                    if (_loop.Phase != BattlePhase.Flying || turn != _loop.TurnIndex)
                        _loop.SyncTurn(turn, player, wind, timeLeft);
                    continue;
                }

                if (msg.Id == PhoneMsg.FightProp)
                {
                    int player = JsonInt(msg.Json, "player", _loop.CurrentLiving);
                    int mask = JsonInt(msg.Json, "mask", 0);
                    _propAvailableMask = (player == MeSeat()) ? mask : 0;
                    RefreshPropButtons();
                    continue;
                }

                if (msg.Id == PhoneMsg.FightState)
                {
                    // Server snapshot: HP + position + facing, used for reconnect continuity.
                    int pc = JsonInt(msg.Json, "playerCount", _loop.Livings.Count);
                    int n = Mathf.Min(pc, _loop.Livings.Count);

                    int[] hp = new int[n];
                    int[] maxHp = new int[n];

                    for (int i = 0; i < n; i++)
                    {
                        hp[i] = JsonInt(msg.Json, "p" + i + "_hp", _loop.Livings[i].Hp);
                        maxHp[i] = JsonInt(msg.Json, "p" + i + "_maxhp", _loop.Livings[i].MaxHp);

                        float x = JsonFloat(msg.Json, "p" + i + "_x", _pos[i].x);
                        int facing = JsonInt(msg.Json, "p" + i + "_facing", _facing[i]);

                        if (_pos != null && i >= 0 && i < _pos.Length)
                        {
                            _pos[i].x = x;
                        }
                        if (_facing != null && i >= 0 && i < _facing.Length)
                        {
                            _facing[i] = facing;
                        }
                        if (_map != null && i < _pos.Length)
                        {
                            PlaceOnGround(i);
                        }
                    }

                    _loop.SyncLivingHp(hp, maxHp);
                    _loop.SyncMatchOverIfNeeded();
                    continue;
                }

                if (msg.Id == PhoneMsg.FightCrater)
                {
                    int mx = JsonInt(msg.Json, "x", -1);
                    int my = JsonInt(msg.Json, "y", -1);
                    int radius = JsonInt(msg.Json, "r", 24);
                    if (mx >= 0 && my >= 0)
                    {
                        ApplyNetCrater(mx, my, radius);
                    }
                    continue;
                }

                if (msg.Id == PhoneMsg.FightDamage)
                {
                    int target = JsonInt(msg.Json, "target", -1);
                    int dmg = JsonInt(msg.Json, "dmg", 0);
                    bool crit = JsonInt(msg.Json, "crit", 0) != 0;
                    if (target >= 0 && target < _loop.Livings.Count)
                    {
                        _loop.ApplyDamage(target, dmg);
                        SpawnDmgPopup(_pos[target], dmg, crit);

                        // If this damage ends the match (e.g. surrender/disconnect),
                        // force the battle loop into MatchOver so FinishMatch() runs.
                        if (PhoneNet.NetBattle && _loop.Phase != BattlePhase.MatchOver)
                        {
                            var teams = new HashSet<int>();
                            for (int i = 0; i < _loop.Livings.Count; i++)
                            {
                                if (_loop.Livings[i].Hp > 0)
                                {
                                    teams.Add(_loop.Livings[i].Team);
                                }
                            }
                            if (teams.Count <= 1)
                            {
                                _loop.SyncMatchOverIfNeeded();
                            }
                        }
                    }
                    continue;
                }

                if (msg.Id == PhoneMsg.FightWalk)
                {
                    int walker = JsonInt(msg.Json, "who", 1 - MeSeat());
                    float x = JsonFloat(msg.Json, "x", -1f);
                    int face = JsonInt(msg.Json, "facing", 1);
                    ApplyNetWalk(walker, x, face);
                    continue;
                }

                if (msg.Id == PhoneMsg.FightReward)
                {
                    _serverRewardGold = JsonInt(msg.Json, "gold", 0);
                    _serverQuestGold = JsonInt(msg.Json, "questGold", 0);
                    _serverRewardWin = ParseWinFlag(msg.Json, "win");
                    _serverRewardReady = true;
                    if (_pendingMatchOver || _loop.Phase == BattlePhase.MatchOver)
                    {
                        TryFinishMatch();
                    }
                    continue;
                }

                if (msg.Id != PhoneMsg.FightFire)
                {
                    continue;
                }

                int who = JsonInt(msg.Json, "who", 1 - MeSeat());
                float angle = JsonFloat(msg.Json, "angle", 45f);
                float power = JsonFloat(msg.Json, "power", 50f);
                int facing = JsonInt(msg.Json, "facing", _facing[Mathf.Clamp(who, 0, 1)]);
                if (who >= 0 && who < _facing.Length)
                {
                    _facing[who] = facing >= 0 ? 1 : -1;
                }

                ApplyNetFire(who, angle, power);
            }
        }

        static int JsonInt(string json, string key, int fallback)
        {
            return Mathf.RoundToInt(JsonFloat(json, key, fallback));
        }

        static string JsonStr(string json, string key, string fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":\"";
            int i = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (i < 0) return fallback;
            i += needle.Length;
            int j = json.IndexOf('"', i);
            if (j < 0) return fallback;
            return json.Substring(i, j - i);
        }

        static float JsonFloat(string json, string key, float fallback)
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

            int s = i + needle.Length;
            int e = s;
            while (e < json.Length && (json[e] == '-' || json[e] == '.' || (json[e] >= '0' && json[e] <= '9')))
            {
                e++;
            }

            if (float.TryParse(json.Substring(s, e - s), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float v))
            {
                return v;
            }

            return fallback;
        }

        void StepShot()
        {
            for (int i = 0; i < 2; i++)
            {
                _shot = _sim.StepFrame(_shot, _loop.Wind);
                int mx = Mathf.RoundToInt(_shot.X);
                int my = _map.Height - Mathf.RoundToInt(_shot.Y);
                if (_shot.X < -30 || _shot.X > _map.Width + 30 || _shot.Y < -30)
                {
                    EndShot(false, mx, my);
                    return;
                }

                if (_map.IsSolid(mx, my))
                {
                    EndShot(true, mx, my);
                    return;
                }

                for (int L = 0; L < _pos.Length; L++)
                {
                    if (_loop.Livings[L].Hp <= 0)
                    {
                        continue;
                    }

                    if (Vector2.Distance(new Vector2(_shot.X, _shot.Y), LivingUnity(_pos[L])) < 28f && L != _loop.CurrentLiving)
                    {
                        Hurt(L, 0f);
                        EndShot(true, mx, my);
                        return;
                    }
                }
            }
        }

        Vector2 LivingUnity(Vector2 mapPos)
        {
            return new Vector2(mapPos.x, _map.Height - mapPos.y);
        }

        void ApplyNetCrater(int mx, int my, int radius)
        {
            if (_map == null || radius <= 0) return;

            _map.CutCircle(mx, my, radius);
            StampCrater(mx, my, radius);

            if (_blastImg != null)
            {
                _blastT = 0.35f;
                var rt = _blastImg.rectTransform;
                rt.anchorMin = rt.anchorMax = MapAnchor(mx, my);
                rt.sizeDelta = new Vector2(72f, 72f);
                _blastImg.gameObject.SetActive(true);
            }

            // Re-seat actors after terrain change.
            if (_pos != null)
            {
                for (int i = 0; i < _pos.Length; i++)
                {
                    if (_loop != null && i < _loop.Livings.Count && _loop.Livings[i].Hp > 0)
                    {
                        PlaceOnGround(i);
                    }
                }
            }
        }

        void EndShot(bool explode, int mx, int my)
        {
            int radius = Mathf.Max(24, Mathf.RoundToInt((_ball.Radii > 0 ? _ball.Radii / 2 : 38) * _propRadius));
            if (explode)
            {
                if (!PhoneNet.NetBattle)
                {
                    _map.CutCircle(mx, my, radius);
                    StampCrater(mx, my, radius);
                    for (int L = 0; L < _pos.Length; L++)
                    {
                        float d = Vector2.Distance(new Vector2(mx, my), _pos[L]);
                        if (d < radius * 2.2f && _loop.Livings[L].Hp > 0)
                        {
                            Hurt(L, d);
                        }
                    }
                }
            }

            if (explode && _blastImg != null && !PhoneNet.NetBattle)
            {
                _blastT = 0.35f;
                var rt = _blastImg.rectTransform;
                rt.anchorMin = rt.anchorMax = MapAnchor(mx, my);
                rt.sizeDelta = new Vector2(72f, 72f);
                _blastImg.gameObject.SetActive(true);
            }

            if (_shotRemaining > 0)
            {
                _shotRemaining--;
                float spread = UnityEngine.Random.Range(-8f, 8f);
                _shot = _sim.Launch(
                    _pos[_loop.CurrentLiving].x + spread,
                    _map.Height - _pos[_loop.CurrentLiving].y - 18f,
                    _aim != null ? _aim.AngleDeg + UnityEngine.Random.Range(-5f, 5f) : 50f,
                    Mathf.Clamp((_aim != null ? _aim.Power : 60f) + UnityEngine.Random.Range(-6f, 6f), 20f, 100f),
                    _facing[_loop.CurrentLiving]);
                return;
            }

            _flying = false;
            _loop.EndShot();
            if (PhoneNet.NetBattle)
            {
                // Server advances turn after fire; do NOT advance locally (prevents desync).
                _loop.FinishSettleOnline();
                PhoneNet.SendFightTurn(_loop.TurnIndex, _loop.CurrentLiving, _loop.Wind);
            }
            else
            {
                _loop.FinishSettle();
            }

            if (!PhoneNet.NetBattle)
            {
                TryPetFollowUp(_lastShooter);
            }

            if (_loop.Phase == BattlePhase.MatchOver)
            {
                TryFinishMatch();
            }

            ClearProp();
        }

        void TryFinishMatch()
        {
            if (_resultOpen || _loop == null || _loop.Phase != BattlePhase.MatchOver)
            {
                return;
            }

            if (PhoneNet.NetBattle && !_serverRewardReady)
            {
                _pendingMatchOver = true;
                PhoneNet.ReportFightOver(_loop.WouldTeamWin(MeSeat()));
                return;
            }

            FinishMatch();
        }

        static bool ParseWinFlag(string json, string key)
        {
            if (string.IsNullOrEmpty(json)) return false;

            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (i < 0) return false;

            int s = i + needle.Length;
            while (s < json.Length && char.IsWhiteSpace(json[s])) s++;

            if (s >= json.Length) return false;
            if (json[s] == 't' || json[s] == 'T') return true;
            if (json[s] == 'f' || json[s] == 'F') return false;

            int e = s;
            while (e < json.Length && (json[e] == '-' || json[e] == '.' || (json[e] >= '0' && json[e] <= '9')))
            {
                e++;
            }

            if (int.TryParse(json.Substring(s, e - s), out int n))
            {
                return n != 0;
            }

            return false;
        }

        void ClearProp()
        {
            _propId = 0;
            _propPower = 0f;
            _propDmg = 1f;
            _propRadius = 1f;
            _propCrit = false;
        }

        void TickSuicideTimer()
        {
            int suicideSec = _app?.Config?.SuicideTime ?? 120;
            if (suicideSec <= 0 || _loop == null || _loop.Phase == BattlePhase.MatchOver)
            {
                return;
            }

            if (Time.time - _battleStartTime < suicideSec)
            {
                return;
            }

            if (PhoneNet.NetBattle)
            {
                PhoneNet.Fight?.Send(PhoneMsg.FightSurrender, "{}");
            }
            else
            {
                int me = MeSeat();
                if (me >= 0 && me < _loop.Livings.Count)
                {
                    _loop.ApplyDamage(me, _loop.Livings[me].Hp);
                }

                _loop.SyncMatchOverIfNeeded();
                if (_loop.Phase != BattlePhase.MatchOver)
                {
                    _loop.EndMatchTimeout();
                }
            }
        }

        void TryPetFollowUp(int shooter)
        {
            if (_app?.Database == null || _seatPetIds == null || shooter < 0 || shooter >= _seatPetIds.Length)
            {
                return;
            }

            int petId = _seatPetIds[shooter];
            PetSkillInfo skill = _app.Database.ResolvePetPassiveSkill(petId);
            if (skill == null || skill.BallType != 3 || skill.DamagePercent <= 0)
            {
                return;
            }

            if (!_app.Database.RollPetSkill(skill, shooter + _loop.TurnIndex))
            {
                return;
            }

            int src = shooter;
            int bombHurt = _app.Database.ComputeBombHurt(_ball, _propDmg);
            bombHurt = Mathf.Max(1, Mathf.RoundToInt(bombHurt * skill.DamagePercent / 100f));

            for (int t = 0; t < _loop.Livings.Count; t++)
            {
                if (t == src || _loop.Livings[t].Hp <= 0)
                {
                    continue;
                }

                if (_loop.Livings[src].Team == _loop.Livings[t].Team)
                {
                    continue;
                }

                float dist = Vector2.Distance(_pos[src], _pos[t]);
                int dmg = DamageCalculator.Compute(_loop.Livings[src], _loop.Livings[t], bombHurt, dist * 0.35f, false);
                _loop.ApplyDamage(t, dmg);
                SpawnDmgPopup(_pos[t], dmg, false);
            }
        }

        void Hurt(int index, float dist)
        {
            if (PhoneNet.NetBattle)
            {
                // Server-authoritative: don't compute damage locally.
                // Server will broadcast FightDamage with correct values.
                return;
            }

            int src = _loop.CurrentLiving;
            int bombHurt = _app?.Database != null
                ? _app.Database.ComputeBombHurt(_ball, _propDmg)
                : DamageCalculator.ComputeBombHurt(_ball, _propDmg);
            bool crit = _propCrit || DamageCalculator.RollCrit(_loop.Livings[src].Luck, src + _loop.TurnIndex);
            bool armorPierce = _app?.Database != null && _app.Database.PropIgnoresArmour(_propId);
            int dmg = DamageCalculator.Compute(_loop.Livings[src], _loop.Livings[index], bombHurt, dist, crit, armorPierce);
            _loop.ApplyDamage(index, dmg);
            SpawnDmgPopup(_pos[index], dmg, crit);
        }

        struct DmgPopup { public Text Label; public float T; }

        void SpawnDmgPopup(Vector2 mapPos, int dmg, bool crit)
        {
            var go = new GameObject("Dmg", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(_world, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = MapAnchor(mapPos.x, mapPos.y - 30f);
            rt.sizeDelta = new Vector2(140f, 50f);
            rt.pivot = new Vector2(0.5f, 0f);
            var txt = go.GetComponent<Text>();
            txt.font = UiKit.Font;
            txt.fontSize = crit ? 38 : 30;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = crit ? new Color(1f, 0.2f, 0.1f) : new Color(1f, 0.9f, 0.3f);
            txt.text = (crit ? "暴击 " : "-") + dmg;
            txt.raycastTarget = false;
            _dmgPopups.Add(new DmgPopup { Label = txt, T = 1.2f });
        }

        void TickDmgPopups()
        {
            for (int i = _dmgPopups.Count - 1; i >= 0; i--)
            {
                var p = _dmgPopups[i];
                p.T -= Time.deltaTime;
                if (p.T <= 0f)
                {
                    Destroy(p.Label.gameObject);
                    _dmgPopups.RemoveAt(i);
                    continue;
                }

                _dmgPopups[i] = p;
                var rt = p.Label.rectTransform;
                rt.anchoredPosition += new Vector2(0f, 60f * Time.deltaTime);
                p.Label.color = new Color(p.Label.color.r, p.Label.color.g, p.Label.color.b, Mathf.Clamp01(p.T / 0.4f));
            }
        }

        void FinishMatch()
        {
            if (_resultOpen)
            {
                return;
            }

            _resultOpen = true;
            bool net = PhoneNet.NetBattle;
            bool win = net ? _serverRewardWin : (_loop.Livings[MeSeat()].Hp > 0);

            if (net)
            {
                PhoneNet.ReportFightOver(win);
            }

            int gold;
            int questGold = 0;
            if (net)
            {
                gold = _serverRewardGold;
                questGold = _serverQuestGold;
            }
            else if (_app.Database != null)
            {
                gold = win ? _app.Database.BattleWinGold() : _app.Database.BattleLoseGold();
                if (win && _npcId != 0)
                {
                    gold += _app.Database.ComputePveWinGold(_npcId, _app.Profile.LabyrinthFloor, _app.Profile.PendingLabyrinth != 0);
                }
            }
            else
            {
                gold = win ? 486 : 48;
            }

            if (!net)
            {
                if (win)
                {
                    _app.Profile.Win++;
                }
                else
                {
                    _app.Profile.Lose++;
                }

                _app.Profile.Gold += gold;

                if (win)
                {
                    if (_app.Database != null)
                    {
                        _app.Profile.Honor += _app.Database.BattleWinHonor(_app.Profile.Level, _npcId != 0);
                        int gpGain = _npcId != 0 && _app.Database.Npcs.TryGetValue(_npcId, out NpcInfo npcInfo)
                            ? Mathf.Max(1, npcInfo.Experience)
                            : _app.Database.BattleWinGp(_app.Profile.Level, _npcId != 0);
                        _app.Profile.AddGp(_app.Database, gpGain);
                    }

                    if (_app.Profile.PendingLabyrinth != 0)
                    {
                        _app.Profile.LabyrinthFloor++;
                    }
                    questGold = _app.Profile.CompleteAcceptedQuests(_app.Database);
                    _app.Profile.PendingReward = 0;
                    _app.Profile.PendingLabyrinth = 0;
                }

                _app.Profile.Save();
            }
            string detail = win
                ? $"击败 {_foeName}" + (questGold > 0 ? $"\n任务奖励 +{questGold} 金" : "")
                : $"{_foeName} 获胜";
            BattleResultScreen.Show(_app.SafeArea, _app, win, gold, detail);
        }

        void PlaceOnGround(int i)
        {
            int x = Mathf.Clamp(Mathf.RoundToInt(_pos[i].x), 0, _map.Width - 1);
            _pos[i].y = _map.FindStandY(x, 0);
        }

        void StampCrater(int cx, int cy, int radius)
        {
            if (_foreTex == null)
            {
                return;
            }

            Color[] px = _foreTex.GetPixels();
            int w = _foreTex.width;
            int h = _foreTex.height;
            int r2 = radius * radius;
            Color[] craterPx = null;
            int cw = 0;
            int ch = 0;
            if (_craterTex != null)
            {
                craterPx = _craterTex.GetPixels();
                cw = _craterTex.width;
                ch = _craterTex.height;
            }

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y > r2)
                    {
                        continue;
                    }

                    int tx = cx + x;
                    int ty = h - 1 - (cy + y);
                    if (tx < 0 || ty < 0 || tx >= w || ty >= h)
                    {
                        continue;
                    }

                    int idx = ty * w + tx;
                    Color c = px[idx];
                    float cut = 1f;
                    if (craterPx != null && cw > 0 && ch > 0)
                    {
                        int sx = Mathf.Clamp((x + radius) * cw / (radius * 2 + 1), 0, cw - 1);
                        int sy = Mathf.Clamp((y + radius) * ch / (radius * 2 + 1), 0, ch - 1);
                        cut = craterPx[sy * cw + sx].a;
                    }

                    if (cut > 0.15f || craterPx == null)
                    {
                        c.a = 0f;
                    }

                    px[idx] = c;
                }
            }

            _foreTex.SetPixels(px);
            _foreTex.Apply(false, false);
        }

        void BuildActors(Transform parent)
        {
            _livingSheet = SpriteSheet.TryLoad(
                _app.Loader,
                GamePaths.PathCombine("Resource", "image", "game", "living", "living948.png"),
                GamePaths.PathCombine("Resource", "image", "game", "bonesLiving", "game_living_living948.png"));
            if (_livingSheet != null)
            {
                _walkFrames = _livingSheet.Sequence("Image/01_");
                _atkFrames = _livingSheet.Sequence("Image/attack");
                if (_walkFrames.Count == 0)
                {
                    _walkFrames.AddRange(_livingSheet.Frames);
                }
            }

            int craterId = _ball.Crater > 0 ? _ball.Crater : 65;
            string[] craterPaths =
            {
                GamePaths.PathCombine(GamePaths.BombCrater(craterId), "crater.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(craterId), "crater1.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(craterId), "Crater.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(65), "crater1.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(65), "Crater.png"),
                GamePaths.PathCombine(GamePaths.BombCrater(65), "crater.png")
            };
            foreach (string path in craterPaths)
            {
                if (_app.Loader.TryReadBytes(path, out byte[] bytes))
                {
                    _craterTex = SpriteSheet.LoadTexture(bytes, true);
                    if (_craterTex != null)
                    {
                        break;
                    }
                }
            }

            int actorCount = _loop != null ? _loop.Livings.Count : 2;
            _livingImg = new RawImage[actorCount];
            _hpFill = new RawImage[actorCount];
            _petImgs = new RawImage[actorCount];
            _titleImgs = new RawImage[actorCount];
            for (int i = 0; i < actorCount; i++)
            {
                var go = new GameObject("Living" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                go.transform.SetParent(_world, false);
                var raw = go.GetComponent<RawImage>();
                raw.raycastTarget = false;
                raw.color = Color.white;
                _livingImg[i] = raw;

                var hpGo = new GameObject("Hp" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                hpGo.transform.SetParent(go.transform, false);
                var hpRt = hpGo.GetComponent<RectTransform>();
                hpRt.anchorMin = new Vector2(0.1f, 1.05f);
                hpRt.anchorMax = new Vector2(0.9f, 1.18f);
                hpRt.offsetMin = hpRt.offsetMax = Vector2.zero;
                var hp = hpGo.GetComponent<RawImage>();
                hp.texture = Texture2D.whiteTexture;
                int team = i < _loop.Livings.Count ? _loop.Livings[i].Team : ((i % 2) + 1);
                PcSkin.Apply(hp, PcSkin.Game, team == 1 ? "game_HPStrip1" : "game_HPStrip2");
                hp.raycastTarget = false;
                _hpFill[i] = hp;

                DecorateActor(i);
            }

            NpcInfo npcArt = _npcId != 0 && _app.Database != null ? _app.Database.GetNpc(_npcId) : null;
            _npcSprite = PcArt.NpcLiving(_app.Loader, npcArt);
            if (_npcSprite != null && !PhoneNet.NetBattle && actorCount > 1)
            {
                int botSeat = 1;
                if (_livingImg[botSeat] != null)
                {
                    _livingImg[botSeat].texture = _npcSprite;
                    _livingImg[botSeat].uvRect = new Rect(0f, 0f, 1f, 1f);
                }
            }

            Texture2D bullet = PcArt.Bullet(_app.Loader, _ball.Id > 0 ? _ball.Id : _ball.FlyingPartical);
            var shotGo = new GameObject("Shot", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            shotGo.transform.SetParent(_world, false);
            _shotImg = shotGo.GetComponent<RawImage>();
            _shotImg.texture = bullet != null ? bullet : Texture2D.whiteTexture;
            _shotImg.color = Color.white;
            _shotImg.raycastTarget = false;
            shotGo.SetActive(false);

            _blastTex = PcArt.Blast(_app.Loader, _ball.BombPartical > 0 ? _ball.BombPartical : _ball.Id);
            var blastGo = new GameObject("Blast", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            blastGo.transform.SetParent(_world, false);
            _blastImg = blastGo.GetComponent<RawImage>();
            _blastImg.texture = _blastTex != null ? _blastTex : Texture2D.whiteTexture;
            _blastImg.color = Color.white;
            _blastImg.raycastTarget = false;
            blastGo.SetActive(false);

            _dots = new RawImage[10];
            for (int i = 0; i < _dots.Length; i++)
            {
                var d = new GameObject("Dot" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                d.transform.SetParent(_world, false);
                var raw = d.GetComponent<RawImage>();
                PcSkin.Apply(raw, PcSkin.Game, "game_takeAimAsset");
                if (raw.texture == null)
                {
                    raw.texture = Texture2D.whiteTexture;
                    raw.color = new Color(1f, 1f, 0.4f, 0.55f);
                }
                else
                {
                    raw.color = Color.white;
                }
                raw.raycastTarget = false;
                d.SetActive(false);
                _dots[i] = raw;
            }
        }

        SeatLook GetSeatLook(int seat)
        {
            if (_seatLooks != null && seat >= 0 && seat < _seatLooks.Length)
            {
                return _seatLooks[seat];
            }

            if (seat == MeSeat() || (!PhoneNet.NetBattle && seat == 0))
            {
                return new SeatLook
                {
                    Sex = _app.Profile.Sex,
                    Level = _app.Profile.Level,
                    EquipHead = _app.Profile.EquipHead,
                    EquipHair = _app.Profile.EquipHair,
                    EquipFace = _app.Profile.EquipFace,
                    EquipCloth = _app.Profile.EquipCloth,
                    EquipGlass = _app.Profile.EquipGlass,
                    EquipWeapon = _app.Profile.EquipWeapon,
                    PetId = _app.Profile.PetId,
                    TitleId = _app.Profile.TitleId
                };
            }

            return new SeatLook { Sex = 1, Level = 20, EquipWeapon = 7001 };
        }

        void DecorateActor(int seat)
        {
            if (_livingImg == null || seat < 0 || seat >= _livingImg.Length || _livingImg[seat] == null)
            {
                return;
            }

            Transform root = _livingImg[seat].transform;
            RawImage body = _livingImg[seat];
            SeatLook look = GetSeatLook(seat);
            bool useSheetAnim = _livingSheet != null && !PhoneNet.NetBattle && seat == 0 && _npcId == 0;

            if (useSheetAnim)
            {
                body.texture = _livingSheet.Texture;
                SheetFrame fr = _walkFrames.Count > 0 ? _walkFrames[Mathf.Min(6, _walkFrames.Count - 1)] : _livingSheet.Frames[0];
                body.uvRect = fr.Uv;
            }
            else
            {
                Texture2D living = PcArt.DefaultLiving(_app.Loader);
                if (living != null)
                {
                    body.texture = living;
                    body.uvRect = new Rect(0f, 0f, 1f, 1f);
                }
                else
                {
                    PcSkin.Apply(body, PcSkin.Game, "game_defaultCharacter");
                    if (body.texture == Texture2D.whiteTexture || body.texture == null)
                    {
                        PcSkin.Apply(body, PcSkin.Default, "image_deafult_player");
                    }
                }
            }

            if (_app.Database == null || useSheetAnim)
            {
                return;
            }

            AddEquipLayer(root, "Cloth", look.EquipCloth, look.Sex, new Vector2(0f, 0f), new Vector2(1f, 1f));
            AddEquipLayer(root, "Hair", look.EquipHair, look.Sex, new Vector2(0.05f, 0.45f), new Vector2(0.95f, 1.05f));
            AddEquipLayer(root, "Head", look.EquipHead, look.Sex, new Vector2(0.1f, 0.55f), new Vector2(0.9f, 1.05f));
            AddEquipLayer(root, "Face", look.EquipFace, look.Sex, new Vector2(0.2f, 0.45f), new Vector2(0.8f, 0.75f));
            AddEquipLayer(root, "Glass", look.EquipGlass, look.Sex, new Vector2(0.15f, 0.6f), new Vector2(0.85f, 0.9f));
            AddEquipLayer(root, "Weapon", look.EquipWeapon, look.Sex, new Vector2(0.5f, 0.1f), new Vector2(1.2f, 0.7f));

            if (look.NpcId != 0 && _app.Database != null)
            {
                NpcInfo npcArt = _app.Database.GetNpc(look.NpcId);
                Texture2D npcTex = PcArt.NpcLiving(_app.Loader, npcArt);
                if (npcTex != null)
                {
                    body.texture = npcTex;
                    body.uvRect = new Rect(0f, 0f, 1f, 1f);
                }
            }

            if (_app.Database.Pets.TryGetValue(look.PetId, out PetInfo pet))
            {
                Texture2D petTex = PcArt.PetIcon(_app.Loader, pet.Pic);
                if (petTex != null)
                {
                    var pgo = new GameObject("Pet", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    pgo.transform.SetParent(root, false);
                    var prt = pgo.GetComponent<RectTransform>();
                    prt.anchorMin = new Vector2(-0.35f, 0.05f);
                    prt.anchorMax = new Vector2(0.25f, 0.7f);
                    prt.offsetMin = prt.offsetMax = Vector2.zero;
                    var petImg = pgo.GetComponent<RawImage>();
                    petImg.texture = petTex;
                    petImg.raycastTarget = false;
                    if (_petImgs != null && seat < _petImgs.Length) _petImgs[seat] = petImg;
                }
            }

            if (_app.Database.Titles.TryGetValue(look.TitleId, out TitleInfo title))
            {
                Texture2D banner = PcArt.TitleBanner(_app.Loader, title.Pic);
                if (banner != null)
                {
                    var tgo = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    tgo.transform.SetParent(root, false);
                    var trt = tgo.GetComponent<RectTransform>();
                    trt.anchorMin = new Vector2(-0.2f, 1.2f);
                    trt.anchorMax = new Vector2(1.2f, 1.55f);
                    trt.offsetMin = trt.offsetMax = Vector2.zero;
                    var titleImg = tgo.GetComponent<RawImage>();
                    titleImg.texture = banner;
                    titleImg.raycastTarget = false;
                    if (_titleImgs != null && seat < _titleImgs.Length) _titleImgs[seat] = titleImg;
                }
            }

            RawImage lv = PcSkin.Slice(root, "Lv", PcSkin.Game, "level_" + Mathf.Clamp(look.Level, 1, 70), false);
            if (lv != null)
            {
                var lrt = lv.rectTransform;
                lrt.anchorMin = new Vector2(0.35f, 1.55f);
                lrt.anchorMax = new Vector2(0.65f, 1.85f);
                lrt.offsetMin = lrt.offsetMax = Vector2.zero;
                lv.raycastTarget = false;
            }
        }

        void AddEquipLayer(Transform parent, string name, int templateId, int sex, Vector2 anchorMin, Vector2 anchorMax)
        {
            if (_app.Database == null || templateId <= 0)
            {
                return;
            }

            Texture2D tex = PcArt.EquipLayer(_app.Loader, _app.Database.GetItem(templateId), sex);
            if (tex == null)
            {
                return;
            }

            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var raw = go.GetComponent<RawImage>();
            raw.texture = tex;
            raw.raycastTarget = false;
        }

        void UpdateActors()
        {
            if (_livingImg == null || _map == null || _pos == null)
            {
                return;
            }

            _animT += Time.deltaTime;
            if (_blastT > 0f && _blastImg != null)
            {
                _blastT -= Time.deltaTime;
                _blastImg.gameObject.SetActive(_blastT > 0f);
            }

            int me = MeSeat();
            bool walking = _loop.Phase == BattlePhase.Aiming && _loop.CurrentLiving == me && _move != null && _move.Direction != 0;
            bool firing = _flying;
            for (int i = 0; i < _livingImg.Length; i++)
            {
                bool dead = _loop.Livings[i].Hp <= 0;
                if (_petImgs != null && i < _petImgs.Length && _petImgs[i] != null)
                {
                    _petImgs[i].gameObject.SetActive(!dead);
                }

                if (_titleImgs != null && i < _titleImgs.Length && _titleImgs[i] != null)
                {
                    _titleImgs[i].gameObject.SetActive(!dead);
                }

                if (dead)
                {
                    PcSkin.Apply(_livingImg[i], PcSkin.Game, "game_tombAsset");
                    _livingImg[i].gameObject.SetActive(_livingImg[i].texture != null);
                    if (_hpFill[i] != null)
                    {
                        _hpFill[i].gameObject.SetActive(false);
                    }

                    continue;
                }

                _livingImg[i].gameObject.SetActive(true);
                if (_hpFill[i] != null)
                {
                    _hpFill[i].gameObject.SetActive(true);
                }

                SheetFrame frame = PickFrame(i == me && (walking || firing));
                bool useNpcSprite = !PhoneNet.NetBattle && i != me && _npcSprite != null;
                bool useSheet = !useNpcSprite && _livingSheet != null && _livingImg[i].texture == _livingSheet.Texture;
                if (useNpcSprite)
                {
                    _livingImg[i].uvRect = new Rect(0f, 0f, 1f, 1f);
                    frame = new SheetFrame { Uv = _livingImg[i].uvRect, Size = FitSprite(_npcSprite.width, _npcSprite.height, 96f, 120f) };
                }
                else if (useSheet)
                {
                    _livingImg[i].uvRect = frame.Uv;
                    frame = new SheetFrame { Uv = frame.Uv, Size = FitSprite(frame.Size.x, frame.Size.y, 80f, 100f) };
                }
                else
                {
                    frame = new SheetFrame { Uv = new Rect(0f, 0f, 1f, 1f), Size = FitSprite(_livingImg[i].texture != null ? _livingImg[i].texture.width : 72, _livingImg[i].texture != null ? _livingImg[i].texture.height : 90, 80f, 100f) };
                }
                float sx = _world.rect.width / Mathf.Max(1, _map.Width);
                float sy = _world.rect.height / Mathf.Max(1, _map.Height);
                var rt = _livingImg[i].rectTransform;
                rt.pivot = new Vector2(0.5f, 0f);
                rt.anchorMin = rt.anchorMax = MapAnchor(_pos[i].x, _pos[i].y);
                rt.sizeDelta = new Vector2(Mathf.Max(48f, frame.Size.x * sx), Mathf.Max(56f, frame.Size.y * sy));
                rt.localScale = new Vector3(_facing[i] >= 0 ? 1f : -1f, 1f, 1f);
                if (_hpFill[i] != null)
                {
                    float pct = _loop.Livings[i].MaxHp <= 0 ? 0f : (float)_loop.Livings[i].Hp / _loop.Livings[i].MaxHp;
                    _hpFill[i].rectTransform.anchorMax = new Vector2(0.1f + 0.8f * Mathf.Clamp01(pct), 1.18f);
                }
            }

            if (_shotImg != null)
            {
                _shotImg.gameObject.SetActive(_flying);
                if (_flying)
                {
                    var rt = _shotImg.rectTransform;
                    rt.anchorMin = rt.anchorMax = UnityAnchor(_shot.X, _shot.Y);
                    rt.sizeDelta = new Vector2(28f, 28f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                }
            }

            UpdateAimDots();
        }

        SheetFrame PickFrame(bool animate)
        {
            List<SheetFrame> seq = _flying && _atkFrames.Count > 0 ? _atkFrames : _walkFrames;
            if (seq == null || seq.Count == 0)
            {
                return new SheetFrame { Uv = new Rect(0f, 0f, 1f, 1f), Size = new Vector2(72f, 90f) };
            }

            int idx = animate ? (int)(_animT * 10f) % seq.Count : Mathf.Min(6, seq.Count - 1);
            return seq[idx];
        }

        Vector2 MapAnchor(float mapX, float mapYFromTop)
        {
            return new Vector2(mapX / _map.Width, 1f - mapYFromTop / _map.Height);
        }

        Vector2 UnityAnchor(float ux, float uy)
        {
            return new Vector2(ux / _map.Width, uy / _map.Height);
        }

        void UpdateAimDots()
        {
            if (_dots == null)
            {
                return;
            }

            int me = MeSeat();
            bool show = _loop.Phase == BattlePhase.Aiming && _loop.CurrentLiving == me && _aim != null && !_flying;
            if (!show)
            {
                for (int i = 0; i < _dots.Length; i++)
                {
                    _dots[i].gameObject.SetActive(false);
                }

                return;
            }

            Vector2 p = _pos[me];
            if (_ballsByLiving != null && me >= 0 && me < _ballsByLiving.Length && _ballsByLiving[me] != null)
            {
                _ball = _ballsByLiving[me];
                _sim.ApplyBall(_ball);
            }
            ProjectileState s = _sim.Launch(p.x, _map.Height - p.y - 18f, _aim.AngleDeg, _aim.Power, _facing[me]);
            for (int i = 0; i < _dots.Length; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    s = _sim.StepFrame(s, _loop.Wind);
                }

                var rt = _dots[i].rectTransform;
                rt.anchorMin = rt.anchorMax = UnityAnchor(s.X, s.Y);
                rt.sizeDelta = new Vector2(8f, 8f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                _dots[i].gameObject.SetActive(s.X > 0 && s.X < _map.Width && s.Y > 0);
            }
        }

        void DrawHud()
        {
            if (_hud == null)
            {
                return;
            }

            int seat = MeSeat();
            var me = _loop.Livings[seat];
            string turnName = _playerNames != null && _loop.CurrentLiving >= 0 && _loop.CurrentLiving < _playerNames.Length
                ? _playerNames[_loop.CurrentLiving]
                : "P" + (_loop.CurrentLiving + 1);
            string aim = _aim != null ? $"{_aim.AngleDeg:0}° {_aim.Power:0}" : "";
            _hud.text = $"Map {_mapId}  vs {_foeName}  Turn:{turnName}  Wind {_loop.Wind:+0;-0}  HP {me.Hp}/{me.MaxHp}  {_loop.Phase}  {aim}  t{_loop.TurnTimeLeft:0}s  ball {_ball.Id}" +
                (_propId > 0 ? "  prop " + _propId : "") +
                (PhoneNet.NetBattle ? "  seat " + seat : "");
        }

        int MeSeat()
        {
            int max = _loop != null ? Mathf.Max(1, _loop.Livings.Count - 1) : 1;
            return PhoneNet.NetBattle ? Mathf.Clamp(PhoneNet.Seat, 0, max) : 0;
        }

        static Vector2 FitSprite(float w, float h, float maxW, float maxH)
        {
            if (w <= 0f || h <= 0f)
            {
                return new Vector2(maxW, maxH);
            }

            float s = Mathf.Min(1f, Mathf.Min(maxW / w, maxH / h));
            return new Vector2(w * s, h * s);
        }
    }
}
