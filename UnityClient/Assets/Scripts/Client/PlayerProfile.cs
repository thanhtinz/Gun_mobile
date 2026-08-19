using System;
using System.Collections.Generic;
using System.IO;
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
        public List<BagItem> Bag = new List<BagItem>();
        public List<int> AcceptedQuests = new List<int>();
        public List<int> CompletedQuests = new List<int>();

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
            return fresh;
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
            }

            if (EquipWeapon == 0)
            {
                EquipWeapon = 7001;
                WeaponId = 7001;
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
            if (db != null)
            {
                AddStats(db.GetItem(EquipHead), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipHair), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipFace), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipCloth), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipGlass), ref atk, ref def, ref agi, ref luk);
                AddStats(db.GetItem(EquipWeapon), ref atk, ref def, ref agi, ref luk);
            }

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
    }

    public sealed class ModuleDef
    {
        public string Id;
        public string Title;
        public string TablePath;
        public bool OpensBattle;

        public ModuleDef(string id, string title, string tablePath = null, bool opensBattle = false)
        {
            Id = id;
            Title = title;
            TablePath = tablePath;
            OpensBattle = opensBattle;
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
            new ModuleDef("dungeon", "副本", "Request/LoadMapsItems.xml", true),
            new ModuleDef("character", "角色"),
            new ModuleDef("shop", "商城", "Request/ShopItemList.xml"),
            new ModuleDef("bag", "背包 / 图鉴", "Request/TemplateAlllist.xml"),
            new ModuleDef("quest", "任务", "Request/QuestList.xml"),
            new ModuleDef("npc", "NPC", "Request/NPCInfoList.xml"),
            new ModuleDef("ball", "炮弹", "Request/BallList.xml"),
            new ModuleDef("bomb", "炸弹配置", "Request/bombconfig.xml"),
            new ModuleDef("pet", "宠物", "Request/petskillinfo.xml"),
            new ModuleDef("card", "卡片", "Request/cardtemplateinfo.xml"),
            new ModuleDef("title", "称号", "Request/newtitleinfo.xml"),
            new ModuleDef("totem", "图腾", "Request/toteminfo.xml"),
            new ModuleDef("horse", "坐骑", "Request/mounttemplateOUT.xml"),
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
        };
    }
}
