using System.Collections;
using GunMobile.Core;
using GunMobile.Logic;
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
            var bg = UiKit.Panel(safe, "Module", new Color(0.07f, 0.08f, 0.12f, 1f));
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

        public void Run(GameApp app, int mapId, int npcId = 0)
        {
            _app = app;
            _mapId = mapId;
            _npcId = npcId;
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
                _foeName = "Bot";
            }

            _loop.Reset(new[] { player, bot });
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
            var bar = new GameObject("Hud", typeof(RectTransform), typeof(Image));
            bar.transform.SetParent(parent, false);
            var rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.86f);
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            bar.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);
            _hud = UiKit.Label(bar.transform, "HudText", "", 24, Color.white, TextAnchor.MiddleLeft);
            UiKit.Stretch(_hud.gameObject).offsetMin = new Vector2(20f, 0f);

            var exit = UiKit.Button(bar.transform, "Exit", "退出", () =>
            {
                _app.ShowHall();
            }, new Vector2(140f, 50f));
            exit.GetComponent<RectTransform>().anchorMin = exit.GetComponent<RectTransform>().anchorMax = new Vector2(0.93f, 0.5f);

            var move = MobileUiBootstrap.CreateHudLayer(parent as RectTransform, "Move", TextAnchor.LowerLeft, MobileUiBootstrap.FingerButtonSize * 3f);
            move.gameObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.12f);
            _move = move.gameObject.AddComponent<TouchMoveController>();

            var aim = MobileUiBootstrap.CreateHudLayer(parent as RectTransform, "Aim", TextAnchor.LowerRight, MobileUiBootstrap.FingerButtonSize * 3.4f);
            aim.gameObject.AddComponent<Image>().color = new Color(1f, 0.8f, 0.2f, 0.16f);
            _aim = aim.gameObject.AddComponent<TouchAimController>();
        }

        void Update()
        {
            if (_map == null || _loop == null || _pos == null)
            {
                return;
            }

            _loop.TickClock(Time.deltaTime);
            int cur = _loop.CurrentLiving;
            if (_loop.Phase == BattlePhase.Aiming && cur == 0 && _aim != null)
            {
                int walk = _move != null ? _move.Direction : 0;
                if (walk != 0)
                {
                    _facing[0] = walk;
                    _pos[0].x = Mathf.Clamp(_pos[0].x + walk * 80f * Time.deltaTime, 20f, _map.Width - 20f);
                    PlaceOnGround(0);
                    _aim.SetFacing(_facing[0]);
                }

                if (_aim.FireReleased)
                {
                    _aim.ConsumeFire();
                    Fire(0, _aim.AngleDeg, _aim.Power);
                }
            }
            else if (_loop.Phase == BattlePhase.Aiming && cur == 1 && !_flying && !_botQueued)
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

            Fire(1, bestA, bestP);
        }

        void Fire(int who, float angle, float power)
        {
            _loop.BeginShot();
            _aim?.SetFacing(_facing[who]);
            Vector2 p = _pos[who];
            float unityY = _map.Height - p.y - 18f;
            _shot = _sim.Launch(p.x, unityY, angle, power, _facing[who]);
            _flying = true;
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
            int radius = Mathf.Max(24, _ball.Radii > 0 ? _ball.Radii / 2 : 38);
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
            if (_loop.Phase == BattlePhase.MatchOver)
            {
                FinishMatch();
            }
        }

        void Hurt(int index, float dist)
        {
            int src = _loop.CurrentLiving;
            int bombHurt = 80 + Mathf.RoundToInt(Mathf.Abs(_ball.Power) * 80f);
            if (bombHurt < 40)
            {
                bombHurt = 140;
            }

            int dmg = DamageCalculator.Compute(_loop.Livings[src], _loop.Livings[index], bombHurt, dist, DamageCalculator.RollCrit(_loop.Livings[src].Luck, src + _loop.TurnIndex));
            _loop.ApplyDamage(index, dmg);
        }

        void FinishMatch()
        {
            bool win = _loop.Livings[0].Hp > 0;
            if (win)
            {
                _app.Profile.Win++;
                _app.Profile.Gold += 800 + Mathf.Max(0, _app.Profile.PendingReward);
                _app.Profile.Honor += _npcId != 0 ? 12 : 4;
                if (_app.Profile.PendingLabyrinth != 0)
                {
                    _app.Profile.LabyrinthFloor++;
                }
            }
            else
            {
                _app.Profile.Lose++;
            }

            _app.Profile.PendingReward = 0;
            _app.Profile.PendingLabyrinth = 0;
            _app.Profile.Save();
            _app.ShowHall();
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
                    c.a = 0f;
                    px[idx] = c;
                }
            }

            _foreTex.SetPixels(px);
            _foreTex.Apply(false, false);
        }

        void DrawHud()
        {
            if (_hud == null)
            {
                return;
            }

            var me = _loop.Livings[0];
            var foe = _loop.Livings[1];
            string aim = _aim != null ? $"{_aim.AngleDeg:0}° {_aim.Power:0}" : "";
            _hud.text = $"Map {_mapId}  {_foeName}  Wind {_loop.Wind:+0;-0}  HP {me.Hp}/{me.MaxHp} vs {foe.Hp}  {_loop.Phase}  {aim}  ball {_ball.Id} r{_ball.Radii}";
        }

        void OnGUI()
        {
            if (_map == null || _pos == null)
            {
                return;
            }

            // Tanks as IMGUI dots in screen space mapped from map pixels via the world rect.
            if (_world == null)
            {
                return;
            }

            Vector3[] corners = new Vector3[4];
            _world.GetWorldCorners(corners);
            float x0 = corners[0].x;
            float y0 = corners[0].y;
            float x1 = corners[2].x;
            float y1 = corners[2].y;
            for (int i = 0; i < _pos.Length; i++)
            {
                if (_loop.Livings[i].Hp <= 0)
                {
                    continue;
                }

                float sx = Mathf.Lerp(x0, x1, _pos[i].x / _map.Width);
                float sy = Mathf.Lerp(y1, y0, _pos[i].y / _map.Height);
                Rect r = new Rect(sx - 14f, Screen.height - sy - 18f, 28f, 36f);
                GUI.color = i == 0 ? new Color(0.3f, 0.85f, 1f) : new Color(1f, 0.4f, 0.35f);
                GUI.DrawTexture(r, Texture2D.whiteTexture);
            }

            if (_flying)
            {
                float sx = Mathf.Lerp(x0, x1, _shot.X / _map.Width);
                float sy = Mathf.Lerp(y0, y1, _shot.Y / _map.Height);
                GUI.color = Color.yellow;
                GUI.DrawTexture(new Rect(sx - 5f, Screen.height - sy - 5f, 10f, 10f), Texture2D.whiteTexture);
            }

            GUI.color = Color.white;
        }
    }
}
