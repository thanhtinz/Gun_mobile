using System;
using System.Collections.Generic;
using UnityEngine;

namespace GunMobile.UI
{
    /// <summary>
    /// Tiny pool for HUD icons, damage popups, and bomb sprites. PC Flash allocated per shot; phones cannot.
    /// </summary>
    public sealed class UiObjectPool
    {
        readonly GameObject _prefab;
        readonly Transform _parent;
        readonly Stack<GameObject> _free = new Stack<GameObject>();
        readonly HashSet<GameObject> _live = new HashSet<GameObject>();
        readonly int _maxLive;

        public UiObjectPool(GameObject prefab, Transform parent, int preload = 8, int maxLive = 64)
        {
            _prefab = prefab;
            _parent = parent;
            _maxLive = maxLive;
            for (int i = 0; i < preload; i++)
            {
                _free.Push(Create());
            }
        }

        public GameObject Rent()
        {
            if (_live.Count >= _maxLive)
            {
                return null;
            }

            GameObject go = _free.Count > 0 ? _free.Pop() : Create();
            go.SetActive(true);
            _live.Add(go);
            return go;
        }

        public void Return(GameObject go)
        {
            if (go == null || !_live.Remove(go))
            {
                return;
            }

            go.SetActive(false);
            go.transform.SetParent(_parent, false);
            _free.Push(go);
        }

        public void ReturnAll()
        {
            var copy = new List<GameObject>(_live);
            foreach (GameObject go in copy)
            {
                Return(go);
            }
        }

        GameObject Create()
        {
            GameObject go = UnityEngine.Object.Instantiate(_prefab, _parent);
            go.SetActive(false);
            return go;
        }
    }

    /// <summary>
    /// Builds a lightweight uGUI tree from a Morn &lt;View&gt; so PC XML can be previewed on device.
    /// Skins (asset.*) still need a sprite atlas lookup; missing skins become placeholders.
    /// </summary>
    public static class MornUiBuilder
    {
        public static RectTransform Build(Transform parent, Core.MornView view, Func<string, Sprite> skinLookup = null)
        {
            var root = Create("View", parent, view.Width, view.Height);
            foreach (System.Xml.Linq.XElement child in view.Root.Elements())
            {
                BuildNode(root, child, skinLookup);
            }

            return root;
        }

        static void BuildNode(Transform parent, System.Xml.Linq.XElement el, Func<string, Sprite> skinLookup)
        {
            string type = el.Name.LocalName;
            float x = F(el, "x");
            float y = F(el, "y");
            float w = F(el, "width", 100f);
            float h = F(el, "height", 40f);
            var node = Create(type, parent, w, h);
            var rt = node.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(x, -y);

            string skin = el.Attribute("skin")?.Value ?? el.Attribute("checkButtonSkin")?.Value;
            if (!string.IsNullOrEmpty(skin) && skinLookup != null)
            {
                Sprite sprite = skinLookup(skin);
                if (sprite != null)
                {
                    var img = node.gameObject.AddComponent<UnityEngine.UI.Image>();
                    img.sprite = sprite;
                    img.raycastTarget = type == "Button" || type == "CheckBox";
                }
            }

            string label = el.Attribute("label")?.Value ?? el.Attribute("text")?.Value;
            if (!string.IsNullOrEmpty(label))
            {
                var textGo = Create("Label", node, w, h);
                var text = textGo.gameObject.AddComponent<UnityEngine.UI.Text>();
                text.text = label;
                text.alignment = TextAnchor.MiddleCenter;
                text.font = UiFonts.Default;
                text.fontSize = Mathf.Max(MinReadableFontSize, (int)FontSizeOf(el));
                text.raycastTarget = false;
            }

            foreach (System.Xml.Linq.XElement child in el.Elements())
            {
                BuildNode(node, child, skinLookup);
            }
        }

        /// <summary>Floor applied to a phone screen, whatever the .ui asks for.</summary>
        const int MinReadableFontSize = 12;

        /// <summary>
        /// The font size a Morn widget asks for.
        /// </summary>
        /// <remarks>
        /// The two families name it differently, and this used to read only the
        /// button one. Measured across the 95 shipped bundles (950 views), of the
        /// 3322 widgets that render text, 2695 carry <c>size</c> and no
        /// <c>labelSize</c> — every Label, LabelEx, TextInput, TextArea and
        /// NameTextEx — so 81% of the text in the game fell back to the hard-coded
        /// 14. 1617 of those ask for something other than 14 and rendered at the
        /// wrong size; the rest happened to want 14 anyway.
        /// <para>
        /// <c>labelSize</c> is the Button family's attribute: 206 Button, 50
        /// CheckBox, 41 TabButtonEx. Not one widget in the dump carries both, so
        /// the two are disjoint and reading each where it appears cannot conflict.
        /// </para>
        /// 147 widgets ask for 10 or 11px and are still floored at
        /// <see cref="MinReadableFontSize"/> — that clamp is a deliberate phone
        /// legibility choice, not part of this bug, so it is left in place.
        /// </remarks>
        static float FontSizeOf(System.Xml.Linq.XElement el)
        {
            if (el.Attribute("size") != null)
            {
                return F(el, "size", 14f);
            }

            return F(el, "labelSize", 14f);
        }

        static RectTransform Create(string name, Transform parent, float w, float h)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);
            return rt;
        }

        static float F(System.Xml.Linq.XElement el, string name, float fallback = 0f)
        {
            var attr = el.Attribute(name);
            if (attr == null)
            {
                return fallback;
            }

            return float.TryParse(attr.Value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float v)
                ? v
                : fallback;
        }
    }
}
