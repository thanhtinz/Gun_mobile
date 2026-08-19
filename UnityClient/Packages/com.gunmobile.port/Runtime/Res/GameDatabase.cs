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
    }

    public sealed class PetInfo
    {
        public int TemplateId;
        public string Name = "";
        public int Attack;
        public int Defence;
        public int Blood;
        public int Agility;
        public int Luck;
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
            Debug.Log($"GunMobile DB items={db.Items.Count} shop={db.Shop.Count} quests={db.Quests.Count} maps={db.Maps.Count} balls={db.Balls.Count} pets={db.Pets.Count} npcs={db.Npcs.Count} pve={db.Pve.Count}");
            return db;
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

                Items[id] = new ItemTemplate
                {
                    TemplateId = id,
                    Name = Str(row, "Name"),
                    Description = Str(row, "Description"),
                    CategoryId = Int(row, "CategoryID"),
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
                    Level = Int(row, "Level")
                };
            }
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
                    Amount = Mathf.Max(1, Int(row, "Amount"))
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
                    Lucky = Int(row, "Lucky")
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
                    Attack = Int(row, "HighAttack") / 10,
                    Defence = Int(row, "HighDefence") / 10,
                    Blood = Int(row, "HighBlood") / 5,
                    Agility = Int(row, "HighAgility") / 10,
                    Luck = Int(row, "HighLuck") / 10
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
                    LuckAdd = Int(row, "LuckAdd")
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
