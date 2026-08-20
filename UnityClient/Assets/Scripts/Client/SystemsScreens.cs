using System.Collections.Generic;
using GunMobile.Core;
using GunMobile.Net;
using GunMobile.Res;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GunMobile.Client
{
    static class SysUi
    {
        public static Transform Begin(RectTransform safe, GameApp app, string title)
        {
            UiKit.ClearChildren(safe);
            PcSkin.Warm(app.Loader);
            var bg = UiKit.Panel(safe, "Sys", Color.black);
            PcSkin.Slice(bg.transform, "Bg", PcSkin.Hall, "hall_scene_bg_1", true);
            ShopScreen.Header(bg.transform, app, title);
            return ShopScreen.BodyScroll(bg.transform).content;
        }

        public static Button Row(Transform content, string id, string cap, UnityAction click)
        {
            var btn = UiKit.Button(content, id, cap, click, new Vector2(0f, 72f));
            btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
            return btn;
        }

        public static void Note(Transform content, string text)
        {
            ShopScreen.AddNote(content, text);
        }

        public static void Fight(GameApp app, int mapId, int npcId, bool labyrinth = false)
        {
            app.Profile.PendingLabyrinth = labyrinth ? 1 : 0;
            app.Profile.Save();
            PhoneNet.EnsureConnected(app.Profile.Nick);
            if (PhoneNet.BeginPveFight(mapId, npcId, labyrinth))
            {
                return;
            }

            PhoneNet.NetBattle = false;
            app.ShowBattle(mapId, npcId);
        }

        public static XmlResultTable Table(GameApp app, string path)
        {
            if (!app.Loader.TryReadBytes(path, out byte[] bytes))
            {
                return null;
            }

            try
            {
                return XmlResultTable.LoadBytes(bytes);
            }
            catch
            {
                return null;
            }
        }

        public static string ItemName(GameApp app, int templateId)
        {
            ItemTemplate t = app.Database.GetItem(templateId);
            return t != null ? t.Name : "#" + templateId;
        }
    }

    public static class PetScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "宠物 · 出战加成");
            SysUi.Note(body, app.Profile.PetId == 0 ? "点选一只宠物出战。" : "出战 #" + app.Profile.PetId);
            int n = 0;
            foreach (PetInfo pet in app.Database.Pets.Values)
            {
                PetInfo local = pet;
                bool on = app.Profile.PetId == pet.TemplateId;
                var btn = SysUi.Row(body, "p" + pet.TemplateId,
                    $"{(on ? "[出战] " : "")}{pet.Name}  ATK{pet.Attack} DEF{pet.Defence} HP{pet.Blood}",
                    () =>
                    {
                        PhoneNet.SelectPet(local.TemplateId);
                    });
                PcArt.Decorate(btn.transform, PcArt.PetIcon(app.Loader, pet.Pic));
                n++;
                if (n >= 120)
                {
                    SysUi.Note(body, $"… {app.Database.Pets.Count - n} more pets");
                    break;
                }
            }

            if (n == 0)
            {
                SysUi.Note(body, "Missing pettemplateinfo.xml");
            }
        }
    }

    public static class CardScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "卡片");
            CardMainLevelInfo main = app.Database != null ? app.Database.GetCardMainLevel(app.Profile.CardMainLevel) : null;
            CardMainLevelInfo next = app.Database != null ? app.Database.GetCardMainLevel(app.Profile.CardMainLevel + 1) : null;
            SysUi.Note(body, "CardMain Lv" + app.Profile.CardMainLevel +
                (main != null ? $"  ATK+{main.Attack} DEF+{main.Defence}" : ""));
            if (next != null && next.NeedItem1Count > 0)
                SysUi.Row(body, "cardMainUp", $"升级CardMain  {next.NeedItem1Count} 金币", () => PhoneNet.UpgradeCardMain());
            app.Profile.EnsureOwnedCards();
            int suitN = 0;
            foreach (CardSuitInfo suit in app.Database.CardSuits)
            {
                bool complete = suit.NeedCardTempIds != null && suit.NeedCardTempIds.Length > 0;
                if (complete)
                    for (int i = 0; i < suit.NeedCardTempIds.Length; i++)
                        if (!GameDatabase.ListHasInt(app.Profile.OwnedCardTemplateIds, suit.NeedCardTempIds[i])) { complete = false; break; }
                SysUi.Note(body, $"{(complete ? "[套装] " : "")}{suit.SuitName}  ATK+{suit.Attack} HP+{suit.Hp}");
                if (++suitN >= 12) break;
            }
            int n = 0;
            foreach (CardInfo card in app.Database.Cards)
            {
                if (card.AddAttack + card.AddDefend + card.AddAgility + card.AddLucky <= 0)
                {
                    continue;
                }

                CardInfo local = card;
                bool on = app.Profile.CardId == card.Id;
                SysUi.Row(body, "c" + card.Id,
                    $"{(on ? "[装备] " : "")}Card {card.CardId}  ATK+{card.AddAttack} DEF+{card.AddDefend} AGI+{card.AddAgility}",
                    () =>
                    {
                        PhoneNet.SelectCard(local.Id);
                    });
                n++;
                if (n >= 100)
                {
                    break;
                }
            }

            if (n == 0)
            {
                SysUi.Note(body, "No card bonuses in cardtemplateinfo.xml");
            }
        }
    }

    public static class TitleScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "称号");
            int n = 0;
            foreach (TitleInfo t in app.Database.Titles.Values)
            {
                TitleInfo local = t;
                bool on = app.Profile.TitleId == t.Id;
                var btn = SysUi.Row(body, "t" + t.Id,
                    $"{(on ? "[佩戴] " : "")}{t.Name}  ATK+{t.Att} DEF+{t.Def}",
                    () =>
                    {
                        PhoneNet.SelectTitle(local.Id);
                    });
                PcArt.Decorate(btn.transform, PcArt.TitleBanner(app.Loader, t.Pic), 0.22f);
                n++;
                if (n >= 80)
                {
                    SysUi.Note(body, $"… {app.Database.Titles.Count - n} more titles");
                    break;
                }
            }
        }
    }

    public static class TotemScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "图腾  Honor " + app.Profile.Honor);
            int n = 0;
            foreach (TotemInfo t in app.Database.Totems.Values)
            {
                TotemInfo local = t;
                bool on = app.Profile.TotemId == t.Id;
                SysUi.Row(body, "to" + t.Id,
                    $"{(on ? "[激活] " : "")}#{t.Id}  ATK+{t.AddAttack} HP+{t.AddBlood}  荣誉{t.ConsumeHonor}",
                    () =>
                    {
                        PhoneNet.BuyTotem(local.Id);
                    });
                n++;
                if (n >= 80)
                {
                    break;
                }
            }
        }
    }

    public static class MountScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "坐骑  Grade " + app.Profile.MountGrade);
            int cost = app.Database != null ? app.Database.MountUpgradeCost(app.Profile.MountGrade) : 0;
            if (cost > 0)
            {
                SysUi.Row(body, "up", $"升级坐骑  {cost} 金币", () => PhoneNet.UpgradeMount());
            }

            foreach (MountGrade m in app.Database.Mounts.Values)
            {
                MountGrade local = m;
                bool on = app.Profile.MountGrade >= m.Grade;
                SysUi.Note(body, $"{(on ? "[已达成] " : "")}Grade {m.Grade}  HP+{m.AddBlood} DMG+{m.AddDamage} MAG{m.MagicAttack}");
            }

            if (app.Database.Mounts.Count == 0)
            {
                SysUi.Note(body, "Missing mounttemplateOUT.xml");
            }
        }
    }

    public static class ElfScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "精灵");
            app.Profile.SyncElfIntimacyLevel(app.Database);
            SysUi.Note(body, $"亲密度 Lv{app.Profile.ElfIntimacyLevel}  EXP {app.Profile.ElfIntimacyExp}  今日 {app.Profile.ElfIntimacyActions}/10");
            SysUi.Row(body, "elfGift", "赠送礼物 +15 EXP", () => PhoneNet.ElfIntimacyAction("gift"));
            SysUi.Row(body, "elfTalk", "互动 +10 EXP", () => PhoneNet.ElfIntimacyAction("interact"));
            foreach (ElfInfo e in app.Database.Elves.Values)
            {
                ElfInfo local = e;
                bool on = app.Profile.ElfId == e.TemplateId;
                SysUi.Row(body, "e" + e.TemplateId,
                    $"{(on ? "[跟随] " : "")}{e.Name}  ★{e.StarLevel}  ATK~{e.AttackHint} HP~{e.HpHint}",
                    () =>
                    {
                        PhoneNet.SelectElf(local.TemplateId);
                    });
            }

            if (app.Database.Elves.Count == 0)
            {
                SysUi.Note(body, "Missing TS_ElfTemplate.xml");
            }
        }
    }

    public static class JampsScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "探险手册");
            JampsManualInfo manual = app.Database != null ? app.Database.GetJampsManual(app.Profile.JampsManualLevel) : null;
            JampsManualInfo nextManual = app.Database != null ? app.Database.GetJampsManual(app.Profile.JampsManualLevel + 1) : null;
            SysUi.Note(body, $"手册 Lv{app.Profile.JampsManualLevel}  {(manual != null ? manual.Name : "")}" +
                (nextManual != null ? "  → " + nextManual.Name : ""));
            if (nextManual != null) SysUi.Row(body, "jampsUp", "升级手册", () => PhoneNet.UpgradeJamps());
            foreach (JampsChapterInfo chapter in app.Database.JampsChapters.Values)
            {
                SysUi.Note(body, $"— {chapter.Name} —");
                int shown = 0;
                foreach (KeyValuePair<int, JampsPageInfo> kv in app.Database.JampsPages)
                {
                    if (kv.Value.ChapterId != chapter.Id) continue;
                    JampsPageInfo page = kv.Value;
                    int owned = app.Database.CountJampsDebrisForPage(app.Profile.JampsDebrisOwned, page.Id);
                    bool collected = app.Profile.HasJampsPageCollected(page.Id);
                    bool activated = app.Profile.HasJampsPageActivated(page.Id);
                    SysUi.Note(body, $"{(activated ? "[已激活]" : collected ? "[已收集]" : $"碎片 {owned}/{page.DebrisCount}")} {page.Name}");
                    if (!collected && owned >= page.DebrisCount)
                    {
                        int pageId = page.Id;
                        SysUi.Row(body, "jc" + pageId, $"收集 {page.Name}", () => PhoneNet.JampsClaimPage("collect", pageId, 0));
                    }
                    else if (collected && !activated)
                    {
                        int pageId = page.Id;
                        SysUi.Row(body, "ja" + pageId, $"激活 {page.Name}  {page.ActivateCurrency}金", () => PhoneNet.JampsClaimPage("activate", pageId, 0));
                    }
                    if (++shown >= 5) break;
                }
            }
            int debrisN = 0;
            foreach (KeyValuePair<int, JampsDebrisInfo> kv in app.Database.JampsDebris)
            {
                if (app.Profile.HasJampsDebris(kv.Value.Id) || app.Profile.HasJampsPageCollected(kv.Value.PageId)) continue;
                int debrisId = kv.Value.Id;
                SysUi.Row(body, "jd" + debrisId, $"购买碎片 #{debrisId}  {kv.Value.JampsCurrency}金",
                    () => PhoneNet.JampsClaimPage("debris", kv.Value.PageId, debrisId));
                if (++debrisN >= 20) break;
            }
            if (app.Database.JampsPages.Count == 0) SysUi.Note(body, "Missing jampspageitemlist.xml");
        }
    }

    public static class FarmScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "农场  收获 " + app.Profile.FarmHarvests);
            int farmCost = app.Database != null ? app.Database.FarmBuyVegetableCost() : 200;
            SysUi.Note(body, "合成食物：消耗蔬菜，获得成品。没有蔬菜时用 " + farmCost + " 金币补货。");
            foreach (FarmRecipe r in app.Database.Farm)
            {
                FarmRecipe local = r;
                var btn = SysUi.Row(body, "f" + r.FoodId,
                    $"{SysUi.ItemName(app, r.VegetableId)} x{r.NeedCount}  →  {SysUi.ItemName(app, r.FoodId)}",
                    () => Cook(app, local));
                ShopScreen.DecorateIcon(app, btn, r.FoodId);
            }

            if (app.Database.Farm.Count == 0)
            {
                SysUi.Note(body, "Missing foodcomposelist.xml");
            }
        }

        static void Cook(GameApp app, FarmRecipe r)
        {
            PhoneNet.CookFarm(r.FoodId);
        }
    }

    public static class ConsortiaScreen
    {
        static string _createName = "弹弹公会";

        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "公会  " + (string.IsNullOrEmpty(app.Profile.ConsortiaName) ? "未加入" : app.Profile.ConsortiaName));
            int donateGold = app.Database != null ? app.Database.ConfigInt("ConsortiaMinOffer", 500) : 500;
            int createCost = app.Database != null ? app.Database.ConsortiaCreateCost() : 4000;
            SysUi.Note(body, "创建 " + createCost + " 金 · 加入已有公会 · 捐献 " + donateGold + " 金 → 荣誉");

            if (!string.IsNullOrEmpty(app.Profile.ConsortiaName))
            {
                SysUi.Row(body, "donate", "捐献 " + donateGold + " 金币", PhoneNet.DonateGuild);
                SysUi.Row(body, "leave", "退出公会", PhoneNet.LeaveGuild);
                ShowMembers(body, PhoneNet.LastGuildJson);
            }
            else
            {
                SysUi.Row(body, "create", "创建公会「" + _createName + "」  " + createCost + " 金", () =>
                {
                    PhoneNet.CreateGuild(_createName);
                });
            }

            XmlResultTable table = SysUi.Table(app, "Request/CelebByConsortiaRiches.xml")
                                 ?? SysUi.Table(app, "Request/CelebByConsortiaRiches_Out.xml");
            if (table == null)
            {
                SysUi.Note(body, "Missing CelebByConsortiaRiches.xml");
                return;
            }

            int n = 0;
            foreach (var row in table.Rows)
            {
                string name = GameDatabase.Str(row, "ConsortiaName");
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                string local = name;
                int lv = GameDatabase.Int(row, "Level");
                int riches = GameDatabase.Int(row, "Riches");
                SysUi.Row(body, "g" + n, $"{name}  Lv{lv}  财富{riches}  会长 {GameDatabase.Str(row, "ChairmanName")}", () =>
                {
                    PhoneNet.JoinGuild(local);
                });
                n++;
            }
        }

        static void ShowMembers(Transform body, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            int idx = json.IndexOf("[", StringComparison.Ordinal);
            int end = json.LastIndexOf("]", StringComparison.Ordinal);
            if (idx < 0 || end <= idx)
            {
                return;
            }

            string arr = json.Substring(idx + 1, end - idx - 1);
            int pos = 0;
            int shown = 0;
            while (pos < arr.Length && shown < 20)
            {
                int ob = arr.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = arr.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = arr.Substring(ob, cb - ob + 1);
                pos = cb + 1;
                string nick = GameApp.JsonStr(entry, "nick", "?");
                int level = GameApp.JsonInt(entry, "level", 1);
                bool online = entry.IndexOf("\"online\":true", StringComparison.Ordinal) >= 0;
                SysUi.Note(body, "成员  " + nick + "  Lv" + level + (online ? "  在线" : "  离线"));
                shown++;
            }
        }
    }

    public static class RankScreen
    {
        static string _rankType = "gp";

        public static void Show(RectTransform safe, GameApp app)
        {
            PhoneNet.RequestRank(_rankType);
            Transform body = SysUi.Begin(safe, app, "排行榜 · PC Celeb");
            SysUi.Row(body, "gp", _rankType == "gp" ? "[经验日增]" : "经验日增", () => { _rankType = "gp"; Show(safe, app); });
            SysUi.Row(body, "fight", _rankType == "fight" ? "[战斗力]" : "战斗力", () => { _rankType = "fight"; Show(safe, app); });
            SysUi.Row(body, "offer", _rankType == "offer" ? "[功勋日增]" : "功勋日增", () => { _rankType = "offer"; Show(safe, app); });
            SysUi.Note(body, $"你: {app.Profile.Nick}  Lv.{app.Profile.Level}  GP {app.Profile.Gp}  {app.Profile.Win}W/{app.Profile.Lose}L");

            string json = PhoneNet.LastRankJson;
            if (string.IsNullOrEmpty(json))
            {
                SysUi.Note(body, "正在加载排行...");
                return;
            }

            int idx = json.IndexOf("[", StringComparison.Ordinal);
            int end = json.LastIndexOf("]", StringComparison.Ordinal);
            if (idx < 0 || end < 0)
            {
                SysUi.Note(body, "排行数据格式错误");
                return;
            }

            string arr = json.Substring(idx + 1, end - idx - 1);
            int rank = 1;
            int pos = 0;
            while (pos < arr.Length && rank <= 50)
            {
                int ob = arr.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = arr.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = arr.Substring(ob, cb - ob + 1);
                pos = cb + 1;

                int listed = GameApp.JsonInt(entry, "rank", rank);
                string nick = GameApp.JsonStr(entry, "nick", "?");
                int level = GameApp.JsonInt(entry, "level", 1);
                int gp = GameApp.JsonInt(entry, "gp", 0);
                int fightPower = GameApp.JsonInt(entry, "fightPower", 0);
                int offer = GameApp.JsonInt(entry, "offer", 0);
                int win = GameApp.JsonInt(entry, "win", 0);
                int vip = GameApp.JsonInt(entry, "vip", 0);
                string consortia = GameApp.JsonStr(entry, "consortia", "");
                bool self = entry.IndexOf("\"self\":true", StringComparison.Ordinal) >= 0;
                string tag = self ? " (你)" : "";
                string metric = _rankType == "fight"
                    ? $"战力 {fightPower}"
                    : _rankType == "offer"
                        ? $"功勋 {offer}"
                        : $"GP {gp}";
                SysUi.Note(body, $"#{listed}  {nick}{tag}  Lv{level}  {metric}  {win}胜  VIP{vip}  {consortia}");
                rank++;
            }

            if (rank == 1)
            {
                SysUi.Note(body, "暂无排行数据");
            }
        }
    }

    public static class AuctionScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            PhoneNet.RequestAuctionList();
            Transform body = SysUi.Begin(safe, app, "拍卖行");
            SysUi.Note(body, "购买市场物品 · 挂售背包道具（ReclaimValue 底价）");
            RenderMarket(body, app, PhoneNet.LastAuctionListJson);

            SysUi.Note(body, "— 挂售 / 快速出售 —");
            foreach (BagItem slot in app.Profile.Bag)
            {
                BagItem local = slot;
                ItemTemplate item = app.Database.GetItem(slot.TemplateId);
                int price = app.Database != null ? app.Database.AuctionPrice(item) : 80;
                var btn = SysUi.Row(body, "a" + slot.TemplateId,
                    $"{(item != null ? item.Name : "#" + slot.TemplateId)} x{slot.Count}  挂售 {price} 金", () =>
                    {
                        PhoneNet.ListAuction(local.TemplateId, price, 1);
                    });
                ShopScreen.DecorateIcon(app, btn, slot.TemplateId);
            }
        }

        static void RenderMarket(Transform body, GameApp app, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                SysUi.Note(body, "正在加载拍卖行...");
                return;
            }

            int idx = json.IndexOf("[", StringComparison.Ordinal);
            int end = json.LastIndexOf("]", StringComparison.Ordinal);
            if (idx < 0 || end <= idx)
            {
                SysUi.Note(body, "暂无拍卖物品");
                return;
            }

            string arr = json.Substring(idx + 1, end - idx - 1);
            int pos = 0;
            int shown = 0;
            while (pos < arr.Length && shown < 30)
            {
                int ob = arr.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = arr.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = arr.Substring(ob, cb - ob + 1);
                pos = cb + 1;

                int id = GameApp.JsonInt(entry, "id", 0);
                int templateId = GameApp.JsonInt(entry, "templateId", 0);
                int count = GameApp.JsonInt(entry, "count", 1);
                int price = GameApp.JsonInt(entry, "price", 0);
                int strengthen = GameApp.JsonInt(entry, "strengthen", 0);
                string seller = GameApp.JsonStr(entry, "seller", "?");
                string name = SysUi.ItemName(app, templateId);
                int listingId = id;
                var btn = SysUi.Row(body, "buy" + id,
                    seller + "  " + name + " x" + count + "  +" + strengthen + "  " + price + " 金", () =>
                    {
                        PhoneNet.BuyAuction(listingId);
                    });
                ShopScreen.DecorateIcon(app, btn, templateId);
                shown++;
            }

            if (shown == 0)
            {
                SysUi.Note(body, "暂无拍卖物品");
            }
        }
    }

    public static class VipScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "VIP " + app.Profile.VipLevel);
            SysUi.Row(body, "up", "升级 VIP  500 点券", () =>
            {
                PhoneNet.UpgradeVip();
            });
            foreach (ShopOffer offer in app.Database.VipShop)
            {
                ShopOffer local = offer;
                string name = SysUi.ItemName(app, offer.TemplateId);
                var btn = SysUi.Row(body, "v" + offer.Id, $"{name}  {offer.AValue1}点券", () =>
                {
                    PhoneNet.ShopBuy(local.Id);
                });
                ShopScreen.DecorateIcon(app, btn, offer.TemplateId);
            }
        }
    }

    public static class LotteryScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            int cost1 = app.Database != null ? app.Database.LotteryDrawCost(1) : 100;
            int cost10 = app.Database != null ? app.Database.LotteryDrawCost(10) : 1000;
            Transform body = SysUi.Begin(safe, app, $"抽奖  {cost1} 金币 / 次");
            if (app.Database.Lottery.Count == 0)
            {
                SysUi.Note(body, "Missing newlotteryitem.xml");
                return;
            }

            SysUi.Row(body, "draw", "抽一次", () => Draw(app));
            SysUi.Row(body, "draw10", "抽十次  " + cost10 + " 金", () =>
            {
                PhoneNet.DrawLottery(10);
            });
            int shown = 0;
            foreach (LotteryDrop d in app.Database.Lottery)
            {
                SysUi.Note(body, $"奖池 {SysUi.ItemName(app, d.TemplateId)} x{d.Count}");
                shown++;
                if (shown >= 40)
                {
                    break;
                }
            }
        }

        static bool Draw(GameApp app, bool refresh = true)
        {
            PhoneNet.DrawLottery(1);
            return app.Database.Lottery.Count > 0;
        }
    }

    public static class LabyrinthScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "迷宫  当前层 " + app.Profile.LabyrinthFloor);
            int floor = Mathf.Max(1, app.Profile.LabyrinthFloor);
            NpcInfo npc = app.Database.PickNpc(floor * 97, 10 + floor, 250000);
            int mapId = app.Database.PickMapId(floor * 13);
            string npcName = npc != null ? npc.Name : "守卫";
            int npcId = npc != null ? npc.Id : 0;
            SysUi.Note(body, $"第 {floor} 层  Map {mapId}  vs {npcName}");
            SysUi.Row(body, "go", "挑战本层", () => SysUi.Fight(app, mapId, npcId, true));
        }
    }

    public static class WorldBossScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            int maxHits = app.Database != null ? app.Database.ConfigInt("WorldBossDayLimit", 3) : 3;
            Transform body = SysUi.Begin(safe, app, "世界BOSS");
            SysUi.Note(body, "campwaritems.xml  ·  今日 " + app.Profile.WorldBossHits + " / " + maxHits);
            SysUi.Row(body, "wbstart", "挑战世界BOSS", () =>
            {
                PhoneNet.WorldBossStart();
                app.ShowRoom();
            });

            var bosses = new List<NpcInfo>();
            foreach (NpcInfo n in app.Database.Npcs.Values)
            {
                if (n.Blood >= 1_000_000)
                {
                    bosses.Add(n);
                }
            }

            bosses.Sort((a, b) => b.Blood.CompareTo(a.Blood));
            int nShow = 0;
            foreach (NpcInfo n in bosses)
            {
                NpcInfo local = n;
                GameDatabase.ClientCombatStats(n, out int hp, out int atk, out _, out _, out _);
                SysUi.Row(body, "wb" + n.Id, $"{n.Name}  Lv{n.Level}  战 HP{hp} ATK{atk}  (表 Blood {n.Blood})", () =>
                {
                    int mapId = app.Database.PickMapId(local.Id);
                    SysUi.Fight(app, mapId, local.Id);
                });
                nShow++;
                if (nShow >= 24)
                {
                    break;
                }
            }

            if (nShow == 0)
            {
                SysUi.Note(body, "No world-boss rows (Blood>=1e6).");
            }
        }
    }

    public static class DungeonScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "副本 · LoadPVEItems");
            if (app.Database.Pve.Count == 0)
            {
                SysUi.Note(body, "Missing LoadPVEItems.xml — 用房间开战也可。");
                return;
            }

            foreach (PveMission m in app.Database.Pve)
            {
                PveMission local = m;
                NpcInfo npc = app.Database.PickNpc(m.Id, Mathf.Max(m.LevelLimits, m.MinLv, 1), 200000);
                int npcId = npc != null ? npc.Id : 0;
                string foe = npc != null ? npc.Name : "副本怪";
                SysUi.Row(body, "d" + m.Id, $"{m.Name}  需求Lv{m.LevelLimits}  vs {foe}", () =>
                {
                    int mapId = app.Database.PickMapId(local.Id);
                    SysUi.Fight(app, mapId, npcId);
                });
            }
        }
    }

    public static class NpcHuntScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "NPC 狩猎  " + app.Database.Npcs.Count);
            int n = 0;
            foreach (NpcInfo npc in app.Database.Npcs.Values)
            {
                if (npc.Blood <= 0 || npc.Blood > 500000)
                {
                    continue;
                }

                NpcInfo local = npc;
                GameDatabase.ClientCombatStats(npc, out int hp, out int atk, out _, out _, out _);
                var btn = SysUi.Row(body, "n" + npc.Id, $"{npc.Name}  Lv{npc.Level}  HP{hp} ATK{atk}", () =>
                {
                    int mapId = app.Database.PickMapId(local.Id);
                    SysUi.Fight(app, mapId, local.Id);
                });
                PcArt.Decorate(btn.transform, PcArt.NpcLiving(app.Loader, npc));
                n++;
                if (n >= 150)
                {
                    SysUi.Note(body, "… more NPCs in NPCInfoList.xml");
                    break;
                }
            }
        }
    }

    public static class ForgeScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "铁匠铺 · 强化");
            SysUi.Note(body, "强化背包装备。成功率随等级下降；失败不掉级。");
            foreach (BagItem slot in app.Profile.Bag)
            {
                ItemTemplate item = app.Database.GetItem(slot.TemplateId);
                if (item == null || !item.CanEquip)
                {
                    continue;
                }

                BagItem local = slot;
                int next = slot.Strengthen + 1;
                int rock = 2;
                if (!app.Database.StrengthenRock.TryGetValue(next, out rock))
                {
                    rock = 200 * next;
                }

                int gold = Mathf.Max(100, rock * 40);
                var btn = SysUi.Row(body, "str" + slot.TemplateId,
                    $"{item.Name}  +{slot.Strengthen}  → +{next}  {gold}金",
                    () =>
                    {
                        if (local.Strengthen >= 15) return;
                        PhoneNet.StrengthenItem(local.TemplateId);
                    });
                ShopScreen.DecorateIcon(app, btn, slot.TemplateId);
            }
        }
    }

    public static class TexpScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            int texpCost = app.Database != null ? app.Database.TexpTrainGoldCost() : 400;
            int texpGain = app.Database != null ? app.Database.TexpTrainGain() : 25;
            Transform body = SysUi.Begin(safe, app, "修炼  Texp " + app.Profile.Texp);
            MornScreenHost.TryEmbedMorn(body, app, "ddttexpsystem.ui");
            SysUi.Row(body, "texp", $"修炼一次  {texpCost} 金币  Texp+{texpGain}", () => PhoneNet.TrainTexp());
        }
    }

    public static class GemScreen
    {
        static readonly string[] SpiritLabels = { "攻击之魂", "防御之魂", "敏捷之魂", "幸运之魂", "生命之魂" };

        public static void Show(RectTransform safe, GameApp app)
        {
            app.Profile.EnsureFightSpirits();
            Transform body = SysUi.Begin(safe, app, "战魂  Lv." + app.Profile.GemLevel);
            SysUi.Note(body, "fightspirittemplatelist.xml · SpiritInfoList 武器镶嵌 Lv." + app.Profile.GemLevel);

            int gemCost = app.Database != null ? app.Database.GemUpgradeCost(app.Profile.GemLevel) : 0;
            if (gemCost > 0 && app.Profile.GemLevel < 12)
            {
                SysUi.Row(body, "gem", "武器战魂 +1  " + gemCost + " 金币", PhoneNet.UpgradeGem);
            }

            for (int i = 0; i < app.Profile.FightSpirits.Count; i++)
            {
                FightSpiritSlot slot = app.Profile.FightSpirits[i];
                string label = i < SpiritLabels.Length ? SpiritLabels[i] : ("魂" + slot.SpiritId);
                FightSpiritTemplate row = app.Database.GetFightSpirit(slot.SpiritId, slot.Level);
                FightSpiritTemplate next = app.Database.GetFightSpirit(slot.SpiritId, slot.Level + 1);
                int cost = app.Database.FightSpiritUpgradeCost(slot.SpiritId, slot.Level);
                string stats = row != null
                    ? $"ATK{row.Attack / 100} DEF{row.Defence / 100} AGI{row.Agility / 100} LUK{row.Lucky / 100} HP{row.Blood / 100}"
                    : "Lv0";
                if (next != null && cost > 0 && slot.Level < 12)
                {
                    int spiritId = slot.SpiritId;
                    SysUi.Row(body, "fs" + spiritId,
                        label + "  Lv" + slot.Level + "  " + stats + "  → Lv" + (slot.Level + 1) + "  " + cost + " 金",
                        () => PhoneNet.UpgradeFightSpirit(spiritId));
                }
                else
                {
                    SysUi.Note(body, label + "  Lv" + slot.Level + "  " + stats);
                }
            }
        }
    }

    public static class KingBlessScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "弹王盟约");
            int today = System.DateTime.Now.DayOfYear;
            bool done = app.Profile.KingBlessDay == today;
            int blessGold = app.Database != null ? app.Database.KingBlessGold(app.Profile.VipLevel) : 400;
            SysUi.Note(body, done ? "今日已领取盟约礼包。" : $"领取金币 +{blessGold}（PC TakeCardMoney/2 + VIP×80）");
            if (!done)
            {
                SysUi.Row(body, "kb", "领取", () =>
                {
                    PhoneNet.ClaimKingBless();
                });
            }
        }
    }

    public static class FriendScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            PhoneNet.RefreshFriends();
            Transform body = SysUi.Begin(safe, app, "好友");
            app.Profile.EnsureStarterBag();
            SysUi.Row(body, "add", "添加在线玩家", () =>
            {
                string name = "Player" + ((PhoneNet.PlayerId % 9) + 1);
                PhoneNet.AddFriend(name);
            });
            RenderFriends(body, app, PhoneNet.LastFriendListJson);
            if (string.IsNullOrEmpty(PhoneNet.LastFriendListJson))
            {
                foreach (string f in app.Profile.Friends)
                {
                    string local = f;
                    SysUi.Row(body, "f" + f, "好友  " + f, () => PhoneNet.RemoveFriend(local));
                    AddMailRows(body, local);
                }
            }
        }

        static void AddMailRows(Transform body, string nick)
        {
            string local = nick;
            SysUi.Row(body, "mail100_" + nick, nick + "  寄信 100 金",
                () => PhoneNet.SendMail(local, 100, "好友邮件", "来自好友的小礼物"));
            SysUi.Row(body, "mail500_" + nick, nick + "  寄信 500 金",
                () => PhoneNet.SendMail(local, 500, "好友邮件", "来自好友的小礼物"));
        }

        static void RenderFriends(Transform body, GameApp app, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            int idx = json.IndexOf("[", StringComparison.Ordinal);
            int end = json.LastIndexOf("]", StringComparison.Ordinal);
            if (idx < 0 || end <= idx)
            {
                return;
            }

            string arr = json.Substring(idx + 1, end - idx - 1);
            int pos = 0;
            while (pos < arr.Length)
            {
                int ob = arr.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = arr.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = arr.Substring(ob, cb - ob + 1);
                pos = cb + 1;
                string nick = GameApp.JsonStr(entry, "nick", null);
                if (string.IsNullOrEmpty(nick))
                {
                    continue;
                }

                bool online = entry.IndexOf("\"online\":true", StringComparison.Ordinal) >= 0;
                string local = nick;
                SysUi.Row(body, "fr" + nick, nick + (online ? "  在线" : "  离线"), () => PhoneNet.RemoveFriend(local));
                AddMailRows(body, nick);
            }
        }
    }

    public static class MailInboxScreen
    {
        static string FirstFriendNick(GameApp app)
        {
            string json = PhoneNet.LastFriendListJson;
            if (!string.IsNullOrEmpty(json))
            {
                int idx = json.IndexOf("[", StringComparison.Ordinal);
                int end = json.LastIndexOf("]", StringComparison.Ordinal);
                if (idx >= 0 && end > idx)
                {
                    string arr = json.Substring(idx + 1, end - idx - 1);
                    int pos = 0;
                    while (pos < arr.Length)
                    {
                        int ob = arr.IndexOf('{', pos);
                        if (ob < 0) break;
                        int cb = arr.IndexOf('}', ob);
                        if (cb < 0) break;
                        string entry = arr.Substring(ob, cb - ob + 1);
                        pos = cb + 1;
                        string nick = GameApp.JsonStr(entry, "nick", null);
                        if (!string.IsNullOrEmpty(nick))
                        {
                            return nick;
                        }
                    }
                }
            }

            app.Profile.EnsureStarterBag();
            if (app.Profile.Friends != null && app.Profile.Friends.Count > 0)
            {
                return app.Profile.Friends[0];
            }

            return "训练教官";
        }

        public static void Show(RectTransform safe, GameApp app)
        {
            PhoneNet.RequestMailList();
            Transform body = SysUi.Begin(safe, app, "邮件");
            app.Profile.EnsureStarterBag();
            if (app.Profile.Friends != null && app.Profile.Friends.Count > 0)
            {
                string target = FirstFriendNick(app);
                SysUi.Row(body, "send100", "发送邮件给 " + target + " (100 金)",
                    () => PhoneNet.SendMail(target, 100, "玩家邮件", "一点心意"));
                SysUi.Row(body, "send500", "发送邮件给 " + target + " (500 金)",
                    () => PhoneNet.SendMail(target, 500, "玩家邮件", "一点心意"));
            }

            string json = PhoneNet.LastMailListJson;
            if (string.IsNullOrEmpty(json))
            {
                SysUi.Note(body, "正在加载邮件...");
                SysUi.Row(body, "claim", "全部领取", () => PhoneNet.Road?.Send(PhoneMsg.MailClaim, "{\"id\":0}"));
                return;
            }

            int idx = json.IndexOf("[", StringComparison.Ordinal);
            int end = json.LastIndexOf("]", StringComparison.Ordinal);
            int count = 0;
            if (idx >= 0 && end > idx)
            {
                string arr = json.Substring(idx + 1, end - idx - 1);
                int pos = 0;
                while (pos < arr.Length)
                {
                    int ob = arr.IndexOf('{', pos);
                    if (ob < 0) break;
                    int cb = arr.IndexOf('}', ob);
                    if (cb < 0) break;
                    string entry = arr.Substring(ob, cb - ob + 1);
                    pos = cb + 1;

                    int id = GameApp.JsonInt(entry, "id", 0);
                    string subject = GameApp.JsonStr(entry, "subject", "邮件");
                    string mailBody = GameApp.JsonStr(entry, "body", "");
                    int gold = GameApp.JsonInt(entry, "gold", 0);
                    int itemId = GameApp.JsonInt(entry, "itemId", 0);
                    int itemCount = GameApp.JsonInt(entry, "itemCount", 0);
                    bool claimed = entry.IndexOf("\"claimed\":true", StringComparison.Ordinal) >= 0;
                    count++;
                    string reward = gold > 0 ? $"{gold} 金" : itemId > 0 ? $"#{itemId} x{itemCount}" : "";
                    string status = claimed ? "[已领]" : "领取";
                    int mailId = id;
                    SysUi.Row(body, "m" + id, $"{subject}  {reward}  {status}", () =>
                    {
                        if (!claimed)
                        {
                            PhoneNet.Road?.Send(PhoneMsg.MailClaim, "{\"id\":" + mailId + "}");
                        }
                    });
                    if (!string.IsNullOrEmpty(mailBody))
                    {
                        SysUi.Note(body, mailBody);
                    }
                }
            }

            if (count == 0)
            {
                SysUi.Note(body, "收件箱为空");
            }

            SysUi.Row(body, "claimAll", "全部领取", () => PhoneNet.Road?.Send(PhoneMsg.MailClaim, "{\"id\":0}"));
        }
    }

    public static class ChatScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.PcPanel(safe, "Chat");
            ShopScreen.Header(bg.transform, app, "聊天");
            var scroll = ShopScreen.BodyScroll(bg.transform);
            foreach (string line in app.Profile.ChatLog)
            {
                SysUi.Note(scroll.content, line);
            }

            InputField field = UiKit.Field(bg.transform, "Msg", "说点什么", new Vector2(520f, 56f));
            field.characterLimit = 48;
            field.GetComponent<RectTransform>().anchorMin = field.GetComponent<RectTransform>().anchorMax = new Vector2(0.38f, 0.08f);
            var send = UiKit.Button(bg.transform, "Send", "发送", () =>
            {
                string t = (field.text ?? "").Trim();
                if (t.Length == 0)
                {
                    return;
                }

                PhoneNet.SendChat(t);
                field.text = "";
                Show(safe, app);
            }, new Vector2(140f, 56f));
            send.GetComponent<RectTransform>().anchorMin = send.GetComponent<RectTransform>().anchorMax = new Vector2(0.82f, 0.08f);
        }
    }

    public static class BallPickScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "炮弹  当前 #" + (app.Profile.PreferredBallId == 0 ? "武器默认" : app.Profile.PreferredBallId.ToString()));
            SysUi.Row(body, "def", "使用武器默认炮弹", () => PhoneNet.SelectBall(0));
            int n = 0;
            foreach (var kv in app.Database.Balls)
            {
                int id = kv.Key;
                var ball = kv.Value;
                var btn = SysUi.Row(body, "ball" + id, $"#{id}  Power{ball.Power}  r{ball.Radii}  W{ball.Wind}  m{ball.Mass}", () =>
                {
                    PhoneNet.SelectBall(id);
                });
                int fly = ball.FlyingPartical > 0 ? ball.FlyingPartical : id;
                PcArt.Decorate(btn.transform, PcArt.Bullet(app.Loader, fly) ?? PcArt.Blast(app.Loader, ball.BombPartical > 0 ? ball.BombPartical : id));
                n++;
                if (n >= 80)
                {
                    break;
                }
            }
        }
    }

    public static class BombConfigScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "炸弹配置 · bombconfig");
            int n = 0;
            foreach (var kv in app.Database.Bombs)
            {
                var b = kv.Value;
                string w = SysUi.ItemName(app, b.TemplateId);
                var btn = SysUi.Row(body, "bm" + b.TemplateId, $"{w}  Common ball {b.Common}  Special {b.Special}", () =>
                {
                    if (b.Common > 0)
                    {
                        PhoneNet.SelectBall(b.Common);
                        PhoneNet.EquipItem(b.TemplateId);
                    }
                });
                ShopScreen.DecorateIcon(app, btn, b.TemplateId);
                n++;
                if (n >= 80)
                {
                    break;
                }
            }
        }
    }

    public static class GodCardScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "神卡 · godcardlist");
            int oneCost = app.Database.ConfigInt("GodCardOpenOneTimeMoney", 5000);
            int fiveCost = app.Database.ConfigInt("GodCardOpenFiveTimeMoney", 24688);
            SysUi.Note(body, $"Gold {app.Profile.Gold}  ·  单抽 {oneCost}  ·  五连 {fiveCost}");
            SysUi.Row(body, "open1", "开启 x1", () => PhoneNet.OpenGodCards(1));
            SysUi.Row(body, "open5", "开启 x5", () => PhoneNet.OpenGodCards(5));

            if (app.Profile.GodCards != null && app.Profile.GodCards.Count > 0)
            {
                SysUi.Note(body, "已拥有 / 点击装备");
                foreach (GodCardSlot slot in app.Profile.GodCards)
                {
                    if (!app.Database.GodCards.TryGetValue(slot.Id, out GodCardInfo card))
                    {
                        continue;
                    }

                    GodCardInfo local = card;
                    int sid = slot.Id;
                    bool on = app.Profile.GodCardEquipId == sid;
                    SysUi.Row(body, "gc" + sid,
                        $"{(on ? "[装备] " : "")}{local.Name} x{slot.Count}  Lv{local.Level}",
                        () => PhoneNet.EquipGodCard(sid));
                }
            }
            else
            {
                SysUi.Note(body, "还没有神卡，先抽卡。");
            }

            foreach (GodCardInfo card in app.Database.GodCards.Values)
            {
                bool listed = false;
                foreach (GodCardSlot s in app.Profile.GodCards)
                {
                    if (s.Id == card.Id)
                    {
                        listed = true;
                        break;
                    }
                }

                if (listed)
                {
                    continue;
                }

                SysUi.Note(body, $"图鉴: {card.Name}  合成{card.Composition}  分解{card.Decompose}");
            }
        }
    }

    public static class GodCardRaiseScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "神卡养成 · godcardraise");
            SysUi.Note(body, $"积分 {app.Profile.GodCardPoints}  ·  装备 #{app.Profile.GodCardEquipId}");

            if (app.Profile.GodCards != null && app.Profile.GodCards.Count > 0)
            {
                SysUi.Note(body, "养成卡槽 (消耗重复卡升槽)");
                foreach (GodCardSlot slot in app.Profile.GodCards)
                {
                    if (!app.Database.GodCards.TryGetValue(slot.Id, out GodCardInfo card)) continue;
                    int sid = slot.Id;
                    int gain = app.Database.GodCardRaiseExpGain(card);
                    string label = $"{card.Name} x{slot.Count}  槽Lv{slot.GrooveLevel}  exp{slot.GrooveExp}  (+{gain}/张)";
                    if (slot.Count > 1)
                    {
                        SysUi.Row(body, "raise" + sid, label + "  ·  升槽 x1", () => PhoneNet.RaiseGodCard(sid, 1));
                        if (slot.Count > 2)
                        {
                            int bulk = slot.Count - 1;
                            SysUi.Row(body, "raiseBulk" + sid, label + "  ·  升槽 x" + bulk, () => PhoneNet.RaiseGodCard(sid, bulk));
                        }
                    }
                    else SysUi.Note(body, label + "  (需要更多重复卡)");
                }
            }
            else SysUi.Note(body, "还没有神卡，请先在 godcard 模块抽卡。");

            if (app.Database.GodCardPointRewards.Count > 0)
            {
                SysUi.Note(body, "积分兑换");
                foreach (GodCardPointRewardInfo reward in app.Database.GodCardPointRewards.Values)
                {
                    bool claimed = app.Profile.GodCardPointClaimed != null && app.Profile.GodCardPointClaimed.Contains(reward.Id);
                    string itemName = app.Database.GetItem(reward.ItemId)?.Name ?? ("#" + reward.ItemId);
                    string rowLabel = (claimed ? "[已领] " : "") + itemName + " x" + reward.Count + "  需要 " + reward.Point + " 分";
                    if (!claimed && app.Profile.GodCardPoints >= reward.Point)
                        SysUi.Row(body, "pt" + reward.Id, rowLabel, () => PhoneNet.ClaimGodCardPoint(reward.Id));
                    else SysUi.Note(body, rowLabel);
                }
            }

            if (app.Database.GodCardGroups.Count > 0)
            {
                SysUi.Note(body, "卡组图鉴");
                var groups = new System.Collections.Generic.Dictionary<int, int>();
                foreach (GodCardGroupEntry entry in app.Database.GodCardGroups)
                {
                    if (!groups.ContainsKey(entry.GroupId)) groups[entry.GroupId] = 0;
                    foreach (GodCardSlot slot in app.Profile.GodCards)
                    {
                        if (slot.Id == entry.CardId && slot.Count >= entry.Number) { groups[entry.GroupId]++; break; }
                    }
                }
                foreach (var kv in groups)
                {
                    int need = 0;
                    foreach (GodCardGroupEntry entry in app.Database.GodCardGroups)
                        if (entry.GroupId == kv.Key) need++;
                    SysUi.Note(body, $"组 {kv.Key}: {kv.Value}/{need}");
                }
            }
        }
    }

    public static class EngraveScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "刻印 · engravesetinfo");
            int minLv = app.Database.ConfigInt("EngraveLimitLevel", 20);
            SysUi.Note(body, $"Level {app.Profile.Level} (需要 {minLv}+)  ·  套装 {app.Profile.EngraveSetId}");
            SysUi.Row(body, "clear", "卸下刻印", () => PhoneNet.EquipEngraveSet(0));
            foreach (EngraveSetInfo set in app.Database.EngraveSets.Values)
            {
                EngraveSetInfo local = set;
                bool on = app.Profile.EngraveSetId == set.SetId;
                SysUi.Row(body, "eg" + set.SetId,
                    $"{(on ? "[装备] " : "")}{set.Name}",
                    () => PhoneNet.EquipEngraveSet(local.SetId));
                if (!string.IsNullOrEmpty(set.HelpExplain))
                {
                    SysUi.Note(body, StripTags(set.HelpExplain));
                }
            }
        }

        static string StripTags(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return "";
            }

            return raw.Replace("&lt;", "<").Replace("&gt;", ">").Replace("\"", "");
        }
    }

    public static class StockScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "股票 · StockTemplateInfo");
            int minLv = app.Database.ConfigInt("StockLimitLevel", 30);
            SysUi.Note(body, $"Gold {app.Profile.Gold}  ·  需要等级 {minLv}+");
            foreach (StockInfo stock in app.Database.Stocks.Values)
            {
                StockInfo local = stock;
                int price = app.Database.StockQuote(stock);
                int owned = 0;
                foreach (StockSlot h in app.Profile.StockHoldings)
                {
                    if (h.StockId == stock.StockId)
                    {
                        owned = h.Shares;
                        break;
                    }
                }

                SysUi.Note(body, $"{stock.StockName} #{stock.StockId}  现价 {price}  持有 {owned}");
                SysUi.Row(body, "buy" + stock.StockId, "买入 x10", () => PhoneNet.TradeStock("buy", local.StockId, 10));
                if (owned > 0)
                {
                    SysUi.Row(body, "sell" + stock.StockId, "卖出 x10", () => PhoneNet.TradeStock("sell", local.StockId, 10));
                }
            }
        }
    }

    public static class CalendarScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "日历 · TS_ActivityConfig");
            var now = System.DateTime.Now;
            SysUi.Note(body, $"{now.Year}-{now.Month:D2}  今日 {now.Day}  ·  金 {app.Profile.Gold}");
            app.Profile.EnsureCalendarClaimed();
            if (app.Database != null)
            {
                SysUi.Note(body, "活动列表 (TS_ActivityConfig.xml)");
                int actShown = 0;
                foreach (ActivityConfigEntry entry in app.Database.ActivityConfigs.Values)
                {
                    SysUi.Note(body, "Num" + entry.Num + "  " + entry.Name);
                    if (++actShown >= 8) break;
                }
                if (app.Database.GmActivities.Count > 0)
                {
                    SysUi.Note(body, "GM 活动 (gmactivityinfo.xml)");
                    int gmShown = 0;
                    foreach (GmActivityInfo gm in app.Database.GmActivities)
                    {
                        SysUi.Note(body, "T" + gm.ActivityType + "  " + gm.ActivityName);
                        if (++gmShown >= 6) break;
                    }
                }
                SysUi.Note(body, "每日日历奖励 (TS_EveryDaySignIn.xml)");
                int today = now.Day;
                foreach (SignReward r in app.Database.SignIn)
                {
                    if (r.Day > 28) break;
                    bool claimed = app.Profile.CalendarClaimedDays.Contains(r.Day);
                    string label = "Day " + r.Day + ": " + SysUi.ItemName(app, r.TemplateId) + " x" + r.Count;
                    if (claimed) label = "[已领] " + label;
                    else if (r.Day > today) label += "  (未到)";
                    int day = r.Day;
                    SysUi.Row(body, "cal" + r.Day, label, claimed || r.Day > today ? null : (System.Action)(() => PhoneNet.CalendarClaim(day)));
                }
            }
            if (!string.IsNullOrEmpty(PhoneNet.LastCalendarJson)) SysUi.Note(body, PhoneNet.LastCalendarJson);
        }
    }
}
