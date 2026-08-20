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
        public static string LastSpaRoomJson;
        public static string LastTreasureRoomJson;
        public static string LastChristmasJson;
        public static string LastNewYearJson;
        public static string LastWorshipMoonJson;
        public static string LastSuperLuckerJson;
        public static string LastCalendarJson;
        public static string LastDailyAwardJson;
        public static string LastButterflyJson;
        public static string LastAchievementJson;
        public static string LastLinkPalJson;
        public static string LastMountSkillJson;
        public static string LastQuizJson;
        public static string LastOneYuanJson;
        public static string LastAuditoriumJson;
        public static string LastBoguAdventureJson;
        public static string LastJigsawJson;
        public static string LastBibleJson;
        public static string LastQuizJson;
        public static string LastOneYuanJson;
        public static string LastActivityQuestJson;
        public static string LastSwornJson;
        public static string LastVipStoreJson;
        public static string LastPairUpJson;
        public static string LastShopShowJson;
        public static string LastStockNoticeJson;
        public static string LastJewelJson;
        public static string LastWarPassJson;
        public static string LastTimeLimitShopJson;
        public static string LastBattleTeamJson;
        public static string LastBattleTeamShopJson;
        public static string LastDailyLeagueJson;
        public static string LastScrollJson;
        public static string LastSigilSkillJson;
        public static string LastConsortiaBufferJson;
        public static string LastElfSkillBookJson;
        public static string LastButterflyTaskJson;
        public static string LastChargeSpendJson;
        public static string LastBuffActivateJson;
        public static string LastTotemInfoSyncJson;
        public static string LastActiveListJson;
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

        public static void UpgradePetStar()
        {
            Road?.Send(PhoneMsg.PetStarUpgrade, "{}");
        }

        public static void EquipMountTalisman(int talismanId)
        {
            Road?.Send(PhoneMsg.MountTalismanEquip, "{\"talismanId\":" + talismanId + "}");
        }

        public static void UpgradeManor()
        {
            Road?.Send(PhoneMsg.ManorUpgrade, "{}");
        }

        public static void UpgradeGoldEquip(int oldTemplateId)
        {
            Road?.Send(PhoneMsg.GoldEquipUpgrade, "{\"oldTemplateId\":" + oldTemplateId + "}");
        }

        public static void UpgradeGlory(int templateId)
        {
            Road?.Send(PhoneMsg.GloryUpgrade, "{\"templateId\":" + templateId + "}");
        }

        public static void RollSigil(int quality)
        {
            Road?.Send(PhoneMsg.SigilRoll, "{\"quality\":" + quality + "}");
        }

        public static void UnlockMountSkill(int skillId)
        {
            Road?.Send(PhoneMsg.MountSkillUnlock, "{\"skillId\":" + skillId + "}");
        }

        public static void ClaimAchievement(int achievementId)
        {
            Road?.Send(PhoneMsg.AchievementClaim, "{\"achievementId\":" + achievementId + "}");
        }

        public static void LinkPalEquip(int id)
        {
            Road?.Send(PhoneMsg.LinkPalAction, "{\"action\":\"equip\",\"id\":" + id + "}");
        }

        public static void LinkPalUpgrade()
        {
            Road?.Send(PhoneMsg.LinkPalAction, "{\"action\":\"upgrade\"}");
        }

        public static void EquipJade(int jadeId)
        {
            Road?.Send(PhoneMsg.JadeEquip, "{\"jadeId\":" + jadeId + "}");
        }

        public static void EquipRune(int templateId)
        {
            Road?.Send(PhoneMsg.RuneEquip, "{\"templateId\":" + templateId + "}");
        }

        public static void UpgradeHorseAmulet(string mode = "level")
        {
            if (string.IsNullOrEmpty(mode)) mode = "level";
            Road?.Send(PhoneMsg.HorseAmuletUpgrade, "{\"mode\":\"" + mode + "\"}");
        }

        public static void QuizAnswer(int questionId, int option)
        {
            Road?.Send(PhoneMsg.QuizAnswer, "{\"questionId\":" + questionId + ",\"option\":" + option + "}");
        }

        public static void OneYuanBuy(int id, int goodsId)
        {
            Road?.Send(PhoneMsg.OneYuanBuy, "{\"id\":" + id + ",\"goodsId\":" + goodsId + "}");
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

        public static void RaiseGodCard(int cardId, int count = 1)
        {
            Road?.Send(PhoneMsg.GodCardRaise, "{\"cardId\":" + cardId + ",\"count\":" + count + "}");
        }

        public static void ClaimGodCardPoint(int rewardId)
        {
            Road?.Send(PhoneMsg.GodCardPointClaim, "{\"rewardId\":" + rewardId + "}");
        }

        public static void SendFightSkip(int who)
        {
            Fight?.Send(PhoneMsg.FightSkip, "{\"who\":" + who + "}");
        }

        public static void EquipEngraveSet(int setId)
        {
            Road?.Send(PhoneMsg.EngraveEquip, "{\"setId\":" + setId + "}");
        }

        public static void EngraveDebris(int debrisId, string action = "apply")
        {
            if (string.IsNullOrEmpty(action)) action = "apply";
            Road?.Send(PhoneMsg.EngraveDebrisAction, "{\"action\":\"" + action.Replace("\"", "") + "\",\"debrisId\":" + debrisId + "}");
        }

        public static void UnlockPetSkill(int skillId)
        {
            Road?.Send(PhoneMsg.PetSkillUnlock, "{\"skillId\":" + skillId + "}");
        }

        public static void TradeStock(string action, int stockId, int shares)
        {
            string act = (action ?? "buy").Replace("\"", "");
            Road?.Send(PhoneMsg.StockTrade, "{\"action\":\"" + act + "\",\"stockId\":" + stockId + ",\"shares\":" + shares + "}");
        }

        public static void PairUpClaim(int rewardId = 0, string action = "claim")
        {
            string act = (action ?? "claim").Replace("\"", "");
            Road?.Send(PhoneMsg.PairUpClaim, "{\"action\":\"" + act + "\",\"rewardId\":" + rewardId + "}");
        }

        public static void ShopShowBuy(int shopId)
        {
            Road?.Send(PhoneMsg.ShopShowBuy, "{\"shopId\":" + shopId + "}");
        }

        public static void StockNotice(string action = "list", int newsId = 0, int stockId = 0)
        {
            string act = (action ?? "list").Replace("\"", "");
            Road?.Send(PhoneMsg.StockNotice,
                "{\"action\":\"" + act + "\",\"newsId\":" + newsId + ",\"stockId\":" + stockId + "}");
        }

        public static void JewelEquip(int level = 0, string action = "equip", int skillType = 0)
        {
            string act = (action ?? "equip").Replace("\"", "");
            Road?.Send(PhoneMsg.JewelEquip,
                "{\"action\":\"" + act + "\",\"level\":" + level + ",\"skillType\":" + skillType + "}");
        }
        public static void WarPassClaim(int qid, string action = "claim")
        {
            string act = (action ?? "claim").Replace("\"", "");
            Road?.Send(PhoneMsg.WarPassClaim, "{\"action\":\"" + act + "\",\"qid\":" + qid + "}");
        }
        public static void TimeLimitShopBuy(int shopId)
        {
            Road?.Send(PhoneMsg.TimeLimitShopBuy, "{\"shopId\":" + shopId + "}");
        }
        public static void BattleTeamUpgrade()
        {
            Road?.Send(PhoneMsg.BattleTeamUpgrade, "{}");
        }
        public static void BattleTeamShopBuy(int id)
        {
            Road?.Send(PhoneMsg.BattleTeamShopBuy, "{\"id\":" + id + "}");
        }
        public static void DailyLeagueClaim(int level = 0)
        {
            Road?.Send(PhoneMsg.DailyLeagueClaim, "{\"level\":" + level + "}");
        }

        public static void UseScroll(int templateId = 0, int types = 0, int profile = 0)
        {
            Road?.Send(PhoneMsg.ScrollUse,
                "{\"templateId\":" + templateId + ",\"types\":" + types + ",\"profile\":" + profile + "}");
        }

        public static void UnlockSigilSkill(int skillId)
        {
            Road?.Send(PhoneMsg.SigilSkillUnlock, "{\"skillId\":" + skillId + "}");
        }

        public static void ConsortiaBuffer(string action = "buy", int bufferId = 0, int badgeId = 0, int rank = 0)
        {
            string act = (action ?? "buy").Replace("\"", "");
            Road?.Send(PhoneMsg.ConsortiaBufferBuy,
                "{\"action\":\"" + act + "\",\"bufferId\":" + bufferId + ",\"badgeId\":" + badgeId + ",\"rank\":" + rank + "}");
        }

        public static void UseElfSkillBook(int templateId = 0, int elfType = -1)
        {
            Road?.Send(PhoneMsg.ElfSkillBook, "{\"templateId\":" + templateId + ",\"elfType\":" + elfType + "}");
        }

        public static void ButterflyTask(string action = "claim", int taskId = 0)
        {
            string act = (action ?? "claim").Replace("\"", "");
            Road?.Send(PhoneMsg.ButterflyTaskClaim, "{\"action\":\"" + act + "\",\"taskId\":" + taskId + "}");
        }

        public static void ChargeSpend(string action = "claim", int rewardId = 0, int amount = 0)
        {
            string act = (action ?? "claim").Replace(""", "");
            Road?.Send(PhoneMsg.ChargeSpendClaim,
                "{"action":"" + act + "","rewardId":" + rewardId + ","amount":" + amount + "}");
        }

        public static void ActivateBuff(int buffId, string action = "activate")
        {
            string act = (action ?? "activate").Replace(""", "");
            Road?.Send(PhoneMsg.BuffActivate, "{"action":"" + act + "","buffId":" + buffId + "}");
        }

        public static void TotemInfoSync(string action = "sync", int totemId = 0)
        {
            string act = (action ?? "sync").Replace(""", "");
            Road?.Send(PhoneMsg.TotemInfoSync, "{"action":"" + act + "","totemId":" + totemId + "}");
        }

        public static void ActiveListClaim(int activeId = 0)
        {
            Road?.Send(PhoneMsg.ActiveListClaim, "{"activeId":" + activeId + "}");
        }


        public static void DrawLottery(int count)
        {
            Road?.Send(PhoneMsg.LotteryDraw, "{\"count\":" + count + "}");
        }

        public static void StrengthenItem(int templateId)
        {
            Road?.Send(PhoneMsg.Strengthen, "{\"templateId\":" + templateId + "}");
        }

        public static void ClaimCardBooklet(int templateId, string action = "claim", int profile = -999)
        {
            string body = "{\"templateId\":" + templateId + ",\"action\":\"" + (action ?? "claim").Replace("\"", "") + "\"";
            if (profile != -999) body += ",\"profile\":" + profile;
            body += "}";
            Road?.Send(PhoneMsg.CardBookletClaim, body);
        }

        public static void QueryStrengthenGoodsMap(int templateId, int level = -1, bool apply = false)
        {
            Road?.Send(PhoneMsg.StrengthenGoodsMap, "{\"templateId\":" + templateId + ",\"level\":" + level + ",\"apply\":" + (apply ? 1 : 0) + "}");
        }

        public static void OpenBox(int templateId)
        {
            Road?.Send(PhoneMsg.BoxOpen, "{\"templateId\":" + templateId + "}");
        }

        public static void FuseItem(int fusionId)
        {
            Road?.Send(PhoneMsg.ItemFusion, "{\"fusionId\":" + fusionId + "}");
        }

        public static void ClaimActivityQuest(int questId, string action = "claim", int condictionId = 0)
        {
            Road?.Send(PhoneMsg.ActivityQuestClaim, "{\"questId\":" + questId +
                ",\"action\":\"" + (action ?? "claim").Replace("\"", "") + "\",\"condictionId\":" + condictionId + "}");
        }

        public static void SwornAction(string action, string nick = "", int templateId = 0)
        {
            Road?.Send(PhoneMsg.SwornAction, "{\"action\":\"" + (action ?? "bond").Replace("\"", "") +
                "\",\"nick\":\"" + (nick ?? "").Replace("\"", "") + "\",\"templateId\":" + templateId + "}");
        }

        public static void VipStoreBuy(int id, int goodsId = 0)
        {
            Road?.Send(PhoneMsg.VipStoreBuy, "{\"id\":" + id + ",\"goodsId\":" + goodsId + "}");
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

        public static void MountDraw()
        {
            Road?.Send(PhoneMsg.MountDraw, "{}");
        }

        public static void UpgradePetFightProperty()
        {
            Road?.Send(PhoneMsg.PetFightProperty, "{}");
        }

        public static void NewYearRankClaim(int rewardId = 0)
        {
            Road?.Send(PhoneMsg.NewYearRankClaim, "{\"rewardId\":" + rewardId + "}

        public static void ClaimDailyAward(int awardId = 0)
        {
            Road?.Send(PhoneMsg.DailyAwardClaim, "{"awardId":" + awardId + "}");
        }

        public static void SelectElfTemplate(int elfId)
        {
            Road?.Send(PhoneMsg.ElfTemplateSelect, "{"elfId":" + elfId + "}");
        }

        public static void ButterflyAction(string action)
        {
            Road?.Send(PhoneMsg.ButterflyAction, "{"action":"" + (action ?? "equip").Replace(""", "") + ""}");
        }");
        }

        public static void CarnivalDraw()
        {
            Road?.Send(PhoneMsg.CarnivalDraw, "{}");
        }

        public static void PeakBattleStart(int rank)
        {
            Road?.Send(PhoneMsg.PeakBattleStart, "{\"rank\":" + rank + "}");
        }

        public static void ForcesBattleStart(int quality)
        {
            Road?.Send(PhoneMsg.ForcesBattleStart, "{\"quality\":" + quality + "}");
        }

        public static void UpgradeRelic(int relicId)
        {
            Road?.Send(PhoneMsg.ForcesRelicUpgrade, "{\"relicId\":" + relicId + "}");
        }

        public static void CultureUpgrade(int statType)
        {
            Road?.Send(PhoneMsg.CultureUpgrade, "{\"statType\":" + statType + "}");
        }

        public static void CultureGradeUp()
        {
            Road?.Send(PhoneMsg.CultureUpgrade, "{\"gradeUp\":1}");
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

        public static void ClaimDevilTreasPoint(int rewardId)
        {
            Road?.Send(PhoneMsg.DevilTreasPointClaim, "{\"rewardId\":" + rewardId + "}");
        }

        public static void ClaimDevilTreasRank(int rewardId = 0)
        {
            Road?.Send(PhoneMsg.DevilTreasRankClaim, "{\"rewardId\":" + rewardId + "}");
        }

        public static void RecycleActivityClaim(int templateId, int count = 1)
        {
            Road?.Send(PhoneMsg.RecycleActivityClaim,
                "{\"templateId\":" + templateId + ",\"count\":" + count + "}");
        }

        public static void UpgradeMagicItem()
        {
            Road?.Send(PhoneMsg.MagicItemUpgrade, "{}");
        }

        public static void SpaRoomStart()
        {
            Road?.Send(PhoneMsg.SpaRoomStart, "{}");
        }

        public static void SpaRoomBomb(int index)
        {
            Road?.Send(PhoneMsg.SpaRoomBomb, "{\"index\":" + index + "}");
        }

        public static void TreasureRoomDraw(int count = 1)
        {
            Road?.Send(PhoneMsg.TreasureRoomDraw, "{\"count\":" + count + "}");
        }

        public static void ClaimChristmas()
        {
            Road?.Send(PhoneMsg.ChristmasClaim, "{}");
        }

        public static void NewYearPlay()
        {
            Road?.Send(PhoneMsg.NewYearClaim, "{}");
        }

        public static void NewYearClaimReward(int rewardId)
        {
            Road?.Send(PhoneMsg.NewYearClaim, "{\"rewardId\":" + rewardId + "}");
        }

        public static void WorshipMoonClaim(int batches = 1)
        {
            Road?.Send(PhoneMsg.WorshipMoonClaim, "{\"count\":" + batches + "}");
        }

        public static void SuperLuckerDraw(int count = 1)
        {
            Road?.Send(PhoneMsg.SuperLuckerDraw, "{\"count\":" + count + "}");
        }

        public static void CalendarClaim(int dayIndex) { Road?.Send(PhoneMsg.CalendarClaim, "{\"dayIndex\":" + dayIndex + "}"); }

        public static void QuizAnswer(int questionId, int option)
        {
            Road?.Send(PhoneMsg.QuizAnswer, "{\"questionId\":" + questionId + ",\"option\":" + option + "}");
        }

        public static void OneYuanBuy(int id, int goodsId)
        {
            Road?.Send(PhoneMsg.OneYuanBuy, "{\"id\":" + id + ",\"goodsId\":" + goodsId + "}");
        }

        public static void AuditoriumAction(string action, int tierOrIndex = 0)
        {
            string act = (action ?? "wedding").Replace("\"", "");
            if (string.Equals(act, "fire", System.StringComparison.OrdinalIgnoreCase))
                Road?.Send(PhoneMsg.AuditoriumAction, "{\"action\":\"fire\",\"index\":" + tierOrIndex + "}");
            else if (string.Equals(act, "wedding", System.StringComparison.OrdinalIgnoreCase))
                Road?.Send(PhoneMsg.AuditoriumAction, "{\"action\":\"wedding\",\"tier\":" + tierOrIndex + "}");
            else
                Road?.Send(PhoneMsg.AuditoriumAction, "{\"action\":\"" + act + "\"}");
        }

        public static void BoguAdventureAction(string action, int activityNum = 5, int tier = 0)
        {
            Road?.Send(PhoneMsg.BoguAdventureAction,
                "{\"action\":\"" + (action ?? "spin").Replace("\"", "") + "\",\"activityNum\":" + activityNum + ",\"tier\":" + tier + "}");
        }

        public static void JigsawAction(string action = "claim")
        {
            Road?.Send(PhoneMsg.JigsawAction, "{\"action\":\"" + (action ?? "claim").Replace("\"", "") + "\"}");
        }

        public static void BibleAction(string action = "claim")
        {
            Road?.Send(PhoneMsg.BibleAction, "{\"action\":\"" + (action ?? "claim").Replace("\"", "") + "\"}");
        }

        public static void ClaimRedPacket()
        {
            Road?.Send(PhoneMsg.RedPacketClaim, "{}");
        }

        public static void HomeTemplePractice() { Road?.Send(PhoneMsg.HomeTemplePractice, "{}"); }
        public static void HomeTempleAdvance() { Road?.Send(PhoneMsg.HomeTempleAdvance, "{}"); }
        public static void BankDeposit(string action, int templateId, int amount, int slot = 0) { Road?.Send(PhoneMsg.BankDeposit, "{\"action\":\"" + (action ?? "deposit") + "\",\"templateId\":" + templateId + ",\"amount\":" + amount + ",\"slot\":" + slot + "}"); }
        public static void SweepMission(int missionId) { Road?.Send(PhoneMsg.SweepMission, "{\"missionId\":" + missionId + "}"); }
        public static void SendRedPacket(string friend, int gold)
        {
            string fn = (friend ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
            Road?.Send(PhoneMsg.RedPacketSend, "{\"friend\":\"" + fn + "\",\"gold\":" + gold + "}");
        }

        public static void UpgradeHomeTemple()
        {
            Road?.Send(PhoneMsg.HomeTempleUpgrade, "{}");
        }

        public static void WardrobeEquip(int clothId)
        {
            Road?.Send(PhoneMsg.WardrobeEquip, "{\"clothId\":" + clothId + "}");
        }

        public static void WardrobeUpgrade(int propertyId)
        {
            Road?.Send(PhoneMsg.WardrobeUpgrade, "{\"propertyId\":" + propertyId + "}");
        }

        public static void HonorSystemAction(string action, int honorId = 1)
        {
            Road?.Send(PhoneMsg.HonorSystemAction,
                "{\"action\":\"" + (action ?? "donate").Replace("\"", "") + "\",\"honorId\":" + honorId + "}");
        }

        public static void HonorSystemClaim(int level)
        {
            Road?.Send(PhoneMsg.HonorSystemClaim, "{\"level\":" + level + "}");
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

        public static void CraftEmblem(int types, int profile)
        {
            Road?.Send(PhoneMsg.EmblemCraft, "{\"types\":" + types + ",\"profile\":" + profile + "}");
        }

        public static void EquipEmblem(int emblemId, int equipped = 1)
        {
            Road?.Send(PhoneMsg.EmblemEquip, "{\"emblemId\":" + emblemId + ",\"equipped\":" + equipped + "}");
        }

        public static void ComposeSoulStamp(int quality)
        {
            Road?.Send(PhoneMsg.SoulStampCompose, "{\"quality\":" + quality + "}");
        }

        public static void RefineSoulStamp(int soulStampId)
        {
            Road?.Send(PhoneMsg.SoulStampRefine, "{\"soulStampId\":" + soulStampId + "}");
        }

        public static void DreamlandStart(int chapter, int section)
        {
            Road?.Send(PhoneMsg.DreamlandStart,
                "{\"chapter\":" + chapter + ",\"section\":" + section + "}");
        }

        public static void DreamlandClaim(int chapter, int section)
        {
            Road?.Send(PhoneMsg.DreamlandClaim,
                "{\"chapter\":" + chapter + ",\"section\":" + section + "}");
        }

        public static void WarriorFamStart(int hardType, int level)
        {
            Road?.Send(PhoneMsg.WarriorFamStart,
                "{\"hardType\":" + hardType + ",\"level\":" + level + "}");
        }

        public static void WarriorFamClaim(int hardType, int level)
        {
            Road?.Send(PhoneMsg.WarriorFamClaim,
                "{\"hardType\":" + hardType + ",\"level\":" + level + "}");
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

        public static void GuildUpgrade()
        {
            Road?.Send(PhoneMsg.GuildUpgrade, "{}");
        }

        public static void ConsortiaBossStart()
        {
            Road?.Send(PhoneMsg.ConsortiaBossStart, "{}");
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

        public static void SendWhisper(string to, string msg)
        {
            Road?.Send(PhoneMsg.ChatWhisper,
                "{\"to\":\"" + (to ?? "").Replace("\"", "") +
                "\",\"msg\":\"" + (msg ?? "").Replace("\"", "") + "\"}");
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

        public static void UpgradeJamps() { Road?.Send(PhoneMsg.JampsUpgrade, "{}"); }
        public static void JampsClaimPage(string action, int pageId, int debrisId)
        {
            Road?.Send(PhoneMsg.JampsClaimPage,
                "{\"action\":\"" + (action ?? "").Replace("\"", "") + "\",\"pageId\":" + pageId + ",\"debrisId\":" + debrisId + "}");
        }
        public static void UpgradeCardMain() { Road?.Send(PhoneMsg.CardMainUpgrade, "{}"); }
        public static void ElfIntimacyAction(string action)
        {
            Road?.Send(PhoneMsg.ElfIntimacyAction, "{\"action\":\"" + (action ?? "gift").Replace("\"", "") + "\"}");
        }
    }
}
