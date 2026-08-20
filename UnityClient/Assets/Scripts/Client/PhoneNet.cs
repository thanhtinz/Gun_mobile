using System;
using System.Net;
using System.Net.Sockets;
using GunMobile.Net;
using UnityEngine;

namespace GunMobile.Client
{
    public static class PhoneNet
    {
        public static MobileGameServer GameServer { get; private set; }
        public static PhoneRoadServer LegacyServer { get; private set; }
        public static PhoneRoadClient Road { get; private set; }
        public static PhoneRoadClient Fight { get; private set; }
        public static bool NetBattle;
        public static int Seat;
        public static int PlayerId;
        public static string PeerHost = "127.0.0.1";
        public static int BattleSeed;
        public static int RoomId = -1;
        public static string LastRankJson;
        public static string LastRoomListJson;
        public static string LastMailListJson;
        public static string LastRoomStateJson;
        public static string LastAuctionListJson;
        public static string LastFriendListJson;
        public static string LastGuildJson;
        public static int PendingPveMapId;
        public static int PendingPveNpcId;

        static float _keepAliveT;

        public static void TickKeepAlive(float dt)
        {
            _keepAliveT -= dt;
            if (_keepAliveT > 0f) return;
            _keepAliveT = 15f;
            if (Road != null && Road.Connected) Road.Send(PhoneMsg.Ping, "{}");
            if (Fight != null && Fight.Connected) Fight.Send(PhoneMsg.Ping, "{}");
        }

        public static void RequestRank(string type = "gp")
        {
            Road?.Send(PhoneMsg.RankRequest, "{\"type\":\"" + (type ?? "gp").Replace("\"", "") + "\"}");
        }

        public static void RequestMailList()
        {
            Road?.Send(PhoneMsg.MailList, "{}");
        }

        public static void SetRoomReady(bool ready)
        {
            Road?.Send(PhoneMsg.RoomReady, "{\"ready\":" + (ready ? 1 : 0) + "}");
        }

        public static void LeaveRoom()
        {
            Road?.Send(PhoneMsg.RoomLeave, "{}");
            RoomId = -1;
        }

        public static void Boot()
        {
            Boot(null);
        }

        public static void Boot(GunMobile.Res.GameDatabase db, GunMobile.Res.ResLoader loader = null)
        {
            if (GameServer != null) return;

            GameServer = new MobileGameServer();
            GameServer.Start(db, loader);

            Road = new PhoneRoadClient();
            Fight = new PhoneRoadClient();
        }

        public static bool Login(string nick)
        {
            if (Road == null) Road = new PhoneRoadClient();
            if (!Road.Connected)
            {
                if (!Road.Connect("127.0.0.1", PhonePacket.RoadPort)) return false;
            }
            Road.Send(PhoneMsg.Login, "{\"nick\":\"" + (nick ?? "Player").Replace("\"", "") + "\"}");
            return true;
        }

        public static void Shutdown()
        {
            Fight?.Disconnect();
            Road?.Disconnect();
            GameServer?.Stop();
            LegacyServer?.Stop();
        }

        public static void EnsureConnected(string nick)
        {
            if (Road != null && Road.Connected) return;
            Login(nick);
        }

        public static void RequestProfile()
        {
            Road?.Send(PhoneMsg.GetProfile, "{}");
        }

        public static void ShopBuy(int offerId)
        {
            Road?.Send(PhoneMsg.ShopBuy, "{\"offerId\":" + offerId + "}");
        }

        public static void EquipItem(int templateId)
        {
            Road?.Send(PhoneMsg.EquipItem, "{\"templateId\":" + templateId + "}");
        }

        public static void QuestAccept(int questId)
        {
            Road?.Send(PhoneMsg.QuestAccept, "{\"questId\":" + questId + "}");
        }

        public static void QuestComplete(int questId)
        {
            Road?.Send(PhoneMsg.QuestComplete, "{\"questId\":" + questId + "}");
        }

        public static void SelectPet(int petId)
        {
            Road?.Send(PhoneMsg.PetSelect, "{\"petId\":" + petId + "}");
        }

        public static void SelectTitle(int titleId)
        {
            Road?.Send(PhoneMsg.TitleSelect, "{\"titleId\":" + titleId + "}");
        }

        public static void SelectCard(int cardId)
        {
            Road?.Send(PhoneMsg.CardSelect, "{\"cardId\":" + cardId + "}");
        }

        public static void BuyTotem(int totemId)
        {
            Road?.Send(PhoneMsg.TotemBuy, "{\"totemId\":" + totemId + "}");
        }

        public static void UpgradeMount()
        {
            Road?.Send(PhoneMsg.MountUpgrade, "{}");
        }

        public static void DoSignIn()
        {
            Road?.Send(PhoneMsg.SignIn, "{}");
        }

        public static void OpenGodCards(int count)
        {
            Road?.Send(PhoneMsg.GodCardOpen, "{\"count\":" + count + "}");
        }

        public static void EquipGodCard(int cardId)
        {
            Road?.Send(PhoneMsg.GodCardOpen, "{\"count\":0,\"equipId\":" + cardId + "}");
        }

        public static void EquipEngraveSet(int setId)
        {
            Road?.Send(PhoneMsg.EngraveEquip, "{\"setId\":" + setId + "}");
        }

        public static void TradeStock(string action, int stockId, int shares)
        {
            string act = (action ?? "buy").Replace("\"", "");
            Road?.Send(PhoneMsg.StockTrade, "{\"action\":\"" + act + "\",\"stockId\":" + stockId + ",\"shares\":" + shares + "}");
        }

        public static void DrawLottery(int count)
        {
            Road?.Send(PhoneMsg.LotteryDraw, "{\"count\":" + count + "}");
        }

        public static void StrengthenItem(int templateId)
        {
            Road?.Send(PhoneMsg.Strengthen, "{\"templateId\":" + templateId + "}");
        }

        public static void SelectBall(int ballId)
        {
            Road?.Send(PhoneMsg.BallSelect, "{\"ballId\":" + ballId + "}");
        }

        public static void UpgradeVip()
        {
            Road?.Send(PhoneMsg.VipUpgrade, "{}");
        }

        public static void TrainTexp()
        {
            Road?.Send(PhoneMsg.TexpTrain, "{}");
        }

        public static void UpgradeGem()
        {
            Road?.Send(PhoneMsg.GemUpgrade, "{}");
        }

        public static void UpgradeFightSpirit(int spiritId)
        {
            Road?.Send(PhoneMsg.GemSpiritUpgrade, "{\"spiritId\":" + spiritId + "}");
        }

        public static void UpgradeMagicStone(int templateId)
        {
            Road?.Send(PhoneMsg.MagicStoneUpgrade, "{\"templateId\":" + templateId + "}");
        }

        public static void MagicFusion(int fusionId)
        {
            Road?.Send(PhoneMsg.MagicFusion, "{\"fusionId\":" + fusionId + "}");
        }

        public static void BankTrade(string action, int amount)
        {
            Road?.Send(PhoneMsg.BankTrade, "{\"action\":\"" + action + "\",\"amount\":" + amount + "}");
        }

        public static void MineDig()
        {
            Road?.Send(PhoneMsg.MineDig, "{}");
        }

        public static void TeamDungeonStart(int shopType)
        {
            Road?.Send(PhoneMsg.TeamDungeonStart, "{\"shopType\":" + shopType + "}");
        }

        public static void TreasureDraw()
        {
            Road?.Send(PhoneMsg.TreasureDraw, "{}");
        }

        public static void CarnivalDraw()
        {
            Road?.Send(PhoneMsg.CarnivalDraw, "{}");
        }

        public static void PeakBattleStart(int rank)
        {
            Road?.Send(PhoneMsg.PeakBattleStart, "{\"rank\":" + rank + "}");
        }

        public static void WorldBossStart()
        {
            Road?.Send(PhoneMsg.WorldBossStart, "{}");
        }

        public static void UpgradeNecklace()
        {
            Road?.Send(PhoneMsg.NecklaceUpgrade, "{}");
        }

        public static void DevilTurnSpin(int count = 1)
        {
            Road?.Send(PhoneMsg.DevilTurnSpin, "{\"count\":" + count + "}");
        }

        public static void ClaimRedPacket()
        {
            Road?.Send(PhoneMsg.RedPacketClaim, "{}");
        }

        public static void UpgradeHomeTemple()
        {
            Road?.Send(PhoneMsg.HomeTempleUpgrade, "{}");
        }

        public static void SendMail(string to, int gold, string subject = null, string body = null)
        {
            Road?.Send(PhoneMsg.MailSend,
                "{\"to\":\"" + (to ?? "").Replace("\"", "") +
                "\",\"gold\":" + gold +
                ",\"subject\":\"" + (subject ?? "玩家邮件").Replace("\"", "") +
                "\",\"body\":\"" + (body ?? "").Replace("\"", "") + "\"}");
        }

        public static void SweepLabyrinth()
        {
            Road?.Send(PhoneMsg.SweepLabyrinth, "{}");
        }

        public static void ClaimFirstRecharge()
        {
            Road?.Send(PhoneMsg.FirstRechargeClaim, "{}");
        }

        public static void BuyFirstRechargeShop(int templateId, int count = 1)
        {
            Road?.Send(PhoneMsg.FirstRechargeShop,
                "{\"templateId\":" + templateId + ",\"count\":" + count + "}");
        }

        public static void CookFarm(int foodId)
        {
            Road?.Send(PhoneMsg.FarmCook, "{\"foodId\":" + foodId + "}");
        }

        public static void SellAuction(int templateId, int count = 1)
        {
            Road?.Send(PhoneMsg.AuctionSell, "{\"templateId\":" + templateId + ",\"count\":" + count + "}");
        }

        public static void ListAuction(int templateId, int price, int count = 1)
        {
            Road?.Send(PhoneMsg.AuctionSell,
                "{\"templateId\":" + templateId + ",\"count\":" + count + ",\"list\":1,\"price\":" + price + "}");
        }

        public static void RequestAuctionList()
        {
            Road?.Send(PhoneMsg.AuctionList, "{}");
        }

        public static void BuyAuction(int listingId)
        {
            Road?.Send(PhoneMsg.AuctionBuy, "{\"listingId\":" + listingId + "}");
        }

        public static void SelectElf(int elfId)
        {
            Road?.Send(PhoneMsg.ElfSelect, "{\"elfId\":" + elfId + "}");
        }

        public static void ClaimKingBless()
        {
            Road?.Send(PhoneMsg.KingBless, "{}");
        }

        public static void SetNick(string nick)
        {
            Road?.Send(PhoneMsg.SetNick, "{\"nick\":\"" + (nick ?? "").Replace("\"", "") + "\"}");
        }

        /// <summary>
        /// Online PvE: PveStart on road, then FightStart on fight socket (server adds NPC seat).
        /// </summary>
        public static bool BeginPveFight(int mapId, int npcId, bool labyrinth)
        {
            if (Road == null || !Road.Connected)
            {
                return false;
            }

            Road.Send(PhoneMsg.PveStart,
                "{\"npcId\":" + npcId +
                ",\"labyrinth\":" + (labyrinth ? "1" : "0") + "}");

            string host = string.IsNullOrWhiteSpace(PeerHost) ? "127.0.0.1" : PeerHost;
            Seat = 0;
            NetBattle = true;
            PendingPveMapId = mapId;
            PendingPveNpcId = npcId;

            if (Fight == null || !Fight.Connected)
            {
                if (!ConnectFight(host))
                {
                    NetBattle = false;
                    return false;
                }
            }

            SendStart(mapId);
            return true;
        }

        public static void JoinGuild(string name)
        {
            Road?.Send(PhoneMsg.GuildJoin, "{\"name\":\"" + (name ?? "").Replace("\"", "") + "\"}");
        }

        public static void CreateGuild(string name)
        {
            Road?.Send(PhoneMsg.GuildCreate, "{\"name\":\"" + (name ?? "").Replace("\"", "") + "\"}");
        }

        public static void LeaveGuild()
        {
            Road?.Send(PhoneMsg.GuildLeave, "{}");
        }

        public static void DonateGuild()
        {
            Road?.Send(PhoneMsg.GuildDonate, "{}");
        }

        public static void AddFriend(string name)
        {
            Road?.Send(PhoneMsg.FriendAdd, "{\"name\":\"" + (name ?? "").Replace("\"", "") + "\"}");
        }

        public static void RemoveFriend(string name)
        {
            Road?.Send(PhoneMsg.FriendRemove, "{\"name\":\"" + (name ?? "").Replace("\"", "") + "\"}");
        }

        public static void RefreshFriends()
        {
            Road?.Send(PhoneMsg.FriendAdd, "{\"name\":\"\"}");
        }

        public static void SendChat(string msg)
        {
            Road?.Send(PhoneMsg.ChatSend, "{\"msg\":\"" + (msg ?? "").Replace("\"", "") + "\"}");
        }

        public static void RequestRoomList()
        {
            Road?.Send(PhoneMsg.RoomList, "{}");
        }

        public static void CreateRoom(int mapId, string name, int maxPlayers = 4)
        {
            Road?.Send(PhoneMsg.CreateRoom,
                "{\"mapId\":" + mapId +
                ",\"name\":\"" + (name ?? "Room").Replace("\"", "") +
                "\",\"maxPlayers\":" + Mathf.Clamp(maxPlayers, 2, 4) + "}");
        }

        public static void JoinServerRoom(int roomId)
        {
            Road?.Send(PhoneMsg.JoinRoom, "{\"roomId\":" + roomId + "}");
        }

        public static void ReportDamage(int target, int dmg)
        {
            Fight?.Send(PhoneMsg.FightDamage, "{\"target\":" + target + ",\"dmg\":" + dmg + "}");
        }

        public static void SendFightTurn(int turn, int player, float wind)
        {
            if (Fight == null || !Fight.Connected) return;
            Fight.Send(
                PhoneMsg.FightTurn,
                "{\"turn\":" + turn +
                ",\"player\":" + player +
                ",\"wind\":" + wind.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "}");
        }

        public static void ReportFightOver(bool win)
        {
            Fight?.Send(PhoneMsg.FightOver, "{\"win\":" + (win ? "1" : "0") + "}");
        }

        public static void UseExternalServer()
        {
            try { GameServer?.Stop(); } catch { }
            GameServer = null;
        }

        public static bool ConnectHall(string host, string nick = null)
        {
            PeerHost = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host.Trim();
            Road?.Disconnect();
            Road = new PhoneRoadClient();
            bool ok = Road.Connect(PeerHost, PhonePacket.RoadPort);
            if (ok)
            {
                Road.Send(PhoneMsg.Login, "{\"nick\":\"" + (nick ?? "phone").Replace("\"", "") + "\"}");
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
                Fight.Send(PhoneMsg.JoinRoom, "{\"seat\":" + Seat + ",\"playerId\":" + PlayerId + "}");
            }

            return ok;
        }

        public static void SendPetSkill()
        {
            Fight?.Send(PhoneMsg.FightPetSkill, "{}");
        }

        public static void SendFire(int who, float angle, float power, int facing, int propId = 0, bool special = false)
        {
            if (Fight == null || !Fight.Connected)
            {
                return;
            }

            Fight.Send(
                PhoneMsg.FightFire,
                "{\"who\":" + who + ",\"angle\":" + angle.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                ",\"power\":" + power.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                ",\"facing\":" + facing +
                ",\"prop\":" + propId +
                ",\"special\":" + (special ? "1" : "0") + "}");
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
            string srv = GameServer != null && GameServer.Running
                ? "Server 开 " + GameServer.PlayerCount + "p " + GameServer.RoomCount + "r"
                : (Road != null && Road.Connected ? "Server Ext 已连" : "Server 关");
            string link = Fight != null && Fight.Connected ? " 已连" : "";
            string err = GameServer != null && !string.IsNullOrEmpty(GameServer.LastError) ? "  " + GameServer.LastError : "";
            return srv + link + err + "  IP " + LanIPv4() + "  id " + PlayerId;
        }
    }
}
