using GunMobile.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GunMobile.Client
{
    public static class UiKit
    {
        static Font _font;

        public static Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = UiFonts.Default;
                }

                return _font;
            }
        }

        public static RectTransform Stretch(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Image Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = true;
            Stretch(go);
            return img;
        }

        public static Text Label(Transform parent, string name, string value, int size, Color color, TextAnchor align = TextAnchor.MiddleLeft)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Font;
            text.fontSize = size;
            text.color = color;
            text.alignment = align;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            text.text = value;
            return text;
        }

        public static Button Button(Transform parent, string name, string caption, UnityAction onClick, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            var img = go.GetComponent<Image>();
            img.color = new Color(0.18f, 0.22f, 0.32f, 0.94f);
            var btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(onClick);
            var text = Label(go.transform, "Caption", caption, 28, new Color(1f, 0.95f, 0.75f), TextAnchor.MiddleCenter);
            Stretch(text.gameObject);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            text.resizeTextMaxSize = 28;
            return btn;
        }

        public static InputField Field(Transform parent, string name, string placeholder, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            go.GetComponent<RectTransform>().sizeDelta = size;
            go.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.12f, 0.9f);
            var text = Label(go.transform, "Text", "", 28, Color.white, TextAnchor.MiddleLeft);
            Stretch(text.gameObject);
            text.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(16f, 4f);
            text.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-16f, -4f);
            var ph = Label(go.transform, "Placeholder", placeholder, 26, new Color(1f, 1f, 1f, 0.4f), TextAnchor.MiddleLeft);
            Stretch(ph.gameObject);
            ph.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(16f, 4f);
            var field = go.GetComponent<InputField>();
            field.textComponent = text;
            field.placeholder = ph;
            field.characterLimit = 16;
            return field;
        }

        public static ScrollRect Scroll(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(ScrollRect), typeof(RectMask2D));
            go.transform.SetParent(parent, false);
            Stretch(go);
            go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.15f);
            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(go.transform, false);
            var crt = content.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0f, 1f);
            crt.anchorMax = new Vector2(1f, 1f);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.offsetMin = Vector2.zero;
            crt.offsetMax = Vector2.zero;
            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.spacing = 10f;
            layout.padding = new RectOffset(12, 12, 12, 12);
            content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var scroll = go.GetComponent<ScrollRect>();
            scroll.content = crt;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            return scroll;
        }

        public static void ClearChildren(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(t.GetChild(i).gameObject);
            }
        }
    }
}
