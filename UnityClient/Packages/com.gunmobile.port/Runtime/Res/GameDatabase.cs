using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using GunMobile.Core;
using GunMobile.Logic;
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

    public sealed class CardInfo
    {
        public int Id;
        public int CardId;
        public int AddAttack;
        public int AddDefend;
        public int AddAgility;
        public int AddLucky;
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
        public int ConsumeHonor;
    }

    public sealed class MountGrade
    {
        public int Grade;
        public int Experience;
        public int AddBlood;
        public int AddDamage;
        public int MagicAttack;
    }

    public sealed class LotteryDrop
    {
        public int Id;
        public int TemplateId;
        public int Count;
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
        public Dictionary<int, ElfInfo> Elves { get; } = new Dictionary<int, ElfInfo>();
        public List<FarmRecipe> Farm { get; } = new List<FarmRecipe>();
        public Dictionary<int, int> StrengthenRock { get; } = new Dictionary<int, int>();
        public List<SignReward> SignIn { get; } = new List<SignReward>();
        public Dictionary<string, string> ServerConfig { get; } = new Dictionary<string, string>();
        public List<FightLabDrop> FightLabDrops { get; } = new List<FightLabDrop>();
        public List<LevelGrade> Levels { get; } = new List<LevelGrade>();
        public Dictionary<int, FightPropTemplate> FightPropsByPic { get; } = new Dictionary<int, FightPropTemplate>();

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
            db.BuildKindPassiveSkillMap();
            db.LoadCards(loader);
            db.LoadTitles(loader);
            db.LoadTotems(loader);
            db.LoadMounts(loader);
            db.LoadLottery(loader);
            db.LoadVip(loader);
            db.LoadPve(loader);
            db.LoadSpirits(loader);
            db.LoadElves(loader);
            db.LoadFarm(loader);
            db.LoadStrengthen(loader);
            db.LoadSignIn(loader);
            db.LoadServerConfig(loader);
            db.LoadFightLabDrops(loader);
            db.LoadLevels(loader);
            Debug.Log($"GunMobile DB items={db.Items.Count} shop={db.Shop.Count} quests={db.Quests.Count} maps={db.Maps.Count} balls={db.Balls.Count} pets={db.Pets.Count} npcs={db.Npcs.Count} pve={db.Pve.Count} levels={db.Levels.Count} fightProps={db.FightPropsByPic.Count} cfg={db.ServerConfig.Count}");
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

        public int LotteryDrawCost(int count)
        {
            if (count >= 10)
            {
                return ConfigInt("NewLotteryOpenMoney", 100) * 10;
            }

            return ConfigInt("LotteryMoney", 100);
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
            return new LivingStats
            {
                Attack = atk,
                Defence = def,
                Agility = agi,
                Luck = luk,
                Hp = hp,
                MaxHp = hp,
                Team = 2
            };
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
            return ball != null && ball.BombType == 1;
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
                    AddLucky = Int(row, "AddLucky")
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
                    Count = Mathf.Max(1, Int(row, "count"))
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
                    ReferenceCost = Int(row, "RefrenceValue")
                };
                if (!Spirits.TryGetValue(level, out SpiritInfo prev) ||
                    info.AttackAdd + info.DefendAdd > prev.AttackAdd + prev.DefendAdd)
                {
                    Spirits[level] = info;
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
