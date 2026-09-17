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

        /// <summary>
        /// Repeated child elements of each row, keyed by element name.
        /// </summary>
        /// <remarks>
        /// A row is a flat attribute map, which cannot hold a child element that
        /// appears more than once — and 1971 such children sit in 13 of the shipped
        /// <c>Request/*.xml</c> files. <c>QuestList.xml</c> alone carries 1067
        /// <c>Item_Good</c> (the quest's item rewards) and 317 <c>Item_Condiction</c>.
        /// The flat map kept the first of each and stored its <em>text</em>, which is
        /// empty because the data is in the attributes, so a caller reading
        /// <c>row["Item_Good"]</c> got <c>""</c> and no way to reach the rest.
        /// <para>
        /// That is why <c>GameDatabase</c> walks <c>XElement</c> by hand in 13 places
        /// instead of using this table. The children are kept here so it does not
        /// have to; the flat map is unchanged, so nothing that reads it needs to move.
        /// </para>
        /// </remarks>
        private List<Dictionary<string, List<IReadOnlyDictionary<string, string>>>> _children =
            new List<Dictionary<string, List<IReadOnlyDictionary<string, string>>>>();

        private static readonly IReadOnlyList<IReadOnlyDictionary<string, string>> NoChildren =
            Array.Empty<IReadOnlyDictionary<string, string>>();

        /// <summary>
        /// The <paramref name="name"/> children of <paramref name="row"/>, in document
        /// order; empty when the row has none.
        /// </summary>
        public IReadOnlyList<IReadOnlyDictionary<string, string>> ChildRows(int row, string name)
        {
            if (row < 0 || row >= _children.Count || string.IsNullOrEmpty(name))
            {
                return NoChildren;
            }

            return _children[row].TryGetValue(name, out List<IReadOnlyDictionary<string, string>> found)
                ? found
                : NoChildren;
        }

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
                        table._children.Add(ChildrenOf(nested));
                    }

                    continue;
                }

                rowName = child.Name.LocalName;
                Dictionary<string, string> map = RowFromElement(child);
                if (map.Count > 0)
                {
                    rows.Add(map);
                    table._children.Add(ChildrenOf(child));
                }
            }

            table.RowName = rowName ?? "Item";
            table.Rows = rows;
            return table;
        }

        static Dictionary<string, List<IReadOnlyDictionary<string, string>>> ChildrenOf(XElement el)
        {
            var byName = new Dictionary<string, List<IReadOnlyDictionary<string, string>>>(
                StringComparer.OrdinalIgnoreCase);
            foreach (XElement child in el.Elements())
            {
                if (!child.HasAttributes)
                {
                    // A text-only child is already in the flat map under its own name.
                    continue;
                }

                string name = child.Name.LocalName;
                if (!byName.TryGetValue(name, out List<IReadOnlyDictionary<string, string>> list))
                {
                    list = new List<IReadOnlyDictionary<string, string>>();
                    byName[name] = list;
                }

                list.Add(RowFromElement(child));
            }

            return byName;
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
