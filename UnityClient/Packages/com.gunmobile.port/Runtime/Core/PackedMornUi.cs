using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GunMobile.Core
{
    public sealed class MornView
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public XElement Root { get; set; }
    }

    /// <summary>
    /// Morn .ui files are zlib + a small AMF3-like bundle of named View XML documents.
    /// </summary>
    public static class PackedMornUi
    {
        public static List<MornView> Parse(byte[] data)
        {
            byte[] decoded = ZlibXml.DecodeBytes(data);
            string text = Encoding.UTF8.GetString(decoded);
            var views = new List<MornView>();

            int searchFrom = 0;
            while (true)
            {
                int viewStart = text.IndexOf("<View", searchFrom, StringComparison.Ordinal);
                if (viewStart < 0)
                {
                    break;
                }

                int viewEnd = text.IndexOf("</View>", viewStart, StringComparison.Ordinal);
                if (viewEnd < 0)
                {
                    break;
                }

                viewEnd += "</View>".Length;
                string xml = text.Substring(viewStart, viewEnd - viewStart);
                XElement root = XElement.Parse(xml);
                var view = new MornView
                {
                    Name = FindNameBefore(text, viewStart),
                    Width = IntAttr(root, "width"),
                    Height = IntAttr(root, "height"),
                    Root = root
                };
                views.Add(view);
                searchFrom = viewEnd;
            }

            return views;
        }

        private static string FindNameBefore(string text, int viewStart)
        {
            int xmlExt = text.LastIndexOf(".xml", viewStart, StringComparison.Ordinal);
            if (xmlExt < 0)
            {
                return "View";
            }

            int start = xmlExt;
            while (start > 0)
            {
                char c = text[start - 1];
                if (char.IsLetterOrDigit(c) || c == '_' || c == '/' || c == '-' || c == '.')
                {
                    start--;
                    continue;
                }

                break;
            }

            return text.Substring(start, xmlExt + 4 - start);
        }

        private static int IntAttr(XElement el, string name)
        {
            XAttribute attr = el.Attribute(name);
            if (attr == null)
            {
                return 0;
            }

            int.TryParse(attr.Value, out int value);
            return value;
        }
    }
}
