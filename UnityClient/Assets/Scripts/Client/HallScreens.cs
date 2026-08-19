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
            var bg = UiKit.Panel(safe, "Login", new Color(0.07f, 0.1f, 0.16f, 1f));
            TryHallBackdrop(app, bg.transform);

            var title = UiKit.Label(bg.transform, "Title", "弹弹堂  ·  Gun Mobile", 48, new Color(1f, 0.85f, 0.35f), TextAnchor.MiddleCenter);
            var titleRt = title.rectTransform;
            titleRt.anchorMin = new Vector2(0.1f, 0.72f);
            titleRt.anchorMax = new Vector2(0.9f, 0.88f);
            titleRt.offsetMin = titleRt.offsetMax = Vector2.zero;

            var sub = UiKit.Label(bg.transform, "Sub", "PC data · landscape · Android & iOS", 22, new Color(0.8f, 0.85f, 0.95f), TextAnchor.MiddleCenter);
            sub.rectTransform.anchorMin = new Vector2(0.1f, 0.64f);
            sub.rectTransform.anchorMax = new Vector2(0.9f, 0.72f);
            sub.rectTransform.offsetMin = sub.rectTransform.offsetMax = Vector2.zero;

            InputField nick = UiKit.Field(bg.transform, "Nick", "Nickname / 昵称", new Vector2(560f, 72f));
            nick.text = app.Profile.Nick;
            var nickRt = nick.GetComponent<RectTransform>();
            nickRt.anchorMin = nickRt.anchorMax = new Vector2(0.5f, 0.48f);
            nickRt.pivot = new Vector2(0.5f, 0.5f);

            Button enter = UiKit.Button(bg.transform, "Enter", "进入大厅  Enter Hall", () =>
            {
                app.Profile.Nick = string.IsNullOrWhiteSpace(nick.text) ? "Player" : nick.text.Trim();
                app.ShowHall();
            }, new Vector2(560f, 80f));
            var enterRt = enter.GetComponent<RectTransform>();
            enterRt.anchorMin = enterRt.anchorMax = new Vector2(0.5f, 0.32f);
        }

        static void TryHallBackdrop(GameApp app, Transform parent)
        {
            string path = GamePaths.PathCombine("Flash", "ui", "cn_trad", "starling", "hall_scene", "hall_scene.png");
            if (!app.Loader.TryReadBytes(path, out byte[] bytes))
            {
                return;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            if (!tex.LoadImage(bytes))
            {
                Object.Destroy(tex);
                return;
            }

            var go = new GameObject("Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            go.transform.SetAsFirstSibling();
            UiKit.Stretch(go);
            var raw = go.GetComponent<RawImage>();
            raw.texture = tex;
            raw.color = new Color(1f, 1f, 1f, 0.35f);
            raw.raycastTarget = false;
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
            var bg = UiKit.Panel(safe, "Hall", new Color(0.08f, 0.11f, 0.18f, 1f));
            LoginScreenBackdrop(app, bg.transform);

            var top = new GameObject("Top", typeof(RectTransform), typeof(Image));
            top.transform.SetParent(bg.transform, false);
            var topRt = top.GetComponent<RectTransform>();
            topRt.anchorMin = new Vector2(0f, 0.86f);
            topRt.anchorMax = Vector2.one;
            topRt.offsetMin = topRt.offsetMax = Vector2.zero;
            top.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.45f);
            PlayerProfile p = app.Profile;
            UiKit.Label(top.transform, "Info",
                $"{p.Nick}  Lv.{p.Level} VIP{p.VipLevel}  ATK {p.Attack} DEF {p.Defence}  Gold {p.Gold}  Honor {p.Honor}  {p.Win}W/{p.Lose}L  {(string.IsNullOrEmpty(p.ConsortiaName) ? "" : p.ConsortiaName)}",
                26, Color.white, TextAnchor.MiddleLeft);
            UiKit.Stretch(top.transform.Find("Info").gameObject).offsetMin = new Vector2(24f, 0f);

            var fight = UiKit.Button(top.transform, "Fight", "开战", app.ShowRoom, new Vector2(180f, 56f));
            var frt = fight.GetComponent<RectTransform>();
            frt.anchorMin = frt.anchorMax = new Vector2(0.92f, 0.5f);

            var scroll = UiKit.Scroll(bg.transform, "Modules");
            var srt = scroll.GetComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = new Vector2(1f, 0.86f);
            srt.offsetMin = new Vector2(8f, 8f);
            srt.offsetMax = new Vector2(-8f, -8f);

            var gridGo = scroll.content.gameObject;
            Object.Destroy(gridGo.GetComponent<VerticalLayoutGroup>());
            var grid = gridGo.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(240f, 88f);
            grid.spacing = new Vector2(12f, 12f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 6;
            grid.padding = new RectOffset(8, 8, 8, 8);
            gridGo.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (ModuleDef mod in ModuleCatalog.All)
            {
                ModuleDef local = mod;
                UiKit.Button(scroll.content, local.Id, local.Title, () => app.ShowModule(local), grid.cellSize);
            }
        }

        static void LoginScreenBackdrop(GameApp app, Transform parent)
        {
            string path = GamePaths.PathCombine("Flash", "ui", "cn_trad", "starling", "hall_scene", "hall_scene.png");
            if (!app.Loader.TryReadBytes(path, out byte[] bytes))
            {
                return;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            if (!tex.LoadImage(bytes))
            {
                Object.Destroy(tex);
                return;
            }

            var go = new GameObject("HallArt", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            go.transform.SetAsFirstSibling();
            UiKit.Stretch(go);
            var raw = go.GetComponent<RawImage>();
            raw.texture = tex;
            raw.uvRect = new Rect(0f, 0.08f, 1f, 0.84f);
            raw.color = Color.white;
            raw.raycastTarget = false;
        }
    }

    public static class RoomScreen
    {
        public static readonly string[] PackedMaps = { "1056", "2001", "1005", "1010", "1029", "1048" };

        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Room", new Color(0.06f, 0.08f, 0.12f, 1f));
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
                }, new Vector2(0f, 80f));
                var le = btn.gameObject.AddComponent<LayoutElement>();
                le.preferredHeight = 80f;
                le.flexibleWidth = 1f;
            }
        }
    }
}
