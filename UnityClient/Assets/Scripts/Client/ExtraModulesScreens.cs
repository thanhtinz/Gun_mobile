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
                new ModuleDef("bank", "银行", "Request/banktemplateinfo.xml", false, "bank.ui"),
                "bank.ui");
            app.Profile.EnsureBankDeposits();
            SysUi.Note(body, "现金: " + app.Profile.Gold + "  ·  活期存款: " + app.Profile.BankGold);
            SysUi.Row(body, "dep", "存入 1000 金", () => PhoneNet.BankTrade("deposit", 1000));
            SysUi.Row(body, "dep5", "存入 5000 金", () => PhoneNet.BankTrade("deposit", 5000));
            SysUi.Row(body, "wd", "取出 1000 金", () => PhoneNet.BankTrade("withdraw", 1000));
            SysUi.Row(body, "wd5", "取出 5000 金", () => PhoneNet.BankTrade("withdraw", 5000));
            if (app.Database != null && app.Database.BankTemplates.Count > 0)
            {
                SysUi.Note(body, "banktemplateinfo.xml  ·  定期存款");
                foreach (KeyValuePair<int, BankTemplate> kv in app.Database.BankTemplates)
                {
                    BankTemplate tpl = kv.Value;
                    if (tpl.DeadLine <= 0) continue;
                    int tplId = tpl.Id, depAmount = tpl.MinAmount;
                    SysUi.Row(body, "term" + tplId, tpl.Name + "  " + depAmount + "金  利率" + (tpl.InterestRate / 10f) + "%",
                        () => PhoneNet.BankDeposit("deposit", tplId, depAmount));
                }
                for (int i = 0; i < app.Profile.BankDeposits.Count; i++)
                {
                    BankTermDeposit dep = app.Profile.BankDeposits[i];
                    BankTemplate tpl = app.Database.GetBankTemplate(dep.TemplateId);
                    string tplName = tpl != null ? tpl.Name : ("#" + dep.TemplateId);
                    int slot = i;
                    SysUi.Row(body, "wdterm" + slot, "取出 " + tplName + "  " + dep.Amount + "金",
                        () => PhoneNet.BankDeposit("withdraw", dep.TemplateId, 0, slot));
                }
            }
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
            if (app.Database == null) return;
            int maxActions = app.Database.ConfigInt("HonorSystemAwardLimit", 6);
            SysUi.Note(body, "CelebByDayGPList  ·  荣誉 " + app.Profile.Honor + "  ·  今日 " + app.Profile.AuditoriumActions + " / " + maxActions);
            SysUi.Row(body, "wedding", "举办婚礼  " + app.Database.AuditoriumWeddingCost(0) + " 金", () => PhoneNet.AuditoriumAction("wedding", 0));
            if (app.Database.Fireworks.Count == 0) app.Database.LoadFireworksFromConfig();
            for (int i = 0; i < app.Database.Fireworks.Count && i < 4; i++)
            {
                FireworkEntry fw = app.Database.Fireworks[i];
                int index = i;
                SysUi.Row(body, "fire" + i, "烟花  " + SysUi.ItemName(app, fw.TemplateId) + "  " + fw.GoldCost + "金  +" + fw.HonorGain + "荣誉",
                    () => PhoneNet.AuditoriumAction("fire", index));
            }
            SysUi.Row(body, "redpacket", "红包主持  " + (app.Database.ConfigInt("RedPacketMinGold", 100) * 10) + " 金", () => PhoneNet.AuditoriumAction("redpacket"));
            if (!string.IsNullOrEmpty(PhoneNet.LastAuditoriumJson)) SysUi.Note(body, PhoneNet.LastAuditoriumJson);
            int shown = 0;
            foreach (CelebEntry e in app.Database.CelebGpDay)
            {
                SysUi.Note(body, "#" + e.Rank + "  " + e.Nick + "  Lv" + e.Grade + "  GP+" + e.Gp);
                if (++shown >= 10) break;
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
            SysUi.Note(body, "DevilTreasPointsList.xml  ·  积分 " + app.Profile.DevilTurnPoints + "  ·  今日已转 " + app.Profile.DevilTurnSpins + " 次");
            SysUi.Row(body, "spin1", "转1次  " + unitCost + " 金", () => PhoneNet.DevilTurnSpin(1));
            SysUi.Row(body, "spin10", "转10次  " + tenCost + " 金", () => PhoneNet.DevilTurnSpin(10));
            if (app.Database != null)
            {
                var milestones = new System.Collections.Generic.List<DevilTreasPointReward>(app.Database.DevilTreasPointRewards.Values);
                milestones.Sort((a, b) => a.Points.CompareTo(b.Points));
                foreach (DevilTreasPointReward reward in milestones)
                {
                    bool claimed = app.Profile.DevilTreasPointClaimed != null && app.Profile.DevilTreasPointClaimed.Contains(reward.Id);
                    string state = claimed ? "已领" : app.Profile.DevilTurnPoints >= reward.Points ? "可领" : "未达";
                    int rid = reward.Id;
                    SysUi.Row(body, "mile" + reward.Id,
                        state + "  " + reward.Points + "分 → " + SysUi.ItemName(app, reward.TemplateId),
                        claimed ? (System.Action)(() => { }) : () => PhoneNet.ClaimDevilTreasPoint(rid));
                }

                SysUi.Note(body, "--- 排行奖励 DevilTreasRankRewardList.xml ---");
                app.Profile.EnsureDevilTreasRankClaimed();
                foreach (DevilTreasRankReward row in app.Database.DevilTreasRankRewards)
                {
                    bool claimed = app.Profile.DevilTreasRankClaimed.Contains(row.Id);
                    int rid = row.Id;
                    SysUi.Row(body, "rank" + row.Id,
                        (claimed ? "已领" : "领取") + "  #" + row.RankMin + "-" + row.RankMax + "  " + row.Desc,
                        claimed ? (System.Action)(() => { }) : () => PhoneNet.ClaimDevilTreasRank(rid));
                }
                if (app.Database.DevilTreasSarahToBoxes.Count > 0)
                    SysUi.Note(body, "SarahToBox 兑换 " + app.Database.DevilTreasSarahToBoxes.Count + " 项");

                int shown = 0;
                foreach (DevilTreasItem item in app.Database.DevilTreasItems)
                {
                    SysUi.Note(body, "T" + item.Type + "  " + SysUi.ItemName(app, item.TemplateId) +
                        " x" + item.Value + "  权重" + item.Weight);
                    shown++;
                    if (shown >= 8) break;
                }
            }
        }

        public static void RecycleActivityScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("recycle", "变废为宝", "Request/RecycleActivityInfo.xml", false, "wasteRecycle.ui"),
                "wasteRecycle.ui");
            SysUi.Note(body, "RecycleActivityInfo.xml  ·  积分 " + app.Profile.RecyclePoints);
            if (app.Database == null || app.Database.RecycleActivityItems.Count == 0)
            {
                SysUi.Note(body, "未加载回收表");
                return;
            }
            int shown = 0;
            foreach (RecycleActivityItem row in app.Database.RecycleActivityItems.Values)
            {
                int tid = row.TemplateId;
                string name = tid > 0 ? SysUi.ItemName(app, tid) : ("货币 " + tid);
                SysUi.Row(body, "rec" + tid,
                    "回收 " + name + " x" + row.Count + " → +" + row.Integral + "分",
                    () => PhoneNet.RecycleActivityClaim(tid, 1));
                shown++;
                if (shown >= 12) break;
            }
        }

        public static void MagicItemScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("magicitem", "魔法道具", "Request/MagicItemTemp.xml", false, "magicStone.ui"),
                "magicStone.ui");
            int level = app.Profile.MagicItemLevel;
            MagicItemLevel cur = app.Database != null ? app.Database.GetMagicItemLevel(level) : null;
            MagicItemLevel next = app.Database != null ? app.Database.GetMagicItemLevel(level + 1) : null;
            int cost = app.Database != null ? app.Database.MagicItemUpgradeCost(level) : 0;
            string stats = cur != null ? "MA+" + cur.MagicAttack + "  MD+" + cur.MagicDefence : "Lv0";
            SysUi.Note(body, "MagicItemTemp.xml  ·  当前 Lv" + level + "  " + stats);
            if (next != null && cost > 0)
                SysUi.Row(body, "up", "升级 → Lv" + (level + 1) + "  MA+" + next.MagicAttack + "  " + cost + " 金",
                    PhoneNet.UpgradeMagicItem);
            else
                SysUi.Note(body, "已达最高等级");
        }

        public static void RedPacketScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("redpacket", "红包", null, false, "redpacket.ui"),
                "redpacket.ui");
            int maxClaims = app.Database != null ? app.Database.ConfigInt("RedPacketDayLimit", 5) : 5;
            SysUi.Note(body, "每日红包  ·  已领 " + app.Profile.RedPacketClaims + " / " + maxClaims);
            SysUi.Row(body, "claim", "开红包", PhoneNet.ClaimRedPacket);
            if (app.Profile.Friends != null && app.Profile.Friends.Count > 0)
            {
                string friend = app.Profile.Friends[0];
                SysUi.Row(body, "send", "发红包给 " + friend + "  1000金", () => PhoneNet.SendRedPacket(friend, 1000));
            }
        }

public static void HomeTempleScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("homeTemple", "家园神殿", "Request/HomeTempPracticeList.xml", false, "homeTemple.ui"), "homeTemple.ui");
            int level = app.Profile.HomeTempleLevel;
            int maxLevel = app.Database != null ? app.Database.ConfigInt("HomeTempleMaxLevel", 20) : 20;
            int cost = app.Database != null ? app.Database.HomeTempleUpgradeCost(level) : 800;
            SysUi.Note(body, "建筑 Lv" + level + " / " + maxLevel);
            if (level < maxLevel) SysUi.Row(body, "up", "升级 → Lv" + (level + 1) + "  " + cost + " 金", PhoneNet.UpgradeHomeTemple);
            int practice = app.Profile.HomeTemplePracticeLevel;
            int practiceMax = app.Database != null ? app.Database.HomeTemplePracticeMaxLevel() : 0;
            int practiceCost = app.Database != null ? app.Database.HomeTemplePracticeCost(practice) : 0;
            SysUi.Note(body, "HomeTempPracticeList.xml  ·  修炼 Lv" + practice + " / " + practiceMax);
            if (practiceMax > 0 && practice < practiceMax && practiceCost > 0)
                SysUi.Row(body, "practice", "修炼 → Lv" + (practice + 1) + "  " + practiceCost + " 金", PhoneNet.HomeTemplePractice);
            int advance = app.Profile.HomeTempleAdvanceLevel;
            int advanceMax = app.Database != null ? app.Database.HomeTempleAdvanceMaxLevel() : 0;
            int advanceCost = app.Database != null ? app.Database.HomeTempleAdvanceCost(advance) : 0;
            SysUi.Note(body, "TS_HomeTempAdvance_Template.xml  ·  升华 Lv" + advance + " / " + advanceMax);
            if (advanceMax > 0 && advance < advanceMax && advanceCost > 0)
                SysUi.Row(body, "advance", "升华 → Lv" + (advance + 1) + "  " + advanceCost + " 金", PhoneNet.HomeTempleAdvance);
        }

        public static void SweepScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("sweep", "扫荡", "Request/ts_sweepmisson.xml", false, "sweep.ui"), "sweep.ui");
            app.Profile.EnsureSweepMissionClears();
            int maxSweeps = app.Database != null ? app.Database.ConfigInt("LabyrinthSweepDayLimit", 3) : 3;
            int floor = Mathf.Max(1, app.Profile.LabyrinthFloor);
            int estGold = app.Database != null ? app.Database.ComputePveWinGold(0, floor, true) : floor * 50;
            SysUi.Note(body, "迷宫层 " + floor + "  ·  预计 +" + estGold + " 金  ·  今日 " +
                app.Profile.SweepCount + " / " + maxSweeps);
            SysUi.Row(body, "sweep", "扫荡本层 (跳过战斗)", PhoneNet.SweepLabyrinth);
            SysUi.Row(body, "fight", "手动挑战", () => LabyrinthScreen.Show(safe, app));
            if (app.Database != null && app.Database.SweepMissions.Count > 0)
            {
                SysUi.Note(body, "ts_sweepmisson.xml  ·  任务扫荡");
                int shown = 0;
                foreach (SweepMissionInfo mission in app.Database.SweepMissions)
                {
                    bool unlocked = app.Database.CanSweepMission(app.Profile.Level, floor, app.Profile.SweepMissionClears, mission);
                    int reward = app.Database.SweepMissionGoldReward(mission);
                    string mark = app.Profile.SweepMissionClears.Contains(mission.MissionId) ? "✓" : (unlocked ? "→" : "🔒");
                    int missionId = mission.MissionId;
                    if (unlocked)
                        SysUi.Row(body, "miss" + missionId, mark + " " + mission.Name + "  +" + reward + "金", () => PhoneNet.SweepMission(missionId));
                    else
                        SysUi.Note(body, mark + " " + mission.Name + "  (未解锁)");
                    if (++shown >= 10) break;
                }
            }
        }
        static readonly string[] EmblemTypeNames = { "", "武器", "副手", "衣服", "帽子" };

        public static void EmblemScreen(RectTransform safe, GameApp app)
        {
            app.Profile.EnsureEmblems();
            Transform body = ShowMornModule(safe, app, new ModuleDef("emblem", "徽章", "Request/TS_Emblem.xml", false, "emblem.ui"), "emblem.ui");
            int success = app.Database != null ? app.Database.EmblemComposeSuccessRate() : 700;
            SysUi.Note(body, "TS_Emblem.xml  ·  成功率 " + (success / 10) + "%  ·  已拥有 " + app.Profile.Emblems.Count);
            for (int types = 1; types <= 4; types++)
            {
                for (int profile = 1; profile <= 3; profile++)
                {
                    EmblemTemplate row = null;
                    if (app.Database != null)
                        for (int i = 0; i < app.Database.EmblemList.Count; i++)
                            if (app.Database.EmblemList[i].Types == types && app.Database.EmblemList[i].Profile == profile) { row = app.Database.EmblemList[i]; break; }
                    int cost = row != null && app.Database != null ? app.Database.EmblemCraftGoldCost(row) : 0;
                    string slotName = types < EmblemTypeNames.Length ? EmblemTypeNames[types] : ("部位" + types);
                    int t = types, p = profile;
                    SysUi.Row(body, "craft" + t + "p" + p, slotName + " P" + profile + "  合成 " + cost + " 金", () => PhoneNet.CraftEmblem(t, p));
                }
            }
            for (int i = 0; i < app.Profile.Emblems.Count; i++)
            {
                EmblemSlot slot = app.Profile.Emblems[i];
                string slotName = slot.Types < EmblemTypeNames.Length ? EmblemTypeNames[slot.Types] : ("T" + slot.Types);
                string label = "#" + slot.Id + " " + slotName + " P" + slot.Profile + " 主" + slot.MainValue + (slot.SubValue > 0 ? " 副" + slot.SubValue : "") + (slot.Equipped != 0 ? " [已装备]" : "");
                int id = slot.Id;
                if (slot.Equipped == 0) SysUi.Row(body, "eq" + id, label + "  ·  装备", () => PhoneNet.EquipEmblem(id, 1));
                else SysUi.Row(body, "ueq" + id, label + "  ·  卸下", () => PhoneNet.EquipEmblem(id, 0));
            }
        }

        public static void SoulMarkScreen(RectTransform safe, GameApp app)
        {
            app.Profile.EnsureSoulStamps();
            Transform body = ShowMornModule(safe, app, new ModuleDef("soulmark", "魂印", "Request/TS_SoulStampTemplate.xml", false, "soulMark.ui"), "soulMark.ui");
            SysUi.Note(body, "TS_SoulStampTemplate.xml  ·  已拥有 " + app.Profile.SoulStamps.Count);
            for (int quality = 1; quality <= 5; quality++)
            {
                SoulStampComposeTemplate compose = app.Database != null ? app.Database.GetSoulStampCompose(quality) : null;
                int cost = compose != null && app.Database != null ? app.Database.SoulStampComposeGoldCost(compose) : 0;
                int q = quality;
                SysUi.Row(body, "compose" + q, "品质" + q + "  合成 " + cost + " 金", () => PhoneNet.ComposeSoulStamp(q));
            }
            for (int i = 0; i < app.Profile.SoulStamps.Count; i++)
            {
                SoulStampSlot slot = app.Profile.SoulStamps[i];
                SoulRefineRatio next = app.Database != null ? app.Database.GetSoulRefine(slot.Type, slot.Grade + 1) : null;
                int refineCost = next != null && app.Database != null ? app.Database.SoulStampRefineGoldCost(next) : 0;
                string pro = slot.ProType == 1 ? "ATK" : slot.ProType == 2 ? "DEF" : slot.ProType == 3 ? "AGI" : "LUK";
                string label = "#" + slot.Id + " Q" + slot.Quality + " G" + slot.Grade + " " + pro + "+" + slot.ProValue + (slot.Equipped != 0 ? " [已装备]" : "");
                int id = slot.Id;
                if (next != null && refineCost > 0)
                    SysUi.Row(body, "ref" + id, label + "  ·  精炼 →G" + (slot.Grade + 1) + " " + refineCost + " 金", () => PhoneNet.RefineSoulStamp(id));
                else SysUi.Note(body, label);
            }

            SysUi.Note(body, "--- 符印 TS_SigilProValueLimitTemp.xml ---");
            int sigilCost = app.Database != null ? app.Database.SigilRollGoldCost() : 500;
            SysUi.Note(body, "当前 ProType " + app.Profile.SigilProType + " +" + app.Profile.SigilProValue);
            SysUi.Row(body, "sigilroll", "重铸符印  Q" + UnityEngine.Mathf.Max(1, app.Profile.SigilQuality) + "  " + sigilCost + "金",
                () => PhoneNet.RollSigil(UnityEngine.Mathf.Max(1, app.Profile.SigilQuality)));
        }

        public static void SigilScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "符印 · Sigil");
            int cost = app.Database != null ? app.Database.SigilRollGoldCost() : 500;
            SysUi.Note(body, "TS_SigilProValueLimitTemp.xml  ·  RandomConfig 权重  ·  SigilRollGold");
            SysUi.Note(body, "当前 Q" + UnityEngine.Mathf.Max(1, app.Profile.SigilQuality) + "  ProType " + app.Profile.SigilProType + " +" + app.Profile.SigilProValue);
            if (app.Database == null) return;
            for (int q = 1; q <= 5; q++)
            {
                int quality = q;
                SysUi.Row(body, "sigil" + q, "品质" + q + "  重铸  " + cost + "金", () => PhoneNet.RollSigil(quality));
            }
        }

        public static void JadeScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "玉石 · Jade");
            SysUi.Note(body, "TS_JadeTemp.xml  ·  PhoneMsg 198 JadeEquip");
            JadeTemp equipped = app.Database != null ? app.Database.GetJade(app.Profile.JadeEquipId) : null;
            SysUi.Note(body, equipped != null
                ? "已装备 " + equipped.TemplateName + " Lv" + equipped.Level + "  ATK+" + equipped.Attack + " HP+" + equipped.Hp
                : "未装备玉石");
            if (app.Profile.JadeEquipId > 0)
                SysUi.Row(body, "jadeoff", "卸下玉石", () => PhoneNet.EquipJade(0));
            if (app.Database == null) return;
            int shown = 0;
            foreach (JadeTemp row in app.Database.JadeList)
            {
                if (row.Level != 1 && row.Level != 5 && row.Level != 10) continue;
                JadeTemp local = row;
                bool on = app.Profile.JadeEquipId == row.Id;
                string stats = "ATK" + row.Attack + " DEF" + row.Defence + " LUK" + row.Luck + " HP" + row.Hp + " MAG" + row.MagicAttack;
                SysUi.Row(body, "jade" + row.Id,
                    (on ? "[装备] " : "") + row.TemplateName + " T" + row.Types + "  " + stats,
                    () => PhoneNet.EquipJade(local.Id));
                if (++shown >= 36) break;
            }
        }

        public static void RuneScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "符文 · Rune");
            SysUi.Note(body, "runetemplatelist.xml  ·  PhoneMsg 199 RuneEquip");
            RuneTemplate equipped = app.Database != null ? app.Database.GetRune(app.Profile.RuneTemplateId) : null;
            SysUi.Note(body, equipped != null
                ? "已装备 " + equipped.Name + " #" + equipped.TemplateId + " Type" + equipped.Type1
                : "未装备符文");
            if (app.Profile.RuneTemplateId > 0)
                SysUi.Row(body, "runeoff", "卸下符文", () => PhoneNet.EquipRune(0));
            if (app.Database == null) return;
            int shown = 0;
            foreach (RuneTemplate row in app.Database.RuneList)
            {
                if (row.BaseLevel > 3) continue;
                RuneTemplate local = row;
                bool on = app.Profile.RuneTemplateId == row.TemplateId;
                SysUi.Row(body, "rune" + row.TemplateId,
                    (on ? "[装备] " : "") + row.Name + " Lv" + row.BaseLevel + "  Type" + row.Type1 + " " + row.Attribute1,
                    () => PhoneNet.EquipRune(local.TemplateId));
                if (++shown >= 24) break;
            }
        }

        public static void HorseAmuletScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "坐骑护符 · HorseAmulet");
            SysUi.Note(body, "amuletinfo / amuletgrade / amuletphase  ·  PhoneMsg 200");
            int level = UnityEngine.Mathf.Max(1, app.Profile.HorseAmuletLevel);
            int grade = UnityEngine.Mathf.Max(1, app.Profile.HorseAmuletGrade);
            int phase = UnityEngine.Mathf.Max(1, app.Profile.HorseAmuletPhase);
            HorseAmuletInfo info = app.Database != null ? app.Database.GetHorseAmuletInfo(level) : null;
            HorseAmuletPhase phaseRow = app.Database != null ? app.Database.GetHorseAmuletPhase(phase) : null;
            SysUi.Note(body, "等级 " + level + "  品阶 " + grade + "  阶段 " + phase +
                (info != null ? ("  HP+" + info.Hp) : "") +
                (phaseRow != null ? ("  Kill" + phaseRow.Kill + " Guard" + phaseRow.Guard) : ""));
            if (app.Database == null) return;
            int levelCost = app.Database.HorseAmuletLevelGoldCost(level);
            int gradeCost = app.Database.HorseAmuletGradeGoldCost(grade);
            int phaseCost = app.Database.HorseAmuletPhaseGoldCost(phase);
            if (app.Database.GetHorseAmuletInfo(level + 1) != null || app.Database.HorseAmuletInfos.Count == 0)
                SysUi.Row(body, "halvl", "升级护符  " + levelCost + "金 (Expend/LockPrice)", () => PhoneNet.UpgradeHorseAmulet("level"));
            if (gradeCost > 0)
                SysUi.Row(body, "hagrade", "提升品阶  " + gradeCost + "金 (WahsTimes)", () => PhoneNet.UpgradeHorseAmulet("grade"));
            if (phaseCost > 0)
                SysUi.Row(body, "haphase", "提升阶段  " + phaseCost + "金 (Expend)", () => PhoneNet.UpgradeHorseAmulet("phase"));
        }

        public static void DreamlandScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("dreamland", "梦境", "Request/TS_StoryCopySectionTemplate.xml", false, "dreamlandChallenge.ui"),
                "dreamlandChallenge.ui");
            if (app.Database == null)
            {
                SysUi.Row(body, "fight", "开始梦境战", app.ShowRoom);
                return;
            }

            int chapter = Mathf.Max(1, app.Profile.DreamlandChapter);
            int section = Mathf.Max(1, app.Profile.DreamlandSection);
            StoryCopyChapter ch = app.Database.GetStoryCopyChapter(chapter);
            StoryCopySection row = app.Database.GetStoryCopySection(chapter, section);
            if (row == null && app.Database.StoryCopySections.Count > 0)
            {
                row = app.Database.StoryCopySections[0];
                chapter = row.Chapter;
                section = row.Section;
            }

            if (ch != null)
            {
                SysUi.Note(body, ch.Name + "  ·  " + ch.SectionCount + " 关");
            }

            if (row != null)
            {
                int fee = app.Database.DreamlandEntryFee(row);
                int reward = app.Database.DreamlandRewardGold(row, app.Database.DreamlandNpcId(row, app.Profile.Level));
                int npcId = app.Database.DreamlandNpcId(row, app.Profile.Level);
                int mapId = app.Database.DreamlandMapId(row);
                int localChapter = chapter;
                int localSection = section;
                SysUi.Note(body, "第" + section + "关 " + row.Name + "  ·  今日 " +
                    app.Profile.DreamlandAttempts + " / " + row.PlayLimit + "  ·  已通 " + app.Profile.DreamlandClearedSection);
                SysUi.Row(body, "fight",
                    row.Name + "  费用" + fee + "  奖励~" + reward + "  NPC" + npcId,
                    () =>
                    {
                        PhoneNet.DreamlandStart(localChapter, localSection);
                        PhoneNet.PendingPveMapId = mapId;
                        PhoneNet.PendingPveNpcId = npcId;
                        app.ShowRoom();
                    });
            }

            if (app.Profile.DreamlandClearedSection > 0)
            {
                StoryCopySection cleared = app.Database.GetStoryCopySection(chapter, app.Profile.DreamlandClearedSection);
                if (cleared != null && !string.IsNullOrEmpty(cleared.SweepReward))
                {
                    int clearChapter = chapter;
                    int clearSection = app.Profile.DreamlandClearedSection;
                    SysUi.Row(body, "claim",
                        "领取扫荡 " + cleared.Name + "  " + cleared.SweepReward,
                        () => PhoneNet.DreamlandClaim(clearChapter, clearSection));
                }
            }

            int questShown = 0;
            foreach (StoryCopyQuest quest in app.Database.StoryCopyQuests)
            {
                if (quest.ChapterId != chapter)
                {
                    continue;
                }

                SysUi.Note(body, quest.Name + "  " + quest.QuestAward + "  +" + quest.QuestScore + "分");
                questShown++;
                if (questShown >= 5)
                {
                    break;
                }
            }
        }

        public static void DarkBoundaryScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("darkboundary", "暗界", "Request/ts_warriorfamfightconfig.xml", false, "darkboundary.ui"),
                "darkboundary.ui");
            if (app.Database == null)
            {
                SysUi.Row(body, "fight", "开始勇士秘境", app.ShowRoom);
                return;
            }

            int needLevel = app.Database.ConfigInt("WarriorFamGradeLimit", 30);
            int hardType = Mathf.Clamp(app.Profile.WarriorFamHardType, 0, 2);
            int level = Mathf.Max(1, app.Profile.WarriorFamLevel);
            int fee = app.Database.WarriorFamEntryFee();
            int maxAttempts = app.Database.ConfigInt("WarriorFamEveryDayContinueCount", 1);
            string[] hardNames = { "普通", "困难", "噩梦" };
            SysUi.Note(body, "勇士秘境  Lv" + needLevel + "+  ·  " + hardNames[hardType] +
                "  第" + level + "层  ·  今日 " + app.Profile.WarriorFamAttempts + " / " + maxAttempts);

            WarriorFamFightConfig row = app.Database.GetWarriorFamFight(hardType, level);
            if (row != null)
            {
                int reward = app.Database.WarriorFamRewardGold(row);
                int npcId = app.Database.WarriorFamNpcId(row);
                int localHard = hardType;
                int localLevel = level;
                SysUi.Row(body, "fight" + level,
                    "挑战 L" + level + "  NPC" + npcId + "  费用" + fee + "  奖励~" + reward,
                    () =>
                    {
                        PhoneNet.WarriorFamStart(localHard, localLevel);
                        PhoneNet.PendingPveNpcId = npcId;
                        app.ShowRoom();
                    });
            }

            if (app.Profile.WarriorFamClearedLevel > 0)
            {
                WarriorFamFightConfig cleared = app.Database.GetWarriorFamFight(hardType, app.Profile.WarriorFamClearedLevel);
                if (cleared != null)
                {
                    int localHard = hardType;
                    int clearLevel = app.Profile.WarriorFamClearedLevel;
                    SysUi.Row(body, "claim",
                        "领取 L" + clearLevel + " 奖励  " + cleared.Rewards,
                        () => PhoneNet.WarriorFamClaim(localHard, clearLevel));
                }
            }

            int rankShown = 0;
            foreach (WarriorFamRankEntry entry in app.Database.WarriorFamRanks)
            {
                SysUi.Note(body, "#" + entry.Rank + "  " + entry.Nick + "  L" + entry.Level + "  战力" + entry.FightPower);
                rankShown++;
                if (rankShown >= 8)
                {
                    break;
                }
            }

            if (rankShown == 0)
            {
                SysUi.Note(body, "warriorfamranklist.xml 暂无排行数据");
            }
        }
        public static void MagicWardrobeScreen(RectTransform safe, GameApp app)
        {
            app.Profile.EnsureWardrobeProperties();
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("magicwardrobe", "魔衣橱", "Request/magicclothlist.xml", false, "magicwardrobe.ui"),
                "magicwardrobe.ui");
            SysUi.Note(body, "magicclothlist.xml + clothpropertytemplateinfo.xml  ·  已激活 " + app.Profile.WardrobeProperties.Count);
            if (app.Database == null) return;
            int shown = 0;
            foreach (MagicClothInfo cloth in app.Database.MagicClothList)
            {
                if (cloth.HasShow == 0 && cloth.Id != app.Profile.WardrobeClothId) continue;
                bool on = app.Profile.WardrobeClothId == cloth.Id;
                int id = cloth.Id;
                SysUi.Row(body, "mw" + cloth.Id, (on ? "[穿戴] " : "") + cloth.Name, () => PhoneNet.WardrobeEquip(id));
                if (++shown >= 20) break;
            }
            SysUi.Note(body, "--- 衣橱属性 ---");
            shown = 0;
            foreach (ClothPropertyInfo prop in app.Database.ClothProperties.Values)
            {
                if (prop.Type != 1 && prop.Type != 2) continue;
                bool owned = app.Profile.HasWardrobeProperty(prop.Id);
                int pid = prop.Id;
                if (!owned && prop.Cost > 0)
                    SysUi.Row(body, "wp" + prop.Id, "激活 " + prop.Name + "  " + prop.Cost + "金", () => PhoneNet.WardrobeUpgrade(pid));
                else
                    SysUi.Note(body, (owned ? "[已激活] " : "") + prop.Name + " ATK+" + prop.Attack + " HP+" + prop.Blood);
                if (++shown >= 16) break;
            }
        }

        public static void HonorHallScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("honorhall", "荣誉", "Request/ts_honorsystem_template.xml", false, "honor.ui"),
                "honor.ui");
            SysUi.Note(body, "荣誉经验 " + app.Profile.HonorSystemExp + "  Lv" + app.Profile.HonorSystemLevel);
            if (app.Database == null) return;
            foreach (TotemHonorEntry entry in app.Database.TotemHonorEntries.Values)
            {
                int id = entry.Id;
                SysUi.Row(body, "hd" + id, "#" + id + "  " + entry.NeedMoney + "金 → +" + entry.AddHonor,
                    () => PhoneNet.HonorSystemAction("donate", id));
            }
            SysUi.Row(body, "like", "点赞", () => PhoneNet.HonorSystemAction("like"));
            SysUi.Row(body, "fight", "战斗", () => PhoneNet.HonorSystemAction("fight"));
            for (int lv = 1; lv <= app.Profile.HonorSystemLevel && lv <= 20; lv++)
            {
                HonorSystemLevelInfo row = app.Database.GetHonorSystemLevel(lv);
                if (row == null || row.LevelGift <= 0) continue;
                bool claimed = app.Profile.HonorSystemClaimed != null && app.Profile.HonorSystemClaimed.Contains(lv);
                int claimLv = lv;
                if (!claimed)
                    SysUi.Row(body, "hc" + lv, "领取 Lv" + lv + "  #" + row.LevelGift, () => PhoneNet.HonorSystemClaim(claimLv));
            }
            SysUi.Note(body, "--- 光辉 GloryItemUpgradeList.xml ---");
            if (app.Database != null)
            {
                int shown = 0;
                foreach (GloryItemUpgrade row in app.Database.GloryUpgradeList)
                {
                    int cost = app.Database.GloryUpgradeGoldCost(row);
                    int tid = row.TemplateId;
                    ItemTemplate item = app.Database.GetItem(tid);
                    string name = item != null ? item.Name : ("#" + tid);
                    bool current = app.Profile.GloryTemplateId == tid || app.Profile.GloryTemplateId == row.NextTemplateId;
                    SysUi.Row(body, "glory" + tid, (current ? "[当前] " : "") + name + "  " + cost + "金",
                        () => PhoneNet.UpgradeGlory(tid));
                    if (++shown >= 8) break;
                }
            }
        }

        public static void GloryScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "光辉 · Glory");
            SysUi.Note(body, "GloryItemUpgradeList.xml  ·  NeedExp 金币  ·  CostItemID 背包优先");
            SysUi.Note(body, "当前模板 " + app.Profile.GloryTemplateId);
            if (app.Database == null) return;
            int shown = 0;
            foreach (GloryItemUpgrade row in app.Database.GloryUpgradeList)
            {
                int cost = app.Database.GloryUpgradeGoldCost(row);
                int tid = row.TemplateId;
                ItemTemplate item = app.Database.GetItem(tid);
                string name = item != null ? item.Name : ("#" + tid);
                string next = row.NextTemplateId > 0 ? (" → #" + row.NextTemplateId) : "";
                SysUi.Row(body, "gl" + tid, name + next + "  " + cost + "金  材料#" + row.CostItemId,
                    () => PhoneNet.UpgradeGlory(tid));
                if (++shown >= 16) break;
            }
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

        public static void BoguAdventureScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("boguadventure", "啵咕冒险", "Request/TS_ActivityConfig.xml", false, "boguadventure.ui"),
                "boguadventure.ui");
            ActivityConfigEntry activity = null;
            if (app.Database != null) app.Database.ActivityConfigs.TryGetValue(5, out activity);
            int maxActions = app.Database != null ? app.Database.ConfigInt("MineDayLimit", 5) * 4 : 20;
            SysUi.Note(body, "TS_ActivityConfig Num=5  ·  " + (activity != null ? activity.Name : "啵咕转盘") +
                "  ·  今日 " + app.Profile.BoguAdventureActions + " / " + maxActions);
            int spinCost = app.Database != null ? app.Database.BoguAdventureSpinCost(0) : 125;
            SysUi.Row(body, "spin", "转盘  " + spinCost + " 金", () => PhoneNet.BoguAdventureAction("spin", 5, 0));
            SysUi.Row(body, "sign", "签到 (免费)", () => PhoneNet.BoguAdventureAction("sign", 5));
            SysUi.Row(body, "find", "寻宝", () => PhoneNet.BoguAdventureAction("findMine", 5));
            SysUi.Row(body, "reset", "重置", () => PhoneNet.BoguAdventureAction("reset", 5));
            SysUi.Row(body, "award", "领奖", () => PhoneNet.BoguAdventureAction("getAward", 5));
            if (!string.IsNullOrEmpty(PhoneNet.LastBoguAdventureJson)) SysUi.Note(body, PhoneNet.LastBoguAdventureJson);
        }

        public static void ForcesBattleScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("forcesbattle", "势力战", "Request/cityoccupationsystems.xml", false, "forcesbattle.ui"),
                "forcesbattle.ui");
            if (app.Database == null) return;
            int maxAttempts = app.Database.ConfigInt("CityOccupationAddScoreCount", 30);
            SysUi.Note(body, "cityoccupationsystems.xml  ·  积分 " + app.Profile.ForcesBattleScore +
                "  ·  今日 " + app.Profile.ForcesBattleAttempts + " / " + maxAttempts);
            for (int quality = 1; quality <= 5; quality++)
            {
                int fee = app.Database.ForcesBattleEntryFee(quality);
                int score = app.Database.ForcesBattleScoreGain(quality);
                int localQuality = quality;
                SysUi.Row(body, "fb" + quality, "品质" + quality + "  入场" + fee + "金  +" + score + "分",
                    () => { PhoneNet.ForcesBattleStart(localQuality); app.ShowRoom(); });
            }
            SysUi.Note(body, "TS_Relic 圣物升级 (NeedExp 金币)");
            app.Profile.EnsureRelics();
            int shown = 0;
            foreach (RelicItemInfo item in app.Database.RelicItems.Values)
            {
                RelicSlot slot = app.Profile.FindRelic(item.RelicId);
                int level = slot != null ? slot.UpgradeLevel : 0;
                int cost = app.Database.RelicUpgradeGoldCost(item.RelicId, level);
                int relicId = item.RelicId;
                SysUi.Row(body, "relic" + relicId,
                    item.Name + " Q" + item.Quality + " Lv" + level + (cost > 0 ? "  " + cost + "金" : " MAX"),
                    cost > 0 ? (System.Action)(() => PhoneNet.UpgradeRelic(relicId)) : null);
                if (++shown >= 8) break;
            }
        }

        public static void CultureScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("culture", "文化淬炼", "Request/TS_UpgradeTemplate.xml", false, "culture.ui"),
                "culture.ui");
            if (app.Database == null) return;
            int gradeCost = app.Database.CultureGradeGoldCost(app.Profile.CultureGrade);
            SysUi.Note(body, "ExerciseInfoList 品阶 " + app.Profile.CultureGrade +
                (gradeCost > 0 ? "  ·  升阶 " + gradeCost + " 金" : "  ·  已满"));
            if (gradeCost > 0) SysUi.Row(body, "grade", "品阶升级 → " + (app.Profile.CultureGrade + 1), PhoneNet.CultureGradeUp);
            int[] statTypes = { 116, 117, 118, 119 };
            string[] statNames = { "攻击", "防御", "敏捷", "幸运" };
            for (int i = 0; i < statTypes.Length; i++)
            {
                int statType = statTypes[i];
                int level = app.Profile.GetCultureStatLevel(statType);
                int cost = app.Database.CultureUpgradeGoldCost(statType, level);
                CultureUpgradeRow row = app.Database.GetCultureUpgrade(statType, level);
                int bonus = row != null ? row.Data : 0;
                int localType = statType;
                SysUi.Row(body, "c" + statType,
                    statNames[i] + " Lv" + level + " +" + bonus + (cost > 0 ? "  " + cost + "金" : " MAX"),
                    cost > 0 ? (System.Action)(() => PhoneNet.CultureUpgrade(localType)) : null);
            }
        }
        }

        public static void LabyrinthGameScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("labyrinthgame", "温泉炸弹房", "Request/sparoomfixedbomb.xml", false, "labyrinthgame.ui"),
                "labyrinthgame.ui");
            int dayLimit = app.Database != null ? app.Database.SpaRoomDayScoreLimit() : 100;
            int gameLimit = app.Database != null ? app.Database.SpaRoomGameScoreLimit() : 200;
            SysUi.Note(body, "sparoomfixedbomb.xml / sparoomrandombomb.xml  ·  今日积分 " +
                app.Profile.SpaRoomDayScore + " / " + dayLimit + "  ·  单局上限 " + gameLimit);
            SysUi.Row(body, "start", "开始扫雷", PhoneNet.SpaRoomStart);
            if (!string.IsNullOrEmpty(PhoneNet.LastSpaRoomJson))
            {
                SysUi.Note(body, PhoneNet.LastSpaRoomJson);
            }

            int width = JsonFieldInt(PhoneNet.LastSpaRoomJson, "width", 0);
            int height = JsonFieldInt(PhoneNet.LastSpaRoomJson, "height", 0);
            if (width > 0 && height > 0)
            {
                int maxCells = Mathf.Min(width * height, 20);
                for (int i = 0; i < maxCells; i++)
                {
                    int idx = i;
                    SysUi.Row(body, "cell" + i, "翻开 #" + (i + 1), () => PhoneNet.SpaRoomBomb(idx));
                }
            }
        }

        public static void SpaRoomScreen(RectTransform safe, GameApp app)
        {
            LabyrinthGameScreen(safe, app);
        }

        public static void TreasureRoomScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("treasureroom", "藏宝室", "Request/CarnivalActivityItems.xml", false, "treasureroom.ui"),
                "treasureroom.ui");
            int unitCost = app.Database != null ? app.Database.TreasureRoomDrawCost(app.Profile.TreasureRoomDraws + 1) : 20;
            int freeCount = app.Database != null ? app.Database.ConfigInt("SearchGoodsFreeCount", 15) : 15;
            int freeLeft = Mathf.Max(0, freeCount - app.Profile.TreasureRoomDraws);
            SysUi.Note(body, "CarnivalActivityItems.xml / searchgoodspaymoney.xml  ·  今日已探 " +
                app.Profile.TreasureRoomDraws + "  ·  免费剩余 " + freeLeft + "  ·  " + unitCost + " 金/次");
            SysUi.Row(body, "draw1", "探宝 1 次", () => PhoneNet.TreasureRoomDraw(1));
            SysUi.Row(body, "draw10", "探宝 10 次  " + (unitCost * 10) + " 金", () => PhoneNet.TreasureRoomDraw(10));
            if (!string.IsNullOrEmpty(PhoneNet.LastTreasureRoomJson))
            {
                SysUi.Note(body, PhoneNet.LastTreasureRoomJson);
            }

            if (app.Database != null)
            {
                int shown = 0;
                foreach (CarnivalActivityItem item in app.Database.TreasureRoomPool())
                {
                    if (item.TemplateId <= 100)
                    {
                        continue;
                    }

                    SysUi.Note(body, "Q" + item.Quality + "  " + SysUi.ItemName(app, item.TemplateId) +
                        " x" + item.Count);
                    shown++;
                    if (shown >= 8)
                    {
                        break;
                    }
                }
            }
        }

        public static void ChristmasScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("christmas", "圣诞", "Request/activityhalloweenitems.xml", false, "christmas.ui"),
                "christmas.ui");
            int maxClaims = app.Database != null ? app.Database.ConfigInt("ChristmasPreDayCount", 10) : 10;
            SysUi.Note(body, "ChristmasGifts / activityhalloweenitems.xml  ·  今日 " +
                app.Profile.ChristmasClaims + " / " + maxClaims);
            SysUi.Row(body, "claim", "领取圣诞礼物", PhoneNet.ClaimChristmas);
            if (!string.IsNullOrEmpty(PhoneNet.LastChristmasJson))
            {
                SysUi.Note(body, PhoneNet.LastChristmasJson);
            }

            if (app.Database != null)
            {
                int shown = 0;
                foreach (ChristmasGiftTier tier in app.Database.ChristmasGifts)
                {
                    SysUi.Note(body, SysUi.ItemName(app, tier.ItemId) + "  雪花" + tier.SnowCost);
                    shown++;
                    if (shown >= 6)
                    {
                        break;
                    }
                }
            }
        }

        public static void NewYearScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("newyear", "新年", "Request/TS_NewYearPointReward.xml", false, "newyear.ui"),
                "newyear.ui");
            int freeMax = app.Database != null ? app.Database.ConfigInt("NewYearFreeCount", 3) : 3;
            int buyCost = app.Database != null ? app.Database.ConfigInt("NewYearBuyCost", 1000000) : 1000000;
            int gain = app.Database != null ? app.Database.ConfigInt("NewYearNeedPointLocal", 2000) : 2000;
            SysUi.Note(body, "积分 " + app.Profile.NewYearPoints + "  ·  免费 " +
                app.Profile.NewYearFreeUsed + " / " + freeMax + "  ·  付费 " + buyCost + " 金 +" + gain);
            SysUi.Row(body, "play", "新年活动 (+积分)", PhoneNet.NewYearPlay);
            if (!string.IsNullOrEmpty(PhoneNet.LastNewYearJson))
            {
                SysUi.Note(body, PhoneNet.LastNewYearJson);
            }

            if (app.Database == null)
            {
                return;
            }

            app.Profile.EnsureNewYearClaimed();
            foreach (NewYearPointReward row in app.Database.NewYearPointRewards)
            {
                bool claimed = app.Profile.NewYearPointClaimed.Contains(row.Id);
                string label = "里程碑 " + row.Points + " 分";
                if (claimed)
                {
                    label = "[已领] " + label;
                }
                else if (app.Profile.NewYearPoints < row.Points)
                {
                    label += "  (不足)";
                }

                int rewardId = row.Id;
                SysUi.Row(body, "ny" + row.Id, label,
                    claimed || app.Profile.NewYearPoints < row.Points
                        ? null
                        : (System.Action)(() => PhoneNet.NewYearClaimReward(rewardId)));
            }

            SysUi.Note(body, "--- 新年排行 TS_NewYearRankReward.xml ---");
            app.Profile.EnsureNewYearRankClaimed();
            foreach (NewYearRankReward row in app.Database.NewYearRankRewards)
            {
                bool claimed = app.Profile.NewYearRankClaimed.Contains(row.Id);
                string label = "排名 " + row.RankMin + "-" + row.RankMax + "  #" + row.RewardId;
                if (claimed)
                {
                    label = "[已领] " + label;
                }
                else if (app.Profile.NewYearPoints <= 0)
                {
                    label += "  (无积分)";
                }

                int rid = row.Id;
                SysUi.Row(body, "nyr" + row.Id, label,
                    claimed || app.Profile.NewYearPoints <= 0
                        ? null
                        : (System.Action)(() => PhoneNet.NewYearRankClaim(rid)));
            }
        }

        public static void WorshipMoonScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("worshipthemoon", "拜月", "Request/ServerConfig.xml", false, "worshipthemoon.ui"),
                "worshipthemoon.ui");
            (int batch, int cost) = app.Database != null ? app.Database.WorshipMoonPrice() : (3, 100);
            int maxDraws = app.Database != null ? app.Database.ConfigInt("SearchGoodsFreeLimit", 10) : 10;
            SysUi.Note(body, "WorshipMoonProb/Reward  ·  今日 " + app.Profile.WorshipMoonDraws +
                " / " + maxDraws + "  ·  " + cost + " 金 / " + batch + " 次");
            SysUi.Row(body, "w1", "拜月 " + batch + " 次  " + cost + " 金", () => PhoneNet.WorshipMoonClaim(1));
            SysUi.Row(body, "w3", "拜月 " + (batch * 3) + " 次  " + (cost * 3) + " 金", () => PhoneNet.WorshipMoonClaim(3));
            if (!string.IsNullOrEmpty(PhoneNet.LastWorshipMoonJson))
            {
                SysUi.Note(body, PhoneNet.LastWorshipMoonJson);
            }
        }


        public static void JigsawScreen(RectTransform safe, GameApp app)
        {
            ShowPcActivityScreen(safe, app, "jigsaw", "拼图", "jigsaw.ui", app.Profile.JigsawClaims,
                PhoneNet.LastJigsawJson, PhoneNet.JigsawAction);
        }

        public static void BibleScreen(RectTransform safe, GameApp app)
        {
            ShowPcActivityScreen(safe, app, "bible", "圣经", "bible.ui", app.Profile.BibleClaims,
                PhoneNet.LastBibleJson, PhoneNet.BibleAction);
        }

        static void ShowPcActivityScreen(RectTransform safe, GameApp app, string moduleId, string title,
            string uiFile, int claims, string lastJson, System.Action claimAction)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef(moduleId, title, null, false, uiFile), uiFile);
            PcActivityBinding binding = app.Database != null ? app.Database.ResolvePcActivity(moduleId) : null;
            int maxClaims = app.Database != null && binding != null
                ? app.Database.GetPcActivityDailyMax(binding)
                : 1;
            string source = binding != null ? binding.Source : "TS_ActivityConfig";
            string note = binding != null ? binding.Note : "no PC activity table";
            SysUi.Note(body, source + "  ·  " + note + "  ·  今日 " + claims + " / " + maxClaims);
            if (app.Database != null && binding != null)
            {
                var rewards = app.Database.GetPcActivityRewardRows(binding);
                int shown = 0;
                foreach (var pair in rewards)
                {
                    SysUi.Note(body, SysUi.ItemName(app, pair.templateId) + " x" + pair.count);
                    shown++;
                    if (shown >= 6)
                    {
                        break;
                    }
                }
            }

            if (claims < maxClaims)
            {
                SysUi.Row(body, "claim", "每日领取", claimAction);
            }

            if (!string.IsNullOrEmpty(lastJson))
            {
                SysUi.Note(body, lastJson);
            }
        }


        public static void CarnivalSuperLuckerScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("carnivalSuperLucker", "超级幸运", "Request/CarnivalActivityItems.xml", false, "carnivalSuperLucker.ui"),
                "carnivalSuperLucker.ui");
            int unitCost = app.Database != null ? app.Database.SuperLuckerDrawCost() : 500;
            SysUi.Note(body, "CarnivalActivityItems.xml  ·  今日 " + app.Profile.SuperLuckerDraws +
                " 次  ·  " + unitCost + " 金/次");
            SysUi.Row(body, "draw1", "超级幸运 1 次", () => PhoneNet.SuperLuckerDraw(1));
            SysUi.Row(body, "draw10", "超级幸运 10 次  " + (unitCost * 10) + " 金", () => PhoneNet.SuperLuckerDraw(10));
            if (!string.IsNullOrEmpty(PhoneNet.LastSuperLuckerJson))
            {
                SysUi.Note(body, PhoneNet.LastSuperLuckerJson);
            }

            if (app.Database != null)
            {
                int shown = 0;
                foreach (CarnivalActivityItem item in app.Database.SuperLuckerPool())
                {
                    if (item.TemplateId <= 100)
                    {
                        continue;
                    }

                    SysUi.Note(body, "Q" + item.Quality + "  " + SysUi.ItemName(app, item.TemplateId) +
                        " x" + item.Count);
                    shown++;
                    if (shown >= 8)
                    {
                        break;
                    }
                }
            }
        }

        public static void QuizScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "答题 · loadallquestions");
            int max = app.Database != null ? app.Database.DailyQuizMax() : 5;
            int gold = app.Database != null ? app.Database.QuizGoldReward() : 200;
            SysUi.Note(body, "今日 " + app.Profile.QuizAttempts + " / " + max + "  ·  每次 " + gold + " 金");
            if (app.Database == null || app.Database.QuizQuestionList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/loadallquestions.xml");
                return;
            }

            QuizQuestion q = app.Database.PickQuizQuestion(app.Profile.QuizAttempts);
            if (q == null)
            {
                return;
            }

            SysUi.Note(body, "#" + q.QuestionId + "  " + q.Content);
            if (app.Profile.QuizAttempts < max)
            {
                if (!string.IsNullOrEmpty(q.Option1))
                    SysUi.Row(body, "q1", "1. " + q.Option1, () => PhoneNet.QuizAnswer(q.QuestionId, 1));
                if (!string.IsNullOrEmpty(q.Option2))
                    SysUi.Row(body, "q2", "2. " + q.Option2, () => PhoneNet.QuizAnswer(q.QuestionId, 2));
                if (!string.IsNullOrEmpty(q.Option3))
                    SysUi.Row(body, "q3", "3. " + q.Option3, () => PhoneNet.QuizAnswer(q.QuestionId, 3));
                if (!string.IsNullOrEmpty(q.Option4))
                    SysUi.Row(body, "q4", "4. " + q.Option4, () => PhoneNet.QuizAnswer(q.QuestionId, 4));
            }
            else
            {
                SysUi.Note(body, "今日答题次数已满");
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastQuizJson))
            {
                SysUi.Note(body, PhoneNet.LastQuizJson);
            }
        }

        public static void OneYuanScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "一元购 · oneyuanbuyallgoodstemplate");
            app.Profile.EnsureOneYuanBought();
            SysUi.Note(body, "金 " + app.Profile.Gold + "  ·  礼券 " + app.Profile.Gift);
            if (app.Database == null || app.Database.OneYuanGoodsList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/oneyuanbuyallgoodstemplate.xml");
                return;
            }

            int shown = 0;
            foreach (OneYuanGoods goods in app.Database.OneYuanGoodsList)
            {
                int bought = 0;
                for (int i = 0; i < app.Profile.OneYuanBought.Count; i++)
                {
                    if (app.Profile.OneYuanBought[i] == goods.GoodsId)
                    {
                        bought++;
                    }
                }

                int limit = app.Database.OneYuanDailyLimit(goods);
                string cur = goods.IsBindMoney != 0 ? "礼券" : "金";
                string name = string.IsNullOrEmpty(goods.Name) ? ("#" + goods.GoodsId) : goods.Name;
                string cap = name + "  " + goods.Cost + cur + "  Goods " + goods.GoodsId;
                if (bought >= limit)
                {
                    cap = "[已购] " + cap;
                    SysUi.Note(body, cap);
                }
                else
                {
                    OneYuanGoods local = goods;
                    SysUi.Row(body, "oy" + goods.Id, cap, () => PhoneNet.OneYuanBuy(local.Id, local.GoodsId));
                }

                shown++;
                if (shown >= 40)
                {
                    SysUi.Note(body, "… " + (app.Database.OneYuanGoodsList.Count - shown) + " more");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastOneYuanJson))
            {
                SysUi.Note(body, PhoneNet.LastOneYuanJson);
            }
        }


        public static void PairUpScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "配对积分 · PairUpPointAward");
            app.Profile.EnsurePairUpClaimed();
            int free = app.Database != null ? app.Database.ConfigInt("PairUpDailyFreeCount", 5) : 5;
            int cost = app.Database != null ? app.Database.ConfigInt("PairUpCardCost", 150) : 150;
            int gain = app.Database != null ? app.Database.ConfigInt("PairUpOnePoint", 300) : 300;
            SysUi.Note(body, "积分 " + app.Profile.PairUpPoints + "  ·  今日 " + app.Profile.PairUpPlays + "/" + free +
                "  ·  金 " + app.Profile.Gold);
            SysUi.Row(body, "play", "配对一局  +" + gain + (app.Profile.PairUpPlays >= free ? ("  花费" + cost) : "  免费"),
                () => PhoneNet.PairUpClaim(0, "play"));
            if (app.Database == null || app.Database.PairUpAwardList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/PairUpPointAward.xml");
                return;
            }

            foreach (PairUpPointAward award in app.Database.PairUpAwardList)
            {
                bool claimed = app.Profile.PairUpClaimed.Contains(award.Id);
                bool ready = !claimed && app.Profile.PairUpPoints >= award.Point;
                string name = SysUi.ItemName(app, award.ItemId);
                string cap = (claimed ? "[已领] " : ready ? "[可领] " : "") +
                    "需" + award.Point + "分  " + name + " x" + award.Count;
                int rid = award.Id;
                SysUi.Row(body, "pu" + award.Id, cap,
                    ready ? (System.Action)(() => PhoneNet.PairUpClaim(rid, "claim")) : null);
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastPairUpJson))
            {
                SysUi.Note(body, PhoneNet.LastPairUpJson);
            }
        }

        public static void ShopShowScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "精选商城 · ShopGoodsShowList");
            SysUi.Note(body, "金 " + app.Profile.Gold + "  ·  礼券 " + app.Profile.Gift);
            if (app.Database == null || app.Database.ShopShowList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/ShopGoodsShowList.xml");
                return;
            }

            int shown = 0;
            var seen = new System.Collections.Generic.HashSet<int>();
            foreach (ShopShowEntry entry in app.Database.ShopShowList)
            {
                if (!seen.Add(entry.ShopId)) continue;
                ShopOffer offer = app.Database.GetShopShowOffer(entry.ShopId);
                if (offer == null || offer.TemplateId <= 0 || offer.AValue1 <= 0) continue;
                string name = SysUi.ItemName(app, offer.TemplateId);
                bool gift = offer.APrice1 != -1 && offer.APrice1 <= -2;
                string cur = gift ? "点券" : "金币";
                string cap = "T" + entry.Type + "  " + name + "  " + offer.AValue1 + cur;
                int sid = entry.ShopId;
                var btn = SysUi.Row(body, "ss" + entry.ShopId, cap, () => PhoneNet.ShopShowBuy(sid));
                ShopScreen.DecorateIcon(app, btn, offer.TemplateId);
                if (++shown >= 40)
                {
                    SysUi.Note(body, "… " + (app.Database.ShopShowList.Count - shown) + " more");
                    break;
                }
            }

            if (shown == 0)
            {
                SysUi.Note(body, "精选商品无法解析到 ShopItemList");
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastShopShowJson))
            {
                SysUi.Note(body, PhoneNet.LastShopShowJson);
            }
        }

        public static void JewelScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "首饰加工 · Jewel");
            SysUi.Note(body, "TS_Jewel_Addition / TS_Jewel_SkillName  ·  PhoneMsg 222");
            JewelAddition cur = app.Database != null ? app.Database.GetJewelAddition(app.Profile.JewelLevel) : null;
            JewelSkillName skill = app.Database != null ? app.Database.GetJewelSkill(app.Profile.JewelSkillType) : null;
            SysUi.Note(body, "等级 " + app.Profile.JewelLevel +
                (cur != null ? ("  ATK+" + cur.Attack + " DEF+" + cur.Defend + " AGI+" + cur.Agility + " LUK+" + cur.Luck) : "") +
                "  Exp " + app.Profile.JewelExp +
                (skill != null ? ("  技能 " + skill.Name) : "  未选技能"));
            if (app.Database == null || app.Database.JewelAdditionList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/TS_Jewel_Addition.xml");
                return;
            }

            int upCost = app.Database.JewelUpgradeGoldCost(app.Profile.JewelLevel);
            if (app.Database.GetJewelAddition(app.Profile.JewelLevel + 1) != null)
            {
                SysUi.Row(body, "jewelup", "升级加工  " + upCost + "金", () => PhoneNet.JewelEquip(0, "upgrade"));
            }
            if (app.Profile.JewelLevel > 0)
            {
                SysUi.Row(body, "jeweloff", "卸下加工", () => PhoneNet.JewelEquip(0, "equip"));
            }

            int shown = 0;
            foreach (JewelAddition row in app.Database.JewelAdditionList)
            {
                if (row.Level == 0) continue;
                if (row.Level % 5 != 0 && row.Level != 1 && row.Level != app.Profile.JewelLevel) continue;
                JewelAddition local = row;
                bool on = app.Profile.JewelLevel == row.Level;
                SysUi.Row(body, "jl" + row.Level,
                    (on ? "[装备] " : "") + row.Name + "  ATK" + row.Attack + " DEF" + row.Defend,
                    () => PhoneNet.JewelEquip(local.Level, "equip", app.Profile.JewelSkillType));
                if (++shown >= 16) break;
            }

            shown = 0;
            foreach (JewelSkillName sk in app.Database.JewelSkillList)
            {
                if (sk.Type == 3 || sk.Type == 30) continue;
                JewelSkillName local = sk;
                bool on = app.Profile.JewelSkillType == sk.Type;
                SysUi.Row(body, "js" + sk.Type,
                    (on ? "[技能] " : "") + sk.Name + "  T" + sk.Type,
                    () => PhoneNet.JewelEquip(app.Profile.JewelLevel, "skill", local.Type));
                if (++shown >= 12) break;
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastJewelJson))
            {
                SysUi.Note(body, PhoneNet.LastJewelJson);
            }
        }

        public static void WarPassScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "战令任务 · WarPass");
            app.Profile.EnsureWarPassClaimed();
            SysUi.Note(body, "TS_WarPass_QuestTemplate.xml  ·  PhoneMsg 223  ·  GP " + app.Profile.WarPassGp);
            if (app.Database == null || app.Database.WarPassQuestList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/TS_WarPass_QuestTemplate.xml");
                return;
            }

            int shown = 0;
            foreach (WarPassQuest q in app.Database.WarPassQuestList)
            {
                bool claimed = app.Profile.WarPassClaimed.Contains(q.Qid);
                bool done = app.Profile.WarPassCompleted.Contains(q.Qid);
                string cap = "Q" + q.Qid + " S" + q.SType + "  +" + q.AddGp + "GP  " +
                    (claimed ? "[已领] " : done ? "[可领] " : ("完成" + q.FinishPrice + "点券 ")) +
                    (q.Desc ?? "");
                int qid = q.Qid;
                if (claimed)
                {
                    SysUi.Note(body, cap);
                }
                else if (done || q.FinishPrice <= 0)
                {
                    SysUi.Row(body, "wpc" + q.Qid, cap, () => PhoneNet.WarPassClaim(qid, "claim"));
                }
                else
                {
                    SysUi.Row(body, "wpf" + q.Qid, cap, () => PhoneNet.WarPassClaim(qid, "complete"));
                }
                if (++shown >= 28) break;
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastWarPassJson))
            {
                SysUi.Note(body, PhoneNet.LastWarPassJson);
            }
        }

        public static void TimeLimitShopScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "限时商店 · TimeLimitShop");
            SysUi.Note(body, "金 " + app.Profile.Gold + "  ·  礼券 " + app.Profile.Gift +
                "  ·  荣誉 " + app.Profile.Honor + "  ·  PhoneMsg 224");
            if (app.Database == null || app.Database.TimeLimitShopList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/TS_TimeLimitShopTemp.xml");
                return;
            }

            int shown = 0;
            foreach (TimeLimitShopGoods g in app.Database.TimeLimitShopList)
            {
                if (g.ItemTempId <= 0) continue;
                string name = SysUi.ItemName(app, g.ItemTempId);
                bool gift = GameDatabase.TimeLimitShopIsGift(g);
                string cur = gift ? "点券" : "金币";
                string need = "";
                if (g.NeedGrade > 0) need += " Lv" + g.NeedGrade;
                if (g.NeedMedal > 0) need += " 勋章" + g.NeedMedal;
                string cap = "#" + g.ShopId + "  " + name + " x" + g.ItemCount + "  " + g.PayCounts + cur + need;
                int sid = g.ShopId;
                var btn = SysUi.Row(body, "tls" + g.ShopId, cap, () => PhoneNet.TimeLimitShopBuy(sid));
                ShopScreen.DecorateIcon(app, btn, g.ItemTempId);
                if (++shown >= 40)
                {
                    SysUi.Note(body, "… " + (app.Database.TimeLimitShopList.Count - shown) + " more");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastTimeLimitShopJson))
            {
                SysUi.Note(body, PhoneNet.LastTimeLimitShopJson);
            }
        }

        public static void BattleTeamScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "战队升级 · battleteamlevellist");
            int lv = app.Profile.BattleTeamLevel > 0 ? app.Profile.BattleTeamLevel : 1;
            SysUi.Note(body, "战队等级 " + lv + "  ·  金 " + app.Profile.Gold + "  ·  荣誉 " + app.Profile.Honor);
            if (app.Database == null || app.Database.BattleTeamLevelList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/battleteamlevellist.xml");
                return;
            }

            BattleTeamLevelInfo cur = app.Database.GetBattleTeamLevel(lv);
            if (cur != null)
            {
                SysUi.Note(body, "人数上限 " + cur.MaxPlayerNum + "  ·  Buff " + cur.BuffParam + "/" + cur.BuffTwoParam);
            }

            int next = lv + 1;
            BattleTeamLevelInfo nxt = app.Database.GetBattleTeamLevel(next);
            if (nxt == null)
            {
                SysUi.Note(body, "已达最高等级");
            }
            else
            {
                int cost = app.Database.BattleTeamUpgradeGold(next);
                SysUi.Row(body, "btup", "升级到 " + next + "  花费" + cost + "金  人数" + nxt.MaxPlayerNum,
                    () => PhoneNet.BattleTeamUpgrade());
            }

            if (app.Database.BattleTeamSegments.Count > 0)
            {
                SysUi.Note(body, "段位 " + app.Database.BattleTeamSegments.Count + "  ·  赛季 " +
                    app.Database.BattleTeamSeasons.Count + "  ·  活跃渠道 " +
                    app.Database.BattleTeamActiveTemplates.Count);
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastBattleTeamJson))
            {
                SysUi.Note(body, PhoneNet.LastBattleTeamJson);
            }
        }

        public static void BattleTeamShopScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "战队商店 · battleteamshopitemlist1");
            int lv = app.Profile.BattleTeamLevel > 0 ? app.Profile.BattleTeamLevel : 1;
            SysUi.Note(body, "战队Lv" + lv + "  ·  金 " + app.Profile.Gold + "  ·  荣誉 " + app.Profile.Honor);
            if (app.Database == null || app.Database.BattleTeamShopItemList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/battleteamshopitemlist1.xml");
                return;
            }

            int shown = 0;
            foreach (BattleTeamShopItem item in app.Database.BattleTeamShopItemList)
            {
                string name = SysUi.ItemName(app, item.TemplateId);
                string lockTxt = item.NeedLevel > lv ? (" 需战队" + item.NeedLevel) : "";
                string cond = item.Condition > 0 ? (" 需荣誉" + item.Value) : "";
                string cap = name + "  " + item.Price + "金" + lockTxt + cond;
                int sid = item.Id;
                bool can = item.NeedLevel <= lv;
                var btn = SysUi.Row(body, "bts" + item.Id, cap,
                    can ? (System.Action)(() => PhoneNet.BattleTeamShopBuy(sid)) : null);
                ShopScreen.DecorateIcon(app, btn, item.TemplateId);
                if (++shown >= 40)
                {
                    SysUi.Note(body, "… " + (app.Database.BattleTeamShopItemList.Count - shown) + " more");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastBattleTeamShopJson))
            {
                SysUi.Note(body, PhoneNet.LastBattleTeamShopJson);
            }
        }

        public static void DailyLeagueScreen(RectTransform safe, GameApp app)
        {
            Transform body = SysUi.Begin(safe, app, "每日联赛 · dailyleagueaward");
            app.Profile.EnsureDailyLeagueClaimed();
            int leagueLv = app.Profile.DailyLeagueLevel > 0 ? app.Profile.DailyLeagueLevel : 1;
            if (app.Database != null)
            {
                leagueLv = app.Database.ResolveDailyLeagueLevel(app.Profile.Level, leagueLv);
            }

            SysUi.Note(body, "联赛等级 " + leagueLv + "  ·  角色Lv" + app.Profile.Level + "  ·  金 " + app.Profile.Gold);
            if (app.Database == null || app.Database.DailyLeagueLevelList.Count == 0)
            {
                SysUi.Note(body, "缺少 Request/dailyleagueaward.xml");
                return;
            }

            foreach (DailyLeagueLevelInfo row in app.Database.DailyLeagueLevelList)
            {
                bool claimed = app.Profile.DailyLeagueClaimed.Contains(row.Level);
                bool ready = !claimed && row.Level <= leagueLv;
                var awards = app.Database.GetDailyLeagueAwardsForClass(row.Level);
                int n = awards != null ? awards.Count : 0;
                string name = string.IsNullOrEmpty(row.Name) ? ("段位" + row.Level) : row.Name;
                string cap = (claimed ? "[已领] " : ready ? "[可领] " : "[锁定] ") +
                    name + "  奖励" + n + "项";
                int lv = row.Level;
                SysUi.Row(body, "dl" + row.Level, cap,
                    ready ? (System.Action)(() => PhoneNet.DailyLeagueClaim(lv)) : null);
            }

            if (!string.IsNullOrEmpty(PhoneNet.LastDailyLeagueJson))
            {
                SysUi.Note(body, PhoneNet.LastDailyLeagueJson);
            }
        }


        static int JsonFieldInt(string json, string key, int fallback)
        {
            if (string.IsNullOrEmpty(json))
            {
                return fallback;
            }

            string needle = "\"" + key + "\":";
            int idx = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (idx < 0)
            {
                return fallback;
            }

            idx += needle.Length;
            int end = idx;
            while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '-'))
            {
                end++;
            }

            return int.TryParse(json.Substring(idx, end - idx), out int n) ? n : fallback;
        }

    }
}
