using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GunMobile.Core;
using GunMobile.Logic;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Net
{
    public sealed class ServerPlayer
    {
        public int Id;
        public string Nick = "Player";
        public int Sex = 1;
        public int Level = 20;
        public int Gp;
        public int Gold = 100000;
        public int Gift = 5000;
        public int Attack = 50;
        public int Defence = 40;
        public int Agility = 40;
        public int Luck = 30;
        public int MagicAttack;
        public int MagicDefence;
        public int BaseDamage = 50;
        public int BaseGuard;
        public int Hp = 1200;
        public int Win;
        public int Lose;
        public int WeaponId = 7001;
        public int EquipHead;
        public int EquipHair;
        public int EquipFace;
        public int EquipCloth;
        public int EquipGlass;
        public int EquipWeapon = 7001;
        public int PetId;
        public int CardId;
        public int TitleId;
        public int TotemId;
        public int MountGrade;
        public int VipLevel;
        public int Honor;
        public int Texp;
        public int PreferredBallId;
        public int LastSignDay = -1;
        public int SignIndex;
        public int LabyrinthFloor = 1;
        public string ConsortiaName = "";
        public int ElfId;
        public int GemLevel;
        public int KingBlessDay = -1;
        public int FarmHarvests;
        public int FusionKeys;
        public int BankGold;
        public int MineDay = -1;
        public int MineDigs;
        public int WorldBossDay = -1;
        public int WorldBossHits;
        public int NecklaceLevel;
        public int HomeTempleLevel;
        public int WardrobeClothId;
        public List<int> WardrobeProperties = new List<int>();
        public int HonorSystemExp;
        public int HonorSystemLevel;
        public List<int> HonorSystemClaimed = new List<int>();
        public int HonorSystemDay = -1;
        public int HonorSystemOps;
        public int RedPacketDay = -1;
        public int RedPacketClaims;
        public int DevilTurnDay = -1;
        public int DevilTurnSpins;
        public int SweepDay = -1;
        public int SweepCount;
        public int DreamlandChapter = 1;
        public int DreamlandSection = 1;
        public int DreamlandClearedSection;
        public int DreamlandDay = -1;
        public int DreamlandAttempts;
        public int WarriorFamHardType;
        public int WarriorFamLevel = 1;
        public int WarriorFamClearedLevel;
        public int WarriorFamDay = -1;
        public int WarriorFamAttempts;
        public List<BagSlot> Bag = new List<BagSlot>();
        public List<int> AcceptedQuests = new List<int>();
        public List<int> CompletedQuests = new List<int>();
        public List<string> Friends = new List<string>();
        public List<ServerMail> Mails = new List<ServerMail>();
        public int NextMailId = 1;
        public List<GodCardSlot> GodCards = new List<GodCardSlot>();
        public int GodCardEquipId;
        public int EngraveSetId;
        public List<StockSlot> StockHoldings = new List<StockSlot>();
        public List<FightSpiritSlot> FightSpirits = new List<FightSpiritSlot>();
        public List<MagicStoneSlot> MagicStones = new List<MagicStoneSlot>();
        public List<EmblemSlot> Emblems = new List<EmblemSlot>();
        public List<SoulStampSlot> SoulStamps = new List<SoulStampSlot>();
        public int NextEmblemId = 1;
        public int NextSoulStampId = 1;

        public void EnsureFightSpirits()
        {
            if (FightSpirits == null)
            {
                FightSpirits = new List<FightSpiritSlot>();
            }

            if (FightSpirits.Count == 0 && GameDatabase.DefaultFightSpiritIds != null)
            {
                foreach (int spiritId in GameDatabase.DefaultFightSpiritIds)
                {
                    FightSpirits.Add(new FightSpiritSlot { SpiritId = spiritId, Level = 0 });
                }
            }
        }

        public int GetFightSpiritLevel(int spiritId)
        {
            EnsureFightSpirits();
            for (int i = 0; i < FightSpirits.Count; i++)
            {
                if (FightSpirits[i].SpiritId == spiritId)
                {
                    return FightSpirits[i].Level;
                }
            }

            return 0;
        }

        public void SetFightSpiritLevel(int spiritId, int level)
        {
            EnsureFightSpirits();
            for (int i = 0; i < FightSpirits.Count; i++)
            {
                if (FightSpirits[i].SpiritId == spiritId)
                {
                    FightSpirits[i].Level = level;
                    return;
                }
            }

            FightSpirits.Add(new FightSpiritSlot { SpiritId = spiritId, Level = level });
        }

        public void EnsureMagicStones()
        {
            if (MagicStones == null)
            {
                MagicStones = new List<MagicStoneSlot>();
            }

            if (MagicStones.Count == 0 && GameDatabase.DefaultMagicStoneTemplateIds != null)
            {
                foreach (int templateId in GameDatabase.DefaultMagicStoneTemplateIds)
                {
                    MagicStones.Add(new MagicStoneSlot { TemplateId = templateId, Level = 0 });
                }
            }
        }

        public int GetMagicStoneLevel(int templateId)
        {
            EnsureMagicStones();
            for (int i = 0; i < MagicStones.Count; i++)
            {
                if (MagicStones[i].TemplateId == templateId)
                {
                    return MagicStones[i].Level;
                }
            }

            return 0;
        }

        public void SetMagicStoneLevel(int templateId, int level)
        {
            EnsureMagicStones();
            for (int i = 0; i < MagicStones.Count; i++)
            {
                if (MagicStones[i].TemplateId == templateId)
                {
                    MagicStones[i].Level = level;
                    return;
                }
            }

            MagicStones.Add(new MagicStoneSlot { TemplateId = templateId, Level = level });
        }
        public void EnsureEmblems() { if (Emblems == null) Emblems = new List<EmblemSlot>(); }
        public EmblemSlot FindEmblem(int id) { EnsureEmblems(); for (int i = 0; i < Emblems.Count; i++) if (Emblems[i].Id == id) return Emblems[i]; return null; }
        public void EnsureSoulStamps() { if (SoulStamps == null) SoulStamps = new List<SoulStampSlot>(); }
        public SoulStampSlot FindSoulStamp(int id) { EnsureSoulStamps(); for (int i = 0; i < SoulStamps.Count; i++) if (SoulStamps[i].Id == id) return SoulStamps[i]; return null; }

        public void EnsureWardrobeProperties()
        {
            if (WardrobeProperties == null) WardrobeProperties = new List<int>();
        }

        public bool HasWardrobeProperty(int propertyId)
        {
            EnsureWardrobeProperties();
            return WardrobeProperties.Contains(propertyId);
        }

        public void AddWardrobeProperty(int propertyId)
        {
            EnsureWardrobeProperties();
            if (propertyId > 0 && !WardrobeProperties.Contains(propertyId)) WardrobeProperties.Add(propertyId);
        }

        public void SyncHonorSystemLevel(GameDatabase db)
        {
            HonorSystemLevel = db != null ? db.HonorSystemLevelFromExp(HonorSystemExp) : 0;
        }

        public void EnsureHonorSystemClaimed()
        {
            if (HonorSystemClaimed == null) HonorSystemClaimed = new List<int>();
        }

        public bool HasHonorClaim(int level)
        {
            EnsureHonorSystemClaimed();
            return HonorSystemClaimed.Contains(level);
        }

        public void TouchHonorSystemDay()
        {
            int day = DateTime.UtcNow.DayOfYear;
            if (HonorSystemDay != day) { HonorSystemDay = day; HonorSystemOps = 0; }
        }

        public TcpClient RoadTcp;
        public NetworkStream RoadStream;
        public TcpClient FightTcp;
        public NetworkStream FightStream;
        public int RoomId = -1;
        public int Seat = -1;
        public bool FightPendingLose;
        public long FightDisconnectedAtMs;

        // PvE pending context
        public int PveNpcId;
        public int PveRewardGold;
        public bool PveLabyrinth;
        public bool PveDreamland;
        public int PveDreamlandChapter;
        public int PveDreamlandSection;
        public bool PveWarriorFam;
        public int PveWarriorFamHardType;
        public int PveWarriorFamLevel;

        public void RecalcStats(GameDatabase db)
        {
            if (db == null) return;
            int atk = 50, def = 40, agi = 40, luck = 30, hp = 1200;
            int baseDmg = 0, baseGuard = 0;

            foreach (int eid in new[] { EquipHead, EquipHair, EquipFace, EquipCloth, EquipGlass, EquipWeapon })
            {
                ItemTemplate it = db.GetItem(eid);
                if (it == null) continue;
                atk += it.Attack; def += it.Defence; agi += it.Agility; luck += it.Luck;
                if (eid == EquipWeapon)
                {
                    baseDmg += it.Attack > 0 ? it.Attack : it.Property7;
                }
            }

            if (db.Pets.TryGetValue(PetId, out PetInfo pet))
            {
                atk += pet.Attack; def += pet.Defence; hp += pet.Blood; agi += pet.Agility; luck += pet.Luck;
            }

            if (db.Cards != null)
            {
                foreach (CardInfo c in db.Cards)
                {
                    if (c.Id == CardId)
                    {
                        atk += c.AddAttack; def += c.AddDefend; agi += c.AddAgility; luck += c.AddLucky;
                        baseDmg += c.AddDamage; baseGuard += c.AddGuard;
                        break;
                    }
                }
            }

            if (db.Titles.TryGetValue(TitleId, out TitleInfo ti))
            {
                atk += ti.Att; def += ti.Def; agi += ti.Agi; luck += ti.Luck;
            }

            if (db.Totems.TryGetValue(TotemId, out TotemInfo to))
            {
                atk += to.AddAttack; def += to.AddDefence; agi += to.AddAgility; luck += to.AddLuck; hp += to.AddBlood;
                baseDmg += to.AddDamage; baseGuard += to.AddGuard;
            }

            if (db.Mounts.TryGetValue(MountGrade, out MountGrade mt))
            {
                hp += mt.AddBlood; atk += mt.AddDamage; baseDmg += mt.AddDamage; baseGuard += mt.AddGuard;
            }

            if (GodCardEquipId > 0 && db.GodCards.TryGetValue(GodCardEquipId, out GodCardInfo gc))
            {
                db.ApplyGodCardBonus(gc, ref atk, ref def, ref agi, ref luck, ref hp);
            }

            db.ApplyEngraveSetBonus(EngraveSetId, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard);

            EnsureFightSpirits();
            db.ApplyFightSpiritStats(FightSpirits, ref atk, ref def, ref agi, ref luck, ref hp);

            EnsureMagicStones();
            int magicAtk = 0;
            int magicDef = 0;
            db.ApplyMagicStoneStats(MagicStones, ref atk, ref def, ref agi, ref luck, ref magicAtk, ref magicDef);
            db.ApplyNecklaceBonus(NecklaceLevel, ref hp, ref def);
            db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);
            EnsureEmblems();
            db.ApplyEmblemStats(Emblems, ref atk, ref def, ref agi, ref luck, ref hp, ref magicAtk, ref magicDef);
            EnsureSoulStamps();
            db.ApplySoulStampStats(SoulStamps, ref atk, ref def, ref agi, ref luck, ref hp);
            MagicAttack = magicAtk;
            MagicDefence = magicDef;
            EnsureWardrobeProperties();
            db.ApplyWardrobeBonus(WardrobeProperties, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard);
            SyncHonorSystemLevel(db);
            db.ApplyHonorSystemBonus(HonorSystemLevel, ref atk, ref def, ref agi, ref luck, ref hp);

            if (db.Spirits.TryGetValue(Mathf.Max(1, GemLevel), out SpiritInfo weaponSpirit))
            {
                atk += weaponSpirit.AttackAdd;
                def += weaponSpirit.DefendAdd;
                agi += weaponSpirit.AgilityAdd;
                luck += weaponSpirit.LuckAdd;
            }

            atk += Texp / 4;
            if (db.Levels.Count > 0)
            {
                hp += db.BloodForLevel(Level);
            }
            else
            {
                hp += Level * 30;
            }

            Attack = atk; Defence = def; Agility = agi; Luck = luck; Hp = hp;
            BaseDamage = baseDmg > 0 ? baseDmg : atk;
            BaseGuard = baseGuard;
        }

        public void AddGp(GameDatabase db, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Gp += amount;
            if (db != null)
            {
                Level = db.LevelFromGp(Gp);
            }
        }

        public string ToJson()
        {
            var sb = new StringBuilder(512);
            sb.Append("{");
            J(sb, "id", Id); sb.Append(",");
            J(sb, "nick", Nick); sb.Append(",");
            J(sb, "sex", Sex); sb.Append(",");
            J(sb, "level", Level); sb.Append(",");
            J(sb, "gp", Gp); sb.Append(",");
            J(sb, "gold", Gold); sb.Append(",");
            J(sb, "gift", Gift); sb.Append(",");
            J(sb, "attack", Attack); sb.Append(",");
            J(sb, "defence", Defence); sb.Append(",");
            J(sb, "agility", Agility); sb.Append(",");
            J(sb, "luck", Luck); sb.Append(",");
            J(sb, "magicAttack", MagicAttack); sb.Append(",");
            J(sb, "magicDefence", MagicDefence); sb.Append(",");
            J(sb, "hp", Hp); sb.Append(",");
            J(sb, "win", Win); sb.Append(",");
            J(sb, "lose", Lose); sb.Append(",");
            J(sb, "weaponId", WeaponId); sb.Append(",");
            J(sb, "equipHead", EquipHead); sb.Append(",");
            J(sb, "equipHair", EquipHair); sb.Append(",");
            J(sb, "equipFace", EquipFace); sb.Append(",");
            J(sb, "equipCloth", EquipCloth); sb.Append(",");
            J(sb, "equipGlass", EquipGlass); sb.Append(",");
            J(sb, "equipWeapon", EquipWeapon); sb.Append(",");
            J(sb, "petId", PetId); sb.Append(",");
            J(sb, "cardId", CardId); sb.Append(",");
            J(sb, "titleId", TitleId); sb.Append(",");
            J(sb, "totemId", TotemId); sb.Append(",");
            J(sb, "mountGrade", MountGrade); sb.Append(",");
            J(sb, "vipLevel", VipLevel); sb.Append(",");
            J(sb, "honor", Honor); sb.Append(",");
            J(sb, "texp", Texp); sb.Append(",");
            J(sb, "preferredBallId", PreferredBallId); sb.Append(",");
            J(sb, "lastSignDay", LastSignDay); sb.Append(",");
            J(sb, "signIndex", SignIndex); sb.Append(",");
            J(sb, "labyrinthFloor", LabyrinthFloor); sb.Append(",");
            J(sb, "consortiaName", ConsortiaName); sb.Append(",");
            J(sb, "elfId", ElfId); sb.Append(",");
            J(sb, "gemLevel", GemLevel); sb.Append(",");
            J(sb, "kingBlessDay", KingBlessDay); sb.Append(",");
            J(sb, "farmHarvests", FarmHarvests); sb.Append(",");
            J(sb, "fusionKeys", FusionKeys); sb.Append(",");
            J(sb, "bankGold", BankGold); sb.Append(",");
            J(sb, "mineDigs", MineDigs); sb.Append(",");
            J(sb, "worldBossHits", WorldBossHits); sb.Append(",");
            J(sb, "necklaceLevel", NecklaceLevel); sb.Append(",");
            J(sb, "homeTempleLevel", HomeTempleLevel); sb.Append(",");
            J(sb, "wardrobeClothId", WardrobeClothId); sb.Append(",");
            J(sb, "honorSystemExp", HonorSystemExp); sb.Append(",");
            J(sb, "honorSystemLevel", HonorSystemLevel); sb.Append(",");
            J(sb, "redPacketClaims", RedPacketClaims); sb.Append(",");
            J(sb, "devilTurnSpins", DevilTurnSpins); sb.Append(",");
            J(sb, "sweepCount", SweepCount); sb.Append(",");
            J(sb, "dreamlandChapter", DreamlandChapter); sb.Append(",");
            J(sb, "dreamlandSection", DreamlandSection); sb.Append(",");
            J(sb, "dreamlandClearedSection", DreamlandClearedSection); sb.Append(",");
            J(sb, "dreamlandAttempts", DreamlandAttempts); sb.Append(",");
            J(sb, "warriorFamHardType", WarriorFamHardType); sb.Append(",");
            J(sb, "warriorFamLevel", WarriorFamLevel); sb.Append(",");
            J(sb, "warriorFamClearedLevel", WarriorFamClearedLevel); sb.Append(",");
            J(sb, "warriorFamAttempts", WarriorFamAttempts); sb.Append(",");
            J(sb, "godCardEquipId", GodCardEquipId); sb.Append(",");
            J(sb, "engraveSetId", EngraveSetId); sb.Append(",");
            sb.Append("\"godCards\":[");
            for (int i = 0; i < GodCards.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("{\"id\":").Append(GodCards[i].Id).Append(",\"count\":").Append(GodCards[i].Count).Append("}");
            }
            sb.Append("],");
            sb.Append("\"stockHoldings\":[");
            for (int i = 0; i < StockHoldings.Count; i++)
            {
                if (i > 0) sb.Append(",");
                StockSlot sh = StockHoldings[i];
                sb.Append("{\"stockId\":").Append(sh.StockId).Append(",\"shares\":").Append(sh.Shares)
                    .Append(",\"avgPrice\":").Append(sh.AvgPrice).Append("}");
            }
            sb.Append("],");
            sb.Append("\"friends\":[");
            for (int i = 0; i < Friends.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("\"").Append((Friends[i] ?? "").Replace("\"", "\\\"")).Append("\"");
            }
            sb.Append("],");
            EnsureFightSpirits();
            sb.Append("\"fightSpirits\":[");
            for (int i = 0; i < FightSpirits.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("{\"spiritId\":").Append(FightSpirits[i].SpiritId)
                    .Append(",\"level\":").Append(FightSpirits[i].Level).Append("}");
            }
            sb.Append("],");
            EnsureMagicStones();
            sb.Append("\"magicStones\":[");
            for (int i = 0; i < MagicStones.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("{\"templateId\":").Append(MagicStones[i].TemplateId)
                    .Append(",\"level\":").Append(MagicStones[i].Level).Append("}");
            }
            sb.Append("],");
            EnsureEmblems(); sb.Append("\"emblems\":[");
            for (int i = 0; i < Emblems.Count; i++) { if (i > 0) sb.Append(","); EmblemSlot e = Emblems[i]; sb.Append("{\"id\":").Append(e.Id).Append(",\"templateId\":").Append(e.TemplateId).Append(",\"types\":").Append(e.Types).Append(",\"profile\":").Append(e.Profile).Append(",\"mainType\":").Append(e.MainType).Append(",\"mainValue\":").Append(e.MainValue).Append(",\"subValue\":").Append(e.SubValue).Append(",\"skillId\":").Append(e.SkillId).Append(",\"equipped\":").Append(e.Equipped).Append("}"); }
            sb.Append("],");
            EnsureSoulStamps(); sb.Append("\"soulStamps\":[");
            for (int i = 0; i < SoulStamps.Count; i++) { if (i > 0) sb.Append(","); SoulStampSlot s = SoulStamps[i]; sb.Append("{\"id\":").Append(s.Id).Append(",\"tempId\":").Append(s.TempId).Append(",\"type\":").Append(s.Type).Append(",\"quality\":").Append(s.Quality).Append(",\"grade\":").Append(s.Grade).Append(",\"proType\":").Append(s.ProType).Append(",\"proValue\":").Append(s.ProValue).Append(",\"skillId\":").Append(s.SkillId).Append(",\"equipped\":").Append(s.Equipped).Append("}"); }
            sb.Append("],");
            EnsureWardrobeProperties();
            sb.Append("\"wardrobeProperties\":[");
            for (int i = 0; i < WardrobeProperties.Count; i++) { if (i > 0) sb.Append(","); sb.Append(WardrobeProperties[i]); }
            sb.Append("],");
            EnsureHonorSystemClaimed();
            sb.Append("\"honorSystemClaimed\":[");
            for (int i = 0; i < HonorSystemClaimed.Count; i++) { if (i > 0) sb.Append(","); sb.Append(HonorSystemClaimed[i]); }
            sb.Append("],");
            sb.Append("\"bag\":[");
            for (int i = 0; i < Bag.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("{");
                J(sb, "t", Bag[i].TemplateId); sb.Append(",");
                J(sb, "c", Bag[i].Count); sb.Append(",");
                J(sb, "s", Bag[i].Strengthen);
                sb.Append("}");
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        static void J(StringBuilder sb, string k, int v) { sb.Append("\"").Append(k).Append("\":").Append(v); }
        static void J(StringBuilder sb, string k, string v)
        {
            sb.Append("\"").Append(k).Append("\":\"").Append((v ?? "").Replace("\"", "\\\"")).Append("\"");
        }

        public bool AddItem(int templateId, int count)
        {
            foreach (var s in Bag) { if (s.TemplateId == templateId) { s.Count += count; return true; } }
            Bag.Add(new BagSlot { TemplateId = templateId, Count = count });
            return true;
        }

        public bool Consume(int templateId, int count)
        {
            for (int i = 0; i < Bag.Count; i++)
            {
                if (Bag[i].TemplateId == templateId && Bag[i].Count >= count)
                {
                    Bag[i].Count -= count;
                    if (Bag[i].Count <= 0) Bag.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

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
        }

        public void GrantTemplateReward(GameDatabase db, int templateId, int count)
        {
            if (count <= 0 || db == null)
            {
                return;
            }

            if (db.IsGoldTemplate(templateId))
            {
                Gold += count;
                return;
            }

            if (db.IsGiftTemplate(templateId))
            {
                Gift += count;
                return;
            }

            AddItem(templateId, count);
        }

        public bool Equip(ItemTemplate item)
        {
            if (item == null || !item.CanEquip) return false;
            switch (item.CategoryId)
            {
                case 1: EquipHead = item.TemplateId; break;
                case 2: EquipGlass = item.TemplateId; break;
                case 3: EquipHair = item.TemplateId; break;
                case 5: EquipCloth = item.TemplateId; break;
                case 6: EquipFace = item.TemplateId; break;
                case 7: EquipWeapon = item.TemplateId; WeaponId = item.TemplateId; break;
                default: return false;
            }
            return true;
        }
    }

    public sealed class BagSlot
    {
        public int TemplateId;
        public int Count = 1;
        public int Strengthen;
    }

    [Serializable]
    public sealed class ServerMail
    {
        public int Id;
        public string Subject = "";
        public string Body = "";
        public int Gold;
        public int ItemId;
        public int ItemCount;
        public bool Claimed;
    }

    public sealed class GameRoom
    {
        public int Id;
        public string Name = "Room";
        public int MapId;
        public int MaxPlayers = 4;
        public List<int> PlayerIds = new List<int>();
        public int ReadyMask;
        public bool InBattle;
        public int Seed;
        public int CurrentTurn;
        public int CurrentPlayer;
        public float TurnTimeLeft = 20f;
        public float Wind;
        public int[] Hp;
        public int[] MaxHp;
        public long TurnStartMs;
        public long BattleStartMs;
        public System.Random Rng;

        // Server-authoritative battle state
        public MapCollision Map;
        public LivingStats[] Livings;
        public BallPhysics[] Balls;
        public float[] PosX;
        public float[] PosY;
        public int[] Facing;

        // Server-authoritative props available for the current turn player.
        // Bit mapping uses propIds = [1,2,4,5,6,7] -> bits 0..5.
        public int CurrentPropMask;

        // Cached FightStart JSON for reconnecting clients.
        public string LastFightStartJson = "";

        // Crater cuts applied during battle — replayed to reconnecting clients.
        public List<string> CraterHistory = new List<string>();

        // PvE NPC seat when solo player starts with PveNpcId (from PC NPCInfoList).
        public int PveNpcId;
        public int NpcSeat = -1;

        // Pet active skill MP/cooldown per seat (server-authoritative online).
        public int[] PetMp;
        public float[] PetSkillCd;

        public BattleEffectTracker Effects = new BattleEffectTracker();

        // Cached final battle reward so late/duplicated clients (reconnect, late ack)
        // still receive the exact same gold/win as computed by the server.
        public int[] LastFightGolds;
        public bool[] LastFightWins;
        public int[] LastFightQuestGolds;
    }

    /// <summary>
    /// Full game server replacing PC Road.Service.exe + Fight.Service.exe.
    /// Uses the same PC XML tables but runs in-process with no SQL Server.
    /// Handles: auth, profile, shop, bag, equip, quest, pet, card, title, totem,
    /// mount, elf, sign-in, lottery, forge, guild, friends, mail, chat,
    /// room list, matchmaking, and server-authoritative battle.
    /// </summary>
    public sealed class MobileGameServer
    {
        struct PendingSurrender
        {
            public ServerPlayer Player;
            public GameRoom Room;
        }

        readonly object _lock = new object();
        readonly Dictionary<int, ServerPlayer> _players = new Dictionary<int, ServerPlayer>();
        readonly Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        const bool BattleDebug = false;
        int _nextPlayerId = 1;
        int _nextRoomId = 1;
        TcpListener _road;
        TcpListener _fight;
        Thread _roadThread;
        Thread _fightThread;
        Thread _timerThread;
        volatile bool _run;
        GameDatabase _db;
        ResLoader _loader;
        int _suicideTimeSec = 120;
        System.Random _rng = new System.Random();
        string _savePath;
        string _auctionPath;
        readonly List<AuctionListing> _auctionList = new List<AuctionListing>();
        int _nextAuctionId = 1;

        public bool Running { get; private set; }
        public string LastError { get; private set; } = "";
        public int PlayerCount { get { lock (_lock) return _players.Count; } }
        public int RoomCount { get { lock (_lock) return _rooms.Count; } }

        public void Start(GameDatabase db, string savePath = null)
        {
            Start(db, null, savePath);
        }

        public void Start(GameDatabase db, ResLoader loader, string savePath = null)
        {
            if (Running) return;
            _db = db;
            _loader = loader;
            _suicideTimeSec = ReadSuicideTimeSec(loader);
            _savePath = savePath ?? Path.Combine(Application.persistentDataPath, "server_players");
            _auctionPath = Path.Combine(_savePath, "auction_list.json");
            try
            {
                Directory.CreateDirectory(_savePath);
            }
            catch { }

            LoadAuctionList();

            try
            {
                _road = new TcpListener(IPAddress.Any, PhonePacket.RoadPort);
                _fight = new TcpListener(IPAddress.Any, PhonePacket.FightPort);
                _road.Start();
                _fight.Start();
                _run = true;
                Running = true;
                _roadThread = new Thread(AcceptRoad) { IsBackground = true, Name = "MobileRoad" };
                _fightThread = new Thread(AcceptFight) { IsBackground = true, Name = "MobileFight" };
                _roadThread.Start();
                _fightThread.Start();

                _timerThread = new Thread(TurnTimerLoop) { IsBackground = true, Name = "MobileTurnTimer" };
                _timerThread.Start();
                Debug.Log($"MobileGameServer listening Road:{PhonePacket.RoadPort} Fight:{PhonePacket.FightPort}");
            }
            catch (Exception e)
            {
                LastError = e.Message;
                Debug.LogWarning("MobileGameServer start: " + e.Message);
                Stop();
            }
        }

        public void Stop()
        {
            _run = false;
            Running = false;
            try { _road?.Stop(); } catch { }
            try { _fight?.Stop(); } catch { }
            lock (_lock)
            {
                foreach (var p in _players.Values)
                {
                    try { p.RoadTcp?.Close(); } catch { }
                    try { p.FightTcp?.Close(); } catch { }
                }
            }
        }

        void AcceptRoad()
        {
            while (_run)
            {
                try
                {
                    TcpClient client = _road.AcceptTcpClient();
                    new Thread(() => ServeRoad(client)) { IsBackground = true }.Start();
                }
                catch { if (!_run) return; }
            }
        }

        void AcceptFight()
        {
            while (_run)
            {
                try
                {
                    TcpClient client = _fight.AcceptTcpClient();
                    new Thread(() => ServeFight(client)) { IsBackground = true }.Start();
                }
                catch { if (!_run) return; }
            }
        }

        void Send(NetworkStream ns, ushort id, string json)
        {
            if (ns == null) return;
            byte[] pkt = PhonePacket.Encode(id, json);
            try { ns.Write(pkt, 0, pkt.Length); } catch { }
        }

        void SendTo(ServerPlayer p, ushort id, string json)
        {
            if (p?.RoadStream != null) Send(p.RoadStream, id, json);
        }

        void SendFightTo(ServerPlayer p, ushort id, string json)
        {
            if (p?.FightStream != null) Send(p.FightStream, id, json);
        }

        ServerPlayer FindByRoadTcp(TcpClient tcp)
        {
            lock (_lock)
            {
                foreach (var p in _players.Values)
                    if (p.RoadTcp == tcp) return p;
            }
            return null;
        }

        ServerPlayer FindByFightTcp(TcpClient tcp)
        {
            lock (_lock)
            {
                foreach (var p in _players.Values)
                    if (p.FightTcp == tcp) return p;
            }
            return null;
        }

        void ServeRoad(TcpClient client)
        {
            ServerPlayer player = null;
            try
            {
                using (client)
                using (NetworkStream ns = client.GetStream())
                {
                    var buf = new MemoryStream();
                    var tmp = new byte[4096];
                    while (_run && client.Connected)
                    {
                        int n = ns.Read(tmp, 0, tmp.Length);
                        if (n <= 0) break;
                        buf.Write(tmp, 0, n);
                        byte[] data = buf.ToArray();
                        int used = 0;
                        while (PhonePacket.TryDecode(data, used, data.Length - used, out ushort id, out string json, out int consumed))
                        {
                            used += consumed;
                            if (id == PhoneMsg.Login)
                            {
                                player = HandleLogin(client, ns, json);
                            }
                            else if (player != null)
                            {
                                HandleRoadMsg(player, ns, id, json);
                            }
                        }
                        if (used > 0)
                        {
                            buf.SetLength(0);
                            if (used < data.Length) buf.Write(data, used, data.Length - used);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (player != null)
                {
                    SavePlayer(player);
                    lock (_lock) { player.RoadTcp = null; player.RoadStream = null; }
                }
            }
        }

        void TurnTimerLoop()
        {
            int turnMs = (_db != null ? _db.BattleTurnSeconds() : 20) * 1000;
            if (turnMs < 5000)
            {
                turnMs = 20000;
            }

            int suicideMs = _suicideTimeSec * 1000;

            const long reconnectGraceMs = 30000;
            const int tickMs = 200;

            while (_run)
            {
                try
                {
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    List<GameRoom> advance = null;
                    List<GameRoom> suicideEnd = null;
                    List<PendingSurrender> toSurrender = null;

                    lock (_lock)
                    {
                        foreach (var room in _rooms.Values)
                        {
                            if (room == null || !room.InBattle) continue;
                            if (room.TurnStartMs <= 0) continue;

                            TickPetSkillCooldowns(room, tickMs / 1000f);

                            if (room.BattleStartMs > 0 && now - room.BattleStartMs >= suicideMs)
                            {
                                suicideEnd ??= new List<GameRoom>();
                                suicideEnd.Add(room);
                                continue;
                            }

                            if (now - room.TurnStartMs >= turnMs)
                            {
                                advance ??= new List<GameRoom>();
                                // Mark immediately to reduce double-advance race.
                                room.TurnStartMs = now;
                                advance.Add(room);
                            }
                        }

                        // Reconnect grace: if a player disconnected during battle and
                        // didn't come back in time, treat it as surrender.
                        foreach (var p in _players.Values)
                        {
                            if (p == null || !p.FightPendingLose) continue;
                            if (p.RoomId < 0) continue;
                            if (!_rooms.TryGetValue(p.RoomId, out var room)) continue;
                            if (room == null || !room.InBattle) continue;
                            if (p.FightDisconnectedAtMs <= 0) continue;
                            if (now - p.FightDisconnectedAtMs < reconnectGraceMs) continue;

                            toSurrender ??= new List<PendingSurrender>();
                            toSurrender.Add(new PendingSurrender { Player = p, Room = room });
                        }
                    }

                    if (advance != null)
                    {
                        foreach (var r in advance)
                        {
                            AdvanceTurnFromTimeout(r);
                        }
                    }

                    if (suicideEnd != null)
                    {
                        foreach (var r in suicideEnd)
                        {
                            EndBattle(r);
                        }
                    }

                    if (toSurrender != null)
                    {
                        foreach (var item in toSurrender)
                        {
                            if (item.Player == null || item.Room == null) continue;

                            lock (_lock)
                            {
                                if (!item.Player.FightPendingLose) continue;
                                if (item.Player.RoomId != item.Room.Id) continue;
                                item.Player.FightPendingLose = false;
                                item.Player.FightDisconnectedAtMs = 0;
                            }

                            // HandleSurrender() re-checks room state and will no-op if needed.
                            HandleSurrender(item.Player, item.Room);
                        }
                    }
                }
                catch { }

                Thread.Sleep(tickMs);
            }
        }

        void ServeFight(TcpClient client)
        {
            ServerPlayer player = null;
            try
            {
                using (client)
                using (NetworkStream ns = client.GetStream())
                {
                    var buf = new MemoryStream();
                    var tmp = new byte[4096];
                    while (_run && client.Connected)
                    {
                        int n = ns.Read(tmp, 0, tmp.Length);
                        if (n <= 0) break;
                        buf.Write(tmp, 0, n);
                        byte[] data = buf.ToArray();
                        int used = 0;
                        while (PhonePacket.TryDecode(data, used, data.Length - used, out ushort id, out string json, out int consumed))
                        {
                            used += consumed;
                            if (id == PhoneMsg.JoinRoom)
                            {
                                int playerId = JI(json, "playerId", 0);
                                int roomId = -1;
                                bool inBattle = false;
                                GameRoom snapRoom = null;
                                int turn = 0;
                                int currentPlayer = 0;
                                float wind = 0f;
                                int propMask = 0;
                                int[] hpArr = null;
                                int[] maxHpArr = null;
                                float[] posXArr = null;
                                int[] facingArr = null;
                                string fightStartJson = null;
                                lock (_lock)
                                {
                                    if (_players.TryGetValue(playerId, out player))
                                    {
                                        player.FightTcp = client;
                                        player.FightStream = ns;
                                        player.FightPendingLose = false;
                                        player.FightDisconnectedAtMs = 0;

                                        if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out var room) &&
                                            room.Hp != null && room.Livings != null)
                                        {
                                            roomId = room.Id;
                                            snapRoom = room;
                                            inBattle = room.InBattle;
                                            turn = room.CurrentTurn;
                                            currentPlayer = room.CurrentPlayer;
                                            wind = room.Wind;
                                            propMask = room.CurrentPropMask;
                                            fightStartJson = room.LastFightStartJson;
                                            hpArr = room.Hp != null ? (int[])room.Hp.Clone() : null;
                                            maxHpArr = room.MaxHp != null ? (int[])room.MaxHp.Clone() : null;
                                            posXArr = room.PosX != null ? (float[])room.PosX.Clone() : null;
                                            facingArr = room.Facing != null ? (int[])room.Facing.Clone() : null;
                                        }
                                    }
                                }
                                Send(ns, PhoneMsg.RoomOk, "{\"ok\":true}");

                                // Help the reconnecting client re-sync quickly.
                                int pc = hpArr != null ? hpArr.Length : 0;
                                bool hasState = pc > 0 && posXArr != null && facingArr != null && maxHpArr != null;
                                if (hasState)
                                {
                                    if (inBattle && !string.IsNullOrEmpty(fightStartJson))
                                    {
                                        Send(ns, PhoneMsg.FightStart, fightStartJson);
                                    }

                                    // State snapshot: HP + x + facing, so reconnect can resume close to server state.
                                    if (inBattle)
                                    {
                                        string turnJson = BuildTurnJson(snapRoom);
                                        Send(ns, PhoneMsg.FightTurn, turnJson);

                                        string propJson = "{\"player\":" + currentPlayer +
                                                           ",\"mask\":" + propMask + "}";
                                        Send(ns, PhoneMsg.FightProp, propJson);
                                    }

                                    var sb = new StringBuilder(512);
                                    sb.Append("{\"playerCount\":").Append(pc);
                                    sb.Append(",\"turn\":").Append(turn);
                                    sb.Append(",\"player\":").Append(currentPlayer);
                                    sb.Append(",\"wind\":").Append(wind.ToString(CultureInfo.InvariantCulture));

                                    for (int i = 0; i < pc; i++)
                                    {
                                        sb.Append(",\"p").Append(i).Append("_hp\":").Append(hpArr[i]);
                                        sb.Append(",\"p").Append(i).Append("_maxhp\":").Append(maxHpArr[i]);
                                        sb.Append(",\"p").Append(i).Append("_x\":").Append(posXArr[i].ToString(CultureInfo.InvariantCulture));
                                        sb.Append(",\"p").Append(i).Append("_facing\":").Append(facingArr[i]);
                                        if (snapRoom.PetMp != null && i < snapRoom.PetMp.Length)
                                        {
                                            sb.Append(",\"p").Append(i).Append("_petMp\":").Append(snapRoom.PetMp[i]);
                                        }

                                        if (snapRoom.PetSkillCd != null && i < snapRoom.PetSkillCd.Length)
                                        {
                                            sb.Append(",\"p").Append(i).Append("_petCd\":").Append(Mathf.CeilToInt(snapRoom.PetSkillCd[i]));
                                        }
                                    }
                                    sb.Append("}");
                                    Send(ns, PhoneMsg.FightState, sb.ToString());

                                    if (inBattle && snapRoom != null && snapRoom.CraterHistory.Count > 0)
                                    {
                                        List<string> craters;
                                        lock (_lock)
                                        {
                                            craters = new List<string>(snapRoom.CraterHistory);
                                        }
                                        foreach (string craterJson in craters)
                                        {
                                            Send(ns, PhoneMsg.FightCrater, craterJson);
                                        }
                                    }

                                    // If battle already ended, resend reward+profile so client can finish UI.
                                    if (!inBattle && snapRoom != null)
                                    {
                                        ResendFightReward(player, snapRoom);
                                        SendTo(player, PhoneMsg.ProfileData, player.ToJson());
                                    }
                                }
                            }
                            else if (player != null)
                            {
                                HandleFightMsg(player, ns, id, json);
                            }
                        }
                        if (used > 0)
                        {
                            buf.SetLength(0);
                            if (used < data.Length) buf.Write(data, used, data.Length - used);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (player != null)
                {
                    // Reconnect grace:
                    // if the client disconnects while in a battle, wait a while and only
                    // surrender if they don't reconnect in time.
                    GameRoom dcRoom = null;
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    lock (_lock)
                    {
                        player.FightTcp = null;
                        player.FightStream = null;
                        if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out dcRoom))
                        {
                            if (dcRoom.InBattle)
                            {
                                player.FightPendingLose = true;
                                player.FightDisconnectedAtMs = now;
                            }
                            else
                            {
                                player.FightPendingLose = false;
                                player.FightDisconnectedAtMs = 0;
                                dcRoom = null;
                            }
                        }
                    }
                }
            }
        }

        ServerPlayer HandleLogin(TcpClient tcp, NetworkStream ns, string json)
        {
            string nick = JS(json, "nick", "Player");
            ServerPlayer player;
            lock (_lock)
            {
                player = LoadOrCreate(nick);
                player.RoadTcp = tcp;
                player.RoadStream = ns;
            }
            player.RecalcStats(_db);
            if (player.Gp <= 0 && _db != null && _db.Levels.Count > 0)
            {
                player.Gp = _db.GpForLevel(player.Level);
            }
            EnsureStarterMails(player);
            SavePlayer(player);
            Send(ns, PhoneMsg.LoginOk, "{\"ok\":true,\"playerId\":" + player.Id + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
            return player;
        }

        void HandleRoadMsg(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            switch (id)
            {
                case PhoneMsg.GetProfile:
                    player.RecalcStats(_db);
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;

                case PhoneMsg.ShopBuy:
                    HandleShopBuy(player, ns, json);
                    break;

                case PhoneMsg.EquipItem:
                    HandleEquip(player, ns, json);
                    break;

                case PhoneMsg.QuestAccept:
                case PhoneMsg.QuestComplete:
                    HandleQuest(player, ns, id, json);
                    break;

                case PhoneMsg.PetSelect:
                    player.PetId = JI(json, "petId", player.PetId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.TitleSelect:
                    player.TitleId = JI(json, "titleId", player.TitleId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.CardSelect:
                    player.CardId = JI(json, "cardId", player.CardId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.TotemBuy:
                    HandleTotemBuy(player, ns, json);
                    break;

                case PhoneMsg.MountUpgrade:
                    HandleMountUpgrade(player, ns, json);
                    break;

                case PhoneMsg.SignIn:
                    HandleSignIn(player, ns);
                    break;

                case PhoneMsg.LotteryDraw:
                    HandleLottery(player, ns, json);
                    break;

                case PhoneMsg.Strengthen:
                    HandleStrengthen(player, ns, json);
                    break;

                case PhoneMsg.BallSelect:
                    player.PreferredBallId = JI(json, "ballId", 0);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.GuildJoin:
                {
                    string gName = JS(json, "name", "");
                    if (!string.IsNullOrEmpty(gName))
                    {
                        player.ConsortiaName = gName;
                        SavePlayer(player);
                    }
                    SendGuildResult(player, ns);
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;
                }

                case PhoneMsg.GuildCreate:
                    HandleGuildCreate(player, ns, json);
                    break;

                case PhoneMsg.GuildLeave:
                    HandleGuildLeave(player, ns);
                    break;

                case PhoneMsg.GuildDonate:
                {
                    int donateGold = _db != null ? _db.ConfigInt("ConsortiaMinOffer", 500) : 500;
                    if (player.Gold >= donateGold && !string.IsNullOrEmpty(player.ConsortiaName))
                    {
                        player.Gold -= donateGold;
                        int honorGain = _db != null ? _db.ConfigInt("ConsortiaOfferCess", 10) * donateGold / 100 : donateGold / 10;
                        player.Honor += Mathf.Max(1, honorGain);
                        SavePlayer(player);
                    }
                    Send(ns, PhoneMsg.GuildResult, "{\"ok\":true}");
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;
                }

                case PhoneMsg.FriendAdd:
                {
                    string fn = JS(json, "name", "");
                    if (string.IsNullOrEmpty(fn))
                    {
                        SendFriendResult(player, ns);
                        Send(ns, PhoneMsg.ProfileData, player.ToJson());
                        break;
                    }

                    if (!player.Friends.Contains(fn))
                    {
                        player.Friends.Add(fn);
                        SavePlayer(player);
                        // Mutual: add this player to the friend's list too
                        lock (_lock)
                        {
                            foreach (var fp in _players.Values)
                            {
                                if (string.Equals(fp.Nick, fn, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!fp.Friends.Contains(player.Nick))
                                    {
                                        fp.Friends.Add(player.Nick);
                                        SavePlayer(fp);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    // Return friend list
                    SendFriendResult(player, ns);
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;
                }

                case PhoneMsg.FriendRemove:
                    HandleFriendRemove(player, ns, json);
                    break;

                case PhoneMsg.MailClaim:
                    HandleMailClaim(player, ns, json);
                    break;

                case PhoneMsg.MailList:
                    Send(ns, PhoneMsg.MailListData, BuildMailListJson(player));
                    break;

                case PhoneMsg.ChatSend:
                {
                    string msg = JS(json, "msg", "");
                    if (!string.IsNullOrEmpty(msg))
                    {
                        string broadcast = "{\"from\":\"" + (player.Nick ?? "").Replace("\"", "") + "\",\"msg\":\"" + msg.Replace("\"", "") + "\"}";
                        lock (_lock)
                        {
                            foreach (var p in _players.Values)
                                SendTo(p, PhoneMsg.ChatBroadcast, broadcast);
                        }
                    }
                    break;
                }

                case PhoneMsg.RankRequest:
                    HandleRankRequest(player, ns, json);
                    break;

                case PhoneMsg.RoomList:
                    HandleRoomList(player, ns);
                    break;

                case PhoneMsg.CreateRoom:
                    HandleCreateRoom(player, ns, json);
                    break;

                case PhoneMsg.JoinRoom:
                    HandleJoinRoom(player, ns, json);
                    break;

                case PhoneMsg.RoomReady:
                    HandleRoomReady(player, ns, json);
                    break;

                case PhoneMsg.RoomLeave:
                    HandleRoomLeave(player, ns);
                    break;

                case PhoneMsg.VipUpgrade:
                    HandleVipUpgrade(player, ns);
                    break;

                case PhoneMsg.TexpTrain:
                    HandleTexpTrain(player, ns);
                    break;

                case PhoneMsg.GemUpgrade:
                    HandleGemUpgrade(player, ns);
                    break;

                case PhoneMsg.GemSpiritUpgrade:
                    HandleGemSpiritUpgrade(player, ns, json);
                    break;

                case PhoneMsg.MagicStoneUpgrade:
                    HandleMagicStoneUpgrade(player, ns, json);
                    break;

                case PhoneMsg.MagicFusion:
                    HandleMagicFusion(player, ns, json);
                    break;

                case PhoneMsg.BankTrade:
                    HandleBankTrade(player, ns, json);
                    break;

                case PhoneMsg.MineDig:
                    HandleMineDig(player, ns);
                    break;

                case PhoneMsg.TeamDungeonStart:
                    HandleTeamDungeonStart(player, ns, json);
                    break;

                case PhoneMsg.TreasureDraw:
                    HandlePoolDraw(player, ns, PhoneMsg.TreasureDraw, 1, 8, _db != null ? _db.TreasureDrawCost() : 200);
                    break;

                case PhoneMsg.CarnivalDraw:
                    HandlePoolDraw(player, ns, PhoneMsg.CarnivalDraw, 10, 99, _db != null ? _db.CarnivalDrawCost() : 500);
                    break;

                case PhoneMsg.PeakBattleStart:
                    HandlePeakBattleStart(player, ns, json);
                    break;

                case PhoneMsg.WorldBossStart:
                    HandleWorldBossStart(player, ns);
                    break;

                case PhoneMsg.NecklaceUpgrade:
                    HandleNecklaceUpgrade(player, ns);
                    break;

                case PhoneMsg.DevilTurnSpin:
                    HandleDevilTurnSpin(player, ns, json);
                    break;

                case PhoneMsg.RedPacketClaim:
                    HandleRedPacketClaim(player, ns);
                    break;

                case PhoneMsg.HomeTempleUpgrade:
                    HandleHomeTempleUpgrade(player, ns);
                    break;

                case PhoneMsg.MailSend:
                    HandleMailSend(player, ns, json);
                    break;

                case PhoneMsg.SweepLabyrinth:
                    HandleSweepLabyrinth(player, ns);
                    break;
                case PhoneMsg.EmblemCraft: HandleEmblemCraft(player, ns, json); break;
                case PhoneMsg.EmblemEquip: HandleEmblemEquip(player, ns, json); break;
                case PhoneMsg.SoulStampCompose: HandleSoulStampCompose(player, ns, json); break;
                case PhoneMsg.SoulStampRefine: HandleSoulStampRefine(player, ns, json); break;
                case PhoneMsg.WardrobeEquip: HandleWardrobeEquip(player, ns, json); break;
                case PhoneMsg.WardrobeUpgrade: HandleWardrobeUpgrade(player, ns, json); break;
                case PhoneMsg.HonorSystemAction: HandleHonorSystemAction(player, ns, json); break;
                case PhoneMsg.HonorSystemClaim: HandleHonorSystemClaim(player, ns, json); break;

                case PhoneMsg.PveStart:
                {
                    player.PveNpcId = JI(json, "npcId", 0);
                    player.PveLabyrinth = JI(json, "labyrinth", 0) != 0;
                    player.PveRewardGold = _db != null
                        ? _db.ComputePveWinGold(player.PveNpcId, player.LabyrinthFloor, player.PveLabyrinth)
                        : 0;
                    Send(ns, PhoneMsg.PveResult, "{\"ok\":true,\"reward\":" + player.PveRewardGold + "}");
                    break;
                }

                case PhoneMsg.FarmCook:
                    HandleFarmCook(player, ns, json);
                    break;

                case PhoneMsg.AuctionSell:
                    HandleAuctionSell(player, ns, json);
                    break;

                case PhoneMsg.AuctionList:
                    Send(ns, PhoneMsg.AuctionListData, BuildAuctionListJson());
                    break;

                case PhoneMsg.AuctionBuy:
                    HandleAuctionBuy(player, ns, json);
                    break;

                case PhoneMsg.ElfSelect:
                    player.ElfId = JI(json, "elfId", player.ElfId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.KingBless:
                    HandleKingBless(player, ns);
                    break;

                case PhoneMsg.GodCardOpen:
                    HandleGodCardOpen(player, ns, json);
                    break;

                case PhoneMsg.EngraveEquip:
                    HandleEngraveEquip(player, ns, json);
                    break;

                case PhoneMsg.StockTrade:
                    HandleStockTrade(player, ns, json);
                    break;

                case PhoneMsg.SetNick:
                {
                    string newNick = JS(json, "nick", player.Nick);
                    if (!string.IsNullOrWhiteSpace(newNick))
                    {
                        player.Nick = newNick.Trim();
                        SavePlayer(player);
                    }
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;
                }

                case PhoneMsg.Ping:
                    Send(ns, PhoneMsg.Ping, "{}");
                    break;

                default:
                    Send(ns, PhoneMsg.Error, "{\"err\":\"unknown road msg " + id + "\"}");
                    break;
            }
        }

        void HandleFightMsg(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            GameRoom room = null;
            List<int> assignedPlayers = null;

            lock (_lock)
            {
                // Normal flow: player must already belong to a room (RoomId set via road join/create).
                if (id != PhoneMsg.FightStart)
                {
                    if (player.RoomId < 0 || !_rooms.TryGetValue(player.RoomId, out room)) return;
                }
                else
                {
                    // Host can press "start fight" without explicit road-room join:
                    // auto-create a room and absorb other "waiting" fight clients.
                    if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out room))
                    {
                        // already in room
                    }
                    else
                    {
                        int mapId = JI(json, "map", 1056);
                        int maxPlayers = 4;
                        room = new GameRoom
                        {
                            Id = _nextRoomId++,
                            MapId = mapId,
                            Name = (player.Nick ?? "Player") + "'s Room",
                            MaxPlayers = maxPlayers
                        };

                        player.RoomId = room.Id;
                        player.Seat = 0;
                        room.PlayerIds.Add(player.Id);

                        // Auto-assign up to maxPlayers-1 waiting clients that already connected to fight socket.
                        foreach (var p in _players.Values)
                        {
                            if (room.PlayerIds.Count >= room.MaxPlayers) break;
                            if (p == player) continue;
                            if (p.RoomId >= 0) continue; // already in a room
                            if (p.FightTcp == null) continue; // not connected to fight yet

                            p.RoomId = room.Id;
                            p.Seat = room.PlayerIds.Count;
                            room.PlayerIds.Add(p.Id);
                        }

                        _rooms[room.Id] = room;
                        assignedPlayers = new List<int>(room.PlayerIds);
                    }
                }
            }

            // Notify clients about their assigned seat (RoomOk is consumed from the ROAD socket).
            if (assignedPlayers != null)
            {
                foreach (int pid in assignedPlayers)
                {
                    if (_players.TryGetValue(pid, out var p))
                    {
                        SendTo(p, PhoneMsg.RoomOk, "{\"roomId\":" + room.Id + ",\"seat\":" + p.Seat + "}");
                    }
                }
            }

            switch (id)
            {
                case PhoneMsg.FightStart:
                    HandleFightStart(player, room, json);
                    break;

                case PhoneMsg.FightWalk:
                {
                    bool allowW;
                    lock (_lock) { allowW = room.InBattle && player.Seat == room.CurrentPlayer; }
                    if (!allowW) return;
                    float wx = JF(json, "x", room.PosX[player.Seat]);
                    int wf = JI(json, "facing", room.Facing[player.Seat]);
                    lock (_lock)
                    {
                        room.PosX[player.Seat] = wx;
                        room.Facing[player.Seat] = wf >= 0 ? 1 : -1;
                        if (room.Map != null)
                            room.PosY[player.Seat] = room.Map.FindStandY(Mathf.Clamp(Mathf.RoundToInt(wx), 0, room.Map.Width - 1), 0);
                    }
                    BroadcastToRoom(room, id, json, player.Id);
                    break;
                }

                case PhoneMsg.FightFire:
                {
                    bool allowF;
                    lock (_lock) { allowF = room.InBattle && player.Seat == room.CurrentPlayer; }
                    if (!allowF) return;
                    string firePayload = EnsureJsonField(json, "who", player.Seat);
                    BroadcastToRoom(room, PhoneMsg.FightFire, firePayload, player.Id);
                    ServerSimulateFire(player, room, firePayload);
                    break;
                }

                case PhoneMsg.FightTurn:
                {
                    int turn = JI(json, "turn", room.CurrentTurn);
                    int who = JI(json, "player", room.CurrentPlayer);
                    float wind = JF(json, "wind", room.Wind);

                    int serverTurn;
                    int serverPlayer;
                    float serverWind;
                    bool okToAdvanceTimer = false;
                    lock (_lock)
                    {
                        if (!room.InBattle)
                        {
                            serverTurn = room.CurrentTurn;
                            serverPlayer = room.CurrentPlayer;
                            serverWind = room.Wind;
                        }
                        else
                        {
                            // Validate sender & turn. Never trust client values blindly.
                            okToAdvanceTimer = player.Seat == room.CurrentPlayer && room.CurrentTurn == turn && who == room.CurrentPlayer;

                            if (okToAdvanceTimer)
                            {
                                room.TurnStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            }

                            serverTurn = room.CurrentTurn;
                            serverPlayer = room.CurrentPlayer;
                            serverWind = room.Wind;
                        }
                    }

                    // Broadcast the server's authoritative turn state.
                    string turnJson = BuildTurnJson(room);
                    BroadcastToRoom(room, PhoneMsg.FightTurn, turnJson, -1);
                    break;
                }

                case PhoneMsg.FightDamage:
                    // Server-authoritative: ignore client-reported damage.
                    // Damage is computed by ServerSimulateFire().
                    break;

                case PhoneMsg.FightOver:
                    HandleFightOver(player, room, json);
                    break;

                case PhoneMsg.FightSurrender:
                    HandleSurrender(player, room);
                    break;

                case PhoneMsg.FightPetSkill:
                    HandlePetActiveSkill(player, room);
                    break;

                default:
                    Send(ns, PhoneMsg.Error, "{\"err\":\"unknown fight msg " + id + "\"}");
                    break;
            }
        }

        void HandleFarmCook(ServerPlayer player, NetworkStream ns, string json)
        {
            int foodId = JI(json, "foodId", 0);
            FarmRecipe recipe = null;
            if (_db != null)
            {
                foreach (FarmRecipe r in _db.Farm)
                {
                    if (r.FoodId == foodId)
                    {
                        recipe = r;
                        break;
                    }
                }
            }

            if (recipe == null)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            if (!player.Consume(recipe.VegetableId, recipe.NeedCount))
            {
                int cost = _db != null ? _db.FarmBuyVegetableCost() : 200;
                if (player.Gold < cost)
                {
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    return;
                }

                player.Gold -= cost;
                player.AddItem(recipe.VegetableId, recipe.NeedCount);
                if (!player.Consume(recipe.VegetableId, recipe.NeedCount))
                {
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    return;
                }
            }

            player.AddItem(recipe.FoodId, 1);
            player.FarmHarvests++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleAuctionSell(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 0);
            int count = JI(json, "count", 1);
            if (count < 1)
            {
                count = 1;
            }

            bool listOnMarket = JI(json, "list", 0) != 0;
            int askPrice = JI(json, "price", 0);
            BagSlot slot = null;
            for (int i = 0; i < player.Bag.Count; i++)
            {
                if (player.Bag[i].TemplateId == templateId && player.Bag[i].Count >= count)
                {
                    slot = player.Bag[i];
                    break;
                }
            }

            if (slot == null)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            ItemTemplate item = _db != null ? _db.GetItem(templateId) : null;
            int floorPrice = _db != null ? _db.AuctionPrice(item) : 80;
            int strengthen = slot.Strengthen;
            if (listOnMarket)
            {
                int price = askPrice > 0 ? askPrice : floorPrice;
                if (!player.Consume(templateId, count))
                {
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    return;
                }

                AuctionListing listing;
                lock (_lock)
                {
                    listing = new AuctionListing
                    {
                        Id = _nextAuctionId++,
                        SellerId = player.Id,
                        SellerNick = player.Nick ?? "",
                        TemplateId = templateId,
                        Count = count,
                        Price = price,
                        Strengthen = strengthen
                    };
                    _auctionList.Add(listing);
                    SaveAuctionList();
                }

                Send(ns, PhoneMsg.AuctionListData,
                    "{\"ok\":true,\"listed\":" + listing.Id + ",\"listings\":" + BuildAuctionListJson() + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }

            if (!player.Consume(templateId, count))
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            player.Gold += floorPrice * count;
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleAuctionBuy(ServerPlayer player, NetworkStream ns, string json)
        {
            int listingId = JI(json, "listingId", 0);
            AuctionListing listing = null;
            lock (_lock)
            {
                for (int i = 0; i < _auctionList.Count; i++)
                {
                    if (_auctionList[i].Id == listingId)
                    {
                        listing = _auctionList[i];
                        break;
                    }
                }
            }

            if (listing == null)
            {
                Send(ns, PhoneMsg.AuctionListData, "{\"ok\":false,\"err\":\"listing gone\"}");
                return;
            }

            int total = listing.Price * listing.Count;
            if (player.Gold < total)
            {
                Send(ns, PhoneMsg.AuctionListData, "{\"ok\":false,\"err\":\"not enough gold\"}");
                return;
            }

            player.Gold -= total;
            player.AddItem(listing.TemplateId, listing.Count);
            for (int i = 0; i < player.Bag.Count; i++)
            {
                if (player.Bag[i].TemplateId == listing.TemplateId)
                {
                    player.Bag[i].Strengthen = Mathf.Max(player.Bag[i].Strengthen, listing.Strengthen);
                    break;
                }
            }

            ServerPlayer seller = null;
            lock (_lock)
            {
                _auctionList.Remove(listing);
                SaveAuctionList();
                foreach (ServerPlayer p in _players.Values)
                {
                    if (p.Id == listing.SellerId)
                    {
                        seller = p;
                        break;
                    }
                }
            }

            if (seller != null)
            {
                seller.Gold += total;
                SavePlayer(seller);
                SendTo(seller, PhoneMsg.ProfileData, seller.ToJson());
            }
            else
            {
                CreditOfflineSeller(listing.SellerId, total);
            }

            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.AuctionListData,
                "{\"ok\":true,\"bought\":" + listingId + ",\"listings\":" + BuildAuctionListJson() + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        string BuildAuctionListJson()
        {
            var sb = new StringBuilder(512);
            sb.Append("[");
            lock (_lock)
            {
                for (int i = 0; i < _auctionList.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    AuctionListing a = _auctionList[i];
                    sb.Append("{\"id\":").Append(a.Id)
                        .Append(",\"seller\":\"").Append((a.SellerNick ?? "").Replace("\"", ""))
                        .Append("\",\"templateId\":").Append(a.TemplateId)
                        .Append(",\"count\":").Append(a.Count)
                        .Append(",\"price\":").Append(a.Price)
                        .Append(",\"strengthen\":").Append(a.Strengthen).Append("}");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        void LoadAuctionList()
        {
            _auctionList.Clear();
            _nextAuctionId = 1;
            try
            {
                if (!File.Exists(_auctionPath))
                {
                    SeedAuctionIfEmpty();
                    return;
                }

                string json = File.ReadAllText(_auctionPath);
                int idx = json.IndexOf("[", StringComparison.Ordinal);
                int end = json.LastIndexOf("]", StringComparison.Ordinal);
                if (idx < 0 || end <= idx)
                {
                    SeedAuctionIfEmpty();
                    return;
                }

                string body = json.Substring(idx + 1, end - idx - 1);
                int pos = 0;
                while (pos < body.Length)
                {
                    int ob = body.IndexOf('{', pos);
                    if (ob < 0) break;
                    int cb = body.IndexOf('}', ob);
                    if (cb < 0) break;
                    string entry = body.Substring(ob, cb - ob + 1);
                    pos = cb + 1;
                    var listing = new AuctionListing
                    {
                        Id = JI(entry, "id", 0),
                        SellerId = JI(entry, "sellerId", 0),
                        SellerNick = JS(entry, "sellerNick", "NPC"),
                        TemplateId = JI(entry, "templateId", 0),
                        Count = JI(entry, "count", 1),
                        Price = JI(entry, "price", 0),
                        Strengthen = JI(entry, "strengthen", 0)
                    };
                    if (listing.Id <= 0 || listing.TemplateId <= 0)
                    {
                        continue;
                    }

                    _auctionList.Add(listing);
                    _nextAuctionId = Mathf.Max(_nextAuctionId, listing.Id + 1);
                }

                if (_auctionList.Count == 0)
                {
                    SeedAuctionIfEmpty();
                }
            }
            catch
            {
                SeedAuctionIfEmpty();
            }
        }

        void SaveAuctionList()
        {
            if (string.IsNullOrEmpty(_auctionPath))
            {
                return;
            }

            var sb = new StringBuilder(512);
            sb.Append("{\"listings\":[");
            lock (_lock)
            {
                for (int i = 0; i < _auctionList.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    AuctionListing a = _auctionList[i];
                    sb.Append("{\"id\":").Append(a.Id)
                        .Append(",\"sellerId\":").Append(a.SellerId)
                        .Append(",\"sellerNick\":\"").Append((a.SellerNick ?? "").Replace("\"", ""))
                        .Append("\",\"templateId\":").Append(a.TemplateId)
                        .Append(",\"count\":").Append(a.Count)
                        .Append(",\"price\":").Append(a.Price)
                        .Append(",\"strengthen\":").Append(a.Strengthen).Append("}");
                }
            }
            sb.Append("],\"nextId\":").Append(_nextAuctionId).Append("}");
            try
            {
                File.WriteAllText(_auctionPath, sb.ToString());
            }
            catch { }
        }

        void SeedAuctionIfEmpty()
        {
            if (_db == null || _db.Shop.Count == 0)
            {
                return;
            }

            lock (_lock)
            {
                if (_auctionList.Count > 0)
                {
                    return;
                }

                int seeded = 0;
                for (int i = 0; i < _db.Shop.Count && seeded < 12; i++)
                {
                    ShopOffer offer = _db.Shop[i];
                    ItemTemplate item = _db.GetItem(offer.TemplateId);
                    if (item == null || offer.TemplateId <= 0)
                    {
                        continue;
                    }

                    _auctionList.Add(new AuctionListing
                    {
                        Id = _nextAuctionId++,
                        SellerId = 0,
                        SellerNick = "系统",
                        TemplateId = offer.TemplateId,
                        Count = 1,
                        Price = Mathf.Max(offer.AValue1, _db.AuctionPrice(item)),
                        Strengthen = 0
                    });
                    seeded++;
                }

                SaveAuctionList();
            }
        }

        void CreditOfflineSeller(int sellerId, int gold)
        {
            if (sellerId <= 0 || gold <= 0)
            {
                return;
            }

            try
            {
                string path = Path.Combine(_savePath, sellerId + ".json");
                if (!File.Exists(path))
                {
                    return;
                }

                ServerPlayerSave save = JsonUtility.FromJson<ServerPlayerSave>(File.ReadAllText(path));
                if (save == null)
                {
                    return;
                }

                save.Gold += gold;
                File.WriteAllText(path, JsonUtility.ToJson(save, true));
            }
            catch { }
        }

        void SendGuildResult(ServerPlayer player, NetworkStream ns)
        {
            var gMembers = new StringBuilder();
            lock (_lock)
            {
                int gm = 0;
                foreach (ServerPlayer p in _players.Values)
                {
                    if (!string.Equals(p.ConsortiaName, player.ConsortiaName, StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrEmpty(player.ConsortiaName))
                    {
                        continue;
                    }

                    if (gm > 0) gMembers.Append(",");
                    gMembers.Append("{\"nick\":\"").Append((p.Nick ?? "").Replace("\"", ""))
                        .Append("\",\"level\":").Append(p.Level)
                        .Append(",\"online\":").Append(p.RoadStream != null ? "true" : "false")
                        .Append("}");
                    gm++;
                }
            }

            Send(ns, PhoneMsg.GuildResult,
                "{\"ok\":true,\"name\":\"" + (player.ConsortiaName ?? "").Replace("\"", "") + "\",\"members\":[" + gMembers + "]}");
        }

        void HandleGuildCreate(ServerPlayer player, NetworkStream ns, string json)
        {
            string gName = JS(json, "name", "").Trim();
            if (string.IsNullOrEmpty(gName))
            {
                Send(ns, PhoneMsg.GuildResult, "{\"ok\":false,\"err\":\"name required\"}");
                return;
            }

            if (!string.IsNullOrEmpty(player.ConsortiaName))
            {
                Send(ns, PhoneMsg.GuildResult, "{\"ok\":false,\"err\":\"already in guild\"}");
                return;
            }

            int cost = _db != null ? _db.ConsortiaCreateCost() : 4000;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.GuildResult, "{\"ok\":false,\"err\":\"not enough gold\"}");
                return;
            }

            lock (_lock)
            {
                foreach (ServerPlayer p in _players.Values)
                {
                    if (string.Equals(p.ConsortiaName, gName, StringComparison.OrdinalIgnoreCase))
                    {
                        Send(ns, PhoneMsg.GuildResult, "{\"ok\":false,\"err\":\"name taken\"}");
                        return;
                    }
                }
            }

            player.Gold -= cost;
            player.ConsortiaName = gName;
            SavePlayer(player);
            SendGuildResult(player, ns);
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleGuildLeave(ServerPlayer player, NetworkStream ns)
        {
            if (string.IsNullOrEmpty(player.ConsortiaName))
            {
                Send(ns, PhoneMsg.GuildResult, "{\"ok\":false,\"err\":\"not in guild\"}");
                return;
            }

            player.ConsortiaName = "";
            SavePlayer(player);
            Send(ns, PhoneMsg.GuildResult, "{\"ok\":true,\"name\":\"\",\"members\":[]}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleFriendRemove(ServerPlayer player, NetworkStream ns, string json)
        {
            string fn = JS(json, "name", "");
            if (!string.IsNullOrEmpty(fn))
            {
                player.Friends.RemoveAll(x => string.Equals(x, fn, StringComparison.OrdinalIgnoreCase));
                SavePlayer(player);
                lock (_lock)
                {
                    foreach (ServerPlayer fp in _players.Values)
                    {
                        if (string.Equals(fp.Nick, fn, StringComparison.OrdinalIgnoreCase))
                        {
                            fp.Friends.RemoveAll(x => string.Equals(x, player.Nick, StringComparison.OrdinalIgnoreCase));
                            SavePlayer(fp);
                            break;
                        }
                    }
                }
            }

            SendFriendResult(player, ns);
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void SendFriendResult(ServerPlayer player, NetworkStream ns)
        {
            var fl = new StringBuilder();
            for (int fi = 0; fi < player.Friends.Count; fi++)
            {
                if (fi > 0) fl.Append(",");
                string fname = player.Friends[fi];
                bool online = false;
                lock (_lock)
                {
                    foreach (ServerPlayer fp in _players.Values)
                    {
                        if (string.Equals(fp.Nick, fname, StringComparison.OrdinalIgnoreCase))
                        {
                            online = fp.RoadStream != null;
                            break;
                        }
                    }
                }
                fl.Append("{\"nick\":\"").Append(fname.Replace("\"", ""))
                  .Append("\",\"online\":").Append(online ? "true" : "false").Append("}");
            }

            Send(ns, PhoneMsg.FriendResult, "{\"ok\":true,\"friends\":[" + fl + "]}");
        }

        void HandleGemSpiritUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            int spiritId = JI(json, "spiritId", 100001);
            player.EnsureFightSpirits();
            int level = player.GetFightSpiritLevel(spiritId);
            int cost = _db != null ? _db.FightSpiritUpgradeCost(spiritId, level) : 0;
            if (cost <= 0 || player.Gold < cost || level >= 12)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            player.Gold -= cost;
            player.SetFightSpiritLevel(spiritId, level + 1);
            int maxLevel = 0;
            for (int i = 0; i < player.FightSpirits.Count; i++)
            {
                maxLevel = Mathf.Max(maxLevel, player.FightSpirits[i].Level);
            }

            player.GemLevel = maxLevel;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleMagicStoneUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 100101);
            player.EnsureMagicStones();
            int level = player.GetMagicStoneLevel(templateId);
            int cost = _db != null ? _db.MagicStoneUpgradeCost(templateId, level) : 0;
            if (cost <= 0 || player.Gold < cost || level >= 10)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            player.Gold -= cost;
            player.SetMagicStoneLevel(templateId, level + 1);
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleMagicFusion(ServerPlayer player, NetworkStream ns, string json)
        {
            int fusionId = JI(json, "fusionId", 0);
            if (_db == null)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            MagicFusionRecipe recipe = _db.GetMagicFusion(fusionId);
            if (recipe == null)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            if (recipe.Type == 1)
            {
                int keyCost = recipe.NeedKey > 0 ? recipe.NeedKey : 10000;
                if (player.Gold < recipe.NeedGold || player.FusionKeys < keyCost)
                {
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    return;
                }

                player.Gold -= recipe.NeedGold;
                player.FusionKeys -= keyCost;
                if (recipe.ItemId > 0)
                {
                    player.AddItem(recipe.ItemId, 1);
                }
            }
            else
            {
                if (recipe.ItemId > 0 && !player.Consume(recipe.ItemId, 1))
                {
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    return;
                }

                player.FusionKeys += Mathf.Max(1, recipe.GetKeys);
            }

            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleBankTrade(ServerPlayer player, NetworkStream ns, string json)
        {
            string action = JS(json, "action", "deposit");
            int amount = JI(json, "amount", 0);
            if (amount <= 0)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            if (action == "withdraw")
            {
                amount = Mathf.Min(amount, player.BankGold);
                player.BankGold -= amount;
                player.Gold += amount;
            }
            else
            {
                amount = Mathf.Min(amount, player.Gold);
                player.Gold -= amount;
                player.BankGold += amount;
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleMineDig(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.MineDay != today)
            {
                player.MineDay = today;
                player.MineDigs = 0;
            }

            int maxDigs = _db != null ? _db.ConfigInt("MineDayLimit", 5) : 5;
            if (player.MineDigs >= maxDigs)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            player.MineDigs++;
            int goldGain = _db != null ? _db.ConfigInt("MineGoldReward", 500) : 500;
            lock (_lock)
            {
                goldGain += _rng.Next(0, 400);
            }

            player.Gold += goldGain;
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleTeamDungeonStart(ServerPlayer player, NetworkStream ns, string json)
        {
            int shopType = JI(json, "shopType", 113);
            TeamDungeonShopEntry entry = null;
            if (_db != null)
            {
                for (int i = 0; i < _db.TeamDungeonShop.Count; i++)
                {
                    if (_db.TeamDungeonShop[i].ShopType == shopType)
                    {
                        entry = _db.TeamDungeonShop[i];
                        break;
                    }
                }
            }

            int needLevel = entry != null ? entry.NeedLevel : 1;
            if (player.Level < needLevel)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"level\"}");
                return;
            }

            int entryFee = entry != null && entry.Condition > 0 ? entry.Condition * 10 : 500;
            if (player.Gold < entryFee)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= entryFee;
            player.PveNpcId = _db != null ? _db.TeamDungeonNpcId(shopType) : 44401;
            player.PveLabyrinth = false;
            player.PveRewardGold = entry != null && entry.Value > 0
                ? entry.Value
                : (_db != null ? _db.ComputePveWinGold(player.PveNpcId, player.LabyrinthFloor, false) : 800);
            SavePlayer(player);
            Send(ns, PhoneMsg.PveResult,
                "{\"ok\":true,\"reward\":" + player.PveRewardGold + ",\"npcId\":" + player.PveNpcId + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandlePoolDraw(ServerPlayer player, NetworkStream ns, ushort resultMsg, int minType, int maxType, int cost)
        {
            if (_db == null || player.Gold < cost)
            {
                Send(ns, resultMsg, "{\"ok\":false}");
                return;
            }

            List<LotteryDrop> pool = _db.LotteryPool(minType, maxType);
            if (pool.Count == 0)
            {
                pool = _db.Lottery;
            }

            if (pool.Count == 0)
            {
                Send(ns, resultMsg, "{\"ok\":false}");
                return;
            }

            player.Gold -= cost;
            int idx;
            lock (_lock)
            {
                idx = _rng.Next(0, pool.Count);
            }

            LotteryDrop drop = pool[idx];
            player.AddItem(drop.TemplateId, drop.Count);
            SavePlayer(player);
            Send(ns, resultMsg, "{\"ok\":true,\"item\":" + drop.TemplateId + ",\"count\":" + drop.Count + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandlePeakBattleStart(ServerPlayer player, NetworkStream ns, string json)
        {
            int rankIndex = JI(json, "rank", 0);
            CelebEntry target = _db != null ? _db.GetPeakBattleTarget(rankIndex) : null;
            if (target == null)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"no target\"}");
                return;
            }

            int entryFee = 300 + rankIndex * 100;
            if (player.Gold < entryFee)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= entryFee;
            player.PveNpcId = _db.PeakBattleNpcId(target);
            player.PveLabyrinth = false;
            player.PveRewardGold = Mathf.Clamp(target.FightPower / 50000, 500, 5000);
            player.Honor += Mathf.Max(10, 50 - rankIndex * 3);
            SavePlayer(player);
            Send(ns, PhoneMsg.PveResult,
                "{\"ok\":true,\"reward\":" + player.PveRewardGold + ",\"npcId\":" + player.PveNpcId +
                ",\"target\":\"" + (target.Nick ?? "").Replace("\"", "") + "\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleWorldBossStart(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.WorldBossDay != today)
            {
                player.WorldBossDay = today;
                player.WorldBossHits = 0;
            }

            int maxHits = _db != null ? _db.ConfigInt("WorldBossDayLimit", 3) : 3;
            if (player.WorldBossHits >= maxHits)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            player.WorldBossHits++;
            player.PveNpcId = _db != null ? _db.WorldBossNpcId() : 44410;
            player.PveLabyrinth = false;
            CampWarReward reward = _db != null ? _db.CampWarRewardForRank(1) : null;
            player.PveRewardGold = _db != null ? _db.ComputePveWinGold(player.PveNpcId, player.LabyrinthFloor, false) : 1200;
            if (reward != null)
            {
                player.GrantTemplateReward(_db, reward.ItemId, reward.Count);
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.PveResult,
                "{\"ok\":true,\"reward\":" + player.PveRewardGold + ",\"npcId\":" + player.PveNpcId + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleNecklaceUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.NecklaceUpgrade, "{\"ok\":false}");
                return;
            }

            NecklaceCastingLevel next = _db.GetNecklaceLevel(player.NecklaceLevel + 1);
            if (next == null)
            {
                Send(ns, PhoneMsg.NecklaceUpgrade, "{\"ok\":false,\"err\":\"max\"}");
                return;
            }

            int cost = _db.NecklaceUpgradeCost(player.NecklaceLevel);
            if (cost <= 0 || player.Gold < cost)
            {
                Send(ns, PhoneMsg.NecklaceUpgrade, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= cost;
            player.NecklaceLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.NecklaceUpgrade,
                "{\"ok\":true,\"level\":" + player.NecklaceLevel + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleEmblemCraft(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.EmblemCraft, "{\"ok\":false}"); return; }
            int types = JI(json, "types", 1), profile = JI(json, "profile", 1);
            EmblemTemplate row = null;
            for (int i = 0; i < _db.EmblemList.Count; i++)
                if (_db.EmblemList[i].Types == types && _db.EmblemList[i].Profile == profile) { row = _db.EmblemList[i]; break; }
            if (row == null) { Send(ns, PhoneMsg.EmblemCraft, "{\"ok\":false,\"err\":\"template\"}"); return; }
            int cost = _db.EmblemCraftGoldCost(row);
            if (cost <= 0 || player.Gold < cost) { Send(ns, PhoneMsg.EmblemCraft, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost;
            int roll; lock (_lock) { roll = _rng.Next(0, 1000); }
            if (roll >= _db.EmblemComposeSuccessRate()) {
                SavePlayer(player); Send(ns, PhoneMsg.EmblemCraft, "{\"ok\":false,\"err\":\"fail\",\"cost\":" + cost + "}"); Send(ns, PhoneMsg.ProfileData, player.ToJson()); return; }
            int skillId = 0; int skillRoll; lock (_lock) { skillRoll = _rng.Next(0, 1000); }
            if (skillRoll < _db.EmblemComposeSkillRate()) { int[] skills = _db.EmblemSkillIds(); if (skills.Length > 0) lock (_lock) { skillId = skills[_rng.Next(0, skills.Length)]; } }
            System.Random rng; lock (_lock) { rng = _rng; }
            player.EnsureEmblems();
            var slot = new EmblemSlot { Id = player.NextEmblemId++, TemplateId = row.TemplateId, Types = row.Types, Profile = row.Profile, MainType = row.MainType,
                MainValue = _db.RollRange(row.MainValue, rng), SubValue = row.SubCount > 0 ? _db.RollRange(row.SubValue, rng) : 0, SkillId = skillId, Equipped = 0 };
            player.Emblems.Add(slot); player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.EmblemCraft, "{\"ok\":true,\"emblemId\":" + slot.Id + ",\"templateId\":" + slot.TemplateId + ",\"mainValue\":" + slot.MainValue + ",\"subValue\":" + slot.SubValue + ",\"skillId\":" + slot.SkillId + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }
        void HandleEmblemEquip(ServerPlayer player, NetworkStream ns, string json)
        {
            int emblemId = JI(json, "emblemId", 0), equipped = JI(json, "equipped", 1);
            player.EnsureEmblems(); EmblemSlot slot = player.FindEmblem(emblemId);
            if (slot == null) { Send(ns, PhoneMsg.EmblemEquip, "{\"ok\":false,\"err\":\"missing\"}"); return; }
            if (equipped != 0) { for (int i = 0; i < player.Emblems.Count; i++) { EmblemSlot o = player.Emblems[i]; if (o != null && o.Types == slot.Types && o.Id != slot.Id) o.Equipped = 0; } slot.Equipped = 1; } else slot.Equipped = 0;
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.EmblemEquip, "{\"ok\":true,\"emblemId\":" + emblemId + ",\"equipped\":" + slot.Equipped + "}"); Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }
        void HandleSoulStampCompose(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.SoulStampCompose, "{\"ok\":false}"); return; }
            int quality = JI(json, "quality", 1);
            SoulStampComposeTemplate compose = _db.GetSoulStampCompose(quality);
            if (compose == null) { Send(ns, PhoneMsg.SoulStampCompose, "{\"ok\":false,\"err\":\"template\"}"); return; }
            int cost = _db.SoulStampComposeGoldCost(compose);
            if (cost <= 0 || player.Gold < cost) { Send(ns, PhoneMsg.SoulStampCompose, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            System.Random rng; lock (_lock) { rng = _rng; }
            SoulStampTemplate row = _db.PickSoulStampByQuality(quality, rng);
            if (row == null) { Send(ns, PhoneMsg.SoulStampCompose, "{\"ok\":false,\"err\":\"template\"}"); return; }
            int proType = _db.PickSoulStampProType(row, rng), proValue = _db.RollSoulStampProValue(row.TempId, proType, rng);
            player.Gold -= cost; player.EnsureSoulStamps();
            var slot = new SoulStampSlot { Id = player.NextSoulStampId++, TempId = row.TempId, Type = row.Type, Quality = row.Quality, Grade = 1, ProType = proType, ProValue = proValue, SkillId = _db.SoulStampSkillId(row, 1), Equipped = 0 };
            player.SoulStamps.Add(slot);
            for (int i = 0; i < player.SoulStamps.Count; i++) { SoulStampSlot o = player.SoulStamps[i]; if (o != null && o.Type == slot.Type && o.Id != slot.Id) o.Equipped = 0; }
            slot.Equipped = 1; player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.SoulStampCompose, "{\"ok\":true,\"soulStampId\":" + slot.Id + ",\"tempId\":" + slot.TempId + ",\"proType\":" + slot.ProType + ",\"proValue\":" + slot.ProValue + ",\"skillId\":" + slot.SkillId + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }
        void HandleSoulStampRefine(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.SoulStampRefine, "{\"ok\":false}"); return; }
            int soulStampId = JI(json, "soulStampId", 0);
            player.EnsureSoulStamps(); SoulStampSlot slot = player.FindSoulStamp(soulStampId);
            if (slot == null) { Send(ns, PhoneMsg.SoulStampRefine, "{\"ok\":false,\"err\":\"missing\"}"); return; }
            int nextGrade = slot.Grade + 1; SoulRefineRatio ratio = _db.GetSoulRefine(slot.Type, nextGrade);
            if (ratio == null) { Send(ns, PhoneMsg.SoulStampRefine, "{\"ok\":false,\"err\":\"max\"}"); return; }
            int cost = _db.SoulStampRefineGoldCost(ratio);
            if (cost <= 0 || player.Gold < cost) { Send(ns, PhoneMsg.SoulStampRefine, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost; int roll; lock (_lock) { roll = _rng.Next(0, 1000); }
            if (roll >= ratio.Rate) { SavePlayer(player); Send(ns, PhoneMsg.SoulStampRefine, "{\"ok\":false,\"err\":\"fail\",\"cost\":" + cost + "}"); Send(ns, PhoneMsg.ProfileData, player.ToJson()); return; }
            slot.Grade = nextGrade; SoulStampTemplate row = _db.GetSoulStamp(slot.TempId);
            if (row != null) slot.SkillId = _db.SoulStampSkillId(row, slot.Grade);
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.SoulStampRefine, "{\"ok\":true,\"soulStampId\":" + slot.Id + ",\"grade\":" + slot.Grade + ",\"skillId\":" + slot.SkillId + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleWardrobeEquip(ServerPlayer player, NetworkStream ns, string json)
        {
            int clothId = JI(json, "clothId", 0);
            if (_db == null || clothId <= 0) { Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":false,\"err\":\"cloth\"}"); return; }
            MagicClothInfo cloth = _db.GetMagicCloth(clothId);
            if (cloth == null || !_db.MagicClothMatchesSex(player.Sex, cloth.Sex))
            { Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":false,\"err\":\"cloth\"}"); return; }
            player.WardrobeClothId = clothId;
            _db.ApplyMagicClothOutfit(cloth, ref player.EquipHead, ref player.EquipHair, ref player.EquipFace,
                ref player.EquipCloth, ref player.EquipGlass, ref player.EquipWeapon);
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":true,\"clothId\":" + clothId + "}");
            Send(ns, PhoneMsg.StatResult, player.ToJson()); Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleWardrobeUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            int propertyId = JI(json, "propertyId", 0);
            if (_db == null || propertyId <= 0) { Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":false}"); return; }
            ClothPropertyInfo row = _db.GetClothProperty(propertyId);
            if (row == null || player.HasWardrobeProperty(propertyId))
            { Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":false,\"err\":\"property\"}"); return; }
            int cost = row.Cost > 0 ? row.Cost : 800;
            if (player.Gold < cost) { Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost; player.AddWardrobeProperty(propertyId);
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.WardrobeUpgrade, "{\"ok\":true,\"propertyId\":" + propertyId + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.StatResult, player.ToJson()); Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleHonorSystemAction(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.HonorSystemAction, "{\"ok\":false}"); return; }
            string action = JS(json, "action", "donate");
            player.TouchHonorSystemDay();
            if (player.HonorSystemOps >= _db.HonorSystemOpLimit())
            { Send(ns, PhoneMsg.HonorSystemAction, "{\"ok\":false,\"err\":\"limit\"}"); return; }
            int gain = 0;
            if (string.Equals(action, "donate", StringComparison.OrdinalIgnoreCase))
            {
                TotemHonorEntry entry = _db.GetTotemHonorEntry(JI(json, "honorId", 1));
                if (entry == null || player.Gold < entry.NeedMoney)
                { Send(ns, PhoneMsg.HonorSystemAction, "{\"ok\":false,\"err\":\"gold\"}"); return; }
                player.Gold -= entry.NeedMoney; gain = entry.AddHonor;
            }
            else if (string.Equals(action, "like", StringComparison.OrdinalIgnoreCase)) gain = _db.HonorSystemLikeHonorGain();
            else if (string.Equals(action, "fight", StringComparison.OrdinalIgnoreCase)) gain = _db.HonorSystemFightHonorGain();
            else { Send(ns, PhoneMsg.HonorSystemAction, "{\"ok\":false,\"err\":\"action\"}"); return; }
            player.HonorSystemOps++; player.HonorSystemExp += gain; player.SyncHonorSystemLevel(_db);
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.HonorSystemAction, "{\"ok\":true,\"gain\":" + gain + ",\"honorSystemExp\":" + player.HonorSystemExp + ",\"honorSystemLevel\":" + player.HonorSystemLevel + "}");
            Send(ns, PhoneMsg.StatResult, player.ToJson()); Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleHonorSystemClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int level = JI(json, "level", player.HonorSystemLevel);
            if (_db == null || level <= 0) { Send(ns, PhoneMsg.HonorSystemClaim, "{\"ok\":false}"); return; }
            player.SyncHonorSystemLevel(_db);
            if (player.HonorSystemLevel < level || player.HasHonorClaim(level))
            { Send(ns, PhoneMsg.HonorSystemClaim, "{\"ok\":false,\"err\":\"level\"}"); return; }
            HonorSystemLevelInfo row = _db.GetHonorSystemLevel(level);
            if (row == null || row.LevelGift <= 0)
            { Send(ns, PhoneMsg.HonorSystemClaim, "{\"ok\":false,\"err\":\"gift\"}"); return; }
            player.EnsureHonorSystemClaimed(); player.HonorSystemClaimed.Add(level);
            player.AddItem(row.LevelGift, 1); SavePlayer(player);
            Send(ns, PhoneMsg.HonorSystemClaim, "{\"ok\":true,\"level\":" + level + ",\"itemId\":" + row.LevelGift + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleDevilTurnSpin(ServerPlayer player, NetworkStream ns, string json)
        {
            int count = JI(json, "count", 1);
            count = Mathf.Clamp(count, 1, 10);
            int unitCost = _db != null ? _db.ConfigInt("DevilTreasureOneCost", 10000) : 10000;
            int cost = count == 10 && _db != null
                ? _db.ConfigInt("DevilTreasureTenCost", unitCost * 10)
                : unitCost * count;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.DevilTurnSpin, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            int today = DateTime.Now.DayOfYear;
            if (player.DevilTurnDay != today)
            {
                player.DevilTurnDay = today;
                player.DevilTurnSpins = 0;
            }

            player.Gold -= cost;
            var rewards = new StringBuilder("[");
            for (int i = 0; i < count; i++)
            {
                DevilTreasItem drop;
                lock (_lock)
                {
                    drop = _db != null ? _db.RollDevilTreas(_rng) : null;
                }

                if (drop == null)
                {
                    continue;
                }

                int amount = Mathf.Max(1, drop.Value);
                if (_db != null)
                {
                    player.GrantTemplateReward(_db, drop.TemplateId, amount);
                }

                if (i > 0)
                {
                    rewards.Append(",");
                }

                rewards.Append("{\"item\":").Append(drop.TemplateId)
                    .Append(",\"count\":").Append(amount)
                    .Append(",\"type\":").Append(drop.Type).Append("}");
            }

            player.DevilTurnSpins += count;
            SavePlayer(player);
            rewards.Append("]");
            Send(ns, PhoneMsg.DevilTurnSpin,
                "{\"ok\":true,\"cost\":" + cost + ",\"rewards\":" + rewards + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleRedPacketClaim(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.RedPacketDay != today)
            {
                player.RedPacketDay = today;
                player.RedPacketClaims = 0;
            }

            int maxClaims = _db != null ? _db.ConfigInt("RedPacketDayLimit", 5) : 5;
            if (player.RedPacketClaims >= maxClaims)
            {
                Send(ns, PhoneMsg.RedPacketClaim, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int minGold = _db != null ? _db.ConfigInt("RedPacketMinGold", 100) : 100;
            int maxGold = _db != null ? _db.ConfigInt("RedPacketMaxGold", 500) : 500;
            int gold;
            lock (_lock)
            {
                gold = _rng.Next(minGold, maxGold + 1);
            }

            player.RedPacketClaims++;
            player.Gold += gold;
            SavePlayer(player);
            Send(ns, PhoneMsg.RedPacketClaim,
                "{\"ok\":true,\"gold\":" + gold + ",\"claims\":" + player.RedPacketClaims + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleHomeTempleUpgrade(ServerPlayer player, NetworkStream ns)
        {
            int maxLevel = _db != null ? _db.ConfigInt("HomeTempleMaxLevel", 20) : 20;
            if (player.HomeTempleLevel >= maxLevel)
            {
                Send(ns, PhoneMsg.HomeTempleUpgrade, "{\"ok\":false,\"err\":\"max\"}");
                return;
            }

            int cost = _db != null ? _db.HomeTempleUpgradeCost(player.HomeTempleLevel) : 800;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.HomeTempleUpgrade, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= cost;
            player.HomeTempleLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.HomeTempleUpgrade,
                "{\"ok\":true,\"level\":" + player.HomeTempleLevel + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleMailSend(ServerPlayer player, NetworkStream ns, string json)
        {
            string to = JS(json, "to", "");
            string subject = JS(json, "subject", "玩家邮件");
            string body = JS(json, "body", "");
            int gold = JI(json, "gold", 0);
            int itemId = JI(json, "itemId", 0);
            int itemCount = JI(json, "itemCount", 0);

            if (string.IsNullOrWhiteSpace(to))
            {
                Send(ns, PhoneMsg.MailSend, "{\"ok\":false,\"err\":\"to\"}");
                return;
            }

            to = to.Trim();
            if (gold > 0 && player.Gold < gold)
            {
                Send(ns, PhoneMsg.MailSend, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (itemId > 0 && itemCount > 0)
            {
                int have = 0;
                foreach (BagSlot b in player.Bag)
                {
                    if (b.TemplateId == itemId)
                    {
                        have += b.Count;
                    }
                }

                if (have < itemCount)
                {
                    Send(ns, PhoneMsg.MailSend, "{\"ok\":false,\"err\":\"item\"}");
                    return;
                }
            }

            ServerPlayer target = null;
            lock (_lock)
            {
                foreach (ServerPlayer p in _players.Values)
                {
                    if (p != player && string.Equals(p.Nick, to, StringComparison.OrdinalIgnoreCase))
                    {
                        target = p;
                        break;
                    }
                }
            }

            if (target == null)
            {
                Send(ns, PhoneMsg.MailSend, "{\"ok\":false,\"err\":\"offline\"}");
                return;
            }

            if (gold > 0)
            {
                player.Gold -= gold;
            }

            if (itemId > 0 && itemCount > 0)
            {
                player.Consume(itemId, itemCount);
            }

            if (target.Mails == null)
            {
                target.Mails = new List<ServerMail>();
            }

            target.Mails.Add(new ServerMail
            {
                Id = target.NextMailId++,
                Subject = subject,
                Body = string.IsNullOrEmpty(body)
                    ? "来自 " + (player.Nick ?? "Player") + " 的邮件。"
                    : body,
                Gold = gold,
                ItemId = itemId,
                ItemCount = itemCount
            });

            SavePlayer(player);
            SavePlayer(target);
            Send(ns, PhoneMsg.MailSend, "{\"ok\":true,\"to\":\"" + to.Replace("\"", "") + "\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
            SendTo(target, PhoneMsg.MailListData, BuildMailListJson(target));
        }

        void HandleSweepLabyrinth(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.SweepDay != today)
            {
                player.SweepDay = today;
                player.SweepCount = 0;
            }

            int maxSweeps = _db != null ? _db.ConfigInt("LabyrinthSweepDayLimit", 3) : 3;
            if (player.SweepCount >= maxSweeps)
            {
                Send(ns, PhoneMsg.SweepLabyrinth, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int floor = Mathf.Max(1, player.LabyrinthFloor);
            int gold = _db != null ? _db.ComputePveWinGold(0, floor, true) : floor * 50;
            if (gold <= 0)
            {
                gold = 50 + floor * 30;
            }

            player.SweepCount++;
            player.Gold += gold;
            player.AddGp(_db, Mathf.Max(10, floor * 5));
            player.LabyrinthFloor++;
            SavePlayer(player);
            Send(ns, PhoneMsg.SweepLabyrinth,
                "{\"ok\":true,\"gold\":" + gold + ",\"floor\":" + player.LabyrinthFloor + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleKingBless(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.KingBlessDay == today)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            player.KingBlessDay = today;
            int gold = _db != null ? _db.KingBlessGold(player.VipLevel) : 400;
            player.Gold += gold;
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleGodCardOpen(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null || _db.GodCards.Count == 0)
            {
                Send(ns, PhoneMsg.GodCardResult, "{\"ok\":false,\"err\":\"no godcard table\"}");
                return;
            }

            int count = JI(json, "count", 1);
            int equipId = JI(json, "equipId", 0);
            if (count <= 0 && equipId > 0)
            {
                if (!OwnsGodCard(player, equipId))
                {
                    Send(ns, PhoneMsg.GodCardResult, "{\"ok\":false,\"err\":\"card not owned\"}");
                    return;
                }

                player.GodCardEquipId = equipId;
                player.RecalcStats(_db);
                SavePlayer(player);
                Send(ns, PhoneMsg.GodCardResult, "{\"ok\":true,\"profile\":" + player.ToJson() + "}");
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            if (count != 5)
            {
                count = 1;
            }

            if (equipId > 0 && OwnsGodCard(player, equipId))
            {
                player.GodCardEquipId = equipId;
            }

            int cost = count == 5
                ? _db.ConfigInt("GodCardOpenFiveTimeMoney", 24688)
                : _db.ConfigInt("GodCardOpenOneTimeMoney", 5000);
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.GodCardResult, "{\"ok\":false,\"err\":\"not enough gold\"}");
                return;
            }

            player.Gold -= cost;
            var rolled = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int id = RollGodCard(player);
                AddGodCard(player, id);
                rolled.Add(id);
            }

            player.RecalcStats(_db);
            SavePlayer(player);
            var sb = new StringBuilder();
            sb.Append("{\"ok\":true,\"cards\":[");
            for (int i = 0; i < rolled.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(rolled[i]);
            }

            sb.Append("],\"profile\":").Append(player.ToJson()).Append("}");
            Send(ns, PhoneMsg.GodCardResult, sb.ToString());
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        int RollGodCard(ServerPlayer player)
        {
            int total = 0;
            foreach (GodCardInfo card in _db.GodCards.Values)
            {
                int w = card.Composition > 0 ? card.Composition : Mathf.Max(1, 50 - card.Level * 8);
                total += w;
            }

            if (total <= 0)
            {
                return 1;
            }

            int roll = player.Id * 17 + DateTime.UtcNow.Millisecond;
            lock (_lock)
            {
                roll = _rooms.Count * 31 + roll;
            }

            roll = Mathf.Abs(roll) % total;
            foreach (GodCardInfo card in _db.GodCards.Values)
            {
                int w = card.Composition > 0 ? card.Composition : Mathf.Max(1, 50 - card.Level * 8);
                roll -= w;
                if (roll < 0)
                {
                    return card.Id;
                }
            }

            return 1;
        }

        static void AddGodCard(ServerPlayer player, int id)
        {
            foreach (GodCardSlot slot in player.GodCards)
            {
                if (slot.Id == id)
                {
                    slot.Count++;
                    return;
                }
            }

            player.GodCards.Add(new GodCardSlot { Id = id, Count = 1 });
        }

        static bool OwnsGodCard(ServerPlayer player, int id)
        {
            foreach (GodCardSlot slot in player.GodCards)
            {
                if (slot.Id == id && slot.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        void HandleEngraveEquip(ServerPlayer player, NetworkStream ns, string json)
        {
            int setId = JI(json, "setId", 0);
            if (setId > 0 && (_db == null || !_db.EngraveSets.ContainsKey(setId)))
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }

            int minLevel = _db != null ? _db.ConfigInt("EngraveLimitLevel", 20) : 20;
            if (setId > 0 && player.Level < minLevel)
            {
                Send(ns, PhoneMsg.Error, "{\"err\":\"level too low for engrave\"}");
                return;
            }

            player.EngraveSetId = setId;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleStockTrade(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null || _db.Stocks.Count == 0)
            {
                Send(ns, PhoneMsg.StockResult, "{\"ok\":false,\"err\":\"no stock table\"}");
                return;
            }

            int minLevel = _db.ConfigInt("StockLimitLevel", 30);
            if (player.Level < minLevel)
            {
                Send(ns, PhoneMsg.StockResult, "{\"ok\":false,\"err\":\"level too low\"}");
                return;
            }

            string action = JS(json, "action", "buy");
            int stockId = JI(json, "stockId", 0);
            int shares = Mathf.Max(1, JI(json, "shares", 1));
            if (!_db.Stocks.TryGetValue(stockId, out StockInfo stock))
            {
                Send(ns, PhoneMsg.StockResult, "{\"ok\":false,\"err\":\"unknown stock\"}");
                return;
            }

            int price = _db.StockQuote(stock);
            if (action == "sell")
            {
                StockSlot holding = FindStock(player, stockId);
                if (holding == null || holding.Shares < shares)
                {
                    Send(ns, PhoneMsg.StockResult, "{\"ok\":false,\"err\":\"not enough shares\"}");
                    return;
                }

                holding.Shares -= shares;
                if (holding.Shares <= 0)
                {
                    player.StockHoldings.Remove(holding);
                }

                player.Gold += price * shares;
            }
            else
            {
                int cost = price * shares;
                if (player.Gold < cost)
                {
                    Send(ns, PhoneMsg.StockResult, "{\"ok\":false,\"err\":\"not enough gold\"}");
                    return;
                }

                player.Gold -= cost;
                StockSlot holding = FindStock(player, stockId);
                if (holding == null)
                {
                    player.StockHoldings.Add(new StockSlot { StockId = stockId, Shares = shares, AvgPrice = price });
                }
                else
                {
                    int totalCost = holding.AvgPrice * holding.Shares + cost;
                    holding.Shares += shares;
                    holding.AvgPrice = holding.Shares > 0 ? totalCost / holding.Shares : price;
                }
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.StockResult, "{\"ok\":true,\"price\":" + price + ",\"profile\":" + player.ToJson() + "}");
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        static StockSlot FindStock(ServerPlayer player, int stockId)
        {
            foreach (StockSlot s in player.StockHoldings)
            {
                if (s.StockId == stockId)
                {
                    return s;
                }
            }

            return null;
        }

        void HandleSurrender(ServerPlayer player, GameRoom room)
        {
            lock (_lock)
            {
                if (!room.InBattle) return;
                int seat = player.Seat;
                if (seat >= 0 && seat < room.Hp.Length)
                {
                    room.Hp[seat] = 0;
                    if (room.Livings != null && seat < room.Livings.Length)
                    {
                        var ls = room.Livings[seat];
                        ls.Hp = 0;
                        room.Livings[seat] = ls;
                    }
                }
            }
            string dmgJson = "{\"target\":" + player.Seat + ",\"dmg\":9999,\"crit\":false,\"surrender\":true}";
            BroadcastToRoom(room, PhoneMsg.FightDamage, dmgJson, -1);
            bool gameOver;
            lock (_lock)
            {
                gameOver = CountAliveTeams(room) <= 1;
            }
            if (gameOver)
            {
                EndBattle(room);
            }
        }

        void HandleShopBuy(ServerPlayer player, NetworkStream ns, string json)
        {
            int offerId = JI(json, "offerId", 0);
            ShopOffer offer = null;
            if (_db != null)
            {
                foreach (var o in _db.Shop) { if (o.Id == offerId) { offer = o; break; } }
            }

            if (offer == null)
            {
                Send(ns, PhoneMsg.ShopResult, "{\"ok\":false,\"err\":\"no offer\"}");
                return;
            }

            bool isGift = offer.APrice1 == -2;
            int price = offer.AValue1;
            if (isGift)
            {
                if (player.Gift < price) { Send(ns, PhoneMsg.ShopResult, "{\"ok\":false,\"err\":\"not enough gift\"}"); return; }
                player.Gift -= price;
            }
            else
            {
                if (player.Gold < price) { Send(ns, PhoneMsg.ShopResult, "{\"ok\":false,\"err\":\"not enough gold\"}"); return; }
                player.Gold -= price;
            }

            player.AddItem(offer.TemplateId, 1);
            SavePlayer(player);
            Send(ns, PhoneMsg.ShopResult, "{\"ok\":true,\"templateId\":" + offer.TemplateId + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleEquip(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 0);
            ItemTemplate item = _db?.GetItem(templateId);
            if (item != null && player.Equip(item))
            {
                player.RecalcStats(_db);
                SavePlayer(player);
                Send(ns, PhoneMsg.EquipResult, "{\"ok\":true}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
            }
            else
            {
                Send(ns, PhoneMsg.EquipResult, "{\"ok\":false}");
            }
        }

        void HandleQuest(ServerPlayer player, NetworkStream ns, ushort id, string json)
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
        }

        void HandleTotemBuy(ServerPlayer player, NetworkStream ns, string json)
        {
            int totemId = JI(json, "totemId", 0);
            if (_db != null && _db.Totems.TryGetValue(totemId, out TotemInfo t))
            {
                if (player.Honor >= t.ConsumeHonor)
                {
                    if (player.TotemId != totemId && t.ConsumeHonor > 0)
                        player.Honor -= t.ConsumeHonor;
                    player.TotemId = totemId;
                    player.RecalcStats(_db);
                    SavePlayer(player);
                }
            }
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleMountUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            int cost = _db != null ? _db.MountUpgradeCost(player.MountGrade) : 0;
            if (cost > 0 && player.Gold >= cost)
            {
                player.Gold -= cost;
                player.MountGrade++;
                player.RecalcStats(_db);
                SavePlayer(player);
            }
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleSignIn(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.LastSignDay == today)
            {
                Send(ns, PhoneMsg.SignInResult, "{\"ok\":false,\"err\":\"already signed\"}");
                return;
            }

            player.LastSignDay = today;
            player.SignIndex = Mathf.Min(28, player.SignIndex + 1);
            if (_db != null && _db.SignIn.Count > 0)
            {
                int dayIdx = Mathf.Clamp(player.SignIndex, 1, _db.SignIn.Count);
                SignReward reward = null;
                foreach (SignReward r in _db.SignIn)
                {
                    if (r.Day == dayIdx)
                    {
                        reward = r;
                        break;
                    }
                }

                if (reward == null && dayIdx - 1 < _db.SignIn.Count)
                {
                    reward = _db.SignIn[dayIdx - 1];
                }

                if (reward != null)
                {
                    player.GrantTemplateReward(_db, reward.TemplateId, reward.Count);
                }
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.SignInResult, "{\"ok\":true}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleLottery(ServerPlayer player, NetworkStream ns, string json)
        {
            int count = JI(json, "count", 1);
            if (count < 1) count = 1;
            int cost = _db != null ? _db.LotteryDrawCost(count) : 100;
            if (player.Gold < cost || _db == null || _db.Lottery.Count == 0)
            {
                Send(ns, PhoneMsg.LotteryResult, "{\"ok\":false}");
                return;
            }
            player.Gold -= cost;
            int draws = count == 1 ? 1 : 10;
            var won = new List<int>();
            for (int i = 0; i < draws; i++)
            {
                int idx;
                lock (_lock) { idx = _rng.Next(0, _db.Lottery.Count); }
                var drop = _db.Lottery[idx];
                player.AddItem(drop.TemplateId, drop.Count);
                won.Add(drop.TemplateId);
            }
            SavePlayer(player);
            var sb = new StringBuilder("{\"ok\":true,\"items\":[");
            for (int i = 0; i < won.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(won[i]);
            }
            sb.Append("]}");
            Send(ns, PhoneMsg.LotteryResult, sb.ToString());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleStrengthen(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 0);
            BagSlot slot = null;
            foreach (var s in player.Bag) { if (s.TemplateId == templateId) { slot = s; break; } }
            if (slot == null || slot.Strengthen >= 15)
            {
                Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":false}");
                return;
            }
            int next = slot.Strengthen + 1;
            int gold = _db != null ? _db.StrengthenGoldCost(next) : Mathf.Max(100, 200 * next);
            if (player.Gold < gold)
            {
                Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }
            player.Gold -= gold;
            int successRate = _db != null ? _db.StrengthenSuccessChance(slot.Strengthen) : Mathf.Clamp(90 - slot.Strengthen * 5, 20, 90);
            int roll;
            lock (_lock) { roll = _rng.Next(0, 100); }
            bool success = roll < successRate;
            if (success) slot.Strengthen++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":true,\"success\":" + (success ? "true" : "false") + ",\"level\":" + slot.Strengthen + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleVipUpgrade(ServerPlayer player, NetworkStream ns)
        {
            int cost = _db != null ? _db.VipUpgradeGiftCost() : 500;
            if (player.Gift < cost || player.VipLevel >= 15)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }
            player.Gift -= cost;
            player.VipLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleTexpTrain(ServerPlayer player, NetworkStream ns)
        {
            int cost = _db != null ? _db.TexpTrainGoldCost() : 400;
            int gain = _db != null ? _db.TexpTrainGain() : 25;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }
            player.Gold -= cost;
            player.Texp += gain;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleGemUpgrade(ServerPlayer player, NetworkStream ns)
        {
            int cost = _db != null ? _db.GemUpgradeCost(player.GemLevel) : 0;
            if (cost <= 0 || player.Gold < cost || player.GemLevel >= 12)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }
            player.Gold -= cost;
            player.GemLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void EnsureStarterMails(ServerPlayer player)
        {
            if (player.Mails == null)
            {
                player.Mails = new List<ServerMail>();
            }

            bool hasWelcome = false;
            bool hasDaily = false;
            foreach (ServerMail m in player.Mails)
            {
                if (m.Subject != null && m.Subject.Contains("系统奖励")) hasWelcome = true;
                if (m.Subject != null && m.Subject.Contains("签到")) hasDaily = true;
            }

            if (!hasWelcome)
            {
                int gold = 0;
                if (_db != null)
                {
                    int itemId = _db.ConfigInt("CheckRewardItem", 11001);
                    int count = _db.ConfigInt("CheckCount", 10);
                    ItemTemplate item = _db.GetItem(itemId);
                    if (item != null)
                    {
                        gold = Mathf.Max(0, (item.Attack + item.Defence) * count);
                    }
                }

                player.Mails.Add(new ServerMail
                {
                    Id = player.NextMailId++,
                    Subject = "系统奖励",
                    Body = "来自 PC serverconfig CheckRewardItem 的离线奖励。",
                    Gold = gold
                });
            }

            if (!hasDaily)
            {
                player.Mails.Add(new ServerMail
                {
                    Id = player.NextMailId++,
                    Subject = "每日签到补发",
                    Body = "登录奖励，可在邮件中领取。",
                    Gold = _db != null ? _db.ConfigInt("EveryDaySignInGold", 500) : 500
                });
            }
        }

        string BuildMailListJson(ServerPlayer player)
        {
            var sb = new StringBuilder("{\"mails\":[");
            for (int i = 0; i < player.Mails.Count; i++)
            {
                ServerMail m = player.Mails[i];
                if (i > 0) sb.Append(",");
                sb.Append("{\"id\":").Append(m.Id)
                  .Append(",\"subject\":\"").Append((m.Subject ?? "").Replace("\"", ""))
                  .Append("\",\"body\":\"").Append((m.Body ?? "").Replace("\"", ""))
                  .Append("\",\"gold\":").Append(m.Gold)
                  .Append(",\"itemId\":").Append(m.ItemId)
                  .Append(",\"itemCount\":").Append(m.ItemCount)
                  .Append(",\"claimed\":").Append(m.Claimed ? "true" : "false")
                  .Append("}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        void HandleMailClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int mailId = JI(json, "id", 0);
            int mailGold = 0;
            int claimedItems = 0;
            if (mailId <= 0)
            {
                foreach (ServerMail m in player.Mails)
                {
                    if (m.Claimed) continue;
                    mailGold += ClaimOneMail(player, m);
                    if (m.ItemId > 0 && m.ItemCount > 0) claimedItems++;
                }
            }
            else
            {
                foreach (ServerMail m in player.Mails)
                {
                    if (m.Id != mailId || m.Claimed) continue;
                    mailGold += ClaimOneMail(player, m);
                    if (m.ItemId > 0 && m.ItemCount > 0) claimedItems++;
                    break;
                }
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.MailResult, "{\"ok\":true,\"gold\":" + mailGold + ",\"items\":" + claimedItems + "}");
            Send(ns, PhoneMsg.MailListData, BuildMailListJson(player));
            if (mailGold > 0 || claimedItems > 0)
            {
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
            }
        }

        int ClaimOneMail(ServerPlayer player, ServerMail m)
        {
            if (m.Claimed) return 0;
            m.Claimed = true;
            if (m.Gold > 0) player.Gold += m.Gold;
            if (m.ItemId > 0 && m.ItemCount > 0) player.AddItem(m.ItemId, m.ItemCount);
            return m.Gold;
        }

        string BuildRoomStateJson(GameRoom room)
        {
            var sb = new StringBuilder("{\"roomId\":").Append(room.Id)
              .Append(",\"map\":").Append(room.MapId)
              .Append(",\"max\":").Append(room.MaxPlayers)
              .Append(",\"readyMask\":").Append(room.ReadyMask)
              .Append(",\"inBattle\":").Append(room.InBattle ? "true" : "false")
              .Append(",\"players\":[");
            lock (_lock)
            {
                for (int i = 0; i < room.PlayerIds.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    int pid = room.PlayerIds[i];
                    ServerPlayer p = _players.TryGetValue(pid, out ServerPlayer sp) ? sp : null;
                    bool ready = (room.ReadyMask & (1 << i)) != 0;
                    sb.Append("{\"seat\":").Append(i)
                      .Append(",\"nick\":\"").Append((p?.Nick ?? "P" + (i + 1)).Replace("\"", ""))
                      .Append("\",\"ready\":").Append(ready ? "true" : "false")
                      .Append(",\"online\":").Append(p?.RoadStream != null ? "true" : "false")
                      .Append("}");
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        void BroadcastRoomState(GameRoom room)
        {
            string json = BuildRoomStateJson(room);
            lock (_lock)
            {
                foreach (int pid in room.PlayerIds)
                {
                    if (_players.TryGetValue(pid, out ServerPlayer p))
                    {
                        SendTo(p, PhoneMsg.RoomState, json);
                    }
                }
            }
        }

        bool AllHumanPlayersReady(GameRoom room)
        {
            int n = room.PlayerIds.Count;
            if (n <= 1) return true;
            int mask = (1 << n) - 1;
            return (room.ReadyMask & mask) == mask;
        }

        void HandleRoomReady(ServerPlayer player, NetworkStream ns, string json)
        {
            bool ready = JI(json, "ready", 1) != 0;
            GameRoom room;
            lock (_lock)
            {
                if (player.RoomId < 0 || !_rooms.TryGetValue(player.RoomId, out room) || room.InBattle)
                {
                    Send(ns, PhoneMsg.Error, "{\"err\":\"not in room\"}");
                    return;
                }

                int seat = room.PlayerIds.IndexOf(player.Id);
                if (seat < 0)
                {
                    Send(ns, PhoneMsg.Error, "{\"err\":\"not in room\"}");
                    return;
                }

                if (ready)
                {
                    room.ReadyMask |= 1 << seat;
                }
                else
                {
                    room.ReadyMask &= ~(1 << seat);
                }
            }
            BroadcastRoomState(room);
            Send(ns, PhoneMsg.RoomState, BuildRoomStateJson(room));
        }

        void HandleRoomLeave(ServerPlayer player, NetworkStream ns)
        {
            GameRoom room = null;
            lock (_lock)
            {
                if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out room))
                {
                    int seat = room.PlayerIds.IndexOf(player.Id);
                    if (seat >= 0)
                    {
                        room.PlayerIds.RemoveAt(seat);
                        room.ReadyMask = ShiftReadyMask(room.ReadyMask, seat);
                    }

                    player.RoomId = -1;
                    player.Seat = -1;
                    if (room.PlayerIds.Count == 0)
                    {
                        _rooms.Remove(room.Id);
                        room = null;
                    }
                }
            }

            Send(ns, PhoneMsg.RoomOk, "{\"roomId\":-1,\"seat\":-1}");
            if (room != null)
            {
                BroadcastRoomState(room);
            }
        }

        static int ShiftReadyMask(int mask, int removedSeat)
        {
            int low = mask & ((1 << removedSeat) - 1);
            int high = (mask >> (removedSeat + 1)) << removedSeat;
            return low | high;
        }

        void HandleRankRequest(ServerPlayer player, NetworkStream ns, string json)
        {
            string type = JS(json, "type", "gp");
            var celeb = _db != null ? _db.CelebForType(type) : null;
            var sb = new StringBuilder("{\"type\":\"").Append(type.Replace("\"", "")).Append("\",\"ranks\":[");
            int count = 0;
            if (celeb != null && celeb.Count > 0)
            {
                count = Mathf.Min(50, celeb.Count);
                for (int i = 0; i < count; i++)
                {
                    CelebEntry e = celeb[i];
                    if (i > 0) sb.Append(",");
                    sb.Append("{\"rank\":").Append(e.Rank)
                      .Append(",\"nick\":\"").Append((e.Nick ?? "").Replace("\"", ""))
                      .Append("\",\"level\":").Append(e.Grade)
                      .Append(",\"gp\":").Append(e.Gp)
                      .Append(",\"fightPower\":").Append(e.FightPower)
                      .Append(",\"offer\":").Append(e.Offer)
                      .Append(",\"win\":").Append(e.WinCount)
                      .Append(",\"lose\":").Append(Mathf.Max(0, e.TotalCount - e.WinCount))
                      .Append(",\"vip\":").Append(e.VipLevel)
                      .Append(",\"consortia\":\"").Append((e.ConsortiaName ?? "").Replace("\"", ""))
                      .Append("\"}");
                }
            }
            else
            {
                var sorted = new List<ServerPlayer>();
                lock (_lock)
                {
                    sorted.AddRange(_players.Values);
                }
                sorted.Sort((a, b) => b.Win.CompareTo(a.Win));
                count = Mathf.Min(50, sorted.Count);
                for (int i = 0; i < count; i++)
                {
                    if (i > 0) sb.Append(",");
                    ServerPlayer p = sorted[i];
                    sb.Append("{\"rank\":").Append(i + 1)
                      .Append(",\"nick\":\"").Append((p.Nick ?? "").Replace("\"", ""))
                      .Append("\",\"level\":").Append(p.Level)
                      .Append(",\"gp\":").Append(p.Gp)
                      .Append(",\"fightPower\":0")
                      .Append(",\"offer\":").Append(p.Honor)
                      .Append(",\"win\":").Append(p.Win)
                      .Append(",\"lose\":").Append(p.Lose)
                      .Append(",\"vip\":").Append(p.VipLevel)
                      .Append(",\"consortia\":\"").Append((p.ConsortiaName ?? "").Replace("\"", ""))
                      .Append("\"}");
                }
            }

            bool listed = false;
            string selfNick = (player.Nick ?? "").Replace("\"", "");
            string body = sb.ToString();
            if (!string.IsNullOrEmpty(selfNick) && body.IndexOf("\"nick\":\"" + selfNick + "\"", StringComparison.Ordinal) >= 0)
            {
                listed = true;
            }

            if (!listed && !string.IsNullOrEmpty(selfNick))
            {
                if (count > 0) sb.Append(",");
                sb.Append("{\"rank\":0")
                  .Append(",\"nick\":\"").Append(selfNick)
                  .Append("\",\"level\":").Append(player.Level)
                  .Append(",\"gp\":").Append(player.Gp)
                  .Append(",\"fightPower\":0")
                  .Append(",\"offer\":").Append(player.Honor)
                  .Append(",\"win\":").Append(player.Win)
                  .Append(",\"lose\":").Append(player.Lose)
                  .Append(",\"vip\":").Append(player.VipLevel)
                  .Append(",\"consortia\":\"").Append((player.ConsortiaName ?? "").Replace("\"", ""))
                  .Append("\",\"self\":true}");
            }

            sb.Append("]}");
            Send(ns, PhoneMsg.RankData, sb.ToString());
        }

        const float BattleTurnSeconds = 20f;

        float BattleTurnSecondsValue()
        {
            if (_db != null)
            {
                int sec = _db.BattleTurnSeconds();
                if (sec >= 5)
                {
                    return sec;
                }
            }

            return BattleTurnSeconds;
        }

        float TurnTimeLeftSeconds(GameRoom room)
        {
            float budget = BattleTurnSecondsValue();
            if (room == null || room.TurnStartMs <= 0) return budget;
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return Mathf.Max(0f, budget - (now - room.TurnStartMs) / 1000f);
        }

        string BuildTurnJson(GameRoom room)
        {
            float tl = TurnTimeLeftSeconds(room);
            return "{\"turn\":" + room.CurrentTurn +
                   ",\"player\":" + room.CurrentPlayer +
                   ",\"wind\":" + room.Wind.ToString(CultureInfo.InvariantCulture) +
                   ",\"timeLeft\":" + tl.ToString(CultureInfo.InvariantCulture) + "}";
        }

        void HandleRoomList(ServerPlayer player, NetworkStream ns)
        {
            var sb = new StringBuilder("{\"rooms\":[");
            int i = 0;
            lock (_lock)
            {
                foreach (var r in _rooms.Values)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("{\"id\":").Append(r.Id)
                      .Append(",\"name\":\"").Append(r.Name.Replace("\"", ""))
                      .Append("\",\"map\":").Append(r.MapId)
                      .Append(",\"players\":").Append(r.PlayerIds.Count)
                      .Append(",\"max\":").Append(r.MaxPlayers)
                      .Append(",\"inBattle\":").Append(r.InBattle ? "true" : "false")
                      .Append("}");
                    i++;
                }
            }
            sb.Append("]}");
            Send(ns, PhoneMsg.RoomListData, sb.ToString());
        }

        void HandleCreateRoom(ServerPlayer player, NetworkStream ns, string json)
        {
            int mapId = JI(json, "mapId", 1056);
            string name = JS(json, "name", player.Nick + "'s Room");
            int maxPlayers = Mathf.Clamp(JI(json, "maxPlayers", 4), 2, 4);
            GameRoom room;
            lock (_lock)
            {
                room = new GameRoom { Id = _nextRoomId++, MapId = mapId, Name = name, MaxPlayers = maxPlayers };
                room.PlayerIds.Add(player.Id);
                player.RoomId = room.Id;
                player.Seat = 0;
                _rooms[room.Id] = room;
            }
            Send(ns, PhoneMsg.RoomCreated, "{\"roomId\":" + room.Id + ",\"seat\":0,\"maxPlayers\":" + maxPlayers + "}");
            BroadcastRoomState(room);
        }

        void HandleJoinRoom(ServerPlayer player, NetworkStream ns, string json)
        {
            int roomId = JI(json, "roomId", 0);
            lock (_lock)
            {
                if (!_rooms.TryGetValue(roomId, out GameRoom room) || room.PlayerIds.Count >= room.MaxPlayers || room.InBattle)
                {
                    Send(ns, PhoneMsg.Error, "{\"err\":\"room full or not found\"}");
                    return;
                }
                room.PlayerIds.Add(player.Id);
                player.RoomId = roomId;
                player.Seat = room.PlayerIds.Count - 1;
                Send(ns, PhoneMsg.RoomOk, "{\"roomId\":" + roomId + ",\"seat\":" + player.Seat + "}");
                BroadcastRoomState(room);
                return;
            }
        }

        void HandleFightStart(ServerPlayer host, GameRoom room, string json)
        {
            int mapId = JI(json, "map", room.MapId);
            int seed = JI(json, "seed", Environment.TickCount);
            int n;
            lock (_lock)
            {
                if (room.InBattle)
                {
                    return;
                }

                int humanCount = room.PlayerIds.Count;
                bool soloPve = _players.TryGetValue(host.Id, out ServerPlayer hostPlayer) && hostPlayer.PveNpcId > 0 && humanCount == 1;
                if (!soloPve && humanCount > 1 && !AllHumanPlayersReady(room))
                {
                    if (host.RoadStream != null)
                    {
                        Send(host.RoadStream, PhoneMsg.Error, "{\"err\":\"not all ready\"}");
                    }
                    return;
                }

                room.MapId = mapId;
                room.InBattle = true;
                room.Seed = seed;
                room.CurrentTurn = 0;
                room.Wind = new System.Random(seed).Next(-3, 4) * 10;
                int pveNpcId = 0;
                if (hostPlayer != null && hostPlayer.PveNpcId > 0 && humanCount == 1)
                {
                    pveNpcId = hostPlayer.PveNpcId;
                }

                n = humanCount + (pveNpcId > 0 ? 1 : 0);
                room.PveNpcId = pveNpcId;
                room.NpcSeat = pveNpcId > 0 ? humanCount : -1;
                room.Hp = new int[n];
                room.MaxHp = new int[n];
                room.Livings = new LivingStats[n];
                room.Balls = new BallPhysics[n];
                room.PosX = new float[n];
                room.PosY = new float[n];
                room.Facing = new int[n];
                room.PetMp = new int[n];
                room.PetSkillCd = new float[n];
                room.Effects.Clear();
                for (int i = 0; i < n; i++)
                {
                    int team = (i % 2) + 1;
                    room.Balls[i] = BallPhysics.Default;
                    room.PetMp[i] = 100;
                    room.PetSkillCd[i] = 0f;
                    if (i < humanCount && _players.TryGetValue(room.PlayerIds[i], out ServerPlayer p))
                    {
                        p.RecalcStats(_db);
                        room.Hp[i] = p.Hp;
                        room.MaxHp[i] = p.Hp;
                        room.Livings[i] = new LivingStats
                        {
                            Attack = p.Attack, Defence = p.Defence,
                            Agility = p.Agility, Luck = p.Luck,
                            MagicAttack = p.MagicAttack, MagicDefence = p.MagicDefence,
                            BaseDamage = p.BaseDamage, BaseGuard = p.BaseGuard,
                            Grade = p.Level > 0 ? p.Level : 1,
                            Hp = p.Hp, MaxHp = p.Hp, Team = team
                        };
                        if (_db != null)
                        {
                            room.Balls[i] = _db.ResolveBall(p.WeaponId, p.PreferredBallId);
                            room.PetMp[i] = _db.PetMpMax(p.PetId);
                        }
                    }
                    else if (pveNpcId > 0 && _db != null)
                    {
                        LivingStats npc = _db.MakeNpcLiving(pveNpcId);
                        npc.Team = 2;
                        room.Hp[i] = npc.Hp;
                        room.MaxHp[i] = npc.MaxHp;
                        room.Livings[i] = npc;
                        if (_db.DefaultBallId(7001) > 0)
                        {
                            room.Balls[i] = _db.GetBall(_db.DefaultBallId(7001));
                        }
                    }
                    else
                    {
                        room.Hp[i] = 1200;
                        room.MaxHp[i] = 1200;
                        room.Livings[i] = new LivingStats
                        {
                            Attack = 110, Defence = 85, Agility = 70, Luck = 40,
                            Hp = 1200, MaxHp = 1200, Team = team
                        };
                    }
                }
                room.Rng = new System.Random(seed);
                room.BattleStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                room.TurnStartMs = room.BattleStartMs;
                room.CurrentPlayer = 0;
                room.CurrentPropMask = GeneratePropMask(room);
                room.CraterHistory.Clear();

                room.Map = null;
                if (_loader != null)
                {
                    string mapPath = GamePaths.MapCollision(mapId);
                    if (_loader.TryReadBytes(mapPath, out byte[] mapBytes))
                    {
                        try { room.Map = MapCollision.Load(mapBytes); }
                        catch { }
                    }
                }

                int mapW = room.Map != null ? room.Map.Width : 1250;
                // Spread positions evenly across the map
                for (int i = 0; i < n; i++)
                {
                    float frac = n <= 1 ? 0.1f : (float)i / (n - 1);
                    int px = Mathf.RoundToInt(Mathf.Lerp(140f, mapW - 160f, frac));
                    room.PosX[i] = px;
                    room.PosY[i] = room.Map != null ? room.Map.FindStandY(Mathf.Clamp(px, 0, mapW - 1), 0) : 0f;
                    room.Facing[i] = frac < 0.5f ? 1 : -1;
                }
            }

            // Build dynamic FightStart JSON with per-player stats
            var sb = new StringBuilder("{\"map\":").Append(mapId)
                .Append(",\"seed\":").Append(seed)
                .Append(",\"wind\":").Append(room.Wind)
                .Append(",\"playerCount\":").Append(n);
            for (int i = 0; i < n; i++)
            {
                string p = "p" + i + "_";
                var ls = room.Livings[i];
                sb.Append(",\"").Append(p).Append("atk\":").Append(ls.Attack);
                sb.Append(",\"").Append(p).Append("def\":").Append(ls.Defence);
                sb.Append(",\"").Append(p).Append("agi\":").Append(ls.Agility);
                sb.Append(",\"").Append(p).Append("luck\":").Append(ls.Luck);
                sb.Append(",\"").Append(p).Append("hp\":").Append(room.Hp[i]);
                sb.Append(",\"").Append(p).Append("maxhp\":").Append(room.MaxHp[i]);
                sb.Append(",\"").Append(p).Append("team\":").Append(ls.Team);

                // Weapon/ball + appearance
                int wid = 7001, ballId = 0;
                int sex = 1, level = 20;
                int equipHead = 0, equipHair = 0, equipFace = 0, equipCloth = 0, equipGlass = 0, equipWeapon = 7001;
                int petId = 0, titleId = 0;
                string nick = "Player";
                lock (_lock)
                {
                    if (i < room.PlayerIds.Count && _players.TryGetValue(room.PlayerIds[i], out ServerPlayer sp))
                    {
                        wid = sp.WeaponId;
                        ballId = sp.PreferredBallId;
                        sex = sp.Sex;
                        level = sp.Level;
                        equipHead = sp.EquipHead;
                        equipHair = sp.EquipHair;
                        equipFace = sp.EquipFace;
                        equipCloth = sp.EquipCloth;
                        equipGlass = sp.EquipGlass;
                        equipWeapon = sp.EquipWeapon;
                        petId = sp.PetId;
                        titleId = sp.TitleId;
                        nick = sp.Nick ?? "Player";
                    }
                }
                sb.Append(",\"").Append(p).Append("weaponId\":").Append(wid);
                sb.Append(",\"").Append(p).Append("preferredBallId\":").Append(ballId);
                sb.Append(",\"").Append(p).Append("sex\":").Append(sex);
                sb.Append(",\"").Append(p).Append("level\":").Append(level);
                sb.Append(",\"").Append(p).Append("equipHead\":").Append(equipHead);
                sb.Append(",\"").Append(p).Append("equipHair\":").Append(equipHair);
                sb.Append(",\"").Append(p).Append("equipFace\":").Append(equipFace);
                sb.Append(",\"").Append(p).Append("equipCloth\":").Append(equipCloth);
                sb.Append(",\"").Append(p).Append("equipGlass\":").Append(equipGlass);
                sb.Append(",\"").Append(p).Append("equipWeapon\":").Append(equipWeapon);
                sb.Append(",\"").Append(p).Append("petId\":").Append(petId);
                sb.Append(",\"").Append(p).Append("titleId\":").Append(titleId);
                sb.Append(",\"").Append(p).Append("nick\":\"").Append((nick ?? "Player").Replace("\"", "")).Append("\"");
                int seatPetMp = room.PetMp != null && i < room.PetMp.Length ? room.PetMp[i] : 100;
                sb.Append(",\"").Append(p).Append("petMp\":").Append(seatPetMp);
                if (i == room.NpcSeat && room.PveNpcId > 0)
                {
                    sb.Append(",\"").Append(p).Append("npcId\":").Append(room.PveNpcId);
                }
            }
            sb.Append("}");
            string startJson = sb.ToString();
            lock (_lock)
            {
                room.LastFightStartJson = startJson;
            }
            BroadcastToRoom(room, PhoneMsg.FightStart, startJson, -1);

            // Push current turn available props to clients.
            string propJson = "{\"player\":" + room.CurrentPlayer + ",\"mask\":" + room.CurrentPropMask + "}";
            BroadcastToRoom(room, PhoneMsg.FightProp, propJson, -1);
            if (BattleDebug)
            {
                Debug.Log($"[Battle] FightStart room={room.Id} curPlayer={room.CurrentPlayer} propMask={room.CurrentPropMask}");
            }

            ScheduleNpcTurnIfNeeded(room);
        }

        void HandleFightDamage(ServerPlayer player, GameRoom room, string json)
        {
            int target = JI(json, "target", -1);
            int dmg = JI(json, "dmg", 0);
            dmg = Mathf.Clamp(dmg, 0, 9999);
            bool gameOver = false;
            lock (_lock)
            {
                if (target >= 0 && target < room.Hp.Length)
                {
                    room.Hp[target] = Mathf.Max(0, room.Hp[target] - dmg);
                    if (room.Hp[target] <= 0)
                    {
                        int alive = 0;
                        for (int i = 0; i < room.Hp.Length; i++)
                            if (room.Hp[i] > 0) alive++;
                        gameOver = alive <= 1;
                    }
                }
            }
            BroadcastToRoom(room, PhoneMsg.FightDamage, json, -1);
            if (gameOver)
            {
                AdvanceTurn(room);
            }
        }

        void ApplyPropModifiers(int propId, out float dmgMul, out float radiusMul, out float powerAdd, out bool forceCrit)
        {
            if (_db != null)
            {
                _db.ApplyFightProp(propId, out dmgMul, out radiusMul, out powerAdd, out forceCrit);
                return;
            }

            dmgMul = 1f; radiusMul = 1f; powerAdd = 0f; forceCrit = false;
            switch (propId)
            {
                case 1: dmgMul = 1.25f; radiusMul = 1.35f; break;
                case 2: dmgMul = 1.2f; break;
                case 5: powerAdd = 12f; break;
                case 6: dmgMul = 1.4f; break;
                case 7: forceCrit = true; break;
            }
        }

        int PropBitIndex(int propId) => GameDatabase.FightPropBitIndex(propId);

        int GeneratePropMask(GameRoom room)
        {
            if (_db != null)
            {
                lock (_lock)
                {
                    return _db.GenerateFightPropMask(room.Rng);
                }
            }

            int mask = 0;
            int[] pool = new int[] { 1, 2, 4, 5, 6, 7 };
            for (int i = 0; i < 3; i++)
            {
                int j = room.Rng != null ? room.Rng.Next(i, pool.Length) : i;
                int tmp = pool[i];
                pool[i] = pool[j];
                pool[j] = tmp;
                int bit = PropBitIndex(pool[i]);
                if (bit >= 0) mask |= 1 << bit;
            }
            return mask;
        }

        void ServerSimulateFire(ServerPlayer player, GameRoom room, string json)
        {
            int who = player.Seat;
            float angle = JF(json, "angle", 45f);
            float power = JF(json, "power", 50f);
            int facing = JI(json, "facing", room.Facing[who]);
            int propId = JI(json, "prop", 0);
            int rawPropId = propId;
            bool specialShot = JI(json, "special", 0) != 0;

            // Server validates propId based on the props available for the current turn player.
            // If not available, treat as no-prop (propId=0).
            int propMask;
            lock (_lock) { propMask = room.CurrentPropMask; }
            if (propId != 0)
            {
                int bit = PropBitIndex(propId);
                if (bit < 0 || (propMask & (1 << bit)) == 0)
                {
                    propId = 0;
                }
            }

            if (BattleDebug)
            {
                Debug.Log($"[Battle] Fire seat={who} turn={room.CurrentTurn} propMask={propMask} rawProp={rawPropId} usedProp={propId}");
            }

            ApplyPropModifiers(propId, out float propDmg, out float propRadius, out float propPower, out bool propCrit);
            power = Mathf.Clamp(power + propPower, 1f, 100f);
            bool armorPierce = _db != null && _db.PropIgnoresArmour(propId);

            MapCollision map;
            BallPhysics ball;
            float wind;
            float startX, startY;
            LivingStats[] livings;
            float[] posX, posY;
            int[] hp;
            lock (_lock)
            {
                map = room.Map;
                if (_db != null)
                {
                    ball = specialShot
                        ? _db.ResolveSpecialBall(player.WeaponId)
                        : _db.ResolveBallForShot(player.WeaponId, player.PreferredBallId, propId);
                }
                else
                {
                    ball = (room.Balls != null && who < room.Balls.Length) ? room.Balls[who] : BallPhysics.Default;
                }
                wind = room.Wind;
                startX = room.PosX[who];
                startY = room.PosY[who];
                livings = (LivingStats[])room.Livings.Clone();
                posX = (float[])room.PosX.Clone();
                posY = (float[])room.PosY.Clone();
                hp = (int[])room.Hp.Clone();
                room.Facing[who] = facing >= 0 ? 1 : -1;
            }

            if (map == null) return;

            var sim = new ProjectileSimulator();
            sim.ApplyBall(ball);

            int mapH = map.Height;
            int mapW = map.Width;
            float unityY = mapH - startY - 18f;

            int shotCount = Mathf.Max(1, ball.Amount);
            int blastRadius = Mathf.Max(20, Mathf.RoundToInt(ball.Radii * propRadius));

            for (int s = 0; s < shotCount; s++)
            {
                float spreadX = s == 0 ? 0f : (room.Rng != null ? (float)(room.Rng.NextDouble() * 16.0 - 8.0) : 0f);
                float spreadA = s == 0 ? 0f : (room.Rng != null ? (float)(room.Rng.NextDouble() * 10.0 - 5.0) : 0f);
                float spreadP = s == 0 ? 0f : (room.Rng != null ? (float)(room.Rng.NextDouble() * 12.0 - 6.0) : 0f);
                var pathSamples = new List<int>();

                var state = sim.FlyUntilSampled(
                    sim.Launch(startX + spreadX, unityY, angle + spreadA, Mathf.Clamp(power + spreadP, 1f, 100f), facing >= 0 ? 1 : -1),
                    wind,
                    (fx, fy) =>
                    {
                        int mx = Mathf.RoundToInt(fx);
                        int my = mapH - 1 - Mathf.RoundToInt(fy);
                        return map.IsSolid(mx, my);
                    },
                    (fx, fy) =>
                    {
                        int mx = Mathf.RoundToInt(fx);
                        int my = mapH - 1 - Mathf.RoundToInt(fy);
                        return mx < -200 || mx > mapW + 200 || my > mapH + 200;
                    },
                    mapH,
                    pathSamples,
                    4,
                    12f);

                int hitMapX = Mathf.RoundToInt(state.X);
                int hitMapY = mapH - 1 - Mathf.RoundToInt(state.Y);

                int cutRadius = Mathf.Max(24, Mathf.RoundToInt((ball.Radii > 0 ? ball.Radii / 2f : 38f) * propRadius));
                bool lastShot = s >= shotCount - 1;
                var pathSb = new StringBuilder(128);
                pathSb.Append("[");
                for (int pi = 0; pi < pathSamples.Count; pi++)
                {
                    if (pi > 0) pathSb.Append(",");
                    pathSb.Append(pathSamples[pi]);
                }
                pathSb.Append("]");
                string shotJson = "{\"who\":" + who + ",\"shot\":" + s + ",\"x\":" + hitMapX +
                                  ",\"y\":" + hitMapY + ",\"r\":" + cutRadius +
                                  ",\"blast\":" + blastRadius + ",\"total\":" + shotCount +
                                  ",\"done\":" + (lastShot ? "true" : "false") +
                                  ",\"path\":" + pathSb + "}";
                BroadcastToRoom(room, PhoneMsg.FightShotResult, shotJson, -1);

                map.CutCircle(hitMapX, hitMapY, cutRadius);
                string craterJson = "{\"x\":" + hitMapX +
                                    ",\"y\":" + hitMapY +
                                    ",\"r\":" + cutRadius + "}";
                lock (_lock)
                {
                    room.CraterHistory.Add(craterJson);
                }
                BroadcastToRoom(room, PhoneMsg.FightCrater, craterJson, -1);

                int bombHurt = _db != null
                    ? _db.ComputeBombHurt(ball, propDmg)
                    : DamageCalculator.ComputeBombHurt(ball, propDmg);
                bool healBall = GameDatabase.BallIsHeal(ball);

                for (int t = 0; t < hp.Length; t++)
                {
                    if (hp[t] <= 0) continue;
                    float tx = posX[t];
                    float ty = posY[t];
                    float dx = hitMapX - tx;
                    float dy = hitMapY - ty;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > blastRadius) continue;

                    if (healBall)
                    {
                        if (livings[t].Team != livings[who].Team)
                        {
                            continue;
                        }

                        int heal = DamageCalculator.ComputeHeal(bombHurt, dist, blastRadius);
                        int newHp;
                        lock (_lock)
                        {
                            room.Hp[t] = Mathf.Min(livings[t].MaxHp, room.Hp[t] + heal);
                            newHp = room.Hp[t];
                            if (room.Livings != null && t < room.Livings.Length)
                            {
                                var ls = room.Livings[t];
                                ls.Hp = newHp;
                                room.Livings[t] = ls;
                            }
                        }
                        hp[t] = newHp;

                        string healJson = "{\"target\":" + t + ",\"heal\":" + heal + "}";
                        BroadcastToRoom(room, PhoneMsg.FightDamage, healJson, -1);
                        continue;
                    }

                    bool crit = propCrit || DamageCalculator.RollCrit(livings[who].Luck, who + (room.CurrentTurn + s));
                    LivingStats atk = EffectiveLiving(room, livings, who);
                    LivingStats defLiving = EffectiveLiving(room, livings, t);
                    BattleDamageMods atkMods = room.Effects.GetOutgoingMods(who);
                    BattleDamageMods defMods = room.Effects.GetMods(t);
                    int dmg = DamageCalculator.Compute(atk, defLiving, bombHurt, dist, crit, armorPierce, atkMods, defMods, blastRadius);
                    dmg = Mathf.Clamp(dmg, 0, hp[t]);

                    lock (_lock)
                    {
                        room.Hp[t] = Mathf.Max(0, room.Hp[t] - dmg);
                        if (room.Livings != null && t < room.Livings.Length)
                        {
                            var ls = room.Livings[t];
                            ls.Hp = room.Hp[t];
                            room.Livings[t] = ls;
                        }
                    }
                    hp[t] = Mathf.Max(0, hp[t] - dmg);

                    string dmgJson = "{\"target\":" + t + ",\"dmg\":" + dmg + ",\"crit\":" + (crit ? "true" : "false") + "}";
                    BroadcastToRoom(room, PhoneMsg.FightDamage, dmgJson, -1);
                }
            }

            ApplyPetFollowUp(player, room, who, livings, hp, posX, posY, ball, propDmg);

            bool gameOver;
            lock (_lock)
            {
                gameOver = CountAliveTeams(room) <= 1;
            }
            if (gameOver)
            {
                EndBattle(room);
            }
            else
            {
                AdvanceTurn(room);
            }
        }

        void HandlePetActiveSkill(ServerPlayer player, GameRoom room)
        {
            if (room == null || player == null || !room.InBattle || _db == null)
            {
                return;
            }

            int who = player.Seat;
            if (who != room.CurrentPlayer)
            {
                return;
            }

            PetSkillInfo skill = _db.ResolvePetActiveSkill(player.PetId);
            if (skill == null)
            {
                return;
            }

            int costMp = Mathf.Max(1, skill.CostMp);
            float cdSec = _db.PetSkillCooldownSec(skill);
            lock (_lock)
            {
                if (room.PetMp == null || room.PetSkillCd == null || who >= room.PetMp.Length)
                {
                    return;
                }

                if (room.PetSkillCd[who] > 0f || room.PetMp[who] < costMp)
                {
                    return;
                }

                room.PetMp[who] -= costMp;
                room.PetSkillCd[who] = cdSec;
            }

            LivingStats[] livings;
            int[] hp;
            float[] posX;
            float[] posY;
            lock (_lock)
            {
                if (room.Livings == null || room.Hp == null || who >= room.Livings.Length)
                {
                    return;
                }

                livings = (LivingStats[])room.Livings.Clone();
                hp = (int[])room.Hp.Clone();
                posX = (float[])room.PosX.Clone();
                posY = (float[])room.PosY.Clone();
            }

            BroadcastPetSkillState(room, who);

            if (skill.BallType == 2)
            {
                int pct = Mathf.Max(1, skill.DamagePercent);
                for (int t = 0; t < hp.Length; t++)
                {
                    if (hp[t] <= 0 || livings[t].Team != livings[who].Team)
                    {
                        continue;
                    }

                    int heal = Mathf.Max(1, livings[t].MaxHp * pct / 100);
                    lock (_lock)
                    {
                        room.Hp[t] = Mathf.Min(livings[t].MaxHp, room.Hp[t] + heal);
                        if (room.Livings != null && t < room.Livings.Length)
                        {
                            var ls = room.Livings[t];
                            ls.Hp = room.Hp[t];
                            room.Livings[t] = ls;
                        }
                    }

                    BroadcastToRoom(room, PhoneMsg.FightDamage, "{\"target\":" + t + ",\"heal\":" + heal + "}", -1);
                }

                ApplyRoomPetSkillEffects(room, skill, who, who);
                return;
            }

            BallPhysics ball = _db.PetSkillBall(skill);
            int bombHurt = _db.ComputeBombHurt(ball, 1f);
            bombHurt = Mathf.Max(1, Mathf.RoundToInt(bombHurt * Mathf.Max(100, skill.DamagePercent) / 100f));
            bool forceCrit = _db.PetSkillForceCrit(skill);

            int best = -1;
            float bestDist = float.MaxValue;
            for (int t = 0; t < hp.Length; t++)
            {
                if (t == who || hp[t] <= 0 || livings[t].Team == livings[who].Team)
                {
                    continue;
                }

                float dx = posX[who] - posX[t];
                float dy = posY[who] - posY[t];
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = t;
                }
            }

            if (best < 0)
            {
                return;
            }

            bool crit = forceCrit || DamageCalculator.RollCrit(livings[who].Luck, who + room.CurrentTurn);
            LivingStats atk = EffectiveLiving(room, livings, who);
            LivingStats defLiving = EffectiveLiving(room, livings, best);
            BattleDamageMods atkMods = room.Effects.GetOutgoingMods(who);
            BattleDamageMods defMods = room.Effects.GetMods(best);
            int dmg = DamageCalculator.Compute(atk, defLiving, bombHurt, bestDist * 0.2f, crit, false, atkMods, defMods, 80f);
            dmg = Mathf.Clamp(dmg, 0, hp[best]);
            lock (_lock)
            {
                room.Hp[best] = Mathf.Max(0, room.Hp[best] - dmg);
                if (room.Livings != null && best < room.Livings.Length)
                {
                    var ls = room.Livings[best];
                    ls.Hp = room.Hp[best];
                    room.Livings[best] = ls;
                }
            }

            BroadcastToRoom(room, PhoneMsg.FightDamage,
                "{\"target\":" + best + ",\"dmg\":" + dmg + ",\"crit\":" + (crit ? "true" : "false") + ",\"pet\":true}", -1);

            ApplyRoomPetSkillEffects(room, skill, who, best);

            bool gameOver;
            lock (_lock)
            {
                gameOver = CountAliveTeams(room) <= 1;
            }

            if (gameOver)
            {
                EndBattle(room);
            }
        }

        void TickPetSkillCooldowns(GameRoom room, float dt)
        {
            if (room?.PetSkillCd == null || dt <= 0f)
            {
                return;
            }

            for (int i = 0; i < room.PetSkillCd.Length; i++)
            {
                if (room.PetSkillCd[i] > 0f)
                {
                    room.PetSkillCd[i] = Mathf.Max(0f, room.PetSkillCd[i] - dt);
                }
            }
        }

        void BroadcastPetSkillState(GameRoom room, int who)
        {
            if (room?.PetMp == null || room.PetSkillCd == null || who < 0 || who >= room.PetMp.Length)
            {
                return;
            }

            int cdSec = Mathf.CeilToInt(room.PetSkillCd[who]);
            string json = "{\"who\":" + who +
                            ",\"mp\":" + room.PetMp[who] +
                            ",\"cd\":" + cdSec + "}";
            BroadcastToRoom(room, PhoneMsg.FightPetSkill, json, -1);
        }

        void ApplyPetFollowUp(ServerPlayer shooter, GameRoom room, int who, LivingStats[] livings, int[] hp, float[] posX, float[] posY, BallPhysics ball, float propDmg)
        {
            if (_db == null || shooter == null || livings == null || hp == null || who < 0 || who >= livings.Length)
            {
                return;
            }

            PetSkillInfo skill = _db.ResolvePetPassiveSkill(shooter.PetId);
            if (skill == null || skill.BallType != 3 || skill.DamagePercent <= 0)
            {
                return;
            }

            if (!_db.RollPetSkill(skill, who + room.CurrentTurn))
            {
                return;
            }

            int bombHurt = _db.ComputeBombHurt(ball, propDmg);
            bombHurt = Mathf.Max(1, Mathf.RoundToInt(bombHurt * skill.DamagePercent / 100f));
            float blastRadius = Mathf.Max(20, ball.Radii);

            for (int t = 0; t < hp.Length; t++)
            {
                if (t == who || hp[t] <= 0 || livings[t].Team == livings[who].Team)
                {
                    continue;
                }

                float dx = posX[who] - posX[t];
                float dy = posY[who] - posY[t];
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                int dmg = DamageCalculator.Compute(livings[who], livings[t], bombHurt, dist * 0.35f, false, false, default, default, blastRadius);
                dmg = Mathf.Clamp(dmg, 0, hp[t]);
                if (dmg <= 0)
                {
                    continue;
                }

                lock (_lock)
                {
                    room.Hp[t] = Mathf.Max(0, room.Hp[t] - dmg);
                    if (room.Livings != null && t < room.Livings.Length)
                    {
                        var ls = room.Livings[t];
                        ls.Hp = room.Hp[t];
                        room.Livings[t] = ls;
                    }
                }
                hp[t] = Mathf.Max(0, hp[t] - dmg);
                string dmgJson = "{\"target\":" + t + ",\"dmg\":" + dmg + ",\"crit\":false,\"pet\":true}";
                BroadcastToRoom(room, PhoneMsg.FightDamage, dmgJson, -1);
            }
        }

        static int ReadSuicideTimeSec(ResLoader loader)
        {
            if (loader == null)
            {
                return 120;
            }

            try
            {
                if (loader.TryReadBytes(GamePaths.PathCombine("Flash", "config.xml"), out byte[] bytes))
                {
                    return FlashConfig.Load(ZlibXml.Load(bytes)).SuicideTime;
                }
            }
            catch { }

            return 120;
        }

        void EndBattle(GameRoom room)
        {
            if (room == null) return;

            List<(ServerPlayer player, int gold, bool win, int questGold)> payouts = null;
            HashSet<int> aliveTeams = null;
            lock (_lock)
            {
                if (!room.InBattle) return;
                room.InBattle = false;
                room.PveNpcId = 0;
                room.NpcSeat = -1;

                aliveTeams = new HashSet<int>();
                if (room.Hp != null && room.Livings != null)
                {
                    for (int i = 0; i < room.Hp.Length; i++)
                    {
                        if (room.Hp[i] > 0 && i < room.Livings.Length)
                        {
                            aliveTeams.Add(room.Livings[i].Team);
                        }
                    }
                }

                payouts = new List<(ServerPlayer, int, bool, int)>();
                int n = room.Hp != null ? room.Hp.Length : 0;
                room.LastFightGolds = n > 0 ? new int[n] : null;
                room.LastFightWins = n > 0 ? new bool[n] : null;
                room.LastFightQuestGolds = n > 0 ? new int[n] : null;
                foreach (int pid in room.PlayerIds)
                {
                    if (!_players.TryGetValue(pid, out ServerPlayer p)) continue;

                    int seat = p.Seat;
                    int myTeam = seat >= 0 && room.Livings != null && seat < room.Livings.Length
                        ? room.Livings[seat].Team
                        : 1;
                    bool win = aliveTeams.Count == 1 && aliveTeams.Contains(myTeam);
                    bool pve = p.PveNpcId != 0 || room.PveNpcId != 0;
                    int winGold = _db != null ? _db.BattleWinGold() : 486;
                    int loseGold = _db != null ? _db.BattleLoseGold() : 48;
                    int gold = win ? winGold : loseGold;
                    int questGold = 0;
                    int pveNpcId = p.PveNpcId != 0 ? p.PveNpcId : room.PveNpcId;

                    if (win && p.PveRewardGold > 0)
                    {
                        gold += p.PveRewardGold;
                    }
                    if (win && p.PveLabyrinth)
                    {
                        p.LabyrinthFloor++;
                    }

                    p.PveNpcId = 0;
                    p.PveRewardGold = 0;
                    p.PveLabyrinth = false;

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
                    }
                    else
                    {
                        p.Lose++;
                        p.Gold += gold;
                    }

                    p.RecalcStats(_db);
                    SavePlayer(p);
                    int totalGold = gold + questGold;
                    payouts.Add((p, totalGold, win, questGold));

                    if (room.LastFightGolds != null && seat >= 0 && seat < room.LastFightGolds.Length)
                    {
                        room.LastFightGolds[seat] = totalGold;
                        room.LastFightWins[seat] = win;
                        if (room.LastFightQuestGolds != null && seat < room.LastFightQuestGolds.Length)
                        {
                            room.LastFightQuestGolds[seat] = questGold;
                        }
                    }
                }
            }

            if (payouts == null) return;
            foreach (var item in payouts)
            {
                SendFightTo(item.player, PhoneMsg.FightReward,
                    "{\"gold\":" + item.gold +
                    ",\"questGold\":" + item.questGold +
                    ",\"win\":" + (item.win ? "1" : "0") + "}");
                SendTo(item.player, PhoneMsg.ProfileData, item.player.ToJson());
            }
        }

        bool PlayerTeamWon(GameRoom room, ServerPlayer player)
        {
            if (room == null || player == null || room.Hp == null || room.Livings == null) return false;

            var aliveTeams = new HashSet<int>();
            for (int i = 0; i < room.Hp.Length; i++)
            {
                if (room.Hp[i] > 0 && i < room.Livings.Length)
                {
                    aliveTeams.Add(room.Livings[i].Team);
                }
            }

            int seat = player.Seat;
            int myTeam = seat >= 0 && seat < room.Livings.Length ? room.Livings[seat].Team : 1;
            return aliveTeams.Count == 1 && aliveTeams.Contains(myTeam);
        }

        void ResendFightReward(ServerPlayer player, GameRoom room)
        {
            if (player == null || room == null) return;
            int seat = player.Seat;
            bool win;
            int gold;
            int questGold = 0;

            if (room.LastFightGolds != null && room.LastFightWins != null &&
                seat >= 0 && seat < room.LastFightGolds.Length && seat < room.LastFightWins.Length)
            {
                gold = room.LastFightGolds[seat];
                win = room.LastFightWins[seat];
                if (room.LastFightQuestGolds != null && seat < room.LastFightQuestGolds.Length)
                {
                    questGold = room.LastFightQuestGolds[seat];
                }
            }
            else
            {
                win = PlayerTeamWon(room, player);
                gold = win ? 800 : 100;
            }
            SendFightTo(player, PhoneMsg.FightReward,
                "{\"gold\":" + gold + ",\"questGold\":" + questGold + ",\"win\":" + (win ? "1" : "0") + "}");
        }

        int CountAliveTeams(GameRoom room)
        {
            var teams = new HashSet<int>();
            for (int i = 0; i < room.Hp.Length; i++)
            {
                if (room.Hp[i] > 0 && room.Livings != null && i < room.Livings.Length)
                    teams.Add(room.Livings[i].Team);
            }
            return teams.Count;
        }

        void AdvanceTurnFromTimeout(GameRoom room)
        {
            if (room == null)
            {
                return;
            }

            int skipped = room.CurrentPlayer;
            string skipJson = "{\"who\":" + skipped + ",\"reason\":\"timeout\"}";
            BroadcastToRoom(room, PhoneMsg.FightSkip, skipJson, -1);
            AdvanceTurn(room);
        }

        void AdvanceTurn(GameRoom room)
        {
            bool ended = false;
            List<(int seat, int heal, int dmg)> turnPulses = null;
            lock (_lock)
            {
                if (CountAliveTeams(room) <= 1)
                {
                    ended = true;
                }
                else
                {
                    turnPulses = TickRoomTurnEffectsLocked(room);
                    room.CurrentTurn++;
                    int n = room.Hp.Length;
                    for (int j = 1; j <= n; j++)
                    {
                        int idx = (room.CurrentPlayer + j) % n;
                        if (room.Hp[idx] > 0)
                        {
                            room.CurrentPlayer = idx;
                            break;
                        }
                    }
                    room.Wind = room.Rng != null ? room.Rng.Next(-3, 4) * 10 : 0;
                    room.TurnStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    room.CurrentPropMask = GeneratePropMask(room);
                }
            }

            if (turnPulses != null)
            {
                BroadcastTurnEffectPulses(room, turnPulses);
            }

            if (ended)
            {
                EndBattle(room);
                return;
            }

            string turnJson = BuildTurnJson(room);
            BroadcastToRoom(room, PhoneMsg.FightTurn, turnJson, -1);

            string propJson = "{\"player\":" + room.CurrentPlayer + ",\"mask\":" + room.CurrentPropMask + "}";
            BroadcastToRoom(room, PhoneMsg.FightProp, propJson, -1);
            if (BattleDebug)
            {
                Debug.Log($"[Battle] FightTurn room={room.Id} turn={room.CurrentTurn} curPlayer={room.CurrentPlayer} propMask={room.CurrentPropMask}");
            }

            ScheduleNpcTurnIfNeeded(room);
        }

        List<(int seat, int heal, int dmg)> TickRoomTurnEffectsLocked(GameRoom room)
        {
            if (room?.Effects == null || room.Livings == null || room.Hp == null)
            {
                return null;
            }

            var livings = (LivingStats[])room.Livings.Clone();
            var hp = (int[])room.Hp.Clone();
            List<(int, int, int)> pulses = room.Effects.TickTurn(livings, hp);
            for (int i = 0; i < hp.Length && i < room.Hp.Length; i++)
            {
                room.Hp[i] = hp[i];
                if (i < room.Livings.Length)
                {
                    LivingStats ls = room.Livings[i];
                    ls.Hp = hp[i];
                    room.Livings[i] = ls;
                }
            }

            return pulses;
        }

        void BroadcastTurnEffectPulses(GameRoom room, List<(int seat, int heal, int dmg)> pulses)
        {
            if (room == null || pulses == null)
            {
                return;
            }

            foreach ((int seat, int heal, int dmg) in pulses)
            {
                if (heal > 0)
                {
                    BroadcastToRoom(room, PhoneMsg.FightDamage, "{\"target\":" + seat + ",\"heal\":" + heal + "}", -1);
                }
                else if (dmg > 0)
                {
                    BroadcastToRoom(room, PhoneMsg.FightDamage,
                        "{\"target\":" + seat + ",\"dmg\":" + dmg + ",\"crit\":false,\"dot\":true}", -1);
                }
            }
        }

        void ApplyRoomPetSkillEffects(GameRoom room, PetSkillInfo skill, int sourceSeat, int targetSeat)
        {
            if (room == null || skill == null || _db == null)
            {
                return;
            }

            List<BattleEffect> effects = _db.BuildPetSkillEffects(skill, sourceSeat, targetSeat);
            if (effects == null || effects.Count == 0)
            {
                return;
            }

            lock (_lock)
            {
                room.Effects.AddRange(effects);
            }
        }

        LivingStats EffectiveLiving(GameRoom room, LivingStats[] livings, int seat)
        {
            if (room?.Effects == null || livings == null || seat < 0 || seat >= livings.Length)
            {
                return seat >= 0 && seat < livings.Length ? livings[seat] : default;
            }

            return room.Effects.ApplyDefence(livings[seat], seat);
        }

        void ScheduleNpcTurnIfNeeded(GameRoom room)
        {
            if (room == null) return;
            int npcSeat;
            lock (_lock)
            {
                if (!room.InBattle || room.NpcSeat < 0 || room.CurrentPlayer != room.NpcSeat)
                {
                    return;
                }

                if (room.Hp == null || room.NpcSeat >= room.Hp.Length || room.Hp[room.NpcSeat] <= 0)
                {
                    return;
                }

                npcSeat = room.NpcSeat;
            }

            int roomId = room.Id;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(900);
                GameRoom liveRoom;
                lock (_lock)
                {
                    if (!_rooms.TryGetValue(roomId, out liveRoom) || liveRoom == null || !liveRoom.InBattle)
                    {
                        return;
                    }

                    if (liveRoom.CurrentPlayer != npcSeat || liveRoom.Hp[npcSeat] <= 0)
                    {
                        return;
                    }
                }

                AutoNpcFire(liveRoom, npcSeat);
            });
        }

        void AutoNpcFire(GameRoom room, int npcSeat)
        {
            if (room == null || room.Hp == null || room.PosX == null || room.PosY == null || room.Livings == null)
            {
                return;
            }

            int npcTeam = npcSeat < room.Livings.Length ? room.Livings[npcSeat].Team : 2;
            int target = -1;
            float bestDist = float.MaxValue;
            float npcX;
            float npcY;
            float wind;
            int facing;
            int propMask;
            lock (_lock)
            {
                if (!room.InBattle || room.CurrentPlayer != npcSeat || room.Hp[npcSeat] <= 0)
                {
                    return;
                }

                npcX = room.PosX[npcSeat];
                npcY = room.PosY[npcSeat];
                wind = room.Wind;
                facing = room.Facing[npcSeat];
                propMask = room.CurrentPropMask;
                for (int i = 0; i < room.Hp.Length; i++)
                {
                    if (i == npcSeat || room.Hp[i] <= 0 || i >= room.Livings.Length)
                    {
                        continue;
                    }

                    if (room.Livings[i].Team == npcTeam)
                    {
                        continue;
                    }

                    float dx = room.PosX[i] - npcX;
                    float dy = room.PosY[i] - npcY;
                    float dist = dx * dx + dy * dy;
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        target = i;
                    }
                }
            }

            if (target < 0)
            {
                return;
            }

            float tx;
            float ty;
            lock (_lock)
            {
                tx = room.PosX[target];
                ty = room.PosY[target];
            }

            float dxShot = tx - npcX;
            float dyShot = ty - npcY;
            if (Mathf.Abs(dxShot) < 1f)
            {
                dxShot = facing >= 0 ? 1f : -1f;
            }

            float angle = Mathf.Clamp(Mathf.Atan2(-dyShot, dxShot) * Mathf.Rad2Deg, 5f, 85f);
            float distShot = Mathf.Sqrt(dxShot * dxShot + dyShot * dyShot);
            float power = Mathf.Clamp(distShot / 12f + 18f + wind * 0.02f, 20f, 90f);

            int propId = 0;
            if (propMask != 0 && _db != null)
            {
                foreach (int pic in GameDatabase.BattlePropPicIds)
                {
                    int bit = GameDatabase.FightPropBitIndex(pic);
                    if (bit >= 0 && (propMask & (1 << bit)) != 0 && (pic == 2 || pic == 6 || pic == 7))
                    {
                        propId = pic;
                        break;
                    }
                }
            }

            string fireJson = "{\"who\":" + npcSeat +
                              ",\"angle\":" + angle.ToString(CultureInfo.InvariantCulture) +
                              ",\"power\":" + power.ToString(CultureInfo.InvariantCulture) +
                              ",\"facing\":" + (dxShot >= 0 ? 1 : -1) +
                              ",\"prop\":" + propId + ",\"special\":0}";
            var npcPlayer = new ServerPlayer { Seat = npcSeat, Id = -1 };
            BroadcastToRoom(room, PhoneMsg.FightFire, fireJson, -1);
            ServerSimulateFire(npcPlayer, room, fireJson);
        }

        void HandleFightOver(ServerPlayer player, GameRoom room, string json)
        {
            if (room == null || player == null) return;

            bool endedNow = false;
            lock (_lock)
            {
                if (room.InBattle)
                {
                    endedNow = true;
                }
            }

            if (endedNow)
            {
                EndBattle(room);
                return;
            }

            // Battle already ended server-side — resend reward for late/duplicate client ack.
            ResendFightReward(player, room);
        }

        void BroadcastToRoom(GameRoom room, ushort id, string json, int excludePlayerId)
        {
            lock (_lock)
            {
                foreach (int pid in room.PlayerIds)
                {
                    if (pid == excludePlayerId) continue;
                    if (_players.TryGetValue(pid, out ServerPlayer p))
                        SendFightTo(p, id, json);
                }
            }
        }

        ServerPlayer LoadOrCreate(string nick)
        {
            foreach (var p in _players.Values)
            {
                if (string.Equals(p.Nick, nick, StringComparison.OrdinalIgnoreCase))
                    return p;
            }

            string file = Path.Combine(_savePath, SanitizeFileName(nick) + ".json");
            if (File.Exists(file))
            {
                try
                {
                    var loaded = JsonUtility.FromJson<ServerPlayerSave>(File.ReadAllText(file));
                    if (loaded != null)
                    {
                        var p = FromSave(loaded);
                        p.Id = _nextPlayerId++;
                        _players[p.Id] = p;
                        return p;
                    }
                }
                catch { }
            }

            var fresh = new ServerPlayer
            {
                Id = _nextPlayerId++,
                Nick = nick,
                Bag = new List<BagSlot>
                {
                    new BagSlot { TemplateId = 7001, Count = 1 },
                    new BagSlot { TemplateId = 1102, Count = 1 },
                    new BagSlot { TemplateId = 5102, Count = 1 }
                }
            };
            _players[fresh.Id] = fresh;
            SavePlayer(fresh);
            return fresh;
        }

        void SavePlayer(ServerPlayer p)
        {
            try
            {
                string file = Path.Combine(_savePath, SanitizeFileName(p.Nick) + ".json");
                File.WriteAllText(file, JsonUtility.ToJson(ToSave(p), true));
            }
            catch { }
        }

        static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        static int JI(string json, string key, int fallback)
        {
            return Mathf.RoundToInt(JF(json, key, fallback));
        }

        static string EnsureJsonField(string json, string key, int value)
        {
            if (string.IsNullOrEmpty(json))
            {
                return "{\"" + key + "\":" + value + "}";
            }

            if (json.IndexOf("\"" + key + "\"", StringComparison.Ordinal) >= 0)
            {
                return json;
            }

            string trimmed = json.Trim();
            if (trimmed.StartsWith("{", StringComparison.Ordinal) && trimmed.EndsWith("}", StringComparison.Ordinal))
            {
                return "{\"" + key + "\":" + value + "," + trimmed.Substring(1);
            }

            return json;
        }

        static float JF(string json, string key, float fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, StringComparison.Ordinal);
            if (i < 0) return fallback;
            int s = i + needle.Length;
            int e = s;
            while (e < json.Length && (json[e] == '-' || json[e] == '.' || (json[e] >= '0' && json[e] <= '9'))) e++;
            if (float.TryParse(json.Substring(s, e - s), NumberStyles.Float, CultureInfo.InvariantCulture, out float v)) return v;
            return fallback;
        }

        static string JS(string json, string key, string fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":\"";
            int i = json.IndexOf(needle, StringComparison.Ordinal);
            if (i < 0) return fallback;
            int s = i + needle.Length;
            int e = json.IndexOf('"', s);
            return e > s ? json.Substring(s, e - s) : fallback;
        }

        [Serializable]
        class FightSpiritSlotSave { public int spiritId; public int level; }

        class MagicStoneSlotSave { public int templateId; public int level; }
        class EmblemSlotSave { public int id, templateId, types, profile, mainType, mainValue, subValue, skillId, equipped; }
        class SoulStampSlotSave { public int id, tempId, type, quality, grade, proType, proValue, skillId, equipped; }

        [Serializable]
        class ServerPlayerSave


        [Serializable]
        class ServerPlayerSave
        {
            public string Nick = "Player";
            public int Sex = 1;
            public int Level = 20;
            public int Gp;
            public int Gold = 100000;
            public int Gift = 5000;
            public int Win, Lose;
            public int WeaponId = 7001;
            public int EquipHead, EquipHair, EquipFace, EquipCloth, EquipGlass, EquipWeapon = 7001;
            public int PetId, CardId, TitleId, TotemId, MountGrade, VipLevel, Honor, Texp;
            public int PreferredBallId, LastSignDay = -1, SignIndex, LabyrinthFloor = 1;
            public string ConsortiaName = "";
            public int ElfId, GemLevel, KingBlessDay = -1, FarmHarvests;
            public int FusionKeys, BankGold, MineDay = -1, MineDigs;
            public int WorldBossDay = -1, WorldBossHits;
            public int NecklaceLevel, HomeTempleLevel;
            public int WardrobeClothId, HonorSystemExp, HonorSystemLevel;
            public int HonorSystemDay = -1, HonorSystemOps;
            public List<int> WardrobeProperties = new List<int>();
            public List<int> HonorSystemClaimed = new List<int>();
            public int RedPacketDay = -1, RedPacketClaims;
            public int DevilTurnDay = -1, DevilTurnSpins;
            public int SweepDay = -1, SweepCount;
            public int GodCardEquipId, EngraveSetId;
            public int NextEmblemId = 1, NextSoulStampId = 1;
            public int NextMailId = 1;
            public List<BagSlotSave> Bag = new List<BagSlotSave>();
            public List<GodCardSlotSave> GodCards = new List<GodCardSlotSave>();
            public List<StockSlotSave> StockHoldings = new List<StockSlotSave>();
            public List<int> AcceptedQuests = new List<int>();
            public List<int> CompletedQuests = new List<int>();
            public List<string> Friends = new List<string>();
            public List<FightSpiritSlotSave> FightSpirits = new List<FightSpiritSlotSave>();
            public List<MagicStoneSlotSave> MagicStones = new List<MagicStoneSlotSave>();
            public List<EmblemSlotSave> Emblems = new List<EmblemSlotSave>();
            public List<SoulStampSlotSave> SoulStamps = new List<SoulStampSlotSave>();
            public List<ServerMailSave> Mails = new List<ServerMailSave>();
        }

        [Serializable]
        class ServerMailSave
        {
            public int Id;
            public string Subject = "";
            public string Body = "";
            public int Gold;
            public int ItemId;
            public int ItemCount;
            public bool Claimed;
        }

        [Serializable]
        class BagSlotSave { public int t; public int c = 1; public int s; }

        [Serializable]
        class GodCardSlotSave { public int id; public int count = 1; }

        [Serializable]
        class StockSlotSave { public int stockId; public int shares; public int avgPrice; }

        static ServerPlayerSave ToSave(ServerPlayer p)
        {
            var s = new ServerPlayerSave
            {
                Nick = p.Nick, Sex = p.Sex, Level = p.Level, Gp = p.Gp, Gold = p.Gold, Gift = p.Gift,
                Win = p.Win, Lose = p.Lose, WeaponId = p.WeaponId,
                EquipHead = p.EquipHead, EquipHair = p.EquipHair, EquipFace = p.EquipFace,
                EquipCloth = p.EquipCloth, EquipGlass = p.EquipGlass, EquipWeapon = p.EquipWeapon,
                PetId = p.PetId, CardId = p.CardId, TitleId = p.TitleId, TotemId = p.TotemId,
                MountGrade = p.MountGrade, VipLevel = p.VipLevel, Honor = p.Honor, Texp = p.Texp,
                PreferredBallId = p.PreferredBallId, LastSignDay = p.LastSignDay, SignIndex = p.SignIndex,
                LabyrinthFloor = p.LabyrinthFloor, ConsortiaName = p.ConsortiaName,
                ElfId = p.ElfId, GemLevel = p.GemLevel, KingBlessDay = p.KingBlessDay, FarmHarvests = p.FarmHarvests,
                FusionKeys = p.FusionKeys, BankGold = p.BankGold, MineDay = p.MineDay, MineDigs = p.MineDigs,
                WorldBossDay = p.WorldBossDay, WorldBossHits = p.WorldBossHits,
                NecklaceLevel = p.NecklaceLevel, HomeTempleLevel = p.HomeTempleLevel,
                WardrobeClothId = p.WardrobeClothId, HonorSystemExp = p.HonorSystemExp,
                HonorSystemLevel = p.HonorSystemLevel, HonorSystemDay = p.HonorSystemDay,
                HonorSystemOps = p.HonorSystemOps,
                WardrobeProperties = p.WardrobeProperties ?? new List<int>(),
                HonorSystemClaimed = p.HonorSystemClaimed ?? new List<int>(),
                RedPacketDay = p.RedPacketDay, RedPacketClaims = p.RedPacketClaims,
                DevilTurnDay = p.DevilTurnDay, DevilTurnSpins = p.DevilTurnSpins,
                SweepDay = p.SweepDay, SweepCount = p.SweepCount,
                GodCardEquipId = p.GodCardEquipId, EngraveSetId = p.EngraveSetId,
                NextEmblemId = p.NextEmblemId, NextSoulStampId = p.NextSoulStampId,
                AcceptedQuests = p.AcceptedQuests, CompletedQuests = p.CompletedQuests,
                Friends = p.Friends, NextMailId = p.NextMailId
            };
            p.EnsureFightSpirits();
            foreach (FightSpiritSlot fs in p.FightSpirits)
            {
                s.FightSpirits.Add(new FightSpiritSlotSave { spiritId = fs.SpiritId, level = fs.Level });
            }
            p.EnsureMagicStones();
            foreach (MagicStoneSlot ms in p.MagicStones)
            {
                s.MagicStones.Add(new MagicStoneSlotSave { templateId = ms.TemplateId, level = ms.Level });
            }
            p.EnsureEmblems(); foreach (EmblemSlot e in p.Emblems) s.Emblems.Add(new EmblemSlotSave { id = e.Id, templateId = e.TemplateId, types = e.Types, profile = e.Profile, mainType = e.MainType, mainValue = e.MainValue, subValue = e.SubValue, skillId = e.SkillId, equipped = e.Equipped });
            p.EnsureSoulStamps(); foreach (SoulStampSlot ss in p.SoulStamps) s.SoulStamps.Add(new SoulStampSlotSave { id = ss.Id, tempId = ss.TempId, type = ss.Type, quality = ss.Quality, grade = ss.Grade, proType = ss.ProType, proValue = ss.ProValue, skillId = ss.SkillId, equipped = ss.Equipped });
            foreach (var b in p.Bag) s.Bag.Add(new BagSlotSave { t = b.TemplateId, c = b.Count, s = b.Strengthen });
            foreach (GodCardSlot g in p.GodCards) s.GodCards.Add(new GodCardSlotSave { id = g.Id, count = g.Count });
            foreach (StockSlot sh in p.StockHoldings) s.StockHoldings.Add(new StockSlotSave { stockId = sh.StockId, shares = sh.Shares, avgPrice = sh.AvgPrice });
            foreach (ServerMail m in p.Mails)
            {
                s.Mails.Add(new ServerMailSave
                {
                    Id = m.Id, Subject = m.Subject, Body = m.Body, Gold = m.Gold,
                    ItemId = m.ItemId, ItemCount = m.ItemCount, Claimed = m.Claimed
                });
            }
            return s;
        }

        static ServerPlayer FromSave(ServerPlayerSave s)
        {
            var p = new ServerPlayer
            {
                Nick = s.Nick, Sex = s.Sex, Level = s.Level, Gp = s.Gp, Gold = s.Gold, Gift = s.Gift,
                Win = s.Win, Lose = s.Lose, WeaponId = s.WeaponId,
                EquipHead = s.EquipHead, EquipHair = s.EquipHair, EquipFace = s.EquipFace,
                EquipCloth = s.EquipCloth, EquipGlass = s.EquipGlass, EquipWeapon = s.EquipWeapon,
                PetId = s.PetId, CardId = s.CardId, TitleId = s.TitleId, TotemId = s.TotemId,
                MountGrade = s.MountGrade, VipLevel = s.VipLevel, Honor = s.Honor, Texp = s.Texp,
                PreferredBallId = s.PreferredBallId, LastSignDay = s.LastSignDay, SignIndex = s.SignIndex,
                LabyrinthFloor = s.LabyrinthFloor, ConsortiaName = s.ConsortiaName,
                ElfId = s.ElfId, GemLevel = s.GemLevel, KingBlessDay = s.KingBlessDay, FarmHarvests = s.FarmHarvests,
                FusionKeys = s.FusionKeys, BankGold = s.BankGold, MineDay = s.MineDay, MineDigs = s.MineDigs,
                WorldBossDay = s.WorldBossDay, WorldBossHits = s.WorldBossHits,
                NecklaceLevel = s.NecklaceLevel, HomeTempleLevel = s.HomeTempleLevel,
                WardrobeClothId = s.WardrobeClothId, HonorSystemExp = s.HonorSystemExp,
                HonorSystemLevel = s.HonorSystemLevel, HonorSystemDay = s.HonorSystemDay,
                HonorSystemOps = s.HonorSystemOps,
                WardrobeProperties = s.WardrobeProperties ?? new List<int>(),
                HonorSystemClaimed = s.HonorSystemClaimed ?? new List<int>(),
                RedPacketDay = s.RedPacketDay, RedPacketClaims = s.RedPacketClaims,
                DevilTurnDay = s.DevilTurnDay, DevilTurnSpins = s.DevilTurnSpins,
                SweepDay = s.SweepDay, SweepCount = s.SweepCount,
                GodCardEquipId = s.GodCardEquipId, EngraveSetId = s.EngraveSetId,
                NextEmblemId = s.NextEmblemId > 0 ? s.NextEmblemId : 1,
                NextSoulStampId = s.NextSoulStampId > 0 ? s.NextSoulStampId : 1,
                AcceptedQuests = s.AcceptedQuests ?? new List<int>(),
                CompletedQuests = s.CompletedQuests ?? new List<int>(),
                Friends = s.Friends ?? new List<string>(),
                NextMailId = s.NextMailId > 0 ? s.NextMailId : 1,
                Mails = new List<ServerMail>()
            };
            if (s.FightSpirits != null)
            {
                foreach (FightSpiritSlotSave fs in s.FightSpirits)
                {
                    p.FightSpirits.Add(new FightSpiritSlot { SpiritId = fs.spiritId, Level = fs.level });
                }
            }

            if (s.MagicStones != null)
            {
                foreach (MagicStoneSlotSave ms in s.MagicStones)
                {
                    p.MagicStones.Add(new MagicStoneSlot { TemplateId = ms.templateId, Level = ms.level });
                }
            }

            p.EnsureFightSpirits();
            p.EnsureMagicStones();
            if (s.Emblems != null) foreach (EmblemSlotSave e in s.Emblems) p.Emblems.Add(new EmblemSlot { Id = e.id, TemplateId = e.templateId, Types = e.types, Profile = e.profile, MainType = e.mainType, MainValue = e.mainValue, SubValue = e.subValue, SkillId = e.skillId, Equipped = e.equipped });
            if (s.SoulStamps != null) foreach (SoulStampSlotSave ss in s.SoulStamps) p.SoulStamps.Add(new SoulStampSlot { Id = ss.id, TempId = ss.tempId, Type = ss.type, Quality = ss.quality, Grade = ss.grade, ProType = ss.proType, ProValue = ss.proValue, SkillId = ss.skillId, Equipped = ss.equipped });
            p.EnsureEmblems(); p.EnsureSoulStamps();
            p.EnsureWardrobeProperties();
            p.EnsureHonorSystemClaimed();
            foreach (var b in s.Bag) p.Bag.Add(new BagSlot { TemplateId = b.t, Count = b.c, Strengthen = b.s });
            if (s.GodCards != null)
            {
                foreach (GodCardSlotSave g in s.GodCards) p.GodCards.Add(new GodCardSlot { Id = g.id, Count = g.count });
            }

            if (s.StockHoldings != null)
            {
                foreach (StockSlotSave sh in s.StockHoldings)
                {
                    p.StockHoldings.Add(new StockSlot { StockId = sh.stockId, Shares = sh.shares, AvgPrice = sh.avgPrice });
                }
            }

            if (s.Mails != null)
            {
                foreach (ServerMailSave m in s.Mails)
                {
                    p.Mails.Add(new ServerMail
                    {
                        Id = m.Id, Subject = m.Subject, Body = m.Body, Gold = m.Gold,
                        ItemId = m.ItemId, ItemCount = m.ItemCount, Claimed = m.Claimed
                    });
                }
            }
            return p;
        }
    }
}
