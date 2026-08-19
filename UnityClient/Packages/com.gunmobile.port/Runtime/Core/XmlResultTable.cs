using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace GunMobile.Core
{
    /// <summary>
    /// Most Request/*.xml files are &lt;Result&gt;&lt;Item ...attrs /&gt;&lt;/Result&gt; tables.
    /// </summary>
    public sealed class XmlResultTable
    {
        public bool Ok { get; private set; }
        public string Message { get; private set; }
        public string RowName { get; private set; }
        public IReadOnlyList<IReadOnlyDictionary<string, string>> Rows { get; private set; }

        public static XmlResultTable Parse(XDocument doc)
        {
            var table = new XmlResultTable();
            XElement root = doc.Root;
            if (root == null)
            {
                table.Ok = false;
                table.Message = "empty xml";
                table.Rows = Array.Empty<IReadOnlyDictionary<string, string>>();
                return table;
            }

            table.Ok = Attr(root, "value", "true").Equals("true", StringComparison.OrdinalIgnoreCase);
            table.Message = Attr(root, "message", string.Empty);

            var rows = new List<IReadOnlyDictionary<string, string>>();
            string rowName = null;
            foreach (XElement child in root.Elements())
            {
                if (child.HasElements && !child.HasAttributes)
                {
                    foreach (XElement nested in child.Elements())
                    {
                        Dictionary<string, string> nestedMap = RowFromElement(nested);
                        if (nestedMap.Count == 0)
                        {
                            continue;
                        }

                        rowName = nested.Name.LocalName;
                        rows.Add(nestedMap);
                    }

                    continue;
                }

                rowName = child.Name.LocalName;
                Dictionary<string, string> map = RowFromElement(child);
                if (map.Count > 0)
                {
                    rows.Add(map);
                }
            }

            table.RowName = rowName ?? "Item";
            table.Rows = rows;
            return table;
        }

        static Dictionary<string, string> RowFromElement(XElement el)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (XAttribute attr in el.Attributes())
            {
                map[attr.Name.LocalName] = attr.Value;
            }

            foreach (XElement nested in el.Elements())
            {
                if (!map.ContainsKey(nested.Name.LocalName))
                {
                    map[nested.Name.LocalName] = nested.Value ?? string.Empty;
                }
            }

            return map;
        }

        public static XmlResultTable LoadBytes(byte[] data)
        {
            return Parse(ZlibXml.Load(data));
        }

        public bool TryGetInt(int row, string key, out int value)
        {
            value = 0;
            if (row < 0 || row >= Rows.Count)
            {
                return false;
            }

            if (!Rows[row].TryGetValue(key, out string raw) || string.IsNullOrEmpty(raw))
            {
                return false;
            }

            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private static string Attr(XElement el, string name, string fallback)
        {
            XAttribute attr = el.Attribute(name);
            return attr != null ? attr.Value : fallback;
        }
    }
}
