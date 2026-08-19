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
    /// Auto-boots via [RuntimeInitializeOnLoadMethod] so no scene setup is needed.
    /// </summary>
    public sealed class DedicatedServerBootstrap : MonoBehaviour
    {
        static MobileGameServer _server;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBoot()
        {
#if UNITY_SERVER || UNITY_STANDALONE_LINUX
            if (!Application.isBatchMode) return;
            var go = new GameObject("DedicatedServer");
            DontDestroyOnLoad(go);
            go.AddComponent<DedicatedServerBootstrap>();
#endif
        }

        void Start()
        {
            if (_server != null && _server.Running) return;
            StartCoroutine(BootRoutine());
        }

        IEnumerator BootRoutine()
        {
            Debug.Log("[DedicatedServer] Loading PC data...");
            var loader = PcContent.CreateLoader();
            yield return PcContent.Install(loader, s => Debug.Log("[DedicatedServer] " + s));

            var db = GameDatabase.Load(loader) ?? new GameDatabase();
            Debug.Log($"[DedicatedServer] DB loaded: items={db.Items.Count} shop={db.Shop.Count} maps={db.Maps.Count} npcs={db.Npcs.Count}");

            string savePath = System.IO.Path.Combine(Application.persistentDataPath, "server_players");

            _server = new MobileGameServer();
            _server.Start(db, loader, savePath);
            Debug.Log($"[DedicatedServer] Online! Road={PhonePacket.RoadPort} Fight={PhonePacket.FightPort} save={savePath}");

            while (_server.Running)
            {
                yield return new WaitForSeconds(30f);
                Debug.Log($"[DedicatedServer] players={_server.PlayerCount} rooms={_server.RoomCount}");
            }

            Debug.LogWarning("[DedicatedServer] Server stopped unexpectedly: " + _server.LastError);
        }

        void OnApplicationQuit()
        {
            _server?.Stop();
            Debug.Log("[DedicatedServer] Shutdown.");
        }
    }
}
