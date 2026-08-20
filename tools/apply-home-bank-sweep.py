#!/usr/bin/env python3
"""Apply home/bank/sweep PC XML feature (PhoneMsg 170-173) on clean origin/main."""
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

def patch(path: str, old: str, new: str, count: int = 1):
    p = ROOT / path
    t = p.read_text()
    if old not in t:
        if new.split("\n", 1)[0].strip() in t and "HomeTemplePractice" in new:
            print(f"SKIP (already patched): {path}")
            return
        raise SystemExit(f"Pattern not found in {path}:\n{old[:120]}...")
    p.write_text(t.replace(old, new, count))
    print(f"Patched {path}")

# --- PhonePacket ---
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/PhonePacket.cs",
    "        public const ushort SuperLuckerDraw = 169;\n        public const ushort RoomReady = 86;",
    """        public const ushort SuperLuckerDraw = 169;
        public const ushort HomeTemplePractice = 170;
        public const ushort HomeTempleAdvance = 171;
        public const ushort BankDeposit = 172;
        public const ushort SweepMission = 173;
        public const ushort RoomReady = 86;""",
)

# --- PlayerExtras ---
p = ROOT / "UnityClient/Packages/com.gunmobile.port/Runtime/Net/PlayerExtras.cs"
t = p.read_text()
if "BankTermDeposit" not in t:
    patch(
        "UnityClient/Packages/com.gunmobile.port/Runtime/Net/PlayerExtras.cs",
        "    [System.Serializable]\n    public sealed class AuctionListing",
        """    [System.Serializable]
    public sealed class BankTermDeposit { public int TemplateId; public int Amount; public int DepositDay; }

    [System.Serializable]
    public sealed class AuctionListing""",
    )

# --- GameDatabase types ---
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        public int Guardian;\n    }\n\n    public sealed class DevilTreasItem",
    """        public int Guardian;
    }

    public sealed class HomeTemplePracticeLevel
    {
        public int Level, Exp, Attack, Defence, Guard, Luck, Blood, MagicDefence;
    }

    public sealed class HomeTempleAdvanceLevel
    {
        public int Level, Count1, Count2, Blood, MagicDefend, Toughness, AvoidInjury, TricRevolt, Guardian;
        public string Name = "";
    }

    public sealed class BankTemplate
    {
        public int Id, InterestRate, MinAmount, Multiple, Consume, DeadLine;
        public string Name = "";
    }

    public sealed class SweepMissionInfo
    {
        public int MissionId, CostCount, CostEnergy, LvMin, LvMax, MapId, DropId;
        public string Name = "";
        public int[] ConditionIds = System.Array.Empty<int>();
    }

    public sealed class SweepConditionInfo
    {
        public int Id, Type, Condition1, Condition2;
    }

    public sealed class DevilTreasItem""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        public Dictionary<int, NecklaceCastingLevel> NecklaceLevels { get; } = new Dictionary<int, NecklaceCastingLevel>();\n        public List<DevilTreasItem> DevilTreasItems { get; } = new List<DevilTreasItem>();",
    """        public Dictionary<int, NecklaceCastingLevel> NecklaceLevels { get; } = new Dictionary<int, NecklaceCastingLevel>();
        public Dictionary<int, HomeTemplePracticeLevel> HomeTemplePracticeLevels { get; } = new Dictionary<int, HomeTemplePracticeLevel>();
        public Dictionary<int, HomeTempleAdvanceLevel> HomeTempleAdvanceLevels { get; } = new Dictionary<int, HomeTempleAdvanceLevel>();
        public Dictionary<int, BankTemplate> BankTemplates { get; } = new Dictionary<int, BankTemplate>();
        public List<SweepMissionInfo> SweepMissions { get; } = new List<SweepMissionInfo>();
        public Dictionary<int, SweepConditionInfo> SweepConditions { get; } = new Dictionary<int, SweepConditionInfo>();
        public List<DevilTreasItem> DevilTreasItems { get; } = new List<DevilTreasItem>();""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "            db.LoadNecklace(loader);\n            db.LoadEmblems(loader);",
    """            db.LoadNecklace(loader);
            db.LoadHomeTemple(loader);
            db.LoadBankTemplates(loader);
            db.LoadSweepMissions(loader);
            db.LoadEmblems(loader);""",
)

HELPERS = '''
        public HomeTemplePracticeLevel GetHomeTemplePracticeLevel(int level) { HomeTemplePracticeLevels.TryGetValue(level, out HomeTemplePracticeLevel row); return row; }
        public int HomeTemplePracticeMaxLevel() { int max = 0; foreach (var kv in HomeTemplePracticeLevels) if (kv.Key > max) max = kv.Key; return max; }
        public int HomeTemplePracticeCost(int currentLevel) { var next = GetHomeTemplePracticeLevel(currentLevel + 1); return next != null ? next.Exp : 0; }
        public void ApplyHomeTemplePracticeBonus(int level, ref int atk, ref int def, ref int agi, ref int luck, ref int hp, ref int magicDef)
        {
            var row = GetHomeTemplePracticeLevel(level); if (row == null) return;
            atk += row.Attack; def += row.Defence; agi += row.Guard; luck += row.Luck; hp += row.Blood; magicDef += row.MagicDefence;
        }
        public HomeTempleAdvanceLevel GetHomeTempleAdvanceLevel(int level) { HomeTempleAdvanceLevels.TryGetValue(level, out HomeTempleAdvanceLevel row); return row; }
        public int HomeTempleAdvanceMaxLevel() { int max = 0; foreach (var kv in HomeTempleAdvanceLevels) if (kv.Key > max) max = kv.Key; return max; }
        public int HomeTempleAdvanceCost(int currentLevel) { var next = GetHomeTempleAdvanceLevel(currentLevel + 1); return next == null ? 0 : next.Count1 * 100 + next.Count2 * 150; }
        public void ApplyHomeTempleAdvanceBonus(int level, ref int hp, ref int magicDef, ref int def)
        {
            var row = GetHomeTempleAdvanceLevel(level); if (row == null) return;
            hp += row.Blood; magicDef += row.MagicDefend; def += row.Toughness / 10 + row.Guardian / 10 + row.AvoidInjury / 10;
        }
        public BankTemplate GetBankTemplate(int id) { BankTemplates.TryGetValue(id, out BankTemplate row); return row; }
        public bool BankDepositMature(int daysHeld, BankTemplate tpl) => tpl != null && (tpl.DeadLine <= 0 || daysHeld >= tpl.DeadLine * 30);
        public int BankDepositInterest(int amount, BankTemplate tpl, int daysHeld)
        {
            if (tpl == null || amount <= 0 || tpl.InterestRate <= 0) return 0;
            if (tpl.DeadLine <= 0) return amount * tpl.InterestRate * daysHeld / (1000 * 365);
            return BankDepositMature(daysHeld, tpl) ? amount * tpl.InterestRate / 1000 : 0;
        }
        public SweepMissionInfo GetSweepMission(int missionId) { for (int i = 0; i < SweepMissions.Count; i++) if (SweepMissions[i].MissionId == missionId) return SweepMissions[i]; return null; }
        static bool ListContainsInt(System.Collections.Generic.IReadOnlyList<int> list, int value) { if (list == null) return false; for (int i = 0; i < list.Count; i++) if (list[i] == value) return true; return false; }
        public bool CanSweepMission(int playerLevel, int labyrinthFloor, System.Collections.Generic.IReadOnlyList<int> clears, SweepMissionInfo mission)
        {
            if (mission == null || playerLevel < mission.LvMin || (mission.LvMax > 0 && playerLevel > mission.LvMax)) return false;
            int idx = -1; for (int i = 0; i < SweepMissions.Count; i++) if (SweepMissions[i].MissionId == mission.MissionId) { idx = i; break; }
            if (idx > 0 && !ListContainsInt(clears, SweepMissions[idx - 1].MissionId)) return false;
            if (SweepConditions.TryGetValue(mission.MissionId, out SweepConditionInfo cond))
            {
                if (cond.Condition1 > 0 && labyrinthFloor < cond.Condition1) return false;
                if (cond.Condition2 > 0 && (clears == null || clears.Count < cond.Condition2)) return false;
            }
            return true;
        }
        public int SweepMissionGoldReward(SweepMissionInfo mission)
        {
            if (mission == null) return 0;
            int gold = ComputePveWinGold(mission.MapId, 0, false);
            if (gold <= 0) gold = ComputePveWinGold(0, System.Math.Max(1, mission.LvMin / 10), true);
            return gold > 0 ? gold : 50 + mission.CostEnergy * 10;
        }
        static int[] ParseIdList(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return System.Array.Empty<int>();
            string[] parts = raw.Split(','); var list = new System.Collections.Generic.List<int>();
            for (int i = 0; i < parts.Length; i++) if (int.TryParse(parts[i].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int n)) list.Add(n);
            return list.ToArray();
        }
        static void ParseLvLimit(string raw, out int min, out int max)
        {
            min = 0; max = 999; if (string.IsNullOrEmpty(raw)) return;
            string[] parts = raw.Split(',');
            if (parts.Length > 0) int.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out min);
            if (parts.Length > 1) int.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out max);
        }
'''

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        public void ApplyHomeTempleBonus(int level, ref int atk, ref int hp)\n        {\n            hp += level * 120;\n            atk += level * 15;\n        }\n\n        static long SoulStampProKey",
    "        public void ApplyHomeTempleBonus(int level, ref int atk, ref int hp)\n        {\n            hp += level * 120;\n            atk += level * 15;\n        }\n" + HELPERS + "\n        static long SoulStampProKey",
)

LOADERS = '''
        void LoadHomeTemple(ResLoader loader)
        {
            if (TryTable(loader, "Request/HomeTempPracticeList.xml", out XmlResultTable practice))
                foreach (var row in practice.Rows)
                {
                    int level = Int(row, "Level");
                    HomeTemplePracticeLevels[level] = new HomeTemplePracticeLevel
                    {
                        Level = level, Exp = Int(row, "Exp"), Attack = Int(row, "Attack"), Defence = Int(row, "Defence"),
                        Guard = Int(row, "Guard"), Luck = Int(row, "Luck"), Blood = Int(row, "Blood"), MagicDefence = Int(row, "MagicDefence")
                    };
                }
            if (TryTable(loader, "Request/TS_HomeTempAdvance_Template.xml", out XmlResultTable advance))
                foreach (var row in advance.Rows)
                {
                    int level = Int(row, "Level");
                    HomeTempleAdvanceLevels[level] = new HomeTempleAdvanceLevel
                    {
                        Level = level, Count1 = Int(row, "Count1"), Count2 = Int(row, "Count2"), Blood = Int(row, "Blood"),
                        MagicDefend = Int(row, "MagicDefend"), Toughness = Int(row, "Toughness"), AvoidInjury = Int(row, "AvoidInjury"),
                        TricRevolt = Int(row, "TricRevolt"), Guardian = Int(row, "Guardian"), Name = Str(row, "Name")
                    };
                }
        }

        void LoadBankTemplates(ResLoader loader)
        {
            if (!TryTable(loader, "Request/banktemplateinfo.xml", out XmlResultTable table)) return;
            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                BankTemplates[id] = new BankTemplate
                {
                    Id = id, Name = Str(row, "Name"), InterestRate = Int(row, "InterestRate"), MinAmount = Int(row, "MinAmount"),
                    Multiple = Int(row, "Multiple"), Consume = Int(row, "Consume"), DeadLine = Int(row, "DeadLine")
                };
            }
        }

        void LoadSweepMissions(ResLoader loader)
        {
            if (TryTable(loader, "Request/ts_sweepcondition.xml", out XmlResultTable conditions))
                foreach (var row in conditions.Rows)
                {
                    int id = Int(row, "ID");
                    SweepConditions[id] = new SweepConditionInfo { Id = id, Type = Int(row, "Type"), Condition1 = Int(row, "Condition1"), Condition2 = Int(row, "Condition2") };
                }
            if (!TryTable(loader, "Request/ts_sweepmisson.xml", out XmlResultTable missions)) return;
            foreach (var row in missions.Rows)
            {
                ParseLvLimit(Str(row, "LvLimit"), out int lvMin, out int lvMax);
                var mission = new SweepMissionInfo
                {
                    MissionId = Int(row, "MissionId"), Name = Str(row, "Name"), CostCount = Int(row, "CostCount"), CostEnergy = Int(row, "CostEnergy"),
                    ConditionIds = ParseIdList(Str(row, "ConditionIDs")), LvMin = lvMin, LvMax = lvMax, MapId = Int(row, "MapId"), DropId = Int(row, "DropId")
                };
                if (mission.MissionId <= 0) mission.MissionId = Int(row, "ID");
                SweepMissions.Add(mission);
            }
            SweepMissions.Sort((a, b) => a.MissionId.CompareTo(b.MissionId));
        }

'''

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "            }\n        }\n\n        void LoadEmblems(ResLoader loader)",
    "            }\n        }\n" + LOADERS + "\n        void LoadEmblems(ResLoader loader)",
)

# --- PhoneNet ---
patch(
    "UnityClient/Assets/Scripts/Client/PhoneNet.cs",
    "        public static void UpgradeHomeTemple()\n        {\n            Road?.Send(PhoneMsg.HomeTempleUpgrade, \"{}\");\n        }\n\n        public static void WardrobeEquip(int clothId)",
    """        public static void UpgradeHomeTemple() { Road?.Send(PhoneMsg.HomeTempleUpgrade, "{}"); }
        public static void HomeTemplePractice() { Road?.Send(PhoneMsg.HomeTemplePractice, "{}"); }
        public static void HomeTempleAdvance() { Road?.Send(PhoneMsg.HomeTempleAdvance, "{}"); }
        public static void BankDeposit(string action, int templateId, int amount, int slot = 0) { Road?.Send(PhoneMsg.BankDeposit, "{\\"action\\":\\"" + (action ?? "deposit") + "\\",\\"templateId\\":" + templateId + ",\\"amount\\":" + amount + ",\\"slot\\":" + slot + "}"); }
        public static void SweepMission(int missionId) { Road?.Send(PhoneMsg.SweepMission, "{\\"missionId\\":" + missionId + "}"); }

        public static void WardrobeEquip(int clothId)""",
)

# --- PlayerProfile ---
patch(
    "UnityClient/Assets/Scripts/Client/PlayerProfile.cs",
    "        public int HomeTempleLevel;\n        public int WardrobeClothId;",
    """        public int HomeTempleLevel;
        public int HomeTemplePracticeLevel;
        public int HomeTempleAdvanceLevel;
        public List<BankTermDeposit> BankDeposits = new List<BankTermDeposit>();
        public List<int> SweepMissionClears = new List<int>();
        public int WardrobeClothId;""",
)

patch(
    "UnityClient/Assets/Scripts/Client/PlayerProfile.cs",
    "        public void EnsureEmblems() { if (Emblems == null) Emblems = new List<EmblemSlot>(); }\n        public void EnsureSoulStamps()",
    """        public void EnsureEmblems() { if (Emblems == null) Emblems = new List<EmblemSlot>(); }
        public void EnsureBankDeposits() { if (BankDeposits == null) BankDeposits = new List<BankTermDeposit>(); }
        public void EnsureSweepMissionClears() { if (SweepMissionClears == null) SweepMissionClears = new List<int>(); }
        public void EnsureSoulStamps()""",
)

patch(
    "UnityClient/Assets/Scripts/Client/PlayerProfile.cs",
    "                db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);\n                EnsureEmblems();",
    """                db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);
                int htMagicDef = magicDef;
                db.ApplyHomeTemplePracticeBonus(HomeTemplePracticeLevel, ref atk, ref def, ref agi, ref luk, ref hp, ref htMagicDef);
                db.ApplyHomeTempleAdvanceBonus(HomeTempleAdvanceLevel, ref hp, ref htMagicDef, ref def);
                magicDef = htMagicDef;
                EnsureEmblems();""",
)

print("Core patches done — MobileGameServer, GameApp, ExtraModulesScreens patched separately")
