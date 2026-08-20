using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using GunMobile.Core;
using GunMobile.Res;
using UnityEngine;
using UnityEngine.Networking;

namespace GunMobile.Client
{
    public static class PcContent
    {
        public const string Folder = "PcData";

        public static string StreamingPcData => Path.Combine(Application.streamingAssetsPath, Folder);
        public static string PersistentPcData => Path.Combine(Application.persistentDataPath, Folder);

        public static ResLoader CreateLoader()
        {
            var loader = new ResLoader(StreamingPcData, PersistentPcData);
            string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            string unpacked = Path.Combine(repoRoot, "legacy", "unpacked");
            if (Directory.Exists(unpacked))
            {
                loader.ExtraRoots.Add(unpacked);
            }

            string repoData = Path.Combine(repoRoot, "legacy", "data");
            if (Directory.Exists(repoData))
            {
                loader.ExtraRoots.Add(repoData);
            }

            return loader;
        }

        public static IEnumerator Install(ResLoader loader, Action<string> status = null)
        {
            Directory.CreateDirectory(PersistentPcData);

            if (!File.Exists(Path.Combine(PersistentPcData, ".ready")))
            {
                yield return InstallContentIndex(status);
            }

            yield return CopyEquipGameManifest(status);
            yield return EnsureEquipArmAssets(status);
        }

        static IEnumerator InstallContentIndex(Action<string> status)
        {
            byte[] indexBytes = null;
            yield return ReadStreaming("content_index.json", b => indexBytes = b);
            if (indexBytes == null || indexBytes.Length == 0)
            {
                status?.Invoke("No content index; using live search paths.");
                yield break;
            }

            string json = ZlibXml.DecodeText(indexBytes);
            var files = ParseFiles(json);
            int copied = 0;
            for (int i = 0; i < files.Count; i++)
            {
                string rel = files[i];
                string dest = Path.Combine(PersistentPcData, rel);
                if (File.Exists(dest))
                {
                    continue;
                }

                byte[] data = null;
                yield return ReadStreaming(rel, b => data = b);
                if (data == null)
                {
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? PersistentPcData);
                File.WriteAllBytes(dest, data);
                copied++;
                if (copied % 40 == 0)
                {
                    status?.Invoke($"Unpack PC data {i + 1}/{files.Count}");
                    yield return null;
                }
            }

            File.WriteAllText(Path.Combine(PersistentPcData, ".ready"), copied.ToString());
        }

        static IEnumerator EnsureEquipArmAssets(Action<string> status)
        {
            if (Application.isBatchMode)
            {
                yield break;
            }

            string marker = Path.Combine(PersistentPcData, ".equip_ready");
            if (File.Exists(marker))
            {
                yield break;
            }

            string equipDest = Path.Combine(PersistentPcData, "Resource", "image", "equip");
            if (Directory.Exists(equipDest))
            {
                File.WriteAllText(marker, "local");
                yield break;
            }

#if UNITY_EDITOR
            yield return CopyUnpackedEquipAssets(status);
            if (Directory.Exists(equipDest))
            {
                File.WriteAllText(marker, "editor-unpacked");
                yield break;
            }
#endif

            yield return DownloadEquipArmBundle(status);
            if (Directory.Exists(equipDest))
            {
                File.WriteAllText(marker, "bundle");
            }
        }

        static IEnumerator DownloadEquipArmBundle(Action<string> status)
        {
            byte[] srcBytes = null;
            yield return ReadStreaming("pc_asset_sources.json", b => srcBytes = b);
            if (srcBytes == null || srcBytes.Length == 0)
            {
                status?.Invoke("No costume bundle URL; hall icons may be missing.");
                yield break;
            }

            string json = System.Text.Encoding.UTF8.GetString(srcBytes);
            if (!TryParseEquipBundle(json, out string url, out string expectedSha, out long expectedSize))
            {
                status?.Invoke("Invalid pc_asset_sources.json");
                yield break;
            }

            string zipPath = Path.Combine(PersistentPcData, "equip_arm_bundle.zip");
            if (!File.Exists(zipPath) || (expectedSize > 0 && new FileInfo(zipPath).Length != expectedSize))
            {
                status?.Invoke("Downloading PC costumes (~95 MB)…");
                yield return DownloadFile(url, zipPath, status);
            }

            if (!File.Exists(zipPath))
            {
                status?.Invoke("Costume download failed.");
                yield break;
            }

            if (!string.IsNullOrEmpty(expectedSha) && !Sha256Matches(zipPath, expectedSha))
            {
                File.Delete(zipPath);
                status?.Invoke("Costume bundle checksum failed; retry later.");
                yield break;
            }

            status?.Invoke("Installing PC costumes…");
            yield return null;
            try
            {
                ExtractZip(zipPath, PersistentPcData);
                status?.Invoke("PC costume assets ready.");
            }
            catch (Exception ex)
            {
                status?.Invoke("Costume install failed: " + ex.Message);
            }
        }

        static IEnumerator DownloadFile(string url, string destPath, Action<string> status)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destPath) ?? PersistentPcData);
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }

            using (var req = UnityWebRequest.Get(url))
            {
                req.downloadHandler = new DownloadHandlerFile(destPath);
                var op = req.SendWebRequest();
                while (!op.isDone)
                {
                    if (req.downloadProgress > 0.01f)
                    {
                        status?.Invoke($"Downloading costumes {Mathf.RoundToInt(req.downloadProgress * 100f)}%");
                    }

                    yield return null;
                }

                if (req.result != UnityWebRequest.Result.Success)
                {
                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);
                    }

                    status?.Invoke("Download error: " + req.error);
                }
            }
        }

        static void ExtractZip(string zipPath, string destRoot)
        {
            using (FileStream fs = File.OpenRead(zipPath))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        continue;
                    }

                    string rel = entry.FullName.Replace('\\', '/');
                    string dest = Path.Combine(destRoot, rel);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? destRoot);
                    using (Stream src = entry.Open())
                    using (FileStream dst = File.Create(dest))
                    {
                        src.CopyTo(dst);
                    }
                }
            }
        }

        static bool Sha256Matches(string path, string expectedHex)
        {
            using (var sha = SHA256.Create())
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] hash = sha.ComputeHash(fs);
                string hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                return string.Equals(hex, expectedHex.Trim().ToLowerInvariant(), StringComparison.Ordinal);
            }
        }

        static bool TryParseEquipBundle(string json, out string url, out string sha256, out long sizeBytes)
        {
            url = null;
            sha256 = null;
            sizeBytes = 0;
            int block = json.IndexOf("\"equipArmBundle\"", StringComparison.Ordinal);
            if (block < 0)
            {
                return false;
            }

            url = ParseJsonString(json, "url", block);
            sha256 = ParseJsonString(json, "sha256", block);
            sizeBytes = ParseJsonLong(json, "sizeBytes", block);

            return !string.IsNullOrEmpty(url);
        }

        static long ParseJsonLong(string json, string key, int searchFrom = 0)
        {
            string token = "\"" + key + "\"";
            int i = json.IndexOf(token, searchFrom, StringComparison.Ordinal);
            if (i < 0)
            {
                return 0;
            }

            i = json.IndexOf(':', i);
            if (i < 0)
            {
                return 0;
            }

            int j = i + 1;
            while (j < json.Length && char.IsWhiteSpace(json[j]))
            {
                j++;
            }

            int k = j;
            while (k < json.Length && (char.IsDigit(json[k]) || json[k] == '-'))
            {
                k++;
            }

            if (k <= j)
            {
                return 0;
            }

            long.TryParse(json.Substring(j, k - j), out long value);
            return value;
        }

        static string ParseJsonString(string json, string key, int searchFrom = 0)
        {
            string token = "\"" + key + "\"";
            int i = json.IndexOf(token, searchFrom, StringComparison.Ordinal);
            if (i < 0)
            {
                return null;
            }

            i = json.IndexOf(':', i);
            if (i < 0)
            {
                return null;
            }

            i = json.IndexOf('"', i + 1);
            if (i < 0)
            {
                return null;
            }

            int j = json.IndexOf('"', i + 1);
            if (j < 0)
            {
                return null;
            }

            return json.Substring(i + 1, j - i - 1);
        }

        static IEnumerator CopyEquipGameManifest(Action<string> status)
        {
            byte[] manifestBytes = null;
            yield return ReadStreaming("equip_game_manifest.json", b => manifestBytes = b);
            if (manifestBytes == null || manifestBytes.Length == 0)
            {
                yield break;
            }

            string json = System.Text.Encoding.UTF8.GetString(manifestBytes);
            var files = ParseFiles(json);
            int copied = 0;
            for (int i = 0; i < files.Count; i++)
            {
                string rel = files[i];
                string dest = Path.Combine(PersistentPcData, rel);
                if (File.Exists(dest))
                {
                    continue;
                }

                byte[] data = null;
                yield return ReadStreaming(rel, b => data = b);
                if (data == null)
                {
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? PersistentPcData);
                File.WriteAllBytes(dest, data);
                copied++;
            }

            if (copied > 0)
            {
                status?.Invoke($"Installed {copied} equip game.png sheets.");
            }
        }

        static IEnumerator CopyUnpackedEquipAssets(Action<string> status)
        {
            string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            string equipSrc = Path.Combine(repoRoot, "legacy", "unpacked", "Resource", "image", "equip");
            string armSrc = Path.Combine(repoRoot, "legacy", "unpacked", "Resource", "image", "arm");
            if (!Directory.Exists(equipSrc) && !Directory.Exists(armSrc))
            {
                yield break;
            }

            if (Directory.Exists(equipSrc))
            {
                string equipDest = Path.Combine(PersistentPcData, "Resource", "image", "equip");
                if (!Directory.Exists(equipDest))
                {
                    status?.Invoke("Copying PC equip assets…");
                    yield return null;
                    try
                    {
                        CopyDirectory(equipSrc, equipDest);
                    }
                    catch (Exception ex)
                    {
                        status?.Invoke("Equip copy skipped: " + ex.Message);
                    }
                }
            }

            if (Directory.Exists(armSrc))
            {
                string armDest = Path.Combine(PersistentPcData, "Resource", "image", "arm");
                if (!Directory.Exists(armDest))
                {
                    status?.Invoke("Copying PC arm assets…");
                    yield return null;
                    try
                    {
                        CopyDirectory(armSrc, armDest);
                        status?.Invoke("PC equip assets ready.");
                    }
                    catch (Exception ex)
                    {
                        status?.Invoke("Arm copy skipped: " + ex.Message);
                    }
                }
            }
        }

        static void CopyDirectory(string src, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (string dir in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dir.Replace(src, dest));
            }

            foreach (string file in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
            {
                string target = file.Replace(src, dest);
                Directory.CreateDirectory(Path.GetDirectoryName(target) ?? dest);
                if (!File.Exists(target))
                {
                    File.Copy(file, target);
                }
            }
        }

        static IEnumerator ReadStreaming(string relative, Action<byte[]> done)
        {
            string src = Path.Combine(StreamingPcData, relative).Replace("\\", "/");
#if UNITY_ANDROID && !UNITY_EDITOR
            using (UnityWebRequest req = UnityWebRequest.Get(src))
            {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    done(req.downloadHandler.data);
                }
                else
                {
                    done(null);
                }
            }
#else
            if (File.Exists(src))
            {
                done(File.ReadAllBytes(src));
            }
            else
            {
                done(null);
            }

            yield break;
#endif
        }

        static List<string> ParseFiles(string json)
        {
            var files = new List<string>();
            int start = json.IndexOf("\"files\"", StringComparison.Ordinal);
            if (start < 0)
            {
                return files;
            }

            int arr = json.IndexOf('[', start);
            int end = json.IndexOf(']', arr);
            if (arr < 0 || end < 0)
            {
                return files;
            }

            string body = json.Substring(arr + 1, end - arr - 1);
            foreach (string raw in body.Split(','))
            {
                string item = raw.Trim().Trim('"');
                if (item.Length > 0)
                {
                    files.Add(item.Replace("\\/", "/"));
                }
            }

            return files;
        }
    }
}
