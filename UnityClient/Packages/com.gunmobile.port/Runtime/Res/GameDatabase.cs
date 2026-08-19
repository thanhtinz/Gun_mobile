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
            Debug.Log($"GunMobile DB items={db.Items.Count} shop={db.Shop.Count} quests={db.Quests.Count} maps={db.Maps.Count} balls={db.Balls.Count}");
            return db;
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
