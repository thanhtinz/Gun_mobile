using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GunMobile.SmokeClient
{
    /// <summary>
    /// Bắn PhoneMsg tới MobileGameServer đang chạy rồi kiểm tra phản hồi.
    ///
    /// Phần 1 (hall): gửi toàn bộ msg 233-269, fail nếu thiếu reply hoặc reply là
    /// err":"config" (nghĩa là bảng PC tương ứng không nạp được nữa).
    /// Phần 2 (battle): hai kết nối tạo phòng, sẵn sàng, vào trận rồi bắn một phát;
    /// server phải tự mô phỏng và trả FightShotResult + FightCrater.
    ///
    /// Dùng: dotnet SmokeClient.dll [host] [port]
    /// </summary>
    static class Program
    {
        const ushort Magic = 0x7D01;

        const ushort Login = 2;
        const ushort LoginOk = 3;
        const ushort JoinRoom = 10;
        const ushort RoomOk = 11;
        const ushort ProfileData = 21;
        const ushort CreateRoom = 82;
        const ushort RoomCreated = 83;
        const ushort RoomReady = 86;
        const ushort FightStart = 91;
        const ushort FightFire = 93;
        const ushort FightTurn = 97;
        const ushort FightCrater = 103;
        const ushort FightShotResult = 117;

        readonly struct Step
        {
            public Step(ushort id, string name, string json) { Id = id; Name = name; Json = json; }
            public ushort Id { get; }
            public string Name { get; }
            public string Json { get; }
        }

        static readonly Step[] Steps =
        {
            new Step(Login, "Login", "{\"nick\":\"smoke\"}"),
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

        sealed class Conn : IDisposable
        {
            readonly TcpClient _tcp = new TcpClient();
            readonly Queue<KeyValuePair<ushort, string>> _pending = new Queue<KeyValuePair<ushort, string>>();
            readonly object _gate = new object();
            NetworkStream _stream;

            public Conn(string host, int port)
            {
                // Server nạp ~250 bảng trước khi bind cổng nên thử lại vài lần.
                Exception last = null;
                for (int attempt = 0; attempt < 60; attempt++)
                {
                    try
                    {
                        _tcp.Connect(host, port);
                        last = null;
                        break;
                    }
                    catch (SocketException e)
                    {
                        last = e;
                        Thread.Sleep(1000);
                    }
                }

                if (last != null) throw last;

                _stream = _tcp.GetStream();
                var reader = new Thread(ReadLoop) { IsBackground = true };
                reader.Start();
            }
            public void Send(ushort msgId, string json)
            {
                byte[] packet = Encode(msgId, json);
                _stream.Write(packet, 0, packet.Length);
                _stream.Flush();
            }

            public string WaitFor(ushort msgId, TimeSpan timeout)
            {
                DateTime deadline = DateTime.UtcNow + timeout;
                lock (_gate)
                {
                    while (true)
                    {
                        int count = _pending.Count;
                        for (int i = 0; i < count; i++)
                        {
                            KeyValuePair<ushort, string> item = _pending.Dequeue();
                            if (item.Key == msgId) return item.Value;
                            _pending.Enqueue(item);
                        }

                        TimeSpan left = deadline - DateTime.UtcNow;
                        if (left <= TimeSpan.Zero) return null;
                        Monitor.Wait(_gate, left);
                    }
                }
            }

            void ReadLoop()
            {
                var buffer = new byte[1 << 20];
                int have = 0;
                try
                {
                    while (true)
                    {
                        int read = _stream.Read(buffer, have, buffer.Length - have);
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
                                lock (_gate)
                                {
                                    _pending.Enqueue(new KeyValuePair<ushort, string>(id, json));
                                    Monitor.PulseAll(_gate);
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

            public void Dispose()
            {
                try { _stream?.Dispose(); } catch { }
                try { _tcp.Dispose(); } catch { }
            }
        }

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

            int failures = RunHall(host, port);
            failures += RunBattle(host, port);

            Console.WriteLine(failures == 0
                ? "SMOKE OK (hall + battle)"
                : $"SMOKE FAILED ({failures} lỗi)");
            return failures == 0 ? 0 : 1;
        }

        static int RunHall(string host, int port)
        {
            Console.WriteLine("-- hall --");
            using var conn = new Conn(host, port);
            int failures = 0;

            foreach (Step step in Steps)
            {
                conn.Send(step.Id, step.Json);
                ushort expect = step.Id == Login ? LoginOk : step.Id;
                string reply = conn.WaitFor(expect, TimeSpan.FromSeconds(5));

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

                bool ok = reply.Contains("\"ok\":true") || step.Id == Login;
                Console.WriteLine($"{(ok ? "ok  " : "rej ")}  {step.Id,3} {step.Name}: {Trim(reply)}");
            }

            Console.WriteLine($"hall: {Steps.Length} messages, {failures} lỗi");
            return failures;
        }

        // Trận đấu server-authoritative: client chỉ gửi góc/lực, server mô phỏng
        // đường đạn rồi phát FightShotResult + FightCrater.
        // Sảnh chạy trên cổng 4396, còn msg trận nằm ở cổng Fight 1910; socket trận
        // gắn với player bằng JoinRoom {playerId} lấy từ LoginOk.
        static int RunBattle(string host, int roadPort)
        {
            Console.WriteLine("-- battle --");
            int failures = 0;
            int fightPort = 1910;
            var timeout = TimeSpan.FromSeconds(10);

            using var a = new Conn(host, roadPort);
            using var b = new Conn(host, roadPort);

            a.Send(Login, "{\"nick\":\"smokeHost\"}");
            b.Send(Login, "{\"nick\":\"smokeJoin\"}");
            string loginA = a.WaitFor(LoginOk, timeout);
            string loginB = b.WaitFor(LoginOk, timeout);
            if (loginA == null || loginB == null)
            {
                Console.WriteLine("FAIL  battle: login thất bại");
                return failures + 1;
            }

            int idA = JsonInt(loginA, "playerId", -1);
            int idB = JsonInt(loginB, "playerId", -1);

            a.Send(CreateRoom, "{\"mapId\":1005,\"maxPlayers\":2,\"name\":\"smoke\"}");
            string created = a.WaitFor(RoomCreated, timeout);
            if (created == null)
            {
                Console.WriteLine("FAIL  battle: không tạo được phòng");
                return failures + 1;
            }

            int roomId = JsonInt(created, "roomId", -1);
            Console.WriteLine($"ok    room {roomId}: {Trim(created)}");

            b.Send(JoinRoom, "{\"roomId\":" + roomId + "}");
            string joined = b.WaitFor(RoomOk, timeout);
            if (joined == null)
            {
                Console.WriteLine("FAIL  battle: joiner không vào được phòng");
                return failures + 1;
            }

            Console.WriteLine($"ok    join: {Trim(joined)}");

            a.Send(RoomReady, "{\"ready\":1}");
            b.Send(RoomReady, "{\"ready\":1}");
            Thread.Sleep(300);

            using var fa = new Conn(host, fightPort);
            using var fb = new Conn(host, fightPort);
            fa.Send(JoinRoom, "{\"playerId\":" + idA + "}");
            fb.Send(JoinRoom, "{\"playerId\":" + idB + "}");
            Thread.Sleep(300);

            // seed 11 → Random(11).Next(-3,4) == 0, tức gió 0: đạn không bị thổi lệch
            // nên có thể khẳng định phát bắn phải rơi trong map.
            fa.Send(FightStart, "{\"map\":1005,\"seed\":11}");
            string startA = fa.WaitFor(FightStart, timeout);
            string startB = fb.WaitFor(FightStart, timeout);
            if (startA == null || startB == null)
            {
                Console.WriteLine("FAIL  battle: FightStart không tới cả hai client");
                return failures + 1;
            }

            Console.WriteLine($"ok    FightStart: playerCount={JsonInt(startA, "playerCount", -1)} " +
                              $"wind={JsonInt(startA, "wind", 0)} p0_hp={JsonInt(startA, "p0_hp", -1)}");

            // Bắn vài phát với góc/lực khác nhau; mỗi lượt đổi ghế vì server chỉ nhận
            // FightFire từ người đang tới lượt. Yêu cầu: mọi phát đều phải có quỹ đạo
            // + hố nổ khớp điểm va chạm, và ít nhất một phát rơi trong map (chứng minh
            // server thực sự dò va chạm với fore.map chứ không chỉ bắn ra ngoài).
            // Tầm bay ≈ v²·sin(2θ)/0.7 nên lực 15-30 mới rơi trong map rộng ~1300px;
            // lực 40+ bay vượt biên (đúng vật lý PC, không phải lỗi).
            var shots = new[]
            {
                new[] { 45, 20 },
                new[] { 60, 25 },
                new[] { 45, 15 },
                new[] { 55, 28 },
            };

            int simulated = 0;
            int landedInMap = 0;
            int turnSeat = 0;   // FightStart đặt lượt đầu cho ghế 0

            for (int i = 0; i < shots.Length; i++)
            {
                Conn shooter = turnSeat == 0 ? fa : fb;
                int facing = turnSeat == 0 ? 1 : -1;   // ghế 0 đứng bên trái, ghế 1 bên phải
                shooter.Send(FightFire,
                    "{\"angle\":" + shots[i][0] + ",\"power\":" + shots[i][1] + ",\"facing\":" + facing + "}");

                string shot = fb.WaitFor(FightShotResult, TimeSpan.FromSeconds(6));
                if (shot == null)
                {
                    Console.WriteLine($"FAIL  battle: ghế {turnSeat} bắn nhưng không có FightShotResult");
                    failures++;
                    break;
                }

                simulated++;
                int shotWho = JsonInt(shot, "who", -1);
                if (shotWho != turnSeat)
                {
                    Console.WriteLine($"FAIL  battle: FightShotResult ghi who={shotWho} nhưng đang là lượt ghế {turnSeat}: {Trim(shot)}");
                    failures++;
                    break;
                }

                int hx = JsonInt(shot, "x", int.MinValue);
                int hy = JsonInt(shot, "y", int.MinValue);
                bool inMap = hx >= 0 && hy >= 0;
                if (inMap) landedInMap++;

                string crater = fb.WaitFor(FightCrater, TimeSpan.FromSeconds(6));
                if (crater == null)
                {
                    Console.WriteLine("FAIL  battle: không có FightCrater (địa hình không bị cắt)");
                    failures++;
                    break;
                }

                if (JsonInt(crater, "x", int.MaxValue) != hx || JsonInt(crater, "y", int.MaxValue) != hy)
                {
                    Console.WriteLine($"FAIL  battle: hố nổ lệch điểm va chạm: {Trim(crater)}");
                    failures++;
                    break;
                }

                // path rỗng nghĩa là đạn chạm ngay trước mũi súng — vẫn hợp lệ.
                bool hasPath = shot.IndexOf("\"path\":[]", StringComparison.Ordinal) < 0 &&
                               shot.IndexOf("\"path\":[", StringComparison.Ordinal) >= 0;
                string head = PathHead(shot);
                Console.WriteLine($"ok    ghế {turnSeat} bắn {shots[i][0]}°/{shots[i][1]} → ({hx},{hy}) path{head}" +
                                  (inMap ? " trong map" : " bay ra ngoài") +
                                  (hasPath ? ", có quỹ đạo" : ", chạm ngay") + ", hố nổ khớp");

                // Server phải tự chuyển lượt sau mỗi phát, nếu không trận sẽ đứng.
                string turn = fb.WaitFor(FightTurn, TimeSpan.FromSeconds(6));
                if (turn == null)
                {
                    Console.WriteLine("FAIL  battle: server không chuyển lượt sau phát bắn");
                    failures++;
                    break;
                }

                int nextSeat = JsonInt(turn, "player", -1);
                if (nextSeat == turnSeat)
                {
                    Console.WriteLine($"FAIL  battle: lượt vẫn ở ghế {turnSeat} sau khi bắn: {Trim(turn)}");
                    failures++;
                    break;
                }

                int turnWind = JsonInt(turn, "wind", 0);
                Console.WriteLine($"ok    chuyển lượt: ghế {turnSeat} → {nextSeat}, gió {turnWind}");
                turnSeat = nextSeat;
            }

            if (simulated == 0)
            {
                Console.WriteLine("FAIL  battle: server không mô phỏng phát nào (thiếu FightShotResult)");
                failures++;
            }
            else if (landedInMap == 0)
            {
                Console.WriteLine($"FAIL  battle: {simulated} phát đều bay ra ngoài map — va chạm fore.map có vấn đề?");
                failures++;
            }

            Console.WriteLine($"battle: {failures} lỗi");
            return failures;
        }

        static int JsonInt(string json, string key, int fallback)
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            string needle = "\"" + key + "\":";
            int i = json.IndexOf(needle, StringComparison.Ordinal);
            if (i < 0) return fallback;

            i += needle.Length;
            while (i < json.Length && (json[i] == ' ' || json[i] == '"')) i++;

            int start = i;
            if (i < json.Length && (json[i] == '-' || json[i] == '+')) i++;
            while (i < json.Length && char.IsDigit(json[i])) i++;
            if (i == start) return fallback;

            return int.TryParse(json.Substring(start, i - start), out int value) ? value : fallback;
        }

        static string PathHead(string json)
        {
            int i = json.IndexOf("\"path\":[", StringComparison.Ordinal);
            if (i < 0) return "-";
            int end = json.IndexOf(']', i);
            if (end < 0) return "-";
            string body = json.Substring(i + 8, end - i - 8);
            if (body.Length == 0) return "[]";
            string[] parts = body.Split(',');
            var sb = new StringBuilder("[");
            for (int k = 0; k < parts.Length && k < 4; k++) { if (k > 0) sb.Append(','); sb.Append(parts[k]); }
            if (parts.Length > 8)
            {
                sb.Append(" .. ");
                for (int k = parts.Length - 4; k < parts.Length; k++) { if (k > parts.Length - 4) sb.Append(','); sb.Append(parts[k]); }
            }
            return sb.Append("] n=").Append(parts.Length / 2).ToString();
        }

        static string Trim(string s) => s.Length > 140 ? s.Substring(0, 140) + "..." : s;
    }
}
