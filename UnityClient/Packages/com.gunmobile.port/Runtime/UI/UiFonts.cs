using UnityEngine;

namespace GunMobile.UI
{
    /// <summary>
    /// Unity 6 removed the built-in Arial.ttf. Prefer LegacyRuntime, then OS fonts.
    /// </summary>
    public static class UiFonts
    {
        static Font _font;

        public static Font Default
        {
            get
            {
                if (_font != null)
                {
                    return _font;
                }

                _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                if (_font == null)
                {
                    _font = Font.CreateDynamicFontFromOSFont(
                        new[] { "Arial", "Helvetica", "Noto Sans CJK SC", "Droid Sans Fallback", "sans-serif" },
                        24);
                }

                return _font;
            }
        }
    }
}
