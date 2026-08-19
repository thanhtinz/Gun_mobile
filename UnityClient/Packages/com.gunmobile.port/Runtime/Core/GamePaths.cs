using System;
using System.IO;

namespace GunMobile.Core
{
    /// <summary>
    /// Path layout copied from the PC Flash client + IIS resource site.
    /// </summary>
    public static class GamePaths
    {
        public const string DefaultLanguage = "cn_trad";
        public const int DesignWidth = 1000;
        public const int DesignHeight = 600;

        public static string FlashRoot => "Flash";
        public static string ResourceRoot => "Resource";
        public static string RequestRoot => "Request";
        public static string ImageRoot => PathCombine(ResourceRoot, "image");

        public static string ConfigXml => PathCombine(FlashRoot, "config.xml");
        public static string CharacterDefine => PathCombine(FlashRoot, "characterdefine.xml");

        public static string UiRoot(string language = DefaultLanguage)
        {
            return PathCombine(FlashRoot, "ui", language);
        }

        public static string UiXml(string language = DefaultLanguage)
        {
            return PathCombine(UiRoot(language), "xml", "xml");
        }

        public static string MornUi(string language = DefaultLanguage)
        {
            return PathCombine(UiRoot(language), "morn", "ui");
        }

        public static string Starling(string language, string scene)
        {
            return PathCombine(UiRoot(language), "starling", scene);
        }

        public static string MapImage(int mapId)
        {
            return PathCombine(ImageRoot, "map", mapId.ToString());
        }

        public static string MapCollision(int mapId)
        {
            return PathCombine("Service", "Road", "map", mapId.ToString(), "fore.map");
        }

        public static string BombSprite(int bombId)
        {
            return PathCombine(ImageRoot, "bomb", bombId.ToString());
        }

        public static string BombCrater(int craterId)
        {
            return PathCombine(ImageRoot, "bomb", "crater", craterId.ToString());
        }

        public static string Equip(string slot, string itemKey)
        {
            return PathCombine(ImageRoot, "equip", slot, itemKey);
        }

        public static string Arm(string itemKey)
        {
            return PathCombine(ImageRoot, "arm", itemKey);
        }

        public static string RequestXml(string fileName)
        {
            if (!fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xml";
            }

            return PathCombine(RequestRoot, fileName);
        }

        public static string PathCombine(params string[] parts)
        {
            return string.Join("/", parts).Replace('\\', '/');
        }

        public static string Normalize(string relative)
        {
            return (relative ?? string.Empty).Replace('\\', '/').TrimStart('/');
        }
    }
}
