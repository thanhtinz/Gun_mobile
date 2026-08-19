using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace GunMobile.Res
{
    public sealed class CharacterPart
    {
        public string Resource;
        public int Width;
        public int Height;
        public int[] Frames = Array.Empty<int>();
        public Vector2[] Points = Array.Empty<Vector2>();
    }

    public sealed class CharacterAction
    {
        public string Name;
        public string Next;
        public int Sound;
        public List<CharacterPart> Parts = new List<CharacterPart>();
    }

    /// <summary>
    /// Flash/characterdefine.xml — layered 2D character (head/body/effect) used by the PC client.
    /// </summary>
    public sealed class CharacterDefine
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Register { get; private set; }
        public IReadOnlyDictionary<string, CharacterAction> Actions { get; private set; }

        public static CharacterDefine Load(XDocument doc)
        {
            XElement root = doc.Root;
            var def = new CharacterDefine
            {
                Width = Int(root, "width"),
                Height = Int(root, "height"),
                Register = new Vector2(Int(root, "registerX"), Int(root, "registerY"))
            };

            var actions = new Dictionary<string, CharacterAction>(StringComparer.OrdinalIgnoreCase);
            XElement set = root.Element("actionSet");
            if (set != null)
            {
                foreach (XElement actionEl in set.Elements("action"))
                {
                    var action = new CharacterAction
                    {
                        Name = actionEl.Attribute("name")?.Value,
                        Next = actionEl.Attribute("next")?.Value,
                        Sound = Int(actionEl, "sound")
                    };

                    foreach (XElement asset in actionEl.Elements("asset"))
                    {
                        action.Parts.Add(new CharacterPart
                        {
                            Resource = asset.Attribute("resource")?.Value,
                            Width = Int(asset, "width"),
                            Height = Int(asset, "height"),
                            Frames = SplitInts(asset.Attribute("frames")?.Value),
                            Points = SplitPoints(asset.Attribute("points")?.Value)
                        });
                    }

                    if (!string.IsNullOrEmpty(action.Name))
                    {
                        actions[action.Name] = action;
                    }
                }
            }

            def.Actions = actions;
            return def;
        }

        public Vector2 LocalOffset(CharacterPart part, int frameIndex)
        {
            if (part.Points == null || part.Points.Length == 0)
            {
                return Vector2.zero;
            }

            int i = Mathf.Clamp(frameIndex, 0, part.Points.Length - 1);
            Vector2 p = part.Points[i];
            return new Vector2(p.x - Register.x, Register.y - p.y);
        }

        private static int Int(XElement el, string name)
        {
            if (el == null)
            {
                return 0;
            }

            XAttribute attr = el.Attribute(name);
            if (attr == null)
            {
                return 0;
            }

            int.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value);
            return value;
        }

        private static int[] SplitInts(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return Array.Empty<int>();
            }

            return raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out int n) ? n : 0)
                .ToArray();
        }

        private static Vector2[] SplitPoints(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return Array.Empty<Vector2>();
            }

            string[] tokens = raw.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var pts = new Vector2[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                string[] xy = tokens[i].Split(',');
                float x = xy.Length > 0 && float.TryParse(xy[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float fx) ? fx : 0f;
                float y = xy.Length > 1 && float.TryParse(xy[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float fy) ? fy : 0f;
                pts[i] = new Vector2(x, y);
            }

            return pts;
        }
    }
}
