using System;
using System.IO;
using System.Threading;
using GunMobile.Core;
using GunMobile.Net;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Standalone
{
    static class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("GunMobile Standalone Server (.NET)");
            string pcRoot = ResolvePcDataRoot(args);
            string dataDir = Application.persistentDataPath;
            Directory.CreateDirectory(dataDir);
            string savePath = Path.Combine(dataDir, "server_players");
            Directory.CreateDirectory(savePath);

            Console.WriteLine("PC data root: " + pcRoot);
            Console.WriteLine("Player save:  " + savePath);

            var loader = CreateLoader(pcRoot, dataDir);
            var db = GameDatabase.Load(loader) ?? new GameDatabase();
            var maps = MapCatalog.DiscoverCollisionIds(loader);
            Console.WriteLine($"DB items={db.Items.Count} shop={db.Shop.Count} maps={db.Maps.Count} npcs={db.Npcs.Count} collisionMaps={maps.Count}");

            var server = new MobileGameServer();
            server.Start(db, loader, savePath);
            if (!server.Running)
            {
                Console.Error.WriteLine("Failed to start: " + server.LastError);
                return 1;
            }

            Console.WriteLine($"Listening Road={PhonePacket.RoadPort} Fight={PhonePacket.FightPort}");
            Console.WriteLine("Press Ctrl+C to stop.");
            using var stop = new ManualResetEventSlim(false);
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                stop.Set();
            };

            stop.Wait();
            server.Stop();
            Console.WriteLine("Stopped.");
            return 0;
        }

        static string ResolvePcDataRoot(string[] args)
        {
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                return Path.GetFullPath(args[0]);
            }

            string env = Environment.GetEnvironmentVariable("GUNMOBILE_PC_DATA");
            if (!string.IsNullOrWhiteSpace(env))
            {
                return Path.GetFullPath(env);
            }

            string[] candidates =
            {
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "PcData")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "UnityClient", "Assets", "StreamingAssets", "PcData")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "UnityClient", "Assets", "StreamingAssets", "PcData")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "legacy", "data"))
            };

            foreach (string c in candidates)
            {
                if (Directory.Exists(c))
                {
                    return c;
                }
            }

            return candidates[0];
        }

        static ResLoader CreateLoader(string pcRoot, string dataDir)
        {
            string persistentPc = Path.Combine(dataDir, "PcData");
            Directory.CreateDirectory(persistentPc);
            var loader = new ResLoader(pcRoot, persistentPc);

            string repoLegacy = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "legacy", "unpacked"));
            if (Directory.Exists(repoLegacy))
            {
                loader.ExtraRoots.Add(repoLegacy);
            }

            string repoData = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "legacy", "data"));
            if (Directory.Exists(repoData))
            {
                loader.ExtraRoots.Add(repoData);
            }

            return loader;
        }
    }
}
