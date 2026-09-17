using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using GunMobile.Core;
using UnityEngine;

namespace GunMobile.Res
{
    public struct SheetFrame
    {
        public string Name;
        public Rect Pixel;
        public Rect Uv;
        public Vector2 Size;
    }

    /// <summary>
    /// PNG, or a TexturePacker atlas packed as a zip (PC living948.png is PK-zip
    /// of xml+png). Starling SubTexture y is top-left; Unity UV y is bottom-left.
    /// </summary>
    public sealed class SpriteSheet
    {
        public Texture2D Texture { get; private set; }
        public List<SheetFrame> Frames { get; } = new List<SheetFrame>();

        public static SpriteSheet LoadBytes(byte[] data)
        {
            if (data == null || data.Length < 8)
            {
                return null;
            }

            // Strip before testing the signature, not after. 8 of the 90 zip atlases
            // in the dump are obfuscated (the mount art under image/mounts/horse), so
            // testing the raw bytes sees the five byte prefix instead of "PK": they
            // took the PNG path, which cannot decode a zip, and loaded as nothing.
            data = Obfuscation.Strip(data);
            if (data[0] == 0x50 && data[1] == 0x4B)
            {
                return LoadZipAtlas(data);
            }

            return LoadPng(data);
        }

        public static SpriteSheet TryLoad(ResLoader loader, params string[] paths)
        {
            if (loader == null)
            {
                return null;
            }

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path) || !loader.TryReadBytes(path, out byte[] bytes))
                {
                    continue;
                }

                SpriteSheet sheet = LoadBytes(bytes);
                if (sheet != null && sheet.Texture != null)
                {
                    return sheet;
                }
            }

            return null;
        }

        /// <summary>PC Starling atlas: PNG + XML under Flash/ui/*/starling.</summary>
        public static SpriteSheet TryLoadStarling(ResLoader loader, string pngPath, string xmlPath)
        {
            if (loader == null || !loader.TryReadBytes(pngPath, out byte[] png))
            {
                return null;
            }

            Texture2D tex = LoadTexture(png, false);
            if (tex == null)
            {
                return null;
            }

            var sheet = new SpriteSheet { Texture = tex };
            if (loader.TryReadBytes(xmlPath, out byte[] xml))
            {
                try
                {
                    FillFromAtlas(sheet, ZlibXml.Load(xml), tex.width, tex.height);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Starling xml: " + e.Message);
                }
            }

            if (sheet.Frames.Count == 0)
            {
                sheet.Frames.Add(new SheetFrame
                {
                    Name = "full",
                    Pixel = new Rect(0, 0, tex.width, tex.height),
                    Uv = new Rect(0f, 0f, 1f, 1f),
                    Size = new Vector2(tex.width, tex.height)
                });
            }

            return sheet;
        }

        // Morn skin lookup: đổi một frame trong atlas thành Sprite (cache theo tên).
        public Sprite Get(string name)
        {
            if (Texture == null || string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (_spriteCache.TryGetValue(name, out Sprite cached))
            {
                return cached;
            }

            if (!TryGet(name, out SheetFrame frame))
            {
                return null;
            }

            if (frame.Pixel.width <= 0f || frame.Pixel.height <= 0f)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(Texture, UnityRect(frame), new Vector2(0.5f, 0.5f), 100f);
            _spriteCache[name] = sprite;
            return sprite;
        }

        /// <summary>
        /// <paramref name="frame"/>'s rectangle in the space <see cref="Sprite.Create"/>
        /// expects.
        /// </summary>
        /// <remarks>
        /// <see cref="SheetFrame.Pixel"/> keeps Starling's y, measured from the top of
        /// the texture; Unity measures a sprite rect from the bottom. Handing
        /// <c>Pixel</c> to <c>Sprite.Create</c> unchanged mirrors the frame about the
        /// texture's middle, so the sprite is cut from a different part of the atlas
        /// — 285 of the 286 frames in the six shipped Starling atlases, a median of
        /// 268px and as much as 1585px away from where they belong.
        /// <para>
        /// The conversion lives here so it is written once. It was previously spelled
        /// out in PcSkin.Chrome, correctly, and left out of <see cref="Get"/>, which
        /// is what MornScreenHost calls for every Morn widget's skin.
        /// </para>
        /// </remarks>
        public Rect UnityRect(SheetFrame frame)
        {
            float height = Texture != null ? Texture.height : 0f;
            return new Rect(
                frame.Pixel.x,
                height - frame.Pixel.y - frame.Pixel.height,
                frame.Pixel.width,
                frame.Pixel.height);
        }

        readonly Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

        public bool TryGet(string name, out SheetFrame frame)
        {
            frame = default;
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            for (int i = 0; i < Frames.Count; i++)
            {
                if (string.Equals(Frames[i].Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    frame = Frames[i];
                    return true;
                }
            }

            return TryUv(name, out frame);
        }

        public static Texture2D LoadTexture(byte[] data, bool readable)
        {
            if (PkmImage.IsPkm(data))
            {
                return PkmImage.Load(data, readable);
            }

            data = StripToPng(data);
            if (data == null)
            {
                return null;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(data, !readable))
            {
                UnityEngine.Object.Destroy(tex);
                return null;
            }

            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        /// <summary>Offset of the IHDR width's most significant byte inside a PNG stream.</summary>
        /// <summary>
        /// Recovers a loadable PNG from a legacy resource file.
        /// </summary>
        /// <remarks>
        /// Obfuscated files need both halves of the scheme undone, which is
        /// <see cref="Obfuscation"/>'s job; this used to clear the corrupted IHDR
        /// width byte itself, which was right for PNG and useless for every other
        /// format the same packer wrapped. What is left here is the fallback for a
        /// file that merely has some other junk ahead of the PNG signature.
        /// </remarks>
        public static byte[] StripToPng(byte[] data)
        {
            if (data == null || data.Length < 8)
            {
                return null;
            }

            data = Obfuscation.Strip(data);
            if (data[0] == 0x89 && data[1] == 0x50)
            {
                return data;
            }

            for (int i = 1; i < Mathf.Min(32, data.Length - 8); i++)
            {
                if (data[i] == 0x89 && data[i + 1] == 0x50 && data[i + 2] == 0x4E && data[i + 3] == 0x47)
                {
                    var slice = new byte[data.Length - i];
                    System.Buffer.BlockCopy(data, i, slice, 0, slice.Length);
                    return slice;
                }
            }

            return data;
        }

        public bool TryUv(string nameContains, out SheetFrame frame)
        {
            frame = default;
            if (string.IsNullOrEmpty(nameContains))
            {
                return false;
            }

            for (int i = 0; i < Frames.Count; i++)
            {
                if (Frames[i].Name.IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    frame = Frames[i];
                    return true;
                }
            }

            return false;
        }

        public List<SheetFrame> Sequence(string prefix)
        {
            var list = new List<SheetFrame>();
            for (int i = 0; i < Frames.Count; i++)
            {
                if (Frames[i].Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
                    Frames[i].Name.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    list.Add(Frames[i]);
                }
            }

            list.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
            return list;
        }

        static SpriteSheet LoadPng(byte[] data)
        {
            Texture2D tex = LoadTexture(data, false);
            if (tex == null)
            {
                return null;
            }

            var sheet = new SpriteSheet { Texture = tex };
            var pixel = new Rect(0, 0, tex.width, tex.height);
            sheet.Frames.Add(new SheetFrame
            {
                Name = "full",
                Pixel = pixel,
                Uv = new Rect(0f, 0f, 1f, 1f),
                Size = new Vector2(tex.width, tex.height)
            });
            return sheet;
        }

        static SpriteSheet LoadZipAtlas(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream(data, false))
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Read))
                {
                    byte[] png = null;
                    byte[] xml = null;
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        string name = entry.FullName.Replace('\\', '/');
                        string low = name.ToLowerInvariant();
                        if (low.EndsWith(".png") && png == null)
                        {
                            png = ReadEntry(entry);
                        }
                        else if (low.EndsWith(".xml") && xml == null)
                        {
                            xml = ReadEntry(entry);
                        }
                    }

                    if (png == null)
                    {
                        return null;
                    }

                    Texture2D tex = LoadTexture(png, false);
                    if (tex == null)
                    {
                        return null;
                    }

                    var sheet = new SpriteSheet { Texture = tex };
                    if (xml != null)
                    {
                        FillFromAtlas(sheet, ZlibXml.Load(xml), tex.width, tex.height);
                    }

                    if (sheet.Frames.Count == 0)
                    {
                        sheet.Frames.Add(new SheetFrame
                        {
                            Name = "full",
                            Pixel = new Rect(0, 0, tex.width, tex.height),
                            Uv = new Rect(0f, 0f, 1f, 1f),
                            Size = new Vector2(tex.width, tex.height)
                        });
                    }

                    return sheet;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("SpriteSheet zip: " + e.Message);
                return null;
            }
        }

        static void FillFromAtlas(SpriteSheet sheet, XDocument doc, int tw, int th)
        {
            TextureAtlasParser atlas = TextureAtlasParser.Parse(doc);
            foreach (AtlasSprite info in atlas.Sprites)
            {
                var pixel = new Rect(info.Region.x, info.Region.y, info.Region.width, info.Region.height);
                sheet.Frames.Add(new SheetFrame
                {
                    Name = info.Name ?? "",
                    Pixel = pixel,
                    Uv = PixelToUv(pixel, tw, th),
                    Size = new Vector2(info.Region.width, info.Region.height)
                });
            }
        }

        public static Rect PixelToUv(Rect pixel, int texW, int texH)
        {
            if (texW <= 0 || texH <= 0)
            {
                return new Rect(0f, 0f, 1f, 1f);
            }

            return new Rect(
                pixel.x / texW,
                1f - (pixel.y + pixel.height) / texH,
                pixel.width / texW,
                pixel.height / texH);
        }

        static byte[] ReadEntry(ZipArchiveEntry entry)
        {
            using (Stream s = entry.Open())
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
