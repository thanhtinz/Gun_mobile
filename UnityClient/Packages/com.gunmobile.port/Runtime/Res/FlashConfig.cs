using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using GunMobile.Core;

namespace GunMobile.Res
{
    public sealed class FeatureFlag
    {
        public string Name;
        public bool Enabled = true;
        public string Value = string.Empty;
        public Dictionary<string, string> Attrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Parses Flash/config.xml (CDN roots, language, module toggles, 25fps battle budget).
    /// </summary>
    public sealed class FlashConfig
    {
        public string FlashSite { get; private set; }
        public string ResourceSite { get; private set; }
        public string RequestPath { get; private set; }
        public string Language { get; private set; } = GamePaths.DefaultLanguage;
        public int FrameTimeOverMs { get; private set; } = 67;
        public int SuicideTime { get; private set; } = 120;
        public int StrengthMax { get; private set; } = 12;
        public IReadOnlyDictionary<string, FeatureFlag> Flags { get; private set; }

        public static FlashConfig Load(XDocument doc)
        {
            var cfg = new FlashConfig();
            XElement config = doc.Root?.Element("config");
            if (config == null)
            {
                throw new InvalidOperationException("config.xml missing <config>");
            }

            cfg.FlashSite = Val(config.Element("FLASHSITE"));
            cfg.ResourceSite = Val(config.Element("SITE"));
            cfg.RequestPath = Val(config.Element("REQUEST_PATH"));
            cfg.Language = Val(config.Element("LANGUAGE"), GamePaths.DefaultLanguage);
            cfg.SuicideTime = IntVal(config.Element("SUCIDE_TIME"), 120);
            cfg.StrengthMax = IntVal(config.Element("STHRENTH_MAX"), 12);

            XElement frame = config.Element("GAME_FRAME_CONFIG");
            if (frame != null)
            {
                cfg.FrameTimeOverMs = IntVal(frame.Element("FRAME_TIME_OVER_TAG"), 67);
            }

            var flags = new Dictionary<string, FeatureFlag>(StringComparer.OrdinalIgnoreCase);
            CollectFlags(config, flags);
            cfg.Flags = flags;
            return cfg;
        }

        public bool IsEnabled(string name, bool fallback = true)
        {
            if (Flags != null && Flags.TryGetValue(name, out FeatureFlag flag))
            {
                return flag.Enabled;
            }

            return fallback;
        }

        private static void CollectFlags(XElement parent, Dictionary<string, FeatureFlag> dst)
        {
            foreach (XElement el in parent.Elements())
            {
                var flag = new FeatureFlag { Name = el.Name.LocalName };
                foreach (XAttribute attr in el.Attributes())
                {
                    flag.Attrs[attr.Name.LocalName] = attr.Value;
                    if (attr.Name.LocalName.Equals("enable", StringComparison.OrdinalIgnoreCase) ||
                        attr.Name.LocalName.Equals("value", StringComparison.OrdinalIgnoreCase))
                    {
                        if (bool.TryParse(attr.Value, out bool b))
                        {
                            flag.Enabled = b;
                        }
                        else
                        {
                            flag.Value = attr.Value;
                            if (attr.Value == "false" || attr.Value == "0")
                            {
                                flag.Enabled = false;
                            }
                        }
                    }
                }

                if (el.Attribute("enable") != null || el.Attribute("value") != null)
                {
                    dst[flag.Name] = flag;
                }

                if (el.HasElements)
                {
                    CollectFlags(el, dst);
                }
            }
        }

        private static string Val(XElement el, string fallback = "")
        {
            if (el == null)
            {
                return fallback;
            }

            XAttribute attr = el.Attribute("value");
            return attr != null ? attr.Value : fallback;
        }

        private static int IntVal(XElement el, int fallback)
        {
            string raw = Val(el, null);
            if (raw != null && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n))
            {
                return n;
            }

            return fallback;
        }
    }
}
