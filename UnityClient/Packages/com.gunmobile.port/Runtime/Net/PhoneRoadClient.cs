using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace GunMobile.Net
{
    public readonly struct PhoneInbox
    {
        public readonly ushort Id;
        public readonly string Json;

        public PhoneInbox(ushort id, string json)
        {
            Id = id;
            Json = json ?? "{}";
        }
    }

    public sealed class PhoneRoadClient
    {
        TcpClient _tcp;
        NetworkStream _ns;
        Thread _recv;
        volatile bool _run;
        readonly Queue<PhoneInbox> _inbox = new Queue<PhoneInbox>();
        readonly object _lock = new object();

        public bool Connected => _tcp != null && _tcp.Connected;
        public string LastError { get; private set; } = "";

        public bool Connect(string host, int port, int timeoutMs = 2500)
        {
            Disconnect();
            try
            {
                _tcp = new TcpClient();
                IAsyncResult ar = _tcp.BeginConnect(host, port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(timeoutMs))
                {
                    _tcp.Close();
                    LastError = "timeout";
                    return false;
                }

                _tcp.EndConnect(ar);
                _ns = _tcp.GetStream();
                _run = true;
                _recv = new Thread(RecvLoop) { IsBackground = true, Name = "PhoneRoadClient" };
                _recv.Start();
                LastError = "";
                return true;
            }
            catch (Exception e)
            {
                LastError = e.Message;
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            _run = false;
            try { _ns?.Close(); } catch { }
            try { _tcp?.Close(); } catch { }
            _tcp = null;
            _ns = null;
        }

        public void Send(ushort id, string json)
        {
            if (!Connected)
            {
                return;
            }

            byte[] pkt = PhonePacket.Encode(id, json);
            try
            {
                _ns.Write(pkt, 0, pkt.Length);
            }
            catch (Exception e)
            {
                LastError = e.Message;
            }
        }

        public bool TryDequeue(out PhoneInbox msg)
        {
            lock (_lock)
            {
                if (_inbox.Count == 0)
                {
                    msg = default;
                    return false;
                }

                msg = _inbox.Dequeue();
                return true;
            }
        }

        void RecvLoop()
        {
            var buf = new MemoryStream();
            var tmp = new byte[4096];
            try
            {
                while (_run && _ns != null)
                {
                    int n = _ns.Read(tmp, 0, tmp.Length);
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
                        lock (_lock)
                        {
                            _inbox.Enqueue(new PhoneInbox(id, json));
                        }
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
            catch
            {
            }
        }
    }
}
