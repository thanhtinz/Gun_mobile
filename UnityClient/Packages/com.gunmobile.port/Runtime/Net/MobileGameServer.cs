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
        public int MountTalismanId;
        public int ManorGrade = 1;
        public int GoldEquipId;
        public int GloryTemplateId;
        public int SigilQuality = 1;
        public int SigilProType;
        public int SigilProValue;
        public int VipLevel;
        public int Honor;
        public int Texp;
        public int PreferredBallId;
        public int LastSignDay = -1;
        public int SignIndex;
        public int LabyrinthFloor = 1;
        public string ConsortiaName = "";
        public int GuildLevel;
        public int ConsortiaBossDay = -1;
        public int ConsortiaBossHits;
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
        public int HomeTemplePracticeLevel;
        public int HomeTempleAdvanceLevel;
        public List<BankTermDeposit> BankDeposits = new List<BankTermDeposit>();
        public List<int> SweepMissionClears = new List<int>();
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
        public int DevilTurnPoints;
        public List<int> DevilTreasPointClaimed = new List<int>();
        public Dictionary<int, List<int>> QuestProgress = new Dictionary<int, List<int>>();
        public int SpaRoomDay = -1;
        public int SpaRoomDayScore;
        public bool SpaRoomActive;
        public int SpaRoomWidth;
        public int SpaRoomHeight;
        public List<int> SpaRoomMap = new List<int>();
        public List<int> SpaRoomPicked = new List<int>();
        public int SpaRoomScore;
        public int TreasureRoomDay = -1;
        public int TreasureRoomDraws;
        public int ChristmasDay = -1;
        public int ChristmasClaims;
        public int NewYearDay = -1;
        public int NewYearFreeUsed;
        public int NewYearPoints;
        public List<int> NewYearPointClaimed = new List<int>();
        public int WorshipMoonDay = -1;
        public int WorshipMoonDraws;
        public int SuperLuckerDay = -1;
        public int SuperLuckerDraws;
        public int JigsawDay = -1;
        public int JigsawClaims;
        public int BibleDay = -1;
        public int BibleClaims;
        public int SweepDay = -1;
        public int SweepCount;
        public bool FirstRechargeClaimed;
        public Dictionary<int, int> FirstRechargeShopBuys = new Dictionary<int, int>();
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
        public int GodCardPoints;
        public List<int> GodCardPointClaimed = new List<int>();
        public int EngraveSetId;
        public List<StockSlot> StockHoldings = new List<StockSlot>();
        public List<FightSpiritSlot> FightSpirits = new List<FightSpiritSlot>();
        public List<MagicStoneSlot> MagicStones = new List<MagicStoneSlot>();
        public List<EmblemSlot> Emblems = new List<EmblemSlot>();
        public List<SoulStampSlot> SoulStamps = new List<SoulStampSlot>();
        public List<RelicSlot> Relics = new List<RelicSlot>();
        public int ForcesBattleScore;
        public int ForcesBattleDay = -1;
        public int ForcesBattleAttempts;
        public int CultureGrade = 1;
        public int CultureAtk;
        public int CultureDef;
        public int CultureAgi;
        public int CultureLuck;
        public int JampsManualLevel = 1;
        public List<int> JampsDebrisOwned = new List<int>();
        public List<int> JampsPagesCollected = new List<int>();
        public List<int> JampsPagesActivated = new List<int>();
        public int CardMainLevel;
        public List<int> OwnedCardTemplateIds = new List<int>();
        public List<int> CardBookletProfiles = new List<int>();
        public List<int> CardBookletClaimed = new List<int>();
        public int CardSoul;
        public int ElfIntimacyExp;
        public int ElfIntimacyLevel;
        public int ElfIntimacyDay = -1;
        public int ElfIntimacyActions;
        public int NextEmblemId = 1;
        public int NextSoulStampId = 1;
        public int CalendarMonth;
        public List<int> CalendarClaimedDays = new List<int>();
        public int AuditoriumDay = -1;
        public int AuditoriumActions;
        public int BoguAdventureDay = -1;
        public int BoguAdventureActions;
        public int QuizDay = -1;
        public int QuizAttempts;
        public int OneYuanDay = -1;
        public List<int> OneYuanBought = new List<int>();

        public void EnsureBankDeposits() { if (BankDeposits == null) BankDeposits = new List<BankTermDeposit>(); }
        public void EnsureSweepMissionClears() { if (SweepMissionClears == null) SweepMissionClears = new List<int>(); }
        public void EnsureCalendarClaimed() { if (CalendarClaimedDays == null) CalendarClaimedDays = new List<int>(); }
        public void TouchCalendarMonth() { EnsureCalendarClaimed(); int mk = DateTime.Now.Year * 100 + DateTime.Now.Month; if (CalendarMonth != mk) { CalendarMonth = mk; CalendarClaimedDays.Clear(); } }
        public void TouchAuditoriumDay() { int t = DateTime.Now.DayOfYear; if (AuditoriumDay != t) { AuditoriumDay = t; AuditoriumActions = 0; } }
        public void TouchBoguAdventureDay() { int t = DateTime.Now.DayOfYear; if (BoguAdventureDay != t) { BoguAdventureDay = t; BoguAdventureActions = 0; } }
        public void EnsureOneYuanBought() { if (OneYuanBought == null) OneYuanBought = new List<int>(); }
        public void TouchQuizDay() { int t = DateTime.Now.DayOfYear; if (QuizDay != t) { QuizDay = t; QuizAttempts = 0; } }
        public void TouchOneYuanDay() { EnsureOneYuanBought(); int t = DateTime.Now.DayOfYear; if (OneYuanDay != t) { OneYuanDay = t; OneYuanBought.Clear(); } }

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
        public void EnsureRelics() { if (Relics == null) Relics = new List<RelicSlot>(); if (Relics.Count == 0) Relics.Add(new RelicSlot { RelicId = 1, UpgradeLevel = 0 }); }
        public RelicSlot FindRelic(int relicId) { EnsureRelics(); for (int i = 0; i < Relics.Count; i++) if (Relics[i].RelicId == relicId) return Relics[i]; return null; }
        public int GetCultureStatLevel(int statType) { switch (statType) { case 116: return CultureAtk; case 117: return CultureDef; case 118: return CultureAgi; case 119: return CultureLuck; default: return 0; } }
        public void SetCultureStatLevel(int statType, int level) { switch (statType) { case 116: CultureAtk = level; break; case 117: CultureDef = level; break; case 118: CultureAgi = level; break; case 119: CultureLuck = level; break; } }

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

        public void EnsureJampsLists()
        {
            if (JampsDebrisOwned == null) JampsDebrisOwned = new List<int>();
            if (JampsPagesCollected == null) JampsPagesCollected = new List<int>();
            if (JampsPagesActivated == null) JampsPagesActivated = new List<int>();
        }
        public bool HasJampsDebris(int debrisId) { EnsureJampsLists(); return JampsDebrisOwned.Contains(debrisId); }
        public bool HasJampsPageCollected(int pageId) { EnsureJampsLists(); return JampsPagesCollected.Contains(pageId); }
        public bool HasJampsPageActivated(int pageId) { EnsureJampsLists(); return JampsPagesActivated.Contains(pageId); }
        public void EnsureOwnedCards()
        {
            if (OwnedCardTemplateIds == null) OwnedCardTemplateIds = new List<int>();
            if (CardBookletProfiles == null) CardBookletProfiles = new List<int>();
            if (CardBookletClaimed == null) CardBookletClaimed = new List<int>();
            while (CardBookletProfiles.Count < OwnedCardTemplateIds.Count) CardBookletProfiles.Add(0);
        }
        public bool HasCardBookletClaimed(int templateId)
        {
            EnsureOwnedCards();
            return CardBookletClaimed.Contains(templateId);
        }
        public void SetCardBookletProfile(int templateId, int profile)
        {
            EnsureOwnedCards();
            for (int i = 0; i < OwnedCardTemplateIds.Count; i++)
            {
                if (OwnedCardTemplateIds[i] == templateId)
                {
                    while (CardBookletProfiles.Count <= i) CardBookletProfiles.Add(0);
                    CardBookletProfiles[i] = profile;
                    return;
                }
            }
            OwnedCardTemplateIds.Add(templateId);
            CardBookletProfiles.Add(profile);
        }
        public void SyncElfIntimacyLevel(GameDatabase db) { ElfIntimacyLevel = db != null ? db.ElfIntimacyLevelFromExp(ElfIntimacyExp) : 0; }
        public void TouchElfIntimacyDay() { int day = DateTime.UtcNow.DayOfYear; if (ElfIntimacyDay != day) { ElfIntimacyDay = day; ElfIntimacyActions = 0; } }

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

            db.ApplyMountTalismanBonus(MountTalismanId, ref hp);
            db.ApplyGoldEquipBonus(EquipWeapon, ref atk, ref def, ref agi, ref luck, ref hp);
            db.ApplyGloryBonus(GloryTemplateId, ref atk, ref def, ref agi, ref luck, ref hp);

            if (GodCardEquipId > 0 && db.GodCards.TryGetValue(GodCardEquipId, out GodCardInfo gc))
            {
                db.ApplyGodCardBonus(gc, ref atk, ref def, ref agi, ref luck, ref hp);
                GodCardSlot grooveSlot = FindGodCardSlot(GodCardEquipId);
                if (grooveSlot != null)
                    db.ApplyGodCardGrooveBonus(db.GodCardGrooveType(gc), grooveSlot.GrooveLevel, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard);
            }

            db.ApplyEngraveSetBonus(EngraveSetId, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard);

            EnsureFightSpirits();
            db.ApplyFightSpiritStats(FightSpirits, ref atk, ref def, ref agi, ref luck, ref hp);

            EnsureMagicStones();
            int magicAtk = 0;
            int magicDef = 0;
            db.ApplyMagicStoneStats(MagicStones, ref atk, ref def, ref agi, ref luck, ref magicAtk, ref magicDef);
            db.ApplySigilBonus(SigilProType, SigilProValue, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard, ref magicAtk, ref magicDef);
            db.ApplyNecklaceBonus(NecklaceLevel, ref hp, ref def);
            db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);
            db.ApplyHomeTemplePracticeBonus(HomeTemplePracticeLevel, ref atk, ref def, ref agi, ref luck, ref hp, ref magicDef);
            db.ApplyHomeTempleAdvanceBonus(HomeTempleAdvanceLevel, ref hp, ref magicDef, ref def);
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
            EnsureRelics();
            db.ApplyRelicStats(Relics, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref magicAtk, ref magicDef);
            db.ApplyCultureBonus(CultureGrade, CultureAtk, CultureDef, CultureAgi, CultureLuck, ref atk, ref def, ref agi, ref luck, ref hp, ref magicAtk, ref magicDef);
            EnsureJampsLists();
            db.ApplyJampsBonus(JampsManualLevel, JampsPagesCollected, JampsPagesActivated, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard, ref magicAtk, ref magicDef);
            db.ApplyCardMainBonus(CardMainLevel, ref atk, ref def, ref agi, ref luck);
            EnsureOwnedCards();
            db.ApplyCardSuitBonus(OwnedCardTemplateIds, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard);
            db.ApplyCardBookletBonus(OwnedCardTemplateIds, CardBookletProfiles, ref atk, ref def, ref agi, ref luck, ref hp, ref baseDmg, ref baseGuard);
            SyncElfIntimacyLevel(db);
            db.ApplyElfIntimacyBonus(ElfIntimacyLevel, ref atk, ref def, ref hp);
            db.ApplyGuildLevelBonus(GuildLevel, ref atk);

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
            J(sb, "mountTalismanId", MountTalismanId); sb.Append(",");
            J(sb, "manorGrade", ManorGrade); sb.Append(",");
            J(sb, "goldEquipId", GoldEquipId); sb.Append(",");
            J(sb, "gloryTemplateId", GloryTemplateId); sb.Append(",");
            J(sb, "sigilQuality", SigilQuality); sb.Append(",");
            J(sb, "sigilProType", SigilProType); sb.Append(",");
            J(sb, "sigilProValue", SigilProValue); sb.Append(",");
            J(sb, "vipLevel", VipLevel); sb.Append(",");
            J(sb, "honor", Honor); sb.Append(",");
            J(sb, "texp", Texp); sb.Append(",");
            J(sb, "preferredBallId", PreferredBallId); sb.Append(",");
            J(sb, "lastSignDay", LastSignDay); sb.Append(",");
            J(sb, "signIndex", SignIndex); sb.Append(",");
            J(sb, "labyrinthFloor", LabyrinthFloor); sb.Append(",");
            J(sb, "consortiaName", ConsortiaName); sb.Append(",");
            J(sb, "guildLevel", GuildLevel); sb.Append(",");
            J(sb, "consortiaBossHits", ConsortiaBossHits); sb.Append(",");
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
            J(sb, "homeTemplePracticeLevel", HomeTemplePracticeLevel); sb.Append(",");
            J(sb, "homeTempleAdvanceLevel", HomeTempleAdvanceLevel); sb.Append(",");
            EnsureBankDeposits();
            sb.Append("\"bankDeposits\":[");
            for (int i = 0; i < BankDeposits.Count; i++)
            {
                if (i > 0) sb.Append(",");
                BankTermDeposit dep = BankDeposits[i];
                sb.Append("{\"templateId\":").Append(dep.TemplateId).Append(",\"amount\":").Append(dep.Amount)
                    .Append(",\"depositDay\":").Append(dep.DepositDay).Append("}");
            }
            sb.Append("],");
            EnsureSweepMissionClears();
            sb.Append("\"sweepMissionClears\":[");
            for (int i = 0; i < SweepMissionClears.Count; i++) { if (i > 0) sb.Append(","); sb.Append(SweepMissionClears[i]); }
            sb.Append("],");
            J(sb, "wardrobeClothId", WardrobeClothId); sb.Append(",");
            J(sb, "honorSystemExp", HonorSystemExp); sb.Append(",");
            J(sb, "honorSystemLevel", HonorSystemLevel); sb.Append(",");
            J(sb, "redPacketClaims", RedPacketClaims); sb.Append(",");
            J(sb, "devilTurnSpins", DevilTurnSpins); sb.Append(",");
            J(sb, "devilTurnPoints", DevilTurnPoints); sb.Append(",");
            EnsureDevilTreasPointClaimed();
            sb.Append("\"devilTreasPointClaimed\":[");
            for (int i = 0; i < DevilTreasPointClaimed.Count; i++) { if (i > 0) sb.Append(","); sb.Append(DevilTreasPointClaimed[i]); }
            sb.Append("],");
            sb.Append("\"acceptedQuests\":[");
            for (int i = 0; i < AcceptedQuests.Count; i++) { if (i > 0) sb.Append(","); sb.Append(AcceptedQuests[i]); }
            sb.Append("],");
            sb.Append("\"completedQuests\":[");
            for (int i = 0; i < CompletedQuests.Count; i++) { if (i > 0) sb.Append(","); sb.Append(CompletedQuests[i]); }
            sb.Append("],");
            if (QuestProgress != null && QuestProgress.Count > 0)
            {
                sb.Append("\"questProgress\":{");
                bool qpFirst = true;
                foreach (KeyValuePair<int, List<int>> kv in QuestProgress)
                {
                    if (!qpFirst) sb.Append(",");
                    qpFirst = false;
                    sb.Append("\"").Append(kv.Key).Append("\":[");
                    for (int i = 0; i < kv.Value.Count; i++) { if (i > 0) sb.Append(","); sb.Append(kv.Value[i]); }
                    sb.Append("]");
                }
                sb.Append("},");
            }
            J(sb, "spaRoomDayScore", SpaRoomDayScore); sb.Append(",");
            J(sb, "treasureRoomDraws", TreasureRoomDraws); sb.Append(",");
            J(sb, "christmasClaims", ChristmasClaims); sb.Append(",");
            J(sb, "newYearPoints", NewYearPoints); sb.Append(",");
            J(sb, "newYearFreeUsed", NewYearFreeUsed); sb.Append(",");
            sb.Append("\"newYearPointClaimed\":[");
            for (int i = 0; i < NewYearPointClaimed.Count; i++) { if (i > 0) sb.Append(","); sb.Append(NewYearPointClaimed[i]); }
            sb.Append("],");
            J(sb, "worshipMoonDraws", WorshipMoonDraws); sb.Append(",");
            J(sb, "superLuckerDraws", SuperLuckerDraws); sb.Append(",");
            J(sb, "jigsawClaims", JigsawClaims); sb.Append(",");
            J(sb, "bibleClaims", BibleClaims); sb.Append(",");
            J(sb, "sweepCount", SweepCount); sb.Append(",");
            J(sb, "firstRechargeClaimed", FirstRechargeClaimed ? 1 : 0); sb.Append(",");
            sb.Append("\"firstRechargeShopBuys\":[");
            bool firstBuy = true;
            foreach (KeyValuePair<int, int> kv in FirstRechargeShopBuys)
            {
                if (!firstBuy) sb.Append(",");
                firstBuy = false;
                sb.Append("{\"templateId\":").Append(kv.Key).Append(",\"count\":").Append(kv.Value).Append("}");
            }
            sb.Append("],");
            J(sb, "dreamlandChapter", DreamlandChapter); sb.Append(",");
            J(sb, "dreamlandSection", DreamlandSection); sb.Append(",");
            J(sb, "dreamlandClearedSection", DreamlandClearedSection); sb.Append(",");
            J(sb, "dreamlandAttempts", DreamlandAttempts); sb.Append(",");
            J(sb, "warriorFamHardType", WarriorFamHardType); sb.Append(",");
            J(sb, "warriorFamLevel", WarriorFamLevel); sb.Append(",");
            J(sb, "warriorFamClearedLevel", WarriorFamClearedLevel); sb.Append(",");
            J(sb, "warriorFamAttempts", WarriorFamAttempts); sb.Append(",");
            J(sb, "forcesBattleScore", ForcesBattleScore); sb.Append(",");
            J(sb, "forcesBattleAttempts", ForcesBattleAttempts); sb.Append(",");
            J(sb, "cultureGrade", CultureGrade); sb.Append(",");
            J(sb, "cultureAtk", CultureAtk); sb.Append(",");
            J(sb, "cultureDef", CultureDef); sb.Append(",");
            J(sb, "cultureAgi", CultureAgi); sb.Append(",");
            J(sb, "cultureLuck", CultureLuck); sb.Append(",");
            J(sb, "jampsManualLevel", JampsManualLevel); sb.Append(",");
            sb.Append("\"jampsDebrisOwned\":[");
            EnsureJampsLists();
            for (int i = 0; i < JampsDebrisOwned.Count; i++) { if (i > 0) sb.Append(","); sb.Append(JampsDebrisOwned[i]); }
            sb.Append("],");
            sb.Append("\"jampsPagesCollected\":[");
            for (int i = 0; i < JampsPagesCollected.Count; i++) { if (i > 0) sb.Append(","); sb.Append(JampsPagesCollected[i]); }
            sb.Append("],");
            sb.Append("\"jampsPagesActivated\":[");
            for (int i = 0; i < JampsPagesActivated.Count; i++) { if (i > 0) sb.Append(","); sb.Append(JampsPagesActivated[i]); }
            sb.Append("],");
            J(sb, "cardMainLevel", CardMainLevel); sb.Append(",");
            sb.Append("\"ownedCardTemplateIds\":[");
            EnsureOwnedCards();
            for (int i = 0; i < OwnedCardTemplateIds.Count; i++) { if (i > 0) sb.Append(","); sb.Append(OwnedCardTemplateIds[i]); }
            sb.Append("],");
            sb.Append("\"cardBookletProfiles\":[");
            for (int i = 0; i < CardBookletProfiles.Count; i++) { if (i > 0) sb.Append(","); sb.Append(CardBookletProfiles[i]); }
            sb.Append("],");
            sb.Append("\"cardBookletClaimed\":[");
            for (int i = 0; i < CardBookletClaimed.Count; i++) { if (i > 0) sb.Append(","); sb.Append(CardBookletClaimed[i]); }
            sb.Append("],");
            J(sb, "cardSoul", CardSoul); sb.Append(",");
            J(sb, "elfIntimacyExp", ElfIntimacyExp); sb.Append(",");
            J(sb, "elfIntimacyLevel", ElfIntimacyLevel); sb.Append(",");
            J(sb, "elfIntimacyActions", ElfIntimacyActions); sb.Append(",");
            EnsureCalendarClaimed();
            J(sb, "calendarMonth", CalendarMonth); sb.Append(",");
            sb.Append("\"calendarClaimedDays\":["); for (int i = 0; i < CalendarClaimedDays.Count; i++) { if (i > 0) sb.Append(","); sb.Append(CalendarClaimedDays[i]); } sb.Append("],");
            J(sb, "auditoriumActions", AuditoriumActions); sb.Append(",");
            J(sb, "boguAdventureActions", BoguAdventureActions); sb.Append(",");
            TouchQuizDay();
            J(sb, "quizAttempts", QuizAttempts); sb.Append(",");
            TouchOneYuanDay();
            sb.Append("\"oneYuanBought\":[");
            for (int i = 0; i < OneYuanBought.Count; i++) { if (i > 0) sb.Append(","); sb.Append(OneYuanBought[i]); }
            sb.Append("],");
            J(sb, "godCardEquipId", GodCardEquipId); sb.Append(",");
            J(sb, "godCardPoints", GodCardPoints); sb.Append(",");
            sb.Append("\"godCardPointClaimed\":[");
            EnsureGodCardPointClaimed();
            for (int i = 0; i < GodCardPointClaimed.Count; i++) { if (i > 0) sb.Append(","); sb.Append(GodCardPointClaimed[i]); }
            sb.Append("],");
            J(sb, "engraveSetId", EngraveSetId); sb.Append(",");
            sb.Append("\"godCards\":[");
            for (int i = 0; i < GodCards.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("{\"id\":").Append(GodCards[i].Id).Append(",\"count\":").Append(GodCards[i].Count).Append(",\"grooveLevel\":").Append(GodCards[i].GrooveLevel).Append(",\"grooveExp\":").Append(GodCards[i].GrooveExp).Append("}");
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
            EnsureRelics(); sb.Append("\"relics\":[");
            for (int i = 0; i < Relics.Count; i++) { if (i > 0) sb.Append(","); RelicSlot r = Relics[i]; sb.Append("{\"relicId\":").Append(r.RelicId).Append(",\"upgradeLevel\":").Append(r.UpgradeLevel).Append("}"); }
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

        public void EnsureGodCardPointClaimed() { if (GodCardPointClaimed == null) GodCardPointClaimed = new List<int>(); }
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
        readonly Dictionary<string, int> _guildLevels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
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
                {
                    player.CardId = JI(json, "cardId", player.CardId);
                    if (_db != null)
                    {
                        CardInfo card = _db.GetCard(player.CardId);
                        if (card != null)
                        {
                            player.EnsureOwnedCards();
                            if (!player.OwnedCardTemplateIds.Contains(card.CardId))
                                player.OwnedCardTemplateIds.Add(card.CardId);
                        }
                    }
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;
                }

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
                        ResolveGuildLevel(player);
                        player.RecalcStats(_db);
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
                    HandleFriendAdd(player, ns, json);
                    break;

                case PhoneMsg.FriendRemove:
                    HandleFriendRemove(player, ns, json);
                    break;

                case PhoneMsg.MailClaim:
                    HandleMailClaim(player, ns, json);
                    break;

                case PhoneMsg.MailList:
                    Send(ns, PhoneMsg.MailListData, BuildMailListJson(player));
                    break;

                case PhoneMsg.Chat:
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

                case PhoneMsg.ChatWhisper:
                    HandleChatWhisper(player, ns, json);
                    break;

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

                case PhoneMsg.DevilTreasPointClaim:
                    HandleDevilTreasPointClaim(player, ns, json);
                    break;

                case PhoneMsg.RedPacketSend:
                    HandleRedPacketSend(player, ns, json);
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

                case PhoneMsg.FirstRechargeClaim:
                    HandleFirstRechargeClaim(player, ns);
                    break;

                case PhoneMsg.FirstRechargeShop:
                    HandleFirstRechargeShop(player, ns, json);
                    break;

                case PhoneMsg.SpaRoomStart:
                    HandleSpaRoomStart(player, ns);
                    break;

                case PhoneMsg.SpaRoomBomb:
                    HandleSpaRoomBomb(player, ns, json);
                    break;

                case PhoneMsg.TreasureRoomDraw:
                    HandleTreasureRoomDraw(player, ns, json);
                    break;

                case PhoneMsg.ChristmasClaim:
                    HandleChristmasClaim(player, ns);
                    break;

                case PhoneMsg.NewYearClaim:
                    HandleNewYearClaim(player, ns, json);
                    break;

                case PhoneMsg.WorshipMoonClaim:
                    HandleWorshipMoonClaim(player, ns, json);
                    break;

                case PhoneMsg.SuperLuckerDraw:
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

                case PhoneMsg.JampsUpgrade:
                    HandleJampsUpgrade(player, ns);
                    break;

                case PhoneMsg.JampsClaimPage:
                    HandleJampsClaimPage(player, ns, json);
                    break;

                case PhoneMsg.CardMainUpgrade:
                    HandleCardMainUpgrade(player, ns);
                    break;

                case PhoneMsg.ElfIntimacyAction:
                    HandleElfIntimacyAction(player, ns, json);
                    break;

                case PhoneMsg.PetStarUpgrade:
                    HandlePetStarUpgrade(player, ns);
                    break;

                case PhoneMsg.MountTalismanEquip:
                    HandleMountTalismanEquip(player, ns, json);
                    break;

                case PhoneMsg.ManorUpgrade:
                    HandleManorUpgrade(player, ns);
                    break;

                case PhoneMsg.GoldEquipUpgrade:
                    HandleGoldEquipUpgrade(player, ns, json);
                    break;

                case PhoneMsg.GloryUpgrade:
                    HandleGloryUpgrade(player, ns, json);
                    break;

                case PhoneMsg.SigilRoll:
                    HandleSigilRoll(player, ns, json);
                    break;

                case PhoneMsg.QuizAnswer:
                    HandleQuizAnswer(player, ns, json);
                    break;

                case PhoneMsg.OneYuanBuy:
                    HandleOneYuanBuy(player, ns, json);
                    break;

                case PhoneMsg.CardBookletClaim:
                    HandleCardBookletClaim(player, ns, json);
                    break;

                case PhoneMsg.StrengthenGoodsMap:
                    HandleStrengthenGoodsMap(player, ns, json);
                    break;

                case PhoneMsg.BoxOpen:
                    HandleBoxOpen(player, ns, json);
                    break;

                case PhoneMsg.ItemFusion:
                    HandleItemFusion(player, ns, json);
                    break;

                case PhoneMsg.CalendarClaim: HandleCalendarClaim(player, ns, json); break;
                case PhoneMsg.AuditoriumAction: HandleAuditoriumAction(player, ns, json); break;
                case PhoneMsg.BoguAdventureAction: HandleBoguAdventureAction(player, ns, json); break;
                case PhoneMsg.JigsawAction:
                    HandlePcActivityAction(player, ns, json, "jigsaw", PhoneMsg.JigsawAction);
                    break;

                case PhoneMsg.BibleAction:
                    HandlePcActivityAction(player, ns, json, "bible", PhoneMsg.BibleAction);
                    break;

                case PhoneMsg.GuildUpgrade:
                    HandleGuildUpgrade(player, ns);
                    break;

                case PhoneMsg.ConsortiaBossStart:
                    HandleConsortiaBossStart(player, ns);
                    break;

                case PhoneMsg.EmblemCraft: HandleEmblemCraft(player, ns, json); break;
                case PhoneMsg.EmblemEquip: HandleEmblemEquip(player, ns, json); break;
                case PhoneMsg.SoulStampCompose: HandleSoulStampCompose(player, ns, json); break;
                case PhoneMsg.SoulStampRefine: HandleSoulStampRefine(player, ns, json); break;
                case PhoneMsg.WardrobeEquip: HandleWardrobeEquip(player, ns, json); break;
                case PhoneMsg.WardrobeUpgrade: HandleWardrobeUpgrade(player, ns, json); break;
                case PhoneMsg.HonorSystemAction: HandleHonorSystemAction(player, ns, json); break;
                case PhoneMsg.HonorSystemClaim: HandleHonorSystemClaim(player, ns, json); break;
                case PhoneMsg.GodCardRaise: HandleGodCardRaise(player, ns, json); break;
                case PhoneMsg.GodCardPointClaim: HandleGodCardPointClaim(player, ns, json); break;

                case PhoneMsg.DreamlandStart:
                    HandleDreamlandStart(player, ns, json);
                    break;

                case PhoneMsg.DreamlandClaim:
                    HandleDreamlandClaim(player, ns, json);
                    break;

                case PhoneMsg.WarriorFamStart:
                    HandleWarriorFamStart(player, ns, json);
                    break;

                case PhoneMsg.WarriorFamClaim:
                    HandleWarriorFamClaim(player, ns, json);
                    break;

                case PhoneMsg.ForcesBattleStart:
                    HandleForcesBattleStart(player, ns, json);
                    break;

                case PhoneMsg.ForcesRelicUpgrade:
                    HandleForcesRelicUpgrade(player, ns, json);
                    break;

                case PhoneMsg.CultureUpgrade:
                    HandleCultureUpgrade(player, ns, json);
                    break;

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
                case PhoneMsg.FightSkip:
                {
                    int who = JI(json, "who", player.Seat);
                    bool allowSkip;
                    lock (_lock) { allowSkip = room.InBattle && player.Seat == who; }
                    if (!allowSkip) return;
                    BroadcastToRoom(room, PhoneMsg.FightSkip, EnsureJsonField(json, "who", who), player.Id);
                    break;
                }

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
            int harvestGold = _db != null ? _db.ManorHarvestGold(player.ManorGrade) : 0;
            if (harvestGold > 0)
            {
                player.Gold += harvestGold;
            }
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
                "{\"ok\":true,\"name\":\"" + (player.ConsortiaName ?? "").Replace("\"", "") +
                "\",\"guildLevel\":" + player.GuildLevel + ",\"members\":[" + gMembers + "]}");
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
            player.GuildLevel = 1;
            lock (_lock) { _guildLevels[gName] = 1; }
            player.RecalcStats(_db);
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
            player.GuildLevel = 0;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.GuildResult, "{\"ok\":true,\"name\":\"\",\"members\":[]}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleFriendAdd(ServerPlayer player, NetworkStream ns, string json)
        {
            string fn = JS(json, "name", "");
            if (string.IsNullOrEmpty(fn))
            {
                SendFriendResult(player, ns);
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }

            fn = fn.Trim();
            if (!player.Friends.Contains(fn))
            {
                player.Friends.Add(fn);
                SavePlayer(player);
            }

            // Mutual: in-memory player first (including disconnected sessions still in _players), then save file.
            lock (_lock)
            {
                ServerPlayer fp = FindPlayerByNick(fn, player);
                if (fp != null)
                {
                    if (!fp.Friends.Contains(player.Nick))
                    {
                        fp.Friends.Add(player.Nick);
                        SavePlayer(fp);
                    }
                }
                else
                {
                    TryAddFriendToSaveFile(fn, player.Nick);
                }
            }

            SendFriendResult(player, ns);
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }


        int ResolveGuildLevel(ServerPlayer player)
        {
            if (player == null || string.IsNullOrEmpty(player.ConsortiaName))
            {
                if (player != null) player.GuildLevel = 0;
                return 0;
            }

            lock (_lock)
            {
                if (_guildLevels.TryGetValue(player.ConsortiaName, out int shared))
                {
                    player.GuildLevel = Mathf.Max(1, shared);
                    return player.GuildLevel;
                }

                int seed = player.GuildLevel > 0 ? player.GuildLevel : 1;
                if (seed > 10) seed = 10;
                _guildLevels[player.ConsortiaName] = seed;
                player.GuildLevel = seed;
                return seed;
            }
        }

        void ApplyGuildLevelToMembers(string name, int level)
        {
            if (string.IsNullOrEmpty(name)) return;
            lock (_lock)
            {
                _guildLevels[name] = level;
                foreach (ServerPlayer p in _players.Values)
                {
                    if (!string.Equals(p.ConsortiaName, name, StringComparison.OrdinalIgnoreCase))
                        continue;
                    p.GuildLevel = level;
                    p.RecalcStats(_db);
                    SavePlayer(p);
                    if (p.RoadStream != null)
                        Send(p.RoadStream, PhoneMsg.ProfileData, p.ToJson());
                }
            }
        }

        void HandleGuildUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (string.IsNullOrEmpty(player.ConsortiaName))
            {
                Send(ns, PhoneMsg.GuildUpgrade, "{\"ok\":false,\"err\":\"guild\"}");
                return;
            }

            int current = ResolveGuildLevel(player);
            int maxLv = _db != null ? Mathf.Min(10, _db.ConsortiaMaxLevel()) : 10;
            if (current >= maxLv)
            {
                Send(ns, PhoneMsg.GuildUpgrade, "{\"ok\":false,\"err\":\"max\"}");
                return;
            }

            int next = current + 1;
            int cost = _db != null ? _db.ConsortiaNeedGold(next) : 0;
            if (cost <= 0)
            {
                Send(ns, PhoneMsg.GuildUpgrade, "{\"ok\":false,\"err\":\"xml\"}");
                return;
            }

            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.GuildUpgrade, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= cost;
            player.Honor += next * 10;
            ApplyGuildLevelToMembers(player.ConsortiaName, next);
            player.GuildLevel = next;
            Send(ns, PhoneMsg.GuildUpgrade,
                "{\"ok\":true,\"level\":" + next + ",\"cost\":" + cost + "}");
            SendGuildResult(player, ns);
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleConsortiaBossStart(ServerPlayer player, NetworkStream ns)
        {
            if (string.IsNullOrEmpty(player.ConsortiaName))
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"guild\"}");
                return;
            }

            int guildLv = ResolveGuildLevel(player);
            int today = DateTime.Now.DayOfYear;
            if (player.ConsortiaBossDay != today)
            {
                player.ConsortiaBossDay = today;
                player.ConsortiaBossHits = 0;
            }

            int maxHits = _db != null ? _db.ConfigInt("ConsortiaBossDayLimit", 3) : 3;
            if (player.ConsortiaBossHits >= maxHits)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int npcId = _db != null ? _db.ConsortiaBossNpcId(guildLv, player.Level) : 0;
            if (npcId <= 0)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"npc\"}");
                return;
            }

            player.ConsortiaBossHits++;
            player.PveNpcId = npcId;
            player.PveLabyrinth = false;
            player.PveRewardGold = _db != null ? _db.ComputePveWinGold(npcId, player.LabyrinthFloor, false) : 0;
            SavePlayer(player);
            string result = "{\"ok\":true,\"reward\":" + player.PveRewardGold + ",\"npcId\":" + npcId +
                            ",\"guildLevel\":" + guildLv + "}";
            Send(ns, PhoneMsg.ConsortiaBossStart, result);
            Send(ns, PhoneMsg.PveResult, result);
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

        void HandleForcesBattleStart(ServerPlayer player, NetworkStream ns, string json)
        {
            int quality = Mathf.Clamp(JI(json, "quality", 1), 1, 5);
            int today = DateTime.Now.DayOfYear;
            if (player.ForcesBattleDay != today) { player.ForcesBattleDay = today; player.ForcesBattleAttempts = 0; }
            int maxAttempts = _db != null ? _db.ConfigInt("CityOccupationAddScoreCount", 30) : 30;
            if (player.ForcesBattleAttempts >= maxAttempts) { Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"limit\"}"); return; }
            int entryFee = _db != null ? _db.ForcesBattleEntryFee(quality) : quality * 100;
            if (player.Gold < entryFee) { Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= entryFee;
            player.ForcesBattleAttempts++;
            player.PveNpcId = _db != null ? _db.ForcesBattleNpcId(quality, player.Level) : 44401;
            player.PveLabyrinth = false;
            player.PveRewardGold = _db != null ? _db.ForcesBattleRewardGold(quality, player.Level) : 800;
            player.ForcesBattleScore += _db != null ? _db.ForcesBattleScoreGain(quality) : quality;
            SavePlayer(player);
            Send(ns, PhoneMsg.PveResult, "{\"ok\":true,\"reward\":" + player.PveRewardGold + ",\"npcId\":" + player.PveNpcId + ",\"quality\":" + quality + ",\"score\":" + player.ForcesBattleScore + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleForcesRelicUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.ForcesRelicUpgrade, "{\"ok\":false}"); return; }
            int relicId = JI(json, "relicId", 1);
            player.EnsureRelics();
            RelicSlot slot = player.FindRelic(relicId);
            if (slot == null) { slot = new RelicSlot { RelicId = relicId, UpgradeLevel = 0 }; player.Relics.Add(slot); }
            if (_db.GetRelicItem(relicId) == null) { Send(ns, PhoneMsg.ForcesRelicUpgrade, "{\"ok\":false,\"err\":\"relic\"}"); return; }
            if (_db.GetRelicUpgrade(relicId, slot.UpgradeLevel + 1) == null) { Send(ns, PhoneMsg.ForcesRelicUpgrade, "{\"ok\":false,\"err\":\"max\"}"); return; }
            int cost = _db.RelicUpgradeGoldCost(relicId, slot.UpgradeLevel);
            if (cost <= 0 || player.Gold < cost) { Send(ns, PhoneMsg.ForcesRelicUpgrade, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost;
            slot.UpgradeLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.ForcesRelicUpgrade, "{\"ok\":true,\"relicId\":" + relicId + ",\"level\":" + slot.UpgradeLevel + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleCultureUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.CultureResult, "{\"ok\":false}"); return; }
            if (JI(json, "gradeUp", 0) != 0)
            {
                if (_db.GetExerciseInfo(player.CultureGrade + 1) == null) { Send(ns, PhoneMsg.CultureResult, "{\"ok\":false,\"err\":\"max\"}"); return; }
                int gradeCost = _db.CultureGradeGoldCost(player.CultureGrade);
                if (gradeCost <= 0 || player.Gold < gradeCost) { Send(ns, PhoneMsg.CultureResult, "{\"ok\":false,\"err\":\"gold\"}"); return; }
                player.Gold -= gradeCost;
                player.CultureGrade++;
                player.RecalcStats(_db);
                SavePlayer(player);
                Send(ns, PhoneMsg.CultureResult, "{\"ok\":true,\"cultureGrade\":" + player.CultureGrade + ",\"cost\":" + gradeCost + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }
            int statType = JI(json, "statType", 116);
            if (statType < 116 || statType > 119) { Send(ns, PhoneMsg.CultureResult, "{\"ok\":false,\"err\":\"stat\"}"); return; }
            int current = player.GetCultureStatLevel(statType);
            if (_db.GetCultureUpgrade(statType, current + 1) == null) { Send(ns, PhoneMsg.CultureResult, "{\"ok\":false,\"err\":\"max\"}"); return; }
            int cost = _db.CultureUpgradeGoldCost(statType, current);
            if (cost <= 0 || player.Gold < cost) { Send(ns, PhoneMsg.CultureResult, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost;
            player.SetCultureStatLevel(statType, current + 1);
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.CultureResult, "{\"ok\":true,\"statType\":" + statType + ",\"level\":" + (current + 1) + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleJampsUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (_db == null) { Send(ns, PhoneMsg.JampsUpgrade, "{\"ok\":false}"); return; }
            int targetLevel = player.JampsManualLevel + 1;
            if (_db.GetJampsManual(targetLevel) == null) { Send(ns, PhoneMsg.JampsUpgrade, "{\"ok\":false,\"err\":\"max\"}"); return; }
            List<JampsUpgradeCondition> conditions = _db.GetJampsUpgradeConditions(targetLevel);
            for (int i = 0; i < conditions.Count; i++)
                if (!CheckJampsUpgradeCondition(player, conditions[i])) { Send(ns, PhoneMsg.JampsUpgrade, "{\"ok\":false,\"err\":\"cond\"}"); return; }
            for (int i = 0; i < conditions.Count; i++)
                if (conditions[i].ConditionType == 1 && !player.Consume(conditions[i].Parameter1, conditions[i].Parameter2))
                { Send(ns, PhoneMsg.JampsUpgrade, "{\"ok\":false,\"err\":\"item\"}"); return; }
            player.JampsManualLevel = targetLevel;
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.JampsUpgrade, "{\"ok\":true,\"level\":" + targetLevel + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        bool CheckJampsUpgradeCondition(ServerPlayer player, JampsUpgradeCondition cond)
        {
            if (_db == null || cond == null) return false;
            player.EnsureJampsLists();
            switch (cond.ConditionType)
            {
                case 1:
                    for (int i = 0; i < player.Bag.Count; i++)
                        if (player.Bag[i].TemplateId == cond.Parameter1 && player.Bag[i].Count >= cond.Parameter2) return true;
                    return false;
                case 2: return player.JampsPagesCollected.Count >= cond.Parameter1;
                case 3:
                    int c3 = 0;
                    foreach (int pageId in player.JampsPagesCollected) { JampsPageInfo p = _db.GetJampsPage(pageId); if (p != null && p.ChapterId == cond.Parameter1) c3++; }
                    return c3 >= cond.Parameter2;
                case 4: return HasJampsPages(player, cond.Parameter1, cond.Parameter2, cond.Parameter3, false);
                case 5:
                    int c5 = 0;
                    foreach (int pageId in player.JampsPagesActivated) { JampsPageInfo p = _db.GetJampsPage(pageId); if (p != null && p.ChapterId == cond.Parameter1) c5++; }
                    return c5 >= cond.Parameter2;
                case 6: return HasJampsPages(player, cond.Parameter1, cond.Parameter2, cond.Parameter3, true);
                case 7: return player.JampsPagesActivated.Count >= cond.Parameter1;
                default: return true;
            }
        }

        bool HasJampsPages(ServerPlayer player, int chapterId, int pageA, int pageB, bool activated)
        {
            IReadOnlyList<int> pages = activated ? player.JampsPagesActivated : player.JampsPagesCollected;
            bool hasA = false, hasB = false;
            for (int i = 0; i < pages.Count; i++)
            {
                JampsPageInfo page = _db.GetJampsPage(pages[i]);
                if (page == null || page.ChapterId != chapterId) continue;
                if (pages[i] == pageA) hasA = true;
                if (pageB > 0 && pages[i] == pageB) hasB = true;
            }
            return hasA && (pageB <= 0 || hasB);
        }

        void HandleJampsClaimPage(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false}"); return; }
            string action = JS(json, "action", "");
            int pageId = JI(json, "pageId", 0), debrisId = JI(json, "debrisId", 0);
            player.EnsureJampsLists();
            if (action == "debris")
            {
                JampsDebrisInfo debris = _db.GetJampsDebris(debrisId);
                if (debris == null || player.HasJampsDebris(debrisId)) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"debris\"}"); return; }
                if (player.Gold < debris.JampsCurrency) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"gold\"}"); return; }
                player.Gold -= debris.JampsCurrency; player.JampsDebrisOwned.Add(debrisId);
            }
            else if (action == "collect")
            {
                JampsPageInfo page = _db.GetJampsPage(pageId);
                if (page == null || player.HasJampsPageCollected(pageId)) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"page\"}"); return; }
                if (_db.CountJampsDebrisForPage(player.JampsDebrisOwned, pageId) < page.DebrisCount) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"needDebris\"}"); return; }
                player.JampsPagesCollected.Add(pageId);
            }
            else if (action == "activate")
            {
                JampsPageInfo page = _db.GetJampsPage(pageId);
                if (page == null || !player.HasJampsPageCollected(pageId) || player.HasJampsPageActivated(pageId)) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"page\"}"); return; }
                if (player.Gold < page.ActivateCurrency) { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"gold\"}"); return; }
                player.Gold -= page.ActivateCurrency; player.JampsPagesActivated.Add(pageId);
            }
            else { Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":false,\"err\":\"action\"}"); return; }
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.JampsClaimPage, "{\"ok\":true,\"action\":\"" + action + "\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleCardMainUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (_db == null) { Send(ns, PhoneMsg.CardMainUpgrade, "{\"ok\":false}"); return; }
            int nextLevel = player.CardMainLevel + 1;
            CardMainLevelInfo row = _db.GetCardMainLevel(nextLevel);
            if (row == null) { Send(ns, PhoneMsg.CardMainUpgrade, "{\"ok\":false,\"err\":\"max\"}"); return; }
            int cost = row.NeedItem1Count;
            if (cost <= 0 || player.Gold < cost) { Send(ns, PhoneMsg.CardMainUpgrade, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost; player.CardMainLevel = nextLevel;
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.CardMainUpgrade, "{\"ok\":true,\"level\":" + nextLevel + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleElfIntimacyAction(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.ElfIntimacyAction, "{\"ok\":false}"); return; }
            if (player.ElfId <= 0) { Send(ns, PhoneMsg.ElfIntimacyAction, "{\"ok\":false,\"err\":\"elf\"}"); return; }
            player.TouchElfIntimacyDay();
            int maxActions = _db.ConfigInt("ElfIntimacyDayLimit", 10);
            if (player.ElfIntimacyActions >= maxActions) { Send(ns, PhoneMsg.ElfIntimacyAction, "{\"ok\":false,\"err\":\"limit\"}"); return; }
            string action = JS(json, "action", "gift");
            int gain = action == "interact" ? 10 : 15;
            player.ElfIntimacyExp += gain; player.ElfIntimacyActions++;
            player.SyncElfIntimacyLevel(_db); player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.ElfIntimacyAction, "{\"ok\":true,\"exp\":" + player.ElfIntimacyExp + ",\"level\":" + player.ElfIntimacyLevel + ",\"gain\":" + gain + "}");
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

        void HandleDreamlandStart(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"no db\"}");
                return;
            }

            int chapter = JI(json, "chapter", player.DreamlandChapter > 0 ? player.DreamlandChapter : 1);
            int section = JI(json, "section", player.DreamlandSection > 0 ? player.DreamlandSection : 1);
            StoryCopySection row = _db.GetStoryCopySection(chapter, section);
            if (row == null)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"section\"}");
                return;
            }

            int today = DateTime.Now.DayOfYear;
            if (player.DreamlandDay != today)
            {
                player.DreamlandDay = today;
                player.DreamlandAttempts = 0;
            }

            if (player.DreamlandAttempts >= row.PlayLimit)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int entryFee = _db.DreamlandEntryFee(row);
            if (player.Gold < entryFee)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= entryFee;
            player.DreamlandChapter = chapter;
            player.DreamlandSection = section;
            player.DreamlandAttempts++;
            player.PveNpcId = _db.DreamlandNpcId(row, player.Level);
            player.PveLabyrinth = false;
            player.PveDreamland = true;
            player.PveDreamlandChapter = chapter;
            player.PveDreamlandSection = section;
            player.PveWarriorFam = false;
            player.PveRewardGold = _db.DreamlandRewardGold(row, player.PveNpcId);
            SavePlayer(player);
            Send(ns, PhoneMsg.PveResult,
                "{\"ok\":true,\"reward\":" + player.PveRewardGold +
                ",\"npcId\":" + player.PveNpcId +
                ",\"map\":" + _db.DreamlandMapId(row) +
                ",\"chapter\":" + chapter + ",\"section\":" + section + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleDreamlandClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.DreamlandClaim, "{\"ok\":false}");
                return;
            }

            int chapter = JI(json, "chapter", player.DreamlandChapter > 0 ? player.DreamlandChapter : 1);
            int section = JI(json, "section", player.DreamlandClearedSection > 0 ? player.DreamlandClearedSection : 1);
            if (section <= 0 || section > player.DreamlandClearedSection)
            {
                Send(ns, PhoneMsg.DreamlandClaim, "{\"ok\":false,\"err\":\"locked\"}");
                return;
            }

            StoryCopySection row = _db.GetStoryCopySection(chapter, section);
            if (row == null || string.IsNullOrEmpty(row.SweepReward))
            {
                Send(ns, PhoneMsg.DreamlandClaim, "{\"ok\":false,\"err\":\"section\"}");
                return;
            }

            _db.GrantRewardPairs(player, row.SweepReward);
            SavePlayer(player);
            Send(ns, PhoneMsg.DreamlandClaim,
                "{\"ok\":true,\"chapter\":" + chapter + ",\"section\":" + section + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleWarriorFamStart(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"no db\"}");
                return;
            }

            int needLevel = _db.ConfigInt("WarriorFamGradeLimit", 30);
            if (player.Level < needLevel)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"level\"}");
                return;
            }

            int hardType = JI(json, "hardType", player.WarriorFamHardType);
            hardType = Mathf.Clamp(hardType, 0, 2);
            int level = JI(json, "level", player.WarriorFamLevel > 0 ? player.WarriorFamLevel : 1);
            int maxLevel = _db.ConfigInt("WarriorFamMaxLevel", 100);
            level = Mathf.Clamp(level, 1, maxLevel);
            WarriorFamFightConfig row = _db.GetWarriorFamFight(hardType, level);
            if (row == null)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"level\"}");
                return;
            }

            int today = DateTime.Now.DayOfYear;
            if (player.WarriorFamDay != today)
            {
                player.WarriorFamDay = today;
                player.WarriorFamAttempts = 0;
            }

            int maxAttempts = _db.ConfigInt("WarriorFamEveryDayContinueCount", 1);
            if (player.WarriorFamAttempts >= maxAttempts)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int entryFee = _db.WarriorFamEntryFee();
            if (player.Gold < entryFee)
            {
                Send(ns, PhoneMsg.PveResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            player.Gold -= entryFee;
            player.WarriorFamHardType = hardType;
            player.WarriorFamLevel = level;
            player.WarriorFamAttempts++;
            player.PveNpcId = _db.WarriorFamNpcId(row);
            player.PveLabyrinth = false;
            player.PveDreamland = false;
            player.PveWarriorFam = true;
            player.PveWarriorFamHardType = hardType;
            player.PveWarriorFamLevel = level;
            player.PveRewardGold = _db.WarriorFamRewardGold(row);
            SavePlayer(player);
            Send(ns, PhoneMsg.PveResult,
                "{\"ok\":true,\"reward\":" + player.PveRewardGold +
                ",\"npcId\":" + player.PveNpcId +
                ",\"hardType\":" + hardType + ",\"level\":" + level + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleWarriorFamClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.WarriorFamClaim, "{\"ok\":false}");
                return;
            }

            int hardType = JI(json, "hardType", player.WarriorFamHardType);
            hardType = Mathf.Clamp(hardType, 0, 2);
            int level = JI(json, "level", player.WarriorFamClearedLevel > 0 ? player.WarriorFamClearedLevel : 1);
            if (level <= 0 || level > player.WarriorFamClearedLevel)
            {
                Send(ns, PhoneMsg.WarriorFamClaim, "{\"ok\":false,\"err\":\"locked\"}");
                return;
            }

            WarriorFamFightConfig row = _db.GetWarriorFamFight(hardType, level);
            if (row == null || string.IsNullOrEmpty(row.Rewards))
            {
                Send(ns, PhoneMsg.WarriorFamClaim, "{\"ok\":false,\"err\":\"level\"}");
                return;
            }

            _db.GrantRewardPairs(player, row.Rewards);
            SavePlayer(player);
            Send(ns, PhoneMsg.WarriorFamClaim,
                "{\"ok\":true,\"hardType\":" + hardType + ",\"level\":" + level + "}");
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
            int pointsPerSpin = _db != null ? _db.ConfigInt("DevilTreasurePointPerSpin", 100) : 100;
            player.DevilTurnPoints += count * pointsPerSpin;
            SavePlayer(player);
            rewards.Append("]");
            Send(ns, PhoneMsg.DevilTurnSpin,
                "{\"ok\":true,\"cost\":" + cost + ",\"rewards\":" + rewards + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleSpaRoomStart(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.SpaRoomDay != today)
            {
                player.SpaRoomDay = today;
                player.SpaRoomDayScore = 0;
            }

            int dayLimit = _db != null ? _db.SpaRoomDayScoreLimit() : 100;
            if (player.SpaRoomDayScore >= dayLimit)
            {
                Send(ns, PhoneMsg.SpaRoomStart, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            SpaRoomFixedLevel fixedRow = _db != null ? _db.GetSpaRoomFixedLevel(player.Level) : null;
            int width = fixedRow != null && fixedRow.XAxes > 0 ? fixedRow.XAxes : 10;
            int height = fixedRow != null && fixedRow.YAxes > 0 ? fixedRow.YAxes : 10;
            int[] map;
            lock (_lock)
            {
                map = _db != null ? _db.BuildSpaRoomMap(player.Level, _rng) : new int[width * height];
            }

            if (map.Length != width * height && map.Length > 0)
            {
                width = fixedRow != null && fixedRow.XAxes > 0 ? fixedRow.XAxes : 4;
                height = Mathf.Max(1, map.Length / width);
            }

            player.SpaRoomActive = true;
            player.SpaRoomWidth = width;
            player.SpaRoomHeight = height;
            player.SpaRoomMap = new List<int>(map);
            player.SpaRoomPicked = new List<int>();
            player.SpaRoomScore = 0;
            SavePlayer(player);
            Send(ns, PhoneMsg.SpaRoomStart,
                "{\"ok\":true,\"width\":" + width + ",\"height\":" + height +
                ",\"level\":" + player.Level + ",\"dayScore\":" + player.SpaRoomDayScore +
                ",\"dayLimit\":" + dayLimit + ",\"gameLimit\":" +
                (_db != null ? _db.SpaRoomGameScoreLimit() : 200) + "}");
        }

        void HandleSpaRoomBomb(ServerPlayer player, NetworkStream ns, string json)
        {
            if (!player.SpaRoomActive || player.SpaRoomMap == null || player.SpaRoomMap.Count == 0)
            {
                Send(ns, PhoneMsg.SpaRoomBomb, "{\"ok\":false,\"err\":\"no game\"}");
                return;
            }

            int index = JI(json, "index", -1);
            if (index < 0 || index >= player.SpaRoomMap.Count)
            {
                Send(ns, PhoneMsg.SpaRoomBomb, "{\"ok\":false,\"err\":\"index\"}");
                return;
            }

            if (player.SpaRoomPicked != null)
            {
                for (int i = 0; i < player.SpaRoomPicked.Count; i++)
                {
                    if (player.SpaRoomPicked[i] == index)
                    {
                        Send(ns, PhoneMsg.SpaRoomBomb, "{\"ok\":false,\"err\":\"picked\"}");
                        return;
                    }
                }
            }
            else
            {
                player.SpaRoomPicked = new List<int>();
            }

            int cellType = player.SpaRoomMap[index];
            player.SpaRoomPicked.Add(index);
            int gold = 0;
            int itemId = 0;
            int itemCount = 0;
            bool gameOver = false;
            int scoreGain = 0;
            int dayLimit = _db != null ? _db.SpaRoomDayScoreLimit() : 100;
            int gameLimit = _db != null ? _db.SpaRoomGameScoreLimit() : 200;

            if (cellType == 6)
            {
                gameOver = true;
            }
            else if (cellType >= 1 && cellType <= 5)
            {
                scoreGain = cellType * 10;
                gold = cellType * 20;
                if (_db != null)
                {
                    itemId = _db.SpaRoomGiftForCellType(cellType);
                    if (itemId > 0)
                    {
                        itemCount = 1;
                        player.AddItem(itemId, itemCount);
                    }
                }
            }
            else if (cellType > 0)
            {
                scoreGain = 5;
                gold = 10;
            }

            int remainingDay = dayLimit - player.SpaRoomDayScore;
            if (scoreGain > remainingDay)
            {
                scoreGain = remainingDay;
            }

            int remainingGame = gameLimit - player.SpaRoomScore;
            if (scoreGain > remainingGame)
            {
                scoreGain = remainingGame;
            }

            player.SpaRoomScore += scoreGain;
            player.SpaRoomDayScore += scoreGain;
            player.Gold += gold;

            if (player.SpaRoomDayScore >= dayLimit || player.SpaRoomScore >= gameLimit)
            {
                gameOver = true;
            }

            if (gameOver)
            {
                player.SpaRoomActive = false;
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.SpaRoomBomb,
                "{\"ok\":true,\"index\":" + index + ",\"cellType\":" + cellType +
                ",\"gold\":" + gold + ",\"item\":" + itemId + ",\"count\":" + itemCount +
                ",\"score\":" + scoreGain + ",\"sessionScore\":" + player.SpaRoomScore +
                ",\"dayScore\":" + player.SpaRoomDayScore + ",\"gameOver\":" +
                (gameOver ? "true" : "false") + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleTreasureRoomDraw(ServerPlayer player, NetworkStream ns, string json)
        {
            int count = JI(json, "count", 1);
            count = Mathf.Clamp(count, 1, 10);
            int today = DateTime.Now.DayOfYear;
            if (player.TreasureRoomDay != today)
            {
                player.TreasureRoomDay = today;
                player.TreasureRoomDraws = 0;
            }

            int freeLeft = _db != null ? _db.ConfigInt("SearchGoodsFreeCount", 15) : 15;
            freeLeft = Mathf.Max(0, freeLeft - player.TreasureRoomDraws);
            int paidDraws = count;
            if (freeLeft >= count)
            {
                paidDraws = 0;
            }
            else
            {
                paidDraws = count - freeLeft;
            }

            int unitCost = _db != null ? _db.TreasureRoomDrawCost(player.TreasureRoomDraws + 1) : 20;
            int cost = paidDraws * unitCost;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.TreasureRoomResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (_db == null || _db.TreasureRoomPool().Count == 0)
            {
                Send(ns, PhoneMsg.TreasureRoomResult, "{\"ok\":false,\"err\":\"pool\"}");
                return;
            }

            player.Gold -= cost;
            var rewards = new StringBuilder("[");
            for (int i = 0; i < count; i++)
            {
                CarnivalActivityItem drop;
                lock (_lock)
                {
                    drop = _db.RollTreasureRoomItem(_rng);
                }

                if (drop == null)
                {
                    continue;
                }

                int templateId = drop.TemplateId;
                int amount = Mathf.Max(1, drop.Count);
                if (templateId > 100)
                {
                    player.AddItem(templateId, amount);
                }
                else
                {
                    player.Gold += amount * 50;
                }

                if (i > 0)
                {
                    rewards.Append(",");
                }

                rewards.Append("{\"item\":").Append(templateId)
                    .Append(",\"count\":").Append(amount)
                    .Append(",\"quality\":").Append(drop.Quality).Append("}");
            }

            player.TreasureRoomDraws += count;
            SavePlayer(player);
            rewards.Append("]");
            Send(ns, PhoneMsg.TreasureRoomResult,
                "{\"ok\":true,\"cost\":" + cost + ",\"draws\":" + player.TreasureRoomDraws +
                ",\"rewards\":" + rewards + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleChristmasClaim(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.ChristmasDay != today)
            {
                player.ChristmasDay = today;
                player.ChristmasClaims = 0;
            }

            int maxClaims = _db != null ? _db.ConfigInt("ChristmasPreDayCount", 10) : 10;
            if (player.ChristmasClaims >= maxClaims)
            {
                Send(ns, PhoneMsg.ChristmasClaim, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int itemId = 11271;
            int count = 1;
            if (_db != null && _db.ChristmasGifts.Count > 0)
            {
                ChristmasGiftTier tier = _db.ChristmasGifts[player.ChristmasClaims % _db.ChristmasGifts.Count];
                itemId = tier.ItemId;
                count = 1;
            }

            HalloweenRewardItem bonus = null;
            if (_db != null)
            {
                lock (_lock)
                {
                    int level = Mathf.Clamp(player.ChristmasClaims + 1, 1, 7);
                    bonus = _db.RollHalloweenItem(_rng, level);
                }
            }

            player.ChristmasClaims++;
            player.AddItem(itemId, count);
            if (bonus != null)
            {
                player.AddItem(bonus.TemplateId, bonus.Count);
            }

            SavePlayer(player);
            Send(ns, PhoneMsg.ChristmasClaim,
                "{\"ok\":true,\"item\":" + itemId + ",\"count\":" + count +
                ",\"bonus\":" + (bonus != null ? bonus.TemplateId : 0) +
                ",\"claims\":" + player.ChristmasClaims + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandlePcActivityAction(ServerPlayer player, NetworkStream ns, string json, string moduleId, ushort msgId)
        {
            string action = JS(json, "action", "claim");
            if (action != "claim")
            {
                Send(ns, msgId, "{\"ok\":false,\"err\":\"action\"}");
                return;
            }

            if (_db == null)
            {
                Send(ns, msgId, "{\"ok\":false,\"err\":\"config\"}");
                return;
            }

            PcActivityBinding binding = _db.ResolvePcActivity(moduleId);
            List<(int templateId, int count)> rewards = _db.GetPcActivityRewardRows(binding);
            if (rewards.Count == 0)
            {
                Send(ns, msgId, "{\"ok\":false,\"err\":\"config\"}");
                return;
            }

            int today = DateTime.Now.DayOfYear;
            ref int day = ref GetPcActivityDay(player, moduleId);
            ref int claims = ref GetPcActivityClaims(player, moduleId);
            if (day != today)
            {
                day = today;
                claims = 0;
            }

            int maxClaims = _db.GetPcActivityDailyMax(binding);
            if (claims >= maxClaims)
            {
                Send(ns, msgId, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            (int templateId, int count) reward = rewards[claims % rewards.Count];
            player.AddItem(reward.templateId, reward.count);
            claims++;
            SavePlayer(player);
            Send(ns, msgId,
                "{\"ok\":true,\"module\":\"" + moduleId + "\",\"item\":" + reward.templateId +
                ",\"count\":" + reward.count + ",\"claims\":" + claims +
                ",\"activityNum\":" + binding.ActivityConfigNum +
                ",\"source\":\"" + (binding.Source ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        ref int GetPcActivityDay(ServerPlayer player, string moduleId)
        {
            if (moduleId == "bible")
            {
                return ref player.BibleDay;
            }

            return ref player.JigsawDay;
        }

        ref int GetPcActivityClaims(ServerPlayer player, string moduleId)
        {
            if (moduleId == "bible")
            {
                return ref player.BibleClaims;
            }

            return ref player.JigsawClaims;
        }


        void HandleNewYearClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.NewYearDay != today)
            {
                player.NewYearDay = today;
                player.NewYearFreeUsed = 0;
            }

            int rewardId = JI(json, "rewardId", 0);
            if (rewardId > 0)
            {
                if (_db == null)
                {
                    Send(ns, PhoneMsg.NewYearClaim, "{\"ok\":false,\"err\":\"config\"}");
                    return;
                }

                NewYearPointReward row = _db.GetNewYearPointReward(rewardId);
                if (row == null)
                {
                    Send(ns, PhoneMsg.NewYearClaim, "{\"ok\":false,\"err\":\"reward\"}");
                    return;
                }

                if (player.NewYearPointClaimed.Contains(rewardId))
                {
                    Send(ns, PhoneMsg.NewYearClaim, "{\"ok\":false,\"err\":\"claimed\"}");
                    return;
                }

                if (player.NewYearPoints < row.Points)
                {
                    Send(ns, PhoneMsg.NewYearClaim, "{\"ok\":false,\"err\":\"points\"}");
                    return;
                }

                player.NewYearPointClaimed.Add(rewardId);
                _db.GrantColonRewardPairs(player, row.ViewIds);
                SavePlayer(player);
                Send(ns, PhoneMsg.NewYearClaim,
                    "{\"ok\":true,\"rewardId\":" + rewardId + ",\"points\":" + player.NewYearPoints + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }

            int freeMax = _db != null ? _db.ConfigInt("NewYearFreeCount", 3) : 3;
            int buyCost = _db != null ? _db.ConfigInt("NewYearBuyCost", 1000000) : 1000000;
            int pointGain = _db != null ? _db.ConfigInt("NewYearNeedPointLocal", 2000) : 2000;
            bool free = player.NewYearFreeUsed < freeMax;
            if (!free && player.Gold < buyCost)
            {
                Send(ns, PhoneMsg.NewYearClaim, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (!free)
            {
                player.Gold -= buyCost;
            }
            else
            {
                player.NewYearFreeUsed++;
            }

            player.NewYearPoints += pointGain;
            SavePlayer(player);
            Send(ns, PhoneMsg.NewYearClaim,
                "{\"ok\":true,\"action\":\"play\",\"points\":" + player.NewYearPoints +
                ",\"freeUsed\":" + player.NewYearFreeUsed + ",\"gain\":" + pointGain + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleWorshipMoonClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.WorshipMoonDay != today)
            {
                player.WorshipMoonDay = today;
                player.WorshipMoonDraws = 0;
            }

            int maxDraws = _db != null ? _db.ConfigInt("SearchGoodsFreeLimit", 10) : 10;
            if (player.WorshipMoonDraws >= maxDraws)
            {
                Send(ns, PhoneMsg.WorshipMoonClaim, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            (int batchCount, int goldCost) = _db != null ? _db.WorshipMoonPrice() : (1, 100);
            int batches = JI(json, "count", 1);
            batches = Mathf.Clamp(batches, 1, 3);
            int draws = batchCount * batches;
            int cost = goldCost * batches;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.WorshipMoonClaim, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (_db == null)
            {
                Send(ns, PhoneMsg.WorshipMoonClaim, "{\"ok\":false,\"err\":\"config\"}");
                return;
            }

            player.Gold -= cost;
            var rewards = new StringBuilder("[");
            for (int i = 0; i < draws; i++)
            {
                int rewardIndex;
                lock (_lock)
                {
                    rewardIndex = _db.RollWorshipMoonRewardIndex(_rng);
                }

                int templateId = _db.WorshipMoonRewardId(rewardIndex);
                player.AddItem(templateId, 1);
                if (i > 0)
                {
                    rewards.Append(",");
                }

                rewards.Append("{\"item\":").Append(templateId).Append(",\"count\":1}");
            }

            int tenReward = _db.ConfigInt("WorshipTenReward", 0);
            if (tenReward > 0 && player.WorshipMoonDraws + draws >= 10 &&
                player.WorshipMoonDraws < 10)
            {
                player.AddItem(tenReward, 1);
                rewards.Append(",{\"item\":").Append(tenReward).Append(",\"count\":1,\"bonus\":1}");
            }

            player.WorshipMoonDraws += draws;
            SavePlayer(player);
            rewards.Append("]");
            Send(ns, PhoneMsg.WorshipMoonClaim,
                "{\"ok\":true,\"cost\":" + cost + ",\"draws\":" + player.WorshipMoonDraws +
                ",\"rewards\":" + rewards + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleSuperLuckerDraw(ServerPlayer player, NetworkStream ns, string json)
        {
            int count = JI(json, "count", 1);
            count = Mathf.Clamp(count, 1, 10);
            int today = DateTime.Now.DayOfYear;
            if (player.SuperLuckerDay != today)
            {
                player.SuperLuckerDay = today;
                player.SuperLuckerDraws = 0;
            }

            int unitCost = _db != null ? _db.SuperLuckerDrawCost() : 500;
            int cost = unitCost * count;
            if (player.Gold < cost)
            {
                Send(ns, PhoneMsg.SuperLuckerDraw, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (_db == null || _db.SuperLuckerPool().Count == 0)
            {
                Send(ns, PhoneMsg.SuperLuckerDraw, "{\"ok\":false,\"err\":\"pool\"}");
                return;
            }

            player.Gold -= cost;
            var rewards = new StringBuilder("[");
            for (int i = 0; i < count; i++)
            {
                CarnivalActivityItem drop;
                lock (_lock)
                {
                    drop = _db.RollSuperLuckerItem(_rng);
                }

                if (drop == null)
                {
                    continue;
                }

                int templateId = drop.TemplateId;
                int amount = Mathf.Max(1, drop.Count);
                if (templateId > 100)
                {
                    player.AddItem(templateId, amount);
                }
                else
                {
                    player.Gold += amount * 50;
                }

                if (i > 0)
                {
                    rewards.Append(",");
                }

                rewards.Append("{\"item\":").Append(templateId)
                    .Append(",\"count\":").Append(amount)
                    .Append(",\"quality\":").Append(drop.Quality).Append("}");
            }

            player.SuperLuckerDraws += count;
            SavePlayer(player);
            rewards.Append("]");
            Send(ns, PhoneMsg.SuperLuckerDraw,
                "{\"ok\":true,\"cost\":" + cost + ",\"draws\":" + player.SuperLuckerDraws +
                ",\"rewards\":" + rewards + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleQuizAnswer(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.QuizAnswer, "{\"ok\":false,\"err\":\"config\"}");
                return;
            }

            player.TouchQuizDay();
            int max = _db.DailyQuizMax();
            if (player.QuizAttempts >= max)
            {
                Send(ns, PhoneMsg.QuizAnswer, "{\"ok\":false,\"err\":\"limit\",\"attempts\":" + player.QuizAttempts + ",\"max\":" + max + "}");
                return;
            }

            int questionId = JI(json, "questionId", 0);
            QuizQuestion q = _db.GetQuizQuestion(questionId) ?? _db.PickQuizQuestion(player.QuizAttempts);
            if (q == null)
            {
                Send(ns, PhoneMsg.QuizAnswer, "{\"ok\":false,\"err\":\"none\"}");
                return;
            }

            int option = JI(json, "option", 0);
            if (option <= 0)
            {
                option = JI(json, "answer", 0);
            }

            if (option < 1 || option > 4)
            {
                Send(ns, PhoneMsg.QuizAnswer, "{\"ok\":false,\"err\":\"option\"}");
                return;
            }

            int gold = _db.QuizGoldReward();
            bool correct = option == q.CorrectOption;
            player.Gold += gold;
            player.QuizAttempts++;
            SavePlayer(player);
            Send(ns, PhoneMsg.QuizAnswer, "{\"ok\":true,\"correct\":" + (correct ? "true" : "false") +
                ",\"questionId\":" + q.QuestionId + ",\"option\":" + option +
                ",\"gold\":" + gold + ",\"attempts\":" + player.QuizAttempts + ",\"max\":" + max + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleOneYuanBuy(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.OneYuanBuy, "{\"ok\":false,\"err\":\"config\"}");
                return;
            }

            int id = JI(json, "id", 0);
            int goodsId = JI(json, "goodsId", 0);
            OneYuanGoods row = _db.GetOneYuanGoods(id, goodsId);
            if (row == null)
            {
                Send(ns, PhoneMsg.OneYuanBuy, "{\"ok\":false,\"err\":\"goods\"}");
                return;
            }

            player.TouchOneYuanDay();
            int bought = 0;
            for (int i = 0; i < player.OneYuanBought.Count; i++)
            {
                if (player.OneYuanBought[i] == row.GoodsId)
                {
                    bought++;
                }
            }

            int limit = _db.OneYuanDailyLimit(row);
            if (bought >= limit)
            {
                Send(ns, PhoneMsg.OneYuanBuy, "{\"ok\":false,\"err\":\"limit\",\"goodsId\":" + row.GoodsId + "}");
                return;
            }

            int cost = Mathf.Max(0, row.Cost);
            bool gift = row.IsBindMoney != 0;
            if (gift)
            {
                if (player.Gift < cost)
                {
                    Send(ns, PhoneMsg.OneYuanBuy, "{\"ok\":false,\"err\":\"gift\"}");
                    return;
                }

                player.Gift -= cost;
            }
            else
            {
                if (player.Gold < cost)
                {
                    Send(ns, PhoneMsg.OneYuanBuy, "{\"ok\":false,\"err\":\"gold\"}");
                    return;
                }

                player.Gold -= cost;
            }

            player.AddItem(row.GoodsId, 1);
            player.OneYuanBought.Add(row.GoodsId);
            SavePlayer(player);
            Send(ns, PhoneMsg.OneYuanBuy, "{\"ok\":true,\"id\":" + row.Id + ",\"goodsId\":" + row.GoodsId +
                ",\"cost\":" + cost + ",\"gift\":" + (gift ? "true" : "false") + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleCalendarClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.CalendarClaim, "{\"ok\":false,\"err\":\"config\"}"); return; }
            player.TouchCalendarMonth();
            int dayIndex = JI(json, "dayIndex", DateTime.Now.Day);
            if (dayIndex < 1 || dayIndex > DateTime.Now.Day) { Send(ns, PhoneMsg.CalendarClaim, "{\"ok\":false,\"err\":\"day\"}"); return; }
            if (player.CalendarClaimedDays.Contains(dayIndex)) { Send(ns, PhoneMsg.CalendarClaim, "{\"ok\":false,\"err\":\"claimed\"}"); return; }
            SignReward reward = _db.GetCalendarDayReward(dayIndex);
            if (reward == null || reward.TemplateId <= 0) { Send(ns, PhoneMsg.CalendarClaim, "{\"ok\":false,\"err\":\"reward\"}"); return; }
            player.CalendarClaimedDays.Add(dayIndex);
            player.AddItem(reward.TemplateId, Mathf.Max(1, reward.Count));
            SavePlayer(player);
            Send(ns, PhoneMsg.CalendarClaim, "{\"ok\":true,\"dayIndex\":" + dayIndex + ",\"itemId\":" + reward.TemplateId + ",\"count\":" + reward.Count + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleAuditoriumAction(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.AuditoriumAction, "{\"ok\":false,\"err\":\"config\"}"); return; }
            string action = JS(json, "action", "wedding");
            player.TouchAuditoriumDay();
            if (player.AuditoriumActions >= _db.ConfigInt("HonorSystemAwardLimit", 6)) { Send(ns, PhoneMsg.AuditoriumAction, "{\"ok\":false,\"err\":\"limit\"}"); return; }
            int cost = 0, honorGain = _db.HonorSystemLikeHonorGain(), itemId = 0;
            if (string.Equals(action, "wedding", StringComparison.OrdinalIgnoreCase)) { cost = _db.AuditoriumWeddingCost(JI(json, "tier", 0)); honorGain = Mathf.Max(honorGain, cost / 3000); }
            else if (string.Equals(action, "fire", StringComparison.OrdinalIgnoreCase))
            {
                FireworkEntry fw = _db.GetFireworkEntry(JI(json, "index", 0));
                if (fw == null) { Send(ns, PhoneMsg.AuditoriumAction, "{\"ok\":false,\"err\":\"firework\"}"); return; }
                cost = fw.GoldCost; honorGain = fw.HonorGain; itemId = fw.TemplateId;
            }
            else if (string.Equals(action, "redpacket", StringComparison.OrdinalIgnoreCase)) { cost = _db.ConfigInt("RedPacketMinGold", 100) * 10; honorGain = Mathf.Max(honorGain, 15); }
            else { Send(ns, PhoneMsg.AuditoriumAction, "{\"ok\":false,\"err\":\"action\"}"); return; }
            if (player.Gold < cost) { Send(ns, PhoneMsg.AuditoriumAction, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            player.Gold -= cost; player.Honor += honorGain; if (itemId > 0) player.AddItem(itemId, 1);
            player.AuditoriumActions++; player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.AuditoriumAction, "{\"ok\":true,\"action\":\"" + action + "\",\"cost\":" + cost + ",\"honorGain\":" + honorGain + ",\"actions\":" + player.AuditoriumActions + "}");
            Send(ns, PhoneMsg.StatResult, player.ToJson()); Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleBoguAdventureAction(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.BoguAdventureAction, "{\"ok\":false,\"err\":\"config\"}"); return; }
            int activityNum = JI(json, "activityNum", 5);
            string action = JS(json, "action", "spin");
            player.TouchBoguAdventureDay();
            if (player.BoguAdventureActions >= _db.ConfigInt("MineDayLimit", 5) * 4) { Send(ns, PhoneMsg.BoguAdventureAction, "{\"ok\":false,\"err\":\"limit\"}"); return; }
            if (!_db.ActivityConfigs.TryGetValue(activityNum, out ActivityConfigEntry cfg) || cfg == null)
            { Send(ns, PhoneMsg.BoguAdventureAction, "{\"ok\":false,\"err\":\"activity\",\"activityNum\":" + activityNum + "}"); return; }
            int cost = 0, itemId = 0, count = 1;
            if (string.Equals(action, "spin", StringComparison.OrdinalIgnoreCase)) { cost = _db.BoguAdventureSpinCost(JI(json, "tier", 0)); itemId = _db.BoguAdventureRewardItemId(); }
            else if (string.Equals(action, "reset", StringComparison.OrdinalIgnoreCase))
            {
                cost = 200;
                if (!string.IsNullOrEmpty(cfg.Params2)) { string[] p = cfg.Params2.Split(','); if (p.Length > 1 && int.TryParse(p[1].Trim(), out int rc)) cost = Mathf.Abs(rc); }
            }
            else if (string.Equals(action, "sign", StringComparison.OrdinalIgnoreCase) || string.Equals(action, "findMine", StringComparison.OrdinalIgnoreCase))
            { itemId = _db.BoguAdventureRewardItemId(); count = string.Equals(action, "findMine", StringComparison.OrdinalIgnoreCase) ? 2 : 1; }
            else if (string.Equals(action, "getAward", StringComparison.OrdinalIgnoreCase)) { itemId = _db.BoguAdventureRewardItemId(); count = 3; }
            else { Send(ns, PhoneMsg.BoguAdventureAction, "{\"ok\":false,\"err\":\"action\"}"); return; }
            if (cost > 0 && player.Gold < cost) { Send(ns, PhoneMsg.BoguAdventureAction, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            if (cost > 0) player.Gold -= cost; if (itemId > 0) player.AddItem(itemId, count);
            player.BoguAdventureActions++; SavePlayer(player);
            Send(ns, PhoneMsg.BoguAdventureAction, "{\"ok\":true,\"action\":\"" + action + "\",\"activityNum\":" + activityNum + ",\"activityName\":\"" + (cfg.Name ?? "").Replace("\"", "") + "\",\"cost\":" + cost + ",\"itemId\":" + itemId + ",\"count\":" + count + ",\"actions\":" + player.BoguAdventureActions + "}");
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

        void HandleDevilTreasPointClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int rewardId = JI(json, "rewardId", 0);
            DevilTreasPointReward reward = _db != null ? _db.GetDevilTreasPointReward(rewardId) : null;
            if (reward == null || reward.TemplateId <= 0)
            {
                Send(ns, PhoneMsg.DevilTreasPointClaim, "{\"ok\":false,\"err\":\"reward\"}");
                return;
            }

            player.EnsureDevilTreasPointClaimed();
            if (player.DevilTreasPointClaimed.Contains(rewardId) || player.DevilTurnPoints < reward.Points)
            {
                Send(ns, PhoneMsg.DevilTreasPointClaim, "{\"ok\":false,\"err\":\"points\"}");
                return;
            }

            player.DevilTreasPointClaimed.Add(rewardId);
            player.GrantTemplateReward(_db, reward.TemplateId, 1);
            SavePlayer(player);
            Send(ns, PhoneMsg.DevilTreasPointClaim,
                "{\"ok\":true,\"rewardId\":" + rewardId + ",\"profile\":" + player.ToJson() + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleRedPacketSend(ServerPlayer player, NetworkStream ns, string json)
        {
            string friend = JS(json, "friend", "");
            int gold = Mathf.Clamp(JI(json, "gold", 0), 1, 50000);
            if (string.IsNullOrEmpty(friend) || !player.Friends.Contains(friend))
            {
                Send(ns, PhoneMsg.RedPacketSend, "{\"ok\":false,\"err\":\"friend\"}");
                return;
            }

            if (player.Gold < gold)
            {
                Send(ns, PhoneMsg.RedPacketSend, "{\"ok\":false,\"err\":\"gold\"}");
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
                Send(ns, PhoneMsg.RedPacketSend, "{\"ok\":false,\"err\":\"offline\"}");
                return;
            }

            player.Gold -= gold;
            target.Gold += gold;
            SavePlayer(player);
            SavePlayer(target);
            Send(ns, PhoneMsg.RedPacketSend,
                "{\"ok\":true,\"gold\":" + gold + ",\"friend\":\"" + friend.Replace("\"", "\\\"") + "\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
            SendTo(target, PhoneMsg.ProfileData, target.ToJson());
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

            var mail = new ServerMail
            {
                Subject = subject,
                Body = string.IsNullOrEmpty(body)
                    ? "来自 " + (player.Nick ?? "Player") + " 的邮件。"
                    : body,
                Gold = gold,
                ItemId = itemId,
                ItemCount = itemCount
            };

            bool delivered = false;
            lock (_lock)
            {
                // Prefer in-memory _players (including disconnected sessions with null RoadStream).
                ServerPlayer target = FindPlayerByNick(to, player);
                if (target != null)
                {
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

                    mail.Id = target.NextMailId++;
                    target.Mails.Add(mail);
                    SavePlayer(player);
                    SavePlayer(target);
                    SendTo(target, PhoneMsg.MailListData, BuildMailListJson(target));
                    delivered = true;
                }
                else if (TryAppendMailToSaveFile(to, mail))
                {
                    if (gold > 0)
                    {
                        player.Gold -= gold;
                    }

                    if (itemId > 0 && itemCount > 0)
                    {
                        player.Consume(itemId, itemCount);
                    }

                    SavePlayer(player);
                    delivered = true;
                }
            }

            if (!delivered)
            {
                Send(ns, PhoneMsg.MailSend, "{\"ok\":false,\"err\":\"offline\"}");
                return;
            }

            Send(ns, PhoneMsg.MailSend, "{\"ok\":true,\"to\":\"" + to.Replace("\"", "") + "\"}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleChatWhisper(ServerPlayer player, NetworkStream ns, string json)
        {
            string to = JS(json, "to", "");
            string msg = JS(json, "msg", "");
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrEmpty(msg))
            {
                Send(ns, PhoneMsg.Error, "{\"err\":\"whisper\"}");
                return;
            }

            to = to.Trim();
            ServerPlayer target;
            lock (_lock)
            {
                target = FindPlayerByNick(to, player);
            }

            if (target == null || target.RoadStream == null)
            {
                Send(ns, PhoneMsg.Error, "{\"err\":\"offline\"}");
                return;
            }

            string broadcast = "{\"from\":\"" + (player.Nick ?? "").Replace("\"", "") +
                               "\",\"to\":\"" + to.Replace("\"", "") +
                               "\",\"whisper\":true,\"msg\":\"" + msg.Replace("\"", "") + "\"}";
            SendTo(target, PhoneMsg.ChatBroadcast, broadcast);
            Send(ns, PhoneMsg.ChatBroadcast, broadcast);
        }

        void HandleFirstRechargeClaim(ServerPlayer player, NetworkStream ns)
        {
            if (player.FirstRechargeClaimed)
            {
                Send(ns, PhoneMsg.FirstRechargeClaim, "{\"ok\":false,\"err\":\"claimed\"}");
                return;
            }

            if (_db == null)
            {
                Send(ns, PhoneMsg.FirstRechargeClaim, "{\"ok\":false}");
                return;
            }

            FirstRechargeConfig cfg = _db.GetFirstRechargeConfig();
            if (cfg == null)
            {
                Send(ns, PhoneMsg.FirstRechargeClaim, "{\"ok\":false,\"err\":\"config\"}");
                return;
            }

            var granted = new StringBuilder("[");
            bool first = true;
            for (int i = 0; i < cfg.RewardItemIds.Length; i++)
            {
                int itemId = cfg.RewardItemIds[i];
                if (itemId <= 0)
                {
                    continue;
                }

                int count = i < cfg.RewardCounts.Length ? cfg.RewardCounts[i] : 1;
                count = Mathf.Max(1, count);
                player.AddItem(itemId, count);
                if (!first) granted.Append(",");
                first = false;
                granted.Append("{\"itemId\":").Append(itemId).Append(",\"count\":").Append(count).Append("}");
            }

            if (cfg.ExtraItemId1 > 0)
            {
                player.AddItem(cfg.ExtraItemId1, 1);
                if (!first) granted.Append(",");
                first = false;
                granted.Append("{\"itemId\":").Append(cfg.ExtraItemId1).Append(",\"count\":1}");
            }

            if (cfg.ExtraItemId2 > 0)
            {
                player.AddItem(cfg.ExtraItemId2, 1);
                if (!first) granted.Append(",");
                first = false;
                granted.Append("{\"itemId\":").Append(cfg.ExtraItemId2).Append(",\"count\":1}");
            }

            if (cfg.RankAwardId > 0)
            {
                player.AddItem(cfg.RankAwardId, 1);
                if (!first) granted.Append(",");
                granted.Append("{\"itemId\":").Append(cfg.RankAwardId).Append(",\"count\":1}");
            }

            granted.Append("]");
            player.FirstRechargeClaimed = true;
            SavePlayer(player);
            Send(ns, PhoneMsg.FirstRechargeClaim, "{\"ok\":true,\"items\":" + granted + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleFirstRechargeShop(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 0);
            int count = Mathf.Clamp(JI(json, "count", 1), 1, 99);
            if (templateId <= 0 || _db == null)
            {
                Send(ns, PhoneMsg.FirstRechargeShop, "{\"ok\":false}");
                return;
            }

            FirstPayShopItem offer = _db.GetFirstPayShopItem(templateId);
            if (offer == null)
            {
                Send(ns, PhoneMsg.FirstRechargeShop, "{\"ok\":false,\"err\":\"item\"}");
                return;
            }

            player.FirstRechargeShopBuys.TryGetValue(templateId, out int bought);
            if (bought + count > offer.LimitBuyCount)
            {
                Send(ns, PhoneMsg.FirstRechargeShop, "{\"ok\":false,\"err\":\"limit\"}");
                return;
            }

            int cost = offer.NeedGoldBeans * count;
            if (player.Gift < cost)
            {
                Send(ns, PhoneMsg.FirstRechargeShop, "{\"ok\":false,\"err\":\"beans\"}");
                return;
            }

            player.Gift -= cost;
            int giveCount = offer.ItemTempCount * count;
            player.AddItem(offer.ItemTempId, giveCount);
            player.FirstRechargeShopBuys[templateId] = bought + count;
            SavePlayer(player);
            Send(ns, PhoneMsg.FirstRechargeShop,
                "{\"ok\":true,\"templateId\":" + templateId + ",\"count\":" + count +
                ",\"itemId\":" + offer.ItemTempId + ",\"itemCount\":" + giveCount +
                ",\"cost\":" + cost + ",\"bought\":" + (bought + count) + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
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
        static GodCardSlot GetGodCardSlot(ServerPlayer player, int id) => player.FindGodCardSlot(id);
        static bool ConsumeGodCards(ServerPlayer player, int id, int count)
        {
            GodCardSlot slot = GetGodCardSlot(player, id);
            if (slot == null || slot.Count < count) return false;
            slot.Count -= count;
            if (slot.Count <= 0) { player.GodCards.Remove(slot); if (player.GodCardEquipId == id) player.GodCardEquipId = 0; }
            return true;
        }
        void SyncGodCardGrooveLevel(ServerPlayer player, GodCardSlot slot, GodCardInfo card)
        {
            if (_db == null || slot == null || card == null) return;
            int type = _db.GodCardGrooveType(card), maxLevel = _db.MaxCardGrooveLevel(type);
            while (slot.GrooveLevel < maxLevel && slot.GrooveExp >= _db.NextCardGrooveExp(type, slot.GrooveLevel)) slot.GrooveLevel++;
        }
        void HandleGodCardRaise(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null || _db.GodCards.Count == 0) { Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":false,\"err\":\"no table\"}"); return; }
            int cardId = JI(json, "cardId", 0), useCount = JI(json, "count", 1);
            if (cardId <= 0 || useCount <= 0 || !_db.GodCards.TryGetValue(cardId, out GodCardInfo card)) { Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":false,\"err\":\"card\"}"); return; }
            GodCardSlot slot = GetGodCardSlot(player, cardId);
            if (slot == null || slot.Count - useCount < 1) { Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":false,\"err\":\"not enough\"}"); return; }
            int type = _db.GodCardGrooveType(card);
            if (slot.GrooveLevel >= _db.MaxCardGrooveLevel(type)) { Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":false,\"err\":\"max\"}"); return; }
            int expGain = _db.GodCardRaiseExpGain(card), pointGain = _db.GodCardRaisePointGain(card);
            if (!ConsumeGodCards(player, cardId, useCount)) { Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":false,\"err\":\"consume\"}"); return; }
            slot = GetGodCardSlot(player, cardId);
            if (slot == null) { Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":false,\"err\":\"consume\"}"); return; }
            slot.GrooveExp += expGain * useCount; player.GodCardPoints += pointGain * useCount;
            SyncGodCardGrooveLevel(player, slot, card); player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.GodCardRaise, "{\"ok\":true,\"cardId\":" + cardId + ",\"grooveLevel\":" + slot.GrooveLevel + ",\"grooveExp\":" + slot.GrooveExp + ",\"godCardPoints\":" + player.GodCardPoints + ",\"profile\":" + player.ToJson() + "}");
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }
        void HandleGodCardPointClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            int rewardId = JI(json, "rewardId", 0);
            GodCardPointRewardInfo reward = rewardId > 0 && _db != null ? _db.GetGodCardPointReward(rewardId) : null;
            if (reward == null || reward.ItemId <= 0) { Send(ns, PhoneMsg.GodCardPointClaim, "{\"ok\":false,\"err\":\"reward\"}"); return; }
            player.EnsureGodCardPointClaimed();
            if (player.GodCardPointClaimed.Contains(rewardId) || player.GodCardPoints < reward.Point) { Send(ns, PhoneMsg.GodCardPointClaim, "{\"ok\":false,\"err\":\"points\"}"); return; }
            player.GodCardPointClaimed.Add(rewardId); player.AddItem(reward.ItemId, reward.Count > 0 ? reward.Count : 1);
            SavePlayer(player);
            Send(ns, PhoneMsg.GodCardPointClaim, "{\"ok\":true,\"rewardId\":" + rewardId + ",\"profile\":" + player.ToJson() + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
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
                    Send(ns, PhoneMsg.QuestResult, "{\"ok\":false,\"err\":\"not ready\"}");
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

        void HandlePetStarUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.PetStarUpgrade, "{\"ok\":false}");
                return;
            }

            PetStarUpgrade row = _db.GetPetStarUpgrade(player.PetId);
            if (row == null || row.NewId <= 0)
            {
                Send(ns, PhoneMsg.PetStarUpgrade, "{\"ok\":false,\"err\":\"none\"}");
                return;
            }

            int cost = Mathf.Max(0, row.Exp);
            bool payGold = cost > 0 && player.Gold >= cost;
            bool payGp = cost > 0 && !payGold && player.Gp >= cost;
            if (cost > 0 && !payGold && !payGp)
            {
                Send(ns, PhoneMsg.PetStarUpgrade, "{\"ok\":false,\"err\":\"cost\"}");
                return;
            }

            if (payGold)
            {
                player.Gold -= cost;
            }
            else if (payGp)
            {
                player.Gp -= cost;
            }

            player.PetId = row.NewId;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.PetStarUpgrade, "{\"ok\":true,\"petId\":" + player.PetId + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleMountTalismanEquip(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.MountTalismanEquip, "{\"ok\":false}");
                return;
            }

            int talismanId = JI(json, "talismanId", 0);
            MountTalismanInfo row = _db.GetMountTalisman(talismanId);
            if (row == null)
            {
                Send(ns, PhoneMsg.MountTalismanEquip, "{\"ok\":false,\"err\":\"none\"}");
                return;
            }

            if (player.MountTalismanId != talismanId && row.Consume > 0 && player.Gold < row.Consume)
            {
                Send(ns, PhoneMsg.MountTalismanEquip, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (player.MountTalismanId != talismanId && row.Consume > 0)
            {
                player.Gold -= row.Consume;
            }

            player.MountTalismanId = talismanId;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.MountTalismanEquip, "{\"ok\":true,\"talismanId\":" + talismanId + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleManorUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (_db == null)
            {
                Send(ns, PhoneMsg.ManorUpgrade, "{\"ok\":false}");
                return;
            }

            int current = player.ManorGrade > 0 ? player.ManorGrade : 1;
            ManorPlantInfo next = _db.GetManorPlant(1, current + 1);
            if (next == null)
            {
                Send(ns, PhoneMsg.ManorUpgrade, "{\"ok\":false,\"err\":\"max\"}");
                return;
            }

            if (next.NeedGrade1 > 0 && player.Level < next.NeedGrade1)
            {
                Send(ns, PhoneMsg.ManorUpgrade, "{\"ok\":false,\"err\":\"level\"}");
                return;
            }

            int cost = _db.ManorUpgradeCost(current);
            if (cost > 0 && player.Gold < cost)
            {
                Send(ns, PhoneMsg.ManorUpgrade, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }

            if (cost > 0)
            {
                player.Gold -= cost;
            }

            player.ManorGrade = current + 1;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.ManorUpgrade, "{\"ok\":true,\"grade\":" + player.ManorGrade + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleGoldEquipUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.GoldEquipUpgrade, "{\"ok\":false}"); return; }
            int oldId = JI(json, "oldTemplateId", player.EquipWeapon);
            GoldEquipTemplate row = _db.GetGoldEquipByOld(oldId);
            if (row == null || row.NewTemplateId <= 0) { Send(ns, PhoneMsg.GoldEquipUpgrade, "{\"ok\":false,\"err\":\"none\"}"); return; }
            if (player.EquipWeapon != row.OldTemplateId) { Send(ns, PhoneMsg.GoldEquipUpgrade, "{\"ok\":false,\"err\":\"weapon\"}"); return; }
            int cost = _db.GoldEquipUpgradeGoldCost(row);
            if (cost > 0 && player.Gold < cost) { Send(ns, PhoneMsg.GoldEquipUpgrade, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            if (cost > 0) player.Gold -= cost;
            foreach (BagSlot slot in player.Bag)
            {
                if (slot.TemplateId == row.OldTemplateId) slot.TemplateId = row.NewTemplateId;
            }
            player.EquipWeapon = row.NewTemplateId;
            player.WeaponId = row.NewTemplateId;
            player.GoldEquipId = row.Id;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.GoldEquipUpgrade, "{\"ok\":true,\"oldTemplateId\":" + row.OldTemplateId + ",\"newTemplateId\":" + row.NewTemplateId + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleGloryUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.GloryUpgrade, "{\"ok\":false}"); return; }
            int templateId = JI(json, "templateId", player.GloryTemplateId);
            GloryItemUpgrade row = _db.GetGloryUpgrade(templateId);
            if (row == null) { Send(ns, PhoneMsg.GloryUpgrade, "{\"ok\":false,\"err\":\"none\"}"); return; }
            int goldCost = _db.GloryUpgradeGoldCost(row);
            bool usedItem = row.CostItemId > 0 && player.Consume(row.CostItemId, 1);
            if (!usedItem && row.CostItemId > 0) goldCost += _db.GloryCostItemGoldFallback(row);
            if (goldCost > 0 && player.Gold < goldCost)
            {
                if (usedItem) player.AddItem(row.CostItemId, 1);
                Send(ns, PhoneMsg.GloryUpgrade, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }
            if (goldCost > 0) player.Gold -= goldCost;
            player.GloryTemplateId = row.NextTemplateId > 0 ? row.NextTemplateId : row.TemplateId;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.GloryUpgrade, "{\"ok\":true,\"templateId\":" + player.GloryTemplateId + ",\"cost\":" + goldCost + ",\"usedItem\":" + (usedItem ? "true" : "false") + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleSigilRoll(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.SigilRoll, "{\"ok\":false}"); return; }
            int quality = JI(json, "quality", player.SigilQuality > 0 ? player.SigilQuality : 1);
            if (quality <= 0) quality = 1;
            int cost = _db.SigilRollGoldCost();
            if (cost > 0 && player.Gold < cost) { Send(ns, PhoneMsg.SigilRoll, "{\"ok\":false,\"err\":\"gold\"}"); return; }
            SigilProLimit rolled;
            lock (_lock) { rolled = _db.RollSigil(quality, _rng); }
            if (rolled == null) { Send(ns, PhoneMsg.SigilRoll, "{\"ok\":false,\"err\":\"none\"}"); return; }
            if (cost > 0) player.Gold -= cost;
            player.SigilQuality = quality;
            player.SigilProType = rolled.ProType;
            player.SigilProValue = _db.SigilBonusValue(rolled);
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.SigilRoll, "{\"ok\":true,\"quality\":" + quality + ",\"proType\":" + player.SigilProType + ",\"proValue\":" + player.SigilProValue + ",\"cost\":" + cost + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
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


        int TryRemapStrengthenGoods(ServerPlayer player, BagSlot slot)
        {
            if (_db == null || slot == null) return 0;
            StrengthenGoodsInfo map = _db.FindStrengthenGoodsRemap(slot.TemplateId, slot.Strengthen);
            if (map == null || map.GainEquip <= 0 || map.GainEquip == slot.TemplateId) return 0;
            int oldId = slot.TemplateId;
            slot.TemplateId = map.GainEquip;
            if (player.EquipWeapon == oldId) { player.EquipWeapon = map.GainEquip; player.WeaponId = map.GainEquip; }
            if (player.EquipHead == oldId) player.EquipHead = map.GainEquip;
            if (player.EquipHair == oldId) player.EquipHair = map.GainEquip;
            if (player.EquipFace == oldId) player.EquipFace = map.GainEquip;
            if (player.EquipCloth == oldId) player.EquipCloth = map.GainEquip;
            if (player.EquipGlass == oldId) player.EquipGlass = map.GainEquip;
            return map.GainEquip;
        }

        void HandleCardBookletClaim(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":false,\"err\":\"config\"}"); return; }
            int templateId = JI(json, "templateId", 0);
            string action = JS(json, "action", "claim");
            int profile = JI(json, "profile", -999);
            player.EnsureOwnedCards();

            if (action == "draw" || action == "open")
            {
                if (templateId <= 0 && _db.CardBooklets.Count > 0) templateId = _db.CardBooklets[0].TemplateId;
                CardBookletInfo rolled;
                lock (_lock) { rolled = _db.RollCardBooklet(templateId, _rng); }
                if (rolled == null) { Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":false,\"err\":\"none\"}"); return; }
                player.SetCardBookletProfile(rolled.TemplateId, rolled.Profile);
                player.RecalcStats(_db); SavePlayer(player);
                Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":true,\"action\":\"draw\",\"templateId\":" + rolled.TemplateId +
                    ",\"profile\":" + rolled.Profile + ",\"name\":\"" + (rolled.TemplateName ?? "").Replace("\"", "") + "\"}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }

            if (action == "recycle")
            {
                int idx = player.OwnedCardTemplateIds.IndexOf(templateId);
                if (idx < 0) { Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":false,\"err\":\"owned\"}"); return; }
                int prof = profile != -999 ? profile : (idx < player.CardBookletProfiles.Count ? player.CardBookletProfiles[idx] : 0);
                CardBookletInfo row = _db.GetCardBooklet(templateId, prof) ?? _db.GetCardBooklet(templateId, 0);
                int soul = row != null ? Mathf.Max(1, row.RecyclCount) : 5;
                player.OwnedCardTemplateIds.RemoveAt(idx);
                if (idx < player.CardBookletProfiles.Count) player.CardBookletProfiles.RemoveAt(idx);
                player.CardSoul += soul;
                player.RecalcStats(_db); SavePlayer(player);
                Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":true,\"action\":\"recycle\",\"templateId\":" + templateId + ",\"soul\":" + soul + ",\"cardSoul\":" + player.CardSoul + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }

            if (templateId <= 0) { Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":false,\"err\":\"template\"}"); return; }
            if (!player.OwnedCardTemplateIds.Contains(templateId))
            {
                CardBookletInfo baseRow = _db.GetCardBooklet(templateId, 0);
                if (baseRow == null) { Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":false,\"err\":\"none\"}"); return; }
                player.SetCardBookletProfile(templateId, 0);
            }
            if (player.HasCardBookletClaimed(templateId)) { Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":false,\"err\":\"claimed\"}"); return; }
            int ownIdx = player.OwnedCardTemplateIds.IndexOf(templateId);
            int ownProf = ownIdx >= 0 && ownIdx < player.CardBookletProfiles.Count ? player.CardBookletProfiles[ownIdx] : 0;
            if (profile != -999) ownProf = profile;
            CardBookletInfo reward = _db.GetCardBooklet(templateId, ownProf) ?? _db.GetCardBooklet(templateId, 0);
            int gold = reward != null ? Mathf.Max(10, reward.RecyclCount * 10) : 50;
            player.Gold += gold;
            player.CardBookletClaimed.Add(templateId);
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.CardBookletClaim, "{\"ok\":true,\"action\":\"claim\",\"templateId\":" + templateId +
                ",\"profile\":" + ownProf + ",\"gold\":" + gold + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleStrengthenGoodsMap(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.StrengthenGoodsMap, "{\"ok\":false,\"err\":\"config\"}"); return; }
            int templateId = JI(json, "templateId", 0);
            int level = JI(json, "level", -1);
            bool apply = JI(json, "apply", 0) != 0 || JS(json, "action", "") == "remap";
            BagSlot slot = null;
            foreach (var s in player.Bag) { if (s.TemplateId == templateId) { slot = s; break; } }
            if (level < 0) level = slot != null ? slot.Strengthen : 0;
            StrengthenGoodsInfo map = _db.GetStrengthenGoodsMap(templateId, level) ?? _db.FindStrengthenGoodsRemap(templateId, level);
            if (map == null)
            {
                Send(ns, PhoneMsg.StrengthenGoodsMap, "{\"ok\":false,\"err\":\"none\",\"templateId\":" + templateId + ",\"level\":" + level + "}");
                return;
            }
            int gain = map.GainEquip;
            if (apply && slot != null && gain > 0 && gain != slot.TemplateId)
            {
                slot.Strengthen = Mathf.Max(slot.Strengthen, map.Level);
                int remapped = TryRemapStrengthenGoods(player, slot);
                player.RecalcStats(_db); SavePlayer(player);
                Send(ns, PhoneMsg.StrengthenGoodsMap, "{\"ok\":true,\"applied\":true,\"currentEquip\":" + templateId +
                    ",\"level\":" + map.Level + ",\"gainEquip\":" + gain + ",\"templateId\":" + (remapped > 0 ? remapped : slot.TemplateId) + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }
            Send(ns, PhoneMsg.StrengthenGoodsMap, "{\"ok\":true,\"applied\":false,\"currentEquip\":" + map.CurrentEquip +
                ",\"level\":" + map.Level + ",\"gainEquip\":" + map.GainEquip + ",\"originalEquip\":" + map.OriginalEquip + "}");
        }

        void HandleBoxOpen(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.BoxOpen, "{\"ok\":false,\"err\":\"config\"}"); return; }
            int templateId = JI(json, "templateId", 0);
            if (templateId <= 0) { Send(ns, PhoneMsg.BoxOpen, "{\"ok\":false,\"err\":\"template\"}"); return; }
            if (_db.GetBoxDrops(templateId).Count == 0) { Send(ns, PhoneMsg.BoxOpen, "{\"ok\":false,\"err\":\"box\"}"); return; }
            if (!player.Consume(templateId, 1)) { Send(ns, PhoneMsg.BoxOpen, "{\"ok\":false,\"err\":\"bag\"}"); return; }
            BoxTempDrop drop;
            lock (_lock) { drop = _db.RollBoxDrop(templateId, _rng); }
            if (drop == null) { player.AddItem(templateId, 1); Send(ns, PhoneMsg.BoxOpen, "{\"ok\":false,\"err\":\"roll\"}"); return; }
            int rewardId = drop.TemplateId;
            int count = Mathf.Max(1, drop.ItemCount);
            if (rewardId < 0)
            {
                int gold = Mathf.Abs(rewardId) * count;
                player.Gold += gold;
                player.RecalcStats(_db); SavePlayer(player);
                Send(ns, PhoneMsg.BoxOpen, "{\"ok\":true,\"boxTemplateId\":" + templateId + ",\"gold\":" + gold + ",\"count\":" + count + "}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
                return;
            }
            player.AddItem(rewardId, count);
            if (_db.GetCardBookletRows(rewardId).Count > 0)
            {
                CardBookletInfo rolled;
                lock (_lock) { rolled = _db.RollCardBooklet(rewardId, _rng); }
                if (rolled != null) player.SetCardBookletProfile(rewardId, rolled.Profile);
            }
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.BoxOpen, "{\"ok\":true,\"boxTemplateId\":" + templateId + ",\"rewardTemplateId\":" + rewardId +
                ",\"count\":" + count + ",\"strengthenLevel\":" + drop.StrengthenLevel + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleItemFusion(ServerPlayer player, NetworkStream ns, string json)
        {
            if (_db == null) { Send(ns, PhoneMsg.ItemFusion, "{\"ok\":false,\"err\":\"config\"}"); return; }
            int fusionId = JI(json, "fusionId", 0);
            ItemFusionRecipe recipe = _db.GetItemFusion(fusionId);
            if (recipe == null) { Send(ns, PhoneMsg.ItemFusion, "{\"ok\":false,\"err\":\"recipe\"}"); return; }
            int[] items = { recipe.Item1, recipe.Item2, recipe.Item3, recipe.Item4 };
            int[] counts = { recipe.Count1, recipe.Count2, recipe.Count3, recipe.Count4 };
            for (int i = 0; i < 4; i++)
            {
                if (items[i] > 0 && counts[i] > 0 && !HasEnoughBag(player, items[i], counts[i]))
                {
                    Send(ns, PhoneMsg.ItemFusion, "{\"ok\":false,\"err\":\"mat\",\"item\":" + items[i] + "}");
                    return;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (items[i] > 0 && counts[i] > 0) player.Consume(items[i], counts[i]);
            }
            int rate = recipe.FusionRate > 0 ? recipe.FusionRate : 10000;
            int roll;
            lock (_lock) { roll = _rng.Next(0, 10000); }
            bool success = roll < Mathf.Clamp(rate, 1, 10000);
            if (success && recipe.Reward > 0) player.AddItem(recipe.Reward, 1);
            player.RecalcStats(_db); SavePlayer(player);
            Send(ns, PhoneMsg.ItemFusion, "{\"ok\":true,\"success\":" + (success ? "true" : "false") +
                ",\"fusionId\":" + fusionId + ",\"reward\":" + (success ? recipe.Reward : 0) + ",\"rate\":" + rate + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        static bool HasEnoughBag(ServerPlayer player, int templateId, int count)
        {
            foreach (var s in player.Bag)
                if (s.TemplateId == templateId && s.Count >= count) return true;
            return false;
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
            int remapped = 0;
            if (success)
            {
                slot.Strengthen++;
                remapped = TryRemapStrengthenGoods(player, slot);
            }
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":true,\"success\":" + (success ? "true" : "false") + ",\"level\":" + slot.Strengthen +
                ",\"templateId\":" + slot.TemplateId + (remapped > 0 ? ",\"remapped\":" + remapped : "") + "}");
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

                    if (win && p.PveDreamland && _db != null)
                    {
                        StoryCopySection dreamSec = _db.GetStoryCopySection(p.PveDreamlandChapter, p.PveDreamlandSection);
                        if (dreamSec != null)
                        {
                            if (!string.IsNullOrEmpty(dreamSec.ThreeStarAward) &&
                                int.TryParse(dreamSec.ThreeStarAward, NumberStyles.Integer, CultureInfo.InvariantCulture, out int awardId))
                                p.GrantTemplateReward(_db, awardId, 1);
                            if (p.PveDreamlandSection > p.DreamlandClearedSection)
                                p.DreamlandClearedSection = p.PveDreamlandSection;
                            StoryCopySection next = _db.GetStoryCopySection(p.PveDreamlandChapter, p.PveDreamlandSection + 1);
                            if (next != null) p.DreamlandSection = next.Section;
                        }
                    }

                    if (win && p.PveWarriorFam && _db != null)
                    {
                        WarriorFamFightConfig famRow = _db.GetWarriorFamFight(p.PveWarriorFamHardType, p.PveWarriorFamLevel);
                        if (famRow != null)
                        {
                            if (p.PveWarriorFamLevel > p.WarriorFamClearedLevel)
                            {
                                _db.GrantRewardPairs(p, famRow.FirstRewards);
                                p.WarriorFamClearedLevel = p.PveWarriorFamLevel;
                            }
                            _db.GrantRewardPairs(p, famRow.Rewards);
                            int maxLevel = _db.ConfigInt("WarriorFamMaxLevel", 100);
                            if (p.PveWarriorFamLevel < maxLevel && _db.GetWarriorFamFight(p.PveWarriorFamHardType, p.PveWarriorFamLevel + 1) != null)
                                p.WarriorFamLevel = p.PveWarriorFamLevel + 1;
                        }
                    }

                    p.PveNpcId = 0;
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
                        ResolveGuildLevel(p);
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

        ServerPlayer FindPlayerByNick(string nick, ServerPlayer exclude)
        {
            foreach (ServerPlayer p in _players.Values)
            {
                if (p == exclude) continue;
                if (string.Equals(p.Nick, nick, StringComparison.OrdinalIgnoreCase))
                    return p;
            }
            return null;
        }

        string SaveFileForNick(string nick)
        {
            return Path.Combine(_savePath, SanitizeFileName(nick) + ".json");
        }

        /// <summary>
        /// Append mail to an offline player's save without inserting them into _players.
        /// Assigns Id from NextMailId and increments it. Caller should hold _lock.
        /// </summary>
        bool TryAppendMailToSaveFile(string nick, ServerMail mail)
        {
            string file = SaveFileForNick(nick);
            if (!File.Exists(file))
                return false;
            try
            {
                var loaded = JsonUtility.FromJson<ServerPlayerSave>(File.ReadAllText(file));
                if (loaded == null)
                    return false;
                ServerPlayer p = FromSave(loaded);
                if (p.Mails == null)
                    p.Mails = new List<ServerMail>();
                mail.Id = p.NextMailId++;
                p.Mails.Add(mail);
                File.WriteAllText(file, JsonUtility.ToJson(ToSave(p), true));
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool TryAddFriendToSaveFile(string nick, string friendNick)
        {
            if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(friendNick))
                return false;
            string file = SaveFileForNick(nick);
            if (!File.Exists(file))
                return false;
            try
            {
                var loaded = JsonUtility.FromJson<ServerPlayerSave>(File.ReadAllText(file));
                if (loaded == null)
                    return false;
                ServerPlayer p = FromSave(loaded);
                if (p.Friends == null)
                    p.Friends = new List<string>();
                foreach (string existing in p.Friends)
                {
                    if (string.Equals(existing, friendNick, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                p.Friends.Add(friendNick);
                File.WriteAllText(file, JsonUtility.ToJson(ToSave(p), true));
                return true;
            }
            catch
            {
                return false;
            }
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
        class RelicSlotSave { public int relicId; public int upgradeLevel; }
        class BankTermDepositSave { public int templateId; public int amount; public int depositDay; }
        class QuestProgressSave { public int questId; public List<int> progress = new List<int>(); }

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
            public int PetId, CardId, TitleId, TotemId, MountGrade, MountTalismanId, ManorGrade = 1, GoldEquipId, GloryTemplateId, SigilQuality = 1, SigilProType, SigilProValue, VipLevel, Honor, Texp;
            public int PreferredBallId, LastSignDay = -1, SignIndex, LabyrinthFloor = 1;
            public string ConsortiaName = "";
            public int GuildLevel;
            public int ConsortiaBossDay = -1, ConsortiaBossHits;
            public int ElfId, GemLevel, KingBlessDay = -1, FarmHarvests;
            public int FusionKeys, BankGold, MineDay = -1, MineDigs;
            public int WorldBossDay = -1, WorldBossHits;
            public int NecklaceLevel, HomeTempleLevel, HomeTemplePracticeLevel, HomeTempleAdvanceLevel;
            public List<BankTermDepositSave> BankDeposits = new List<BankTermDepositSave>();
            public List<int> SweepMissionClears = new List<int>();
            public int WardrobeClothId, HonorSystemExp, HonorSystemLevel;
            public int HonorSystemDay = -1, HonorSystemOps;
            public List<int> WardrobeProperties = new List<int>();
            public List<int> HonorSystemClaimed = new List<int>();
            public int RedPacketDay = -1, RedPacketClaims;
            public int DevilTurnDay = -1, DevilTurnSpins, DevilTurnPoints;
            public List<int> DevilTreasPointClaimed = new List<int>();
            public List<QuestProgressSave> QuestProgress = new List<QuestProgressSave>();
            public int SpaRoomDay = -1, SpaRoomDayScore;
            public int TreasureRoomDay = -1, TreasureRoomDraws;
            public int ChristmasDay = -1, ChristmasClaims;
            public int NewYearDay = -1, NewYearFreeUsed, NewYearPoints;
            public List<int> NewYearPointClaimed = new List<int>();
            public int WorshipMoonDay = -1, WorshipMoonDraws;
            public int SuperLuckerDay = -1, SuperLuckerDraws;
            public int JigsawDay = -1, JigsawClaims;
            public int BibleDay = -1, BibleClaims;
            public int SweepDay = -1, SweepCount;
            public bool FirstRechargeClaimed;
            public List<FirstRechargeBuySave> FirstRechargeShopBuys = new List<FirstRechargeBuySave>();
            public int DreamlandChapter = 1, DreamlandSection = 1, DreamlandClearedSection;
            public int DreamlandDay = -1, DreamlandAttempts;
            public int WarriorFamHardType, WarriorFamLevel = 1, WarriorFamClearedLevel;
            public int WarriorFamDay = -1, WarriorFamAttempts;
            public int ForcesBattleScore, ForcesBattleDay = -1, ForcesBattleAttempts;
            public int CultureGrade = 1, CultureAtk, CultureDef, CultureAgi, CultureLuck;
            public int JampsManualLevel = 1;
            public List<int> JampsDebrisOwned = new List<int>();
            public List<int> JampsPagesCollected = new List<int>();
            public List<int> JampsPagesActivated = new List<int>();
            public int CardMainLevel;
            public List<int> OwnedCardTemplateIds = new List<int>();
            public List<int> CardBookletProfiles = new List<int>();
            public List<int> CardBookletClaimed = new List<int>();
            public int CardSoul;
            public int ElfIntimacyExp;
            public int ElfIntimacyLevel;
            public int ElfIntimacyDay = -1;
            public int ElfIntimacyActions;
            public int CalendarMonth;
            public List<int> CalendarClaimedDays = new List<int>();
            public int AuditoriumDay = -1, AuditoriumActions;
            public int BoguAdventureDay = -1, BoguAdventureActions;
            public int QuizDay = -1, QuizAttempts;
            public int OneYuanDay = -1;
            public List<int> OneYuanBought = new List<int>();
            public int GodCardEquipId, EngraveSetId;
            public int GodCardPoints;
            public List<int> GodCardPointClaimed = new List<int>();
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
            public List<RelicSlotSave> Relics = new List<RelicSlotSave>();
            public List<ServerMailSave> Mails = new List<ServerMailSave>();
        }

        [Serializable]
        class FirstRechargeBuySave
        {
            public int templateId;
            public int count;
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
        class GodCardSlotSave { public int id; public int count = 1; public int grooveLevel; public int grooveExp; }

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
                MountGrade = p.MountGrade, MountTalismanId = p.MountTalismanId, ManorGrade = p.ManorGrade,
                GoldEquipId = p.GoldEquipId, GloryTemplateId = p.GloryTemplateId,
                SigilQuality = p.SigilQuality, SigilProType = p.SigilProType, SigilProValue = p.SigilProValue,
                VipLevel = p.VipLevel, Honor = p.Honor, Texp = p.Texp,
                PreferredBallId = p.PreferredBallId, LastSignDay = p.LastSignDay, SignIndex = p.SignIndex,
                LabyrinthFloor = p.LabyrinthFloor, ConsortiaName = p.ConsortiaName, GuildLevel = p.GuildLevel,
                ConsortiaBossDay = p.ConsortiaBossDay, ConsortiaBossHits = p.ConsortiaBossHits,
                ElfId = p.ElfId, GemLevel = p.GemLevel, KingBlessDay = p.KingBlessDay, FarmHarvests = p.FarmHarvests,
                FusionKeys = p.FusionKeys, BankGold = p.BankGold, MineDay = p.MineDay, MineDigs = p.MineDigs,
                WorldBossDay = p.WorldBossDay, WorldBossHits = p.WorldBossHits,
                NecklaceLevel = p.NecklaceLevel, HomeTempleLevel = p.HomeTempleLevel,
                HomeTemplePracticeLevel = p.HomeTemplePracticeLevel, HomeTempleAdvanceLevel = p.HomeTempleAdvanceLevel,
                BankDeposits = new List<BankTermDepositSave>(),
                SweepMissionClears = p.SweepMissionClears ?? new List<int>(),
                WardrobeClothId = p.WardrobeClothId, HonorSystemExp = p.HonorSystemExp,
                HonorSystemLevel = p.HonorSystemLevel, HonorSystemDay = p.HonorSystemDay,
                HonorSystemOps = p.HonorSystemOps,
                WardrobeProperties = p.WardrobeProperties ?? new List<int>(),
                HonorSystemClaimed = p.HonorSystemClaimed ?? new List<int>(),
                RedPacketDay = p.RedPacketDay, RedPacketClaims = p.RedPacketClaims,
                DevilTurnDay = p.DevilTurnDay, DevilTurnSpins = p.DevilTurnSpins, DevilTurnPoints = p.DevilTurnPoints,
                DevilTreasPointClaimed = p.DevilTreasPointClaimed ?? new List<int>(),
                QuestProgress = new List<QuestProgressSave>(),
                SpaRoomDay = p.SpaRoomDay, SpaRoomDayScore = p.SpaRoomDayScore,
                TreasureRoomDay = p.TreasureRoomDay, TreasureRoomDraws = p.TreasureRoomDraws,
                ChristmasDay = p.ChristmasDay, ChristmasClaims = p.ChristmasClaims,
                NewYearDay = p.NewYearDay, NewYearFreeUsed = p.NewYearFreeUsed, NewYearPoints = p.NewYearPoints,
                NewYearPointClaimed = p.NewYearPointClaimed ?? new List<int>(),
                WorshipMoonDay = p.WorshipMoonDay, WorshipMoonDraws = p.WorshipMoonDraws,
                SuperLuckerDay = p.SuperLuckerDay, SuperLuckerDraws = p.SuperLuckerDraws,
                JigsawDay = p.JigsawDay, JigsawClaims = p.JigsawClaims,
                BibleDay = p.BibleDay, BibleClaims = p.BibleClaims,
                SweepDay = p.SweepDay, SweepCount = p.SweepCount,
                FirstRechargeClaimed = p.FirstRechargeClaimed,
                DreamlandChapter = p.DreamlandChapter, DreamlandSection = p.DreamlandSection,
                DreamlandClearedSection = p.DreamlandClearedSection, DreamlandDay = p.DreamlandDay,
                DreamlandAttempts = p.DreamlandAttempts,
                WarriorFamHardType = p.WarriorFamHardType, WarriorFamLevel = p.WarriorFamLevel,
                WarriorFamClearedLevel = p.WarriorFamClearedLevel, WarriorFamDay = p.WarriorFamDay,
                WarriorFamAttempts = p.WarriorFamAttempts,
                ForcesBattleScore = p.ForcesBattleScore, ForcesBattleDay = p.ForcesBattleDay,
                ForcesBattleAttempts = p.ForcesBattleAttempts,
                CultureGrade = p.CultureGrade, CultureAtk = p.CultureAtk, CultureDef = p.CultureDef,
                CultureAgi = p.CultureAgi, CultureLuck = p.CultureLuck,
                JampsManualLevel = p.JampsManualLevel,
                JampsDebrisOwned = p.JampsDebrisOwned ?? new List<int>(),
                JampsPagesCollected = p.JampsPagesCollected ?? new List<int>(),
                JampsPagesActivated = p.JampsPagesActivated ?? new List<int>(),
                CardMainLevel = p.CardMainLevel,
                OwnedCardTemplateIds = p.OwnedCardTemplateIds ?? new List<int>(),
                CardBookletProfiles = p.CardBookletProfiles ?? new List<int>(),
                CardBookletClaimed = p.CardBookletClaimed ?? new List<int>(),
                CardSoul = p.CardSoul,
                ElfIntimacyExp = p.ElfIntimacyExp,
                ElfIntimacyLevel = p.ElfIntimacyLevel,
                ElfIntimacyDay = p.ElfIntimacyDay,
                ElfIntimacyActions = p.ElfIntimacyActions,
                CalendarMonth = p.CalendarMonth,
                CalendarClaimedDays = p.CalendarClaimedDays ?? new List<int>(),
                AuditoriumDay = p.AuditoriumDay, AuditoriumActions = p.AuditoriumActions,
                BoguAdventureDay = p.BoguAdventureDay, BoguAdventureActions = p.BoguAdventureActions,
                QuizDay = p.QuizDay, QuizAttempts = p.QuizAttempts,
                OneYuanDay = p.OneYuanDay,
                OneYuanBought = p.OneYuanBought ?? new List<int>(),
                GodCardEquipId = p.GodCardEquipId, EngraveSetId = p.EngraveSetId,
                GodCardPoints = p.GodCardPoints, GodCardPointClaimed = p.GodCardPointClaimed ?? new List<int>(),
                NextEmblemId = p.NextEmblemId, NextSoulStampId = p.NextSoulStampId,
                AcceptedQuests = p.AcceptedQuests, CompletedQuests = p.CompletedQuests,
                Friends = p.Friends, NextMailId = p.NextMailId
            };
            p.EnsureFightSpirits();
            p.EnsureBankDeposits();
            foreach (BankTermDeposit dep in p.BankDeposits)
                s.BankDeposits.Add(new BankTermDepositSave { templateId = dep.TemplateId, amount = dep.Amount, depositDay = dep.DepositDay });
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
            p.EnsureRelics(); foreach (RelicSlot r in p.Relics) s.Relics.Add(new RelicSlotSave { relicId = r.RelicId, upgradeLevel = r.UpgradeLevel });
            foreach (var b in p.Bag) s.Bag.Add(new BagSlotSave { t = b.TemplateId, c = b.Count, s = b.Strengthen });
            foreach (GodCardSlot g in p.GodCards) s.GodCards.Add(new GodCardSlotSave { id = g.Id, count = g.Count, grooveLevel = g.GrooveLevel, grooveExp = g.GrooveExp });
            foreach (StockSlot sh in p.StockHoldings) s.StockHoldings.Add(new StockSlotSave { stockId = sh.StockId, shares = sh.Shares, avgPrice = sh.AvgPrice });
            foreach (KeyValuePair<int, int> kv in p.FirstRechargeShopBuys)
            {
                s.FirstRechargeShopBuys.Add(new FirstRechargeBuySave { templateId = kv.Key, count = kv.Value });
            }
            if (p.QuestProgress != null)
            {
                foreach (KeyValuePair<int, List<int>> kv in p.QuestProgress)
                {
                    s.QuestProgress.Add(new QuestProgressSave { questId = kv.Key, progress = kv.Value ?? new List<int>() });
                }
            }
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
                MountGrade = s.MountGrade, MountTalismanId = s.MountTalismanId,
                ManorGrade = s.ManorGrade > 0 ? s.ManorGrade : 1,
                GoldEquipId = s.GoldEquipId, GloryTemplateId = s.GloryTemplateId,
                SigilQuality = s.SigilQuality > 0 ? s.SigilQuality : 1, SigilProType = s.SigilProType, SigilProValue = s.SigilProValue,
                VipLevel = s.VipLevel, Honor = s.Honor, Texp = s.Texp,
                PreferredBallId = s.PreferredBallId, LastSignDay = s.LastSignDay, SignIndex = s.SignIndex,
                LabyrinthFloor = s.LabyrinthFloor, ConsortiaName = s.ConsortiaName, GuildLevel = s.GuildLevel,
                ConsortiaBossDay = s.ConsortiaBossDay, ConsortiaBossHits = s.ConsortiaBossHits,
                ElfId = s.ElfId, GemLevel = s.GemLevel, KingBlessDay = s.KingBlessDay, FarmHarvests = s.FarmHarvests,
                FusionKeys = s.FusionKeys, BankGold = s.BankGold, MineDay = s.MineDay, MineDigs = s.MineDigs,
                WorldBossDay = s.WorldBossDay, WorldBossHits = s.WorldBossHits,
                NecklaceLevel = s.NecklaceLevel, HomeTempleLevel = s.HomeTempleLevel,
                HomeTemplePracticeLevel = s.HomeTemplePracticeLevel, HomeTempleAdvanceLevel = s.HomeTempleAdvanceLevel,
                SweepMissionClears = s.SweepMissionClears ?? new List<int>(),
                WardrobeClothId = s.WardrobeClothId, HonorSystemExp = s.HonorSystemExp,
                HonorSystemLevel = s.HonorSystemLevel, HonorSystemDay = s.HonorSystemDay,
                HonorSystemOps = s.HonorSystemOps,
                WardrobeProperties = s.WardrobeProperties ?? new List<int>(),
                HonorSystemClaimed = s.HonorSystemClaimed ?? new List<int>(),
                RedPacketDay = s.RedPacketDay, RedPacketClaims = s.RedPacketClaims,
                DevilTurnDay = s.DevilTurnDay, DevilTurnSpins = s.DevilTurnSpins, DevilTurnPoints = s.DevilTurnPoints,
                DevilTreasPointClaimed = s.DevilTreasPointClaimed ?? new List<int>(),
                SpaRoomDay = s.SpaRoomDay, SpaRoomDayScore = s.SpaRoomDayScore,
                TreasureRoomDay = s.TreasureRoomDay, TreasureRoomDraws = s.TreasureRoomDraws,
                ChristmasDay = s.ChristmasDay, ChristmasClaims = s.ChristmasClaims,
                NewYearDay = s.NewYearDay, NewYearFreeUsed = s.NewYearFreeUsed, NewYearPoints = s.NewYearPoints,
                NewYearPointClaimed = s.NewYearPointClaimed ?? new List<int>(),
                WorshipMoonDay = s.WorshipMoonDay, WorshipMoonDraws = s.WorshipMoonDraws,
                SuperLuckerDay = s.SuperLuckerDay, SuperLuckerDraws = s.SuperLuckerDraws,
                JigsawDay = s.JigsawDay, JigsawClaims = s.JigsawClaims,
                BibleDay = s.BibleDay, BibleClaims = s.BibleClaims,
                SweepDay = s.SweepDay, SweepCount = s.SweepCount,
                FirstRechargeClaimed = s.FirstRechargeClaimed,
                DreamlandChapter = s.DreamlandChapter > 0 ? s.DreamlandChapter : 1,
                DreamlandSection = s.DreamlandSection > 0 ? s.DreamlandSection : 1,
                DreamlandClearedSection = s.DreamlandClearedSection,
                DreamlandDay = s.DreamlandDay, DreamlandAttempts = s.DreamlandAttempts,
                WarriorFamHardType = s.WarriorFamHardType,
                WarriorFamLevel = s.WarriorFamLevel > 0 ? s.WarriorFamLevel : 1,
                WarriorFamClearedLevel = s.WarriorFamClearedLevel,
                WarriorFamDay = s.WarriorFamDay, WarriorFamAttempts = s.WarriorFamAttempts,
                ForcesBattleScore = s.ForcesBattleScore, ForcesBattleDay = s.ForcesBattleDay,
                ForcesBattleAttempts = s.ForcesBattleAttempts,
                CultureGrade = s.CultureGrade > 0 ? s.CultureGrade : 1,
                CultureAtk = s.CultureAtk, CultureDef = s.CultureDef,
                CultureAgi = s.CultureAgi, CultureLuck = s.CultureLuck,
                JampsManualLevel = s.JampsManualLevel > 0 ? s.JampsManualLevel : 1,
                JampsDebrisOwned = s.JampsDebrisOwned ?? new List<int>(),
                JampsPagesCollected = s.JampsPagesCollected ?? new List<int>(),
                JampsPagesActivated = s.JampsPagesActivated ?? new List<int>(),
                CardMainLevel = s.CardMainLevel,
                OwnedCardTemplateIds = s.OwnedCardTemplateIds ?? new List<int>(),
                CardBookletProfiles = s.CardBookletProfiles ?? new List<int>(),
                CardBookletClaimed = s.CardBookletClaimed ?? new List<int>(),
                CardSoul = s.CardSoul,
                ElfIntimacyExp = s.ElfIntimacyExp,
                ElfIntimacyLevel = s.ElfIntimacyLevel,
                ElfIntimacyDay = s.ElfIntimacyDay,
                ElfIntimacyActions = s.ElfIntimacyActions,
                CalendarMonth = s.CalendarMonth,
                CalendarClaimedDays = s.CalendarClaimedDays ?? new List<int>(),
                AuditoriumDay = s.AuditoriumDay, AuditoriumActions = s.AuditoriumActions,
                BoguAdventureDay = s.BoguAdventureDay, BoguAdventureActions = s.BoguAdventureActions,
                QuizDay = s.QuizDay, QuizAttempts = s.QuizAttempts,
                OneYuanDay = s.OneYuanDay,
                OneYuanBought = s.OneYuanBought ?? new List<int>(),
                GodCardEquipId = s.GodCardEquipId, EngraveSetId = s.EngraveSetId,
                GodCardPoints = s.GodCardPoints, GodCardPointClaimed = s.GodCardPointClaimed ?? new List<int>(),
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
            p.EnsureBankDeposits();
            p.EnsureSweepMissionClears();
            if (s.BankDeposits != null)
                foreach (BankTermDepositSave dep in s.BankDeposits)
                    p.BankDeposits.Add(new BankTermDeposit { TemplateId = dep.templateId, Amount = dep.amount, DepositDay = dep.depositDay });
            p.EnsureMagicStones();
            if (s.FirstRechargeShopBuys != null)
            {
                foreach (FirstRechargeBuySave buy in s.FirstRechargeShopBuys)
                {
                    if (buy.templateId > 0 && buy.count > 0)
                    {
                        p.FirstRechargeShopBuys[buy.templateId] = buy.count;
                    }
                }
            }
            if (s.Emblems != null) foreach (EmblemSlotSave e in s.Emblems) p.Emblems.Add(new EmblemSlot { Id = e.id, TemplateId = e.templateId, Types = e.types, Profile = e.profile, MainType = e.mainType, MainValue = e.mainValue, SubValue = e.subValue, SkillId = e.skillId, Equipped = e.equipped });
            if (s.SoulStamps != null) foreach (SoulStampSlotSave ss in s.SoulStamps) p.SoulStamps.Add(new SoulStampSlot { Id = ss.id, TempId = ss.tempId, Type = ss.type, Quality = ss.quality, Grade = ss.grade, ProType = ss.proType, ProValue = ss.proValue, SkillId = ss.skillId, Equipped = ss.equipped });
            if (s.Relics != null) foreach (RelicSlotSave r in s.Relics) p.Relics.Add(new RelicSlot { RelicId = r.relicId, UpgradeLevel = r.upgradeLevel });
            p.EnsureEmblems(); p.EnsureSoulStamps(); p.EnsureRelics();
            p.EnsureWardrobeProperties();
            p.EnsureHonorSystemClaimed();
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
            foreach (var b in s.Bag) p.Bag.Add(new BagSlot { TemplateId = b.t, Count = b.c, Strengthen = b.s });
            if (s.GodCards != null)
            {
                foreach (GodCardSlotSave g in s.GodCards) p.GodCards.Add(new GodCardSlot { Id = g.id, Count = g.count, GrooveLevel = g.grooveLevel, GrooveExp = g.grooveExp });
            }
            p.EnsureGodCardPointClaimed();

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
