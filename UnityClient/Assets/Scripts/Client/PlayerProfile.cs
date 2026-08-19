using System.IO;
using UnityEngine;

namespace GunMobile.Client
{
    [System.Serializable]
    public sealed class PlayerProfile
    {
        public string Nick = "Player";
        public int Sex = 1;
        public int Level = 20;
        public int Gold = 100000;
        public int Gift = 5000;
        public int Attack = 120;
        public int Defence = 90;
        public int Agility = 80;
        public int Luck = 70;
        public int Hp = 1200;
        public int Win;
        public int Lose;
        public int MapId = 1056;

        public static string PathOnDisk => Path.Combine(Application.persistentDataPath, "player.json");

        public static PlayerProfile Load()
        {
            try
            {
                if (File.Exists(PathOnDisk))
                {
                    return JsonUtility.FromJson<PlayerProfile>(File.ReadAllText(PathOnDisk)) ?? new PlayerProfile();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("PlayerProfile load: " + e.Message);
            }

            return new PlayerProfile();
        }

        public void Save()
        {
            File.WriteAllText(PathOnDisk, JsonUtility.ToJson(this, true));
        }
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
            new ModuleDef("dungeon", "副本", "Request/LoadMapsItems.xml"),
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
