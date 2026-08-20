using System;
using System.IO;
using System.Text;

namespace GunMobile.Net
{
    /// <summary>
    /// Phone Road/Fight packet. Magic 0x7D01 so a PC Flash client will not
    /// mistake this for 7road GSPacketIn. Same TCP ports as the PC services
    /// (4396 hall / 1910 fight) so two phones can LAN-play without SQL Server.
    /// Layout: int32 LE payload-bytes, uint16 magic, uint16 msgId, UTF-8 JSON.
    /// </summary>
    public static class PhonePacket
    {
        public const ushort Magic = 0x7D01;
        public const int RoadPort = 4396;
        public const int FightPort = 1910;

        public static byte[] Encode(ushort msgId, string json)
        {
            byte[] body = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(json) ? "{}" : json);
            var buf = new byte[8 + body.Length];
            WriteInt(buf, 0, 4 + body.Length);
            WriteUShort(buf, 4, Magic);
            WriteUShort(buf, 6, msgId);
            Buffer.BlockCopy(body, 0, buf, 8, body.Length);
            return buf;
        }

        public static bool TryDecode(byte[] data, int offset, int count, out ushort msgId, out string json, out int consumed)
        {
            msgId = 0;
            json = "{}";
            consumed = 0;
            if (count < 8)
            {
                return false;
            }

            int payload = ReadInt(data, offset);
            if (payload < 4 || payload > 1_000_000)
            {
                return false;
            }

            int total = 4 + payload;
            if (count < total)
            {
                return false;
            }

            ushort magic = ReadUShort(data, offset + 4);
            if (magic != Magic)
            {
                return false;
            }

            msgId = ReadUShort(data, offset + 6);
            int bodyLen = payload - 4;
            json = bodyLen <= 0 ? "{}" : Encoding.UTF8.GetString(data, offset + 8, bodyLen);
            consumed = total;
            return true;
        }

        static void WriteInt(byte[] b, int o, int v)
        {
            b[o] = (byte)v;
            b[o + 1] = (byte)(v >> 8);
            b[o + 2] = (byte)(v >> 16);
            b[o + 3] = (byte)(v >> 24);
        }

        static int ReadInt(byte[] b, int o)
        {
            return b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24);
        }

        static void WriteUShort(byte[] b, int o, ushort v)
        {
            b[o] = (byte)v;
            b[o + 1] = (byte)(v >> 8);
        }

        static ushort ReadUShort(byte[] b, int o)
        {
            return (ushort)(b[o] | (b[o + 1] << 8));
        }
    }

    public static class PhoneMsg
    {
        public const ushort Ping = 1;
        public const ushort Login = 2;
        public const ushort LoginOk = 3;
        public const ushort Chat = 6;
        public const ushort JoinRoom = 10;
        public const ushort RoomOk = 11;
        public const ushort GetProfile = 20;
        public const ushort ProfileData = 21;
        public const ushort ShopBuy = 30;
        public const ushort ShopResult = 31;
        public const ushort EquipItem = 32;
        public const ushort EquipResult = 33;
        public const ushort QuestAccept = 40;
        public const ushort QuestComplete = 41;
        public const ushort QuestResult = 42;
        public const ushort PetSelect = 50;
        public const ushort TitleSelect = 51;
        public const ushort CardSelect = 52;
        public const ushort TotemBuy = 53;
        public const ushort MountUpgrade = 54;
        public const ushort StatResult = 55;
        public const ushort SignIn = 60;
        public const ushort SignInResult = 61;
        public const ushort LotteryDraw = 62;
        public const ushort LotteryResult = 63;
        public const ushort Strengthen = 64;
        public const ushort StrengthenResult = 65;
        public const ushort BallSelect = 66;
        public const ushort VipUpgrade = 67;
        public const ushort TexpTrain = 68;
        public const ushort GemUpgrade = 69;
        public const ushort GuildJoin = 70;
        public const ushort GuildDonate = 71;
        public const ushort GuildResult = 72;
        public const ushort FriendAdd = 73;
        public const ushort FriendResult = 74;
        public const ushort MailClaim = 75;
        public const ushort MailResult = 76;
        public const ushort ChatSend = 77;
        public const ushort ChatBroadcast = 78;
        public const ushort RoomList = 80;
        public const ushort RoomListData = 81;
        public const ushort CreateRoom = 82;
        public const ushort RoomCreated = 83;
        public const ushort FightStart = 91;
        public const ushort FightWalk = 92;
        public const ushort FightFire = 93;
        public const ushort FightDamage = 94;
        public const ushort FightOver = 95;
        public const ushort FightReward = 96;
        public const ushort FightTurn = 97;
        public const ushort FightSurrender = 98;
        public const ushort FightProp = 99;
        public const ushort RankData = 85;
        public const ushort RankRequest = 84;
        public const ushort PveStart = 100;
        public const ushort PveResult = 101;
        public const ushort FightState = 102;
        public const ushort FightCrater = 103;
        public const ushort FarmCook = 104;
        public const ushort AuctionSell = 105;
        public const ushort ElfSelect = 106;
        public const ushort KingBless = 107;
        public const ushort SetNick = 108;
        public const ushort FightPetSkill = 109;
        public const ushort MailList = 110;
        public const ushort MailListData = 111;
        public const ushort GodCardOpen = 112;
        public const ushort GodCardResult = 113;
        public const ushort EngraveEquip = 114;
        public const ushort StockTrade = 115;
        public const ushort StockResult = 116;
        public const ushort FightShotResult = 117;
        public const ushort FightSkip = 118;
        public const ushort GuildCreate = 119;
        public const ushort GuildLeave = 120;
        public const ushort FriendRemove = 121;
        public const ushort AuctionList = 122;
        public const ushort AuctionListData = 123;
        public const ushort AuctionBuy = 124;
        public const ushort GemSpiritUpgrade = 125;
        public const ushort MagicStoneUpgrade = 126;
        public const ushort MagicFusion = 127;
        public const ushort BankTrade = 128;
        public const ushort MineDig = 129;
        public const ushort TeamDungeonStart = 130;
        public const ushort TreasureDraw = 131;
        public const ushort CarnivalDraw = 132;
        public const ushort PeakBattleStart = 133;
        public const ushort WorldBossStart = 134;
        public const ushort NecklaceUpgrade = 135;
        public const ushort DevilTurnSpin = 136;
        public const ushort RedPacketClaim = 137;
        public const ushort HomeTempleUpgrade = 138;
        public const ushort MailSend = 139;
        public const ushort SweepLabyrinth = 140;
        public const ushort EmblemCraft = 141;
        public const ushort EmblemEquip = 142;
        public const ushort SoulStampCompose = 143;
        public const ushort SoulStampRefine = 144;
        public const ushort GodCardRaise = 145;
        public const ushort GodCardPointClaim = 146;
        public const ushort ForcesBattleStart = 147;
        public const ushort ForcesRelicUpgrade = 148;
        public const ushort CultureUpgrade = 157;
        public const ushort CultureResult = 158;
        public const ushort DreamlandStart = 149;
        public const ushort DreamlandClaim = 150;
        public const ushort WardrobeEquip = 151;
        public const ushort WardrobeUpgrade = 152;
        public const ushort HonorSystemAction = 153;
        public const ushort HonorSystemClaim = 154;
        public const ushort WarriorFamStart = 155;
        public const ushort WarriorFamClaim = 156;
        public const ushort FirstRechargeClaim = 161;
        public const ushort FirstRechargeShop = 162;
        public const ushort SpaRoomStart = 159;
        public const ushort SpaRoomBomb = 160;
        public const ushort TreasureRoomDraw = 163;
        public const ushort TreasureRoomResult = 164;
        public const ushort ChristmasClaim = 165;
        public const ushort NewYearClaim = 166;
        public const ushort BoguAdventureAction = 167;
        public const ushort WorshipMoonClaim = 168;
        public const ushort SuperLuckerDraw = 169;
        public const ushort HomeTemplePractice = 170;
        public const ushort HomeTempleAdvance = 171;
        public const ushort BankDeposit = 172;
        public const ushort SweepMission = 173;
        public const ushort JampsUpgrade = 174;
        public const ushort JampsClaimPage = 175;
        public const ushort CardMainUpgrade = 176;
        public const ushort ElfIntimacyAction = 177;
        public const ushort DevilTreasPointClaim = 178;
        public const ushort RedPacketSend = 179;
        public const ushort CalendarClaim = 180;
        public const ushort AuditoriumAction = 181;
        public const ushort JigsawAction = 182;
        public const ushort BibleAction = 183;
        public const ushort ChatWhisper = 184;
        public const ushort GuildUpgrade = 185;
        public const ushort ConsortiaBossStart = 186;
        public const ushort PetStarUpgrade = 187;
        public const ushort MountTalismanEquip = 188;
        public const ushort ManorUpgrade = 189;
        public const ushort QuizAnswer = 190;
        public const ushort OneYuanBuy = 191;
        public const ushort GoldEquipUpgrade = 192;
        public const ushort GloryUpgrade = 193;
        public const ushort SigilRoll = 194;
        public const ushort MountSkillUnlock = 195;
        public const ushort AchievementClaim = 196;
        public const ushort LinkPalAction = 197;
        public const ushort JadeEquip = 198;
        public const ushort RuneEquip = 199;
        public const ushort HorseAmuletUpgrade = 200;
        public const ushort CardBookletClaim = 201;
        public const ushort StrengthenGoodsMap = 202;
        public const ushort BoxOpen = 203;
        public const ushort ItemFusion = 204;
        public const ushort EngraveDebrisAction = 205;
        public const ushort PetSkillUnlock = 206;
        public const ushort ActivityQuestClaim = 207;
        public const ushort SwornAction = 208;
        public const ushort VipStoreBuy = 209;
        public const ushort MountDraw = 210;
        public const ushort PetFightProperty = 211;
        public const ushort NewYearRankClaim = 212;
        public const ushort RoomReady = 86;
        public const ushort RoomState = 87;
        public const ushort RoomLeave = 88;
        public const ushort Error = 255;
    }
}
