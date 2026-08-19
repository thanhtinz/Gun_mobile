using System.Collections.Generic;
using GunMobile.Core;
using GunMobile.Res;
using UnityEngine;
using UnityEngine.UI;

namespace GunMobile.Client
{
    public static class ShopScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Shop", new Color(0.07f, 0.08f, 0.12f, 1f));
            Header(bg.transform, app, "商城 · ShopItemList");

            var scroll = BodyScroll(bg.transform);
            int shown = 0;
            foreach (ShopOffer offer in app.Database.Shop)
            {
                if (!offer.CanBuy && offer.APrice1 != -1 && offer.APrice1 != -2)
                {
                    continue;
                }

                if (offer.AValue1 <= 0)
                {
                    continue;
                }

                ItemTemplate item = app.Database.GetItem(offer.TemplateId);
                string name = item != null ? item.Name : ("#" + offer.TemplateId);
                string cur = offer.APrice1 == -2 ? "点券" : "金币";
                string cap = $"{name}  {offer.AValue1}{cur}  {StatLine(item)}";
                ShopOffer local = offer;
                var btn = UiKit.Button(scroll.content, "s" + offer.Id, cap, () => Buy(app, local), new Vector2(0f, 72f));
                btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
                shown++;
                if (shown >= 200)
                {
                    AddNote(scroll.content, $"… {app.Database.Shop.Count - shown} more offers in PC table");
                    break;
                }
            }

            if (shown == 0)
            {
                AddNote(scroll.content, "Shop table missing. Unpack Request/shopitemlist_out.xml.");
            }
        }

        static void Buy(GameApp app, ShopOffer offer)
        {
            bool gift = offer.APrice1 == -2;
            int price = offer.AValue1;
            if (gift)
            {
                if (app.Profile.Gift < price)
                {
                    Debug.Log("Not enough gift");
                    return;
                }

                app.Profile.Gift -= price;
            }
            else
            {
                if (app.Profile.Gold < price)
                {
                    Debug.Log("Not enough gold");
                    return;
                }

                app.Profile.Gold -= price;
            }

            app.Profile.AddItem(offer.TemplateId, 1);
            app.Profile.Save();
            Show(app.SafeArea, app);
        }

        static string StatLine(ItemTemplate item)
        {
            if (item == null)
            {
                return "";
            }

            return $"ATK{item.Attack} DEF{item.Defence}";
        }

        public static void Header(Transform bg, GameApp app, string title)
        {
            var back = UiKit.Button(bg, "Back", "← 大厅", app.ShowHall, new Vector2(160f, 56f));
            back.GetComponent<RectTransform>().anchorMin = back.GetComponent<RectTransform>().anchorMax = new Vector2(0.08f, 0.93f);
            UiKit.Label(bg, "Title", title + $"   Gold {app.Profile.Gold}  Gift {app.Profile.Gift}", 28, new Color(1f, 0.9f, 0.5f), TextAnchor.MiddleCenter)
                .rectTransform.anchorMin = new Vector2(0.18f, 0.88f);
            bg.Find("Title").GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 0.98f);
            bg.Find("Title").GetComponent<RectTransform>().offsetMin = Vector2.zero;
            bg.Find("Title").GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }

        public static ScrollRect BodyScroll(Transform bg)
        {
            var scroll = UiKit.Scroll(bg, "Rows");
            var srt = scroll.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.03f, 0.04f);
            srt.anchorMax = new Vector2(0.97f, 0.86f);
            return scroll;
        }

        public static void AddNote(Transform parent, string text)
        {
            var label = UiKit.Label(parent, "row", text, 22, Color.white);
            label.gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;
        }
    }

    public static class BagScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Bag", new Color(0.07f, 0.08f, 0.12f, 1f));
            ShopScreen.Header(bg.transform, app, "背包 · 装备");
            var scroll = ShopScreen.BodyScroll(bg.transform);
            app.Profile.EnsureStarterBag();
            if (app.Profile.Bag.Count == 0)
            {
                ShopScreen.AddNote(scroll.content, "Empty bag. Buy from 商城.");
                return;
            }

            foreach (BagItem slot in app.Profile.Bag)
            {
                BagItem local = slot;
                ItemTemplate item = app.Database.GetItem(slot.TemplateId);
                string name = item != null ? item.Name : ("#" + slot.TemplateId);
                bool eq = IsEquipped(app.Profile, slot.TemplateId);
                string cap = $"{name}  x{slot.Count}  {(eq ? "[已穿]" : item != null && item.CanEquip ? "装备" : "")}  {item?.Description ?? ""}";
                if (cap.Length > 80)
                {
                    cap = cap.Substring(0, 80) + "…";
                }

                var btn = UiKit.Button(scroll.content, "b" + slot.TemplateId, cap, () =>
                {
                    if (item != null && app.Profile.Equip(item))
                    {
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
                    }
                }, new Vector2(0f, 72f));
                btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
            }
        }

        static bool IsEquipped(PlayerProfile p, int id)
        {
            return p.EquipHead == id || p.EquipHair == id || p.EquipFace == id || p.EquipCloth == id || p.EquipGlass == id || p.EquipWeapon == id;
        }
    }

    public static class QuestScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Quest", new Color(0.07f, 0.08f, 0.12f, 1f));
            ShopScreen.Header(bg.transform, app, "任务 · QuestList");
            var scroll = ShopScreen.BodyScroll(bg.transform);
            int shown = 0;
            foreach (QuestInfo q in app.Database.Quests)
            {
                if (q.NeedMinLevel > app.Profile.Level)
                {
                    continue;
                }

                if (q.NeedMaxLevel > 0 && app.Profile.Level > q.NeedMaxLevel)
                {
                    continue;
                }

                bool done = app.Profile.QuestDone(q.Id);
                bool acc = app.Profile.QuestAccepted(q.Id);
                string state = done ? "已完成" : acc ? "领取奖励" : "接取";
                string cap = $"{q.Title}  [{state}]  Gold+{q.RewardGold} GP+{q.RewardGp}";
                QuestInfo local = q;
                var btn = UiKit.Button(scroll.content, "q" + q.Id, cap, () => Toggle(app, local), new Vector2(0f, 72f));
                btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
                shown++;
                if (shown >= 120)
                {
                    ShopScreen.AddNote(scroll.content, "… more quests in PC QuestList.xml");
                    break;
                }
            }
        }

        static void Toggle(GameApp app, QuestInfo q)
        {
            if (app.Profile.QuestDone(q.Id) && !q.CanRepeat)
            {
                return;
            }

            if (!app.Profile.QuestAccepted(q.Id))
            {
                app.Profile.AcceptedQuests.Add(q.Id);
            }
            else
            {
                app.Profile.AcceptedQuests.Remove(q.Id);
                if (!app.Profile.CompletedQuests.Contains(q.Id))
                {
                    app.Profile.CompletedQuests.Add(q.Id);
                }

                app.Profile.Gold += Mathf.Max(50, q.RewardGold);
                app.Profile.Level = Mathf.Min(70, app.Profile.Level + (q.RewardGp > 0 ? 0 : 0));
            }

            app.Profile.Save();
            Show(app.SafeArea, app);
        }
    }

    public static class CharacterScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Hero", new Color(0.07f, 0.09f, 0.14f, 1f));
            ShopScreen.Header(bg.transform, app, "角色");
            app.Profile.RecalcStats(app.Database);
            PlayerProfile p = app.Profile;
            string text =
                $"{p.Nick}  {(p.Sex == 1 ? "♂" : "♀")}  Lv.{p.Level}  VIP{p.VipLevel}\n" +
                $"ATK {p.Attack}  DEF {p.Defence}  AGI {p.Agility}  LUCK {p.Luck}  HP {p.Hp}\n" +
                $"Gold {p.Gold}  Gift {p.Gift}  Honor {p.Honor}  Texp {p.Texp}  {p.Win}W/{p.Lose}L\n" +
                $"Weapon #{p.EquipWeapon}  Cloth #{p.EquipCloth}  Head #{p.EquipHead}\n" +
                $"Pet #{p.PetId}  Title #{p.TitleId}  Mount {p.MountGrade}  迷宫{p.LabyrinthFloor}\n" +
                EquipName(app, p.EquipWeapon) + " / " + EquipName(app, p.EquipCloth);

            var label = UiKit.Label(bg.transform, "Sheet", text, 28, Color.white, TextAnchor.UpperLeft);
            var rt = label.rectTransform;
            rt.anchorMin = new Vector2(0.08f, 0.12f);
            rt.anchorMax = new Vector2(0.55f, 0.82f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            TryIcon(app, bg.transform, p.EquipCloth, "cloth", new Vector2(0.72f, 0.55f));
            TryIcon(app, bg.transform, p.EquipHead, "head", new Vector2(0.72f, 0.78f));
            TryIcon(app, bg.transform, p.EquipWeapon, "weapon", new Vector2(0.88f, 0.55f));

            var bag = UiKit.Button(bg.transform, "OpenBag", "打开背包", () => BagScreen.Show(safe, app), new Vector2(240f, 64f));
            bag.GetComponent<RectTransform>().anchorMin = bag.GetComponent<RectTransform>().anchorMax = new Vector2(0.78f, 0.18f);
        }

        static string EquipName(GameApp app, int id)
        {
            ItemTemplate t = app.Database.GetItem(id);
            return t != null ? t.Name : (id == 0 ? "—" : "#" + id);
        }

        static void TryIcon(GameApp app, Transform parent, int templateId, string slot, Vector2 anchor)
        {
            ItemTemplate item = app.Database.GetItem(templateId);
            if (item == null || string.IsNullOrEmpty(item.Pic))
            {
                return;
            }

            string sex = app.Profile.Sex == 1 ? "m" : "f";
            string[] paths =
            {
                GamePaths.PathCombine("Resource", "image", "equip", sex, slot, item.Pic, "icon_1.png"),
                GamePaths.PathCombine("Resource", "image", "equip", sex, slot, item.Pic.ToLowerInvariant(), "icon_1.png"),
                GamePaths.PathCombine("Resource", "image", "arm", item.Pic, "00.png"),
            };
            byte[] bytes = null;
            foreach (string path in paths)
            {
                if (app.Loader.TryReadBytes(path, out bytes))
                {
                    break;
                }
            }

            if (bytes == null)
            {
                return;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(bytes))
            {
                Object.Destroy(tex);
                return;
            }

            var go = new GameObject(slot, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = anchor;
            rt.sizeDelta = new Vector2(160f, 160f);
            go.GetComponent<RawImage>().texture = tex;
        }
    }

    public static class SignInScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Sign", new Color(0.08f, 0.1f, 0.14f, 1f));
            ShopScreen.Header(bg.transform, app, "签到 · TS_EveryDaySignIn");
            int today = System.DateTime.Now.DayOfYear;
            bool done = app.Profile.LastSignDay == today;
            var scroll = ShopScreen.BodyScroll(bg.transform);
            if (app.Database.SignIn.Count == 0)
            {
                ShopScreen.AddNote(scroll.content, done ? "今天已经签到过了。" : "表缺，金币 +1200");
                if (!done)
                {
                    SysUi.Row(scroll.content, "Go", "签到", () =>
                    {
                        app.Profile.LastSignDay = today;
                        app.Profile.Gold += 1200;
                        app.Profile.Gift += 20;
                        app.Profile.Save();
                        Show(safe, app);
                    });
                }

                return;
            }

            int nextDay = app.Profile.SignIndex >= 28 ? 1 : app.Profile.SignIndex + 1;
            ShopScreen.AddNote(scroll.content, done
                ? $"今天已签。进度 {Mathf.Min(app.Profile.SignIndex, 28)}/28"
                : $"领取第 {nextDay} 天（表 28 日循环）");
            foreach (SignReward r in app.Database.SignIn)
            {
                SignReward local = r;
                string name = r.TemplateId < 0 ? "金币" : SysUi.ItemName(app, r.TemplateId);
                bool claimed = app.Profile.SignIndex >= 28 || r.Day <= app.Profile.SignIndex;
                bool can = !done && r.Day == nextDay;
                string cap = $"第{r.Day}天  {name} x{r.Count}  {(claimed ? "[已领]" : can ? "领取" : "")}";
                SysUi.Row(scroll.content, "d" + r.Day, cap, () =>
                {
                    if (!can)
                    {
                        return;
                    }

                    app.Profile.GrantTemplate(local.TemplateId, local.Count);
                    app.Profile.LastSignDay = today;
                    app.Profile.SignIndex = local.Day;
                    app.Profile.Save();
                    Show(safe, app);
                });
            }
        }
    }

    public static class BattleResultScreen
    {
        public static void Show(RectTransform safe, GameApp app, bool win, int gold, string detail)
        {
            var battle = safe.GetComponent<BattleHost>();
            if (battle != null)
            {
                Object.Destroy(battle);
            }

            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Result", win ? new Color(0.08f, 0.16f, 0.1f, 1f) : new Color(0.16f, 0.07f, 0.08f, 1f));
            UiKit.Label(bg.transform, "Title", win ? "胜利" : "惜败", 48, new Color(1f, 0.9f, 0.4f), TextAnchor.MiddleCenter)
                .rectTransform.anchorMin = new Vector2(0.1f, 0.62f);
            bg.transform.Find("Title").GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.82f);
            UiKit.Label(bg.transform, "Body", detail + $"\n金币 {(win ? "+" : "")}{gold}\n{app.Profile.Win}W / {app.Profile.Lose}L", 28, Color.white, TextAnchor.MiddleCenter)
                .rectTransform.anchorMin = new Vector2(0.1f, 0.32f);
            bg.transform.Find("Body").GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.62f);
            var hall = UiKit.Button(bg.transform, "Hall", "返回大厅", app.ShowHall, new Vector2(280f, 72f));
            hall.GetComponent<RectTransform>().anchorMin = hall.GetComponent<RectTransform>().anchorMax = new Vector2(0.35f, 0.18f);
            var again = UiKit.Button(bg.transform, "Again", "再战", app.ShowRoom, new Vector2(280f, 72f));
            again.GetComponent<RectTransform>().anchorMin = again.GetComponent<RectTransform>().anchorMax = new Vector2(0.65f, 0.18f);
        }
    }

    public static class SettingsScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Set", new Color(0.07f, 0.08f, 0.12f, 1f));
            ShopScreen.Header(bg.transform, app, "设置");
            string lang = app.Config != null ? app.Config.Language : "cn_trad";
            string info =
                $"Language {lang}\n" +
                $"Suicide {app.Config?.SuicideTime ?? 120}s  Frame {app.Config?.FrameTimeOverMs ?? 67}ms\n" +
                $"Bundle com.gunmobile.client  landscape Android+iOS\n" +
                $"Physics 25fps gravity {GunMobile.Logic.PcPhysics.GravityPerFrame}/frame";
            var label = UiKit.Label(bg.transform, "Info", info, 26, Color.white, TextAnchor.UpperLeft);
            label.rectTransform.anchorMin = new Vector2(0.1f, 0.35f);
            label.rectTransform.anchorMax = new Vector2(0.9f, 0.82f);

            InputField nick = UiKit.Field(bg.transform, "Nick", "Nickname", new Vector2(480f, 64f));
            nick.text = app.Profile.Nick;
            nick.GetComponent<RectTransform>().anchorMin = nick.GetComponent<RectTransform>().anchorMax = new Vector2(0.4f, 0.28f);
            var save = UiKit.Button(bg.transform, "SaveNick", "保存昵称", () =>
            {
                app.Profile.Nick = string.IsNullOrWhiteSpace(nick.text) ? app.Profile.Nick : nick.text.Trim();
                app.Profile.Save();
                app.ShowHall();
            }, new Vector2(220f, 64f));
            save.GetComponent<RectTransform>().anchorMin = save.GetComponent<RectTransform>().anchorMax = new Vector2(0.72f, 0.28f);
        }
    }

    public static class MailScreen
    {
        public static void Show(RectTransform safe, GameApp app, string title, string body)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.Panel(safe, "Mail", new Color(0.07f, 0.08f, 0.12f, 1f));
            ShopScreen.Header(bg.transform, app, title);
            var label = UiKit.Label(bg.transform, "Body", body, 26, Color.white, TextAnchor.UpperLeft);
            label.rectTransform.anchorMin = new Vector2(0.08f, 0.1f);
            label.rectTransform.anchorMax = new Vector2(0.92f, 0.82f);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
        }
    }
}
