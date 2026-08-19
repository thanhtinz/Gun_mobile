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
        public const ushort Error = 255;
    }
}
