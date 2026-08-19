using System;
using UnityEngine;

namespace GunMobile.Res
{
    /// <summary>
    /// Khronos PKM container for ETC1/ETC2 (common on Android GPU).
    /// Bake PNG → .pkm via GunMobile → Bake PKM (ETC2) in Unity Editor or tools/png_to_pkm.py.
    /// </summary>
    public static class PkmImage
    {
        public const int HeaderSize = 16;

        public static bool IsPkm(byte[] data)
        {
            return data != null &&
                   data.Length > HeaderSize &&
                   data[0] == (byte)'P' &&
                   data[1] == (byte)'K' &&
                   data[2] == (byte)'M' &&
                   data[3] == (byte)' ';
        }

        public static Texture2D Load(byte[] data, bool readable = false)
        {
            if (!IsPkm(data))
            {
                return null;
            }

            if (data[4] != (byte)'1' && data[4] != (byte)'2')
            {
                return null;
            }

            bool etc2 = data[4] == (byte)'2';
            int width = (data[8] << 8) | data[9];
            int height = (data[10] << 8) | data[11];
            if (width <= 0 || height <= 0)
            {
                return null;
            }

            int format = (data[14] << 8) | data[15];
            TextureFormat texFormat = ResolveFormat(etc2, format);
            if (texFormat == TextureFormat.RGBA32 && !SystemInfo.SupportsTextureFormat(texFormat))
            {
                return null;
            }

            if (!SystemInfo.SupportsTextureFormat(texFormat))
            {
                return null;
            }

            int payload = data.Length - HeaderSize;
            if (payload <= 0)
            {
                return null;
            }

            var tex = new Texture2D(width, height, texFormat, false, false);
            var slice = new byte[payload];
            Buffer.BlockCopy(data, HeaderSize, slice, 0, payload);
            try
            {
                tex.LoadRawTextureData(slice);
                tex.Apply(false, !readable);
            }
            catch
            {
                UnityEngine.Object.Destroy(tex);
                return null;
            }

            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        static TextureFormat ResolveFormat(bool etc2, int format)
        {
            if (!etc2)
            {
                return TextureFormat.ETC_RGB4;
            }

            switch (format)
            {
                case 1:
                    return TextureFormat.ETC2_RGB;
                case 3:
                    return TextureFormat.ETC2_RGBA8;
                case 4:
                    return TextureFormat.ETC2_RGBA1;
                default:
                    return TextureFormat.ETC2_RGBA8;
            }
        }

        /// <summary>Prefer .pkm beside .png/.jpg path.</summary>
        public static string ToPkmPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return imagePath;
            }

            int dot = imagePath.LastIndexOf('.');
            if (dot < 0)
            {
                return imagePath + ".pkm";
            }

            return imagePath.Substring(0, dot) + ".pkm";
        }

        public static string[] WithPkmFallback(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Array.Empty<string>();
            }

            string pkm = ToPkmPath(path);
            if (string.Equals(pkm, path, StringComparison.OrdinalIgnoreCase))
            {
                return new[] { path };
            }

            return new[] { pkm, path };
        }

        public static byte[] WriteHeader(int width, int height, bool etc2Rgba = true)
        {
            int encodedW = (width + 3) >> 2;
            int encodedH = (height + 3) >> 2;
            var header = new byte[HeaderSize];
            header[0] = (byte)'P';
            header[1] = (byte)'K';
            header[2] = (byte)'M';
            header[3] = (byte)' ';
            header[4] = (byte)'2';
            header[5] = (byte)'0';
            header[6] = (byte)'\r';
            header[7] = (byte)'\n';
            header[8] = (byte)(width >> 8);
            header[9] = (byte)(width & 0xFF);
            header[10] = (byte)(height >> 8);
            header[11] = (byte)(height & 0xFF);
            header[12] = (byte)encodedW;
            header[13] = (byte)encodedH;
            header[14] = 0;
            header[15] = (byte)(etc2Rgba ? 3 : 1);
            return header;
        }
    }
}
