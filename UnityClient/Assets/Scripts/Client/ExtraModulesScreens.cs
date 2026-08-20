using GunMobile.Core;
using GunMobile.Net;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Client
{
    public static class ExtraModulesScreens
    {
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
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("magicstone", "魔石", "Request/magicstonetemplate.xml"),
                "magicStone.ui");
            SysUi.Row(body, "up", "魔石强化 +1  500 金", () => PhoneNet.Road?.Send(PhoneMsg.GemUpgrade, "{}"));
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
