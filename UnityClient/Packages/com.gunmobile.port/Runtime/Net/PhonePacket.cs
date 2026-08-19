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
        public const ushort FightStart = 91;
        public const ushort FightWalk = 92;
        public const ushort FightFire = 93;
        public const ushort FightOver = 95;
        public const ushort Error = 255;
    }
}
