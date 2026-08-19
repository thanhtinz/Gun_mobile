using System.IO;
using GunMobile.Res;
using UnityEditor;
using UnityEngine;

namespace GunMobile.EditorTools
{
    /// <summary>Bake PNG/JPG under PcData to Khronos .pkm (ETC2_RGBA8) for mobile GPU.</summary>
    public static class PkmBakeMenu
    {
        const string PcDataRoot = "Assets/StreamingAssets/PcData";

        [MenuItem("GunMobile/Bake PKM (ETC2) from StreamingAssets PNG")]
        public static void BakeStreamingAssetsPkm()
        {
            string root = Path.Combine(Application.dataPath, "StreamingAssets", "PcData");
            if (!Directory.Exists(root))
            {
                Debug.LogWarning("GunMobile PKM: missing StreamingAssets/PcData");
                return;
            }

            int ok = 0;
            int skip = 0;
            foreach (string file in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                {
                    continue;
                }

                if (TryBakeFile(file))
                {
                    ok++;
                }
                else
                {
                    skip++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"GunMobile PKM bake done: {ok} written, {skip} skipped.");
        }

        [MenuItem("GunMobile/Bake PKM for selected PNG textures")]
        public static void BakeSelectedPkm()
        {
            int ok = 0;
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                string full = Path.GetFullPath(path);
                if (TryBakeFile(full))
                {
                    ok++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"GunMobile PKM: baked {ok} selected file(s).");
        }

        public static bool TryBakeFile(string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                return false;
            }

            byte[] srcBytes = File.ReadAllBytes(sourcePath);
            var src = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            if (!src.LoadImage(srcBytes, true))
            {
                Object.DestroyImmediate(src);
                return false;
            }

            int w = src.width;
            int h = src.height;
            int pw = (w + 3) & ~3;
            int ph = (h + 3) & ~3;

            Texture2D padded = src;
            if (pw != w || ph != h)
            {
                padded = new Texture2D(pw, ph, TextureFormat.RGBA32, false, false);
                var clear = new Color[w * h];
                for (int i = 0; i < clear.Length; i++)
                {
                    clear[i] = Color.clear;
                }

                padded.SetPixels(clear);
                padded.SetPixels(0, 0, w, h, src.GetPixels());
                padded.Apply(false, true);
                Object.DestroyImmediate(src);
            }

            EditorUtility.CompressTexture(padded, TextureFormat.ETC2_RGBA8, TextureCompressionQuality.Best);
            byte[] etc = padded.GetRawTextureData();
            Object.DestroyImmediate(padded);

            if (etc == null || etc.Length == 0)
            {
                return false;
            }

            byte[] header = PkmImage.WriteHeader(pw, ph, true);
            string outPath = PkmImage.ToPkmPath(sourcePath);
            using (var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(header, 0, header.Length);
                fs.Write(etc, 0, etc.Length);
            }

            return true;
        }
    }
}
