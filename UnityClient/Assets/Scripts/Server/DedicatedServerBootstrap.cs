using System.Collections;
using GunMobile.Client;
using GunMobile.Net;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Server
{
    /// <summary>
    /// Server-only entrypoint for VPS / headless Linux builds.
    /// Runs MobileGameServer (TCP 4396 / 1910) and keeps the process alive.
    /// </summary>
    public sealed class DedicatedServerBootstrap : MonoBehaviour
    {
        [Header("Server")]
        [Tooltip("If true, runs PcContent.Install(… ) at startup (unpacks PC data into persistent storage).")]
        public bool InstallPcData = true;

        [Tooltip("Optional server player save override folder (relative to Application.persistentDataPath).")]
        public string SaveFolderOverride = "";

        void Start()
        {
            StartCoroutine(BootRoutine());
        }

        IEnumerator BootRoutine()
        {
            var loader = PcContent.CreateLoader();
            if (InstallPcData)
            {
                yield return PcContent.Install(loader, s => Debug.Log("[DedicatedServer] " + s));
            }

            var db = GameDatabase.Load(loader) ?? new GameDatabase();

            string savePath = string.IsNullOrWhiteSpace(SaveFolderOverride)
                ? null
                : System.IO.Path.Combine(Application.persistentDataPath, SaveFolderOverride.Trim());

            var server = new MobileGameServer();
            server.Start(db, savePath);
            Debug.Log("[DedicatedServer] Started. Road=" + PhonePacket.RoadPort + " Fight=" + PhonePacket.FightPort);

            // Keep running.
            while (true)
            {
                yield return null;
            }
        }
    }
}

