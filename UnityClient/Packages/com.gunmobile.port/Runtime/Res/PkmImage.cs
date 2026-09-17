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

        // Khronos PKM header: "PKM ", a two byte version, then five big endian
        // UInt16s. The last pair is what makes a PKM more than a raw ETC blob:
        // ETC compresses 4x4 blocks, so the payload always covers dimensions
        // rounded up to a multiple of four, and the header records the real size
        // alongside so a reader can present the image at the size it was authored.
        const int DataTypeOffset = 6;
        const int ExtendedWidthOffset = 8;
        const int ExtendedHeightOffset = 10;
        const int OriginalWidthOffset = 12;
        const int OriginalHeightOffset = 14;

        public static bool IsPkm(byte[] data)
        {
            return data != null &&
                   data.Length > HeaderSize &&
                   data[0] == (byte)'P' &&
                   data[1] == (byte)'K' &&
                   data[2] == (byte)'M' &&
                   data[3] == (byte)' ';
        }

        /// <summary>
        /// Size the image was authored at, which is what callers must size sprites
        /// and atlas rects against.
        /// </summary>
        /// <remarks>
        /// This used to be read from <see cref="ExtendedWidthOffset"/>, the padded
        /// size, and the bake wrote the padded size into the original slot as well,
        /// so the real size was not recorded anywhere. 4724 of the 4984 images under
        /// StreamingAssets are not a multiple of four in both axes, so nearly every
        /// baked atlas came back 1-3 pixels taller than its .xml describes — and both
        /// <c>SpriteSheet.UnityRect</c> and <c>SpriteSheet.PixelToUv</c> turn
        /// Starling's top-left y into Unity's bottom-left one against the texture
        /// height, so every sprite in such an atlas is offset by exactly that padding.
        /// (<c>ResLoader.CreateUnitySprite</c> does the same conversion and is the
        /// obvious place to look, but it has no callers; the live path is
        /// SpriteSheet.)
        /// </remarks>
        public static bool TryReadSize(byte[] data, out int width, out int height)
        {
            width = 0;
            height = 0;
            if (!IsPkm(data))
            {
                return false;
            }

            int extendedWidth = BigEndian16(data, ExtendedWidthOffset);
            int extendedHeight = BigEndian16(data, ExtendedHeightOffset);
            width = BigEndian16(data, OriginalWidthOffset);
            height = BigEndian16(data, OriginalHeightOffset);

            // Some writers leave the original size at zero. Padded is then the best
            // answer available, and it is never smaller than the truth.
            if (width <= 0 || width > extendedWidth)
            {
                width = extendedWidth;
            }

            if (height <= 0 || height > extendedHeight)
            {
                height = extendedHeight;
            }

            return width > 0 && height > 0;
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
            if (!TryReadSize(data, out int width, out int height))
            {
                return null;
            }

            TextureFormat texFormat = ResolveFormat(etc2, BigEndian16(data, DataTypeOffset));
            if (!SystemInfo.SupportsTextureFormat(texFormat))
            {
                return null;
            }

            int payload = data.Length - HeaderSize;
            if (payload <= 0)
            {
                return null;
            }

            // Unity sizes a compressed texture's raw data by rounding the dimensions
            // up to whole blocks, so the padded payload fits a texture declared at
            // the original size and the padding stays inside the edge blocks.
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

        static int BigEndian16(byte[] data, int offset)
        {
            return (data[offset] << 8) | data[offset + 1];
        }

        /// <summary>Maps the header's data type to the Unity format that matches it.</summary>
        static TextureFormat ResolveFormat(bool etc2, int dataType)
        {
            if (!etc2)
            {
                // PKM 1.0 carries ETC1 and nothing else.
                return TextureFormat.ETC_RGB4;
            }

            switch (dataType)
            {
                case 0:
                    return TextureFormat.ETC_RGB4;
                case 1:
                    return TextureFormat.ETC2_RGB;
                case 3:
                    return TextureFormat.ETC2_RGBA8;
                case 4:
                    return TextureFormat.ETC2_RGBA1;
                case 5:
                    return TextureFormat.EAC_R;
                case 6:
                    return TextureFormat.EAC_RG;
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

        /// <summary>
        /// Header for an ETC payload covering <paramref name="width"/> x
        /// <paramref name="height"/> rounded up to whole 4x4 blocks. Pass the size
        /// the image was authored at, not the padded size — recording the real one
        /// is the entire point of the last field.
        /// </summary>
        public static byte[] WriteHeader(int width, int height, bool etc2Rgba = true)
        {
            int extendedWidth = (width + 3) & ~3;
            int extendedHeight = (height + 3) & ~3;
            var header = new byte[HeaderSize];
            header[0] = (byte)'P';
            header[1] = (byte)'K';
            header[2] = (byte)'M';
            header[3] = (byte)' ';
            header[4] = (byte)'2';
            header[5] = (byte)'0';
            WriteBigEndian16(header, DataTypeOffset, etc2Rgba ? 3 : 1);
            WriteBigEndian16(header, ExtendedWidthOffset, extendedWidth);
            WriteBigEndian16(header, ExtendedHeightOffset, extendedHeight);
            WriteBigEndian16(header, OriginalWidthOffset, width);
            WriteBigEndian16(header, OriginalHeightOffset, height);
            return header;
        }

        static void WriteBigEndian16(byte[] data, int offset, int value)
        {
            data[offset] = (byte)((value >> 8) & 0xFF);
            data[offset + 1] = (byte)(value & 0xFF);
        }
    }
}
