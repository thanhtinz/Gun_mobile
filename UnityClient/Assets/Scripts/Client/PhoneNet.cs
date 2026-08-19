using System;
using System.Net;
using System.Net.Sockets;
using GunMobile.Net;
using UnityEngine;

namespace GunMobile.Client
{
    public static class PhoneNet
    {
        public static PhoneRoadServer Server { get; private set; }
        public static PhoneRoadClient Road { get; private set; }
        public static PhoneRoadClient Fight { get; private set; }
        public static bool NetBattle;
        public static int Seat;
        public static string PeerHost = "127.0.0.1";
        public static int BattleSeed;

        public static void Boot()
        {
            if (Server != null)
            {
                return;
            }

            Server = new PhoneRoadServer();
            Server.Start();
            Road = new PhoneRoadClient();
            Fight = new PhoneRoadClient();
            if (Road.Connect("127.0.0.1", PhonePacket.RoadPort))
            {
                Road.Send(PhoneMsg.Login, "{\"nick\":\"phone\"}");
            }
        }

        public static void Shutdown()
        {
            Fight?.Disconnect();
            Road?.Disconnect();
            Server?.Stop();
        }

        public static bool ConnectHall(string host)
        {
            PeerHost = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host.Trim();
            Road?.Disconnect();
            Road = new PhoneRoadClient();
            bool ok = Road.Connect(PeerHost, PhonePacket.RoadPort);
            if (ok)
            {
                Road.Send(PhoneMsg.Login, "{\"nick\":\"phone\"}");
            }

            return ok;
        }

        public static bool ConnectFight(string host)
        {
            PeerHost = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host.Trim();
            Fight?.Disconnect();
            Fight = new PhoneRoadClient();
            bool ok = Fight.Connect(PeerHost, PhonePacket.FightPort);
            if (ok)
            {
                Fight.Send(PhoneMsg.JoinRoom, "{\"seat\":" + Seat + "}");
            }

            return ok;
        }

        public static void SendFire(int who, float angle, float power, int facing)
        {
            if (Fight == null || !Fight.Connected)
            {
                return;
            }

            Fight.Send(
                PhoneMsg.FightFire,
                "{\"who\":" + who + ",\"angle\":" + angle.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                ",\"power\":" + power.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                ",\"facing\":" + facing + "}");
        }

        public static void SendWalk(int who, float x, int facing)
        {
            if (Fight == null || !Fight.Connected)
            {
                return;
            }

            Fight.Send(
                PhoneMsg.FightWalk,
                "{\"who\":" + who + ",\"x\":" + x.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                ",\"facing\":" + facing + "}");
        }

        public static void SendStart(int mapId)
        {
            if (Fight == null || !Fight.Connected)
            {
                return;
            }

            BattleSeed = Environment.TickCount;
            if (BattleSeed == 0)
            {
                BattleSeed = 1;
            }

            Fight.Send(PhoneMsg.FightStart, "{\"map\":" + mapId + ",\"seed\":" + BattleSeed + "}");
        }

        public static string LanIPv4()
        {
            try
            {
                foreach (IPAddress addr in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(addr))
                    {
                        return addr.ToString();
                    }
                }
            }
            catch
            {
            }

            return "127.0.0.1";
        }

        public static string StatusLine()
        {
            string road = Server != null && Server.Running ? "Road :" + PhonePacket.RoadPort + " 开" : "Road 关";
            string fight = Server != null && Server.Running ? "Fight :" + PhonePacket.FightPort + " 开" : "Fight 关";
            string link = Fight != null && Fight.Connected ? " 已连" : "";
            string peers = Server != null ? " peers " + Server.FightClients : "";
            string err = Server != null && !string.IsNullOrEmpty(Server.LastError) ? "  " + Server.LastError : "";
            return road + "  " + fight + link + peers + err + "  IP " + LanIPv4();
        }
    }
}
