using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using GunMobile.Core;
using GunMobile.Logic;
using GunMobile.Net;
using UnityEngine;

namespace GunMobile.Res
{
    public sealed class ItemTemplate
    {
        public int TemplateId;
        public string Name = "";
        public string Description = "";
        public int CategoryId;
        public int Attack;
        public int Defence;
        public int Agility;
        public int Luck;
        public int NeedLevel;
        public int NeedSex;
        public bool CanEquip;
        public bool CanUse;
        public string Pic = "";
        public int Quality;
        public int Level;
        public int Property1;
        public int Property2;
        public int Property3;
        public int Property4;
        public int Property5;
        public int Property6;
        public int Property7;
        public int Property8;
        public int ReclaimValue;
        public int FloorPrice;
    }

    public sealed class LevelGrade
    {
        public int Grade;
        public int Gp;
        public int Blood;
    }

    public sealed class FightPropTemplate
    {
        public int Pic;
        public int Property1;
        public int Property2;
        public int Property3;
        public int Property4;
        public int Property5;
        public int Property6;
        public int Property7;
        public int Property8;
    }

    public sealed class ShopOffer
    {
        public int Id;
        public int ShopId;
        public int TemplateId;
        public int AUnit;
        public int APrice1;
        public int AValue1;
        public bool CanBuy;
        public int LimitGrade;
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
    }

    public sealed class MapInfo
    {
        public int Id;
        public string Name = "";
        public string Description = "";
        public int ForegroundWidth;
        public int ForegroundHeight;
        public int Type;
        public bool HasCollision;
        public bool HasArt;
    }

    public sealed class NpcInfo
    {
        public int Id;
        public string Name = "";
        public int Level;
        public int Blood;
        public int Attack;
        public int Defence;
        public int Agility;
        public int Lucky;
        public int BaseDamage;
        public int BaseGuard;
        public string ModelId = "";
        public string ResourcesPath = "";
        public int Experience;
        public int DropId;
    }

    public sealed class FightLabDrop
    {
        public int LabId;
        public int Easy;
        public int AwardItem;
        public int Count;
    }

    /// <summary>One row from Request/CelebByDay*.xml (PC leaderboard snapshot).</summary>
    public sealed class CelebEntry
    {
        public int Rank;
        public string Nick = "";
        public int Grade;
        public int Gp;
        public int FightPower;
        public int Offer;
        public int VipLevel;
        public string ConsortiaName = "";
        public int WinCount;
        public int TotalCount;
    }

    public sealed class PetInfo
    {
        public int TemplateId;
        public string Name = "";
        public string Pic = "";
        public int KindId;
        public int StarLevel;
        public int Mp = 100;
        public int Attack;
        public int Defence;
        public int Blood;
        public int Agility;
        public int Luck;
    }

    public sealed class PetSkillInfo
    {
        public int Id;
        public string Name = "";
        public int Pic;
        public int BallType;
        public int Probability;
        public int[] ElementIds = System.Array.Empty<int>();
        public string Description = "";
        public int DamagePercent;
        public int NewBallId;
        public int CostMp;
        public int ColdDown;
    }

    public sealed class PetSkillElementInfo
    {
        public int Id;
        public string Name = "";
        public string Description = "";
        public string EffectPic = "";
    }

    public sealed class CardInfo
    {
        public int Id;
        public int CardId;
        public int AddAttack;
        public int AddDefend;
        public int AddAgility;
        public int AddLucky;
        public int AddDamage;
        public int AddGuard;
    }

    public sealed class TitleInfo
    {
        public int Id;
        public string Name = "";
        public string Pic = "";
        public int Att;
        public int Def;
        public int Agi;
        public int Luck;
    }

    public sealed class TotemInfo
    {
        public int Id;
        public int AddAttack;
        public int AddDefence;
        public int AddAgility;
        public int AddLuck;
        public int AddBlood;
        public int AddDamage;
        public int AddGuard;
        public int ConsumeHonor;
    }

    public sealed class MountGrade
    {
        public int Grade;
        public int Experience;
        public int AddBlood;
        public int AddDamage;
        public int AddGuard;
        public int MagicAttack;
    }

    public sealed class LotteryDrop
    {
        public int Id;
        public int TemplateId;
        public int Count;
        public int Type = 1;
    }

    public sealed class CampWarReward
    {
        public int Id;
        public int MinRank;
        public int MaxRank;
        public int ItemId;
        public int Count = 1;
    }

    public sealed class NecklaceCastingLevel
    {
        public int Level;
        public int NeedItemCount1;
        public int NeedItemCount2;
        public int Hp;
        public int Toughness;
        public int AvoidInjury;
        public int TricRevolt;
        public int Guardian;
    }

    public sealed class DevilTreasItem
    {
        public int Id;
        public int Type;
        public int TemplateId;
        public int Value;
        public int Weight;
    }

    public sealed class ActivityConfigEntry
    {
        public int Num;
        public string Name = "";
        public string Params1 = "";
        public string Params2 = "";
        public string Params3 = "";
        public string Params4 = "";
        public string Params5 = "";
        public string RankAreaAward = "";
    }

    public sealed class FirstPayShopItem
    {
        public int Id;
        public int TemplateId;
        public int ItemTempId;
        public int ItemTempCount = 1;
        public int LimitBuyCount = 1;
        public int NeedGoldBeans;
        public int ShopType;
    }

    public sealed class FirstRechargeConfig
    {
        public int[] RewardItemIds = Array.Empty<int>();
        public int[] RewardCounts = Array.Empty<int>();
        public int ExtraItemId1;
        public int ExtraItemId2;
        public int RankAwardId;
    }

    public sealed class MagicClothInfo
    {
        public int Id;
        public string Name = "";
        public int HasShow;
        public int Type;
        public int HeadId;
        public int HairId;
        public int EffId;
        public int ClothId;
        public int GlassId;
        public int FaceId;
        public int WingId;
        public int SuitsId;
        public int Sex;
    }

    public sealed class ClothGroupPart
    {
        public int GroupId;
        public int TemplateId;
        public int Sex;
        public int Description;
        public int Cost;
        public int Type;
        public int OtherTemplateId;
    }

    public sealed class ClothPropertyInfo
    {
        public int Id;
        public int Sex;
        public string Name = "";
        public int Attack;
        public int Defend;
        public int Agility;
        public int Luck;
        public int Blood;
        public int Damage;
        public int Guard;
        public int Cost;
        public int Type;
    }

    public sealed class HonorSystemLevelInfo
    {
        public int Level;
        public string Name = "";
        public int Exp;
        public int Blood;
        public int StrengthRate;
        public int AdvanceRate;
        public int GoldRate;
        public int SpiritRate;
        public int FusionRate;
        public int LevelGift;
    }

    public sealed class TotemHonorEntry
    {
        public int Id;
        public int Type;
        public int NeedMoney;
        public int AddHonor;
    }

    public sealed class PveMission
    {
        public int Id;
        public string Name = "";
        public string Description = "";
        public int LevelLimits;
        public int MinLv;
        public int Type;
    }

    public sealed class SpiritInfo
    {
        public int Level;
        public int AttackAdd;
        public int DefendAdd;
        public int AgilityAdd;
        public int LuckAdd;
        public int ReferenceCost;
        public int CategoryId;
        public int BagPlace;
    }

    public sealed class FightSpiritTemplate
    {
        public int SpiritId;
        public int Level;
        public string Icon = "";
        public int Exp;
        public int Attack;
        public int Defence;
        public int Agility;
        public int Lucky;
        public int Blood;
    }

    public sealed class MagicStoneTemplate
    {
        public int TemplateId;
        public int Level;
        public int Exp;
        public int Attack;
        public int Defence;
        public int Agility;
        public int Luck;
        public int MagicAttack;
        public int MagicDefence;
    }

    public sealed class EmblemTemplate
    {
        public int Id;
        public int TemplateId;
        public int Types;
        public int Profile;
        public int MainType;
        public int SubCount;
        public string MainValue = "";
        public string SubValue = "";
        public int NeedItem1;
        public int ItemCount1;
        public int NeedItem2;
        public int ItemCount2;
        public int NeedItem3;
        public int ItemCount3;
        public int NeedItem4;
        public int ItemCount4;
    }

    public sealed class SoulStampTemplate
    {
        public int TempId;
        public int Type;
        public int Quality;
        public int[] ProTypes = System.Array.Empty<int>();
        public int SkillId;
        public int SubSkillId;
        public string UpSkillId = "";
        public string UpSubSkillId = "";
    }

    public sealed class SoulStampComposeTemplate
    {
        public int TemplateId;
        public int Quality;
        public int ComposeCost;
        public int ComposePreCost;
    }

    public sealed class SoulStampProBand
    {
        public int Min;
        public int Max;
        public int Weight;
    }

    public sealed class SoulRefineRatio
    {
        public int RatioId;
        public int Grade;
        public int Index;
        public int Rate;
        public int Ratio;
        public int NeedItem1;
        public int ItemCount1;
        public int NeedItem2;
        public int ItemCount2;
        public int NeedItem3;
        public int ItemCount3;
        public int NeedItem4;
        public int ItemCount4;
    }

    public sealed class ElfInfo
    {
        public int TemplateId;
        public string Name = "";
        public int StarLevel;
        public int AttackHint;
        public int HpHint;
    }

    public sealed class FarmRecipe
    {
        public int FoodId;
        public int VegetableId;
        public int NeedCount;
    }

    public sealed class SignReward
    {
        public int Day;
        public int TemplateId;
        public int Count;
    }

    public sealed class GodCardInfo
    {
        public int Id;
        public string Name = "";
        public string Pic = "";
        public int Composition;
        public int Decompose;
        public int Level;
    }

    public sealed class EngraveSetInfo
    {
        public int SetId;
        public string Name = "";
        public string HelpExplain = "";
    }

    public sealed class EngraveElementInfo
    {
        public int Id;
        public string Name = "";
        public int SetId;
        public int Demand;
        public string Attribute = "";
        public int Quality;
    }

    public sealed class StockInfo
    {
        public int StockId;
        public string StockName = "";
        public int BasePrice;
        public int FlowCoeffcient;
    }

    public sealed class MagicFusionRecipe
    {
        public int Id;
        public int ItemId;
        public int Type;
        public int NeedGold;
        public int NeedKey;
        public int GetKeys;
    }

    public sealed class TeamDungeonShopEntry
    {
        public int Id;
        public int ShopType;
        public int NeedLevel;
        public int Price;
        public int Condition;
        public int Value;
    }

    public sealed class StoryCopyChapter
    {
        public int Chapter;
        public string Name = "";
        public int SectionCount;
        public string AllStarAward = "";
        public string QuestBoxAward = "";
        public int QuestMaxScore;
        public string Detail = "";
    }

    public sealed class StoryCopySection
    {
        public int Chapter;
        public int Section;
        public string Name = "";
        public string Detail = "";
        public int MissionId;
        public int MapId;
        public int PlayLimit;
        public string ThreeStarAward = "";
        public string SweepReward = "";
    }

    public sealed class StoryCopyQuest
    {
        public int QuestId;
        public int ChapterId;
        public int ConditionType;
        public string Name = "";
        public int FinishCount;
        public string QuestAward = "";
        public int QuestScore;
        public string Detail = "";
    }

    public sealed class StoryCopyLevelUp
    {
        public int Chapter;
        public int PicId;
        public int PicLevel;
        public string Name = "";
        public int PicSoulCount;
        public int TemplateId;
        public int TemplateCount;
    }

    public sealed class WarriorFamFightConfig
    {
        public int HardType;
        public int Level;
        public int MissionId;
        public string FirstRewards = "";
        public string Rewards = "";
        public int Rank;
    }

    public sealed class WarriorFamRankEntry
    {
        public int Rank;
        public string Nick = "";
        public int Level;
        public int HardType;
        public int FightPower;
    }

    /// <summary>
    /// Loads every packed Request table the mobile client needs (templates, shop, quests, maps, balls, NPCs).
    /// Nested PC XML (<c>ItemTemplate/Item</c>, <c>Store/Item</c>) is flattened.
    /// </summary>
    public sealed class GameDatabase
    {
        public Dictionary<int, ItemTemplate> Items { get; } = new Dictionary<int, ItemTemplate>();
        public List<ShopOffer> Shop { get; } = new List<ShopOffer>();
        public List<QuestInfo> Quests { get; } = new List<QuestInfo>();
        public Dictionary<int, MapInfo> Maps { get; } = new Dictionary<int, MapInfo>();
        public Dictionary<int, BallPhysics> Balls { get; } = new Dictionary<int, BallPhysics>();
        public Dictionary<int, BombInfo> Bombs { get; } = new Dictionary<int, BombInfo>();
        public Dictionary<int, NpcInfo> Npcs { get; } = new Dictionary<int, NpcInfo>();
        public Dictionary<int, PetInfo> Pets { get; } = new Dictionary<int, PetInfo>();
        public Dictionary<int, PetSkillInfo> PetSkills { get; } = new Dictionary<int, PetSkillInfo>();
        public Dictionary<int, PetSkillElementInfo> PetSkillElements { get; } = new Dictionary<int, PetSkillElementInfo>();
        readonly Dictionary<int, int[]> _kindPassiveSkillIds = new Dictionary<int, int[]>();
        readonly Dictionary<int, int[]> _kindActiveSkillIds = new Dictionary<int, int[]>();
        readonly Dictionary<int, List<int>> _skillsByPicPassive = new Dictionary<int, List<int>>();
        readonly Dictionary<int, List<int>> _skillsByPicActive = new Dictionary<int, List<int>>();
        public List<CardInfo> Cards { get; } = new List<CardInfo>();
        public Dictionary<int, TitleInfo> Titles { get; } = new Dictionary<int, TitleInfo>();
        public Dictionary<int, TotemInfo> Totems { get; } = new Dictionary<int, TotemInfo>();
        public Dictionary<int, MountGrade> Mounts { get; } = new Dictionary<int, MountGrade>();
        public List<LotteryDrop> Lottery { get; } = new List<LotteryDrop>();
        public List<ShopOffer> VipShop { get; } = new List<ShopOffer>();
        public List<PveMission> Pve { get; } = new List<PveMission>();
        public Dictionary<int, SpiritInfo> Spirits { get; } = new Dictionary<int, SpiritInfo>();
        public Dictionary<long, FightSpiritTemplate> FightSpirits { get; } = new Dictionary<long, FightSpiritTemplate>();
        public static readonly int[] DefaultFightSpiritIds = { 100001, 100002, 100003, 100004, 100005 };
        public Dictionary<long, MagicStoneTemplate> MagicStones { get; } = new Dictionary<long, MagicStoneTemplate>();
        public static readonly int[] DefaultMagicStoneTemplateIds = { 100101, 100201, 100301, 100401 };
        public List<MagicFusionRecipe> MagicFusions { get; } = new List<MagicFusionRecipe>();
        public List<TeamDungeonShopEntry> TeamDungeonShop { get; } = new List<TeamDungeonShopEntry>();
        public List<CampWarReward> CampWarRewards { get; } = new List<CampWarReward>();
        public List<CelebEntry> CelebAreaFightPower { get; } = new List<CelebEntry>();
        public Dictionary<int, NecklaceCastingLevel> NecklaceLevels { get; } = new Dictionary<int, NecklaceCastingLevel>();
        public List<DevilTreasItem> DevilTreasItems { get; } = new List<DevilTreasItem>();
        public Dictionary<int, ActivityConfigEntry> ActivityConfigs { get; } = new Dictionary<int, ActivityConfigEntry>();
        public List<FirstPayShopItem> FirstPayShop { get; } = new List<FirstPayShopItem>();
        public List<EmblemTemplate> EmblemList { get; } = new List<EmblemTemplate>();
        public Dictionary<int, SoulStampTemplate> SoulStampTemplates { get; } = new Dictionary<int, SoulStampTemplate>();
        public Dictionary<int, SoulStampComposeTemplate> SoulStampCompose { get; } = new Dictionary<int, SoulStampComposeTemplate>();
        readonly Dictionary<long, List<SoulStampProBand>> _soulStampProBands = new Dictionary<long, List<SoulStampProBand>>();
        readonly Dictionary<long, SoulRefineRatio> _soulRefineRatios = new Dictionary<long, SoulRefineRatio>();
        public List<StoryCopyChapter> StoryCopyChapters { get; } = new List<StoryCopyChapter>();
        public List<StoryCopySection> StoryCopySections { get; } = new List<StoryCopySection>();
        public List<StoryCopyQuest> StoryCopyQuests { get; } = new List<StoryCopyQuest>();
        public List<StoryCopyLevelUp> StoryCopyLevelUps { get; } = new List<StoryCopyLevelUp>();
        public List<WarriorFamFightConfig> WarriorFamFights { get; } = new List<WarriorFamFightConfig>();
        public List<WarriorFamRankEntry> WarriorFamRanks { get; } = new List<WarriorFamRankEntry>();
        public List<WarriorFamRankEntry> WarriorHighFamRanks { get; } = new List<WarriorFamRankEntry>();
        public Dictionary<int, MagicClothInfo> MagicCloths { get; } = new Dictionary<int, MagicClothInfo>();
        public List<MagicClothInfo> MagicClothList { get; } = new List<MagicClothInfo>();
        readonly Dictionary<int, List<ClothGroupPart>> _clothGroupParts = new Dictionary<int, List<ClothGroupPart>>();
        public Dictionary<int, ClothPropertyInfo> ClothProperties { get; } = new Dictionary<int, ClothPropertyInfo>();
        public Dictionary<int, HonorSystemLevelInfo> HonorSystemLevels { get; } = new Dictionary<int, HonorSystemLevelInfo>();
        public Dictionary<int, TotemHonorEntry> TotemHonorEntries { get; } = new Dictionary<int, TotemHonorEntry>();
        public Dictionary<int, ElfInfo> Elves { get; } = new Dictionary<int, ElfInfo>();
        public List<FarmRecipe> Farm { get; } = new List<FarmRecipe>();
        public Dictionary<int, int> StrengthenRock { get; } = new Dictionary<int, int>();
        public List<SignReward> SignIn { get; } = new List<SignReward>();
        public Dictionary<int, GodCardInfo> GodCards { get; } = new Dictionary<int, GodCardInfo>();
        public Dictionary<int, EngraveSetInfo> EngraveSets { get; } = new Dictionary<int, EngraveSetInfo>();
        public List<EngraveElementInfo> EngraveElements { get; } = new List<EngraveElementInfo>();
        public Dictionary<int, StockInfo> Stocks { get; } = new Dictionary<int, StockInfo>();
        public Dictionary<string, string> ServerConfig { get; } = new Dictionary<string, string>();
        public List<FightLabDrop> FightLabDrops { get; } = new List<FightLabDrop>();
        public List<LevelGrade> Levels { get; } = new List<LevelGrade>();
        public Dictionary<int, FightPropTemplate> FightPropsByPic { get; } = new Dictionary<int, FightPropTemplate>();
        public List<CelebEntry> CelebGpDay { get; } = new List<CelebEntry>();
        public List<CelebEntry> CelebFightPowerDay { get; } = new List<CelebEntry>();
        public List<CelebEntry> CelebOfferDay { get; } = new List<CelebEntry>();
#if !GUNMOBILE_STANDALONE
        public CharacterDefine CharacterDef { get; private set; }
#endif

        /// <summary>Mobile battle UI prop slots (game_prop_N.png).</summary>
        public static readonly int[] BattlePropPicIds = { 1, 2, 4, 5, 6, 7 };

        public static GameDatabase Load(ResLoader loader)
        {
            var db = new GameDatabase();
            db.LoadItems(loader);
            db.LoadShop(loader);
            db.LoadQuests(loader);
            db.LoadMaps(loader);
            db.LoadBalls(loader);
            db.LoadBombs(loader);
            db.LoadNpcs(loader);
            db.LoadPets(loader);
            db.LoadPetSkills(loader);
            db.LoadPetSkillElements(loader);
            db.BuildKindPassiveSkillMap();
            db.LoadCards(loader);
            db.LoadTitles(loader);
            db.LoadTotems(loader);
            db.LoadMounts(loader);
            db.LoadLottery(loader);
            db.LoadVip(loader);
            db.LoadPve(loader);
            db.LoadSpirits(loader);
            db.LoadFightSpirits(loader);
            db.LoadMagicStones(loader);
            db.LoadMagicFusions(loader);
            db.LoadTeamDungeonShop(loader);
            db.LoadCampWar(loader);
            db.LoadNecklace(loader);
            db.LoadEmblems(loader);
            db.LoadSoulStamps(loader);
            db.LoadStoryCopy(loader);
            db.LoadWarriorFam(loader);
            db.LoadMagicCloths(loader);
            db.LoadClothGroups(loader);
            db.LoadClothProperties(loader);
            db.LoadHonorSystem(loader);
            db.LoadTotemHonor(loader);
            db.LoadDevilTreas(loader);
            db.LoadActivityConfig(loader);
            db.LoadFirstPayShop(loader);
            db.LoadFirstCopy(loader);
            db.LoadElves(loader);
            db.LoadFarm(loader);
            db.LoadStrengthen(loader);
            db.LoadSignIn(loader);
            db.LoadGodCards(loader);
            db.LoadEngrave(loader);
            db.LoadStocks(loader);
            db.LoadServerConfig(loader);
            db.LoadFightLabDrops(loader);
            db.LoadLevels(loader);
            db.LoadCelebLists(loader);
#if !GUNMOBILE_STANDALONE
            db.LoadCharacterDefine(loader);
#endif
            Debug.Log($"GunMobile DB items={db.Items.Count} shop={db.Shop.Count} quests={db.Quests.Count} maps={db.Maps.Count} balls={db.Balls.Count} pets={db.Pets.Count} npcs={db.Npcs.Count} pve={db.Pve.Count} levels={db.Levels.Count} fightProps={db.FightPropsByPic.Count} celebGp={db.CelebGpDay.Count} cfg={db.ServerConfig.Count}");
            return db;
        }

        public int ConfigInt(string name, int fallback = 0)
        {
            if (!ServerConfig.TryGetValue(name, out string raw) || string.IsNullOrEmpty(raw))
            {
                return fallback;
            }

            int comma = raw.IndexOf(',');
            string head = comma < 0 ? raw : raw.Substring(0, comma);
            return int.TryParse(head.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : fallback;
        }

        public float ConfigFloat(string name, float fallback = 0f)
        {
            if (!ServerConfig.TryGetValue(name, out string raw) || string.IsNullOrEmpty(raw))
            {
                return fallback;
            }

            return float.TryParse(raw.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float n) ? n : fallback;
        }

        public int ConfigPipeInt(string name, int index, int fallback = 0)
        {
            if (!ServerConfig.TryGetValue(name, out string raw) || string.IsNullOrEmpty(raw))
            {
                return fallback;
            }

            string[] parts = raw.Split('|');
            if (parts.Length == 0)
            {
                return fallback;
            }

            int i = Mathf.Clamp(index, 0, parts.Length - 1);
            return int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : fallback;
        }

        public int LevelTierIndex(int level)
        {
            return Mathf.Clamp((Mathf.Max(1, level) - 1) / 7, 0, 9);
        }

        public int BattleWinGold()
        {
            return ConfigInt("TakeCardMoney", 486);
        }

        public int BattleLoseGold()
        {
            int win = BattleWinGold();
            float discount = ConfigFloat("TakeCardDiscount", 0.1f);
            return Mathf.Max(0, Mathf.RoundToInt(win * discount));
        }

        public int BattleWinHonor(int level, bool pve)
        {
            string key = pve ? "MissionAwardRicheOffer" : "MissionAwardOffer";
            return ConfigPipeInt(key, LevelTierIndex(level), 500);
        }

        public int MountUpgradeCost(int currentGrade)
        {
            int next = currentGrade + 1;
            if (!Mounts.TryGetValue(next, out MountGrade nextGrade))
            {
                return 0;
            }

            int prevExp = 0;
            if (Mounts.TryGetValue(currentGrade, out MountGrade curGrade))
            {
                prevExp = curGrade.Experience;
            }

            return Mathf.Max(0, nextGrade.Experience - prevExp);
        }

        public int GemUpgradeCost(int currentLevel)
        {
            int next = currentLevel + 1;
            foreach (SpiritInfo s in Spirits.Values)
            {
                if (s.Level == next)
                {
                    return s.ReferenceCost;
                }
            }

            return ConfigInt("MustFusionGold", 400);
        }

        public static long FightSpiritKey(int spiritId, int level)
        {
            return ((long)spiritId << 16) | (uint)level;
        }

        public FightSpiritTemplate GetFightSpirit(int spiritId, int level)
        {
            FightSpirits.TryGetValue(FightSpiritKey(spiritId, level), out FightSpiritTemplate row);
            return row;
        }

        public int FightSpiritUpgradeCost(int spiritId, int currentLevel)
        {
            FightSpiritTemplate next = GetFightSpirit(spiritId, currentLevel + 1);
            if (next == null)
            {
                return 0;
            }

            FightSpiritTemplate cur = GetFightSpirit(spiritId, currentLevel);
            int delta = cur != null ? next.Exp - cur.Exp : next.Exp;
            return Mathf.Max(100, delta / 100);
        }

        public void ApplyFightSpiritStats(IReadOnlyList<FightSpiritSlot> slots, ref int atk, ref int def, ref int agi, ref int luck, ref int hp)
        {
            if (slots == null)
            {
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                FightSpiritSlot slot = slots[i];
                if (slot == null || slot.Level <= 0)
                {
                    continue;
                }

                FightSpiritTemplate row = GetFightSpirit(slot.SpiritId, slot.Level);
                if (row == null)
                {
                    continue;
                }

                atk += row.Attack / 100;
                def += row.Defence / 100;
                agi += row.Agility / 100;
                luck += row.Lucky / 100;
                hp += row.Blood / 100;
            }
        }

        public static long MagicStoneKey(int templateId, int level)
        {
            return ((long)templateId << 16) | (uint)level;
        }

        public MagicStoneTemplate GetMagicStone(int templateId, int level)
        {
            MagicStones.TryGetValue(MagicStoneKey(templateId, level), out MagicStoneTemplate row);
            return row;
        }

        public int MagicStoneUpgradeCost(int templateId, int currentLevel)
        {
            MagicStoneTemplate next = GetMagicStone(templateId, currentLevel + 1);
            if (next == null)
            {
                return 0;
            }

            MagicStoneTemplate cur = GetMagicStone(templateId, currentLevel);
            int delta = cur != null ? next.Exp - cur.Exp : next.Exp;
            return Mathf.Max(100, delta / 100);
        }

        public void ApplyMagicStoneStats(IReadOnlyList<MagicStoneSlot> slots, ref int atk, ref int def, ref int agi, ref int luck, ref int magicAtk, ref int magicDef)
        {
            if (slots == null)
            {
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                MagicStoneSlot slot = slots[i];
                if (slot == null || slot.Level <= 0)
                {
                    continue;
                }

                MagicStoneTemplate row = GetMagicStone(slot.TemplateId, slot.Level);
                if (row == null)
                {
                    continue;
                }

                atk += row.Attack;
                def += row.Defence;
                agi += row.Agility;
                luck += row.Luck;
                magicAtk += row.MagicAttack;
                magicDef += row.MagicDefence;
            }
        }

        public MagicFusionRecipe GetMagicFusion(int id)
        {
            for (int i = 0; i < MagicFusions.Count; i++)
            {
                if (MagicFusions[i].Id == id)
                {
                    return MagicFusions[i];
                }
            }

            return null;
        }

        public int TeamDungeonNpcId(int shopType)
        {
            switch (shopType)
            {
                case 113: return 44401;
                case 114: return 44403;
                case 115: return 44405;
                case 116: return 44407;
                default: return 44401;
            }
        }

        public StoryCopySection GetStoryCopySection(int chapter, int section)
        {
            for (int i = 0; i < StoryCopySections.Count; i++)
            {
                StoryCopySection row = StoryCopySections[i];
                if (row.Chapter == chapter && row.Section == section) return row;
            }
            return null;
        }

        public StoryCopyChapter GetStoryCopyChapter(int chapter)
        {
            for (int i = 0; i < StoryCopyChapters.Count; i++)
            {
                if (StoryCopyChapters[i].Chapter == chapter) return StoryCopyChapters[i];
            }
            return null;
        }

        public WarriorFamFightConfig GetWarriorFamFight(int hardType, int level)
        {
            for (int i = 0; i < WarriorFamFights.Count; i++)
            {
                WarriorFamFightConfig row = WarriorFamFights[i];
                if (row.HardType == hardType && row.Level == level) return row;
            }
            return null;
        }

        public List<(int templateId, int count)> ParseRewardPairs(string raw)
        {
            var list = new List<(int templateId, int count)>();
            if (string.IsNullOrWhiteSpace(raw)) return list;
            if (!raw.Contains(",") && raw.Contains("|"))
            {
                string[] p = raw.Split('|');
                if (p.Length >= 2 && int.TryParse(p[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int t) &&
                    int.TryParse(p[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int c))
                    list.Add((t, c));
                return list;
            }
            foreach (string seg in raw.Split('|'))
            {
                string[] p = seg.Split(',');
                if (p.Length >= 2 && int.TryParse(p[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int t) &&
                    int.TryParse(p[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int c))
                    list.Add((t, c));
            }
            return list;
        }

        public void GrantRewardPairs(ServerPlayer player, string raw)
        {
            if (player == null || string.IsNullOrWhiteSpace(raw)) return;
            foreach (var pair in ParseRewardPairs(raw))
                player.GrantTemplateReward(this, pair.templateId, pair.count);
        }

        public int DreamlandNpcId(StoryCopySection section, int playerLevel)
        {
            if (section == null) return 44401;
            if (Npcs.ContainsKey(section.MissionId)) return section.MissionId;
            NpcInfo npc = PickNpc(section.MissionId, playerLevel, 500000);
            return npc != null ? npc.Id : 44401;
        }

        public int DreamlandMapId(StoryCopySection section) =>
            section != null && section.MapId > 0 ? section.MapId : 71003;

        public int DreamlandEntryFee(StoryCopySection section) =>
            section != null ? section.PlayLimit * 100 : 200;

        public int DreamlandRewardGold(StoryCopySection section, int npcId)
        {
            if (section == null) return 0;
            int gold = ComputePveWinGold(npcId, 0, false);
            return gold > 0 ? gold : section.PlayLimit * 50;
        }

        public int WarriorFamNpcId(WarriorFamFightConfig cfg)
        {
            if (cfg == null) return 40001;
            if (Npcs.ContainsKey(cfg.MissionId)) return cfg.MissionId;
            NpcInfo npc = PickNpc(cfg.MissionId, 40 + cfg.Level, 999999999);
            return npc != null ? npc.Id : 40001;
        }

        public int WarriorFamEntryFee()
        {
            int perFloor = ConfigInt("WarriorFamRaidPricePerFloor", 0);
            return perFloor > 0 ? perFloor : ConfigInt("WarriorFamRaidPriceSmall", 30000) / 100;
        }

        public int WarriorFamRewardGold(WarriorFamFightConfig cfg)
        {
            if (cfg == null) return 0;
            foreach (var pair in ParseRewardPairs(cfg.Rewards))
                if (pair.templateId == 11107 || IsGoldTemplate(pair.templateId)) return pair.count;
            return 0;
        }

        public int ConsortiaCreateCost()
        {
            return ConfigInt("MustFusionGold", 400) * 10;
        }

        public int LotteryDrawCost(int count)
        {
            if (count >= 10)
            {
                return ConfigInt("NewLotteryOpenMoney", 100) * 10;
            }

            return ConfigInt("LotteryMoney", 100);
        }

        public int TreasureDrawCost()
        {
            return ConfigInt("TreasureHuntMoney", 200);
        }

        public int CarnivalDrawCost()
        {
            return ConfigInt("CarnivalDrawMoney", 500);
        }

        public List<LotteryDrop> LotteryPool(int minType, int maxType)
        {
            var pool = new List<LotteryDrop>();
            for (int i = 0; i < Lottery.Count; i++)
            {
                LotteryDrop d = Lottery[i];
                if (d.Type >= minType && d.Type <= maxType)
                {
                    pool.Add(d);
                }
            }

            return pool;
        }

        public CampWarReward CampWarRewardForRank(int rank)
        {
            CampWarReward best = null;
            for (int i = 0; i < CampWarRewards.Count; i++)
            {
                CampWarReward r = CampWarRewards[i];
                if (rank >= r.MinRank && rank <= r.MaxRank)
                {
                    if (best == null || r.MaxRank - r.MinRank < best.MaxRank - best.MinRank)
                    {
                        best = r;
                    }
                }
            }

            return best;
        }

        public CelebEntry GetPeakBattleTarget(int rankIndex)
        {
            List<CelebEntry> list = CelebAreaFightPower.Count > 0 ? CelebAreaFightPower : CelebFightPowerDay;
            if (rankIndex < 0 || rankIndex >= list.Count)
            {
                return null;
            }

            return list[rankIndex];
        }

        public int PeakBattleNpcId(CelebEntry celeb)
        {
            if (celeb == null)
            {
                return 44401;
            }

            int grade = Mathf.Max(10, celeb.Grade);
            NpcInfo npc = PickNpc(grade * 97, grade, 500000);
            return npc != null ? npc.Id : 44401;
        }

        public int WorldBossNpcId()
        {
            int bestId = 0;
            int bestBlood = 0;
            foreach (NpcInfo n in Npcs.Values)
            {
                if (n.Blood > bestBlood)
                {
                    bestBlood = n.Blood;
                    bestId = n.Id;
                }
            }

            return bestId > 0 ? bestId : 44410;
        }

        public NecklaceCastingLevel GetNecklaceLevel(int level)
        {
            NecklaceLevels.TryGetValue(level, out NecklaceCastingLevel row);
            return row;
        }

        public int NecklaceUpgradeCost(int currentLevel)
        {
            NecklaceCastingLevel next = GetNecklaceLevel(currentLevel + 1);
            if (next == null)
            {
                return 0;
            }

            return Mathf.Max(500, next.NeedItemCount1 * 20 + next.NeedItemCount2 * 30);
        }

        public void ApplyNecklaceBonus(int level, ref int hp, ref int def)
        {
            NecklaceCastingLevel row = GetNecklaceLevel(level);
            if (row == null)
            {
                return;
            }

            hp += row.Hp;
            def += row.Toughness / 10 + row.Guardian / 10;
        }

        public int HomeTempleUpgradeCost(int currentLevel)
        {
            return ConfigInt("HomeTempleUpgradeGold", 800) + currentLevel * 400;
        }

        public void ApplyHomeTempleBonus(int level, ref int atk, ref int hp)
        {
            hp += level * 120;
            atk += level * 15;
        }

        static long SoulStampProKey(int tempId, int proType) => ((long)tempId << 16) | (uint)proType;
        static long SoulRefineKey(int index, int grade) => ((long)index << 16) | (uint)grade;

        public int[] ConfigIntList(string name)
        {
            if (!ServerConfig.TryGetValue(name, out string raw) || string.IsNullOrEmpty(raw)) return Array.Empty<int>();
            string[] parts = raw.Split(',');
            var list = new List<int>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
                if (int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n)) list.Add(n);
            return list.ToArray();
        }

        public int EmblemComposeSuccessRate()
        {
            if (!ServerConfig.TryGetValue("EmblemComposeRandom", out string raw) || string.IsNullOrEmpty(raw)) return 700;
            string[] parts = raw.Split(',');
            return parts.Length > 0 && int.TryParse(parts[0].Trim(), out int n) ? n : 700;
        }

        public int EmblemComposeSkillRate()
        {
            if (!ServerConfig.TryGetValue("EmblemComposeRandom", out string raw) || string.IsNullOrEmpty(raw)) return 400;
            string[] parts = raw.Split(',');
            return parts.Length > 1 && int.TryParse(parts[1].Trim(), out int n) ? n : 400;
        }

        public int[] EmblemSkillIds() => ConfigIntList("EmblemSkillIds");

        public int EmblemCraftGoldCost(EmblemTemplate row)
        {
            if (row == null) return 0;
            return ConfigInt("EmblemComposeMoney", 2000) + row.ItemCount1 * 10 + row.ItemCount2 * 15 + row.ItemCount3 * 20 + row.ItemCount4 * 25;
        }

        public int RollRange(string range, System.Random rng)
        {
            if (string.IsNullOrEmpty(range) || rng == null) return 0;
            int dash = range.IndexOf('-');
            if (dash < 0) return int.TryParse(range.Trim(), out int single) ? single : 0;
            int min = int.TryParse(range.Substring(0, dash).Trim(), out int a) ? a : 0;
            int max = int.TryParse(range.Substring(dash + 1).Trim(), out int b) ? b : min;
            if (max < min) { int t = min; min = max; max = t; }
            return rng.Next(min, max + 1);
        }

        static void ApplyEmblemValue(int mainType, int value, ref int atk, ref int def, ref int agi, ref int luck, ref int hp, ref int magicAtk, ref int magicDef)
        {
            switch (mainType)
            {
                case 1: atk += value; break;
                case 2: agi += value; break;
                case 3: def += value; break;
                case 4: magicAtk += value; break;
                case 5: hp += value; break;
                case 6: magicDef += value; break;
                case 7: luck += value; break;
                case 8: atk += value / 2; def += value / 2; break;
                default: luck += value / 2; break;
            }
        }

        public void ApplyEmblemStats(IReadOnlyList<EmblemSlot> slots, ref int atk, ref int def, ref int agi, ref int luck, ref int hp, ref int magicAtk, ref int magicDef)
        {
            if (slots == null) return;
            for (int i = 0; i < slots.Count; i++)
            {
                EmblemSlot slot = slots[i];
                if (slot == null || slot.Equipped == 0) continue;
                ApplyEmblemValue(slot.MainType, slot.MainValue, ref atk, ref def, ref agi, ref luck, ref hp, ref magicAtk, ref magicDef);
                if (slot.SubValue > 0) ApplyEmblemValue(7, slot.SubValue, ref atk, ref def, ref agi, ref luck, ref hp, ref magicAtk, ref magicDef);
            }
        }

        public SoulStampComposeTemplate GetSoulStampCompose(int quality) { SoulStampCompose.TryGetValue(quality, out SoulStampComposeTemplate row); return row; }
        public int SoulStampComposeGoldCost(SoulStampComposeTemplate compose) => compose != null ? compose.ComposePreCost : 0;
        public SoulStampTemplate GetSoulStamp(int tempId) { SoulStampTemplates.TryGetValue(tempId, out SoulStampTemplate row); return row; }

        public SoulStampTemplate PickSoulStampByQuality(int quality, System.Random rng)
        {
            var pool = new List<SoulStampTemplate>();
            foreach (var kv in SoulStampTemplates) if (kv.Value != null && kv.Value.Quality == quality) pool.Add(kv.Value);
            return pool.Count == 0 || rng == null ? null : pool[rng.Next(0, pool.Count)];
        }

        public int PickSoulStampProType(SoulStampTemplate row, System.Random rng)
        {
            if (row?.ProTypes == null || row.ProTypes.Length == 0 || rng == null) return 1;
            return row.ProTypes[rng.Next(0, row.ProTypes.Length)];
        }

        public int RollSoulStampProValue(int tempId, int proType, System.Random rng)
        {
            if (!_soulStampProBands.TryGetValue(SoulStampProKey(tempId, proType), out List<SoulStampProBand> bands) || bands.Count == 0 || rng == null) return 0;
            int total = 0; foreach (var b in bands) total += b.Weight;
            if (total <= 0) return bands[0].Min;
            int roll = rng.Next(0, total);
            for (int i = 0; i < bands.Count; i++) { roll -= bands[i].Weight; if (roll < 0) { var band = bands[i]; return rng.Next(band.Min, band.Max + 1); } }
            var last = bands[bands.Count - 1]; return rng.Next(last.Min, last.Max + 1);
        }

        static int ParseGradeSkill(string upSkillId, int grade, int fallback)
        {
            if (string.IsNullOrEmpty(upSkillId)) return fallback;
            foreach (string part in upSkillId.Split('|'))
            {
                string[] seg = part.Split(',');
                if (seg.Length >= 2 && int.TryParse(seg[0], out int g) && g == grade && int.TryParse(seg[1], out int skill)) return skill;
            }
            return fallback;
        }

        public int SoulStampSkillId(SoulStampTemplate row, int grade) => row == null ? 0 : (grade <= 1 ? row.SkillId : ParseGradeSkill(row.UpSkillId, grade, row.SkillId));

        public SoulRefineRatio GetSoulRefine(int typeIndex, int grade) { _soulRefineRatios.TryGetValue(SoulRefineKey(typeIndex, grade), out SoulRefineRatio row); return row; }

        public int SoulStampRefineGoldCost(SoulRefineRatio ratio) => ratio == null ? 0 : ratio.ItemCount1 * 10 + ratio.ItemCount2 * 15 + ratio.ItemCount3 * 20 + ratio.ItemCount4 * 25 + ratio.Ratio * 100;

        public void ApplySoulStampStats(IReadOnlyList<SoulStampSlot> slots, ref int atk, ref int def, ref int agi, ref int luck, ref int hp)
        {
            if (slots == null) return;
            for (int i = 0; i < slots.Count; i++)
            {
                SoulStampSlot slot = slots[i];
                if (slot == null || slot.Equipped == 0) continue;
                switch (slot.ProType) { case 1: atk += slot.ProValue; break; case 2: def += slot.ProValue; break; case 3: agi += slot.ProValue; break; case 4: luck += slot.ProValue; break; default: hp += slot.ProValue / 2; break; }
            }
        }

        public MagicClothInfo GetMagicCloth(int clothId)
        {
            MagicCloths.TryGetValue(clothId, out MagicClothInfo row);
            return row;
        }

        public ClothPropertyInfo GetClothProperty(int propertyId)
        {
            ClothProperties.TryGetValue(propertyId, out ClothPropertyInfo row);
            return row;
        }

        public bool MagicClothMatchesSex(int playerSex, int clothSex)
        {
            int normalized = playerSex == 1 ? 1 : 0;
            return clothSex == normalized || clothSex == 3 || clothSex == 4;
        }

        public void ApplyMagicClothOutfit(MagicClothInfo cloth, ref int equipHead, ref int equipHair, ref int equipFace,
            ref int equipCloth, ref int equipGlass, ref int equipWeapon)
        {
            if (cloth == null) return;
            if (cloth.HeadId > 0) equipHead = cloth.HeadId;
            if (cloth.HairId > 0) equipHair = cloth.HairId;
            if (cloth.FaceId > 0) equipFace = cloth.FaceId;
            if (cloth.ClothId > 0) equipCloth = cloth.ClothId;
            if (cloth.GlassId > 0) equipGlass = cloth.GlassId;
        }

        public void ApplyWardrobeBonus(IReadOnlyList<int> ownedPropertyIds, ref int atk, ref int def, ref int agi,
            ref int luck, ref int hp, ref int baseDmg, ref int baseGuard)
        {
            if (ownedPropertyIds == null) return;
            for (int i = 0; i < ownedPropertyIds.Count; i++)
            {
                if (!ClothProperties.TryGetValue(ownedPropertyIds[i], out ClothPropertyInfo row) || row == null) continue;
                atk += row.Attack; def += row.Defend; agi += row.Agility; luck += row.Luck;
                hp += row.Blood; baseDmg += row.Damage; baseGuard += row.Guard;
            }
        }

        public HonorSystemLevelInfo GetHonorSystemLevel(int level)
        {
            HonorSystemLevels.TryGetValue(level, out HonorSystemLevelInfo row);
            return row;
        }

        public int HonorSystemLevelFromExp(int exp)
        {
            int best = 0;
            foreach (HonorSystemLevelInfo row in HonorSystemLevels.Values)
                if (row.Exp <= exp && row.Level > best) best = row.Level;
            return best;
        }

        public TotemHonorEntry GetTotemHonorEntry(int id)
        {
            TotemHonorEntries.TryGetValue(id, out TotemHonorEntry row);
            return row;
        }

        public int HonorSystemLikeHonorGain()
        {
            if (!ServerConfig.TryGetValue("HonorSystemLikeAddHonor", out string raw) || string.IsNullOrEmpty(raw)) return 5;
            string[] parts = raw.Split(',');
            if (parts.Length >= 2 && int.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int gain)) return gain;
            return 5;
        }

        public int HonorSystemFightHonorGain()
        {
            if (!ServerConfig.TryGetValue("HonorSystemFightConfig", out string raw) || string.IsNullOrEmpty(raw)) return 3;
            int pipe = raw.IndexOf('|'); string head = pipe < 0 ? raw : raw.Substring(0, pipe);
            int comma = head.IndexOf(',');
            if (comma >= 0 && int.TryParse(head.Substring(comma + 1).Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int gain)) return gain;
            return 3;
        }

        public int HonorSystemOpLimit()
        {
            if (!ServerConfig.TryGetValue("HonorSystemOpLimit", out string raw) || string.IsNullOrEmpty(raw)) return 50;
            string[] parts = raw.Split(',');
            if (parts.Length > 0 && int.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int limit)) return limit;
            return 50;
        }

        public void ApplyHonorSystemBonus(int level, ref int atk, ref int def, ref int agi, ref int luck, ref int hp)
        {
            HonorSystemLevelInfo row = GetHonorSystemLevel(level);
            if (row == null) return;
            hp += row.Blood;
            int statBonus = row.StrengthRate / 100;
            atk += statBonus; def += statBonus; agi += statBonus; luck += statBonus;
            atk += row.AdvanceRate / 100; def += row.GoldRate / 100;
            agi += row.SpiritRate / 100; luck += row.FusionRate / 100;
        }

        public DevilTreasItem RollDevilTreas(System.Random rng)
        {
            if (DevilTreasItems.Count == 0)
            {
                return null;
            }

            int total = 0;
            for (int i = 0; i < DevilTreasItems.Count; i++)
            {
                total += Mathf.Max(1, DevilTreasItems[i].Weight);
            }

            int roll = rng != null ? rng.Next(0, total) : 0;
            for (int i = 0; i < DevilTreasItems.Count; i++)
            {
                roll -= Mathf.Max(1, DevilTreasItems[i].Weight);
                if (roll < 0)
                {
                    return DevilTreasItems[i];
                }
            }

            return DevilTreasItems[0];
        }

        public FirstRechargeConfig GetFirstRechargeConfig()
        {
            if (!ActivityConfigs.TryGetValue(8, out ActivityConfigEntry entry) || entry == null)
            {
                return null;
            }

            return new FirstRechargeConfig
            {
                RewardItemIds = ParseCsvInts(entry.Params2),
                RewardCounts = ParseCsvInts(entry.Params3),
                ExtraItemId1 = FirstInt(entry.Params4),
                ExtraItemId2 = FirstInt(entry.Params5),
                RankAwardId = FirstInt(entry.RankAreaAward)
            };
        }

        public FirstPayShopItem GetFirstPayShopItem(int templateId)
        {
            for (int i = 0; i < FirstPayShop.Count; i++)
            {
                if (FirstPayShop[i].TemplateId == templateId)
                {
                    return FirstPayShop[i];
                }
            }

            return null;
        }

        public int LevelFromGp(int gp)
        {
            int level = 1;
            foreach (LevelGrade row in Levels)
            {
                if (gp >= row.Gp)
                {
                    level = row.Grade;
                }
                else
                {
                    break;
                }
            }

            return Mathf.Max(1, level);
        }

        public int GpForLevel(int level)
        {
            foreach (LevelGrade row in Levels)
            {
                if (row.Grade == level)
                {
                    return row.Gp;
                }
            }

            return 0;
        }

        public int BloodForLevel(int level)
        {
            foreach (LevelGrade row in Levels)
            {
                if (row.Grade == level)
                {
                    return row.Blood;
                }
            }

            return 500 + level * 30;
        }

        public int BattleWinGp(int level, bool pve)
        {
            int raw = ConfigPipeInt(pve ? "MissionAwardGP" : "MissionAwardGP", LevelTierIndex(level), 100);
            if (raw >= 10000)
            {
                raw /= 1000;
            }

            return Mathf.Max(1, raw);
        }

        public int AuctionPrice(ItemTemplate item)
        {
            if (item == null)
            {
                return 0;
            }

            if (item.ReclaimValue > 0)
            {
                return item.ReclaimValue;
            }

            if (item.FloorPrice > 0)
            {
                return item.FloorPrice;
            }

            return Mathf.Max(80, (item.Attack + item.Defence) * 12);
        }

        public int FarmBuyVegetableCost()
        {
            return ConfigInt("MustFusionGold", 200);
        }

        public int KingBlessGold(int vipLevel)
        {
            return ConfigInt("TakeCardMoney", 486) / 2 + vipLevel * 80;
        }

        public int BattleTurnSeconds()
        {
            return ConfigInt("EndFightTime", 20);
        }

        public int VipUpgradeGiftCost()
        {
            return ConfigInt("DefaultGiftToken", 500);
        }

        public int TexpTrainGoldCost()
        {
            return ConfigInt("MustFusionGold", 400);
        }

        public int TexpTrainGain()
        {
            return ConfigInt("DispatchesMoney", 25);
        }

        public int StrengthenGoldCost(int nextLevel)
        {
            int rock = 200 * nextLevel;
            if (StrengthenRock.TryGetValue(nextLevel, out int r))
            {
                rock = r;
            }

            return Mathf.Max(100, rock * 40);
        }

        public int StrengthenSuccessChance(int currentLevel)
        {
            int[] rates = ConfigPipeInts("DevilIntervalRandomRate");
            if (rates.Length > 0)
            {
                int idx = Mathf.Clamp(currentLevel, 0, rates.Length - 1);
                int sum = 0;
                foreach (int v in rates)
                {
                    sum += v;
                }

                if (sum > 0)
                {
                    return Mathf.Clamp(Mathf.RoundToInt(rates[idx] * 100f / sum), 5, 95);
                }
            }

            return Mathf.Clamp(90 - currentLevel * 5, 20, 90);
        }

        int[] ConfigPipeInts(string name)
        {
            if (!ServerConfig.TryGetValue(name, out string raw) || string.IsNullOrEmpty(raw))
            {
                return System.Array.Empty<int>();
            }

            string[] parts = raw.Split(',');
            var list = new List<int>(parts.Length);
            foreach (string part in parts)
            {
                if (int.TryParse(part.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n))
                {
                    list.Add(n);
                }
            }

            return list.ToArray();
        }

        public static int FightPropBitIndex(int propPicId)
        {
            for (int i = 0; i < BattlePropPicIds.Length; i++)
            {
                if (BattlePropPicIds[i] == propPicId)
                {
                    return i;
                }
            }

            return -1;
        }

        public void ApplyFightProp(int propPicId, out float dmgMul, out float radiusMul, out float powerAdd, out bool forceCrit)
        {
            dmgMul = 1f;
            radiusMul = 1f;
            powerAdd = 0f;
            forceCrit = false;
            if (propPicId == 0)
            {
                return;
            }

            if (FightPropsByPic.TryGetValue(propPicId, out FightPropTemplate prop))
            {
                if (prop.Property1 == 13 && prop.Property2 > 0)
                {
                    dmgMul = 1f + prop.Property2 / 100f;
                }
                else if (prop.Property1 == 15)
                {
                    dmgMul = 1.4f;
                    forceCrit = prop.Property2 >= 3;
                }

                if (prop.Property4 > 0)
                {
                    float fromP4 = prop.Property4 / 100f;
                    if (fromP4 > 1f)
                    {
                        dmgMul = Mathf.Max(dmgMul, fromP4);
                    }
                }

                if (prop.Property5 > 0)
                {
                    radiusMul = Mathf.Max(1f, prop.Property5 / 500f);
                }

                if (prop.Property7 > 0)
                {
                    powerAdd = prop.Property7 / 15f;
                }

                return;
            }

            switch (propPicId)
            {
                case 1: dmgMul = 1.25f; radiusMul = 1.35f; break;
                case 2: dmgMul = 1.2f; break;
                case 5: powerAdd = 12f; break;
                case 6: dmgMul = 1.4f; break;
                case 7: forceCrit = true; break;
            }
        }

        public int GenerateFightPropMask(System.Random rng)
        {
            int maxProps = ConfigInt("EscapePropMax", 3);
            int mask = 0;
            var pool = new List<int>();
            string weightCfg = ServerConfig.TryGetValue("EscapePropWeight", out string w) ? w : "";
            if (!string.IsNullOrEmpty(weightCfg))
            {
                foreach (string entry in weightCfg.Split('|'))
                {
                    if (string.IsNullOrWhiteSpace(entry))
                    {
                        continue;
                    }

                    string[] parts = entry.Split(',');
                    if (parts.Length < 1)
                    {
                        continue;
                    }

                    if (!int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int templateId))
                    {
                        continue;
                    }

                    ItemTemplate item = GetItem(templateId);
                    if (item == null || item.CategoryId != 10)
                    {
                        continue;
                    }

                    if (!int.TryParse(item.Pic, NumberStyles.Integer, CultureInfo.InvariantCulture, out int pic))
                    {
                        continue;
                    }

                    if (FightPropBitIndex(pic) < 0)
                    {
                        continue;
                    }

                    pool.Add(pic);
                }
            }

            if (pool.Count == 0)
            {
                pool.AddRange(BattlePropPicIds);
            }

            int picks = Mathf.Clamp(maxProps, 1, pool.Count);
            for (int i = 0; i < picks; i++)
            {
                int j = rng != null ? rng.Next(i, pool.Count) : i;
                int tmp = pool[i];
                pool[i] = pool[j];
                pool[j] = tmp;
                int bit = FightPropBitIndex(pool[i]);
                if (bit >= 0)
                {
                    mask |= 1 << bit;
                }
            }

            return mask;
        }

        public int ComputePveWinGold(int npcId, int labyrinthFloor, bool labyrinth)
        {
            if (labyrinth && labyrinthFloor > 0)
            {
                int easy = Mathf.Clamp((labyrinthFloor - 1) % 3, 0, 2);
                int gold = 0;
                foreach (FightLabDrop drop in FightLabDrops)
                {
                    if (drop.LabId != 1000 || drop.Easy != easy)
                    {
                        continue;
                    }

                    if (drop.AwardItem == -300 || drop.AwardItem == -1100)
                    {
                        gold += drop.Count;
                    }
                }

                return gold;
            }

            if (npcId != 0 && Npcs.TryGetValue(npcId, out NpcInfo npc))
            {
                return Mathf.Max(0, npc.Experience);
            }

            return 0;
        }

        public bool IsGoldTemplate(int templateId)
        {
            return templateId == -1100 || templateId == -300;
        }

        public bool IsGiftTemplate(int templateId)
        {
            return templateId < 0 && !IsGoldTemplate(templateId);
        }

        public NpcInfo GetNpc(int id)
        {
            Npcs.TryGetValue(id, out NpcInfo n);
            return n;
        }

        public CardInfo GetCard(int id)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].Id == id || Cards[i].CardId == id)
                {
                    return Cards[i];
                }
            }

            return null;
        }

        public int PickMapId(int seed)
        {
            List<MapInfo> maps = MapCatalog.Playable(this);
            if (maps.Count == 0)
            {
                return 1056;
            }

            return maps[Math.Abs(seed) % maps.Count].Id;
        }

        public NpcInfo PickNpc(int seed, int level, int maxBlood)
        {
            var near = new List<NpcInfo>();
            var any = new List<NpcInfo>();
            foreach (NpcInfo n in Npcs.Values)
            {
                if (n.Blood <= 0 || n.Blood > maxBlood)
                {
                    continue;
                }

                any.Add(n);
                if (Math.Abs(n.Level - level) <= 20)
                {
                    near.Add(n);
                }
            }

            List<NpcInfo> pool = near.Count > 0 ? near : any;
            if (pool.Count == 0)
            {
                return null;
            }

            return pool[Math.Abs(seed) % pool.Count];
        }

        public LivingStats MakeNpcLiving(int npcId)
        {
            if (!Npcs.TryGetValue(npcId, out NpcInfo npc) || npc == null)
            {
                return new LivingStats
                {
                    Attack = 110,
                    Defence = 85,
                    Agility = 70,
                    Luck = 40,
                    Hp = 1200,
                    MaxHp = 1200,
                    Team = 2
                };
            }

            ClientCombatStats(npc, out int hp, out int atk, out int def, out int agi, out int luk);
            int baseDmg = npc.BaseDamage > 0 ? npc.BaseDamage : atk;
            int baseGuard = npc.BaseGuard;
            return new LivingStats
            {
                Attack = atk,
                Defence = def,
                Agility = agi,
                Luck = luk,
                BaseDamage = baseDmg,
                BaseGuard = baseGuard,
                Grade = npc.Level > 0 ? npc.Level : 1,
                Hp = hp,
                MaxHp = hp,
                Team = 2
            };
        }

        /// <summary>Fill BaseDamage/BaseGuard/Grade from PC tables (weapon, card, totem, mount).</summary>
        public void ApplyPcDamageFields(ref LivingStats living, int level, int weaponId, int cardId, int totemId, int mountGrade)
        {
            living.Grade = level > 0 ? level : 1;
            int baseDmg = 0;
            int baseGuard = 0;
            ItemTemplate weapon = GetItem(weaponId);
            if (weapon != null)
            {
                baseDmg += weapon.Attack > 0 ? weapon.Attack : weapon.Property7;
            }

            foreach (CardInfo c in Cards)
            {
                if (c.Id == cardId)
                {
                    baseDmg += c.AddDamage;
                    baseGuard += c.AddGuard;
                    break;
                }
            }

            if (Totems.TryGetValue(totemId, out TotemInfo totem))
            {
                baseDmg += totem.AddDamage;
                baseGuard += totem.AddGuard;
            }

            if (Mounts.TryGetValue(mountGrade, out MountGrade mount))
            {
                baseDmg += mount.AddDamage;
                baseGuard += mount.AddGuard;
            }

            living.BaseDamage = baseDmg > 0 ? baseDmg : living.Attack;
            living.BaseGuard = baseGuard;
        }

        /// <summary>
        /// World-boss Blood values in NPCInfoList go into the billions. Cap so a
        /// phone match can finish while still scaling with Level.
        /// </summary>
        public static void ClientCombatStats(NpcInfo npc, out int hp, out int atk, out int def, out int agi, out int luck)
        {
            hp = npc.Blood <= 0 ? 800 : npc.Blood;
            atk = npc.Attack <= 0 ? 80 + npc.Level * 4 : npc.Attack;
            def = npc.Defence <= 0 ? 50 + npc.Level * 3 : npc.Defence;
            agi = npc.Agility > 0 ? npc.Agility : 40 + npc.Level;
            luck = npc.Lucky > 0 ? npc.Lucky : 20 + npc.Level / 2;
            if (hp > 6000)
            {
                hp = 1800 + Mathf.Min(npc.Level, 80) * 45 + Mathf.Min(hp / 200000, 2500);
            }

            if (atk > 420)
            {
                atk = 130 + npc.Level * 3;
            }

            if (def > 420)
            {
                def = 100 + npc.Level * 2;
            }

            if (agi > 300)
            {
                agi = 80 + npc.Level;
            }

            if (luck > 300)
            {
                luck = 40 + npc.Level / 2;
            }
        }

        public ItemTemplate GetItem(int templateId)
        {
            Items.TryGetValue(templateId, out ItemTemplate t);
            return t;
        }

        public BallPhysics GetBall(int id)
        {
            return Balls.TryGetValue(id, out BallPhysics b) ? b : BallPhysics.Default;
        }

        public int DefaultBallId(int weaponTemplateId)
        {
            if (Bombs.TryGetValue(weaponTemplateId, out BombInfo bomb) && bomb.Common > 0)
            {
                return bomb.Common;
            }

            return 1;
        }

        /// <summary>Default ball from bombconfig Common (no prop).</summary>
        public BallPhysics ResolveBall(int weaponTemplateId, int preferredBallId = 0)
        {
            return GetBall(ResolveBallId(weaponTemplateId, preferredBallId));
        }

        public int ResolveBallId(int weaponTemplateId, int preferredBallId = 0)
        {
            if (preferredBallId > 0)
            {
                return preferredBallId;
            }

            return DefaultBallId(weaponTemplateId);
        }

        /// <summary>
        /// PC bombconfig shot selection: Common / CommonAddWound / CommonMultiBall by fight prop.
        /// </summary>
        public int ResolveBallIdForShot(int weaponTemplateId, int preferredBallId, int propPicId)
        {
            if (preferredBallId > 0)
            {
                return preferredBallId;
            }

            if (!Bombs.TryGetValue(weaponTemplateId, out BombInfo bomb))
            {
                return DefaultBallId(weaponTemplateId);
            }

            if (PropUsesMultiBall(propPicId) && bomb.CommonMultiBall > 0)
            {
                return bomb.CommonMultiBall;
            }

            if (PropUsesAddWound(propPicId) && bomb.CommonAddWound > 0)
            {
                return bomb.CommonAddWound;
            }

            return bomb.Common > 0 ? bomb.Common : 1;
        }

        public BallPhysics ResolveBallForShot(int weaponTemplateId, int preferredBallId, int propPicId)
        {
            return GetBall(ResolveBallIdForShot(weaponTemplateId, preferredBallId, propPicId));
        }

        public BallPhysics ResolveSpecialBall(int weaponTemplateId)
        {
            if (Bombs.TryGetValue(weaponTemplateId, out BombInfo bomb) && bomb.Special > 0)
            {
                return GetBall(bomb.Special);
            }

            return ResolveBall(weaponTemplateId);
        }

        public bool PropUsesMultiBall(int propPicId)
        {
            if (propPicId == 4)
            {
                return true;
            }

            if (FightPropsByPic.TryGetValue(propPicId, out FightPropTemplate prop))
            {
                return prop.Property1 == 14 || prop.Property1 == 15;
            }

            return false;
        }

        public bool PropUsesAddWound(int propPicId)
        {
            if (propPicId == 1 || propPicId == 2 || propPicId == 6)
            {
                return true;
            }

            if (FightPropsByPic.TryGetValue(propPicId, out FightPropTemplate prop))
            {
                return prop.Property1 == 13 || prop.Property1 == 8;
            }

            return false;
        }

        public bool PropIgnoresArmour(int propPicId)
        {
            if (FightPropsByPic.TryGetValue(propPicId, out FightPropTemplate prop))
            {
                return prop.Property1 == 8;
            }

            return false;
        }

        public int ComputeBombHurt(BallPhysics ball, float propDmgMult = 1f)
        {
            return DamageCalculator.ComputeBombHurt(ball, propDmgMult);
        }

        /// <summary>BallList BombType=1 heals allies in blast radius (PC angel/heal bombs).</summary>
        public static bool BallIsHeal(BallPhysics ball)
        {
            return ball.BombType == 1;
        }

        public int PetMpMax(int petTemplateId)
        {
            if (petTemplateId > 0 && Pets.TryGetValue(petTemplateId, out PetInfo pet) && pet.Mp > 0)
            {
                return pet.Mp;
            }

            return 100;
        }

        public float PetSkillCooldownSec(PetSkillInfo skill)
        {
            if (skill == null)
            {
                return 0f;
            }

            int turnSec = BattleTurnSeconds();
            if (turnSec < 5)
            {
                turnSec = 20;
            }

            return skill.ColdDown > 0 ? skill.ColdDown * turnSec : turnSec * 2f;
        }

        public List<BattleEffect> BuildPetSkillEffects(PetSkillInfo skill, int sourceSeat, int targetSeat)
        {
            return BattleEffectParser.FromPetSkill(skill, PetSkillElements, sourceSeat, targetSeat);
        }

        public PetSkillInfo ResolvePetPassiveSkill(int petTemplateId)
        {
            if (petTemplateId <= 0 || !Pets.TryGetValue(petTemplateId, out PetInfo pet))
            {
                return null;
            }

            if (!_kindPassiveSkillIds.TryGetValue(pet.KindId, out int[] skillIds) || skillIds.Length == 0)
            {
                return null;
            }

            int idx = Mathf.Clamp(pet.StarLevel, 1, skillIds.Length) - 1;
            return PetSkills.TryGetValue(skillIds[idx], out PetSkillInfo skill) ? skill : null;
        }

        /// <summary>PC petskillinfo BallType 1/2 active skills (StarLevel 3+).</summary>
        public PetSkillInfo ResolvePetActiveSkill(int petTemplateId)
        {
            if (petTemplateId <= 0 || !Pets.TryGetValue(petTemplateId, out PetInfo pet))
            {
                return null;
            }

            if (pet.StarLevel < 3)
            {
                return null;
            }

            if (!_kindActiveSkillIds.TryGetValue(pet.KindId, out int[] skillIds) || skillIds.Length == 0)
            {
                return null;
            }

            int idx = Mathf.Clamp(pet.StarLevel - 1, 0, skillIds.Length - 1);
            return PetSkills.TryGetValue(skillIds[idx], out PetSkillInfo skill) ? skill : null;
        }

        public BallPhysics PetSkillBall(PetSkillInfo skill)
        {
            if (skill == null || skill.NewBallId <= 0)
            {
                return BallPhysics.Default;
            }

            return GetBall(skill.NewBallId);
        }

        public bool PetSkillForceCrit(PetSkillInfo skill)
        {
            return skill != null &&
                   !string.IsNullOrEmpty(skill.Description) &&
                   skill.Description.IndexOf("百分百暴击", StringComparison.Ordinal) >= 0;
        }

        public bool RollPetSkill(PetSkillInfo skill, int seed)
        {
            if (skill == null || skill.Probability <= 0)
            {
                return false;
            }

            if (skill.Probability >= 10000)
            {
                return true;
            }

            var rng = new System.Random(seed);
            return rng.Next(0, 10000) < skill.Probability;
        }

        void BuildKindPassiveSkillMap()
        {
            // KindID -> petskillinfo Pic groups (passive Pic, active Pic(s)).
            RegisterKindSkillPics(1, 1, 5);
            RegisterKindSkillPics(2, 4, 8);
            RegisterKindSkillPics(3, 2, 6);
            RegisterKindSkillPics(4, 3, 15, 16);
            RegisterKindSkillPics(18, 34, 35);
            RegisterKindSkillPics(19, 18, 52);
            RegisterKindSkillPics(20, 1, 5);
            RegisterKindSkillPics(22, 2, 6);
            RegisterKindSkillPics(24, 34, 35);
            RegisterKindSkillPics(32, 18, 52);
        }

        void RegisterKindSkillPics(int kindId, int passivePic, params int[] activePics)
        {
            if (_skillsByPicPassive.TryGetValue(passivePic, out List<int> passive) && passive.Count > 0)
            {
                _kindPassiveSkillIds[kindId] = passive.ToArray();
            }

            if (activePics == null || activePics.Length == 0)
            {
                return;
            }

            var merged = new List<int>();
            foreach (int activePic in activePics)
            {
                if (_skillsByPicActive.TryGetValue(activePic, out List<int> active))
                {
                    merged.AddRange(active);
                }
            }

            if (merged.Count > 0)
            {
                merged.Sort();
                _kindActiveSkillIds[kindId] = merged.ToArray();
            }
        }

        void IndexPetSkillByPic(int pic, int skillId, int ballType, int probability)
        {
            if (pic <= 0)
            {
                return;
            }

            if (ballType == 3 && probability == 10000)
            {
                if (!_skillsByPicPassive.TryGetValue(pic, out List<int> passive))
                {
                    passive = new List<int>();
                    _skillsByPicPassive[pic] = passive;
                }

                passive.Add(skillId);
                return;
            }

            if (ballType == 1 || ballType == 2)
            {
                if (!_skillsByPicActive.TryGetValue(pic, out List<int> active))
                {
                    active = new List<int>();
                    _skillsByPicActive[pic] = active;
                }

                active.Add(skillId);
            }
        }

        void FinalizePicSkillGroups()
        {
            foreach (List<int> ids in _skillsByPicPassive.Values)
            {
                ids.Sort();
            }

            foreach (List<int> ids in _skillsByPicActive.Values)
            {
                ids.Sort();
            }
        }

        static int ParsePetDamagePercent(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return 0;
            }

            int pctIdx = description.IndexOf('%');
            if (pctIdx > 0)
            {
                int start = pctIdx - 1;
                while (start >= 0 && char.IsDigit(description[start]))
                {
                    start--;
                }

                if (int.TryParse(description.Substring(start + 1, pctIdx - start - 1), out int pct))
                {
                    return pct;
                }
            }

            return 0;
        }

        static int ParseHealPercent(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return 0;
            }

            int idx = description.IndexOf("回复", StringComparison.Ordinal);
            if (idx < 0)
            {
                idx = description.IndexOf("恢复", StringComparison.Ordinal);
            }

            if (idx < 0)
            {
                return 0;
            }

            return ParsePetDamagePercent(description.Substring(idx));
        }

        void LoadItems(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TemplateAlllist.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/TemplateAllList.xml", out table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "TemplateID");
                if (id == 0)
                {
                    continue;
                }

                int categoryId = Int(row, "CategoryID");
                var item = new ItemTemplate
                {
                    TemplateId = id,
                    Name = Str(row, "Name"),
                    Description = Str(row, "Description"),
                    CategoryId = categoryId,
                    Attack = Int(row, "Attack"),
                    Defence = Int(row, "Defence"),
                    Agility = Int(row, "Agility"),
                    Luck = Int(row, "Luck"),
                    NeedLevel = Int(row, "NeedLevel"),
                    NeedSex = Int(row, "NeedSex"),
                    CanEquip = Bool(row, "CanEquip"),
                    CanUse = Bool(row, "CanUse"),
                    Pic = Str(row, "Pic"),
                    Quality = Int(row, "Quality"),
                    Level = Int(row, "Level"),
                    Property1 = Int(row, "Property1"),
                    Property2 = Int(row, "Property2"),
                    Property3 = Int(row, "Property3"),
                    Property4 = Int(row, "Property4"),
                    Property5 = Int(row, "Property5"),
                    Property6 = Int(row, "Property6"),
                    Property7 = Int(row, "Property7"),
                    Property8 = Int(row, "Property8"),
                    ReclaimValue = Int(row, "ReclaimValue"),
                    FloorPrice = Int(row, "FloorPrice")
                };
                Items[id] = item;

                if (categoryId == 10 &&
                    int.TryParse(item.Pic, NumberStyles.Integer, CultureInfo.InvariantCulture, out int picId) &&
                    picId > 0 &&
                    !FightPropsByPic.ContainsKey(picId))
                {
                    FightPropsByPic[picId] = new FightPropTemplate
                    {
                        Pic = picId,
                        Property1 = item.Property1,
                        Property2 = item.Property2,
                        Property3 = item.Property3,
                        Property4 = item.Property4,
                        Property5 = item.Property5,
                        Property6 = item.Property6,
                        Property7 = item.Property7,
                        Property8 = item.Property8
                    };
                }
            }
        }

        void LoadLevels(ResLoader loader)
        {
            if (!TryTable(loader, "Request/levellist.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int grade = Int(row, "Grade");
                if (grade <= 0)
                {
                    continue;
                }

                Levels.Add(new LevelGrade
                {
                    Grade = grade,
                    Gp = Int(row, "GP"),
                    Blood = Int(row, "Blood")
                });
            }

            Levels.Sort((a, b) => a.Grade.CompareTo(b.Grade));
        }

        void LoadShop(ResLoader loader)
        {
            if (!TryTable(loader, "Request/shopitemlist_out.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/ShopItemList.xml", out table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                Shop.Add(new ShopOffer
                {
                    Id = Int(row, "ID"),
                    ShopId = Int(row, "ShopID"),
                    TemplateId = Int(row, "TemplateID"),
                    AUnit = Int(row, "AUnit"),
                    APrice1 = Int(row, "APrice1"),
                    AValue1 = Int(row, "AValue1"),
                    CanBuy = Bool(row, "CanBuy"),
                    LimitGrade = Int(row, "LimitGrade")
                });
            }
        }

        void LoadQuests(ResLoader loader)
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
        }

        void LoadMaps(ResLoader loader)
        {
            if (!TryTable(loader, "Request/LoadMapsItems.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                if (id == 0)
                {
                    continue;
                }

                Maps[id] = new MapInfo
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Description = Str(row, "Description"),
                    ForegroundWidth = Int(row, "ForegroundWidth"),
                    ForegroundHeight = Int(row, "ForegroundHeight"),
                    Type = Int(row, "Type"),
                    HasCollision = loader.Exists(GamePaths.MapCollision(id)),
                    HasArt = loader.Exists(ResLoader.Foreground(id))
                };
            }
        }

        void LoadBalls(ResLoader loader)
        {
            if (!TryTable(loader, "Request/BallList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                Balls[id] = new BallPhysics
                {
                    Id = id,
                    Power = Float(row, "Power"),
                    Radii = Int(row, "Radii"),
                    Crater = Int(row, "Crater"),
                    Wind = Float(row, "Wind"),
                    Weight = Float(row, "Weight"),
                    Mass = Float(row, "Mass"),
                    FlyingPartical = Int(row, "FlyingPartical"),
                    BombPartical = Int(row, "BombPartical"),
                    Amount = Mathf.Max(1, Int(row, "Amount")),
                    BombType = Int(row, "BombType")
                };
            }
        }

        void LoadBombs(ResLoader loader)
        {
            if (!TryTable(loader, "Request/bombconfig.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var kv in BombTable.Load(table))
            {
                Bombs[kv.Key] = kv.Value;
            }
        }

        void LoadNpcs(ResLoader loader)
        {
            if (!TryTable(loader, "Request/NPCInfoList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                Npcs[id] = new NpcInfo
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Level = Int(row, "Level"),
                    Blood = Int(row, "Blood"),
                    Attack = Int(row, "Attack"),
                    Defence = Int(row, "Defence"),
                    Agility = Int(row, "Agility"),
                    Lucky = Int(row, "Lucky"),
                    BaseDamage = Int(row, "BaseDamage"),
                    BaseGuard = Int(row, "BaseGuard"),
                    ModelId = Str(row, "ModelID"),
                    ResourcesPath = Str(row, "ResourcesPath"),
                    Experience = Int(row, "Experience"),
                    DropId = Int(row, "DropId")
                };
            }
        }

        void LoadPets(ResLoader loader)
        {
            if (!TryTable(loader, "Request/pettemplateinfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "TemplateID");
                Pets[id] = new PetInfo
                {
                    TemplateId = id,
                    Name = Str(row, "Name"),
                    Pic = Str(row, "Pic"),
                    KindId = Int(row, "KindID"),
                    StarLevel = Mathf.Max(1, Int(row, "StarLevel")),
                    Mp = Mathf.Max(1, Int(row, "MP")),
                    Attack = Int(row, "HighAttack") / 10,
                    Defence = Int(row, "HighDefence") / 10,
                    Blood = Int(row, "HighBlood") / 5,
                    Agility = Int(row, "HighAgility") / 10,
                    Luck = Int(row, "HighLuck") / 10
                };
            }
        }

        void LoadPetSkills(ResLoader loader)
        {
            if (!TryTable(loader, "Request/petskillinfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                if (id <= 0)
                {
                    continue;
                }

                string elements = Str(row, "ElementIDs");
                int[] elementIds = System.Array.Empty<int>();
                if (!string.IsNullOrEmpty(elements))
                {
                    string[] parts = elements.Split(',');
                    var list = new List<int>(parts.Length);
                    foreach (string part in parts)
                    {
                        if (int.TryParse(part.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int eid) && eid > 0)
                        {
                            list.Add(eid);
                        }
                    }

                    elementIds = list.ToArray();
                }

                string desc = Str(row, "Description");
                int ballType = Int(row, "BallType");
                int probability = Int(row, "Probability");
                int pic = Int(row, "Pic");
                PetSkills[id] = new PetSkillInfo
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Pic = pic,
                    BallType = ballType,
                    Probability = probability,
                    ElementIds = elementIds,
                    Description = desc,
                    DamagePercent = ballType == 2 ? ParseHealPercent(desc) : ParsePetDamagePercent(desc),
                    NewBallId = Int(row, "NewBallID"),
                    CostMp = Int(row, "CostMP"),
                    ColdDown = Int(row, "ColdDown")
                };
                IndexPetSkillByPic(pic, id, ballType, probability);
            }

            FinalizePicSkillGroups();
        }

        void LoadPetSkillElements(ResLoader loader)
        {
            if (!TryTable(loader, "Request/petskillelementinfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                if (id <= 0)
                {
                    continue;
                }

                PetSkillElements[id] = new PetSkillElementInfo
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Description = Str(row, "Description"),
                    EffectPic = Str(row, "EffectPic")
                };
            }
        }

        void LoadCards(ResLoader loader)
        {
            if (!TryTable(loader, "Request/cardtemplateinfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                Cards.Add(new CardInfo
                {
                    Id = Int(row, "ID"),
                    CardId = Int(row, "CardID"),
                    AddAttack = Int(row, "AddAttack"),
                    AddDefend = Int(row, "AddDefend"),
                    AddAgility = Int(row, "AddAgility"),
                    AddLucky = Int(row, "AddLucky"),
                    AddDamage = Int(row, "AddDamage"),
                    AddGuard = Int(row, "AddGuard")
                });
            }
        }

        void LoadTitles(ResLoader loader)
        {
            if (!TryTable(loader, "Request/newtitleinfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                Titles[id] = new TitleInfo
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Pic = Str(row, "Pic"),
                    Att = Int(row, "Att"),
                    Def = Int(row, "Def"),
                    Agi = Int(row, "Agi"),
                    Luck = Int(row, "Luck")
                };
            }
        }

        void LoadTotems(ResLoader loader)
        {
            if (!TryTable(loader, "Request/toteminfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                Totems[id] = new TotemInfo
                {
                    Id = id,
                    AddAttack = Int(row, "AddAttack"),
                    AddDefence = Int(row, "AddDefence"),
                    AddAgility = Int(row, "AddAgility"),
                    AddLuck = Int(row, "AddLuck"),
                    AddBlood = Int(row, "AddBlood"),
                    AddDamage = Int(row, "AddDamage"),
                    AddGuard = Int(row, "AddGuard"),
                    ConsumeHonor = Int(row, "ConsumeHonor")
                };
            }
        }

        void LoadMounts(ResLoader loader)
        {
            if (!TryTable(loader, "Request/mounttemplateOUT.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/mounttemplate.xml", out table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int g = Int(row, "Grade");
                Mounts[g] = new MountGrade
                {
                    Grade = g,
                    Experience = Int(row, "Experience"),
                    AddBlood = Int(row, "AddBlood"),
                    AddDamage = Int(row, "AddDamage"),
                    AddGuard = Int(row, "AddGuard"),
                    MagicAttack = Int(row, "MagicAttack")
                };
            }
        }

        void LoadLottery(ResLoader loader)
        {
            if (!TryTable(loader, "Request/newlotteryitem.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                Lottery.Add(new LotteryDrop
                {
                    Id = Int(row, "ID"),
                    TemplateId = Int(row, "template"),
                    Count = Mathf.Max(1, Int(row, "count")),
                    Type = Int(row, "type")
                });
            }
        }

        void LoadVip(ResLoader loader)
        {
            if (!TryTable(loader, "Request/VipStoreList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                VipShop.Add(new ShopOffer
                {
                    Id = Int(row, "ID"),
                    TemplateId = Int(row, "GoodsID"),
                    AValue1 = Int(row, "Price"),
                    APrice1 = -2,
                    CanBuy = true
                });
            }
        }

        void LoadPve(ResLoader loader)
        {
            if (!TryTable(loader, "Request/LoadPVEItems.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/loadpveitems1.xml", out table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                if (id == 0)
                {
                    continue;
                }

                Pve.Add(new PveMission
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Description = Str(row, "Description"),
                    LevelLimits = Int(row, "LevelLimits"),
                    MinLv = Int(row, "MinLv"),
                    Type = Int(row, "Type")
                });
            }
        }

        void LoadSpirits(ResLoader loader)
        {
            if (!TryTable(loader, "Request/SpiritInfoList.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int level = Int(row, "Level");
                var info = new SpiritInfo
                {
                    Level = level,
                    AttackAdd = Int(row, "AttackAdd"),
                    DefendAdd = Int(row, "DefendAdd"),
                    AgilityAdd = Int(row, "AgilityAdd"),
                    LuckAdd = Int(row, "LuckAdd"),
                    ReferenceCost = Int(row, "RefrenceValue"),
                    CategoryId = Int(row, "CategoryId"),
                    BagPlace = Int(row, "BagPlace")
                };
                if (!Spirits.TryGetValue(level, out SpiritInfo prev) ||
                    info.AttackAdd + info.DefendAdd > prev.AttackAdd + prev.DefendAdd)
                {
                    Spirits[level] = info;
                }
            }
        }

        void LoadFightSpirits(ResLoader loader)
        {
            if (!TryTable(loader, "Request/fightspirittemplatelist.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int spiritId = Int(row, "FightSpiritID");
                int level = Int(row, "Level");
                FightSpirits[FightSpiritKey(spiritId, level)] = new FightSpiritTemplate
                {
                    SpiritId = spiritId,
                    Level = level,
                    Icon = Str(row, "FightSpiritIcon"),
                    Exp = Int(row, "Exp"),
                    Attack = Int(row, "Attack"),
                    Defence = Int(row, "Defence"),
                    Agility = Int(row, "Agility"),
                    Lucky = Int(row, "Lucky"),
                    Blood = Int(row, "Blood")
                };
            }
        }

        void LoadMagicStones(ResLoader loader)
        {
            if (!TryTable(loader, "Request/magicstonetemplate.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int templateId = Int(row, "TemplateID");
                int level = Int(row, "Level");
                MagicStones[MagicStoneKey(templateId, level)] = new MagicStoneTemplate
                {
                    TemplateId = templateId,
                    Level = level,
                    Exp = Int(row, "Exp"),
                    Attack = Int(row, "Attack"),
                    Defence = Int(row, "Defence"),
                    Agility = Int(row, "Agility"),
                    Luck = Int(row, "Luck"),
                    MagicAttack = Int(row, "MagicAttack"),
                    MagicDefence = Int(row, "MagicDefence")
                };
            }
        }

        void LoadMagicFusions(ResLoader loader)
        {
            if (!TryTable(loader, "Request/magicfusiondata.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                MagicFusions.Add(new MagicFusionRecipe
                {
                    Id = Int(row, "ID"),
                    ItemId = Int(row, "ItemID"),
                    Type = Int(row, "Type"),
                    NeedGold = Int(row, "NeedGold"),
                    NeedKey = Int(row, "NeedKey"),
                    GetKeys = Int(row, "GetKeys")
                });
            }
        }

        void LoadTeamDungeonShop(ResLoader loader)
        {
            if (!TryTable(loader, "Request/battleteamshopitemlist.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                TeamDungeonShop.Add(new TeamDungeonShopEntry
                {
                    Id = Int(row, "ID"),
                    ShopType = Int(row, "ShopType"),
                    NeedLevel = Int(row, "NeedLevel"),
                    Price = Int(row, "Price"),
                    Condition = Int(row, "Condition"),
                    Value = Int(row, "Value")
                });
            }
        }

        void LoadStoryCopy(ResLoader loader)
        {
            if (TryTable(loader, "Request/TS_StoryCopyChapterTemplate.xml", out XmlResultTable chapters))
                foreach (var row in chapters.Rows)
                    StoryCopyChapters.Add(new StoryCopyChapter
                    {
                        Chapter = Int(row, "Chapter"), Name = Str(row, "Name"), SectionCount = Int(row, "SectionCount"),
                        AllStarAward = Str(row, "AllStarAward"), QuestBoxAward = Str(row, "QuestBoxAward"),
                        QuestMaxScore = Int(row, "QuestMaxScore"), Detail = Str(row, "Detail")
                    });
            if (TryTable(loader, "Request/TS_StoryCopySectionTemplate.xml", out XmlResultTable sections))
                foreach (var row in sections.Rows)
                    StoryCopySections.Add(new StoryCopySection
                    {
                        Chapter = Int(row, "Chapter"), Section = Int(row, "Section"), Name = Str(row, "Name"),
                        Detail = Str(row, "Detail"), MissionId = Int(row, "MissionID"), MapId = Int(row, "MapID"),
                        PlayLimit = Int(row, "PlayLimit"), ThreeStarAward = Str(row, "ThreeStarAward"),
                        SweepReward = Str(row, "SweepReward")
                    });
            if (TryTable(loader, "Request/TS_StoryCopyQuest.xml", out XmlResultTable quests))
                foreach (var row in quests.Rows)
                    StoryCopyQuests.Add(new StoryCopyQuest
                    {
                        QuestId = Int(row, "QuestID"), ChapterId = Int(row, "ChapterID"),
                        ConditionType = Int(row, "ConditionType"), Name = Str(row, "Name"),
                        FinishCount = Int(row, "FinishCount"), QuestAward = Str(row, "QuestAward"),
                        QuestScore = Int(row, "QuestScore"), Detail = Str(row, "Detail")
                    });
            if (TryTable(loader, "Request/TS_StoryCopyLevelUp.xml", out XmlResultTable levelUps))
                foreach (var row in levelUps.Rows)
                    StoryCopyLevelUps.Add(new StoryCopyLevelUp
                    {
                        Chapter = Int(row, "Chapter"), PicId = Int(row, "PicID"), PicLevel = Int(row, "PicLevel"),
                        Name = Str(row, "Name"), PicSoulCount = Int(row, "PicSoulCount"),
                        TemplateId = Int(row, "TemplateID"), TemplateCount = Int(row, "TemplateCount")
                    });
        }

        void LoadWarriorFam(ResLoader loader)
        {
            if (TryTable(loader, "Request/ts_warriorfamfightconfig.xml", out XmlResultTable fights))
                foreach (var row in fights.Rows)
                    WarriorFamFights.Add(new WarriorFamFightConfig
                    {
                        HardType = Int(row, "HardType"), Level = Int(row, "Level"), MissionId = Int(row, "MissionID"),
                        FirstRewards = Str(row, "FirstRewards"), Rewards = Str(row, "Rewards"), Rank = Int(row, "Rank")
                    });
            LoadWarriorFamRankFile(loader, "Request/warriorfamranklist.xml", WarriorFamRanks);
            LoadWarriorFamRankFile(loader, "Request/warriorhighfamranklist.xml", WarriorHighFamRanks);
        }

        void LoadWarriorFamRankFile(ResLoader loader, string path, List<WarriorFamRankEntry> target)
        {
            if (!TryTable(loader, path, out XmlResultTable table)) return;
            foreach (var row in table.Rows)
                target.Add(new WarriorFamRankEntry
                {
                    Rank = Int(row, "Rank"), Nick = Str(row, "NickName"), Level = Int(row, "Level"),
                    HardType = Int(row, "HardType"), FightPower = Int(row, "FightPower")
                });
        }

        void LoadCampWar(ResLoader loader)
        {
            if (!TryTable(loader, "Request/campwaritems.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                CampWarRewards.Add(new CampWarReward
                {
                    Id = Int(row, "ID"),
                    MinRank = Int(row, "MinRank"),
                    MaxRank = Int(row, "MaxRank"),
                    ItemId = Int(row, "ItemID"),
                    Count = Mathf.Max(1, Int(row, "Count"))
                });
            }
        }

        void LoadNecklace(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TS_NecklaceCasting.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int level = Int(row, "Level");
                NecklaceLevels[level] = new NecklaceCastingLevel
                {
                    Level = level,
                    NeedItemCount1 = Int(row, "NeedItemCount1"),
                    NeedItemCount2 = Int(row, "NeedItemCount2"),
                    Hp = Int(row, "HP"),
                    Toughness = Int(row, "Toughness"),
                    AvoidInjury = Int(row, "AvoidInjury"),
                    TricRevolt = Int(row, "TricRevolt"),
                    Guardian = Int(row, "Guardian")
                };
            }
        }

        void LoadEmblems(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TS_Emblem.xml", out XmlResultTable table)) return;
            foreach (var row in table.Rows)
            {
                EmblemList.Add(new EmblemTemplate
                {
                    Id = Int(row, "ID"), TemplateId = Int(row, "TemplateId"), Types = Int(row, "Types"), Profile = Int(row, "Profile"),
                    MainType = Int(row, "MainType"), SubCount = Int(row, "SubCount"), MainValue = Str(row, "MainValue"), SubValue = Str(row, "SubValue"),
                    NeedItem1 = Int(row, "NeedItem1"), ItemCount1 = Int(row, "ItemCount1"), NeedItem2 = Int(row, "NeedItem2"), ItemCount2 = Int(row, "ItemCount2"),
                    NeedItem3 = Int(row, "NeedItem3"), ItemCount3 = Int(row, "ItemCount3"), NeedItem4 = Int(row, "NeedItem4"), ItemCount4 = Int(row, "ItemCount4")
                });
            }
        }

        void LoadSoulStamps(ResLoader loader)
        {
            if (TryTable(loader, "Request/TS_SoulStampTemplate.xml", out XmlResultTable stamps))
            {
                foreach (var row in stamps.Rows)
                {
                    int tempId = Int(row, "TempID");
                    string proTypesRaw = Str(row, "ProTypes");
                    int[] proTypes = Array.Empty<int>();
                    if (!string.IsNullOrEmpty(proTypesRaw))
                    {
                        string[] parts = proTypesRaw.Split(',');
                        proTypes = new int[parts.Length];
                        for (int i = 0; i < parts.Length; i++) int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out proTypes[i]);
                    }
                    SoulStampTemplates[tempId] = new SoulStampTemplate
                    {
                        TempId = tempId, Type = Int(row, "Type"), Quality = Int(row, "Quality"), ProTypes = proTypes,
                        SkillId = Int(row, "SkillID"), SubSkillId = Int(row, "SubSkillID"), UpSkillId = Str(row, "UpSkillID"), UpSubSkillId = Str(row, "UpSubSkillID")
                    };
                }
            }
            if (TryTable(loader, "Request/TS_SoulStampComposeTemplate.xml", out XmlResultTable composeTable))
            {
                foreach (var row in composeTable.Rows)
                {
                    int quality = Int(row, "Quality");
                    SoulStampCompose[quality] = new SoulStampComposeTemplate { TemplateId = Int(row, "TemplateID"), Quality = quality, ComposeCost = Int(row, "ComposeCost"), ComposePreCost = Int(row, "ComposePreCost") };
                }
            }
            if (TryTable(loader, "Request/TS_SoulStampProTemplate.xml", out XmlResultTable proTable))
            {
                foreach (var row in proTable.Rows)
                {
                    int tempId = Int(row, "TempID"), proType = Int(row, "ProType");
                    long key = SoulStampProKey(tempId, proType);
                    if (!_soulStampProBands.TryGetValue(key, out List<SoulStampProBand> bands)) { bands = new List<SoulStampProBand>(); _soulStampProBands[key] = bands; }
                    foreach (string part in Str(row, "ProValueLimit").Split('|'))
                    {
                        string[] seg = part.Split(',');
                        if (seg.Length < 2) continue;
                        string[] range = seg[0].Split('-');
                        if (range.Length < 2) continue;
                        bands.Add(new SoulStampProBand { Min = int.TryParse(range[0], out int min) ? min : 0, Max = int.TryParse(range[1], out int max) ? max : 0, Weight = int.TryParse(seg[1], out int weight) ? weight : 0 });
                    }
                }
            }
            if (TryTable(loader, "Request/TS_SoulRefine_Ratio.xml", out XmlResultTable refineTable))
            {
                foreach (var row in refineTable.Rows)
                {
                    int index = Int(row, "Index"), grade = Int(row, "Grade");
                    _soulRefineRatios[SoulRefineKey(index, grade)] = new SoulRefineRatio
                    {
                        RatioId = Int(row, "RatioId"), Grade = grade, Index = index, Rate = Int(row, "Rate"), Ratio = Int(row, "Ratio"),
                        NeedItem1 = Int(row, "NeedItem1"), ItemCount1 = Int(row, "ItemCount1"), NeedItem2 = Int(row, "NeedItem2"), ItemCount2 = Int(row, "ItemCount2"),
                        NeedItem3 = Int(row, "NeedItem3"), ItemCount3 = Int(row, "ItemCount3"), NeedItem4 = Int(row, "NeedItem4"), ItemCount4 = Int(row, "ItemCount4")
                    };
                }
            }
        }

        void LoadMagicCloths(ResLoader loader)
        {
            if (!TryTable(loader, "Request/magicclothlist.xml", out XmlResultTable table)) return;
            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID"); if (id <= 0) continue;
                var info = new MagicClothInfo
                {
                    Id = id, Name = Str(row, "Name"), HasShow = Int(row, "HasShow"), Type = Int(row, "Type"),
                    HeadId = Int(row, "HeadID"), HairId = Int(row, "HairID"), EffId = Int(row, "EffID"),
                    ClothId = Int(row, "ClothID"), GlassId = Int(row, "GlassID"), FaceId = Int(row, "FaceID"),
                    WingId = Int(row, "WingID"), SuitsId = Int(row, "SuitsID"), Sex = Int(row, "Sex")
                };
                MagicCloths[id] = info; MagicClothList.Add(info);
            }
        }

        void LoadClothGroups(ResLoader loader)
        {
            if (!TryTable(loader, "Request/clothgrouptemplateinfo.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/clothgrouptemplateinfo1.xml", out table) &&
                !TryTable(loader, "Request/clothgrouptemplateinfo2.xml", out table)) return;
            foreach (var row in table.Rows)
            {
                int groupId = Int(row, "ID"); if (groupId <= 0) continue;
                var part = new ClothGroupPart
                {
                    GroupId = groupId, TemplateId = Int(row, "TemplateID"), Sex = Int(row, "Sex"),
                    Description = Int(row, "Description"), Cost = Int(row, "Cost"), Type = Int(row, "Type"),
                    OtherTemplateId = Int(row, "OtherTemplateID")
                };
                if (!_clothGroupParts.TryGetValue(groupId, out List<ClothGroupPart> list))
                { list = new List<ClothGroupPart>(); _clothGroupParts[groupId] = list; }
                list.Add(part);
            }
        }

        void LoadClothProperties(ResLoader loader)
        {
            if (!TryTable(loader, "Request/clothpropertytemplateinfo.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/clothpropertytemplateinfo1.xml", out table) &&
                !TryTable(loader, "Request/clothpropertytemplateinfo2.xml", out table)) return;
            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID"); if (id <= 0) continue;
                ClothProperties[id] = new ClothPropertyInfo
                {
                    Id = id, Sex = Int(row, "Sex"), Name = Str(row, "Name"), Attack = Int(row, "Attack"),
                    Defend = Int(row, "Defend"), Agility = Int(row, "Agility"), Luck = Int(row, "Luck"),
                    Blood = Int(row, "Blood"), Damage = Int(row, "Damage"), Guard = Int(row, "Guard"),
                    Cost = Int(row, "Cost"), Type = Int(row, "Type")
                };
            }
        }

        void LoadHonorSystem(ResLoader loader)
        {
            if (!TryTable(loader, "Request/ts_honorsystem_template.xml", out XmlResultTable table)) return;
            foreach (var row in table.Rows)
            {
                int level = Int(row, "Level"); if (level <= 0) continue;
                HonorSystemLevels[level] = new HonorSystemLevelInfo
                {
                    Level = level, Name = Str(row, "Name"), Exp = Int(row, "Exp"), Blood = Int(row, "Blood"),
                    StrengthRate = Int(row, "StrengthRate"), AdvanceRate = Int(row, "AdvanceRate"),
                    GoldRate = Int(row, "GoldRate"), SpiritRate = Int(row, "SpiritRate"),
                    FusionRate = Int(row, "FusionRate"), LevelGift = Int(row, "LevelGift")
                };
            }
        }

        void LoadTotemHonor(ResLoader loader)
        {
            if (!TryTable(loader, "Request/totemhonortemplate.xml", out XmlResultTable table)) return;
            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID"); if (id <= 0) continue;
                TotemHonorEntries[id] = new TotemHonorEntry
                {
                    Id = id, Type = Int(row, "Type"), NeedMoney = Int(row, "NeedMoney"), AddHonor = Int(row, "AddHonor")
                };
            }
        }

        void LoadDevilTreas(ResLoader loader)
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

        void LoadActivityConfig(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TS_ActivityConfig.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int num = Int(row, "Num");
                ActivityConfigs[num] = new ActivityConfigEntry
                {
                    Num = num,
                    Name = Str(row, "Name"),
                    Params1 = Str(row, "Params1"),
                    Params2 = Str(row, "Params2"),
                    Params3 = Str(row, "Params3"),
                    Params4 = Str(row, "Params4"),
                    Params5 = Str(row, "Params5"),
                    RankAreaAward = Str(row, "RankAreaAward")
                };
            }
        }

        void LoadFirstPayShop(ResLoader loader)
        {
            if (!TryTable(loader, "Request/ts_firstpayshoptemp.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                FirstPayShop.Add(new FirstPayShopItem
                {
                    Id = Int(row, "ID"),
                    TemplateId = Int(row, "TemplateId"),
                    ItemTempId = Int(row, "ItemTempId"),
                    ItemTempCount = Mathf.Max(1, Int(row, "ItemTempCount")),
                    LimitBuyCount = Mathf.Max(1, Int(row, "LimitBuyCount")),
                    NeedGoldBeans = Int(row, "NeedGoldBeans"),
                    ShopType = Int(row, "shopType")
                });
            }
        }

        void LoadFirstCopy(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TS_FirstCopy.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int copyId = Int(row, "CopyID");
                if (copyId <= 0)
                {
                    copyId = Int(row, "ID");
                }

                if (copyId <= 0)
                {
                    continue;
                }

                if (!ActivityConfigs.ContainsKey(8))
                {
                    ActivityConfigs[8] = new ActivityConfigEntry { Num = 8, Name = "首充" };
                }
            }
        }

        void LoadElves(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TS_ElfTemplate.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "TemplateId");
                Elves[id] = new ElfInfo
                {
                    TemplateId = id,
                    Name = Str(row, "TemplateName"),
                    StarLevel = Int(row, "StarLevel"),
                    AttackHint = FirstInt(Str(row, "AtckRandoms")),
                    HpHint = FirstInt(Str(row, "HPRandoms"))
                };
            }
        }

        void LoadFarm(ResLoader loader)
        {
            if (!TryTable(loader, "Request/foodcomposelist.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                Farm.Add(new FarmRecipe
                {
                    FoodId = Int(row, "FoodID"),
                    VegetableId = Int(row, "VegetableID"),
                    NeedCount = Mathf.Max(1, Int(row, "NeedCount"))
                });
            }
        }

        void LoadStrengthen(ResLoader loader)
        {
            if (!TryTable(loader, "Request/ItemStrengthenList.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/ItemStrengthenList_out.xml", out table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                StrengthenRock[Int(row, "StrengthenLevel")] = Int(row, "Rock");
            }
        }

        void LoadSignIn(ResLoader loader)
        {
            if (!TryTable(loader, "Request/TS_EveryDaySignIn.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                SignIn.Add(new SignReward
                {
                    Day = Int(row, "ID"),
                    TemplateId = Int(row, "TemplateID"),
                    Count = Int(row, "Count")
                });
            }

            SignIn.Sort((a, b) => a.Day.CompareTo(b.Day));
        }

        void LoadGodCards(ResLoader loader)
        {
            if (!TryTable(loader, "Request/godcardlist.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "ID");
                GodCards[id] = new GodCardInfo
                {
                    Id = id,
                    Name = Str(row, "Name"),
                    Pic = Str(row, "Pic"),
                    Composition = Int(row, "Composition"),
                    Decompose = Int(row, "Decompose"),
                    Level = Int(row, "Level")
                };
            }
        }

        void LoadEngrave(ResLoader loader)
        {
            if (TryTable(loader, "Request/engravesetinfo.xml", out XmlResultTable sets))
            {
                foreach (var row in sets.Rows)
                {
                    int setId = Int(row, "SetId");
                    EngraveSets[setId] = new EngraveSetInfo
                    {
                        SetId = setId,
                        Name = Str(row, "Name"),
                        HelpExplain = Str(row, "HelpExplain")
                    };
                }
            }

            if (!TryTable(loader, "Request/engravesetelementinfo.xml", out XmlResultTable elems))
            {
                return;
            }

            foreach (var row in elems.Rows)
            {
                EngraveElements.Add(new EngraveElementInfo
                {
                    Id = Int(row, "Id"),
                    Name = Str(row, "Name"),
                    SetId = Int(row, "SetId"),
                    Demand = Int(row, "Demand"),
                    Attribute = Str(row, "Attribute"),
                    Quality = Int(row, "Quality")
                });
            }
        }

        void LoadStocks(ResLoader loader)
        {
            if (!TryTable(loader, "Request/StockTemplateInfo.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int id = Int(row, "StockID");
                Stocks[id] = new StockInfo
                {
                    StockId = id,
                    StockName = Str(row, "StockName"),
                    BasePrice = Int(row, "BasePrice"),
                    FlowCoeffcient = Int(row, "FlowCoeffcient")
                };
            }
        }

        public int StockQuote(StockInfo stock)
        {
            if (stock == null)
            {
                return 0;
            }

            int day = DateTime.UtcNow.DayOfYear;
            int swing = stock.FlowCoeffcient > 0
                ? (day * stock.FlowCoeffcient / 10000) % Mathf.Max(1, stock.BasePrice / 3)
                : 0;
            return Mathf.Max(1, stock.BasePrice + swing - stock.BasePrice / 4);
        }

        public void ApplyEngraveSetBonus(int setId, ref int atk, ref int def, ref int agi, ref int luk, ref int hp, ref int baseDmg, ref int baseGuard)
        {
            if (setId <= 0)
            {
                return;
            }

            EngraveElementInfo best = null;
            foreach (EngraveElementInfo el in EngraveElements)
            {
                if (el.SetId != setId || el.Demand != 2 || string.IsNullOrEmpty(el.Attribute) || el.Attribute == "0")
                {
                    continue;
                }

                if (best == null || el.Quality > best.Quality)
                {
                    best = el;
                }
            }

            if (best == null)
            {
                return;
            }

            ApplyAttributeString(best.Attribute, ref atk, ref def, ref agi, ref luk, ref hp, ref baseDmg, ref baseGuard);
        }

        public void ApplyGodCardBonus(GodCardInfo card, ref int atk, ref int def, ref int agi, ref int luk, ref int hp)
        {
            if (card == null)
            {
                return;
            }

            int bonus = (card.Level + 1) * 5;
            atk += bonus;
            def += bonus;
            agi += bonus;
            luk += bonus;
            hp += bonus * 20;
        }

        static void ApplyAttributeString(string raw, ref int atk, ref int def, ref int agi, ref int luk, ref int hp, ref int baseDmg, ref int baseGuard)
        {
            foreach (string part in raw.Split(','))
            {
                string[] seg = part.Split('|');
                if (seg.Length < 2 || !int.TryParse(seg[0], out int type) || !int.TryParse(seg[1], out int val))
                {
                    continue;
                }

                switch (type)
                {
                    case 31: atk += val; break;
                    case 33: agi += val; break;
                    case 35: baseGuard += val; break;
                    case 37: hp += val; break;
                    case 101: atk += val / 2; break;
                    case 102: def += val / 2; break;
                    default: def += val / 4; break;
                }
            }
        }

        void LoadServerConfig(ResLoader loader)
        {
            if (!TryTable(loader, "Request/ServerConfig.xml", out XmlResultTable table) &&
                !TryTable(loader, "Request/serverconfigOUT.xml", out table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                string name = Str(row, "Name");
                string value = Str(row, "Value");
                if (!string.IsNullOrEmpty(name))
                {
                    ServerConfig[name] = value;
                }
            }
        }

        void LoadCelebLists(ResLoader loader)
        {
            LoadCelebFile(loader, "Request/CelebByDayGPList.xml", CelebGpDay, "AddDayGP");
            LoadCelebFile(loader, "Request/CelebByDayFightPowerList.xml", CelebFightPowerDay, "FightPower");
            LoadCelebFile(loader, "Request/CelebByDayOfferList.xml", CelebOfferDay, "AddDayOffer");
            LoadCelebFile(loader, "Request/areacelebbydayfightpowerlist.xml", CelebAreaFightPower, "FightPower");
        }

        void LoadCelebFile(ResLoader loader, string path, List<CelebEntry> target, string sortKey)
        {
            if (!TryTable(loader, path, out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                int rank = Int(row, "ID");
                if (rank == 0)
                {
                    rank = target.Count + 1;
                }

                target.Add(new CelebEntry
                {
                    Rank = rank,
                    Nick = Str(row, "NickName"),
                    Grade = Int(row, "Grade"),
                    Gp = Int(row, sortKey == "AddDayOffer" ? "AddDayOffer" : sortKey == "FightPower" ? "FightPower" : "AddDayGP"),
                    FightPower = Int(row, "FightPower"),
                    Offer = Int(row, "AddDayOffer"),
                    VipLevel = Int(row, "VIPLevel"),
                    ConsortiaName = Str(row, "ConsortiaName"),
                    WinCount = Int(row, "WinCount"),
                    TotalCount = Int(row, "TotalCount")
                });
            }
        }

        void LoadCharacterDefine(ResLoader loader)
        {
#if GUNMOBILE_STANDALONE
            return;
#else
            try
            {
                byte[] bytes = loader.ReadBytes("Flash/characterdefine.xml");
                if (bytes == null || bytes.Length == 0)
                {
                    return;
                }

                CharacterDef = CharacterDefine.Load(XDocument.Parse(System.Text.Encoding.UTF8.GetString(bytes)));
            }
            catch (Exception e)
            {
                Debug.LogWarning("GameDatabase characterdefine: " + e.Message);
            }
#endif
        }

        public List<CelebEntry> CelebForType(string type)
        {
            if (string.Equals(type, "fight", StringComparison.OrdinalIgnoreCase))
            {
                return CelebFightPowerDay;
            }

            if (string.Equals(type, "offer", StringComparison.OrdinalIgnoreCase))
            {
                return CelebOfferDay;
            }

            return CelebGpDay;
        }

        void LoadFightLabDrops(ResLoader loader)
        {
            if (!TryTable(loader, "Request/fightlabdropitemlist.xml", out XmlResultTable table))
            {
                return;
            }

            foreach (var row in table.Rows)
            {
                FightLabDrops.Add(new FightLabDrop
                {
                    LabId = Int(row, "ID"),
                    Easy = Int(row, "Easy"),
                    AwardItem = Int(row, "AwardItem"),
                    Count = Int(row, "Count")
                });
            }
        }

        static int[] ParseCsvInts(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                return Array.Empty<int>();
            }

            string[] parts = csv.Split(',');
            var ids = new List<int>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n))
                {
                    ids.Add(n);
                }
            }

            return ids.ToArray();
        }

        static int FirstInt(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                return 0;
            }

            int comma = csv.IndexOf(',');
            string head = comma < 0 ? csv : csv.Substring(0, comma);
            int.TryParse(head.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n);
            return n;
        }

        static bool TryTable(ResLoader loader, string path, out XmlResultTable table)
        {
            table = null;
            if (!loader.TryReadBytes(path, out byte[] bytes))
            {
                return false;
            }

            try
            {
                table = XmlResultTable.LoadBytes(bytes);
                return table != null;
            }
            catch (Exception e)
            {
                Debug.LogWarning("GameDatabase " + path + ": " + e.Message);
                return false;
            }
        }

        public static int Int(IReadOnlyDictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out string raw) || string.IsNullOrEmpty(raw))
            {
                return 0;
            }

            int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n);
            return n;
        }

        public static float Float(IReadOnlyDictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out string raw) || string.IsNullOrEmpty(raw))
            {
                return 0f;
            }

            float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float n);
            return n;
        }

        public static bool Bool(IReadOnlyDictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out string raw) || string.IsNullOrEmpty(raw))
            {
                return false;
            }

            return raw.Equals("true", StringComparison.OrdinalIgnoreCase) || raw == "1";
        }

        public static string Str(IReadOnlyDictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out string raw) ? raw ?? "" : "";
        }
    }

    public static class MapCatalog
    {
        public static List<MapInfo> Playable(GameDatabase db)
        {
            var list = new List<MapInfo>();
            if (db == null)
            {
                return list;
            }

            foreach (MapInfo map in db.Maps.Values)
            {
                if (map.HasCollision)
                {
                    list.Add(map);
                }
            }

            list.Sort((a, b) => a.Id.CompareTo(b.Id));
            return list;
        }

        public static List<int> DiscoverCollisionIds(ResLoader loader)
        {
            var ids = new List<int>();
            foreach (string file in loader.ListFiles("Service/Road/map", "fore.map"))
            {
                string[] parts = file.Replace('\\', '/').Split('/');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    if (parts[i] == "map" && int.TryParse(parts[i + 1], out int id) && !ids.Contains(id))
                    {
                        ids.Add(id);
                    }
                }
            }

            ids.Sort();
            return ids;
        }
    }
}
