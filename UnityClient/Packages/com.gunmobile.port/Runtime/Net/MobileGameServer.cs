using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GunMobile.Core;
using GunMobile.Logic;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Net
{
    public sealed class ServerPlayer
    {
        public int Id;
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
        public int WeaponId = 7001;
        public int EquipHead;
        public int EquipHair;
        public int EquipFace;
        public int EquipCloth;
        public int EquipGlass;
        public int EquipWeapon = 7001;
        public int PetId;
        public int CardId;
        public int TitleId;
        public int TotemId;
        public int MountGrade;
        public int VipLevel;
        public int Honor;
        public int Texp;
        public int PreferredBallId;
        public int LastSignDay = -1;
        public int SignIndex;
        public int LabyrinthFloor = 1;
        public string ConsortiaName = "";
        public int ElfId;
        public int GemLevel;
        public List<BagSlot> Bag = new List<BagSlot>();
        public List<int> AcceptedQuests = new List<int>();
        public List<int> CompletedQuests = new List<int>();
        public List<string> Friends = new List<string>();

        public TcpClient RoadTcp;
        public NetworkStream RoadStream;
        public TcpClient FightTcp;
        public NetworkStream FightStream;
        public int RoomId = -1;
        public int Seat = -1;

        public void RecalcStats(GameDatabase db)
        {
            if (db == null) return;
            int atk = 50, def = 40, agi = 40, luck = 30, hp = 1200;

            foreach (int eid in new[] { EquipHead, EquipHair, EquipFace, EquipCloth, EquipGlass, EquipWeapon })
            {
                ItemTemplate it = db.GetItem(eid);
                if (it == null) continue;
                atk += it.Attack; def += it.Defence; agi += it.Agility; luck += it.Luck;
            }

            if (db.Pets.TryGetValue(PetId, out PetInfo pet))
            {
                atk += pet.Attack; def += pet.Defence; hp += pet.Blood; agi += pet.Agility; luck += pet.Luck;
            }

            if (db.Cards != null)
            {
                foreach (CardInfo c in db.Cards)
                {
                    if (c.Id == CardId) { atk += c.AddAttack; def += c.AddDefend; agi += c.AddAgility; luck += c.AddLucky; break; }
                }
            }

            if (db.Titles.TryGetValue(TitleId, out TitleInfo ti))
            {
                atk += ti.Att; def += ti.Def; agi += ti.Agi; luck += ti.Luck;
            }

            if (db.Totems.TryGetValue(TotemId, out TotemInfo to))
            {
                atk += to.AddAttack; def += to.AddDefence; agi += to.AddAgility; luck += to.AddLuck; hp += to.AddBlood;
            }

            if (db.Mounts.TryGetValue(MountGrade, out MountGrade mt))
            {
                hp += mt.AddBlood; atk += mt.AddDamage;
            }

            atk += Texp / 4;
            hp += GemLevel * 120;
            hp += Level * 30;

            Attack = atk; Defence = def; Agility = agi; Luck = luck; Hp = hp;
        }

        public string ToJson()
        {
            var sb = new StringBuilder(512);
            sb.Append("{");
            J(sb, "id", Id); sb.Append(",");
            J(sb, "nick", Nick); sb.Append(",");
            J(sb, "sex", Sex); sb.Append(",");
            J(sb, "level", Level); sb.Append(",");
            J(sb, "gold", Gold); sb.Append(",");
            J(sb, "gift", Gift); sb.Append(",");
            J(sb, "attack", Attack); sb.Append(",");
            J(sb, "defence", Defence); sb.Append(",");
            J(sb, "agility", Agility); sb.Append(",");
            J(sb, "luck", Luck); sb.Append(",");
            J(sb, "hp", Hp); sb.Append(",");
            J(sb, "win", Win); sb.Append(",");
            J(sb, "lose", Lose); sb.Append(",");
            J(sb, "weaponId", WeaponId); sb.Append(",");
            J(sb, "equipHead", EquipHead); sb.Append(",");
            J(sb, "equipCloth", EquipCloth); sb.Append(",");
            J(sb, "equipWeapon", EquipWeapon); sb.Append(",");
            J(sb, "petId", PetId); sb.Append(",");
            J(sb, "cardId", CardId); sb.Append(",");
            J(sb, "titleId", TitleId); sb.Append(",");
            J(sb, "totemId", TotemId); sb.Append(",");
            J(sb, "mountGrade", MountGrade); sb.Append(",");
            J(sb, "vipLevel", VipLevel); sb.Append(",");
            J(sb, "honor", Honor); sb.Append(",");
            J(sb, "texp", Texp); sb.Append(",");
            J(sb, "preferredBallId", PreferredBallId); sb.Append(",");
            J(sb, "lastSignDay", LastSignDay); sb.Append(",");
            J(sb, "signIndex", SignIndex); sb.Append(",");
            J(sb, "labyrinthFloor", LabyrinthFloor); sb.Append(",");
            J(sb, "consortiaName", ConsortiaName); sb.Append(",");
            J(sb, "elfId", ElfId); sb.Append(",");
            J(sb, "gemLevel", GemLevel); sb.Append(",");
            sb.Append("\"bag\":[");
            for (int i = 0; i < Bag.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("{");
                J(sb, "t", Bag[i].TemplateId); sb.Append(",");
                J(sb, "c", Bag[i].Count); sb.Append(",");
                J(sb, "s", Bag[i].Strengthen);
                sb.Append("}");
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        static void J(StringBuilder sb, string k, int v) { sb.Append("\"").Append(k).Append("\":").Append(v); }
        static void J(StringBuilder sb, string k, string v)
        {
            sb.Append("\"").Append(k).Append("\":\"").Append((v ?? "").Replace("\"", "\\\"")).Append("\"");
        }

        public bool AddItem(int templateId, int count)
        {
            foreach (var s in Bag) { if (s.TemplateId == templateId) { s.Count += count; return true; } }
            Bag.Add(new BagSlot { TemplateId = templateId, Count = count });
            return true;
        }

        public bool Consume(int templateId, int count)
        {
            for (int i = 0; i < Bag.Count; i++)
            {
                if (Bag[i].TemplateId == templateId && Bag[i].Count >= count)
                {
                    Bag[i].Count -= count;
                    if (Bag[i].Count <= 0) Bag.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool Equip(ItemTemplate item)
        {
            if (item == null || !item.CanEquip) return false;
            switch (item.CategoryId)
            {
                case 1: EquipHead = item.TemplateId; break;
                case 2: EquipGlass = item.TemplateId; break;
                case 3: EquipHair = item.TemplateId; break;
                case 5: EquipCloth = item.TemplateId; break;
                case 6: EquipFace = item.TemplateId; break;
                case 7: EquipWeapon = item.TemplateId; WeaponId = item.TemplateId; break;
                default: return false;
            }
            return true;
        }
    }

    public sealed class BagSlot
    {
        public int TemplateId;
        public int Count = 1;
        public int Strengthen;
    }

    public sealed class GameRoom
    {
        public int Id;
        public string Name = "Room";
        public int MapId;
        public int MaxPlayers = 2;
        public List<int> PlayerIds = new List<int>();
        public bool InBattle;
        public int Seed;
        public int CurrentTurn;
        public int CurrentPlayer;
        public float TurnTimeLeft = 20f;
        public float Wind;
        public int[] Hp;
        public int[] MaxHp;
        public long TurnStartMs;
        public System.Random Rng;
    }

    /// <summary>
    /// Full game server replacing PC Road.Service.exe + Fight.Service.exe.
    /// Uses the same PC XML tables but runs in-process with no SQL Server.
    /// Handles: auth, profile, shop, bag, equip, quest, pet, card, title, totem,
    /// mount, elf, sign-in, lottery, forge, guild, friends, mail, chat,
    /// room list, matchmaking, and server-authoritative battle.
    /// </summary>
    public sealed class MobileGameServer
    {
        readonly object _lock = new object();
        readonly Dictionary<int, ServerPlayer> _players = new Dictionary<int, ServerPlayer>();
        readonly Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _nextPlayerId = 1;
        int _nextRoomId = 1;
        TcpListener _road;
        TcpListener _fight;
        Thread _roadThread;
        Thread _fightThread;
        Thread _timerThread;
        volatile bool _run;
        GameDatabase _db;
        System.Random _rng = new System.Random();
        string _savePath;

        public bool Running { get; private set; }
        public string LastError { get; private set; } = "";
        public int PlayerCount { get { lock (_lock) return _players.Count; } }
        public int RoomCount { get { lock (_lock) return _rooms.Count; } }

        public void Start(GameDatabase db, string savePath = null)
        {
            if (Running) return;
            _db = db;
            _savePath = savePath ?? Path.Combine(Application.persistentDataPath, "server_players");
            try
            {
                Directory.CreateDirectory(_savePath);
            }
            catch { }

            try
            {
                _road = new TcpListener(IPAddress.Any, PhonePacket.RoadPort);
                _fight = new TcpListener(IPAddress.Any, PhonePacket.FightPort);
                _road.Start();
                _fight.Start();
                _run = true;
                Running = true;
                _roadThread = new Thread(AcceptRoad) { IsBackground = true, Name = "MobileRoad" };
                _fightThread = new Thread(AcceptFight) { IsBackground = true, Name = "MobileFight" };
                _roadThread.Start();
                _fightThread.Start();

                _timerThread = new Thread(TurnTimerLoop) { IsBackground = true, Name = "MobileTurnTimer" };
                _timerThread.Start();
                Debug.Log($"MobileGameServer listening Road:{PhonePacket.RoadPort} Fight:{PhonePacket.FightPort}");
            }
            catch (Exception e)
            {
                LastError = e.Message;
                Debug.LogWarning("MobileGameServer start: " + e.Message);
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
                foreach (var p in _players.Values)
                {
                    try { p.RoadTcp?.Close(); } catch { }
                    try { p.FightTcp?.Close(); } catch { }
                }
            }
        }

        void AcceptRoad()
        {
            while (_run)
            {
                try
                {
                    TcpClient client = _road.AcceptTcpClient();
                    new Thread(() => ServeRoad(client)) { IsBackground = true }.Start();
                }
                catch { if (!_run) return; }
            }
        }

        void AcceptFight()
        {
            while (_run)
            {
                try
                {
                    TcpClient client = _fight.AcceptTcpClient();
                    new Thread(() => ServeFight(client)) { IsBackground = true }.Start();
                }
                catch { if (!_run) return; }
            }
        }

        void Send(NetworkStream ns, ushort id, string json)
        {
            if (ns == null) return;
            byte[] pkt = PhonePacket.Encode(id, json);
            try { ns.Write(pkt, 0, pkt.Length); } catch { }
        }

        void SendTo(ServerPlayer p, ushort id, string json)
        {
            if (p?.RoadStream != null) Send(p.RoadStream, id, json);
        }

        void SendFightTo(ServerPlayer p, ushort id, string json)
        {
            if (p?.FightStream != null) Send(p.FightStream, id, json);
        }

        ServerPlayer FindByRoadTcp(TcpClient tcp)
        {
            lock (_lock)
            {
                foreach (var p in _players.Values)
                    if (p.RoadTcp == tcp) return p;
            }
            return null;
        }

        ServerPlayer FindByFightTcp(TcpClient tcp)
        {
            lock (_lock)
            {
                foreach (var p in _players.Values)
                    if (p.FightTcp == tcp) return p;
            }
            return null;
        }

        void ServeRoad(TcpClient client)
        {
            ServerPlayer player = null;
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
                        if (n <= 0) break;
                        buf.Write(tmp, 0, n);
                        byte[] data = buf.ToArray();
                        int used = 0;
                        while (PhonePacket.TryDecode(data, used, data.Length - used, out ushort id, out string json, out int consumed))
                        {
                            used += consumed;
                            if (id == PhoneMsg.Login)
                            {
                                player = HandleLogin(client, ns, json);
                            }
                            else if (player != null)
                            {
                                HandleRoadMsg(player, ns, id, json);
                            }
                        }
                        if (used > 0)
                        {
                            buf.SetLength(0);
                            if (used < data.Length) buf.Write(data, used, data.Length - used);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (player != null)
                {
                    SavePlayer(player);
                    lock (_lock) { player.RoadTcp = null; player.RoadStream = null; }
                }
            }
        }

        void TurnTimerLoop()
        {
            // Online battle: if a client doesn't send FightTurn in time,
            // server will auto-advance (skip turn) to prevent deadlocks.
            const long turnMs = 20000; // must match client BattleLoop default (20s)
            const int tickMs = 200;

            while (_run)
            {
                try
                {
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    List<GameRoom> advance = null;

                    lock (_lock)
                    {
                        foreach (var room in _rooms.Values)
                        {
                            if (room == null || !room.InBattle) continue;
                            if (room.TurnStartMs <= 0) continue;

                            if (now - room.TurnStartMs >= turnMs)
                            {
                                advance ??= new List<GameRoom>();
                                // Mark immediately to reduce double-advance race.
                                room.TurnStartMs = now;
                                advance.Add(room);
                            }
                        }
                    }

                    if (advance != null)
                    {
                        foreach (var r in advance)
                        {
                            AdvanceTurn(r);
                        }
                    }
                }
                catch { }

                Thread.Sleep(tickMs);
            }
        }

        void ServeFight(TcpClient client)
        {
            ServerPlayer player = null;
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
                        if (n <= 0) break;
                        buf.Write(tmp, 0, n);
                        byte[] data = buf.ToArray();
                        int used = 0;
                        while (PhonePacket.TryDecode(data, used, data.Length - used, out ushort id, out string json, out int consumed))
                        {
                            used += consumed;
                            if (id == PhoneMsg.JoinRoom)
                            {
                                int playerId = JI(json, "playerId", 0);
                                lock (_lock)
                                {
                                    if (_players.TryGetValue(playerId, out player))
                                    {
                                        player.FightTcp = client;
                                        player.FightStream = ns;
                                    }
                                }
                                Send(ns, PhoneMsg.RoomOk, "{\"ok\":true}");
                            }
                            else if (player != null)
                            {
                                HandleFightMsg(player, ns, id, json);
                            }
                        }
                        if (used > 0)
                        {
                            buf.SetLength(0);
                            if (used < data.Length) buf.Write(data, used, data.Length - used);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (player != null)
                {
                    lock (_lock) { player.FightTcp = null; player.FightStream = null; }
                }
            }
        }

        ServerPlayer HandleLogin(TcpClient tcp, NetworkStream ns, string json)
        {
            string nick = JS(json, "nick", "Player");
            ServerPlayer player;
            lock (_lock)
            {
                player = LoadOrCreate(nick);
                player.RoadTcp = tcp;
                player.RoadStream = ns;
            }
            player.RecalcStats(_db);
            Send(ns, PhoneMsg.LoginOk, "{\"ok\":true,\"playerId\":" + player.Id + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
            return player;
        }

        void HandleRoadMsg(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            switch (id)
            {
                case PhoneMsg.GetProfile:
                    player.RecalcStats(_db);
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;

                case PhoneMsg.ShopBuy:
                    HandleShopBuy(player, ns, json);
                    break;

                case PhoneMsg.EquipItem:
                    HandleEquip(player, ns, json);
                    break;

                case PhoneMsg.QuestAccept:
                case PhoneMsg.QuestComplete:
                    HandleQuest(player, ns, id, json);
                    break;

                case PhoneMsg.PetSelect:
                    player.PetId = JI(json, "petId", player.PetId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.TitleSelect:
                    player.TitleId = JI(json, "titleId", player.TitleId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.CardSelect:
                    player.CardId = JI(json, "cardId", player.CardId);
                    player.RecalcStats(_db);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.TotemBuy:
                    HandleTotemBuy(player, ns, json);
                    break;

                case PhoneMsg.MountUpgrade:
                    HandleMountUpgrade(player, ns, json);
                    break;

                case PhoneMsg.SignIn:
                    HandleSignIn(player, ns);
                    break;

                case PhoneMsg.LotteryDraw:
                    HandleLottery(player, ns, json);
                    break;

                case PhoneMsg.Strengthen:
                    HandleStrengthen(player, ns, json);
                    break;

                case PhoneMsg.BallSelect:
                    player.PreferredBallId = JI(json, "ballId", 0);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.StatResult, player.ToJson());
                    break;

                case PhoneMsg.GuildJoin:
                    player.ConsortiaName = JS(json, "name", player.ConsortiaName);
                    SavePlayer(player);
                    Send(ns, PhoneMsg.GuildResult, "{\"ok\":true}");
                    break;

                case PhoneMsg.GuildDonate:
                    if (player.Gold >= 1000 && !string.IsNullOrEmpty(player.ConsortiaName))
                    {
                        player.Gold -= 1000;
                        player.Honor += 80;
                        SavePlayer(player);
                    }
                    Send(ns, PhoneMsg.GuildResult, player.ToJson());
                    break;

                case PhoneMsg.FriendAdd:
                    string fn = JS(json, "name", "");
                    if (!string.IsNullOrEmpty(fn) && !player.Friends.Contains(fn))
                    {
                        player.Friends.Add(fn);
                        SavePlayer(player);
                    }
                    Send(ns, PhoneMsg.FriendResult, "{\"ok\":true}");
                    break;

                case PhoneMsg.MailClaim:
                    Send(ns, PhoneMsg.MailResult, "{\"ok\":true,\"gold\":0}");
                    break;

                case PhoneMsg.ChatSend:
                {
                    string msg = JS(json, "msg", "");
                    if (!string.IsNullOrEmpty(msg))
                    {
                        string broadcast = "{\"from\":\"" + (player.Nick ?? "").Replace("\"", "") + "\",\"msg\":\"" + msg.Replace("\"", "") + "\"}";
                        lock (_lock)
                        {
                            foreach (var p in _players.Values)
                                SendTo(p, PhoneMsg.ChatBroadcast, broadcast);
                        }
                    }
                    break;
                }

                case PhoneMsg.RoomList:
                    HandleRoomList(player, ns);
                    break;

                case PhoneMsg.CreateRoom:
                    HandleCreateRoom(player, ns, json);
                    break;

                case PhoneMsg.JoinRoom:
                    HandleJoinRoom(player, ns, json);
                    break;

                case PhoneMsg.VipUpgrade:
                    HandleVipUpgrade(player, ns);
                    break;

                case PhoneMsg.TexpTrain:
                    HandleTexpTrain(player, ns);
                    break;

                case PhoneMsg.GemUpgrade:
                    HandleGemUpgrade(player, ns);
                    break;

                case PhoneMsg.Ping:
                    Send(ns, PhoneMsg.Ping, "{}");
                    break;
            }
        }

        void HandleFightMsg(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            GameRoom room;
            lock (_lock)
            {
                if (player.RoomId < 0 || !_rooms.TryGetValue(player.RoomId, out room)) return;
            }

            switch (id)
            {
                case PhoneMsg.FightStart:
                    HandleFightStart(player, room, json);
                    break;

                case PhoneMsg.FightFire:
                case PhoneMsg.FightWalk:
                    // Enforce: only the room.CurrentPlayer is allowed to act.
                    bool allow;
                    lock (_lock)
                    {
                        allow = room.InBattle && player.Seat == room.CurrentPlayer;
                    }
                    if (!allow) return;
                    BroadcastToRoom(room, id, json, player.Id);
                    break;

                case PhoneMsg.FightTurn:
                {
                    int turn = JI(json, "turn", room.CurrentTurn);
                    int who = JI(json, "player", room.CurrentPlayer);
                    float wind = JF(json, "wind", room.Wind);
                    lock (_lock)
                    {
                        room.CurrentTurn = turn;
                        room.CurrentPlayer = who;
                        room.Wind = wind;
                        room.TurnStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    }
                    // Broadcast the server-synced turn to all clients so host/join stay aligned.
                    string turnJson = "{\"turn\":" + turn +
                                       ",\"player\":" + who +
                                       ",\"wind\":" + wind.ToString(CultureInfo.InvariantCulture) + "}";
                    BroadcastToRoom(room, PhoneMsg.FightTurn, turnJson, -1);
                    break;
                }

                case PhoneMsg.FightDamage:
                    HandleFightDamage(player, room, json);
                    break;

                case PhoneMsg.FightOver:
                    HandleFightOver(player, room, json);
                    break;
            }
        }

        void HandleShopBuy(ServerPlayer player, NetworkStream ns, string json)
        {
            int offerId = JI(json, "offerId", 0);
            ShopOffer offer = null;
            if (_db != null)
            {
                foreach (var o in _db.Shop) { if (o.Id == offerId) { offer = o; break; } }
            }

            if (offer == null)
            {
                Send(ns, PhoneMsg.ShopResult, "{\"ok\":false,\"err\":\"no offer\"}");
                return;
            }

            bool isGift = offer.APrice1 == -2;
            int price = offer.AValue1;
            if (isGift)
            {
                if (player.Gift < price) { Send(ns, PhoneMsg.ShopResult, "{\"ok\":false,\"err\":\"not enough gift\"}"); return; }
                player.Gift -= price;
            }
            else
            {
                if (player.Gold < price) { Send(ns, PhoneMsg.ShopResult, "{\"ok\":false,\"err\":\"not enough gold\"}"); return; }
                player.Gold -= price;
            }

            player.AddItem(offer.TemplateId, 1);
            SavePlayer(player);
            Send(ns, PhoneMsg.ShopResult, "{\"ok\":true,\"templateId\":" + offer.TemplateId + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleEquip(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 0);
            ItemTemplate item = _db?.GetItem(templateId);
            if (item != null && player.Equip(item))
            {
                player.RecalcStats(_db);
                SavePlayer(player);
                Send(ns, PhoneMsg.EquipResult, "{\"ok\":true}");
                Send(ns, PhoneMsg.ProfileData, player.ToJson());
            }
            else
            {
                Send(ns, PhoneMsg.EquipResult, "{\"ok\":false}");
            }
        }

        void HandleQuest(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            int questId = JI(json, "questId", 0);
            if (id == PhoneMsg.QuestAccept)
            {
                if (!player.AcceptedQuests.Contains(questId))
                    player.AcceptedQuests.Add(questId);
            }
            else
            {
                player.AcceptedQuests.Remove(questId);
                if (!player.CompletedQuests.Contains(questId))
                    player.CompletedQuests.Add(questId);
                if (_db != null)
                {
                    foreach (var q in _db.Quests)
                    {
                        if (q.Id == questId)
                        {
                            player.Gold += Mathf.Max(50, q.RewardGold);
                            player.Honor += 5;
                            break;
                        }
                    }
                }
            }
            SavePlayer(player);
            Send(ns, PhoneMsg.QuestResult, player.ToJson());
        }

        void HandleTotemBuy(ServerPlayer player, NetworkStream ns, string json)
        {
            int totemId = JI(json, "totemId", 0);
            if (_db != null && _db.Totems.TryGetValue(totemId, out TotemInfo t))
            {
                if (player.Honor >= t.ConsumeHonor)
                {
                    if (player.TotemId != totemId && t.ConsumeHonor > 0)
                        player.Honor -= t.ConsumeHonor;
                    player.TotemId = totemId;
                    player.RecalcStats(_db);
                    SavePlayer(player);
                }
            }
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleMountUpgrade(ServerPlayer player, NetworkStream ns, string json)
        {
            int cost = 800 + player.MountGrade * 200;
            if (player.Gold >= cost)
            {
                player.Gold -= cost;
                player.MountGrade++;
                player.RecalcStats(_db);
                SavePlayer(player);
            }
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleSignIn(ServerPlayer player, NetworkStream ns)
        {
            int today = DateTime.Now.DayOfYear;
            if (player.LastSignDay == today)
            {
                Send(ns, PhoneMsg.SignInResult, "{\"ok\":false,\"err\":\"already signed\"}");
                return;
            }
            player.LastSignDay = today;
            player.SignIndex = Mathf.Min(28, player.SignIndex + 1);
            player.Gold += 1200;
            player.Gift += 20;
            SavePlayer(player);
            Send(ns, PhoneMsg.SignInResult, "{\"ok\":true}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleLottery(ServerPlayer player, NetworkStream ns, string json)
        {
            int count = JI(json, "count", 1);
            if (count < 1) count = 1;
            int cost = count == 1 ? 300 : 2700;
            if (player.Gold < cost || _db == null || _db.Lottery.Count == 0)
            {
                Send(ns, PhoneMsg.LotteryResult, "{\"ok\":false}");
                return;
            }
            player.Gold -= cost;
            int draws = count == 1 ? 1 : 10;
            var won = new List<int>();
            for (int i = 0; i < draws; i++)
            {
                int idx;
                lock (_lock) { idx = _rng.Next(0, _db.Lottery.Count); }
                var drop = _db.Lottery[idx];
                player.AddItem(drop.TemplateId, drop.Count);
                won.Add(drop.TemplateId);
            }
            SavePlayer(player);
            var sb = new StringBuilder("{\"ok\":true,\"items\":[");
            for (int i = 0; i < won.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(won[i]);
            }
            sb.Append("]}");
            Send(ns, PhoneMsg.LotteryResult, sb.ToString());
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleStrengthen(ServerPlayer player, NetworkStream ns, string json)
        {
            int templateId = JI(json, "templateId", 0);
            BagSlot slot = null;
            foreach (var s in player.Bag) { if (s.TemplateId == templateId) { slot = s; break; } }
            if (slot == null || slot.Strengthen >= 15)
            {
                Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":false}");
                return;
            }
            int next = slot.Strengthen + 1;
            int rock = 200 * next;
            if (_db != null && _db.StrengthenRock.TryGetValue(next, out int r)) rock = r;
            int gold = Mathf.Max(100, rock * 40);
            if (player.Gold < gold)
            {
                Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":false,\"err\":\"gold\"}");
                return;
            }
            player.Gold -= gold;
            int chance;
            lock (_lock) { chance = _rng.Next(0, 100); }
            bool success = chance < Mathf.Clamp(90 - slot.Strengthen * 5, 20, 90);
            if (success) slot.Strengthen++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StrengthenResult, "{\"ok\":true,\"success\":" + (success ? "true" : "false") + ",\"level\":" + slot.Strengthen + "}");
            Send(ns, PhoneMsg.ProfileData, player.ToJson());
        }

        void HandleVipUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (player.Gift < 500 || player.VipLevel >= 15)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }
            player.Gift -= 500;
            player.VipLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleTexpTrain(ServerPlayer player, NetworkStream ns)
        {
            if (player.Gold < 400)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }
            player.Gold -= 400;
            player.Texp += 25;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleGemUpgrade(ServerPlayer player, NetworkStream ns)
        {
            if (player.Gold < 600 || player.GemLevel >= 12)
            {
                Send(ns, PhoneMsg.StatResult, player.ToJson());
                return;
            }
            player.Gold -= 600;
            player.GemLevel++;
            player.RecalcStats(_db);
            SavePlayer(player);
            Send(ns, PhoneMsg.StatResult, player.ToJson());
        }

        void HandleRoomList(ServerPlayer player, NetworkStream ns)
        {
            var sb = new StringBuilder("{\"rooms\":[");
            int i = 0;
            lock (_lock)
            {
                foreach (var r in _rooms.Values)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("{\"id\":").Append(r.Id)
                      .Append(",\"name\":\"").Append(r.Name.Replace("\"", ""))
                      .Append("\",\"map\":").Append(r.MapId)
                      .Append(",\"players\":").Append(r.PlayerIds.Count)
                      .Append(",\"max\":").Append(r.MaxPlayers)
                      .Append(",\"inBattle\":").Append(r.InBattle ? "true" : "false")
                      .Append("}");
                    i++;
                }
            }
            sb.Append("]}");
            Send(ns, PhoneMsg.RoomListData, sb.ToString());
        }

        void HandleCreateRoom(ServerPlayer player, NetworkStream ns, string json)
        {
            int mapId = JI(json, "mapId", 1056);
            string name = JS(json, "name", player.Nick + "'s Room");
            GameRoom room;
            lock (_lock)
            {
                room = new GameRoom { Id = _nextRoomId++, MapId = mapId, Name = name };
                room.PlayerIds.Add(player.Id);
                player.RoomId = room.Id;
                player.Seat = 0;
                _rooms[room.Id] = room;
            }
            Send(ns, PhoneMsg.RoomCreated, "{\"roomId\":" + room.Id + ",\"seat\":0}");
        }

        void HandleJoinRoom(ServerPlayer player, NetworkStream ns, string json)
        {
            int roomId = JI(json, "roomId", 0);
            lock (_lock)
            {
                if (!_rooms.TryGetValue(roomId, out GameRoom room) || room.PlayerIds.Count >= room.MaxPlayers || room.InBattle)
                {
                    Send(ns, PhoneMsg.Error, "{\"err\":\"room full or not found\"}");
                    return;
                }
                room.PlayerIds.Add(player.Id);
                player.RoomId = roomId;
                player.Seat = room.PlayerIds.Count - 1;
            }
            Send(ns, PhoneMsg.RoomOk, "{\"roomId\":" + roomId + ",\"seat\":" + player.Seat + "}");
        }

        void HandleFightStart(ServerPlayer host, GameRoom room, string json)
        {
            int mapId = JI(json, "map", room.MapId);
            int seed = JI(json, "seed", Environment.TickCount);
            // Defaults match the client-side fallback bot stats.
            int[] atk = new int[] { 110, 110 };
            int[] def = new int[] { 85, 85 };
            int[] agi = new int[] { 70, 70 };
            int[] luck = new int[] { 40, 40 };
            int[] weaponId = new int[] { 7001, 7001 };
            int[] preferredBallId = new int[] { 0, 0 };
            lock (_lock)
            {
                room.MapId = mapId;
                room.InBattle = true;
                room.Seed = seed;
                room.CurrentTurn = 0;
                room.Wind = new System.Random(seed).Next(-3, 4) * 10;
                room.Hp = new int[room.PlayerIds.Count];
                room.MaxHp = new int[room.PlayerIds.Count];
                for (int i = 0; i < room.PlayerIds.Count; i++)
                {
                    if (_players.TryGetValue(room.PlayerIds[i], out ServerPlayer p))
                    {
                        p.RecalcStats(_db);
                        if (i >= 0 && i < 2)
                        {
                            atk[i] = p.Attack;
                            def[i] = p.Defence;
                            agi[i] = p.Agility;
                            luck[i] = p.Luck;
                            weaponId[i] = p.WeaponId;
                            preferredBallId[i] = p.PreferredBallId;
                        }
                        room.Hp[i] = p.Hp;
                        room.MaxHp[i] = p.Hp;
                    }
                    else
                    {
                        room.Hp[i] = 1200;
                        room.MaxHp[i] = 1200;
                    }
                }
            }
                room.Rng = new System.Random(seed);
                room.TurnStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                room.CurrentPlayer = 0;
            }
            int p0Hp = room.Hp != null && room.Hp.Length > 0 ? room.Hp[0] : 1200;
            int p0MaxHp = room.MaxHp != null && room.MaxHp.Length > 0 ? room.MaxHp[0] : p0Hp;
            int p1Hp = room.Hp != null && room.Hp.Length > 1 ? room.Hp[1] : 1200;
            int p1MaxHp = room.MaxHp != null && room.MaxHp.Length > 1 ? room.MaxHp[1] : p1Hp;

            string startJson = "{"
                + "\"map\":" + mapId
                + ",\"seed\":" + seed
                + ",\"wind\":" + room.Wind
                + ",\"p0_atk\":" + atk[0]
                + ",\"p0_def\":" + def[0]
                + ",\"p0_agi\":" + agi[0]
                + ",\"p0_luck\":" + luck[0]
                + ",\"p0_hp\":" + p0Hp
                + ",\"p0_maxhp\":" + p0MaxHp
                + ",\"p0_weaponId\":" + weaponId[0]
                + ",\"p0_preferredBallId\":" + preferredBallId[0]
                + ",\"p1_atk\":" + atk[1]
                + ",\"p1_def\":" + def[1]
                + ",\"p1_agi\":" + agi[1]
                + ",\"p1_luck\":" + luck[1]
                + ",\"p1_hp\":" + p1Hp
                + ",\"p1_maxhp\":" + p1MaxHp
                + ",\"p1_weaponId\":" + weaponId[1]
                + ",\"p1_preferredBallId\":" + preferredBallId[1]
                + "}";
            BroadcastToRoom(room, PhoneMsg.FightStart, startJson, -1);
        }

        void HandleFightDamage(ServerPlayer player, GameRoom room, string json)
        {
            int target = JI(json, "target", -1);
            int dmg = JI(json, "dmg", 0);
            dmg = Mathf.Clamp(dmg, 0, 9999);
            bool gameOver = false;
            lock (_lock)
            {
                if (target >= 0 && target < room.Hp.Length)
                {
                    room.Hp[target] = Mathf.Max(0, room.Hp[target] - dmg);
                    if (room.Hp[target] <= 0)
                    {
                        int alive = 0;
                        for (int i = 0; i < room.Hp.Length; i++)
                            if (room.Hp[i] > 0) alive++;
                        gameOver = alive <= 1;
                    }
                }
            }
            BroadcastToRoom(room, PhoneMsg.FightDamage, json, -1);
            if (gameOver)
            {
                AdvanceTurn(room);
            }
        }

        void AdvanceTurn(GameRoom room)
        {
            lock (_lock)
            {
                int alive = 0;
                for (int i = 0; i < room.Hp.Length; i++)
                    if (room.Hp[i] > 0) alive++;
                if (alive <= 1)
                {
                    room.InBattle = false;
                    return;
                }
                room.CurrentTurn++;
                int n = room.Hp.Length;
                for (int j = 1; j <= n; j++)
                {
                    int idx = (room.CurrentPlayer + j) % n;
                    if (room.Hp[idx] > 0)
                    {
                        room.CurrentPlayer = idx;
                        break;
                    }
                }
                room.Wind = room.Rng != null ? room.Rng.Next(-3, 4) * 10 : 0;
                room.TurnStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            string turnJson = "{\"turn\":" + room.CurrentTurn + ",\"player\":" + room.CurrentPlayer + ",\"wind\":" + room.Wind + "}";
            BroadcastToRoom(room, PhoneMsg.FightTurn, turnJson, -1);
        }

        void HandleFightOver(ServerPlayer player, GameRoom room, string json)
        {
            bool win = JI(json, "win", 0) == 1;
            int gold = win ? 800 : 100;
            lock (_lock)
            {
                if (win) { player.Win++; player.Gold += gold; }
                else { player.Lose++; player.Gold += gold; }
                player.Level = Mathf.Min(70, player.Level + (win ? 1 : 0));
                player.RecalcStats(_db);
                SavePlayer(player);
                room.InBattle = false;
            }
            SendFightTo(player, PhoneMsg.FightReward, "{\"gold\":" + gold + ",\"win\":" + (win ? "true" : "false") + "}");
            SendTo(player, PhoneMsg.ProfileData, player.ToJson());
        }

        void BroadcastToRoom(GameRoom room, ushort id, string json, int excludePlayerId)
        {
            lock (_lock)
            {
                foreach (int pid in room.PlayerIds)
                {
                    if (pid == excludePlayerId) continue;
                    if (_players.TryGetValue(pid, out ServerPlayer p))
                        SendFightTo(p, id, json);
                }
            }
        }

        ServerPlayer LoadOrCreate(string nick)
        {
            foreach (var p in _players.Values)
            {
                if (string.Equals(p.Nick, nick, StringComparison.OrdinalIgnoreCase))
                    return p;
            }

            string file = Path.Combine(_savePath, SanitizeFileName(nick) + ".json");
            if (File.Exists(file))
            {
                try
                {
                    var loaded = JsonUtility.FromJson<ServerPlayerSave>(File.ReadAllText(file));
                    if (loaded != null)
                    {
                        var p = FromSave(loaded);
                        p.Id = _nextPlayerId++;
                        _players[p.Id] = p;
                        return p;
                    }
                }
                catch { }
            }

            var fresh = new ServerPlayer
            {
                Id = _nextPlayerId++,
                Nick = nick,
                Bag = new List<BagSlot>
                {
                    new BagSlot { TemplateId = 7001, Count = 1 },
                    new BagSlot { TemplateId = 1102, Count = 1 },
                    new BagSlot { TemplateId = 5102, Count = 1 }
                }
            };
            _players[fresh.Id] = fresh;
            SavePlayer(fresh);
            return fresh;
        }

        void SavePlayer(ServerPlayer p)
        {
            try
            {
                string file = Path.Combine(_savePath, SanitizeFileName(p.Nick) + ".json");
                File.WriteAllText(file, JsonUtility.ToJson(ToSave(p), true));
            }
            catch { }
        }

        static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        static int JI(string json, string key, int fallback)
        {
            return Mathf.RoundToInt(JF(json, key, fallback));
        }

        static float JF(string json, string key, float fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, StringComparison.Ordinal);
            if (i < 0) return fallback;
            int s = i + needle.Length;
            int e = s;
            while (e < json.Length && (json[e] == '-' || json[e] == '.' || (json[e] >= '0' && json[e] <= '9'))) e++;
            if (float.TryParse(json.Substring(s, e - s), NumberStyles.Float, CultureInfo.InvariantCulture, out float v)) return v;
            return fallback;
        }

        static string JS(string json, string key, string fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":\"";
            int i = json.IndexOf(needle, StringComparison.Ordinal);
            if (i < 0) return fallback;
            int s = i + needle.Length;
            int e = json.IndexOf('"', s);
            return e > s ? json.Substring(s, e - s) : fallback;
        }

        [Serializable]
        class ServerPlayerSave
        {
            public string Nick = "Player";
            public int Sex = 1;
            public int Level = 20;
            public int Gold = 100000;
            public int Gift = 5000;
            public int Win, Lose;
            public int WeaponId = 7001;
            public int EquipHead, EquipHair, EquipFace, EquipCloth, EquipGlass, EquipWeapon = 7001;
            public int PetId, CardId, TitleId, TotemId, MountGrade, VipLevel, Honor, Texp;
            public int PreferredBallId, LastSignDay = -1, SignIndex, LabyrinthFloor = 1;
            public string ConsortiaName = "";
            public int ElfId, GemLevel;
            public List<BagSlotSave> Bag = new List<BagSlotSave>();
            public List<int> AcceptedQuests = new List<int>();
            public List<int> CompletedQuests = new List<int>();
            public List<string> Friends = new List<string>();
        }

        [Serializable]
        class BagSlotSave { public int t; public int c = 1; public int s; }

        static ServerPlayerSave ToSave(ServerPlayer p)
        {
            var s = new ServerPlayerSave
            {
                Nick = p.Nick, Sex = p.Sex, Level = p.Level, Gold = p.Gold, Gift = p.Gift,
                Win = p.Win, Lose = p.Lose, WeaponId = p.WeaponId,
                EquipHead = p.EquipHead, EquipHair = p.EquipHair, EquipFace = p.EquipFace,
                EquipCloth = p.EquipCloth, EquipGlass = p.EquipGlass, EquipWeapon = p.EquipWeapon,
                PetId = p.PetId, CardId = p.CardId, TitleId = p.TitleId, TotemId = p.TotemId,
                MountGrade = p.MountGrade, VipLevel = p.VipLevel, Honor = p.Honor, Texp = p.Texp,
                PreferredBallId = p.PreferredBallId, LastSignDay = p.LastSignDay, SignIndex = p.SignIndex,
                LabyrinthFloor = p.LabyrinthFloor, ConsortiaName = p.ConsortiaName,
                ElfId = p.ElfId, GemLevel = p.GemLevel,
                AcceptedQuests = p.AcceptedQuests, CompletedQuests = p.CompletedQuests,
                Friends = p.Friends
            };
            foreach (var b in p.Bag) s.Bag.Add(new BagSlotSave { t = b.TemplateId, c = b.Count, s = b.Strengthen });
            return s;
        }

        static ServerPlayer FromSave(ServerPlayerSave s)
        {
            var p = new ServerPlayer
            {
                Nick = s.Nick, Sex = s.Sex, Level = s.Level, Gold = s.Gold, Gift = s.Gift,
                Win = s.Win, Lose = s.Lose, WeaponId = s.WeaponId,
                EquipHead = s.EquipHead, EquipHair = s.EquipHair, EquipFace = s.EquipFace,
                EquipCloth = s.EquipCloth, EquipGlass = s.EquipGlass, EquipWeapon = s.EquipWeapon,
                PetId = s.PetId, CardId = s.CardId, TitleId = s.TitleId, TotemId = s.TotemId,
                MountGrade = s.MountGrade, VipLevel = s.VipLevel, Honor = s.Honor, Texp = s.Texp,
                PreferredBallId = s.PreferredBallId, LastSignDay = s.LastSignDay, SignIndex = s.SignIndex,
                LabyrinthFloor = s.LabyrinthFloor, ConsortiaName = s.ConsortiaName,
                ElfId = s.ElfId, GemLevel = s.GemLevel,
                AcceptedQuests = s.AcceptedQuests ?? new List<int>(),
                CompletedQuests = s.CompletedQuests ?? new List<int>(),
                Friends = s.Friends ?? new List<string>()
            };
            foreach (var b in s.Bag) p.Bag.Add(new BagSlot { TemplateId = b.t, Count = b.c, Strengthen = b.s });
            return p;
        }
    }
}
