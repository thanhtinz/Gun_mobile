using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using GunMobile.Core;
using UnityEngine;

namespace GunMobile.Res
{
    /// <summary>
    /// Resolves PC resource paths against StreamingAssets then persistentDataPath.
    /// Unpack the Ok zips (or a CDN subset) into persistentDataPath to match the original site layout.
    /// </summary>
    public sealed class ResLoader
    {
        public string StreamingRoot { get; }
        public string PersistentRoot { get; }

        public ResLoader(string streamingRoot = null, string persistentRoot = null)
        {
            StreamingRoot = streamingRoot ?? Application.streamingAssetsPath;
            PersistentRoot = persistentRoot ?? Application.persistentDataPath;
        }

        public bool TryReadBytes(string relative, out byte[] bytes)
        {
            relative = GamePaths.Normalize(relative);
            string persistent = Path.Combine(PersistentRoot, relative);
            if (File.Exists(persistent))
            {
                bytes = File.ReadAllBytes(persistent);
                return true;
            }

            string streaming = Path.Combine(StreamingRoot, relative);
            if (File.Exists(streaming))
            {
                bytes = File.ReadAllBytes(streaming);
                return true;
            }

            bytes = null;
            return false;
        }

        public byte[] ReadBytes(string relative)
        {
            if (!TryReadBytes(relative, out byte[] bytes))
            {
                throw new FileNotFoundException("Missing game resource: " + relative);
            }

            return bytes;
        }

        public XDocument ReadXml(string relative)
        {
            return ZlibXml.Load(ReadBytes(relative));
        }

        public XmlResultTable ReadTable(string relative)
        {
            return XmlResultTable.LoadBytes(ReadBytes(relative));
        }

        public Texture2D ReadTexture(string relative, bool linear = false)
        {
            byte[] bytes = ReadBytes(relative);
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, linear);
            if (!tex.LoadImage(bytes, true))
            {
                UnityEngine.Object.Destroy(tex);
                throw new InvalidDataException("Not an image: " + relative);
            }

            tex.name = Path.GetFileName(relative);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        public static string Foreground(int mapId) => GamePaths.PathCombine(GamePaths.MapImage(mapId), "fore.png");
        public static string Background(int mapId) => GamePaths.PathCombine(GamePaths.MapImage(mapId), "back.jpg");
        public static string MiniMap(int mapId) => GamePaths.PathCombine(GamePaths.MapImage(mapId), "samll_map.png");
    }

    public sealed class AtlasSprite
    {
        public string Name;
        public Rect Region;
        public Rect? Frame;
        public Vector2 Pivot;
    }

    /// <summary>
    /// Starling / TexturePacker atlas used by Flash/ui/*/starling/*.xml
    /// </summary>
    public sealed class TextureAtlasParser
    {
        public string ImagePath { get; private set; }
        public IReadOnlyList<AtlasSprite> Sprites { get; private set; }

        public static TextureAtlasParser Parse(XDocument doc)
        {
            var parser = new TextureAtlasParser();
            XElement root = doc.Root;
            parser.ImagePath = root?.Attribute("imagePath")?.Value ?? string.Empty;
            var list = new List<AtlasSprite>();
            if (root != null)
            {
                foreach (XElement sub in root.Elements("SubTexture"))
                {
                    int x = Int(sub, "x");
                    int y = Int(sub, "y");
                    int w = Int(sub, "width");
                    int h = Int(sub, "height");
                    var sprite = new AtlasSprite
                    {
                        Name = sub.Attribute("name")?.Value ?? string.Empty,
                        Region = new Rect(x, y, w, h),
                        Pivot = new Vector2(0.5f, 0.5f)
                    };

                    if (sub.Attribute("frameWidth") != null)
                    {
                        sprite.Frame = new Rect(
                            Int(sub, "frameX"),
                            Int(sub, "frameY"),
                            Int(sub, "frameWidth"),
                            Int(sub, "frameHeight"));
                    }

                    list.Add(sprite);
                }
            }

            parser.Sprites = list;
            return parser;
        }

        public Sprite CreateUnitySprite(Texture2D atlas, AtlasSprite info)
        {
            // Starling y is top-left; Unity sprite rect is bottom-left.
            float y = atlas.height - info.Region.y - info.Region.height;
            var rect = new Rect(info.Region.x, y, info.Region.width, info.Region.height);
            return Sprite.Create(atlas, rect, info.Pivot, 100f, 0, SpriteMeshType.FullRect);
        }

        private static int Int(XElement el, string name)
        {
            XAttribute attr = el.Attribute(name);
            if (attr == null)
            {
                return 0;
            }

            int.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value);
            return value;
        }
    }
}
