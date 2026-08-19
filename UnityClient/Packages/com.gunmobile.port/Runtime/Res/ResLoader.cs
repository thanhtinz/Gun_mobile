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
        public List<string> ExtraRoots { get; } = new List<string>();

        public ResLoader(string streamingRoot = null, string persistentRoot = null)
        {
            StreamingRoot = streamingRoot ?? Application.streamingAssetsPath;
            PersistentRoot = persistentRoot ?? Application.persistentDataPath;
        }

        public IEnumerable<string> SearchRoots()
        {
            if (!string.IsNullOrEmpty(PersistentRoot))
            {
                yield return PersistentRoot;
            }

            foreach (string root in ExtraRoots)
            {
                if (!string.IsNullOrEmpty(root))
                {
                    yield return root;
                }
            }

            if (!string.IsNullOrEmpty(StreamingRoot))
            {
                yield return StreamingRoot;
            }
        }

        public bool TryReadBytes(string relative, out byte[] bytes)
        {
            relative = GamePaths.Normalize(relative);
            foreach (string root in SearchRoots())
            {
                string path = Path.Combine(root, relative);
                if (File.Exists(path))
                {
                    bytes = File.ReadAllBytes(path);
                    return true;
                }
            }

            bytes = null;
            return false;
        }

        public bool Exists(string relative)
        {
            relative = GamePaths.Normalize(relative);
            foreach (string root in SearchRoots())
            {
                if (File.Exists(Path.Combine(root, relative)))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> ListFiles(string relativeDir, string fileName = null)
        {
            relativeDir = GamePaths.Normalize(relativeDir);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string root in SearchRoots())
            {
                string dir = Path.Combine(root, relativeDir);
                if (!Directory.Exists(dir))
                {
                    continue;
                }

                IEnumerable<string> files = string.IsNullOrEmpty(fileName)
                    ? Directory.GetFiles(dir, "*", SearchOption.AllDirectories)
                    : Directory.GetFiles(dir, fileName, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    string rel = GamePaths.Normalize(file.Substring(root.Length).TrimStart('/', '\\'));
                    if (seen.Add(rel))
                    {
                        yield return rel;
                    }
                }
            }
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

        public bool TryReadImageBytes(string relative, out byte[] bytes)
        {
            relative = GamePaths.Normalize(relative);
            string pkm = PkmImage.ToPkmPath(relative);
            if (!string.Equals(pkm, relative, StringComparison.OrdinalIgnoreCase) && TryReadBytes(pkm, out bytes))
            {
                return true;
            }

            return TryReadBytes(relative, out bytes);
        }

        public Texture2D ReadTexture(string relative, bool linear = false)
        {
            if (!TryReadImageBytes(relative, out byte[] bytes))
            {
                throw new FileNotFoundException("Missing game resource: " + relative);
            }

            Texture2D tex = PkmImage.IsPkm(bytes)
                ? PkmImage.Load(bytes, true)
                : SpriteSheet.LoadTexture(bytes, true);
            if (tex == null)
            {
                throw new InvalidDataException("Not an image: " + relative);
            }

            tex.name = Path.GetFileName(relative);
            if (linear)
            {
                // PKM stays GPU-compressed; PNG path already loaded as RGBA32.
            }

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
