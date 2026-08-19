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
            var hallBg = PcSkin.Slice(bg.transform, "HallBg", PcSkin.Hall, "hall_scene_bg_0", true);
            if (hallBg == null)
            {
                PcSkin.Backdrop(
                    bg.transform,
                    app.Loader,
                    GamePaths.PathCombine("Flash", "ui", "cn_trad", "starling", "hall_scene", "hall_scene.png"));
            }

            Place(bg.transform, "Church", "hall_scene_church_build", new Vector2(0.18f, 0.38f), new Vector2(280f, 230f));
            Place(bg.transform, "Poster", "hall_scene_image_poster", new Vector2(0.88f, 0.62f), new Vector2(160f, 140f));
            Place(bg.transform, "Flower", "hall_scene_bg_flower2", new Vector2(0.72f, 0.28f), new Vector2(70f, 110f));

            Hotspot(bg.transform, "roomList", "hall_scene_build_title_roomList", new Vector2(0.5f, 0.42f), () => app.ShowRoom());
            Hotspot(bg.transform, "dungeon", "hall_scene_build_title_dungeon", new Vector2(0.32f, 0.55f), () => Open(app, "dungeon"));
            Hotspot(bg.transform, "labyrinth", "hall_scene_build_title_labyrinth", new Vector2(0.68f, 0.5f), () => Open(app, "labyrinth"));
            Hotspot(bg.transform, "boss", "hall_scene_build_title_cryptBoss", new Vector2(0.22f, 0.62f), () => Open(app, "worldboss"));
            Hotspot(bg.transform, "church", "hall_scene_build_title_church", new Vector2(0.12f, 0.55f), () => Open(app, "church"));
            Hotspot(bg.transform, "home", "hall_scene_build_title_home", new Vector2(0.82f, 0.4f), () => Open(app, "character"));
            Hotspot(bg.transform, "ring", "hall_scene_build_title_ringStation", new Vector2(0.6f, 0.62f), () => Open(app, "rank"));

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
                UiKit.Button(scroll.content, local.Id, local.Title, () => app.ShowModule(local), grid.cellSize);
            }
        }

        static void Place(Transform parent, string name, string frame, Vector2 anchor, Vector2 size)
        {
            RawImage raw = PcSkin.Slice(parent, name, PcSkin.Hall, frame, false);
            if (raw == null)
            {
                return;
            }

            var rt = raw.rectTransform;
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
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
            var hostBtn = UiKit.Button(bg.transform, "Host", "开房 Fight", () =>
            {
                PhoneNet.Seat = 0;
                PhoneNet.NetBattle = true;
                PhoneNet.ConnectFight("127.0.0.1");
            }, new Vector2(160f, 48f));
            hostBtn.GetComponent<RectTransform>().anchorMin = hostBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.42f, 0.82f);
            var joinBtn = UiKit.Button(bg.transform, "Join", "加入", () =>
            {
                PhoneNet.Seat = 1;
                PhoneNet.NetBattle = true;
                PhoneNet.ConnectHall(ip.text);
                PhoneNet.ConnectFight(ip.text);
            }, new Vector2(140f, 48f));
            joinBtn.GetComponent<RectTransform>().anchorMin = joinBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.58f, 0.82f);
            var solo = UiKit.Button(bg.transform, "Solo", "单机Bot", () =>
            {
                PhoneNet.NetBattle = false;
            }, new Vector2(140f, 48f));
            solo.GetComponent<RectTransform>().anchorMin = solo.GetComponent<RectTransform>().anchorMax = new Vector2(0.72f, 0.82f);

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

                    app.ShowBattle(local.Id);
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
