using GunMobile.Core;
using GunMobile.Net;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Client
{
    public static class ExtraModulesScreens
    {
        static readonly string[] MagicStoneLabels = { "攻击魔石", "防御魔石", "敏捷魔石", "幸运魔石" };

        public static Transform ShowMornModule(RectTransform safe, GameApp app, ModuleDef module, string uiFile)
        {
            Transform body = SysUi.Begin(safe, app, module.Title);
            if (!MornScreenHost.TryEmbedMorn(body, app, uiFile))
            {
                SysUi.Note(body, "Morn UI: " + uiFile);
            }

            if (!string.IsNullOrEmpty(module.TablePath))
            {
                XmlResultTable table = SysUi.Table(app, module.TablePath);
                if (table != null)
                {
                    int n = 0;
                    foreach (var row in table.Rows)
                    {
                        string line = FormatRow(row);
                        SysUi.Note(body, line);
                        n++;
                        if (n >= 40)
                        {
                            break;
                        }
                    }
                }
            }

            return body;
        }

        static string FormatRow(System.Collections.Generic.IReadOnlyDictionary<string, string> row)
        {
            var sb = new System.Text.StringBuilder(96);
            int c = 0;
            foreach (var kv in row)
            {
                if (c > 0)
                {
                    sb.Append("  ");
                }

                sb.Append(kv.Key).Append('=').Append(kv.Value);
                c++;
                if (c >= 6)
                {
                    break;
                }
            }

            return sb.ToString();
        }

        public static void MagicStoneScreen(RectTransform safe, GameApp app)
        {
            app.Profile.EnsureMagicStones();
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("magicstone", "魔石", "Request/magicstonetemplate.xml", false, "magicStone.ui"),
                "magicStone.ui");
            SysUi.Note(body, "magicstonetemplate.xml · 40级开启（数据来自 PC Request）");

            for (int i = 0; i < app.Profile.MagicStones.Count; i++)
            {
                MagicStoneSlot slot = app.Profile.MagicStones[i];
                string label = i < MagicStoneLabels.Length ? MagicStoneLabels[i] : ("魔石" + slot.TemplateId);
                MagicStoneTemplate row = app.Database != null
                    ? app.Database.GetMagicStone(slot.TemplateId, slot.Level)
                    : null;
                MagicStoneTemplate next = app.Database != null
                    ? app.Database.GetMagicStone(slot.TemplateId, slot.Level + 1)
                    : null;
                int cost = app.Database != null ? app.Database.MagicStoneUpgradeCost(slot.TemplateId, slot.Level) : 0;
                string stats = row != null
                    ? $"ATK{row.Attack} DEF{row.Defence} AGI{row.Agility} LUK{row.Luck} MAG{row.MagicAttack}/{row.MagicDefence}"
                    : "Lv0";
                if (next != null && cost > 0 && slot.Level < 10)
                {
                    int templateId = slot.TemplateId;
                    SysUi.Row(body, "ms" + templateId,
                        label + "  Lv" + slot.Level + "  " + stats + "  → Lv" + (slot.Level + 1) + "  " + cost + " 金",
                        () => PhoneNet.UpgradeMagicStone(templateId));
                }
                else
                {
                    SysUi.Note(body, label + "  Lv" + slot.Level + "  " + stats);
                }
            }
        }

        public static void EnchantScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("enchant", "附魔", "Request/magicfusiondata.xml", false, "enchant.ui"),
                "enchant.ui");
            SysUi.Note(body, "附魔钥匙: " + app.Profile.FusionKeys + "  ·  magicfusiondata.xml");

            if (app.Database == null)
            {
                return;
            }

            int shown = 0;
            foreach (MagicFusionRecipe recipe in app.Database.MagicFusions)
            {
                if (shown >= 12)
                {
                    break;
                }

                ItemTemplate item = app.Database.GetItem(recipe.ItemId);
                string itemName = item != null ? item.Name : ("#" + recipe.ItemId);
                int fusionId = recipe.Id;
                if (recipe.Type == 1)
                {
                    int keyCost = recipe.NeedKey > 0 ? recipe.NeedKey : 10000;
                    SysUi.Row(body, "f" + fusionId,
                        "合成 " + itemName + "  " + recipe.NeedGold + "金 +" + keyCost + "钥匙",
                        () => PhoneNet.MagicFusion(fusionId));
                }
                else if (recipe.GetKeys > 0)
                {
                    SysUi.Row(body, "k" + fusionId,
                        "兑换 +" + recipe.GetKeys + " 钥匙  (消耗 " + itemName + ")",
                        () => PhoneNet.MagicFusion(fusionId));
                }

                shown++;
            }
        }

        public static void TeamDungeonScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("teamdungeon", "团队副本", "Request/battleteamshopitemlist.xml", false, "teamdungeon.ui"),
                "teamdungeon.ui");
            SysUi.Note(body, "battleteamshopitemlist.xml · 魔穴团队战");

            if (app.Database == null)
            {
                SysUi.Row(body, "fight", "开始团队战", app.ShowRoom);
                return;
            }

            var types = new System.Collections.Generic.HashSet<int>();
            foreach (TeamDungeonShopEntry entry in app.Database.TeamDungeonShop)
            {
                types.Add(entry.ShopType);
            }

            foreach (int shopType in types)
            {
                TeamDungeonShopEntry pick = null;
                foreach (TeamDungeonShopEntry entry in app.Database.TeamDungeonShop)
                {
                    if (entry.ShopType == shopType)
                    {
                        pick = entry;
                        break;
                    }
                }

                if (pick == null)
                {
                    continue;
                }

                int fee = pick.Condition > 0 ? pick.Condition * 10 : 500;
                int reward = pick.Value > 0 ? pick.Value : 800;
                int npcId = app.Database.TeamDungeonNpcId(shopType);
                int localType = shopType;
                SysUi.Row(body, "td" + shopType,
                    "难度" + shopType + "  Lv" + pick.NeedLevel + "+  费用" + fee + "  奖励~" + reward + "  NPC" + npcId,
                    () =>
                    {
                        PhoneNet.TeamDungeonStart(localType);
                        app.ShowRoom();
                    });
            }
        }

        public static void CarnivalScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("carnival", "嘉年华", "Request/newlotteryitem.xml", false, "carnival.ui"),
                "carnival.ui");
            int cost = app.Database != null ? app.Database.CarnivalDrawCost() : 500;
            SysUi.Note(body, "高级奖池 type≥10  ·  " + cost + " 金/次");
            SysUi.Row(body, "draw", "嘉年华抽奖  " + cost + " 金", PhoneNet.CarnivalDraw);
            if (app.Database != null)
            {
                int shown = 0;
                foreach (LotteryDrop d in app.Database.LotteryPool(10, 99))
                {
                    SysUi.Note(body, "T" + d.Type + "  " + SysUi.ItemName(app, d.TemplateId) + " x" + d.Count);
                    shown++;
                    if (shown >= 8)
                    {
                        break;
                    }
                }
            }
        }

        public static void BankScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("bank", "银行", null, false, "bank.ui"),
                "bank.ui");
            SysUi.Note(body, "现金: " + app.Profile.Gold + "  ·  存款: " + app.Profile.BankGold);
            SysUi.Row(body, "dep", "存入 1000 金", () => PhoneNet.BankTrade("deposit", 1000));
            SysUi.Row(body, "dep5", "存入 5000 金", () => PhoneNet.BankTrade("deposit", 5000));
            SysUi.Row(body, "wd", "取出 1000 金", () => PhoneNet.BankTrade("withdraw", 1000));
            SysUi.Row(body, "wd5", "取出 5000 金", () => PhoneNet.BankTrade("withdraw", 5000));
        }

        public static void MinesScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("mines", "矿山", null, false, "mines.ui"),
                "mines.ui");
            int maxDigs = app.Database != null ? app.Database.ConfigInt("MineDayLimit", 5) : 5;
            SysUi.Note(body, "今日已挖: " + app.Profile.MineDigs + " / " + maxDigs);
            SysUi.Row(body, "dig", "挖矿 (+金)", PhoneNet.MineDig);
        }

        public static void AuditoriumScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("auditorium", "礼堂", "Request/CelebByDayGPList.xml", false, "auditorium.ui"),
                "auditorium.ui");
            if (app.Database == null)
            {
                return;
            }

            int shown = 0;
            foreach (CelebEntry e in app.Database.CelebGpDay)
            {
                SysUi.Note(body, "#" + e.Rank + "  " + e.Nick + "  Lv" + e.Grade + "  GP+" + e.Gp + "  " + e.ConsortiaName);
                shown++;
                if (shown >= 15)
                {
                    break;
                }
            }
        }

        public static void TreasureScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("treasure", "寻宝", "Request/newlotteryitem.xml", false, "treasureHunting.ui"),
                "treasureHunting.ui");
            int cost = app.Database != null ? app.Database.TreasureDrawCost() : 200;
            SysUi.Note(body, "寻宝奖池 type1-8  ·  " + cost + " 金/次");
            SysUi.Row(body, "draw", "寻宝  " + cost + " 金", PhoneNet.TreasureDraw);
            if (app.Database != null)
            {
                int shown = 0;
                foreach (LotteryDrop d in app.Database.LotteryPool(1, 8))
                {
                    SysUi.Note(body, "T" + d.Type + "  " + SysUi.ItemName(app, d.TemplateId) + " x" + d.Count);
                    shown++;
                    if (shown >= 8)
                    {
                        break;
                    }
                }
            }
        }

        public static void PeakBattleScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("peakbattle", "巅峰战", "Request/areacelebbydayfightpowerlist.xml", false, "peakBattle.ui"),
                "peakBattle.ui");
            if (app.Database == null)
            {
                return;
            }

            var list = app.Database.CelebAreaFightPower.Count > 0
                ? app.Database.CelebAreaFightPower
                : app.Database.CelebFightPowerDay;
            int shown = 0;
            for (int i = 0; i < list.Count; i++)
            {
                CelebEntry e = list[i];
                int fee = 300 + i * 100;
                int rank = i;
                SysUi.Row(body, "pk" + i,
                    "#" + (i + 1) + "  " + e.Nick + "  Lv" + e.Grade + "  战力" + e.FightPower + "  " + fee + "金",
                    () =>
                    {
                        PhoneNet.PeakBattleStart(rank);
                        app.ShowRoom();
                    });
                shown++;
                if (shown >= 10)
                {
                    break;
                }
            }
        }

        public static void NecklaceScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("necklace", "项链", "Request/TS_NecklaceCasting.xml", false, "necklace.ui"),
                "necklace.ui");
            int level = app.Profile.NecklaceLevel;
            NecklaceCastingLevel row = app.Database != null ? app.Database.GetNecklaceLevel(level) : null;
            NecklaceCastingLevel next = app.Database != null ? app.Database.GetNecklaceLevel(level + 1) : null;
            int cost = app.Database != null ? app.Database.NecklaceUpgradeCost(level) : 0;
            string stats = row != null
                ? "HP+" + row.Hp + "  DEF+" + (row.Toughness / 10 + row.Guardian / 10)
                : "Lv0";
            SysUi.Note(body, "TS_NecklaceCasting.xml  ·  当前 Lv" + level + "  " + stats);
            if (next != null && cost > 0)
            {
                SysUi.Row(body, "up", "升级 → Lv" + (level + 1) + "  HP+" + next.Hp + "  " + cost + " 金",
                    PhoneNet.UpgradeNecklace);
            }
            else
            {
                SysUi.Note(body, "已达最高等级");
            }
        }

        public static void DevilTurnScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("devilturn", "恶魔转盘", "Request/DevilTreasItemList.xml", false, "devilturn.ui"),
                "devilturn.ui");
            int unitCost = app.Database != null ? app.Database.ConfigInt("DevilTreasureOneCost", 10000) : 10000;
            int tenCost = app.Database != null ? app.Database.ConfigInt("DevilTreasureTenCost", unitCost * 10) : unitCost * 10;
            SysUi.Note(body, "DevilTreasItemList.xml  ·  今日已转 " + app.Profile.DevilTurnSpins + " 次");
            SysUi.Row(body, "spin1", "转1次  " + unitCost + " 金", () => PhoneNet.DevilTurnSpin(1));
            SysUi.Row(body, "spin10", "转10次  " + tenCost + " 金", () => PhoneNet.DevilTurnSpin(10));
            if (app.Database != null)
            {
                int shown = 0;
                foreach (DevilTreasItem item in app.Database.DevilTreasItems)
                {
                    SysUi.Note(body, "T" + item.Type + "  " + SysUi.ItemName(app, item.TemplateId) +
                        " x" + item.Value + "  权重" + item.Weight);
                    shown++;
                    if (shown >= 8)
                    {
                        break;
                    }
                }
            }
        }

        public static void RedPacketScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("redpacket", "红包", null, false, "redpacket.ui"),
                "redpacket.ui");
            int maxClaims = app.Database != null ? app.Database.ConfigInt("RedPacketDayLimit", 5) : 5;
            SysUi.Note(body, "每日红包  ·  已领 " + app.Profile.RedPacketClaims + " / " + maxClaims);
            SysUi.Row(body, "claim", "开红包", PhoneNet.ClaimRedPacket);
        }

        public static void HomeTempleScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("homeTemple", "家园神殿", null, false, "homeTemple.ui"),
                "homeTemple.ui");
            int level = app.Profile.HomeTempleLevel;
            int maxLevel = app.Database != null ? app.Database.ConfigInt("HomeTempleMaxLevel", 20) : 20;
            int cost = app.Database != null ? app.Database.HomeTempleUpgradeCost(level) : 800;
            SysUi.Note(body, "当前 Lv" + level + " / " + maxLevel + "  ·  ATK+" + (level * 15) + " HP+" + (level * 120));
            if (level < maxLevel)
            {
                SysUi.Row(body, "up", "升级 → Lv" + (level + 1) + "  " + cost + " 金", PhoneNet.UpgradeHomeTemple);
            }
            else
            {
                SysUi.Note(body, "已达最高等级");
            }
        }

        public static void SweepScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("sweep", "扫荡", "Request/fightlabdropitemlist.xml", false, "sweep.ui"),
                "sweep.ui");
            int maxSweeps = app.Database != null ? app.Database.ConfigInt("LabyrinthSweepDayLimit", 3) : 3;
            int floor = Mathf.Max(1, app.Profile.LabyrinthFloor);
            int estGold = app.Database != null ? app.Database.ComputePveWinGold(0, floor, true) : floor * 50;
            SysUi.Note(body, "迷宫层 " + floor + "  ·  预计 +" + estGold + " 金  ·  今日 " +
                app.Profile.SweepCount + " / " + maxSweeps);
            SysUi.Row(body, "sweep", "扫荡本层 (跳过战斗)", PhoneNet.SweepLabyrinth);
            SysUi.Row(body, "fight", "手动挑战", () => LabyrinthScreen.Show(safe, app));
        }

        public static void FirstRechargeScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("firstrecharge", "首充",
                    "Request/ts_firstpayshoptemp.xml", false, "firstrecharge.ui"),
                "firstrecharge.ui");
            FirstRechargeConfig cfg = app.Database != null ? app.Database.GetFirstRechargeConfig() : null;
            ActivityConfigEntry activity = null;
            if (app.Database != null)
            {
                app.Database.ActivityConfigs.TryGetValue(8, out activity);
            }

            string activityName = activity != null ? activity.Name : "首充";
            SysUi.Note(body, "TS_ActivityConfig Num=8  ·  " + activityName +
                "  ·  已领取: " + (app.Profile.FirstRechargeClaimed ? "是" : "否"));
            if (cfg != null && cfg.RewardItemIds.Length > 0)
            {
                for (int i = 0; i < cfg.RewardItemIds.Length; i++)
                {
                    int itemId = cfg.RewardItemIds[i];
                    int count = i < cfg.RewardCounts.Length ? cfg.RewardCounts[i] : 1;
                    SysUi.Note(body, "奖励  " + SysUi.ItemName(app, itemId) + " x" + count);
                }
            }

            if (!app.Profile.FirstRechargeClaimed)
            {
                SysUi.Row(body, "claim", "领取首充奖励", PhoneNet.ClaimFirstRecharge);
            }

            SysUi.Note(body, "首充商城 (ts_firstpayshoptemp.xml)  ·  金豆 " + app.Profile.Gift);
            if (app.Database != null)
            {
                int shown = 0;
                foreach (FirstPayShopItem item in app.Database.FirstPayShop)
                {
                    string label = SysUi.ItemName(app, item.ItemTempId) + " x" + item.ItemTempCount +
                        "  ·  " + item.NeedGoldBeans + " 豆  ·  限购 " + item.LimitBuyCount;
                    int templateId = item.TemplateId;
                    SysUi.Row(body, "shop" + templateId, label, () => PhoneNet.BuyFirstRechargeShop(templateId));
                    shown++;
                    if (shown >= 12)
                    {
                        break;
                    }
                }
            }
        }
    }
}
