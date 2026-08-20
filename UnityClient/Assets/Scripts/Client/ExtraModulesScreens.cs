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
            ShowMornModule(safe, app,
                new ModuleDef("enchant", "附魔", "Request/magicfusiondata.xml"),
                "enchant.ui");
        }

        public static void TeamDungeonScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("teamdungeon", "团队副本", "Request/battleteamshopitemlist.xml"),
                "teamdungeon.ui");
            SysUi.Row(body, "fight", "开始团队战", app.ShowRoom);
        }

        public static void CarnivalScreen(RectTransform safe, GameApp app)
        {
            ShowMornModule(safe, app,
                new ModuleDef("carnival", "嘉年华", "Request/newlotteryitem.xml"),
                "carnival.ui");
        }

        public static void BankScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("bank", "银行", null),
                "bank.ui");
            SysUi.Note(body, "存款: " + app.Profile.Gold + " 金");
        }

        public static void MinesScreen(RectTransform safe, GameApp app)
        {
            ShowMornModule(safe, app,
                new ModuleDef("mines", "矿山", null),
                "mines.ui");
        }

        public static void AuditoriumScreen(RectTransform safe, GameApp app)
        {
            ShowMornModule(safe, app,
                new ModuleDef("auditorium", "礼堂", "Request/CelebByDayGPList.xml"),
                "auditorium.ui");
        }

        public static void TreasureScreen(RectTransform safe, GameApp app)
        {
            ShowMornModule(safe, app,
                new ModuleDef("treasure", "寻宝", "Request/newlotteryitem.xml"),
                "treasureHunting.ui");
        }

        public static void PeakBattleScreen(RectTransform safe, GameApp app)
        {
            ShowMornModule(safe, app,
                new ModuleDef("peakbattle", "巅峰战", "Request/areacelebbydayfightpowerlist.xml"),
                "peakBattle.ui");
        }
    }
}
