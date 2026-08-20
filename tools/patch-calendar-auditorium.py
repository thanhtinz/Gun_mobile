#!/usr/bin/env python3
"""One-shot patch for calendar/auditorium/bogu PhoneMsg 167/180/181."""
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

def patch(path, old, new, count=1):
    p = ROOT / path
    text = p.read_text()
    if old not in text:
        raise SystemExit(f"MISSING in {path}: {old[:80]!r}")
    p.write_text(text.replace(old, new, count))

# PhonePacket
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/PhonePacket.cs",
    "        public const ushort RedPacketSend = 179;\n        public const ushort RoomReady = 86;",
    "        public const ushort RedPacketSend = 179;\n        public const ushort CalendarClaim = 180;\n        public const ushort AuditoriumAction = 181;\n        public const ushort RoomReady = 86;",
)

# GameDatabase classes
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        public string RankAreaAward = \"\";\n    }\n\n    public sealed class FirstPayShopItem",
    "        public string RankAreaAward = \"\";\n    }\n\n    public sealed class GmActivityInfo\n    {\n        public string ActivityId = \"\";\n        public string ActivityName = \"\";\n        public int ActivityType;\n        public string Desc = \"\";\n    }\n\n    public sealed class FireworkEntry\n    {\n        public int TemplateId;\n        public int GoldCost;\n        public int HonorGain;\n    }\n\n    public sealed class FirstPayShopItem",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        public Dictionary<int, ActivityConfigEntry> ActivityConfigs { get; } = new Dictionary<int, ActivityConfigEntry>();\n        public List<FirstPayShopItem> FirstPayShop",
    "        public Dictionary<int, ActivityConfigEntry> ActivityConfigs { get; } = new Dictionary<int, ActivityConfigEntry>();\n        public List<GmActivityInfo> GmActivities { get; } = new List<GmActivityInfo>();\n        public List<FireworkEntry> Fireworks { get; } = new List<FireworkEntry>();\n        public List<FirstPayShopItem> FirstPayShop",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "            db.LoadActivityConfig(loader);\n            db.LoadFirstPayShop(loader);",
    "            db.LoadActivityConfig(loader);\n            db.LoadGmActivityInfo(loader);\n            db.LoadFirstPayShop(loader);",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "            db.LoadServerConfig(loader);\n            db.BuildSeasonalConfig();",
    "            db.LoadServerConfig(loader);\n            db.LoadFireworksFromConfig();\n            db.BuildSeasonalConfig();",
)

HELPERS = '''
        void LoadGmActivityInfo(ResLoader loader)
        {
            if (!loader.TryReadBytes("Request/gmactivityinfo.xml", out byte[] bytes)) return;
            try
            {
                XDocument doc = ZlibXml.Load(bytes);
                foreach (XElement info in doc.Descendants("ActiveInfo"))
                {
                    XElement act = info.Element("Activity");
                    if (act == null) continue;
                    GmActivities.Add(new GmActivityInfo
                    {
                        ActivityId = (string)act.Attribute("activityId") ?? "",
                        ActivityName = (string)act.Attribute("activityName") ?? "",
                        ActivityType = ParseIntAttr(act, "activityType"),
                        Desc = (string)act.Attribute("desc") ?? ""
                    });
                }
            }
            catch (Exception e) { Debug.LogWarning("GameDatabase gmactivityinfo: " + e.Message); }
        }

        public void LoadFireworksFromConfig()
        {
            Fireworks.Clear();
            if (!ServerConfig.TryGetValue("FireWorksList", out string raw) || string.IsNullOrEmpty(raw)) return;
            foreach (string row in raw.Split('|'))
            {
                string[] parts = row.Split(',');
                if (parts.Length < 2) continue;
                if (!int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int templateId)) continue;
                if (!int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int cost)) continue;
                Fireworks.Add(new FireworkEntry { TemplateId = templateId, GoldCost = cost, HonorGain = Mathf.Max(1, cost / 100) });
            }
        }

        static int ParseIntAttr(XElement el, string name)
        {
            XAttribute attr = el.Attribute(name);
            return attr != null && int.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : 0;
        }

        public SignReward GetCalendarDayReward(int dayIndex)
        {
            if (dayIndex <= 0 || SignIn.Count == 0) return null;
            for (int i = 0; i < SignIn.Count; i++) if (SignIn[i].Day == dayIndex) return SignIn[i];
            return SignIn[Mathf.Clamp(dayIndex - 1, 0, SignIn.Count - 1)];
        }

        public int AuditoriumWeddingCost(int tier = 0)
        {
            if (!ServerConfig.TryGetValue("MarryRoomCreateMoney", out string raw) || string.IsNullOrEmpty(raw)) return 150000;
            string[] parts = raw.Split(',');
            tier = Mathf.Clamp(tier, 0, parts.Length - 1);
            return int.TryParse(parts[tier].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : 150000;
        }

        public FireworkEntry GetFireworkEntry(int index)
        {
            if (Fireworks.Count == 0) LoadFireworksFromConfig();
            return index < 0 || index >= Fireworks.Count ? null : Fireworks[index];
        }

        public int BoguAdventureSpinCost(int tier = 0)
        {
            if (!ActivityConfigs.TryGetValue(5, out ActivityConfigEntry entry) || entry == null) return 125;
            string[] tiers = entry.Params1.Split('|');
            tier = Mathf.Clamp(tier, 0, tiers.Length - 1);
            string[] pair = tiers[tier].Split(',');
            return pair.Length > 1 && int.TryParse(pair[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int cost) ? cost : 125;
        }

        public int BoguAdventureRewardItemId()
        {
            if (!ActivityConfigs.TryGetValue(5, out ActivityConfigEntry entry) || entry == null) return 1125032;
            string[] rankParts = entry.RankAreaAward.Split('|');
            if (rankParts.Length == 0) return 1125032;
            string[] pair = rankParts[0].Split(',');
            return pair.Length > 1 && int.TryParse(pair[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int itemId) ? itemId : 1125032;
        }

        void LoadFirstPayShop(ResLoader loader)'''

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        void LoadFirstPayShop(ResLoader loader)",
    HELPERS,
)

print("GameDatabase patched")
