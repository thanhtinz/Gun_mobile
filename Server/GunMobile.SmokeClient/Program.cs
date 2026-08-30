using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GunMobile.SmokeClient
{
    /// <summary>
    /// Bắn từng PhoneMsg tới MobileGameServer đang chạy rồi kiểm tra phản hồi.
    /// Mục tiêu là bắt hồi quy: mọi msg phải có reply, và không msg nào trả
    /// err":"config" (nghĩa là bảng PC tương ứng không nạp được nữa).
    /// Dùng: dotnet SmokeClient.dll [host] [port]
    /// </summary>
    static class Program
    {
        const ushort Magic = 0x7D01;
        const ushort ProfileData = 21;
        const ushort LoginOk = 3;

        readonly struct Step
        {
            public Step(ushort id, string name, string json) { Id = id; Name = name; Json = json; }
            public ushort Id { get; }
            public string Name { get; }
            public string Json { get; }
        }

        static readonly Step[] Steps =
        {
            new Step(2, "Login", "{\"nick\":\"smoke\"}"),
            new Step(233, "ManorSeedPlant/plant", "{\"action\":\"plant\",\"templateId\":333001}"),
            new Step(233, "ManorSeedPlant/harvest", "{\"action\":\"harvest\"}"),
            new Step(234, "ManorTaskClaim", "{\"taskId\":1}"),
            new Step(235, "CardAchievementClaim", "{\"achievementId\":10101}"),
            new Step(236, "GuardCoreUpgrade/exp", "{\"action\":\"exp\"}"),
            new Step(237, "LightRiddleAnswer/next", "{\"action\":\"next\"}"),
            new Step(237, "LightRiddleAnswer/answer", "{\"action\":\"answer\",\"option\":1}"),
            new Step(238, "FairBattleSkillLearn", "{\"id\":1}"),
            new Step(239, "FairBattleClaim/battle", "{\"action\":\"battle\",\"win\":1}"),
            new Step(240, "OnlineArmUpgrade/dig", "{\"action\":\"dig\",\"floor\":1}"),
            new Step(240, "OnlineArmUpgrade/exp", "{\"action\":\"exp\",\"slot\":0}"),
            new Step(241, "SubWeaponEvolve/exp", "{\"action\":\"exp\"}"),
            new Step(242, "LoveLevelUp/pair", "{\"action\":\"pair\",\"nick\":\"buddy\"}"),
            new Step(242, "LoveLevelUp/add", "{\"action\":\"add\"}"),
            new Step(243, "TreeChallenge", "{}"),
            new Step(244, "DailyActiveClaim/task", "{\"action\":\"task\",\"taskId\":1}"),
            new Step(245, "LoginAwardClaim", "{}"),
            new Step(246, "KingRoadQuest", "{\"questId\":3201}"),
            new Step(247, "ActiveConvert", "{\"activeId\":2}"),
            new Step(248, "MiniGameShopBuy/point", "{\"action\":\"point\"}"),
            new Step(249, "WasteRecycleClaim/draw", "{\"action\":\"draw\"}"),
            new Step(250, "SetsBuild/feed", "{\"action\":\"feed\",\"setsType\":1}"),
            new Step(251, "EngraveRefine/temper", "{\"action\":\"temper\",\"character\":1}"),
            new Step(252, "UserBoxOpen", "{\"id\":1}"),
            new Step(253, "CommunalActive/score", "{\"action\":\"score\",\"activeId\":1}"),
            new Step(254, "GoodsCollect", "{\"id\":1}"),
            new Step(256, "HelpGameReward", "{\"missionId\":1,\"star\":1}"),
            new Step(257, "PetForm/feed", "{\"action\":\"feed\",\"count\":1}"),
            new Step(258, "RuneAdvance", "{\"advancedTempId\":311129}"),
            new Step(259, "ChargeReward/charge", "{\"action\":\"charge\",\"amount\":100}"),
            new Step(260, "ThreeCleanClaim", "{\"id\":1}"),
            new Step(261, "DiceGame", "{}"),
            new Step(262, "HomeFish/fish", "{\"action\":\"fish\"}"),
            new Step(263, "NaiKuaiEquip", "{\"action\":\"equip\",\"id\":1}"),
            new Step(264, "ActivitySystemDraw", "{\"activityType\":8}"),
            new Step(265, "EventRewardDraw", "{\"activityType\":4}"),
            new Step(266, "CardBuffActivate/step", "{\"action\":\"step\"}"),
            new Step(267, "SearchGoods", "{\"starId\":1}"),
            new Step(268, "MaxLevelUp", "{}"),
            new Step(269, "StrengthenExp/add", "{\"action\":\"add\"}"),
        };

        static byte[] Encode(ushort msgId, string json)
        {
            byte[] body = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(json) ? "{}" : json);
            var buf = new byte[8 + body.Length];
            int payload = 4 + body.Length;
            buf[0] = (byte)payload;
            buf[1] = (byte)(payload >> 8);
            buf[2] = (byte)(payload >> 16);
            buf[3] = (byte)(payload >> 24);
            buf[4] = unchecked((byte)Magic);
            buf[5] = (byte)(Magic >> 8);
            buf[6] = unchecked((byte)msgId);
            buf[7] = (byte)(msgId >> 8);
            Buffer.BlockCopy(body, 0, buf, 8, body.Length);
            return buf;
        }

        static int Main(string[] args)
        {
            string host = args.Length > 0 ? args[0] : "127.0.0.1";
            int port = args.Length > 1 && int.TryParse(args[1], out int p) ? p : 4396;

            using var tcp = new TcpClient();
            tcp.Connect(host, port);
            var stream = tcp.GetStream();

            var pending = new Queue<KeyValuePair<ushort, string>>();
            var gate = new object();
            var reader = new Thread(() => ReadLoop(stream, pending, gate)) { IsBackground = true };
            reader.Start();

            int failures = 0;
            foreach (Step step in Steps)
            {
                byte[] packet = Encode(step.Id, step.Json);
                stream.Write(packet, 0, packet.Length);
                stream.Flush();

                ushort expect = step.Id == 2 ? LoginOk : step.Id;
                string reply = WaitFor(expect, pending, gate, TimeSpan.FromSeconds(5));
                if (reply == null)
                {
                    Console.WriteLine($"FAIL  {step.Id,3} {step.Name}: không có phản hồi");
                    failures++;
                    continue;
                }

                if (reply.Contains("\"err\":\"config\""))
                {
                    Console.WriteLine($"FAIL  {step.Id,3} {step.Name}: {reply}");
                    failures++;
                    continue;
                }

                bool ok = reply.Contains("\"ok\":true") || step.Id == 2;
                Console.WriteLine($"{(ok ? "ok  " : "rej ")}  {step.Id,3} {step.Name}: {Trim(reply)}");
            }

            Console.WriteLine(failures == 0
                ? $"SMOKE OK ({Steps.Length} messages, mọi bảng đều nạp được)"
                : $"SMOKE FAILED ({failures}/{Steps.Length})");
            return failures == 0 ? 0 : 1;
        }

        static void ReadLoop(NetworkStream stream, Queue<KeyValuePair<ushort, string>> pending, object gate)
        {
            var buffer = new byte[1 << 20];
            int have = 0;
            try
            {
                while (true)
                {
                    int read = stream.Read(buffer, have, buffer.Length - have);
                    if (read <= 0) return;
                    have += read;

                    int offset = 0;
                    while (have - offset >= 8)
                    {
                        int payload = buffer[offset] | (buffer[offset + 1] << 8) |
                                      (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24);
                        int total = 4 + payload;
                        if (payload < 4 || have - offset < total) break;

                        ushort id = (ushort)(buffer[offset + 6] | (buffer[offset + 7] << 8));
                        string json = Encoding.UTF8.GetString(buffer, offset + 8, payload - 4);
                        if (id != ProfileData)
                        {
                            lock (gate)
                            {
                                pending.Enqueue(new KeyValuePair<ushort, string>(id, json));
                                Monitor.PulseAll(gate);
                            }
                        }

                        offset += total;
                    }

                    if (offset > 0)
                    {
                        Buffer.BlockCopy(buffer, offset, buffer, 0, have - offset);
                        have -= offset;
                    }
                }
            }
            catch
            {
                // socket closed while shutting down
            }
        }

        static string WaitFor(ushort msgId, Queue<KeyValuePair<ushort, string>> pending, object gate, TimeSpan timeout)
        {
            DateTime deadline = DateTime.UtcNow + timeout;
            lock (gate)
            {
                while (true)
                {
                    int count = pending.Count;
                    for (int i = 0; i < count; i++)
                    {
                        KeyValuePair<ushort, string> item = pending.Dequeue();
                        if (item.Key == msgId) return item.Value;
                        pending.Enqueue(item);
                    }

                    TimeSpan left = deadline - DateTime.UtcNow;
                    if (left <= TimeSpan.Zero) return null;
                    Monitor.Wait(gate, left);
                }
            }
        }

        static string Trim(string s) => s.Length > 140 ? s.Substring(0, 140) + "..." : s;
    }
}
