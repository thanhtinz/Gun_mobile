using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
#if UNITY_EDITOR
            string repoData = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "legacy", "data"));
            if (Directory.Exists(repoData))
            {
                loader.ExtraRoots.Add(repoData);
            }
#endif
            return loader;
        }

        public static IEnumerator Install(ResLoader loader, Action<string> status = null)
        {
            Directory.CreateDirectory(PersistentPcData);
            if (File.Exists(Path.Combine(PersistentPcData, ".ready")))
            {
                yield break;
            }

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
