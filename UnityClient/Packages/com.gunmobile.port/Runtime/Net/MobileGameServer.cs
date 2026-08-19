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
        public bool FightPendingLose;
        public long FightDisconnectedAtMs;

        // PvE pending context
        public int PveNpcId;
        public int PveRewardGold;
        public bool PveLabyrinth;

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

        // Server-authoritative battle state
        public MapCollision Map;
        public LivingStats[] Livings;
        public BallPhysics[] Balls;
        public float[] PosX;
        public float[] PosY;
        public int[] Facing;

        // Server-authoritative props available for the current turn player.
        // Bit mapping uses propIds = [1,2,4,5,6,7] -> bits 0..5.
        public int CurrentPropMask;
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
        const bool BattleDebug = false;
        int _nextPlayerId = 1;
        int _nextRoomId = 1;
        TcpListener _road;
        TcpListener _fight;
        Thread _roadThread;
        Thread _fightThread;
        Thread _timerThread;
        volatile bool _run;
        GameDatabase _db;
        ResLoader _loader;
        System.Random _rng = new System.Random();
        string _savePath;

        public bool Running { get; private set; }
        public string LastError { get; private set; } = "";
        public int PlayerCount { get { lock (_lock) return _players.Count; } }
        public int RoomCount { get { lock (_lock) return _rooms.Count; } }

        public void Start(GameDatabase db, string savePath = null)
        {
            Start(db, null, savePath);
        }

        public void Start(GameDatabase db, ResLoader loader, string savePath = null)
        {
            if (Running) return;
            _db = db;
            _loader = loader;
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
            const long reconnectGraceMs = 30000; // allow quick Fight-socket reconnect
            const int tickMs = 200;

            while (_run)
            {
                try
                {
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    List<GameRoom> advance = null;
                    // Collect reconnect-expired disconnects and process outside _lock.
                    class LoseItem { public ServerPlayer Player; public GameRoom Room; }
                    List<LoseItem> toSurrender = null;

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

                        // Reconnect grace: if a player disconnected during battle and
                        // didn't come back in time, treat it as surrender.
                        foreach (var p in _players.Values)
                        {
                            if (p == null || !p.FightPendingLose) continue;
                            if (p.RoomId < 0) continue;
                            if (!_rooms.TryGetValue(p.RoomId, out var room)) continue;
                            if (room == null || !room.InBattle) continue;
                            if (p.FightDisconnectedAtMs <= 0) continue;
                            if (now - p.FightDisconnectedAtMs < reconnectGraceMs) continue;

                            toSurrender ??= new List<LoseItem>();
                            toSurrender.Add(new LoseItem { Player = p, Room = room });
                        }
                    }

                    if (advance != null)
                    {
                        foreach (var r in advance)
                        {
                            AdvanceTurn(r);
                        }
                    }

                    if (toSurrender != null)
                    {
                        foreach (var item in toSurrender)
                        {
                            if (item == null || item.Player == null || item.Room == null) continue;

                            lock (_lock)
                            {
                                if (!item.Player.FightPendingLose) continue;
                                if (item.Player.RoomId != item.Room.Id) continue;
                                item.Player.FightPendingLose = false;
                                item.Player.FightDisconnectedAtMs = 0;
                            }

                            // HandleSurrender() re-checks room state and will no-op if needed.
                            HandleSurrender(item.Player, item.Room);
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
                                int roomId = -1;
                                bool inBattle = false;
                                int turn = 0;
                                int currentPlayer = 0;
                                float wind = 0f;
                                int propMask = 0;
                                int[] hpArr = null;
                                int[] maxHpArr = null;
                                float[] posXArr = null;
                                int[] facingArr = null;
                                lock (_lock)
                                {
                                    if (_players.TryGetValue(playerId, out player))
                                    {
                                        player.FightTcp = client;
                                        player.FightStream = ns;
                                        player.FightPendingLose = false;
                                        player.FightDisconnectedAtMs = 0;

                                        if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out var room) && room.InBattle)
                                        {
                                            roomId = room.Id;
                                            inBattle = true;
                                            turn = room.CurrentTurn;
                                            currentPlayer = room.CurrentPlayer;
                                            wind = room.Wind;
                                            propMask = room.CurrentPropMask;
                                            hpArr = room.Hp != null ? (int[])room.Hp.Clone() : null;
                                            maxHpArr = room.MaxHp != null ? (int[])room.MaxHp.Clone() : null;
                                            posXArr = room.PosX != null ? (float[])room.PosX.Clone() : null;
                                            facingArr = room.Facing != null ? (int[])room.Facing.Clone() : null;
                                        }
                                    }
                                }
                                Send(ns, PhoneMsg.RoomOk, "{\"ok\":true}");

                                // Help the reconnecting client re-sync quickly.
                                if (inBattle)
                                {
                                    string turnJson = "{\"turn\":" + turn +
                                                      ",\"player\":" + currentPlayer +
                                                      ",\"wind\":" + wind.ToString(CultureInfo.InvariantCulture) + "}";
                                    Send(ns, PhoneMsg.FightTurn, turnJson);

                                    string propJson = "{\"player\":" + currentPlayer +
                                                       ",\"mask\":" + propMask + "}";
                                    Send(ns, PhoneMsg.FightProp, propJson);

                                    // State snapshot: HP + x + facing, so reconnect can resume close to server state.
                                    int pc = hpArr != null ? hpArr.Length : 0;
                                    if (pc > 0 && posXArr != null && facingArr != null && maxHpArr != null)
                                    {
                                        var sb = new StringBuilder(512);
                                        sb.Append("{\"playerCount\":").Append(pc);
                                        sb.Append(",\"turn\":").Append(turn);
                                        sb.Append(",\"player\":").Append(currentPlayer);
                                        sb.Append(",\"wind\":").Append(wind.ToString(CultureInfo.InvariantCulture));

                                        for (int i = 0; i < pc; i++)
                                        {
                                            sb.Append(",\"p").Append(i).Append("_hp\":").Append(hpArr[i]);
                                            sb.Append(",\"p").Append(i).Append("_maxhp\":").Append(maxHpArr[i]);
                                            sb.Append(",\"p").Append(i).Append("_x\":").Append(posXArr[i].ToString(CultureInfo.InvariantCulture));
                                            sb.Append(",\"p").Append(i).Append("_facing\":").Append(facingArr[i]);
                                        }
                                        sb.Append("}");
                                        Send(ns, PhoneMsg.FightState, sb.ToString());
                                    }
                                }
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
                    // Reconnect grace:
                    // if the client disconnects while in a battle, wait a while and only
                    // surrender if they don't reconnect in time.
                    GameRoom dcRoom = null;
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    lock (_lock)
                    {
                        player.FightTcp = null;
                        player.FightStream = null;
                        if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out dcRoom))
                        {
                            if (dcRoom.InBattle)
                            {
                                player.FightPendingLose = true;
                                player.FightDisconnectedAtMs = now;
                            }
                            else
                            {
                                player.FightPendingLose = false;
                                player.FightDisconnectedAtMs = 0;
                                dcRoom = null;
                            }
                        }
                    }
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
                {
                    string gName = JS(json, "name", "");
                    if (!string.IsNullOrEmpty(gName))
                    {
                        player.ConsortiaName = gName;
                        SavePlayer(player);
                    }
                    // Return guild info with member list
                    var gMembers = new StringBuilder();
                    lock (_lock)
                    {
                        int gm = 0;
                        foreach (var p in _players.Values)
                        {
                            if (string.Equals(p.ConsortiaName, player.ConsortiaName, StringComparison.OrdinalIgnoreCase))
                            {
                                if (gm > 0) gMembers.Append(",");
                                gMembers.Append("{\"nick\":\"").Append((p.Nick ?? "").Replace("\"", ""))
                                    .Append("\",\"level\":").Append(p.Level)
                                    .Append(",\"online\":").Append(p.RoadStream != null ? "true" : "false")
                                    .Append("}");
                                gm++;
                            }
                        }
                    }
                    Send(ns, PhoneMsg.GuildResult, "{\"ok\":true,\"name\":\"" + (player.ConsortiaName ?? "").Replace("\"", "") + "\",\"members\":[" + gMembers + "]}");
                    break;
                }

                case PhoneMsg.GuildDonate:
                    if (player.Gold >= 1000 && !string.IsNullOrEmpty(player.ConsortiaName))
                    {
                        player.Gold -= 1000;
                        player.Honor += 80;
                        SavePlayer(player);
                    }
                    Send(ns, PhoneMsg.GuildResult, "{\"ok\":true}");
                    Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;

                case PhoneMsg.FriendAdd:
                {
                    string fn = JS(json, "name", "");
                    bool friendFound = false;
                    if (!string.IsNullOrEmpty(fn) && !player.Friends.Contains(fn))
                    {
                        player.Friends.Add(fn);
                        SavePlayer(player);
                        // Mutual: add this player to the friend's list too
                        lock (_lock)
                        {
                            foreach (var fp in _players.Values)
                            {
                                if (string.Equals(fp.Nick, fn, StringComparison.OrdinalIgnoreCase))
                                {
                                    friendFound = true;
                                    if (!fp.Friends.Contains(player.Nick))
                                    {
                                        fp.Friends.Add(player.Nick);
                                        SavePlayer(fp);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    // Return friend list
                    var fl = new StringBuilder();
                    for (int fi = 0; fi < player.Friends.Count; fi++)
                    {
                        if (fi > 0) fl.Append(",");
                        string fname = player.Friends[fi];
                        bool online = false;
                        lock (_lock)
                        {
                            foreach (var fp in _players.Values)
                            {
                                if (string.Equals(fp.Nick, fname, StringComparison.OrdinalIgnoreCase))
                                {
                                    online = fp.RoadStream != null;
                                    break;
                                }
                            }
                        }
                        fl.Append("{\"nick\":\"").Append(fname.Replace("\"", ""))
                          .Append("\",\"online\":").Append(online ? "true" : "false").Append("}");
                    }
                    Send(ns, PhoneMsg.FriendResult, "{\"ok\":true,\"found\":" + (friendFound ? "true" : "false") + ",\"friends\":[" + fl + "]}");
                    break;
                }

                case PhoneMsg.MailClaim:
                {
                    int mailGold = 0;
                    int mailId = JI(json, "id", 0);
                    if (mailId == 1)
                    {
                        mailGold = 500;
                        player.Gold += mailGold;
                        SavePlayer(player);
                    }
                    Send(ns, PhoneMsg.MailResult, "{\"ok\":true,\"gold\":" + mailGold + "}");
                    if (mailGold > 0)
                        Send(ns, PhoneMsg.ProfileData, player.ToJson());
                    break;
                }

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

                case PhoneMsg.RankRequest:
                    HandleRankRequest(player, ns);
                    break;

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

                case PhoneMsg.PveStart:
                    player.PveNpcId = JI(json, "npcId", 0);
                    player.PveRewardGold = JI(json, "reward", 0);
                    player.PveLabyrinth = JI(json, "labyrinth", 0) != 0;
                    Send(ns, PhoneMsg.PveResult, "{\"ok\":true}");
                    break;

                case PhoneMsg.Ping:
                    Send(ns, PhoneMsg.Ping, "{}");
                    break;
            }
        }

        void HandleFightMsg(ServerPlayer player, NetworkStream ns, ushort id, string json)
        {
            GameRoom room = null;
            List<int> assignedPlayers = null;

            lock (_lock)
            {
                // Normal flow: player must already belong to a room (RoomId set via road join/create).
                if (id != PhoneMsg.FightStart)
                {
                    if (player.RoomId < 0 || !_rooms.TryGetValue(player.RoomId, out room)) return;
                }
                else
                {
                    // Host can press "start fight" without explicit road-room join:
                    // auto-create a room and absorb other "waiting" fight clients.
                    if (player.RoomId >= 0 && _rooms.TryGetValue(player.RoomId, out room))
                    {
                        // already in room
                    }
                    else
                    {
                        int mapId = JI(json, "map", 1056);
                        int maxPlayers = 4;
                        room = new GameRoom
                        {
                            Id = _nextRoomId++,
                            MapId = mapId,
                            Name = (player.Nick ?? "Player") + "'s Room",
                            MaxPlayers = maxPlayers
                        };

                        player.RoomId = room.Id;
                        player.Seat = 0;
                        room.PlayerIds.Add(player.Id);

                        // Auto-assign up to maxPlayers-1 waiting clients that already connected to fight socket.
                        foreach (var p in _players.Values)
                        {
                            if (room.PlayerIds.Count >= room.MaxPlayers) break;
                            if (p == player) continue;
                            if (p.RoomId >= 0) continue; // already in a room
                            if (p.FightTcp == null) continue; // not connected to fight yet

                            p.RoomId = room.Id;
                            p.Seat = room.PlayerIds.Count;
                            room.PlayerIds.Add(p.Id);
                        }

                        _rooms[room.Id] = room;
                        assignedPlayers = new List<int>(room.PlayerIds);
                    }
                }
            }

            // Notify clients about their assigned seat (RoomOk is consumed from the ROAD socket).
            if (assignedPlayers != null)
            {
                foreach (int pid in assignedPlayers)
                {
                    if (_players.TryGetValue(pid, out var p))
                    {
                        SendTo(p, PhoneMsg.RoomOk, "{\"roomId\":" + room.Id + ",\"seat\":" + p.Seat + "}");
                    }
                }
            }

            switch (id)
            {
                case PhoneMsg.FightStart:
                    HandleFightStart(player, room, json);
                    break;

                case PhoneMsg.FightWalk:
                {
                    bool allowW;
                    lock (_lock) { allowW = room.InBattle && player.Seat == room.CurrentPlayer; }
                    if (!allowW) return;
                    float wx = JF(json, "x", room.PosX[player.Seat]);
                    int wf = JI(json, "facing", room.Facing[player.Seat]);
                    lock (_lock)
                    {
                        room.PosX[player.Seat] = wx;
                        room.Facing[player.Seat] = wf >= 0 ? 1 : -1;
                        if (room.Map != null)
                            room.PosY[player.Seat] = room.Map.FindStandY(Mathf.Clamp(Mathf.RoundToInt(wx), 0, room.Map.Width - 1), 0);
                    }
                    BroadcastToRoom(room, id, json, player.Id);
                    break;
                }

                case PhoneMsg.FightFire:
                {
                    bool allowF;
                    lock (_lock) { allowF = room.InBattle && player.Seat == room.CurrentPlayer; }
                    if (!allowF) return;
                    BroadcastToRoom(room, PhoneMsg.FightFire, json, player.Id);
                    ServerSimulateFire(player, room, json);
                    break;
                }

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
                    // Server-authoritative: ignore client-reported damage.
                    // Damage is computed by ServerSimulateFire().
                    break;

                case PhoneMsg.FightOver:
                    HandleFightOver(player, room, json);
                    break;

                case PhoneMsg.FightSurrender:
                    HandleSurrender(player, room);
                    break;
            }
        }

        void HandleSurrender(ServerPlayer player, GameRoom room)
        {
            lock (_lock)
            {
                if (!room.InBattle) return;
                int seat = player.Seat;
                if (seat >= 0 && seat < room.Hp.Length)
                {
                    room.Hp[seat] = 0;
                    if (room.Livings != null && seat < room.Livings.Length)
                    {
                        var ls = room.Livings[seat];
                        ls.Hp = 0;
                        room.Livings[seat] = ls;
                    }
                }
            }
            string dmgJson = "{\"target\":" + player.Seat + ",\"dmg\":9999,\"crit\":false,\"surrender\":true}";
            BroadcastToRoom(room, PhoneMsg.FightDamage, dmgJson, -1);
            // Trigger game over check
            bool gameOver;
            lock (_lock)
            {
                int alive = 0;
                for (int i = 0; i < room.Hp.Length; i++)
                    if (room.Hp[i] > 0) alive++;
                gameOver = alive <= 1;
            }
            if (gameOver)
            {
                room.InBattle = false;
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

        void HandleRankRequest(ServerPlayer player, NetworkStream ns)
        {
            var sorted = new List<ServerPlayer>();
            lock (_lock)
            {
                sorted.AddRange(_players.Values);
            }
            sorted.Sort((a, b) => b.Win.CompareTo(a.Win));
            var sb = new StringBuilder("{\"ranks\":[");
            int count = Mathf.Min(50, sorted.Count);
            for (int i = 0; i < count; i++)
            {
                if (i > 0) sb.Append(",");
                var p = sorted[i];
                sb.Append("{\"nick\":\"").Append((p.Nick ?? "").Replace("\"", ""))
                  .Append("\",\"level\":").Append(p.Level)
                  .Append(",\"win\":").Append(p.Win)
                  .Append(",\"lose\":").Append(p.Lose)
                  .Append(",\"vip\":").Append(p.VipLevel)
                  .Append(",\"honor\":").Append(p.Honor)
                  .Append("}");
            }
            sb.Append("]}");
            Send(ns, PhoneMsg.RankData, sb.ToString());
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
            int maxPlayers = Mathf.Clamp(JI(json, "maxPlayers", 2), 2, 4);
            GameRoom room;
            lock (_lock)
            {
                room = new GameRoom { Id = _nextRoomId++, MapId = mapId, Name = name, MaxPlayers = maxPlayers };
                room.PlayerIds.Add(player.Id);
                player.RoomId = room.Id;
                player.Seat = 0;
                _rooms[room.Id] = room;
            }
            Send(ns, PhoneMsg.RoomCreated, "{\"roomId\":" + room.Id + ",\"seat\":0,\"maxPlayers\":" + maxPlayers + "}");
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
            int n;
            lock (_lock)
            {
                room.MapId = mapId;
                room.InBattle = true;
                room.Seed = seed;
                room.CurrentTurn = 0;
                room.Wind = new System.Random(seed).Next(-3, 4) * 10;
                n = room.PlayerIds.Count;
                room.Hp = new int[n];
                room.MaxHp = new int[n];
                room.Livings = new LivingStats[n];
                room.Balls = new BallPhysics[n];
                room.PosX = new float[n];
                room.PosY = new float[n];
                room.Facing = new int[n];
                for (int i = 0; i < n; i++)
                {
                    // Team: even seats = team 1, odd seats = team 2
                    int team = (i % 2) + 1;
                    room.Balls[i] = BallPhysics.Default;
                    if (_players.TryGetValue(room.PlayerIds[i], out ServerPlayer p))
                    {
                        p.RecalcStats(_db);
                        room.Hp[i] = p.Hp;
                        room.MaxHp[i] = p.Hp;
                        room.Livings[i] = new LivingStats
                        {
                            Attack = p.Attack, Defence = p.Defence,
                            Agility = p.Agility, Luck = p.Luck,
                            Hp = p.Hp, MaxHp = p.Hp, Team = team
                        };
                        if (_db != null)
                        {
                            int bid = p.PreferredBallId > 0 ? p.PreferredBallId : _db.DefaultBallId(p.WeaponId);
                            room.Balls[i] = _db.GetBall(bid);
                        }
                    }
                    else
                    {
                        room.Hp[i] = 1200;
                        room.MaxHp[i] = 1200;
                        room.Livings[i] = new LivingStats
                        {
                            Attack = 110, Defence = 85, Agility = 70, Luck = 40,
                            Hp = 1200, MaxHp = 1200, Team = team
                        };
                    }
                }
                room.Rng = new System.Random(seed);
                room.TurnStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                room.CurrentPlayer = 0;
                room.CurrentPropMask = GeneratePropMask(room);

                room.Map = null;
                if (_loader != null)
                {
                    string mapPath = GamePaths.MapCollision(mapId);
                    if (_loader.TryReadBytes(mapPath, out byte[] mapBytes))
                    {
                        try { room.Map = MapCollision.Load(mapBytes); }
                        catch { }
                    }
                }

                int mapW = room.Map != null ? room.Map.Width : 1250;
                // Spread positions evenly across the map
                for (int i = 0; i < n; i++)
                {
                    float frac = n <= 1 ? 0.1f : (float)i / (n - 1);
                    int px = Mathf.RoundToInt(Mathf.Lerp(140f, mapW - 160f, frac));
                    room.PosX[i] = px;
                    room.PosY[i] = room.Map != null ? room.Map.FindStandY(Mathf.Clamp(px, 0, mapW - 1), 0) : 0f;
                    room.Facing[i] = frac < 0.5f ? 1 : -1;
                }
            }

            // Build dynamic FightStart JSON with per-player stats
            var sb = new StringBuilder("{\"map\":").Append(mapId)
                .Append(",\"seed\":").Append(seed)
                .Append(",\"wind\":").Append(room.Wind)
                .Append(",\"playerCount\":").Append(n);
            for (int i = 0; i < n; i++)
            {
                string p = "p" + i + "_";
                var ls = room.Livings[i];
                sb.Append(",\"").Append(p).Append("atk\":").Append(ls.Attack);
                sb.Append(",\"").Append(p).Append("def\":").Append(ls.Defence);
                sb.Append(",\"").Append(p).Append("agi\":").Append(ls.Agility);
                sb.Append(",\"").Append(p).Append("luck\":").Append(ls.Luck);
                sb.Append(",\"").Append(p).Append("hp\":").Append(room.Hp[i]);
                sb.Append(",\"").Append(p).Append("maxhp\":").Append(room.MaxHp[i]);
                sb.Append(",\"").Append(p).Append("team\":").Append(ls.Team);

                // Weapon/ball info
                int wid = 7001, ballId = 0;
                lock (_lock)
                {
                    if (i < room.PlayerIds.Count && _players.TryGetValue(room.PlayerIds[i], out ServerPlayer sp))
                    {
                        wid = sp.WeaponId;
                        ballId = sp.PreferredBallId;
                    }
                }
                sb.Append(",\"").Append(p).Append("weaponId\":").Append(wid);
                sb.Append(",\"").Append(p).Append("preferredBallId\":").Append(ballId);
            }
            sb.Append("}");
            BroadcastToRoom(room, PhoneMsg.FightStart, sb.ToString(), -1);

            // Push current turn available props to clients.
            string propJson = "{\"player\":" + room.CurrentPlayer + ",\"mask\":" + room.CurrentPropMask + "}";
            BroadcastToRoom(room, PhoneMsg.FightProp, propJson, -1);
            if (BattleDebug)
            {
                Debug.Log($"[Battle] FightStart room={room.Id} curPlayer={room.CurrentPlayer} propMask={room.CurrentPropMask}");
            }
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

        static void ApplyPropModifiers(int propId, out float dmgMul, out float radiusMul, out float powerAdd, out bool forceCrit)
        {
            dmgMul = 1f; radiusMul = 1f; powerAdd = 0f; forceCrit = false;
            switch (propId)
            {
                case 1: dmgMul = 1.25f; radiusMul = 1.35f; break;
                case 2: dmgMul = 1.2f; break;
                case 5: powerAdd = 12f; break;
                case 6: dmgMul = 1.4f; break;
                case 7: forceCrit = true; break;
            }
        }

        static int PropBitIndex(int propId)
        {
            // propIds order must match client prop buttons:
            // [1,2,4,5,6,7] -> bits [0..5]
            switch (propId)
            {
                case 1: return 0;
                case 2: return 1;
                case 4: return 2;
                case 5: return 3;
                case 6: return 4;
                case 7: return 5;
                default: return -1;
            }
        }

        int GeneratePropMask(GameRoom room)
        {
            // Randomly choose 3 props among [1,2,4,5,6,7].
            int mask = 0;
            int[] pool = new int[] { 1, 2, 4, 5, 6, 7 };
            // Fisher-Yates shuffle (only need first 3)
            for (int i = 0; i < 3; i++)
            {
                int j = room.Rng != null ? room.Rng.Next(i, pool.Length) : i;
                int tmp = pool[i];
                pool[i] = pool[j];
                pool[j] = tmp;
                int bit = PropBitIndex(pool[i]);
                if (bit >= 0) mask |= 1 << bit;
            }
            return mask;
        }

        void ServerSimulateFire(ServerPlayer player, GameRoom room, string json)
        {
            int who = player.Seat;
            float angle = JF(json, "angle", 45f);
            float power = JF(json, "power", 50f);
            int facing = JI(json, "facing", room.Facing[who]);
            int propId = JI(json, "prop", 0);
            int rawPropId = propId;

            // Server validates propId based on the props available for the current turn player.
            // If not available, treat as no-prop (propId=0).
            int propMask;
            lock (_lock) { propMask = room.CurrentPropMask; }
            if (propId != 0)
            {
                int bit = PropBitIndex(propId);
                if (bit < 0 || (propMask & (1 << bit)) == 0)
                {
                    propId = 0;
                }
            }

            if (BattleDebug)
            {
                Debug.Log($"[Battle] Fire seat={who} turn={room.CurrentTurn} propMask={propMask} rawProp={rawPropId} usedProp={propId}");
            }

            ApplyPropModifiers(propId, out float propDmg, out float propRadius, out float propPower, out bool propCrit);
            power = Mathf.Clamp(power + propPower, 1f, 100f);

            MapCollision map;
            BallPhysics ball;
            float wind;
            float startX, startY;
            LivingStats[] livings;
            float[] posX, posY;
            int[] hp;
            lock (_lock)
            {
                map = room.Map;
                ball = (room.Balls != null && who < room.Balls.Length) ? room.Balls[who] : BallPhysics.Default;
                wind = room.Wind;
                startX = room.PosX[who];
                startY = room.PosY[who];
                livings = (LivingStats[])room.Livings.Clone();
                posX = (float[])room.PosX.Clone();
                posY = (float[])room.PosY.Clone();
                hp = (int[])room.Hp.Clone();
                room.Facing[who] = facing >= 0 ? 1 : -1;
            }

            if (map == null) return;

            var sim = new ProjectileSimulator();
            sim.ApplyBall(ball);

            int mapH = map.Height;
            int mapW = map.Width;
            float unityY = mapH - startY - 18f;

            int shotCount = Mathf.Max(1, ball.Amount);
            int blastRadius = Mathf.Max(20, Mathf.RoundToInt(ball.Radii * propRadius));

            for (int s = 0; s < shotCount; s++)
            {
                float spreadX = s == 0 ? 0f : (room.Rng != null ? (float)(room.Rng.NextDouble() * 16.0 - 8.0) : 0f);
                float spreadA = s == 0 ? 0f : (room.Rng != null ? (float)(room.Rng.NextDouble() * 10.0 - 5.0) : 0f);
                float spreadP = s == 0 ? 0f : (room.Rng != null ? (float)(room.Rng.NextDouble() * 12.0 - 6.0) : 0f);

                var state = sim.FlyUntil(
                    sim.Launch(startX + spreadX, unityY, angle + spreadA, Mathf.Clamp(power + spreadP, 1f, 100f), facing >= 0 ? 1 : -1),
                    wind,
                    (fx, fy) =>
                    {
                        int mx = Mathf.RoundToInt(fx);
                        int my = mapH - 1 - Mathf.RoundToInt(fy);
                        return map.IsSolid(mx, my);
                    },
                    (fx, fy) =>
                    {
                        int mx = Mathf.RoundToInt(fx);
                        int my = mapH - 1 - Mathf.RoundToInt(fy);
                        return mx < -200 || mx > mapW + 200 || my > mapH + 200;
                    },
                    12f);

                int hitMapX = Mathf.RoundToInt(state.X);
                int hitMapY = mapH - 1 - Mathf.RoundToInt(state.Y);

                map.CutCircle(hitMapX, hitMapY, blastRadius / 3);

                int bombHurt = 80 + Mathf.RoundToInt(Mathf.Abs(ball.Power) * 80f);
                if (bombHurt < 40) bombHurt = 140;
                bombHurt = Mathf.RoundToInt(bombHurt * propDmg);

                for (int t = 0; t < hp.Length; t++)
                {
                    if (hp[t] <= 0) continue;
                    float tx = posX[t];
                    float ty = posY[t];
                    float dx = hitMapX - tx;
                    float dy = hitMapY - ty;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > blastRadius) continue;

                    bool crit = propCrit || DamageCalculator.RollCrit(livings[who].Luck, who + (room.CurrentTurn + s));
                    int dmg = DamageCalculator.Compute(livings[who], livings[t], bombHurt, dist, crit);
                    dmg = Mathf.Clamp(dmg, 0, hp[t]);

                    lock (_lock)
                    {
                        room.Hp[t] = Mathf.Max(0, room.Hp[t] - dmg);
                        if (room.Livings != null && t < room.Livings.Length)
                        {
                            var ls = room.Livings[t];
                            ls.Hp = room.Hp[t];
                            room.Livings[t] = ls;
                        }
                    }
                    hp[t] = Mathf.Max(0, hp[t] - dmg);

                    string dmgJson = "{\"target\":" + t + ",\"dmg\":" + dmg + ",\"crit\":" + (crit ? "true" : "false") + "}";
                    BroadcastToRoom(room, PhoneMsg.FightDamage, dmgJson, -1);
                }
            }

            bool gameOver;
            lock (_lock)
            {
                gameOver = CountAliveTeams(room) <= 1;
            }
            if (gameOver)
            {
                AdvanceTurn(room);
            }
        }

        int CountAliveTeams(GameRoom room)
        {
            var teams = new HashSet<int>();
            for (int i = 0; i < room.Hp.Length; i++)
            {
                if (room.Hp[i] > 0 && room.Livings != null && i < room.Livings.Length)
                    teams.Add(room.Livings[i].Team);
            }
            return teams.Count;
        }

        void AdvanceTurn(GameRoom room)
        {
            lock (_lock)
            {
                if (CountAliveTeams(room) <= 1)
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
                room.CurrentPropMask = GeneratePropMask(room);
            }
            string turnJson = "{\"turn\":" + room.CurrentTurn + ",\"player\":" + room.CurrentPlayer + ",\"wind\":" + room.Wind + "}";
            BroadcastToRoom(room, PhoneMsg.FightTurn, turnJson, -1);

            string propJson = "{\"player\":" + room.CurrentPlayer + ",\"mask\":" + room.CurrentPropMask + "}";
            BroadcastToRoom(room, PhoneMsg.FightProp, propJson, -1);
            if (BattleDebug)
            {
                Debug.Log($"[Battle] FightTurn room={room.Id} turn={room.CurrentTurn} curPlayer={room.CurrentPlayer} propMask={room.CurrentPropMask}");
            }
        }

        void HandleFightOver(ServerPlayer player, GameRoom room, string json)
        {
            // Server authoritative: ignore client-provided win flag.
            int seat = player.Seat;
            bool win = room != null && room.Hp != null && seat >= 0 && seat < room.Hp.Length && room.Hp[seat] > 0;
            int gold = win ? 800 : 100;
            lock (_lock)
            {
                // PvE bonus
                if (win && player.PveRewardGold > 0)
                {
                    gold += player.PveRewardGold;
                }
                if (win && player.PveLabyrinth)
                {
                    player.LabyrinthFloor++;
                }
                player.PveNpcId = 0;
                player.PveRewardGold = 0;
                player.PveLabyrinth = false;

                if (win) { player.Win++; player.Gold += gold; player.Honor += 4; }
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
