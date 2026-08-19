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

        public static Texture2D LoadTexture(byte[] png, bool readable)
        {
            png = StripToPng(png);
            if (png == null)
            {
                return null;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(png, !readable))
            {
                UnityEngine.Object.Destroy(tex);
                return null;
            }

            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        public static byte[] StripToPng(byte[] data)
        {
            if (data == null || data.Length < 8)
            {
                return null;
            }

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
