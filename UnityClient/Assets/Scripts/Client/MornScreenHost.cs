using System;
using GunMobile.Core;
using GunMobile.Res;
using GunMobile.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GunMobile.Client
{
    /// <summary>
    /// Loads PC Morn .ui layouts and builds a uGUI preview tree via MornUiBuilder.
    /// </summary>
    public static class MornScreenHost
    {
        public static bool TryEmbedMorn(Transform parent, GameApp app, string uiFile, string viewName = null)
        {
            if (app?.Loader == null || parent == null || string.IsNullOrEmpty(uiFile))
            {
                return false;
            }

            string path = GamePaths.PathCombine(GamePaths.MornUi(), uiFile);
            if (!app.Loader.TryReadBytes(path, out byte[] bytes) || bytes == null || bytes.Length == 0)
            {
                return false;
            }

            var views = PackedMornUi.Parse(bytes);
            if (views == null || views.Count == 0)
            {
                return false;
            }

            MornView view = views[0];
            if (!string.IsNullOrEmpty(viewName))
            {
                for (int i = 0; i < views.Count; i++)
                {
                    if (views[i].Name != null && views[i].Name.IndexOf(viewName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        view = views[i];
                        break;
                    }
                }
            }

            PcSkin.Warm(app.Loader);
            var host = new GameObject("MornHost", typeof(RectTransform));
            host.transform.SetParent(parent, false);
            var rt = host.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(0f, 0f);
            rt.offsetMax = new Vector2(0f, -80f);

            MornUiBuilder.Build(host.transform, view, skin => LookupSkin(app, skin));
            return true;
        }

        static Sprite LookupSkin(GameApp app, string skin)
        {
            if (string.IsNullOrEmpty(skin))
            {
                return null;
            }

            string key = skin;
            int dot = skin.LastIndexOf('.');
            if (dot >= 0)
            {
                key = skin.Substring(dot + 1);
            }

            if (PcSkin.Default != null)
            {
                Sprite s = PcSkin.Default.Get(key);
                if (s != null)
                {
                    return s;
                }
            }

            if (PcSkin.Hall != null)
            {
                return PcSkin.Hall.Get(key);
            }

            return null;
        }
    }
}
