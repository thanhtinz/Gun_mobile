using System.Collections.Generic;
using GunMobile.Core;
using GunMobile.Net;
using GunMobile.Res;
using GunMobile.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GunMobile.Client
{
    public static class LoginScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            var battle = safe.GetComponent<BattleHost>();
            if (battle != null)
            {
                Object.Destroy(battle);
            }

            UiKit.ClearChildren(safe);
            PcSkin.Warm(app.Loader);
            var bg = UiKit.Panel(safe, "Login", Color.black);
            PcSkin.Backdrop(
                bg.transform,
                app.Loader,
                GamePaths.PathCombine("Flash", "1.png"),
                GamePaths.PathCombine("Flash", "2.png"),
                GamePaths.PathCombine("Flash", "3.png"),
                GamePaths.PathCombine("Flash", "4.png"));

            InputField nick = UiKit.Field(bg.transform, "Nick", "Nickname / 昵称", new Vector2(560f, 72f));
            nick.text = app.Profile.Nick;
            var nickRt = nick.GetComponent<RectTransform>();
            nickRt.anchorMin = nickRt.anchorMax = new Vector2(0.5f, 0.22f);
            nickRt.pivot = new Vector2(0.5f, 0.5f);

            Button enter = UiKit.Button(bg.transform, "Enter", "进入大厅", () =>
            {
                app.Profile.Nick = string.IsNullOrWhiteSpace(nick.text) ? "Player" : nick.text.Trim();
                PhoneNet.Login(app.Profile.Nick);
                app.ShowHall();
            }, new Vector2(360f, 72f));
            var enterRt = enter.GetComponent<RectTransform>();
            enterRt.anchorMin = enterRt.anchorMax = new Vector2(0.5f, 0.1f);
        }
    }

    public static class HallScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            var battle = safe.GetComponent<BattleHost>();
            if (battle != null)
            {
                Object.Destroy(battle);
            }

            UiKit.ClearChildren(safe);
            PcSkin.Warm(app.Loader);
            var bg = UiKit.Panel(safe, "Hall", Color.black);
            SpriteSheet build = PcSkin.HallBuild;
            if (build != null)
            {
                Place(bg.transform, build, "Sea", "hall_new_sea", new Vector2(0.5f, 0.22f), new Vector2(1400f, 280f));
                Place(bg.transform, build, "Road", "hall_new_road", new Vector2(0.5f, 0.34f), new Vector2(980f, 220f));
                Place(bg.transform, build, "Town", "hall_new_build1", new Vector2(0.55f, 0.58f), new Vector2(720f, 140f));
                Place(bg.transform, build, "Tree1", "hall_new_tree1", new Vector2(0.22f, 0.62f), new Vector2(420f, 80f));
                Place(bg.transform, build, "Tree2", "hall_new_tree2", new Vector2(0.8f, 0.6f), new Vector2(320f, 72f));
                Place(bg.transform, build, "Boat", "hall_new_boat", new Vector2(0.72f, 0.16f), new Vector2(280f, 56f));
                Building(bg.transform, build, "FightB", "hall_new_fight", "hall_new_fight_name", new Vector2(0.22f, 0.42f), new Vector2(220f, 240f), app.ShowRoom);
                Building(bg.transform, build, "DunB", "hall_new_dungeon", "hall_new_dungeon_name", new Vector2(0.5f, 0.46f), new Vector2(230f, 170f), () => Open(app, "dungeon"));
                Building(bg.transform, build, "AudB", "hall_new_auditorium", "hall_new_auditorium_name", new Vector2(0.78f, 0.48f), new Vector2(210f, 180f), () => Open(app, "consortia"));
                Building(bg.transform, build, "SecB", "hall_new_secret", "hall_new_secret_name", new Vector2(0.36f, 0.52f), new Vector2(90f, 150f), () => Open(app, "labyrinth"));
                RankPodium(bg.transform, build, () => Open(app, "rank"));
            }
            else
            {
                var hallBg = PcSkin.Slice(bg.transform, "HallBg", PcSkin.Hall, "hall_scene_bg_0", true);
                if (hallBg == null)
                {
                    PcSkin.Backdrop(
                        bg.transform,
                        app.Loader,
                        GamePaths.PathCombine("Flash", "ui", "cn_trad", "starling", "hall_scene", "hall_scene.png"));
                }

                PlaceOld(bg.transform, "Church", "hall_scene_church_build", new Vector2(0.18f, 0.38f), new Vector2(280f, 230f));
                Hotspot(bg.transform, "roomList", "hall_scene_build_title_roomList", new Vector2(0.5f, 0.42f), app.ShowRoom);
            }

            PlayerProfile p = app.Profile;
            UiKit.Label(bg.transform, "Info",
                $"{p.Nick}  Lv.{p.Level} VIP{p.VipLevel}  ATK {p.Attack} DEF {p.Defence}  Gold {p.Gold}  Honor {p.Honor}  {p.Win}W/{p.Lose}L",
                22, Color.white, TextAnchor.MiddleLeft)
                .rectTransform.anchorMin = new Vector2(0.02f, 0.92f);
            bg.transform.Find("Info").GetComponent<RectTransform>().anchorMax = new Vector2(0.78f, 1f);
            bg.transform.Find("Info").GetComponent<RectTransform>().offsetMin = Vector2.zero;
            bg.transform.Find("Info").GetComponent<RectTransform>().offsetMax = Vector2.zero;

            var fight = UiKit.Button(bg.transform, "Fight", "开战", app.ShowRoom, new Vector2(160f, 52f));
            fight.GetComponent<RectTransform>().anchorMin = fight.GetComponent<RectTransform>().anchorMax = new Vector2(0.92f, 0.95f);

            var scroll = UiKit.Scroll(bg.transform, "Modules");
            var srt = scroll.GetComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = new Vector2(1f, 0.22f);
            srt.offsetMin = new Vector2(8f, 4f);
            srt.offsetMax = new Vector2(-8f, -4f);
            Object.Destroy(scroll.content.gameObject.GetComponent<VerticalLayoutGroup>());
            var grid = scroll.content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(160f, 56f);
            grid.spacing = new Vector2(8f, 8f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 7;
            grid.padding = new RectOffset(4, 4, 4, 4);
            scroll.content.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (ModuleDef mod in ModuleCatalog.All)
            {
                ModuleDef local = mod;
                var btn = UiKit.Button(scroll.content, local.Id, local.Title, () => app.ShowModule(local), grid.cellSize);
                SkinModule(btn, local.Id);
            }
        }

        static void SkinModule(Button btn, string id)
        {
            string frame = null;
            switch (id)
            {
                case "room": frame = "hall_scene_build_title_roomList"; break;
                case "dungeon": frame = "hall_scene_build_title_dungeon"; break;
                case "labyrinth": frame = "hall_scene_build_title_labyrinth"; break;
                case "church":
                case "signin": frame = "hall_scene_build_title_church"; break;
                case "worldboss": frame = "hall_scene_build_title_cryptBoss"; break;
                case "character": frame = "hall_scene_build_title_home"; break;
                case "consortia": frame = "hall_scene_build_title_ringStation"; break;
            }

            if (string.IsNullOrEmpty(frame) || btn == null)
            {
                return;
            }

            RawImage raw = PcSkin.Slice(btn.transform, "TitleArt", PcSkin.Hall, frame, true);
            if (raw != null)
            {
                raw.raycastTarget = false;
                var cap = btn.transform.Find("Caption") as RectTransform;
                if (cap != null)
                {
                    cap.gameObject.SetActive(false);
                }
            }
        }

        static void RankPodium(Transform parent, SpriteSheet sheet, UnityEngine.Events.UnityAction click)
        {
            var btn = UiKit.Button(parent, "RankPodium", "", click, new Vector2(360f, 90f));
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.78f, 0.72f);
            rt.sizeDelta = new Vector2(360f, 90f);
            btn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.02f);
            RawImage bg = PcSkin.Slice(btn.transform, "RankBg", sheet, "hall_new_rankbg", true);
            if (bg != null)
            {
                bg.raycastTarget = false;
            }

            PlaceOn(btn.transform, sheet, "First", "hall_new_first", new Vector2(0.32f, 0.58f), new Vector2(72f, 56f));
            PlaceOn(btn.transform, sheet, "Second", "hall_new_second", new Vector2(0.14f, 0.48f), new Vector2(56f, 44f));
            PlaceOn(btn.transform, sheet, "Third", "hall_new_third", new Vector2(0.5f, 0.42f), new Vector2(52f, 36f));
        }

        static void PlaceOn(Transform parent, SpriteSheet sheet, string name, string frame, Vector2 anchor, Vector2 size)
        {
            RawImage raw = PcSkin.Slice(parent, name, sheet, frame, false);
            if (raw == null)
            {
                return;
            }

            var rt = raw.rectTransform;
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            raw.raycastTarget = false;
        }

        static void Place(Transform parent, SpriteSheet sheet, string name, string frame, Vector2 anchor, Vector2 size)
        {
            RawImage raw = PcSkin.Slice(parent, name, sheet, frame, false);
            if (raw == null)
            {
                return;
            }

            var rt = raw.rectTransform;
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
        }

        static void PlaceOld(Transform parent, string name, string frame, Vector2 anchor, Vector2 size)
        {
            Place(parent, PcSkin.Hall, name, frame, anchor, size);
        }

        static void Building(Transform parent, SpriteSheet sheet, string name, string frame, string title, Vector2 anchor, Vector2 size, UnityEngine.Events.UnityAction click)
        {
            var btn = UiKit.Button(parent, name, "", click, size);
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = anchor;
            rt.sizeDelta = size;
            var img = btn.GetComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.02f);
            RawImage raw = PcSkin.Slice(btn.transform, "Art", sheet, frame, true);
            if (raw != null)
            {
                raw.raycastTarget = false;
            }

            if (!string.IsNullOrEmpty(title))
            {
                RawImage cap = PcSkin.Slice(btn.transform, "Name", sheet, title, false);
                if (cap != null)
                {
                    var crt = cap.rectTransform;
                    crt.anchorMin = new Vector2(0.15f, 0.02f);
                    crt.anchorMax = new Vector2(0.85f, 0.18f);
                    crt.offsetMin = crt.offsetMax = Vector2.zero;
                    cap.raycastTarget = false;
                }
            }
        }

        static void Hotspot(Transform parent, string name, string frame, Vector2 anchor, UnityEngine.Events.UnityAction click)
        {
            var btn = UiKit.Button(parent, name, "", click, new Vector2(150f, 44f));
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = anchor;
            RawImage raw = PcSkin.Slice(btn.transform, "Art", PcSkin.Hall, frame, true);
            if (raw != null)
            {
                raw.raycastTarget = false;
                var img = btn.GetComponent<Image>();
                img.color = new Color(1f, 1f, 1f, 0.02f);
            }
        }

        static void Open(GameApp app, string id)
        {
            foreach (ModuleDef mod in ModuleCatalog.All)
            {
                if (mod.Id == id)
                {
                    app.ShowModule(mod);
                    return;
                }
            }
        }
    }

    public static class RoomScreen
    {
        public static readonly string[] PackedMaps = { "1056", "2001", "1005", "1010", "1029", "1048" };

        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            PcSkin.Warm(app.Loader);
            var bg = UiKit.Panel(safe, "Room", Color.black);
            PcSkin.Slice(bg.transform, "RoomBg", PcSkin.Hall, "hall_scene_bg_1", true);
            var top = UiKit.Button(bg.transform, "Back", "← 大厅", app.ShowHall, new Vector2(160f, 56f));
            var tr = top.GetComponent<RectTransform>();
            tr.anchorMin = tr.anchorMax = new Vector2(0.08f, 0.93f);

            List<MapInfo> maps = MapCatalog.Playable(app.Database);
            if (maps.Count == 0)
            {
                foreach (int id in MapCatalog.DiscoverCollisionIds(app.Loader))
                {
                    maps.Add(new MapInfo { Id = id, Name = "Map " + id, HasCollision = true });
                }
            }

            UiKit.Label(bg.transform, "Title", $"选地图 · {maps.Count} maps  ·  {PhoneNet.StatusLine()}", 26, Color.white, TextAnchor.MiddleCenter)
                .rectTransform.anchorMin = new Vector2(0.2f, 0.88f);
            bg.transform.Find("Title").GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 0.98f);
            bg.transform.Find("Title").GetComponent<RectTransform>().offsetMin = Vector2.zero;
            bg.transform.Find("Title").GetComponent<RectTransform>().offsetMax = Vector2.zero;

            InputField ip = UiKit.Field(bg.transform, "Ip", "LAN IP", new Vector2(280f, 48f));
            ip.text = PhoneNet.PeerHost;
            ip.characterLimit = 48;
            ip.GetComponent<RectTransform>().anchorMin = ip.GetComponent<RectTransform>().anchorMax = new Vector2(0.22f, 0.82f);
            var hostBtn = UiKit.Button(bg.transform, "Host", "开房 Host", () =>
            {
                PhoneNet.Seat = 0;
                PhoneNet.NetBattle = true;
                PhoneNet.ConnectFight("127.0.0.1");
            }, new Vector2(160f, 48f));
            hostBtn.GetComponent<RectTransform>().anchorMin = hostBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.38f, 0.82f);
            var joinBtn = UiKit.Button(bg.transform, "Join", "加入 LAN", () =>
            {
                PhoneNet.Seat = 1;
                PhoneNet.NetBattle = true;
                PhoneNet.ConnectHall(ip.text);
                PhoneNet.ConnectFight(ip.text);
            }, new Vector2(140f, 48f));
            joinBtn.GetComponent<RectTransform>().anchorMin = joinBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.52f, 0.82f);
            var solo = UiKit.Button(bg.transform, "Solo", "单机Bot", () =>
            {
                PhoneNet.NetBattle = false;
            }, new Vector2(120f, 48f));
            solo.GetComponent<RectTransform>().anchorMin = solo.GetComponent<RectTransform>().anchorMax = new Vector2(0.64f, 0.82f);
            var srvRoom = UiKit.Button(bg.transform, "SrvRoom", "创建房间", () =>
            {
                PhoneNet.CreateRoom(app.Profile.MapId > 0 ? app.Profile.MapId : 1056, app.Profile.Nick);
                PhoneNet.Seat = 0;
                PhoneNet.NetBattle = true;
                PhoneNet.ConnectFight("127.0.0.1");
            }, new Vector2(140f, 48f));
            srvRoom.GetComponent<RectTransform>().anchorMin = srvRoom.GetComponent<RectTransform>().anchorMax = new Vector2(0.78f, 0.82f);

            var scroll = UiKit.Scroll(bg.transform, "Maps");
            var srt = scroll.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.05f, 0.06f);
            srt.anchorMax = new Vector2(0.95f, 0.76f);
            srt.offsetMin = srt.offsetMax = Vector2.zero;

            if (maps.Count == 0)
            {
                foreach (string id in PackedMaps)
                {
                    maps.Add(new MapInfo { Id = int.Parse(id), Name = "Map " + id, HasCollision = true });
                }
            }

            foreach (MapInfo info in maps)
            {
                MapInfo local = info;
                string art = local.HasArt ? "" : "  (no PNG)";
                string mode = PhoneNet.NetBattle ? (PhoneNet.Seat == 0 ? "  ·  开房" : "  ·  加入") : "  ·  vs Bot";
                string caption = $"Map {local.Id}  {local.Name}{art}{mode}";
                var btn = UiKit.Button(scroll.content, "m" + local.Id, caption, () =>
                {
                    if (PhoneNet.NetBattle && PhoneNet.Seat == 0)
                    {
                        if (PhoneNet.Fight == null || !PhoneNet.Fight.Connected)
                        {
                            PhoneNet.ConnectFight("127.0.0.1");
                        }

                        PhoneNet.SendStart(local.Id);
                    }
                    else if (!PhoneNet.NetBattle)
                    {
                        // Solo vs Bot.
                        app.ShowBattle(local.Id);
                    }
                    // LAN joiner (Seat=1) or host (Seat=0) will wait for
                    // server FightStart broadcast (GameApp.PumpFight).
                }, new Vector2(0f, 96f));
                var le = btn.gameObject.AddComponent<LayoutElement>();
                le.preferredHeight = 96f;
                le.flexibleWidth = 1f;
                Texture2D thumb = PcSkin.MapThumb(app.Loader, local.Id);
                if (thumb != null)
                {
                    var thumbGo = new GameObject("Thumb", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    thumbGo.transform.SetParent(btn.transform, false);
                    var trt = thumbGo.GetComponent<RectTransform>();
                    trt.anchorMin = new Vector2(0f, 0.08f);
                    trt.anchorMax = new Vector2(0.22f, 0.92f);
                    trt.offsetMin = trt.offsetMax = Vector2.zero;
                    var raw = thumbGo.GetComponent<RawImage>();
                    raw.texture = thumb;
                    raw.raycastTarget = false;
                    var captionRt = btn.transform.Find("Caption") as RectTransform;
                    if (captionRt != null)
                    {
                        captionRt.offsetMin = new Vector2(btn.GetComponent<RectTransform>().rect.width * 0.22f, 0f);
                    }
                }
            }
        }
    }
}
