#!/bin/bash
set -euo pipefail
cd /workspace
git checkout cursor/home-bank-sweep-9c68
git reset --hard origin/main
python3 tools/apply-home-bank-sweep.py

# MobileGameServer + remaining patches via embedded python
python3 << 'PYEOF'
from pathlib import Path
ROOT = Path("/workspace")

def patch(path, old, new, count=1):
    p = ROOT / path
    t = p.read_text()
    if old not in t:
        raise SystemExit(f"MISSING in {path}: {old[:80]}...")
    p.write_text(t.replace(old, new, count))

HANDLERS = r'''
        void HandleHomeTemplePractice(ServerPlayer player, NetworkStream ns)
        {
            int maxLevel = _db != null ? _db.HomeTemplePracticeMaxLevel() : 0;
            if (maxLevel <= 0 || player.HomeTemplePracticeLevel >= maxLevel)
            {
                Send(ns, PhoneMsg.HomeTemplePractice, "{\"ok\":false,\"err\":\"max\"}");
                return;
            }
            int cost = _db != null ? _db.HomeTemplePracticeCost(player.HomeTemplePracticeLevel) : 0;
            if (cost <= 0 || player.Gold < cost)
            {
                Send(ns, PhoneMsg.HomeTemplePractice, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }
            player.Gold -= cost;
            player.HomeTemplePracticeLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.HomeTemplePractice, "{\"ok\":true,\"level\":" + player.HomeTemplePracticeLevel + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleHomeTempleAdvance(ServerPlayer player, NetworkStream ns)
        {
            int maxLevel = _db != null ? _db.HomeTempleAdvanceMaxLevel() : 0;
            if (maxLevel <= 0 || player.HomeTempleAdvanceLevel >= maxLevel)
            {
                Send(ns, PhoneMsg.HomeTempleAdvance, "{\"ok\":false,\"err\":\"max\"}");
                return;
            }
            int cost = _db != null ? _db.HomeTempleAdvanceCost(player.HomeTempleAdvanceLevel) : 0;
            if (cost <= 0 || player.Gold < cost)
            {
                Send(ns, PhoneMsg.HomeTempleAdvance, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }
            player.Gold -= cost;
            player.HomeTempleAdvanceLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.HomeTempleAdvance, "{\"ok\":true,\"level\":" + player.HomeTempleAdvanceLevel + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleBankDeposit(ServerPlayer player, NetworkStream ns, string json)
        {
            string action = JS(json, "action", "deposit");
            int templateId = JI(json, "templateId", 0);
            int amount = JI(json, "amount", 0);
            int slot = JI(json, "slot", 0);
            player.EnsureBankDeposits();
            if (action == "withdraw")
            {
                if (slot < 0 || slot >= player.BankDeposits.Count)
                {
                    Send(ns, PhoneMsg.BankDeposit, "{\"ok\":false,\"err\":\"slot\"}");
                    return;
                }
                BankTermDeposit dep = player.BankDeposits[slot];
                BankTemplate tpl = _db != null ? _db.GetBankTemplate(dep.TemplateId) : null;
                int today = DateTime.Now.DayOfYear;
                int daysHeld = today >= dep.DepositDay ? today - dep.DepositDay : today + (365 - dep.DepositDay);
                int interest = _db != null ? _db.BankDepositInterest(dep.Amount, tpl, daysHeld) : 0;
                bool mature = _db == null || tpl == null || _db.BankDepositMature(daysHeld, tpl);
                if (tpl != null && tpl.DeadLine > 0 && !mature)
                {
                    Send(ns, PhoneMsg.BankDeposit, "{\"ok\":false,\"err\":\"immature\"}");
                    return;
                }
                int payout = dep.Amount + interest;
                player.Gold += payout;
                player.BankDeposits.RemoveAt(slot);
                SavePlayer(player);
                Send(ns, PhoneMsg.BankDeposit, "{\"ok\":true,\"action\":\"withdraw\",\"payout\":" + payout + ",\"interest\":" + interest + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }
            BankTemplate tplDep = _db != null ? _db.GetBankTemplate(templateId) : null;
            if (tplDep == null || tplDep.DeadLine <= 0)
            {
                Send(ns, PhoneMsg.BankDeposit, "{\"ok\":false,\"err\":\"template\"}");
                return;
            }
            if (amount <= 0) amount = tplDep.MinAmount;
            if (amount < tplDep.MinAmount)
            {
                Send(ns, PhoneMsg.BankDeposit, "{\"ok\":false,\"err\":\"min\"}");
                return;
            }
            if (tplDep.Multiple > 0 && amount % tplDep.Multiple != 0)
            {
                Send(ns, PhoneMsg.BankDeposit, "{\"ok\":false,\"err\":\"multiple\"}");
                return;
            }
            if (player.Gold < amount)
            {
                Send(ns, PhoneMsg.BankDeposit, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }
            player.Gold -= amount;
            player.BankDeposits.Add(new BankTermDeposit { TemplateId = templateId, Amount = amount, DepositDay = DateTime.Now.DayOfYear });
            SavePlayer(player);
            Send(ns, PhoneMsg.BankDeposit, "{\"ok\":true,\"action\":\"deposit\",\"amount\":" + amount + ",\"templateId\":" + templateId + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleSweepMission(ServerPlayer player, NetworkStream ns, string json)
        {
            int missionId = JI(json, "missionId", 0);
            int today = DateTime.Now.DayOfYear;
            if (player.SweepDay != today) { player.SweepDay = today; player.SweepCount = 0; }
            int maxSweeps = _db != null ? _db.ConfigInt("LabyrinthSweepDayLimit", 3) : 3;
            if (player.SweepCount >= maxSweeps)
            {
                Send(ns, PhoneMsg.SweepMission, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }
            SweepMissionInfo mission = _db != null ? _db.GetSweepMission(missionId) : null;
            if (mission == null)
            {
                Send(ns, PhoneMsg.SweepMission, "{\"ok\":false,\"err\":\"mission\"}");
                return;
            }
            player.EnsureSweepMissionClears();
            int floor = Mathf.Max(1, player.LabyrinthFloor);
            if (_db != null && !_db.CanSweepMission(player.Level, floor, player.SweepMissionClears, mission))
            {
                Send(ns, PhoneMsg.SweepMission, "{\"ok\":false,\"err\":\"locked\"}");
                return;
            }
            int gold = _db != null ? _db.SweepMissionGoldReward(mission) : 50;
            player.SweepCount++;
            player.Gold += gold;
            player.AddGp(_db, Mathf.Max(10, mission.CostEnergy * 5));
            if (!player.SweepMissionClears.Contains(missionId)) player.SweepMissionClears.Add(missionId);
            SavePlayer(player);
            Send(ns, PhoneMsg.SweepMission, "{\"ok\":true,\"missionId\":" + missionId + ",\"gold\":" + gold + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

'''

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "        public int HomeTempleLevel;\n        public int WardrobeClothId;",
    """        public int HomeTempleLevel;
        public int HomeTemplePracticeLevel;
        public int HomeTempleAdvanceLevel;
        public List<BankTermDeposit> BankDeposits = new List<BankTermDeposit>();
        public List<int> SweepMissionClears = new List<int>();
        public int WardrobeClothId;""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "        public void EnsureFightSpirits()",
    """        public void EnsureBankDeposits() { if (BankDeposits == null) BankDeposits = new List<BankTermDeposit>(); }
        public void EnsureSweepMissionClears() { if (SweepMissionClears == null) SweepMissionClears = new List<int>(); }

        public void EnsureFightSpirits()""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);\n            EnsureEmblems();",
    """            db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);
            db.ApplyHomeTemplePracticeBonus(HomeTemplePracticeLevel, ref atk, ref def, ref agi, ref luck, ref hp, ref magicDef);
            db.ApplyHomeTempleAdvanceBonus(HomeTempleAdvanceLevel, ref hp, ref magicDef, ref def);
            EnsureEmblems();""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            J(sb, \"homeTempleLevel\", HomeTempleLevel); sb.Append(\",\");\n            J(sb, \"wardrobeClothId\", WardrobeClothId); sb.Append(\",\");",
    """            J(sb, "homeTempleLevel", HomeTempleLevel); sb.Append(",");
            J(sb, "homeTemplePracticeLevel", HomeTemplePracticeLevel); sb.Append(",");
            J(sb, "homeTempleAdvanceLevel", HomeTempleAdvanceLevel); sb.Append(",");
            EnsureBankDeposits();
            sb.Append("\\"bankDeposits\\":[");
            for (int i = 0; i < BankDeposits.Count; i++)
            {
                if (i > 0) sb.Append(",");
                BankTermDeposit dep = BankDeposits[i];
                sb.Append("{\\"templateId\\":").Append(dep.TemplateId).Append(",\\"amount\\":").Append(dep.Amount)
                    .Append(",\\"depositDay\\":").Append(dep.DepositDay).Append("}");
            }
            sb.Append("],");
            EnsureSweepMissionClears();
            sb.Append("\\"sweepMissionClears\\":[");
            for (int i = 0; i < SweepMissionClears.Count; i++) { if (i > 0) sb.Append(","); sb.Append(SweepMissionClears[i]); }
            sb.Append("],");
            J(sb, "wardrobeClothId", WardrobeClothId); sb.Append(",");""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    """                case PhoneMsg.SuperLuckerDraw:
                    HandleSuperLuckerDraw(player, ns, json);
                    break;

                case PhoneMsg.EmblemCraft:""",
    """                case PhoneMsg.SuperLuckerDraw:
                    HandleSuperLuckerDraw(player, ns, json);
                    break;

                case PhoneMsg.HomeTemplePractice:
                    HandleHomeTemplePractice(player, ns);
                    break;

                case PhoneMsg.HomeTempleAdvance:
                    HandleHomeTempleAdvance(player, ns);
                    break;

                case PhoneMsg.BankDeposit:
                    HandleBankDeposit(player, ns, json);
                    break;

                case PhoneMsg.SweepMission:
                    HandleSweepMission(player, ns, json);
                    break;

                case PhoneMsg.EmblemCraft:""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    """            Send(ns, PhoneMsg.HomeTempleUpgrade,
                "{\\"ok\\":true,\\"level\\":" + player.HomeTempleLevel + ",\\"cost\\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleMailSend(ServerPlayer player, NetworkStream ns, string json)""",
    """            Send(ns, PhoneMsg.HomeTempleUpgrade,
                "{\\"ok\\":true,\\"level\\":" + player.HomeTempleLevel + ",\\"cost\\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }
""" + HANDLERS + """
        void HandleMailSend(ServerPlayer player, NetworkStream ns, string json)""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "        class RelicSlotSave { public int relicId; public int upgradeLevel; }\n\n        [Serializable]\n        class ServerPlayerSave",
    """        class RelicSlotSave { public int relicId; public int upgradeLevel; }
        class BankTermDepositSave { public int templateId; public int amount; public int depositDay; }

        [Serializable]
        class ServerPlayerSave""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            public int NecklaceLevel, HomeTempleLevel;\n            public int WardrobeClothId, HonorSystemExp, HonorSystemLevel;",
    """            public int NecklaceLevel, HomeTempleLevel, HomeTemplePracticeLevel, HomeTempleAdvanceLevel;
            public List<BankTermDepositSave> BankDeposits = new List<BankTermDepositSave>();
            public List<int> SweepMissionClears = new List<int>();
            public int WardrobeClothId, HonorSystemExp, HonorSystemLevel;""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "                NecklaceLevel = p.NecklaceLevel, HomeTempleLevel = p.HomeTempleLevel,\n                WardrobeClothId = p.WardrobeClothId, HonorSystemExp = p.HonorSystemExp,",
    """                NecklaceLevel = p.NecklaceLevel, HomeTempleLevel = p.HomeTempleLevel,
                HomeTemplePracticeLevel = p.HomeTemplePracticeLevel, HomeTempleAdvanceLevel = p.HomeTempleAdvanceLevel,
                BankDeposits = new List<BankTermDepositSave>(),
                SweepMissionClears = p.SweepMissionClears ?? new List<int>(),
                WardrobeClothId = p.WardrobeClothId, HonorSystemExp = p.HonorSystemExp,""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            p.EnsureFightSpirits();\n            foreach (FightSpiritSlot fs in p.FightSpirits)",
    """            p.EnsureFightSpirits();
            p.EnsureBankDeposits();
            foreach (BankTermDeposit dep in p.BankDeposits)
                s.BankDeposits.Add(new BankTermDepositSave { templateId = dep.TemplateId, amount = dep.Amount, depositDay = dep.DepositDay });
            foreach (FightSpiritSlot fs in p.FightSpirits)""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "                NecklaceLevel = s.NecklaceLevel, HomeTempleLevel = s.HomeTempleLevel,\n                WardrobeClothId = s.WardrobeClothId, HonorSystemExp = s.HonorSystemExp,",
    """                NecklaceLevel = s.NecklaceLevel, HomeTempleLevel = s.HomeTempleLevel,
                HomeTemplePracticeLevel = s.HomeTemplePracticeLevel, HomeTempleAdvanceLevel = s.HomeTempleAdvanceLevel,
                SweepMissionClears = s.SweepMissionClears ?? new List<int>(),
                WardrobeClothId = s.WardrobeClothId, HonorSystemExp = s.HonorSystemExp,""")

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            p.EnsureFightSpirits();\n            p.EnsureMagicStones();",
    """            p.EnsureFightSpirits();
            p.EnsureBankDeposits();
            p.EnsureSweepMissionClears();
            if (s.BankDeposits != null)
                foreach (BankTermDepositSave dep in s.BankDeposits)
                    p.BankDeposits.Add(new BankTermDeposit { TemplateId = dep.templateId, Amount = dep.amount, DepositDay = dep.depositDay });
            p.EnsureMagicStones();""")

# GameApp
patch("UnityClient/Assets/Scripts/Client/GameApp.cs",
    "                    case PhoneMsg.SweepLabyrinth:\n                    case PhoneMsg.FirstRechargeClaim:",
    """                    case PhoneMsg.SweepLabyrinth:
                    case PhoneMsg.SweepMission:
                    case PhoneMsg.HomeTemplePractice:
                    case PhoneMsg.HomeTempleAdvance:
                    case PhoneMsg.BankDeposit:
                    case PhoneMsg.FirstRechargeClaim:""")

patch("UnityClient/Assets/Scripts/Client/GameApp.cs",
    "            Profile.HomeTempleLevel = JsonInt(json, \"homeTempleLevel\", Profile.HomeTempleLevel);\n            Profile.WardrobeClothId = JsonInt(json, \"wardrobeClothId\", Profile.WardrobeClothId);",
    """            Profile.HomeTempleLevel = JsonInt(json, "homeTempleLevel", Profile.HomeTempleLevel);
            Profile.HomeTemplePracticeLevel = JsonInt(json, "homeTemplePracticeLevel", Profile.HomeTemplePracticeLevel);
            Profile.HomeTempleAdvanceLevel = JsonInt(json, "homeTempleAdvanceLevel", Profile.HomeTempleAdvanceLevel);
            Profile.WardrobeClothId = JsonInt(json, "wardrobeClothId", Profile.WardrobeClothId);""")

PARSE = '''
            ParseBankDepositsFromServer(json);
            ParseSweepMissionClearsFromServer(json);
            Profile.Save();
        }

        void ParseBankDepositsFromServer(string json)
        {
            int idx = json.IndexOf("\\"bankDeposits\\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 15;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.EnsureBankDeposits();
            Profile.BankDeposits.Clear();
            string body = json.Substring(start, end - start + 1);
            int pos = 0;
            while (pos < body.Length)
            {
                int ob = body.IndexOf('{', pos);
                if (ob < 0) break;
                int cb = body.IndexOf('}', ob);
                if (cb < 0) break;
                string entry = body.Substring(ob, cb - ob + 1);
                Profile.BankDeposits.Add(new BankTermDeposit
                {
                    TemplateId = JsonInt(entry, "templateId", 0),
                    Amount = JsonInt(entry, "amount", 0),
                    DepositDay = JsonInt(entry, "depositDay", 0)
                });
                pos = cb + 1;
            }
        }

        void ParseSweepMissionClearsFromServer(string json)
        {
            int idx = json.IndexOf("\\"sweepMissionClears\\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 21;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.EnsureSweepMissionClears();
            Profile.SweepMissionClears.Clear();
            string chunk = json.Substring(start, end - start);
            if (string.IsNullOrWhiteSpace(chunk)) return;
            foreach (string part in chunk.Split(','))
                if (int.TryParse(part.Trim(), out int id) && id > 0) Profile.SweepMissionClears.Add(id);
        }

        void ParseNewYearClaimedFromServer(string json)
'''

patch("UnityClient/Assets/Scripts/Client/GameApp.cs",
    "            ParseNewYearClaimedFromServer(json);\n            Profile.Save();\n        }\n\n        void ParseNewYearClaimedFromServer(string json)",
    "            ParseNewYearClaimedFromServer(json);\n" + PARSE)

# ExtraModulesScreens
patch("UnityClient/Assets/Scripts/Client/ExtraModulesScreens.cs",
    """        public static void BankScreen(RectTransform safe, GameApp app)
        {
            Transform body = ShowMornModule(safe, app,
                new ModuleDef("bank", "银行", null, false, "bank.ui"),
                "bank.ui");
            SysUi.Note(body, "现金: " + app.Profile.Gold + "  ·  存款: " + app.Profile.BankGold);
            SysUi.Row(body, "dep", "存入 1000 金", () => PhoneNet.BankTrade("deposit", 1000));
            SysUi.Row(body, "dep5", "存入 5000 金", () => PhoneNet.BankTrade("deposit", 5000));
            SysUi.Row(body, "wd", "取出 1000 金", () => PhoneNet.BankTrade("withdraw", 1000));
            SysUi.Row(body, "wd5", "取出 5000 金", () => PhoneNet.BankTrade("withdraw", 5000));
        }""",
    open(ROOT / "tools/extra-bank-snippet.txt").read().strip())

patch("UnityClient/Assets/Scripts/Client/ExtraModulesScreens.cs",
    """        public static void HomeTempleScreen(RectTransform safe, GameApp app)
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
        }""",
    open(ROOT / "tools/extra-home-sweep-snippet.txt").read().strip())

print("All server/client patches applied")
PYEOF

echo "Building..."
cd Server/GunMobile.Standalone && dotnet build -c Release

git add -A
git commit -m "Enhance homeTemple, bank, sweep with PC XML (PhoneMsg 170-173)

Load HomeTempPracticeList, TS_HomeTempAdvance_Template, banktemplateinfo,
ts_sweepmisson, and ts_sweepcondition from Request/. Add practice/advance loops
with gold costs and RecalcStats bonuses, term deposit/withdraw with interest,
and mission-based sweep with XML conditions. Extend HomeTempleScreen, BankScreen,
and SweepScreen. PhoneMsg 170-173 only; 138-140 and 141-169 unchanged."

git push -f -u origin cursor/home-bank-sweep-9c68
git rev-parse HEAD
