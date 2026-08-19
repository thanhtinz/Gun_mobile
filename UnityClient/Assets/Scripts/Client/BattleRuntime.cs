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
        public static void Show(RectTransform safe, GameApp app, int mapId, int npcId = 0)
        {
            var host = safe.GetComponent<BattleHost>();
            if (host == null)
            {
                host = safe.gameObject.AddComponent<BattleHost>();
            }

            host.Run(app, mapId, npcId);
        }
    }

    public sealed class BattleHost : MonoBehaviour
    {
        GameApp _app;
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
        Vector2[] _pos;
        int[] _facing;
        ProjectileState _shot;
        bool _flying;
        bool _botQueued;
        float _botDelay;
        int _mapId;
        int _npcId;
        string _foeName = "Bot";
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
        int _propId;
        float _propPower;
        float _propDmg = 1f;
        float _propRadius = 1f;
        bool _propCrit;
        float _walkSendT;
        int _lastWalkDir;
        RawImage _petImg;
        RawImage _titleImg;

        public void Run(GameApp app, int mapId, int npcId = 0)
        {
            _app = app;
            _mapId = mapId;
            _npcId = npcId;
            _resultOpen = false;
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
                    Attack = 110,
                    Defence = 85,
                    Agility = 70,
                    Luck = 40,
                    Hp = 1200,
                    MaxHp = 1200,
                    Team = 2
                };
                _foeName = PhoneNet.NetBattle ? "P2" : "Bot";
            }

            int seed = PhoneNet.NetBattle && PhoneNet.BattleSeed != 0 ? PhoneNet.BattleSeed : 0;
            _loop.Reset(new[] { player, bot }, 20f, seed);
            _ball = BallPhysics.Default;
            if (_app.Database != null)
            {
                int ballId = _app.Profile.PreferredBallId > 0
                    ? _app.Profile.PreferredBallId
                    : _app.Database.DefaultBallId(_app.Profile.WeaponId);
                _ball = _app.Database.GetBall(ballId);
            }

            _sim.ApplyBall(_ball);
            _pos = new[] { new Vector2(140f, 0f), new Vector2(_map.Width - 160f, 0f) };
            _facing = new[] { 1, -1 };
            PlaceOnGround(0);
            PlaceOnGround(1);
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
                btn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.04f);
                RawImage art = PcSkin.Slice(btn.transform, "Art", PcSkin.GameProp, "game_prop_" + id, true);
                if (art != null)
                {
                    art.raycastTarget = false;
                }
            }
        }

        void UseProp(int id)
        {
            if (_loop == null || _loop.Phase != BattlePhase.Aiming || _loop.CurrentLiving != MeSeat())
            {
                return;
            }

            _propId = id;
            _propPower = 0f;
            _propDmg = 1f;
            _propRadius = 1f;
            _propCrit = false;
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

        void Fire(int who, float angle, float power, bool fromNet)
        {
            _loop.BeginShot();
            _aim?.SetFacing(_facing[who]);
            Vector2 p = _pos[who];
            float unityY = _map.Height - p.y - 18f;
            _shot = _sim.Launch(p.x, unityY, angle, power, _facing[who]);
            _flying = true;
            if (!fromNet && PhoneNet.NetBattle)
            {
                PhoneNet.SendFire(who, angle, power, _facing[who]);
            }
        }

        void PumpNet()
        {
            if (!PhoneNet.NetBattle || PhoneNet.Fight == null)
            {
                return;
            }

            while (PhoneNet.Fight.TryDequeue(out var msg))
            {
                if (msg.Id == PhoneMsg.FightWalk)
                {
                    int walker = JsonInt(msg.Json, "who", 1 - MeSeat());
                    float x = JsonFloat(msg.Json, "x", -1f);
                    int face = JsonInt(msg.Json, "facing", 1);
                    ApplyNetWalk(walker, x, face);
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

        void EndShot(bool explode, int mx, int my)
        {
            _flying = false;
            int radius = Mathf.Max(24, Mathf.RoundToInt((_ball.Radii > 0 ? _ball.Radii / 2 : 38) * _propRadius));
            if (explode)
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

            _loop.EndShot();
            _loop.FinishSettle();
            if (explode && _blastImg != null)
            {
                _blastT = 0.35f;
                var rt = _blastImg.rectTransform;
                rt.anchorMin = rt.anchorMax = MapAnchor(mx, my);
                rt.sizeDelta = new Vector2(72f, 72f);
                _blastImg.gameObject.SetActive(true);
            }

            if (_loop.Phase == BattlePhase.MatchOver)
            {
                FinishMatch();
            }

            ClearProp();
        }

        void ClearProp()
        {
            _propId = 0;
            _propPower = 0f;
            _propDmg = 1f;
            _propRadius = 1f;
            _propCrit = false;
        }

        void Hurt(int index, float dist)
        {
            int src = _loop.CurrentLiving;
            int bombHurt = 80 + Mathf.RoundToInt(Mathf.Abs(_ball.Power) * 80f);
            if (bombHurt < 40)
            {
                bombHurt = 140;
            }

            bombHurt = Mathf.RoundToInt(bombHurt * _propDmg);
            bool crit = _propCrit || DamageCalculator.RollCrit(_loop.Livings[src].Luck, src + _loop.TurnIndex);
            int dmg = DamageCalculator.Compute(_loop.Livings[src], _loop.Livings[index], bombHurt, dist, crit);
            _loop.ApplyDamage(index, dmg);
        }

        void FinishMatch()
        {
            if (_resultOpen)
            {
                return;
            }

            _resultOpen = true;
            bool win = _loop.Livings[MeSeat()].Hp > 0;
            int gold = 0;
            int questGold = 0;
            if (win)
            {
                _app.Profile.Win++;
                gold = 800 + Mathf.Max(0, _app.Profile.PendingReward);
                _app.Profile.Gold += gold;
                _app.Profile.Honor += _npcId != 0 ? 12 : 4;
                if (_app.Profile.PendingLabyrinth != 0)
                {
                    _app.Profile.LabyrinthFloor++;
                }

                questGold = _app.Profile.CompleteAcceptedQuests(_app.Database);
            }
            else
            {
                _app.Profile.Lose++;
            }

            _app.Profile.PendingReward = 0;
            _app.Profile.PendingLabyrinth = 0;
            _app.Profile.Save();
            string detail = win
                ? $"击败 {_foeName}" + (questGold > 0 ? $"\n任务奖励 +{questGold} 金" : "")
                : $"{_foeName} 获胜";
            BattleResultScreen.Show(_app.SafeArea, _app, win, gold + questGold, detail);
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

            _livingImg = new RawImage[2];
            _hpFill = new RawImage[2];
            for (int i = 0; i < 2; i++)
            {
                var go = new GameObject("Living" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                go.transform.SetParent(_world, false);
                var raw = go.GetComponent<RawImage>();
                raw.raycastTarget = false;
                if (_livingSheet != null)
                {
                    raw.texture = _livingSheet.Texture;
                    SheetFrame fr = _walkFrames.Count > 0 ? _walkFrames[Mathf.Min(6, _walkFrames.Count - 1)] : _livingSheet.Frames[0];
                    raw.uvRect = fr.Uv;
                }
                else
                {
                    PcSkin.Apply(raw, PcSkin.Game, "game_defaultCharacter");
                    if (raw.texture == Texture2D.whiteTexture || raw.texture == null)
                    {
                        PcSkin.Apply(raw, PcSkin.Default, "image_deafult_player");
                    }
                    if (raw.texture == Texture2D.whiteTexture || raw.texture == null)
                    {
                        raw.texture = Texture2D.whiteTexture;
                    }
                }

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
                PcSkin.Apply(hp, PcSkin.Game, i == 0 ? "game_HPStrip1" : "game_HPStrip2");
                hp.raycastTarget = false;
                _hpFill[i] = hp;
            }

            NpcInfo npcArt = _npcId != 0 && _app.Database != null ? _app.Database.GetNpc(_npcId) : null;
            _npcSprite = PcArt.NpcLiving(_app.Loader, npcArt);
            if (_npcSprite == null && PhoneNet.NetBattle)
            {
                _npcSprite = PcArt.DefaultLiving(_app.Loader);
            }

            if (_npcSprite != null && _livingImg[1] != null)
            {
                _livingImg[1].texture = _npcSprite;
                _livingImg[1].uvRect = new Rect(0f, 0f, 1f, 1f);
                _livingImg[1].color = Color.white;
            }

            if (_app.Database != null)
            {
                Texture2D weap = PcArt.EquipLayer(_app.Loader, _app.Database.GetItem(_app.Profile.EquipWeapon), _app.Profile.Sex);
                if (weap != null && _livingImg[0] != null)
                {
                    var wgo = new GameObject("Weapon", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    wgo.transform.SetParent(_livingImg[0].transform, false);
                    var wrt = wgo.GetComponent<RectTransform>();
                    wrt.anchorMin = new Vector2(0.55f, 0.15f);
                    wrt.anchorMax = new Vector2(1.15f, 0.85f);
                    wrt.offsetMin = wrt.offsetMax = Vector2.zero;
                    var wraw = wgo.GetComponent<RawImage>();
                    wraw.texture = weap;
                    wraw.raycastTarget = false;
                }
            }

            if (_app.Database != null && _app.Database.Pets.TryGetValue(_app.Profile.PetId, out PetInfo pet))
            {
                Texture2D petTex = PcArt.PetIcon(_app.Loader, pet.Pic);
                if (petTex != null && _livingImg[0] != null)
                {
                    var pgo = new GameObject("Pet", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    pgo.transform.SetParent(_livingImg[0].transform, false);
                    var prt = pgo.GetComponent<RectTransform>();
                    prt.anchorMin = new Vector2(-0.35f, 0.05f);
                    prt.anchorMax = new Vector2(0.25f, 0.7f);
                    prt.offsetMin = prt.offsetMax = Vector2.zero;
                    _petImg = pgo.GetComponent<RawImage>();
                    _petImg.texture = petTex;
                    _petImg.raycastTarget = false;
                }
            }

            if (_app.Database != null && _app.Database.Titles.TryGetValue(_app.Profile.TitleId, out TitleInfo title))
            {
                Texture2D banner = PcArt.TitleBanner(_app.Loader, title.Pic);
                if (banner != null && _livingImg[0] != null)
                {
                    var tgo = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    tgo.transform.SetParent(_livingImg[0].transform, false);
                    var trt = tgo.GetComponent<RectTransform>();
                    trt.anchorMin = new Vector2(-0.2f, 1.2f);
                    trt.anchorMax = new Vector2(1.2f, 1.55f);
                    trt.offsetMin = trt.offsetMax = Vector2.zero;
                    _titleImg = tgo.GetComponent<RawImage>();
                    _titleImg.texture = banner;
                    _titleImg.raycastTarget = false;
                }
            }

            if (_livingImg[0] != null)
            {
                RawImage lv = PcSkin.Slice(_livingImg[0].transform, "Lv", PcSkin.Game, "level_" + Mathf.Clamp(_app.Profile.Level, 1, 70), false);
                if (lv != null)
                {
                    var lrt = lv.rectTransform;
                    lrt.anchorMin = new Vector2(0.35f, 1.55f);
                    lrt.anchorMax = new Vector2(0.65f, 1.85f);
                    lrt.offsetMin = lrt.offsetMax = Vector2.zero;
                    lv.raycastTarget = false;
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
                if (i == 0)
                {
                    if (_petImg != null)
                    {
                        _petImg.gameObject.SetActive(!dead);
                    }

                    if (_titleImg != null)
                    {
                        _titleImg.gameObject.SetActive(!dead);
                    }
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
                if (i == 1 && _npcSprite != null)
                {
                    _livingImg[i].uvRect = new Rect(0f, 0f, 1f, 1f);
                    frame = new SheetFrame { Uv = _livingImg[i].uvRect, Size = FitSprite(_npcSprite.width, _npcSprite.height, 96f, 120f) };
                }
                else
                {
                    _livingImg[i].uvRect = frame.Uv;
                    frame = new SheetFrame { Uv = frame.Uv, Size = FitSprite(frame.Size.x, frame.Size.y, 80f, 100f) };
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
            var foe = _loop.Livings[1 - seat];
            string aim = _aim != null ? $"{_aim.AngleDeg:0}° {_aim.Power:0}" : "";
            _hud.text = $"Map {_mapId}  {_foeName}  Wind {_loop.Wind:+0;-0}  HP {me.Hp}/{me.MaxHp} vs {foe.Hp}  {_loop.Phase}  {aim}  t{_loop.TurnTimeLeft:0}s  ball {_ball.Id}" +
                (_propId > 0 ? "  prop " + _propId : "") +
                (PhoneNet.NetBattle ? "  LAN seat " + MeSeat() : "");
        }

        static int MeSeat()
        {
            return PhoneNet.NetBattle ? Mathf.Clamp(PhoneNet.Seat, 0, 1) : 0;
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
