using UnityEngine;
using UnityEngine.UI;

namespace GunMobile.UI
{
    /// <summary>
    /// PC Flash canvas is ~1000x600. On phones we keep landscape, scale with screen, and pad the notch.
    /// </summary>
    public static class MobileUiBootstrap
    {
        public const float ReferenceWidth = 1560f;
        public const float ReferenceHeight = 720f;

        public static Canvas CreateRoot(Transform parent = null, string name = "GunMobileCanvas")
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = 10;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = IsPortrait() ? 0.0f : 0.5f;
            scaler.referencePixelsPerUnit = 100f;

            var safe = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(root.transform, false);
            Stretch(safe.GetComponent<RectTransform>());
            return canvas;
        }

        public static RectTransform CreateHudLayer(Transform safeArea, string name, TextAnchor anchor, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(safeArea, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            ApplyAnchor(rt, anchor);
            return rt;
        }

        public static bool IsPortrait()
        {
            return Screen.height > Screen.width;
        }

        public static float TouchScale
        {
            get
            {
                float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;
                return Mathf.Clamp(dpi / 160f, 0.85f, 2.2f);
            }
        }

        public static Vector2 FingerButtonSize
        {
            get
            {
                float s = 88f * TouchScale;
                return new Vector2(s, s);
            }
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        static void ApplyAnchor(RectTransform rt, TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.LowerLeft:
                    rt.anchorMin = rt.anchorMax = new Vector2(0f, 0f);
                    rt.pivot = new Vector2(0f, 0f);
                    rt.anchoredPosition = new Vector2(24f, 24f);
                    break;
                case TextAnchor.LowerRight:
                    rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
                    rt.pivot = new Vector2(1f, 0f);
                    rt.anchoredPosition = new Vector2(-24f, 24f);
                    break;
                case TextAnchor.UpperCenter:
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
                    rt.pivot = new Vector2(0.5f, 1f);
                    rt.anchoredPosition = new Vector2(0f, -12f);
                    break;
                default:
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    break;
            }
        }
    }

    [DisallowMultipleComponent]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        Rect _last;

        void OnEnable() => Apply();

        void Update()
        {
            if (_last != Screen.safeArea)
            {
                Apply();
            }
        }

        public void Apply()
        {
            var rt = (RectTransform)transform;
            Rect safe = Screen.safeArea;
            _last = safe;
            var min = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            var max = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
