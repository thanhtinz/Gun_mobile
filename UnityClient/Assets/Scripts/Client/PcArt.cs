using GunMobile.Core;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Client
{
    public static class PcArt
    {
        public static Texture2D NpcLiving(ResLoader loader, NpcInfo npc)
        {
            if (loader == null)
            {
                return null;
            }

            string stem = Stem(npc);
            string[] paths =
            {
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", stem + ".jpg"),
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", stem + ".png"),
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", stem.ToLowerInvariant() + ".jpg"),
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", stem.ToLowerInvariant() + ".png"),
                GamePaths.PathCombine("Resource", "image", "game", "living", stem + ".swf"),
                GamePaths.PathCombine("Resource", "image", "game", "living", stem.ToLowerInvariant() + ".swf")
            };
            return SwfImage.TryLoad(loader, paths);
        }

        public static Texture2D Bullet(ResLoader loader, int ballId)
        {
            if (loader == null || ballId <= 0)
            {
                return null;
            }

            string id = ballId.ToString();
            return SwfImage.TryLoad(
                loader,
                GamePaths.PathCombine("Resource", "image", "bomb", "bullet", "extracted", "bullet" + id + ".png"),
                GamePaths.PathCombine("Resource", "image", "bomb", "bullet", "extracted", "bullet" + id + ".jpg"),
                GamePaths.PathCombine("Resource", "image", "bomb", "bullet", "bullet" + id + ".swf"),
                GamePaths.PathCombine("Resource", "image", "bomb", "blastout", "extracted", "blastout" + id + ".png"),
                GamePaths.PathCombine("Resource", "image", "bomb", "blastout", "extracted", "blastout" + id + ".jpg"));
        }

        public static Texture2D Blast(ResLoader loader, int ballId)
        {
            if (loader == null)
            {
                return null;
            }

            string id = ballId > 0 ? ballId.ToString() : "1";
            return SwfImage.TryLoad(
                loader,
                GamePaths.PathCombine("Resource", "image", "bomb", "blastout", "extracted", "blastout" + id + ".png"),
                GamePaths.PathCombine("Resource", "image", "bomb", "blastout", "extracted", "blastout" + id + ".jpg"),
                GamePaths.PathCombine("Resource", "image", "bomb", "blastout", "extracted", "blastout1.png"),
                GamePaths.PathCombine("Resource", "image", "bomb", "blastout", "extracted", "blastout1.jpg"));
        }

        public static Texture2D DefaultLiving(ResLoader loader)
        {
            return SwfImage.TryLoad(
                loader,
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", "living002.jpg"),
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", "living094.jpg"),
                GamePaths.PathCombine("Resource", "image", "game", "living", "extracted", "living003.jpg"));
        }

        public static Texture2D File(ResLoader loader, params string[] paths)
        {
            return SwfImage.TryLoad(loader, paths);
        }

        static string Stem(NpcInfo npc)
        {
            string path = npc != null ? npc.ResourcesPath : "";
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace('\\', '/');
                int slash = path.LastIndexOf('/');
                string file = slash >= 0 ? path.Substring(slash + 1) : path;
                if (file.EndsWith(".swf", System.StringComparison.OrdinalIgnoreCase))
                {
                    file = file.Substring(0, file.Length - 4);
                }

                return file;
            }

            string model = npc != null ? npc.ModelId : "";
            int dot = model.LastIndexOf('.');
            return dot >= 0 ? model.Substring(dot + 1) : "living948";
        }
    }
}
