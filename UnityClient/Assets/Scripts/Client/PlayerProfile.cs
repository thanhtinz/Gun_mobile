using System;
using System.Collections.Generic;
using System.IO;
using GunMobile.Net;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Client
{
    [Serializable]
    public sealed class BagItem
    {
        public int TemplateId;
        public int Count = 1;
        public int Strengthen;
    }

    [Serializable]
    public sealed class PlayerProfile
    {
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
        public int Hp = 1200;
        public int Win;
        public int Lose;
        public int MapId = 1056;
        public int WeaponId = 7001;
        public int EquipHead;
        public int EquipHair;
        public int EquipFace;
        public int EquipCloth;
        public int EquipGlass;
        public int EquipWeapon = 7001;
        public int LastSignDay = -1;
        public int SignIndex;
        public int PetId;
        public int CardId;
        public int TotemId;
        public int TitleId;
        public int MountGrade;
        public int MountTalismanId;
        public List<int> MountSkillIds = new List<int>();
        public int ManorGrade = 1;
        public int GoldEquipId;
        public int GloryTemplateId;
        public int SigilQuality = 1;
        public int SigilProType;
        public int SigilProValue;
        public int LinkPalId;
        public int AchievementPoints;
        public List<int> CompletedAchievements = new List<int>();
        public List<int> ClaimedAchievements = new List<int>();
        public int JadeEquipId;
        public int RuneTemplateId;
        public int HorseAmuletLevel = 1;
        public int HorseAmuletGrade = 1;
        public int HorseAmuletPhase = 1;
        public int VipLevel;
        public int Texp;
        public int LabyrinthFloor = 1;
        public int Honor;
        public string ConsortiaName = "";
        public int GuildLevel;
        public int ConsortiaBossHits;
        public int ElfId;
        public int GemLevel;
        public int KingBlessDay = -1;
        public int FarmHarvests;
        public int FusionKeys;
        public int BankGold;
        public int MineDigs;
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
        public int RedPacketClaims;
        public int DevilTurnSpins;
        public int DevilTurnPoints;
        public List<int> DevilTreasPointClaimed = new List<int>();
        public int SpaRoomDayScore;
        public int TreasureRoomDraws;
        public int ChristmasClaims;
        public int NewYearPoints;
        public int NewYearFreeUsed;
        public List<int> NewYearPointClaimed = new List<int>();
        public int WorshipMoonDraws;
        public int SuperLuckerDraws;
        public int JigsawClaims;
        public int BibleClaims;
        public int SweepCount;
        public bool FirstRechargeClaimed;
        public int DreamlandChapter = 1;
        public int DreamlandSection = 1;
        public int DreamlandClearedSection;
        public int DreamlandAttempts;
        public int WarriorFamHardType;
        public int WarriorFamLevel = 1;
        public int WarriorFamClearedLevel;
        public int WarriorFamAttempts;
        public int ForcesBattleScore;
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
        public int ElfIntimacyActions;
        public int CalendarMonth;
        public List<int> CalendarClaimedDays = new List<int>();
        public int AuditoriumActions;
        public int BoguAdventureActions;
        public int QuizAttempts;
        public List<int> OneYuanBought = new List<int>();
        public List<RelicSlot> Relics = new List<RelicSlot>();
        public int PreferredBallId;
        public int MailGoldWaiting;
        public int PendingReward;
        public int PendingLabyrinth;
        public List<BagItem> Bag = new List<BagItem>();
        public List<int> AcceptedQuests = new List<int>();
        public List<int> CompletedQuests = new List<int>();
        public Dictionary<int, List<int>> QuestProgress = new Dictionary<int, List<int>>();
        public List<string> Friends = new List<string>();
        public List<FightSpiritSlot> FightSpirits = new List<FightSpiritSlot>();
        public List<MagicStoneSlot> MagicStones = new List<MagicStoneSlot>();
        public List<EmblemSlot> Emblems = new List<EmblemSlot>();
        public List<SoulStampSlot> SoulStamps = new List<SoulStampSlot>();
        public List<string> ChatLog = new List<string>();
        public List<GodCardSlot> GodCards = new List<GodCardSlot>();
        public int GodCardEquipId;
        public int GodCardPoints;
        public List<int> GodCardPointClaimed = new List<int>();
        public int EngraveSetId;
        public List<StockSlot> StockHoldings = new List<StockSlot>();

        public void EnsureRelics() { if (Relics == null) Relics = new List<RelicSlot>(); if (Relics.Count == 0) Relics.Add(new RelicSlot { RelicId = 1, UpgradeLevel = 0 }); }
        public void EnsureOneYuanBought() { if (OneYuanBought == null) OneYuanBought = new List<int>(); }
        public void EnsureNewYearClaimed() { if (NewYearPointClaimed == null) NewYearPointClaimed = new List<int>(); }
        public void EnsureCalendarClaimed() { if (CalendarClaimedDays == null) CalendarClaimedDays = new List<int>(); }
        public void EnsureMountSkills() { if (MountSkillIds == null) MountSkillIds = new List<int>(); }
        public void EnsureAchievements()
        {
            if (CompletedAchievements == null) CompletedAchievements = new List<int>();
            if (ClaimedAchievements == null) ClaimedAchievements = new List<int>();
        }
        public void EnsureOneYuanBought() { if (OneYuanBought == null) OneYuanBought = new List<int>(); }
        public RelicSlot FindRelic(int relicId) { EnsureRelics(); for (int i = 0; i < Relics.Count; i++) if (Relics[i].RelicId == relicId) return Relics[i]; return null; }
        public int GetCultureStatLevel(int statType) { switch (statType) { case 116: return CultureAtk; case 117: return CultureDef; case 118: return CultureAgi; case 119: return CultureLuck; default: return 0; } }
        public void EnsureJampsLists() { if (JampsDebrisOwned == null) JampsDebrisOwned = new List<int>(); if (JampsPagesCollected == null) JampsPagesCollected = new List<int>(); if (JampsPagesActivated == null) JampsPagesActivated = new List<int>(); }
        public bool HasJampsDebris(int id) { EnsureJampsLists(); return JampsDebrisOwned.Contains(id); }
        public bool HasJampsPageCollected(int id) { EnsureJampsLists(); return JampsPagesCollected.Contains(id); }
        public bool HasJampsPageActivated(int id) { EnsureJampsLists(); return JampsPagesActivated.Contains(id); }
        public void EnsureOwnedCards()
        {
            if (OwnedCardTemplateIds == null) OwnedCardTemplateIds = new List<int>();
            if (CardBookletProfiles == null) CardBookletProfiles = new List<int>();
            if (CardBookletClaimed == null) CardBookletClaimed = new List<int>();
            while (CardBookletProfiles.Count < OwnedCardTemplateIds.Count) CardBookletProfiles.Add(0);
        }
        public void SyncElfIntimacyLevel(GameDatabase db) { ElfIntimacyLevel = db != null ? db.ElfIntimacyLevelFromExp(ElfIntimacyExp) : 0; }

        public static string PathOnDisk => Path.Combine(Application.persistentDataPath, "player.json");

        public static PlayerProfile Load()
        {
            try
            {
                if (File.Exists(PathOnDisk))
                {
                    var p = JsonUtility.FromJson<PlayerProfile>(File.ReadAllText(PathOnDisk));
                    if (p != null)
                    {
                        p.Bag = p.Bag ?? new List<BagItem>();
                        p.AcceptedQuests = p.AcceptedQuests ?? new List<int>();
                        p.CompletedQuests = p.CompletedQuests ?? new List<int>();
                        p.Friends = p.Friends ?? new List<string>();
                        p.FightSpirits = p.FightSpirits ?? new List<FightSpiritSlot>();
                        p.EnsureFightSpirits();
                        p.MagicStones = p.MagicStones ?? new List<MagicStoneSlot>();
                        p.EnsureMagicStones();
                        p.Emblems = p.Emblems ?? new List<EmblemSlot>();
                        p.EnsureEmblems();
                        p.SoulStamps = p.SoulStamps ?? new List<SoulStampSlot>();
                        p.EnsureSoulStamps();
                        p.WardrobeProperties = p.WardrobeProperties ?? new List<int>();
                        p.EnsureWardrobeProperties();
                        p.HonorSystemClaimed = p.HonorSystemClaimed ?? new List<int>();
                        p.CalendarClaimedDays = p.CalendarClaimedDays ?? new List<int>();
                        p.OneYuanBought = p.OneYuanBought ?? new List<int>();
                        p.ChatLog = p.ChatLog ?? new List<string>();
                        p.GodCards = p.GodCards ?? new List<GodCardSlot>();
                        p.GodCardPointClaimed = p.GodCardPointClaimed ?? new List<int>();
                        p.StockHoldings = p.StockHoldings ?? new List<StockSlot>();
                        p.ConsortiaName = p.ConsortiaName ?? "";
                        return p;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("PlayerProfile load: " + e.Message);
            }

            var fresh = new PlayerProfile();
            fresh.EnsureStarterBag();
            fresh.EnsureFightSpirits();
            fresh.EnsureMagicStones();
            fresh.EnsureEmblems();
            fresh.EnsureSoulStamps();
            fresh.EnsureWardrobeProperties();
            return fresh;
        }

        public void EnsureFightSpirits()
        {
            if (FightSpirits == null)
            {
                FightSpirits = new List<FightSpiritSlot>();
            }

            if (FightSpirits.Count == 0)
            {
                foreach (int spiritId in GameDatabase.DefaultFightSpiritIds)
                {
                    FightSpirits.Add(new FightSpiritSlot { SpiritId = spiritId, Level = 0 });
                }
            }
        }

        public void EnsureMagicStones()
        {
            if (MagicStones == null)
            {
                MagicStones = new List<MagicStoneSlot>();
            }

            if (MagicStones.Count == 0)
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
        public void EnsureBankDeposits() { if (BankDeposits == null) BankDeposits = new List<BankTermDeposit>(); }
        public void EnsureSweepMissionClears() { if (SweepMissionClears == null) SweepMissionClears = new List<int>(); }
        public void EnsureSoulStamps() { if (SoulStamps == null) SoulStamps = new List<SoulStampSlot>(); }

        public void EnsureWardrobeProperties()
        {
            if (WardrobeProperties == null) WardrobeProperties = new List<int>();
        }

        public bool HasWardrobeProperty(int propertyId)
        {
            EnsureWardrobeProperties();
            return WardrobeProperties.Contains(propertyId);
        }

        public GodCardSlot FindGodCardSlot(int id)
        {
            if (GodCards == null) return null;
            foreach (GodCardSlot slot in GodCards)
            {
                if (slot.Id == id) return slot;
            }
            return null;
        }

        public void Save()
        {
            File.WriteAllText(PathOnDisk, JsonUtility.ToJson(this, true));
        }

        public void EnsureStarterBag()
        {
            if (Bag == null)
            {
                Bag = new List<BagItem>();
            }

            if (Bag.Count == 0)
            {
                AddItem(7001, 1);
                AddItem(1102, 1);
                AddItem(1103, 1);
                AddItem(5102, 1);
            }

            if (EquipWeapon == 0)
            {
                EquipWeapon = 7001;
                WeaponId = 7001;
            }

            if (EquipHead == 0 && Find(1102) != null)
            {
                EquipHead = 1102;
            }

            if (EquipCloth == 0 && Find(5102) != null)
            {
                EquipCloth = 5102;
            }

            if (Friends == null)
            {
                Friends = new List<string>();
            }

            if (Friends.Count == 0)
            {
                Friends.Add("小鸡助手");
                Friends.Add("训练教官");
            }

            EnsureFightSpirits();

            if (ChatLog == null)
            {
                ChatLog = new List<string>();
            }
        }

        public BagItem Find(int templateId)
        {
            for (int i = 0; i < Bag.Count; i++)
            {
                if (Bag[i].TemplateId == templateId)
                {
                    return Bag[i];
                }
            }

            return null;
        }

        public void AddItem(int templateId, int count = 1)
        {
            BagItem existing = Find(templateId);
            if (existing != null)
            {
                existing.Count += count;
                return;
            }

            Bag.Add(new BagItem { TemplateId = templateId, Count = count });
        }

        public bool Consume(int templateId, int count = 1)
        {
            BagItem existing = Find(templateId);
            if (existing == null || existing.Count < count)
            {
                return false;
            }

            existing.Count -= count;
            if (existing.Count <= 0)
            {
                Bag.Remove(existing);
            }

            return true;
        }

        public bool Equip(ItemTemplate item)
        {
            if (item == null || !item.CanEquip)
            {
                return false;
            }

            if (Find(item.TemplateId) == null)
            {
                return false;
            }

            switch (item.CategoryId)
            {
                case 1:
                    EquipHead = item.TemplateId;
                    break;
                case 2:
                    EquipGlass = item.TemplateId;
                    break;
                case 3:
                    EquipHair = item.TemplateId;
                    break;
                case 4:
                    EquipFace = item.TemplateId;
                    break;
                case 5:
                    EquipCloth = item.TemplateId;
                    break;
                case 7:
                case 13:
                case 27:
                    EquipWeapon = item.TemplateId;
                    WeaponId = item.TemplateId;
                    break;
                default:
                    if (item.CategoryId >= 13 && item.CategoryId <= 18)
                    {
                        EquipWeapon = item.TemplateId;
                        WeaponId = item.TemplateId;
                    }
                    else
                    {
                        return false;
                    }

                    break;
            }

            RecalcStats(null);
            return true;
        }

        public void RecalcStats(GameDatabase db)
        {
            int atk = 50;
            int def = 40;
            int agi = 40;
            int luk = 30;
            int hp = 1000 + Level * 10;
            if (db != null && db.Levels.Count > 0)
            {
                hp = db.BloodForLevel(Level);
            }

            if (db != null)
            {
                AddStats(db.GetItem(EquipHead), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipHair), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipFace), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipCloth), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipGlass), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipWeapon), ref atk, ref def, ref agi, ref luk);
                if (db.Pets.TryGetValue(PetId, out PetInfo pet))
                {
                    atk += pet.Attack;
                    def += pet.Defence;
                    agi += pet.Agility;
                    luk += pet.Luck;
                    hp += pet.Blood;
                }

                CardInfo card = db.GetCard(CardId);
                if (card != null)
                {
                    atk += card.AddAttack;
                    def += card.AddDefend;
                    agi += card.AddAgility;
                    luk += card.AddLucky;
                }

                if (db.Titles.TryGetValue(TitleId, out TitleInfo title))
                {
                    atk += title.Att;
                    def += title.Def;
                    agi += title.Agi;
                    luk += title.Luck;
                }

                if (db.Totems.TryGetValue(TotemId, out TotemInfo totem))
                {
                    atk += totem.AddAttack;
                    def += totem.AddDefence;
                    agi += totem.AddAgility;
                    luk += totem.AddLuck;
                    hp += totem.AddBlood;
                }

                if (db.Mounts.TryGetValue(MountGrade, out MountGrade mount))
                {
                    hp += mount.AddBlood;
                    atk += mount.AddDamage;
                    atk += mount.MagicAttack / 4;
                }

                db.ApplyMountTalismanBonus(MountTalismanId, ref hp);
                EnsureMountSkills();
                int mountSkillDmg = 0;
                db.ApplyMountSkillBonuses(MountSkillIds, ref atk, ref def, ref agi, ref luk, ref hp, ref mountSkillDmg);
                atk += mountSkillDmg;
                int linkDmg = 0;
                db.ApplyLinkPalBonus(LinkPalId, ref atk, ref def, ref agi, ref luk, ref hp, ref linkDmg);
                atk += linkDmg;
                db.ApplyGoldEquipBonus(EquipWeapon, ref atk, ref def, ref agi, ref luk, ref hp);
                db.ApplyGloryBonus(GloryTemplateId, ref atk, ref def, ref agi, ref luk, ref hp);
                int jadeMa = 0, jadeMd = 0;
                db.ApplyJadeBonus(JadeEquipId, ref atk, ref def, ref agi, ref luk, ref hp, ref jadeMa, ref jadeMd);
                int rDmg = 0, rGuard = 0;
                db.ApplyRuneBonus(RuneTemplateId, ref atk, ref def, ref agi, ref luk, ref hp, ref rDmg, ref rGuard);
                atk += rDmg; def += rGuard;
                int aDmg = 0, aGuard = 0;
                db.ApplyHorseAmuletBonus(HorseAmuletLevel, HorseAmuletGrade, HorseAmuletPhase, ref atk, ref def, ref agi, ref luk, ref hp, ref aDmg, ref aGuard);
                atk += aDmg; def += aGuard;

                if (db.Spirits.TryGetValue(Mathf.Max(1, GemLevel), out SpiritInfo spirit))
                {
                    atk += spirit.AttackAdd;
                    def += spirit.DefendAdd;
                    agi += spirit.AgilityAdd;
                    luk += spirit.LuckAdd;
                }

                EnsureFightSpirits();
                db.ApplyFightSpiritStats(FightSpirits, ref atk, ref def, ref agi, ref luk, ref hp);

                EnsureMagicStones();
                int magicAtk = 0;
                int magicDef = 0;
                db.ApplyMagicStoneStats(MagicStones, ref atk, ref def, ref agi, ref luk, ref magicAtk, ref magicDef);
                magicAtk += jadeMa; magicDef += jadeMd;
                int sDmg = 0, sGuard = 0;
                db.ApplySigilBonus(SigilProType, SigilProValue, ref atk, ref def, ref agi, ref luk, ref hp, ref sDmg, ref sGuard, ref magicAtk, ref magicDef);
                atk += sDmg;
                def += sGuard;
                atk += magicAtk / 4;
                def += magicDef / 4;
                db.ApplyNecklaceBonus(NecklaceLevel, ref hp, ref def);
                db.ApplyHomeTempleBonus(HomeTempleLevel, ref atk, ref hp);
                int htMagicDef = magicDef;
                db.ApplyHomeTemplePracticeBonus(HomeTemplePracticeLevel, ref atk, ref def, ref agi, ref luk, ref hp, ref htMagicDef);
                db.ApplyHomeTempleAdvanceBonus(HomeTempleAdvanceLevel, ref hp, ref htMagicDef, ref def);
                magicDef = htMagicDef;
                EnsureEmblems();
                int eMa = magicAtk, eMd = magicDef;
                db.ApplyEmblemStats(Emblems, ref atk, ref def, ref agi, ref luk, ref hp, ref eMa, ref eMd);
                atk += (eMa - magicAtk) / 4;
                def += (eMd - magicDef) / 4;
                EnsureSoulStamps();
                db.ApplySoulStampStats(SoulStamps, ref atk, ref def, ref agi, ref luk, ref hp);

                EnsureWardrobeProperties();
                int wDmg = 0; int wGuard = 0;
                db.ApplyWardrobeBonus(WardrobeProperties, ref atk, ref def, ref agi, ref luk, ref hp, ref wDmg, ref wGuard);
                atk += wDmg / 4; def += wGuard / 4;
                HonorSystemLevel = db.HonorSystemLevelFromExp(HonorSystemExp);
                db.ApplyHonorSystemBonus(HonorSystemLevel, ref atk, ref def, ref agi, ref luk, ref hp);
                EnsureRelics();
                int rDmg = 0;
                db.ApplyRelicStats(Relics, ref atk, ref def, ref agi, ref luk, ref hp, ref rDmg, ref magicAtk, ref magicDef);
                atk += rDmg / 4;
                db.ApplyCultureBonus(CultureGrade, CultureAtk, CultureDef, CultureAgi, CultureLuck, ref atk, ref def, ref agi, ref luk, ref hp, ref magicAtk, ref magicDef);
                EnsureJampsLists();
                int jDmg = 0; int jGuard = 0;
                db.ApplyJampsBonus(JampsManualLevel, JampsPagesCollected, JampsPagesActivated, ref atk, ref def, ref agi, ref luk, ref hp, ref jDmg, ref jGuard, ref magicAtk, ref magicDef);
                atk += jDmg / 4; def += jGuard / 4;
                db.ApplyCardMainBonus(CardMainLevel, ref atk, ref def, ref agi, ref luk);
                EnsureOwnedCards();
                int cDmg = 0; int cGuard = 0;
                db.ApplyCardSuitBonus(OwnedCardTemplateIds, ref atk, ref def, ref agi, ref luk, ref hp, ref cDmg, ref cGuard);
                EnsureOwnedCards();
                db.ApplyCardBookletBonus(OwnedCardTemplateIds, CardBookletProfiles, ref atk, ref def, ref agi, ref luk, ref hp, ref cDmg, ref cGuard);
                atk += cDmg / 4; def += cGuard / 4;
                SyncElfIntimacyLevel(db);
                db.ApplyElfIntimacyBonus(ElfIntimacyLevel, ref atk, ref def, ref hp);
                atk += magicAtk / 4;
                def += magicDef / 4;

                if (db.Elves.TryGetValue(ElfId, out ElfInfo elf))
                {
                    atk += elf.AttackHint / 3;
                    hp += elf.HpHint / 2;
                }

                int engrDmg = 0;
                int engrGuard = 0;
                if (GodCardEquipId > 0 && db.GodCards.TryGetValue(GodCardEquipId, out GodCardInfo gc))
                {
                    db.ApplyGodCardBonus(gc, ref atk, ref def, ref agi, ref luk, ref hp);
                    GodCardSlot grooveSlot = FindGodCardSlot(GodCardEquipId);
                    if (grooveSlot != null)
                        db.ApplyGodCardGrooveBonus(db.GodCardGrooveType(gc), grooveSlot.GrooveLevel,
                            ref atk, ref def, ref agi, ref luk, ref hp, ref engrDmg, ref engrGuard);
                }

                db.ApplyEngraveSetBonus(EngraveSetId, ref atk, ref def, ref agi, ref luk, ref hp, ref engrDmg, ref engrGuard);
                atk += engrDmg;
                def += engrGuard;

            }

            BagItem weapon = Find(EquipWeapon);
            int str = weapon != null ? weapon.Strengthen : 0;
            atk += str * 8;
            def += str * 6;
            atk += Texp / 10;
            def += Texp / 12;
            atk += VipLevel * 2;
            hp += VipLevel * 20;
            Attack = atk;
            Defence = def;
            Agility = agi;
            Luck = luk;
            Hp = hp;
        }

        static void AddStats(ItemTemplate t, ref int atk, ref int def, ref int agi, ref int luk)
        {
            if (t == null)
            {
                return;
            }

            atk += t.Attack;
            def += t.Defence;
            agi += t.Agility;
            luk += t.Luck;
        }

        public bool QuestDone(int id) => CompletedQuests.Contains(id);

        public bool QuestAccepted(int id) => AcceptedQuests.Contains(id);

        public List<int> GetQuestProgress(int questId)
        {
            if (QuestProgress != null && QuestProgress.TryGetValue(questId, out List<int> prog) && prog != null)
            {
                return prog;
            }

            return null;
        }


        public void GrantTemplate(int templateId, int count)
        {
            if (templateId == 0)
            {
                return;
            }

            if (templateId < 0)
            {
                int gold = count > 100000 ? count / 1000 : count;
                Gold += Mathf.Clamp(gold, 400, 30000);
                return;
            }

            AddItem(templateId, Mathf.Clamp(count, 1, 99));
        }

        public int CompleteAcceptedQuests(GameDatabase db)
        {
            int extra = 0;
            if (db == null || AcceptedQuests.Count == 0)
            {
                return 0;
            }

            var copy = new List<int>(AcceptedQuests);
            AcceptedQuests.Clear();
            foreach (int id in copy)
            {
                if (CompletedQuests.Contains(id))
                {
                    continue;
                }

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

                if (q == null)
                {
                    continue;
                }

                extra += q.RewardGold;
                Gold += q.RewardGold;
                Honor += q.RewardOffer;
                AddGp(db, q.RewardGp);
            }

            return extra;
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
    }

    public sealed class ModuleDef
    {
        public string Id;
        public string Title;
        public string TablePath;
        public bool OpensBattle;
        public string MornUiFile;

        public ModuleDef(string id, string title, string tablePath = null, bool opensBattle = false, string mornUiFile = null)
        {
            Id = id;
            Title = title;
            TablePath = tablePath;
            OpensBattle = opensBattle;
            MornUiFile = mornUiFile;
        }
    }

    /// <summary>
    /// Every major PC hall/system from Flash/config.xml + Request tables.
    /// </summary>
    public static class ModuleCatalog
    {
        public static readonly ModuleDef[] All =
        {
            new ModuleDef("room", "房间 / 开战", null, true),
            new ModuleDef("dungeon", "副本", "Request/LoadPVEItems.xml"),
            new ModuleDef("character", "角色"),
            new ModuleDef("shop", "商城", "Request/ShopItemList.xml"),
            new ModuleDef("bag", "背包 / 图鉴", "Request/TemplateAlllist.xml"),
            new ModuleDef("quest", "任务", "Request/QuestList.xml"),
            new ModuleDef("npc", "NPC 狩猎", "Request/NPCInfoList.xml"),
            new ModuleDef("ball", "炮弹", "Request/BallList.xml"),
            new ModuleDef("bomb", "炸弹配置", "Request/bombconfig.xml"),
            new ModuleDef("pet", "宠物", "Request/petskillinfo.xml"),
            new ModuleDef("card", "卡片", "Request/cardtemplateinfo.xml"),
            new ModuleDef("jamps", "探险手册", "Request/jampsmanualitemlist.xml"),
            new ModuleDef("title", "称号", "Request/newtitleinfo.xml"),
            new ModuleDef("totem", "图腾", "Request/toteminfo.xml"),
            new ModuleDef("horse", "坐骑", "Request/mounttemplateOUT.xml"),
            new ModuleDef("achievement", "成就", "Request/achievementlist.xml"),
            new ModuleDef("linkpal", "灵宝", "Request/TS_LinkPalTemplate.xml"),
            new ModuleDef("elf", "精灵", "Request/TS_ElfIntimacy.xml"),
            new ModuleDef("farm", "农场", "Request/foodcomposelist.xml"),
            new ModuleDef("church", "教堂", "Request/TS_EveryDaySignIn.xml"),
            new ModuleDef("consortia", "公会", "Request/CelebByConsortiaRiches.xml"),
            new ModuleDef("rank", "排行", "Request/CelebByDayGPList.xml"),
            new ModuleDef("auction", "拍卖", "Request/ts_swornitem.xml"),
            new ModuleDef("vip", "VIP", "Request/VipStoreList.xml"),
            new ModuleDef("signin", "签到", "Request/TS_EveryDaySignIn.xml"),
            new ModuleDef("lottery", "抽奖", "Request/newlotteryitem.xml"),
            new ModuleDef("labyrinth", "迷宫", "Request/fightlabdropitemlist.xml"),
            new ModuleDef("worldboss", "世界BOSS", "Request/campwaritems.xml"),
            new ModuleDef("setting", "设置", "Flash/config.xml"),
            new ModuleDef("friend", "好友", null),
            new ModuleDef("mail", "邮件", null),
            new ModuleDef("im", "聊天", "Flash/ui/cn_trad/xml/xml/ddtim.xml"),
            new ModuleDef("store", "铁匠铺", "Flash/ui/cn_trad/xml/xml/forgemain.xml"),
            new ModuleDef("texp", "修炼", "Flash/ui/cn_trad/morn/ui/ddttexpsystem.ui"),
            new ModuleDef("gemstone", "战魂", "Request/SpiritInfoList.xml"),
            new ModuleDef("kingbless", "弹王盟约", "Flash/ui/cn_trad/xml/xml/firstRecharge.xml"),
            new ModuleDef("calendar", "日历", "Flash/ui/cn_trad/xml/xml/ddtcalendar.xml"),
            new ModuleDef("quiz", "答题", "Request/loadallquestions.xml"),
            new ModuleDef("oneyuan", "一元购", "Request/oneyuanbuyallgoodstemplate.xml"),
            new ModuleDef("godcard", "神卡", "Request/godcardlist.xml"),
            new ModuleDef("engrave", "刻印", "Request/engravesetinfo.xml"),
            new ModuleDef("stock", "股票", "Request/StockTemplateInfo.xml"),
            new ModuleDef("magicstone", "魔石", "Request/magicstonetemplate.xml", false, "magicStone.ui"),
            new ModuleDef("enchant", "附魔", "Request/magicfusiondata.xml", false, "enchant.ui"),
            new ModuleDef("teamdungeon", "团队副本", "Request/battleteamshopitemlist.xml", false, "teamdungeon.ui"),
            new ModuleDef("carnival", "嘉年华", "Request/newlotteryitem.xml", false, "carnival.ui"),
            new ModuleDef("bank", "银行", null, false, "bank.ui"),
            new ModuleDef("mines", "矿山", null, false, "mines.ui"),
            new ModuleDef("auditorium", "礼堂", "Request/CelebByDayGPList.xml", false, "auditorium.ui"),
            new ModuleDef("treasure", "寻宝", "Request/newlotteryitem.xml", false, "treasureHunting.ui"),
            new ModuleDef("peakbattle", "巅峰战", "Request/areacelebbydayfightpowerlist.xml", false, "peakBattle.ui"),
            new ModuleDef("necklace", "项链", "Request/TS_NecklaceCasting.xml", false, "necklace.ui"),
            new ModuleDef("christmas", "圣诞", "Request/activityhalloweenitems.xml", false, "christmas.ui"),
            new ModuleDef("newyear", "新年", "Request/TS_NewYearPointReward.xml", false, "newyear.ui"),
            new ModuleDef("redpacket", "红包", null, false, "redpacket.ui"),
            new ModuleDef("devilturn", "恶魔转盘", "Request/DevilTreasItemList.xml", false, "devilturn.ui"),
            new ModuleDef("jigsaw", "拼图", null, false, "jigsaw.ui"),
            new ModuleDef("bible", "圣经", null, false, "bible.ui"),
            new ModuleDef("honorhall", "荣誉", "Request/ts_honorsystem_template.xml", false, "honor.ui"),
            new ModuleDef("glory", "光辉", "Request/GloryItemUpgradeList.xml"),
            new ModuleDef("jade", "玉石", "Request/TS_JadeTemp.xml"),
            new ModuleDef("rune", "符文", "Request/runetemplatelist.xml"),
            new ModuleDef("horseamulet", "坐骑护符", "Request/amuletinfoitemlist.xml"),
            new ModuleDef("firstrecharge", "首充", "Request/ts_firstpayshoptemp.xml", false, "firstrecharge.ui"),
            new ModuleDef("dreamland", "梦境", "Request/TS_StoryCopySectionTemplate.xml", false, "dreamlandChallenge.ui"),
            new ModuleDef("darkboundary", "暗界", "Request/ts_warriorfamfightconfig.xml", false, "darkboundary.ui"),
            new ModuleDef("boguadventure", "啵咕冒险", null, false, "boguadventure.ui"),
            new ModuleDef("worshipthemoon", "拜月", "Request/ServerConfig.xml", false, "worshipthemoon.ui"),
            new ModuleDef("forcesbattle", "势力战", "Request/cityoccupationsystems.xml", false, "forcesbattle.ui"),
            new ModuleDef("soulmark", "魂印", "Request/TS_SoulStampTemplate.xml", false, "soulMark.ui"),
            new ModuleDef("sigil", "符印", "Request/TS_SigilProValueLimitTemp.xml"),
            new ModuleDef("magicwardrobe", "魔衣橱", "Request/magicclothlist.xml", false, "magicwardrobe.ui"),
            new ModuleDef("sweep", "扫荡", null, false, "sweep.ui"),
            new ModuleDef("culture", "文化淬炼", "Request/TS_UpgradeTemplate.xml", false, "culture.ui"),
            new ModuleDef("emblem", "徽章", "Request/TS_Emblem.xml", false, "emblem.ui"),
            new ModuleDef("treasureroom", "藏宝室", "Request/CarnivalActivityItems.xml", false, "treasureroom.ui"),
            new ModuleDef("labyrinthgame", "温泉炸弹房", "Request/sparoomfixedbomb.xml", false, "labyrinthgame.ui"),
            new ModuleDef("godcardraise", "神卡养成", "Request/godcardlist.xml", false, "godcardraise.ui"),
            new ModuleDef("homeTemple", "家园神殿", null, false, "homeTemple.ui"),
            new ModuleDef("carnivalSuperLucker", "超级幸运", "Request/CarnivalActivityItems.xml", false, "carnivalSuperLucker.ui"),
        };
    }
}
