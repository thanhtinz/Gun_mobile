#!/usr/bin/env python3
"""Apply devil-quest-polish feature on clean origin/main."""
from pathlib import Path

ROOT = Path("/workspace")

def patch(path, old, new, count=1):
    p = ROOT / path
    text = p.read_text(encoding="utf-8")
    if old not in text:
        raise SystemExit(f"MISSING in {path}: {old[:80]!r}...")
    if count == -1:
        text = text.replace(old, new)
    else:
        text = text.replace(old, new, count)
    p.write_text(text, encoding="utf-8")
    print(f"OK {path}")

# --- PhonePacket ---
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/PhonePacket.cs",
    "        public const ushort SuperLuckerDraw = 169;\n        public const ushort RoomReady = 86;",
    "        public const ushort SuperLuckerDraw = 169;\n        public const ushort DevilTreasPointClaim = 178;\n        public const ushort RedPacketSend = 179;\n        public const ushort RoomReady = 86;",
)

# --- GameDatabase: types ---
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    """    public sealed class QuestInfo
    {
        public int Id;
        public string Title = "";
        public string Detail = "";
        public int NeedMinLevel;
        public int NeedMaxLevel;
        public int RewardGold;
        public int RewardMoney;
        public int RewardGp;
        public int RewardOffer;
        public string PreQuestId = "";
        public bool CanRepeat;
    }""",
    """    public sealed class QuestCondition
    {
        public int Id;
        public int Type;
        public int Para1;
        public int Para2;
        public bool Optional;
        public string Title = "";
    }

    public sealed class QuestInfo
    {
        public int Id;
        public string Title = "";
        public string Detail = "";
        public int NeedMinLevel;
        public int NeedMaxLevel;
        public int RewardGold;
        public int RewardMoney;
        public int RewardGp;
        public int RewardOffer;
        public string PreQuestId = "";
        public bool CanRepeat;
        public int MapId;
        public List<QuestCondition> Conditions = new List<QuestCondition>();
    }""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    """    public sealed class DevilTreasItem
    {
        public int Id;
        public int Type;
        public int TemplateId;
        public int Value;
        public int Weight;
    }""",
    """    public sealed class DevilTreasItem
    {
        public int Id;
        public int Type;
        public int TemplateId;
        public int Value;
        public int Weight;
    }

    public sealed class DevilTreasPointReward
    {
        public int Id;
        public int Points;
        public int TemplateId;
    }""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "        public List<DevilTreasItem> DevilTreasItems { get; } = new List<DevilTreasItem>();",
    "        public List<DevilTreasItem> DevilTreasItems { get; } = new List<DevilTreasItem>();\n        public Dictionary<int, DevilTreasPointReward> DevilTreasPointRewards { get; } = new Dictionary<int, DevilTreasPointReward>();",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    "            db.LoadDevilTreas(loader);\n            db.LoadSpaRoom(loader);",
    "            db.LoadDevilTreas(loader);\n            db.LoadDevilTreasPoints(loader);\n            db.LoadSpaRoom(loader);",
)

# Replace LoadQuests
OLD_LOAD_QUESTS = """        void LoadQuests(ResLoader loader)
        {
            if (!TryTable(loader, "Request/QuestList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                if (id == 0)
                {
                    id = Int(row, "QuestID");
                }

                Quests.Add(new QuestInfo
                {
                    Id = id,
                    Title = Str(row, "Title"),
                    Detail = Str(row, "Detail"),
                    NeedMinLevel = Int(row, "NeedMinLevel"),
                    NeedMaxLevel = Int(row, "NeedMaxLevel"),
                    RewardGold = Int(row, "RewardGold"),
                    RewardMoney = Int(row, "RewardMoney"),
                    RewardGp = Int(row, "RewardGP"),
                    RewardOffer = Int(row, "RewardOffer"),
                    PreQuestId = Str(row, "PreQuestID"),
                    CanRepeat = Bool(row, "CanRepeat")
                });
            }
        }"""

NEW_LOAD_QUESTS = """        void LoadQuests(ResLoader loader)
        {
            if (!loader.TryReadBytes("Request/QuestList.xml", out byte[] data))
            {
                return;
            }

            XDocument doc = ZlibXml.Load(data);
            XElement root = doc.Root;
            if (root == null)
            {
                return;
            }

            foreach (XElement item in root.Elements())
            {
                if (!string.Equals(item.Name.LocalName, "Item", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int id = QuestAttrInt(item, "ID");
                if (id == 0)
                {
                    id = QuestAttrInt(item, "QuestID");
                }

                if (id == 0)
                {
                    continue;
                }

                var q = new QuestInfo
                {
                    Id = id,
                    Title = QuestAttrStr(item, "Title"),
                    Detail = QuestAttrStr(item, "Detail"),
                    NeedMinLevel = QuestAttrInt(item, "NeedMinLevel"),
                    NeedMaxLevel = QuestAttrInt(item, "NeedMaxLevel"),
                    RewardGold = QuestAttrInt(item, "RewardGold"),
                    RewardMoney = QuestAttrInt(item, "RewardMoney"),
                    RewardGp = QuestAttrInt(item, "RewardGP"),
                    RewardOffer = QuestAttrInt(item, "RewardOffer"),
                    PreQuestId = QuestAttrStr(item, "PreQuestID"),
                    CanRepeat = QuestAttrBool(item, "CanRepeat"),
                    MapId = QuestAttrInt(item, "MapID")
                };

                foreach (XElement cond in item.Elements())
                {
                    if (!string.Equals(cond.Name.LocalName, "Item_Condiction", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    q.Conditions.Add(new QuestCondition
                    {
                        Id = QuestAttrInt(cond, "CondictionID"),
                        Type = QuestAttrInt(cond, "CondictionType"),
                        Para1 = QuestAttrInt(cond, "Para1"),
                        Para2 = QuestAttrInt(cond, "Para2"),
                        Optional = QuestAttrBool(cond, "isOpitional"),
                        Title = QuestAttrStr(cond, "CondictionTitle")
                    });
                }

                Quests.Add(q);
            }
        }

        static int QuestAttrInt(XElement el, string name)
        {
            XAttribute a = el.Attribute(name);
            if (a == null)
            {
                return 0;
            }

            int.TryParse(a.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v);
            return v;
        }

        static string QuestAttrStr(XElement el, string name)
        {
            XAttribute a = el.Attribute(name);
            return a?.Value ?? "";
        }

        static bool QuestAttrBool(XElement el, string name)
        {
            XAttribute a = el.Attribute(name);
            if (a == null)
            {
                return false;
            }

            return string.Equals(a.Value, "true", StringComparison.OrdinalIgnoreCase) || a.Value == "1";
        }"""

patch("UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs", OLD_LOAD_QUESTS, NEW_LOAD_QUESTS)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    """        void LoadDevilTreas(ResLoader loader)
        {
            if (!TryTable(loader, "Request/DevilTreasItemList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                DevilTreasItems.Add(new DevilTreasItem
                {
                    Id = Int(row, "ID"),
                    Type = Int(row, "Type"),
                    TemplateId = Int(row, "TemplateID"),
                    Value = Int(row, "Value"),
                    Weight = Int(row, "Random")
                });
            }
        }


        void LoadSpaRoom(ResLoader loader)""",
    """        void LoadDevilTreas(ResLoader loader)
        {
            if (!TryTable(loader, "Request/DevilTreasItemList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                DevilTreasItems.Add(new DevilTreasItem
                {
                    Id = Int(row, "ID"),
                    Type = Int(row, "Type"),
                    TemplateId = Int(row, "TemplateID"),
                    Value = Int(row, "Value"),
                    Weight = Int(row, "Random")
                });
            }
        }

        void LoadDevilTreasPoints(ResLoader loader)
        {
            if (!TryTable(loader, "Request/DevilTreasPointsList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                DevilTreasPointRewards[id] = new DevilTreasPointReward
                {
                    Id = id,
                    Points = Int(row, "Points"),
                    TemplateId = Int(row, "TemplateID")
                };
            }
        }


        void LoadSpaRoom(ResLoader loader)""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Res/GameDatabase.cs",
    """        public GodCardPointRewardInfo GetGodCardPointReward(int rewardId)
        {
            GodCardPointRewards.TryGetValue(rewardId, out GodCardPointRewardInfo row);
            return row;
        }""",
    """        public GodCardPointRewardInfo GetGodCardPointReward(int rewardId)
        {
            GodCardPointRewards.TryGetValue(rewardId, out GodCardPointRewardInfo row);
            return row;
        }

        public QuestInfo GetQuest(int questId)
        {
            for (int i = 0; i < Quests.Count; i++)
            {
                if (Quests[i].Id == questId)
                {
                    return Quests[i];
                }
            }

            return null;
        }

        public DevilTreasPointReward GetDevilTreasPointReward(int rewardId)
        {
            DevilTreasPointRewards.TryGetValue(rewardId, out DevilTreasPointReward row);
            return row;
        }""",
)

print("GameDatabase patched")

# --- MobileGameServer: ServerPlayer fields ---
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "        public int DevilTurnDay = -1;\n        public int DevilTurnSpins;",
    "        public int DevilTurnDay = -1;\n        public int DevilTurnSpins;\n        public int DevilTurnPoints;\n        public List<int> DevilTreasPointClaimed = new List<int>();\n        public Dictionary<int, List<int>> QuestProgress = new Dictionary<int, List<int>>();",
)

# ToJson devil turn + quests
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            J(sb, \"devilTurnSpins\", DevilTurnSpins); sb.Append(\",\");\n            J(sb, \"spaRoomDayScore\", SpaRoomDayScore); sb.Append(\",\");",
    """            J(sb, "devilTurnSpins", DevilTurnSpins); sb.Append(",");
            J(sb, "devilTurnPoints", DevilTurnPoints); sb.Append(",");
            EnsureDevilTreasPointClaimed();
            sb.Append("\\"devilTreasPointClaimed\\":[");
            for (int i = 0; i < DevilTreasPointClaimed.Count; i++) { if (i > 0) sb.Append(","); sb.Append(DevilTreasPointClaimed[i]); }
            sb.Append("],");
            sb.Append("\\"acceptedQuests\\":[");
            for (int i = 0; i < AcceptedQuests.Count; i++) { if (i > 0) sb.Append(","); sb.Append(AcceptedQuests[i]); }
            sb.Append("],");
            sb.Append("\\"completedQuests\\":[");
            for (int i = 0; i < CompletedQuests.Count; i++) { if (i > 0) sb.Append(","); sb.Append(CompletedQuests[i]); }
            sb.Append("],");
            if (QuestProgress != null && QuestProgress.Count > 0)
            {
                sb.Append("\\"questProgress\\":{");
                bool qpFirst = true;
                foreach (KeyValuePair<int, List<int>> kv in QuestProgress)
                {
                    if (!qpFirst) sb.Append(",");
                    qpFirst = false;
                    sb.Append("\\"").Append(kv.Key).Append("\\":[");
                    for (int i = 0; i < kv.Value.Count; i++) { if (i > 0) sb.Append(","); sb.Append(kv.Value[i]); }
                    sb.Append("]");
                }
                sb.Append("},");
            }
            J(sb, "spaRoomDayScore", SpaRoomDayScore); sb.Append(",");""",
)

# CompleteAcceptedQuests + quest helpers
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    """        public void EnsureGodCardPointClaimed() { if (GodCardPointClaimed == null) GodCardPointClaimed = new List<int>(); }
        public GodCardSlot FindGodCardSlot(int id) { foreach (GodCardSlot slot in GodCards) if (slot.Id == id) return slot; return null; }

        public int CompleteAcceptedQuests(GameDatabase db)
        {
            int extra = 0;
            if (db == null || AcceptedQuests.Count == 0) return 0;

            var copy = new List<int>(AcceptedQuests);
            AcceptedQuests.Clear();
            foreach (int id in copy)
            {
                if (CompletedQuests.Contains(id)) continue;

                CompletedQuests.Add(id);
                QuestInfo q = null;
                for (int i = 0; i < db.Quests.Count; i++)
                {
                    if (db.Quests[i].Id == id)
                    {
                        q = db.Quests[i];
                        break;
                    }
                }

                if (q == null) continue;

                extra += q.RewardGold;
                Gold += q.RewardGold;
                Honor += q.RewardOffer;
                AddGp(db, q.RewardGp);
            }

            return extra;
        }""",
    """        public void EnsureGodCardPointClaimed() { if (GodCardPointClaimed == null) GodCardPointClaimed = new List<int>(); }
        public void EnsureDevilTreasPointClaimed() { if (DevilTreasPointClaimed == null) DevilTreasPointClaimed = new List<int>(); }
        public GodCardSlot FindGodCardSlot(int id) { foreach (GodCardSlot slot in GodCards) if (slot.Id == id) return slot; return null; }

        public void EnsureQuestProgress(int questId, int conditionCount)
        {
            if (QuestProgress == null) QuestProgress = new Dictionary<int, List<int>>();
            if (!QuestProgress.TryGetValue(questId, out List<int> prog) || prog == null) { prog = new List<int>(); QuestProgress[questId] = prog; }
            while (prog.Count < conditionCount) prog.Add(0);
        }

        public void UpdateQuestBattleProgress(GameDatabase db, bool win, int pvpKills, int pveNpcId, int mapId, bool pve)
        {
            if (db == null || AcceptedQuests.Count == 0) return;
            foreach (int questId in AcceptedQuests)
            {
                QuestInfo quest = db.GetQuest(questId);
                if (quest == null || quest.Conditions.Count == 0) continue;
                EnsureQuestProgress(questId, quest.Conditions.Count);
                List<int> prog = QuestProgress[questId];
                for (int ci = 0; ci < quest.Conditions.Count; ci++)
                {
                    QuestCondition cond = quest.Conditions[ci];
                    int need = Mathf.Max(1, cond.Para2);
                    int add = 0;
                    switch (cond.Type)
                    {
                        case 4: if (pvpKills > 0 && !pve) add = pvpKills; break;
                        case 5: add = 1; break;
                        case 6: if (win) add = 1; break;
                        case 8: if (win && pve && (pveNpcId == cond.Para1 || mapId == quest.MapId)) add = 1; break;
                        case 13: if (win && pve && pveNpcId == cond.Para1) add = 1; break;
                        case 21: if (win && (mapId == quest.MapId || cond.Para1 == mapId || cond.Para1 == pveNpcId)) add = 1; break;
                    }
                    if (add > 0) prog[ci] = Mathf.Min(need, prog[ci] + add);
                }
            }
        }

        public bool IsQuestReady(GameDatabase db, int questId)
        {
            if (db == null) return true;
            QuestInfo quest = db.GetQuest(questId);
            if (quest == null || quest.Conditions.Count == 0) return true;
            if (!QuestProgress.TryGetValue(questId, out List<int> prog) || prog == null) return false;
            for (int ci = 0; ci < quest.Conditions.Count; ci++)
            {
                if (quest.Conditions[ci].Optional) continue;
                if (ci >= prog.Count || prog[ci] < Mathf.Max(1, quest.Conditions[ci].Para2)) return false;
            }
            return true;
        }

        public int CompleteAcceptedQuests(GameDatabase db)
        {
            int extra = 0;
            if (db == null || AcceptedQuests.Count == 0) return 0;

            var ready = new List<int>();
            foreach (int id in AcceptedQuests)
            {
                if (IsQuestReady(db, id)) ready.Add(id);
            }
            foreach (int id in ready)
            {
                AcceptedQuests.Remove(id);
                if (CompletedQuests.Contains(id)) continue;
                CompletedQuests.Add(id);
                QuestProgress.Remove(id);
                QuestInfo q = db.GetQuest(id);
                if (q == null) continue;
                extra += q.RewardGold;
                Gold += q.RewardGold;
                Honor += q.RewardOffer;
                AddGp(db, q.RewardGp);
            }
            return extra;
        }""",
)

# Switch cases
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "                case PhoneMsg.RedPacketClaim:\n                    HandleRedPacketClaim(player, ns);\n                    break;",
    """                case PhoneMsg.RedPacketClaim:
                    HandleRedPacketClaim(player, ns);
                    break;

                case PhoneMsg.DevilTreasPointClaim:
                    HandleDevilTreasPointClaim(player, ns, json);
                    break;

                case PhoneMsg.RedPacketSend:
                    HandleRedPacketSend(player, ns, json);
                    break;""",
)

# HandleDevilTurnSpin points
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            player.DevilTurnSpins += count;\n            SavePlayer(player);",
    """            player.DevilTurnSpins += count;
            int pointsPerSpin = _db != null ? _db.ConfigInt("DevilTreasurePointPerSpin", 100) : 100;
            player.DevilTurnPoints += count * pointsPerSpin;
            SavePlayer(player);""",
)

# HandleDevilTreasPointClaim + HandleRedPacketSend after HandleRedPacketClaim
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    """            Send(ns, PhoneMsg.RedPacketClaim,
                "{\\"ok\\":true,\\"gold\\":" + gold + ",\\"claims\\":" + player.RedPacketClaims + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleHomeTempleUpgrade(ServerPlayer player, NetworkStream ns)""",
    """            Send(ns, PhoneMsg.RedPacketClaim,
                "{\\"ok\\":true,\\"gold\\":" + gold + ",\\"claims\\":" + player.RedPacketClaims + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleDevilTreasPointClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int rewardId = JI(json, "rewardId", 0);
            DevilTreasPointReward reward = _db != null ? _db.GetDevilTreasPointReward(rewardId) : null;
            if (reward == null || reward.TemplateId <= 0)
            {
                Send(ns, PhoneMsg.DevilTreasPointClaim, "{\\"ok\\":false,\\"err\\":\\"reward\\"}");
                return;
            }

            player.EnsureDevilTreasPointClaimed();
            if (player.DevilTreasPointClaimed.Contains(rewardId) || player.DevilTurnPoints < reward.Points)
            {
                Send(ns, PhoneMsg.DevilTreasPointClaim, "{\\"ok\\":false,\\"err\\":\\"points\\"}");
                return;
            }

            player.DevilTreasPointClaimed.Add(rewardId);
            player.GrantTemplateReward(_db, reward.TemplateId, 1);
            SavePlayer(player);
            Send(ns, PhoneMsg.DevilTreasPointClaim,
                "{\\"ok\\":true,\\"rewardId\\":" + rewardId + ",\\"profile\\":" + player.ToJson() + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleRedPacketSend(ServerPlayer player, NetworkStream ns, string json)
        {
            string friend = JS(json, "friend", "");
            int gold = Mathf.Clamp(JI(json, "gold", 0), 1, 50000);
            if (string.IsNullOrEmpty(friend) || !player.Friends.Contains(friend))
            {
                Send(ns, PhoneMsg.RedPacketSend, "{\\"ok\\":false,\\"err\\":\\"friend\\"}");
                return;
            }

            if (player.Gold < gold)
            {
                Send(ns, PhoneMsg.RedPacketSend, "{\\"ok\\":false,\\"err\\":\\"gold\\"}");
                return;
            }

            ServerPlayer target = null;
            lock (_lock)
            {
                foreach (ServerPlayer p in _players.Values)
                {
                    if (string.Equals(p.Nick, friend, StringComparison.OrdinalIgnoreCase))
                    {
                        target = p;
                        break;
                    }
                }
            }

            if (target == null || target.RoadStream == null)
            {
                Send(ns, PhoneMsg.RedPacketSend, "{\\"ok\\":false,\\"err\\":\\"offline\\"}");
                return;
            }

            player.Gold -= gold;
            target.Gold += gold;
            SavePlayer(player);
            SavePlayer(target);
            Send(ns, PhoneMsg.RedPacketSend,
                "{\\"ok\\":true,\\"gold\\":" + gold + ",\\"friend\\":\\"" + friend.Replace("\\"", "\\\\\\"") + "\\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
            SendTo(target, PhoneMsg.ProfileData, target.ToJson());
        }

        void HandleHomeTempleUpgrade(ServerPlayer player, NetworkStream ns)""",
)

# HandleQuest
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    """        void HandleQuest(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            int questId = JI(json, "questId", 0);
            if (id == PhoneMsg.QuestAccept)
            {
                if (!player.AcceptedQuests.Contains(questId))
                    player.AcceptedQuests.Add(questId);
            }
            else
            {
                player.AcceptedQuests.Remove(questId);
                if (!player.CompletedQuests.Contains(questId))
                    player.CompletedQuests.Add(questId);
                if (_db != null)
                {
                    foreach (var q in _db.Quests)
                    {
                        if (q.Id == questId)
                        {
                            player.Gold += q.RewardGold;
                            player.Honor += q.RewardOffer;
                            player.AddGp(_db, q.RewardGp);
                            break;
                        }
                    }
                }
            }
            SavePlayer(player);
            Send(ns, PhoneMsg.QuestResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }""",
    """        void HandleQuest(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            int questId = JI(json, "questId", 0);
            if (id == PhoneMsg.QuestAccept)
            {
                if (!player.AcceptedQuests.Contains(questId))
                {
                    player.AcceptedQuests.Add(questId);
                    if (_db != null)
                    {
                        QuestInfo q = _db.GetQuest(questId);
                        if (q != null && q.Conditions.Count > 0)
                        {
                            player.EnsureQuestProgress(questId, q.Conditions.Count);
                        }
                    }
                }
            }
            else
            {
                if (_db != null && !player.IsQuestReady(_db, questId))
                {
                    Send(ns, PhoneMsg.QuestResult, "{\\"ok\\":false,\\"err\\":\\"not ready\\"}");
                    return;
                }

                player.AcceptedQuests.Remove(questId);
                if (!player.CompletedQuests.Contains(questId))
                {
                    player.CompletedQuests.Add(questId);
                }

                player.QuestProgress.Remove(questId);
                if (_db != null)
                {
                    QuestInfo q = _db.GetQuest(questId);
                    if (q != null)
                    {
                        player.Gold += q.RewardGold;
                        player.Honor += q.RewardOffer;
                        player.AddGp(_db, q.RewardGp);
                    }
                }
            }
            SavePlayer(player);
            Send(ns, PhoneMsg.QuestResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }""",
)

# EndBattle quest progress
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    """                    p.PveNpcId = 0;
                    p.PveRewardGold = 0;
                    p.PveLabyrinth = false;
                    p.PveDreamland = false;
                    p.PveWarriorFam = false;

                    if (win)
                    {
                        p.Win++;
                        p.Gold += gold;
                        if (_db != null)
                        {
                            p.Honor += _db.BattleWinHonor(p.Level, pve);
                            int gpGain = pve && pveNpcId > 0 && _db.Npcs.TryGetValue(pveNpcId, out NpcInfo npcInfo)
                                ? Mathf.Max(1, npcInfo.Experience)
                                : _db.BattleWinGp(p.Level, pve);
                            p.AddGp(_db, gpGain);
                        }
                        questGold = p.CompleteAcceptedQuests(_db);
                    }""",
    """                    p.PveNpcId = 0;
                    p.PveRewardGold = 0;
                    p.PveLabyrinth = false;
                    p.PveDreamland = false;
                    p.PveWarriorFam = false;

                    int mapId = room.MapId;
                    int pvpKills = 0;
                    if (room.Hp != null && room.Livings != null)
                    {
                        for (int si = 0; si < room.Hp.Length; si++)
                        {
                            if (room.Hp[si] <= 0 && si < room.Livings.Length && room.Livings[si].Team != myTeam)
                            {
                                pvpKills++;
                            }
                        }
                    }

                    if (_db != null)
                    {
                        p.UpdateQuestBattleProgress(_db, win, pvpKills, pveNpcId, mapId, pve);
                    }

                    if (win)
                    {
                        p.Win++;
                        p.Gold += gold;
                        if (_db != null)
                        {
                            p.Honor += _db.BattleWinHonor(p.Level, pve);
                            int gpGain = pve && pveNpcId > 0 && _db.Npcs.TryGetValue(pveNpcId, out NpcInfo npcInfo)
                                ? Mathf.Max(1, npcInfo.Experience)
                                : _db.BattleWinGp(p.Level, pve);
                            p.AddGp(_db, gpGain);
                        }
                        questGold = p.CompleteAcceptedQuests(_db);
                    }""",
)

# Save/load
patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "        class RelicSlotSave { public int relicId; public int upgradeLevel; }\n\n        [Serializable]\n        class ServerPlayerSave",
    """        class RelicSlotSave { public int relicId; public int upgradeLevel; }
        class QuestProgressSave { public int questId; public List<int> progress = new List<int>(); }

        [Serializable]
        class ServerPlayerSave""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            public int RedPacketDay = -1, RedPacketClaims;\n            public int DevilTurnDay = -1, DevilTurnSpins;",
    "            public int RedPacketDay = -1, RedPacketClaims;\n            public int DevilTurnDay = -1, DevilTurnSpins, DevilTurnPoints;\n            public List<int> DevilTreasPointClaimed = new List<int>();\n            public List<QuestProgressSave> QuestProgress = new List<QuestProgressSave>();",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "                RedPacketDay = p.RedPacketDay, RedPacketClaims = p.RedPacketClaims,\n                DevilTurnDay = p.DevilTurnDay, DevilTurnSpins = p.DevilTurnSpins,",
    """                RedPacketDay = p.RedPacketDay, RedPacketClaims = p.RedPacketClaims,
                DevilTurnDay = p.DevilTurnDay, DevilTurnSpins = p.DevilTurnSpins, DevilTurnPoints = p.DevilTurnPoints,
                DevilTreasPointClaimed = p.DevilTreasPointClaimed ?? new List<int>(),
                QuestProgress = new List<QuestProgressSave>(),""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            foreach (KeyValuePair<int, int> kv in p.FirstRechargeShopBuys)\n            {\n                s.FirstRechargeShopBuys.Add(new FirstRechargeBuySave { templateId = kv.Key, count = kv.Value });\n            }",
    """            foreach (KeyValuePair<int, int> kv in p.FirstRechargeShopBuys)
            {
                s.FirstRechargeShopBuys.Add(new FirstRechargeBuySave { templateId = kv.Key, count = kv.Value });
            }
            if (p.QuestProgress != null)
            {
                foreach (KeyValuePair<int, List<int>> kv in p.QuestProgress)
                {
                    s.QuestProgress.Add(new QuestProgressSave { questId = kv.Key, progress = kv.Value ?? new List<int>() });
                }
            }""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "                RedPacketDay = s.RedPacketDay, RedPacketClaims = s.RedPacketClaims,\n                DevilTurnDay = s.DevilTurnDay, DevilTurnSpins = s.DevilTurnSpins,",
    """                RedPacketDay = s.RedPacketDay, RedPacketClaims = s.RedPacketClaims,
                DevilTurnDay = s.DevilTurnDay, DevilTurnSpins = s.DevilTurnSpins, DevilTurnPoints = s.DevilTurnPoints,
                DevilTreasPointClaimed = s.DevilTreasPointClaimed ?? new List<int>(),""",
)

patch(
    "UnityClient/Packages/com.gunmobile.port/Runtime/Net/MobileGameServer.cs",
    "            p.EnsureHonorSystemClaimed();\n            foreach (var b in s.Bag) p.Bag.Add(new BagSlot { TemplateId = b.t, Count = b.c, Strengthen = b.s });",
    """            p.EnsureHonorSystemClaimed();
            if (s.QuestProgress != null)
            {
                foreach (QuestProgressSave qp in s.QuestProgress)
                {
                    if (qp.questId > 0 && qp.progress != null)
                    {
                        p.QuestProgress[qp.questId] = new List<int>(qp.progress);
                    }
                }
            }
            foreach (var b in s.Bag) p.Bag.Add(new BagSlot { TemplateId = b.t, Count = b.c, Strengthen = b.s });""",
)

print("MobileGameServer patched")

# --- Client: PhoneNet ---
patch(
    "UnityClient/Assets/Scripts/Client/PhoneNet.cs",
    """        public static void DevilTurnSpin(int count = 1)
        {
            Road?.Send(PhoneMsg.DevilTurnSpin, "{\\"count\\":" + count + "}");
        }""",
    """        public static void DevilTurnSpin(int count = 1)
        {
            Road?.Send(PhoneMsg.DevilTurnSpin, "{\\"count\\":" + count + "}");
        }

        public static void ClaimDevilTreasPoint(int rewardId)
        {
            Road?.Send(PhoneMsg.DevilTreasPointClaim, "{\\"rewardId\\":" + rewardId + "}");
        }""",
)

patch(
    "UnityClient/Assets/Scripts/Client/PhoneNet.cs",
    """        public static void ClaimRedPacket()
        {
            Road?.Send(PhoneMsg.RedPacketClaim, "{}");
        }""",
    """        public static void ClaimRedPacket()
        {
            Road?.Send(PhoneMsg.RedPacketClaim, "{}");
        }

        public static void SendRedPacket(string friend, int gold)
        {
            string fn = (friend ?? "").Replace("\\\\", "\\\\\\\\").Replace("\\"", "\\\\\\"");
            Road?.Send(PhoneMsg.RedPacketSend, "{\\"friend\\":\\"" + fn + "\\",\\"gold\\":" + gold + "}");
        }""",
)

# --- PlayerProfile ---
patch(
    "UnityClient/Assets/Scripts/Client/PlayerProfile.cs",
    "        public int DevilTurnSpins;\n        public int SpaRoomDayScore;",
    "        public int DevilTurnSpins;\n        public int DevilTurnPoints;\n        public List<int> DevilTreasPointClaimed = new List<int>();\n        public Dictionary<int, List<int>> QuestProgress = new Dictionary<int, List<int>>();\n        public int SpaRoomDayScore;",
)

patch(
    "UnityClient/Assets/Scripts/Client/PlayerProfile.cs",
    "        public bool QuestAccepted(int id) => AcceptedQuests.Contains(id);\n",
    """        public bool QuestAccepted(int id) => AcceptedQuests.Contains(id);

        public List<int> GetQuestProgress(int questId)
        {
            if (QuestProgress != null && QuestProgress.TryGetValue(questId, out List<int> prog) && prog != null)
            {
                return prog;
            }

            return null;
        }

""",
)

# --- GameApp ---
patch(
    "UnityClient/Assets/Scripts/Client/GameApp.cs",
    "            Profile.DevilTurnSpins = JsonInt(json, \"devilTurnSpins\", Profile.DevilTurnSpins);\n            Profile.SpaRoomDayScore = JsonInt(json, \"spaRoomDayScore\", Profile.SpaRoomDayScore);",
    """            Profile.DevilTurnSpins = JsonInt(json, "devilTurnSpins", Profile.DevilTurnSpins);
            Profile.DevilTurnPoints = JsonInt(json, "devilTurnPoints", Profile.DevilTurnPoints);
            Profile.SpaRoomDayScore = JsonInt(json, "spaRoomDayScore", Profile.SpaRoomDayScore);
            ParseDevilTreasPointClaimedFromServer(json);
            ParseQuestsFromServer(json);""",
)

patch(
    "UnityClient/Assets/Scripts/Client/GameApp.cs",
    """        void ParseGodCardPointClaimedFromServer(string json)
        {
            int idx = json.IndexOf("\\"godCardPointClaimed\\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;""",
    """        void ParseDevilTreasPointClaimedFromServer(string json)
        {
            int idx = json.IndexOf("\\"devilTreasPointClaimed\\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + 25;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            Profile.DevilTreasPointClaimed = Profile.DevilTreasPointClaimed ?? new System.Collections.Generic.List<int>();
            Profile.DevilTreasPointClaimed.Clear();
            string chunk = json.Substring(start, end - start);
            int pos = 0;
            while (pos < chunk.Length)
            {
                while (pos < chunk.Length && (chunk[pos] == ' ' || chunk[pos] == ',')) pos++;
                int ns = pos;
                while (pos < chunk.Length && chunk[pos] >= '0' && chunk[pos] <= '9') pos++;
                if (pos > ns && int.TryParse(chunk.Substring(ns, pos - ns), out int rid) && rid > 0) Profile.DevilTreasPointClaimed.Add(rid);
            }
        }

        void ParseQuestsFromServer(string json)
        {
            ParseIntListField(json, "acceptedQuests", Profile.AcceptedQuests);
            ParseIntListField(json, "completedQuests", Profile.CompletedQuests);
            int qpIdx = json.IndexOf("\\"questProgress\\":{", System.StringComparison.Ordinal);
            if (qpIdx < 0) return;
            Profile.QuestProgress = Profile.QuestProgress ?? new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
            Profile.QuestProgress.Clear();
            int start = qpIdx + 16;
            int end = json.IndexOf('}', start);
            if (end <= start) return;
            string body = json.Substring(start, end - start);
            int pos = 0;
            while (pos < body.Length)
            {
                int qStart = body.IndexOf('"', pos);
                if (qStart < 0) break;
                int qEnd = body.IndexOf('"', qStart + 1);
                if (qEnd < 0) break;
                if (!int.TryParse(body.Substring(qStart + 1, qEnd - qStart - 1), out int questId) || questId <= 0)
                {
                    pos = qEnd + 1;
                    continue;
                }
                int arrStart = body.IndexOf('[', qEnd);
                int arrEnd = body.IndexOf(']', arrStart);
                if (arrStart < 0 || arrEnd <= arrStart) break;
                var prog = new System.Collections.Generic.List<int>();
                string arr = body.Substring(arrStart + 1, arrEnd - arrStart - 1);
                int ap = 0;
                while (ap < arr.Length)
                {
                    while (ap < arr.Length && (arr[ap] == ' ' || arr[ap] == ',')) ap++;
                    int ns = ap;
                    while (ap < arr.Length && arr[ap] >= '0' && arr[ap] <= '9') ap++;
                    if (ap > ns && int.TryParse(arr.Substring(ns, ap - ns), out int val)) prog.Add(val);
                }
                Profile.QuestProgress[questId] = prog;
                pos = arrEnd + 1;
            }
        }

        void ParseIntListField(string json, string field, System.Collections.Generic.List<int> target)
        {
            if (target == null) return;
            string needle = "\\"" + field + "\\":[";
            int idx = json.IndexOf(needle, System.StringComparison.Ordinal);
            if (idx < 0) return;
            int start = idx + needle.Length;
            int end = json.IndexOf(']', start);
            if (end <= start) return;
            target.Clear();
            string chunk = json.Substring(start, end - start);
            int pos = 0;
            while (pos < chunk.Length)
            {
                while (pos < chunk.Length && (chunk[pos] == ' ' || chunk[pos] == ',')) pos++;
                int ns = pos;
                while (pos < chunk.Length && chunk[pos] >= '0' && chunk[pos] <= '9') pos++;
                if (pos > ns && int.TryParse(chunk.Substring(ns, pos - ns), out int val)) target.Add(val);
            }
        }

        void ParseGodCardPointClaimedFromServer(string json)
        {
            int idx = json.IndexOf("\\"godCardPointClaimed\\":[", System.StringComparison.Ordinal);
            if (idx < 0) return;""",
)

# --- ExtraModulesScreens ---
patch(
    "UnityClient/Assets/Scripts/Client/ExtraModulesScreens.cs",
    "            SysUi.Note(body, \"DevilTreasItemList.xml  ·  今日已转 \" + app.Profile.DevilTurnSpins + \" 次\");",
    "            SysUi.Note(body, \"DevilTreasPointsList.xml  ·  积分 \" + app.Profile.DevilTurnPoints + \"  ·  今日已转 \" + app.Profile.DevilTurnSpins + \" 次\");",
)

patch(
    "UnityClient/Assets/Scripts/Client/ExtraModulesScreens.cs",
    """            SysUi.Row(body, "spin10", "转10次  " + tenCost + " 金", () => PhoneNet.DevilTurnSpin(10));
            if (app.Database != null)
            {
                int shown = 0;
                foreach (DevilTreasItem item in app.Database.DevilTreasItems)""",
    """            SysUi.Row(body, "spin10", "转10次  " + tenCost + " 金", () => PhoneNet.DevilTurnSpin(10));
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
                int shown = 0;
                foreach (DevilTreasItem item in app.Database.DevilTreasItems)""",
)

patch(
    "UnityClient/Assets/Scripts/Client/ExtraModulesScreens.cs",
    """            SysUi.Row(body, "claim", "开红包", PhoneNet.ClaimRedPacket);
        }""",
    """            SysUi.Row(body, "claim", "开红包", PhoneNet.ClaimRedPacket);
            if (app.Profile.Friends != null && app.Profile.Friends.Count > 0)
            {
                string friend = app.Profile.Friends[0];
                SysUi.Row(body, "send", "发红包给 " + friend + "  1000金", () => PhoneNet.SendRedPacket(friend, 1000));
            }
        }""",
)

# --- QuestScreen ---
patch(
    "UnityClient/Assets/Scripts/Client/GameplayScreens.cs",
    """                bool done = app.Profile.QuestDone(q.Id);
                bool acc = app.Profile.QuestAccepted(q.Id);
                string state = done ? "已完成" : acc ? "领取奖励" : "接取";
                string cap = $"{q.Title}  [{state}]  Gold+{q.RewardGold} GP+{q.RewardGp}";""",
    """                bool done = app.Profile.QuestDone(q.Id);
                bool acc = app.Profile.QuestAccepted(q.Id);
                string state = done ? "已完成" : acc ? "领取奖励" : "接取";
                string progress = FormatQuestProgress(app, q);
                string cap = progress.Length > 0
                    ? $"{q.Title}  [{state}]  {progress}  Gold+{q.RewardGold} GP+{q.RewardGp}"
                    : $"{q.Title}  [{state}]  Gold+{q.RewardGold} GP+{q.RewardGp}";""",
)

patch(
    "UnityClient/Assets/Scripts/Client/GameplayScreens.cs",
    """        static void Toggle(GameApp app, QuestInfo q)
        {
            if (app.Profile.QuestDone(q.Id) && !q.CanRepeat) return;""",
    """        static string FormatQuestProgress(GameApp app, QuestInfo q)
        {
            if (!app.Profile.QuestAccepted(q.Id) || q.Conditions == null || q.Conditions.Count == 0)
            {
                return "";
            }

            System.Collections.Generic.List<int> prog = app.Profile.GetQuestProgress(q.Id);
            var parts = new System.Collections.Generic.List<string>();
            for (int i = 0; i < q.Conditions.Count; i++)
            {
                int cur = prog != null && i < prog.Count ? prog[i] : 0;
                int need = UnityEngine.Mathf.Max(1, q.Conditions[i].Para2);
                parts.Add(cur + "/" + need);
            }

            return "[" + string.Join(",", parts) + "]";
        }

        static void Toggle(GameApp app, QuestInfo q)
        {
            if (app.Profile.QuestDone(q.Id) && !q.CanRepeat) return;""",
)

print("Client patched")
print("DONE")
