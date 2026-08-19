using System.Collections.Generic;
using GunMobile.Core;
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

        public static void Fight(GameApp app, int mapId, int npcId, int extraGold, bool labyrinth = false)
        {
            app.Profile.PendingReward = extraGold;
            app.Profile.PendingLabyrinth = labyrinth ? 1 : 0;
            app.Profile.Save();
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
                        app.Profile.PetId = local.TemplateId;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
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
                        app.Profile.CardId = local.Id;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
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
                        app.Profile.TitleId = local.Id;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
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
                        if (app.Profile.Honor < local.ConsumeHonor && !on)
                        {
                            return;
                        }

                        if (!on && local.ConsumeHonor > 0)
                        {
                            app.Profile.Honor -= local.ConsumeHonor;
                        }

                        PhoneNet.BuyTotem(local.Id);
                        app.Profile.TotemId = local.Id;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
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
            Transform body = SysUi.Begin(safe, app, "坐骑");
            foreach (MountGrade m in app.Database.Mounts.Values)
            {
                MountGrade local = m;
                bool on = app.Profile.MountGrade == m.Grade;
                SysUi.Row(body, "h" + m.Grade,
                    $"{(on ? "[骑乘] " : "")}Grade {m.Grade}  HP+{m.AddBlood} DMG+{m.AddDamage} MAG{m.MagicAttack}",
                    () =>
                    {
                        PhoneNet.UpgradeMount();
                        app.Profile.MountGrade = local.Grade;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
                    });
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
            foreach (ElfInfo e in app.Database.Elves.Values)
            {
                ElfInfo local = e;
                bool on = app.Profile.ElfId == e.TemplateId;
                SysUi.Row(body, "e" + e.TemplateId,
                    $"{(on ? "[跟随] " : "")}{e.Name}  ★{e.StarLevel}  ATK~{e.AttackHint} HP~{e.HpHint}",
                    () =>
                    {
                        PhoneNet.SelectPet(local.TemplateId);
                        app.Profile.ElfId = local.TemplateId;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
                    });
            }

            if (app.Database.Elves.Count == 0)
            {
                SysUi.Note(body, "Missing TS_ElfTemplate.xml");
            }
        }
    }

    public static class FarmScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "农场  收获 " + app.Profile.FarmHarvests);
            SysUi.Note(body, "合成食物：消耗蔬菜，获得成品。没有蔬菜时用 200 金币补货。");
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
            if (!app.Profile.Consume(r.VegetableId, r.NeedCount))
            {
                if (app.Profile.Gold < 200)
                {
                    return;
                }

                app.Profile.Gold -= 200;
                app.Profile.AddItem(r.VegetableId, r.NeedCount);
                if (!app.Profile.Consume(r.VegetableId, r.NeedCount))
                {
                    return;
                }
            }

            app.Profile.AddItem(r.FoodId, 1);
            app.Profile.FarmHarvests++;
            app.Profile.Save();
            Show(app.SafeArea, app);
        }
    }

    public static class ConsortiaScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "公会  " + (string.IsNullOrEmpty(app.Profile.ConsortiaName) ? "未加入" : app.Profile.ConsortiaName));
            SysUi.Note(body, "点选加入。捐献 1000 金币 → 荣誉 +80。");
            if (!string.IsNullOrEmpty(app.Profile.ConsortiaName))
            {
                SysUi.Row(body, "donate", "捐献 1000 金币", () =>
                {
                    if (app.Profile.Gold < 1000)
                    {
                        return;
                    }

                    PhoneNet.DonateGuild();
                    app.Profile.Gold -= 1000;
                    app.Profile.Honor += 80;
                    app.Profile.Save();
                    Show(safe, app);
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
                    app.Profile.ConsortiaName = local;
                    app.Profile.Save();
                    Show(safe, app);
                });
                n++;
            }
        }
    }

    public static class RankScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            PhoneNet.RequestRank();
            Transform body = SysUi.Begin(safe, app, "排行榜");
            SysUi.Note(body, $"你: {app.Profile.Nick}  Lv.{app.Profile.Level}  {app.Profile.Win}W/{app.Profile.Lose}L");

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

                string nick = GameApp.JsonStr(entry, "nick", "?");
                int level = GameApp.JsonInt(entry, "level", 1);
                int win = GameApp.JsonInt(entry, "win", 0);
                int lose = GameApp.JsonInt(entry, "lose", 0);
                int vip = GameApp.JsonInt(entry, "vip", 0);
                int honor = GameApp.JsonInt(entry, "honor", 0);

                SysUi.Note(body, $"#{rank}  {nick}  Lv{level}  {win}W/{lose}L  VIP{vip}  荣誉{honor}");
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
            Transform body = SysUi.Begin(safe, app, "拍卖");
            SysUi.Note(body, "出售背包道具，获得一半参考价金币（本地）。");
            foreach (BagItem slot in app.Profile.Bag)
            {
                BagItem local = slot;
                ItemTemplate item = app.Database.GetItem(slot.TemplateId);
                int price = item != null ? Mathf.Max(80, (item.Attack + item.Defence) * 12) : 80;
                var btn = SysUi.Row(body, "a" + slot.TemplateId, $"{(item != null ? item.Name : "#" + slot.TemplateId)} x{slot.Count}  卖 {price} 金", () =>
                {
                    if (!app.Profile.Consume(local.TemplateId, 1)) return;
                    PhoneNet.RequestProfile();
                    app.Profile.Gold += price;
                    app.Profile.Save();
                    Show(safe, app);
                });
                ShopScreen.DecorateIcon(app, btn, slot.TemplateId);
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
                if (app.Profile.Gift < 500 || app.Profile.VipLevel >= 15)
                {
                    return;
                }

                PhoneNet.UpgradeVip();
                app.Profile.Gift -= 500;
                app.Profile.VipLevel++;
                app.Profile.RecalcStats(app.Database);
                app.Profile.Save();
                Show(safe, app);
            });
            foreach (ShopOffer offer in app.Database.VipShop)
            {
                ShopOffer local = offer;
                string name = SysUi.ItemName(app, offer.TemplateId);
                var btn = SysUi.Row(body, "v" + offer.Id, $"{name}  {offer.AValue1}点券", () =>
                {
                    if (app.Profile.VipLevel < 1 || app.Profile.Gift < local.AValue1)
                    {
                        return;
                    }

                    PhoneNet.ShopBuy(local.Id);
                    app.Profile.Gift -= local.AValue1;
                    app.Profile.AddItem(local.TemplateId, 1);
                    app.Profile.Save();
                    Show(safe, app);
                });
                ShopScreen.DecorateIcon(app, btn, offer.TemplateId);
            }
        }
    }

    public static class LotteryScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "抽奖  300 金币 / 次");
            if (app.Database.Lottery.Count == 0)
            {
                SysUi.Note(body, "Missing newlotteryitem.xml");
                return;
            }

            SysUi.Row(body, "draw", "抽一次", () => Draw(app));
            SysUi.Row(body, "draw10", "抽十次  2700 金", () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    if (!Draw(app, false))
                    {
                        break;
                    }
                }

                Show(app.SafeArea, app);
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
            int cost = 300;
            if (app.Profile.Gold < cost || app.Database.Lottery.Count == 0)
            {
                return false;
            }

            app.Profile.Gold -= cost;
            LotteryDrop drop = app.Database.Lottery[Random.Range(0, app.Database.Lottery.Count)];
            app.Profile.AddItem(drop.TemplateId, drop.Count);
            app.Profile.Save();
            if (refresh)
            {
                Show(app.SafeArea, app);
            }

            return true;
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
            SysUi.Row(body, "go", "挑战本层", () => SysUi.Fight(app, mapId, npcId, 300 + floor * 40, true));
        }
    }

    public static class WorldBossScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "世界BOSS");
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
                    SysUi.Fight(app, mapId, local.Id, 2000 + local.Level * 20);
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
                    SysUi.Fight(app, mapId, npcId, 500 + local.LevelLimits * 10);
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
                    SysUi.Fight(app, mapId, local.Id, 150 + local.Level * 8);
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
                        if (local.Strengthen >= 15 || app.Profile.Gold < gold) return;
                        PhoneNet.StrengthenItem(local.TemplateId);
                        app.Profile.Gold -= gold;
                        int chance = Mathf.Clamp(90 - local.Strengthen * 5, 20, 90);
                        if (Random.Range(0, 100) < chance) local.Strengthen++;
                        app.Profile.RecalcStats(app.Database);
                        app.Profile.Save();
                        Show(safe, app);
                    });
                ShopScreen.DecorateIcon(app, btn, slot.TemplateId);
            }
        }
    }

    public static class TexpScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "修炼  Texp " + app.Profile.Texp);
            SysUi.Row(body, "texp", "修炼一次  400 金币  Texp+25", () =>
            {
                if (app.Profile.Gold < 400)
                {
                    return;
                }

                PhoneNet.TrainTexp();
                app.Profile.Gold -= 400;
                app.Profile.Texp += 25;
                app.Profile.RecalcStats(app.Database);
                app.Profile.Save();
                Show(safe, app);
            });
        }
    }

    public static class GemScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "战魂  Lv." + app.Profile.GemLevel);
            SysUi.Row(body, "gem", "提升战魂  600 金币", () =>
            {
                if (app.Profile.Gold < 600 || app.Profile.GemLevel >= 12)
                {
                    return;
                }

                PhoneNet.UpgradeGem();
                app.Profile.Gold -= 600;
                app.Profile.GemLevel++;
                app.Profile.RecalcStats(app.Database);
                app.Profile.Save();
                Show(safe, app);
            });
            foreach (SpiritInfo s in app.Database.Spirits.Values)
            {
                SysUi.Note(body, $"Lv{s.Level}  ATK+{s.AttackAdd} DEF+{s.DefendAdd} AGI+{s.AgilityAdd}");
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
            int gold = 400 + app.Profile.VipLevel * 80;
            SysUi.Note(body, done ? "今日已领取盟约礼包。" : $"领取金币 +{gold}（随 VIP）");
            if (!done)
            {
                SysUi.Row(body, "kb", "领取", () =>
                {
                    PhoneNet.DoSignIn();
                    app.Profile.KingBlessDay = today;
                    app.Profile.Gold += gold;
                    app.Profile.Save();
                    Show(safe, app);
                });
            }
        }
    }

    public static class FriendScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "好友");
            app.Profile.EnsureStarterBag();
            SysUi.Row(body, "add", "添加 路人甲", () =>
            {
                string name = "路人" + (app.Profile.Friends.Count + 1);
                if (!app.Profile.Friends.Contains(name))
                {
                    PhoneNet.AddFriend(name);
                    app.Profile.Friends.Add(name);
                    app.Profile.Save();
                }

                Show(safe, app);
            });
            foreach (string f in app.Profile.Friends)
            {
                SysUi.Note(body, "好友  " + f);
            }
        }
    }

    public static class MailInboxScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "邮件");
            if (app.Profile.MailGoldWaiting <= 0)
            {
                app.Profile.MailGoldWaiting = 800;
            }

            SysUi.Note(body, $"系统邮件：离线奖励 {app.Profile.MailGoldWaiting} 金币");
            SysUi.Row(body, "claim", "全部领取", () =>
            {
                PhoneNet.RequestProfile();
                app.Profile.Gold += app.Profile.MailGoldWaiting;
                app.Profile.Honor += 10;
                app.Profile.MailGoldWaiting = 0;
                app.Profile.Save();
                Show(safe, app);
            });
        }
    }

    public static class ChatScreen
    {
        public static void Show(RectTransform safe, GameApp app)
        {
            UiKit.ClearChildren(safe);
            var bg = UiKit.PcPanel(safe, "Chat");
            ShopScreen.Header(bg.transform, app, "聊天（本地）");
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
                app.Profile.ChatLog.Add(app.Profile.Nick + ": " + t);
                if (app.Profile.ChatLog.Count > 40)
                {
                    app.Profile.ChatLog.RemoveAt(0);
                }

                app.Profile.Save();
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
            SysUi.Row(body, "def", "使用武器默认炮弹", () =>
            {
                PhoneNet.SelectBall(0);
                app.Profile.PreferredBallId = 0;
                app.Profile.Save();
                Show(safe, app);
            });
            int n = 0;
            foreach (var kv in app.Database.Balls)
            {
                int id = kv.Key;
                var ball = kv.Value;
                var btn = SysUi.Row(body, "ball" + id, $"#{id}  Power{ball.Power}  r{ball.Radii}  W{ball.Wind}  m{ball.Mass}", () =>
                {
                    PhoneNet.SelectBall(id);
                    app.Profile.PreferredBallId = id;
                    app.Profile.Save();
                    Show(safe, app);
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
                        app.Profile.PreferredBallId = b.Common;
                        app.Profile.WeaponId = b.TemplateId;
                        app.Profile.EquipWeapon = b.TemplateId;
                        app.Profile.AddItem(b.TemplateId, 1);
                        app.Profile.Save();
                    }

                    Show(safe, app);
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
}
