using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace GunMobile.Net
{
    /// <summary>
    /// In-process Road (4396) + Fight (1910) listener. Runs on the phone.
    /// Relays fight shots between two TCP clients. Not the PC SQL/RSA stack.
    /// </summary>
    public sealed class PhoneRoadServer
    {
        TcpListener _road;
        TcpListener _fight;
        Thread _roadThread;
        Thread _fightThread;
        volatile bool _run;
        readonly List<TcpClient> _fightClients = new List<TcpClient>();
        readonly object _lock = new object();
        byte[] _lastFightStart;

        public bool Running { get; private set; }
        public string LastError { get; private set; } = "";
        public int FightClients
        {
            get
            {
                lock (_lock)
                {
                    return _fightClients.Count;
                }
            }
        }

        public void Start()
        {
            if (Running)
            {
                return;
            }

            try
            {
                _road = new TcpListener(IPAddress.Any, PhonePacket.RoadPort);
                _fight = new TcpListener(IPAddress.Any, PhonePacket.FightPort);
                _road.Start();
                _fight.Start();
                _run = true;
                Running = true;
                _roadThread = new Thread(RoadLoop) { IsBackground = true, Name = "PhoneRoad" };
                _fightThread = new Thread(FightLoop) { IsBackground = true, Name = "PhoneFight" };
                _roadThread.Start();
                _fightThread.Start();
                Debug.Log("PhoneRoad listening " + PhonePacket.RoadPort + "/" + PhonePacket.FightPort);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                Debug.LogWarning("PhoneRoad start: " + e.Message);
                Stop();
            }
        }

        public void Stop()
        {
            _run = false;
            Running = false;
            try { _road?.Stop(); } catch { }
            try { _fight?.Stop(); } catch { }
            lock (_lock)
            {
                foreach (TcpClient c in _fightClients)
                {
                    try { c.Close(); } catch { }
                }

                _fightClients.Clear();
            }
        }

        void RoadLoop()
        {
            while (_run)
            {
                try
                {
                    TcpClient client = _road.AcceptTcpClient();
                    var t = new Thread(() => ServeRoad(client)) { IsBackground = true };
                    t.Start();
                }
                catch
                {
                    if (!_run)
                    {
                        return;
                    }
                }
            }
        }

        void ServeRoad(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream ns = client.GetStream())
                {
                    var buf = new MemoryStream();
                    var tmp = new byte[4096];
                    while (_run && client.Connected)
                    {
                        int n = ns.Read(tmp, 0, tmp.Length);
                        if (n <= 0)
                        {
                            break;
                        }

                        buf.Write(tmp, 0, n);
                        byte[] data = buf.ToArray();
                        int used = 0;
                        while (PhonePacket.TryDecode(data, used, data.Length - used, out ushort id, out string json, out int consumed))
                        {
                            used += consumed;
                            ushort replyId = id == PhoneMsg.Login ? PhoneMsg.LoginOk
                                : id == PhoneMsg.JoinRoom ? PhoneMsg.RoomOk
                                : PhoneMsg.Ping;
                            byte[] reply = PhonePacket.Encode(replyId, "{\"ok\":true,\"service\":\"road\"}");
                            ns.Write(reply, 0, reply.Length);
                        }

                        if (used > 0)
                        {
                            buf.SetLength(0);
                            if (used < data.Length)
                            {
                                buf.Write(data, used, data.Length - used);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        void FightLoop()
        {
            while (_run)
            {
                try
                {
                    TcpClient client = _fight.AcceptTcpClient();
                    lock (_lock)
                    {
                        _fightClients.Add(client);
                    }

                    var t = new Thread(() => ServeFight(client)) { IsBackground = true };
                    t.Start();
                }
                catch
                {
                    if (!_run)
                    {
                        return;
                    }
                }
            }
        }

        void ServeFight(TcpClient client)
        {
            try
            {
                using (NetworkStream ns = client.GetStream())
                {
                    byte[] replay;
                    lock (_lock)
                    {
                        replay = _lastFightStart;
                    }

                    if (replay != null && replay.Length > 0)
                    {
                        try { ns.Write(replay, 0, replay.Length); } catch { }
                    }

                    var buf = new MemoryStream();
                    var tmp = new byte[4096];
                    while (_run && client.Connected)
                    {
                        int n = ns.Read(tmp, 0, tmp.Length);
                        if (n <= 0)
                        {
                            break;
                        }

                        buf.Write(tmp, 0, n);
                        byte[] data = buf.ToArray();
                        int used = 0;
                        while (PhonePacket.TryDecode(data, used, data.Length - used, out ushort id, out string json, out int consumed))
                        {
                            used += consumed;
                            BroadcastFight(id, json, client);
                        }

                        if (used > 0)
                        {
                            buf.SetLength(0);
                            if (used < data.Length)
                            {
                                buf.Write(data, used, data.Length - used);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                lock (_lock)
                {
                    _fightClients.Remove(client);
                }

                try { client.Close(); } catch { }
            }
        }

        void BroadcastFight(ushort id, string json, TcpClient from)
        {
            byte[] pkt = PhonePacket.Encode(id, json);
            if (id == PhoneMsg.FightStart)
            {
                lock (_lock)
                {
                    _lastFightStart = pkt;
                }
            }

            lock (_lock)
            {
                for (int i = _fightClients.Count - 1; i >= 0; i--)
                {
                    TcpClient c = _fightClients[i];
                    if (c == from)
                    {
                        continue;
                    }

                    try
                    {
                        NetworkStream ns = c.GetStream();
                        ns.Write(pkt, 0, pkt.Length);
                    }
                    catch
                    {
                        _fightClients.RemoveAt(i);
                    }
                }
            }
        }
    }
}
