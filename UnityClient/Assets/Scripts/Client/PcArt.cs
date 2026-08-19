using System.Collections.Generic;
using GunMobile.Core;
using GunMobile.Res;
using UnityEngine;
using UnityEngine.UI;

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

        public static Texture2D ItemIcon(ResLoader loader, ItemTemplate item, int sex)
        {
            if (loader == null || item == null)
            {
                return null;
            }

            string pic = string.IsNullOrEmpty(item.Pic) ? "" : item.Pic.Trim();
            if (string.IsNullOrEmpty(pic) || pic == "default")
            {
                return null;
            }

            string side = sex == 2 ? "f" : "m";
            string slot = EquipFolder(item.CategoryId);
            var paths = new List<string>();
            foreach (string p in PicKeys(pic))
            {
                if (!string.IsNullOrEmpty(slot) && IsBodySlot(slot))
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "equip", side, slot, p, "icon_1.png"));
                    paths.Add(GamePaths.PathCombine("Resource", "image", "equip", side == "m" ? "f" : "m", slot, p, "icon_1.png"));
                }
                else if (!string.IsNullOrEmpty(slot))
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "equip", slot, p, "icon_1.png"));
                    paths.Add(GamePaths.PathCombine("Resource", "image", "equip", slot, p, "icon.png"));
                }

                if (item.CategoryId == 7)
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "arm", p, "1", "icon.png"));
                    paths.Add(GamePaths.PathCombine("Resource", "image", "arm", p, "icon.png"));
                    paths.Add(GamePaths.PathCombine("Resource", "image", "arm", p, "00.png"));
                }

                if (item.CategoryId == 12)
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "task", p, "icon.png"));
                }

                if (item.CategoryId == 16)
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "specialprop", "chatball", p, "icon.png"));
                }

                if (item.CategoryId == 32)
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "farm", "crops", p, "seed.png"));
                }

                paths.Add(GamePaths.PathCombine("Resource", "image", "unfrightprop", p, "icon.png"));
                paths.Add(GamePaths.PathCombine("Resource", "image", "prop", p, "icon.png"));
                paths.Add(GamePaths.PathCombine("Resource", "image", "gift", p, "icon.png"));
                paths.Add(GamePaths.PathCombine("Resource", "image", "buff", p, "icon.png"));
                paths.Add(GamePaths.PathCombine("Resource", "image", "pet", p, "icon1.png"));
                paths.Add(GamePaths.PathCombine("Resource", "image", "elf", p, "icon.png"));
            }

            return SwfImage.TryLoad(loader, paths.ToArray());
        }

        public static Texture2D EquipLayer(ResLoader loader, ItemTemplate item, int sex)
        {
            if (loader == null || item == null || string.IsNullOrEmpty(item.Pic))
            {
                return null;
            }

            string side = sex == 2 ? "f" : "m";
            string slot = EquipFolder(item.CategoryId);
            var paths = new List<string>();
            foreach (string p in PicKeys(item.Pic))
            {
                if (!string.IsNullOrEmpty(slot) && IsBodySlot(slot))
                {
                    paths.Add(GamePaths.PathCombine("Resource", "image", "equip", side, slot, p, "1", "game.png"));
                }

                paths.Add(GamePaths.PathCombine("Resource", "image", "arm", p, "1", "1", "game.png"));
                paths.Add(GamePaths.PathCombine("Resource", "image", "arm", p, "00.png"));
            }

            return SwfImage.TryLoad(loader, paths.ToArray());
        }

        public static string EquipFolder(int categoryId)
        {
            switch (categoryId)
            {
                case 1: return "head";
                case 2: return "glass";
                case 3: return "hair";
                case 4: return "eff";
                case 5: return "cloth";
                case 6: return "face";
                case 8: return "armlet";
                case 9: return "ring";
                case 13: return "suits";
                case 14: return "necklace";
                case 15: return "wing";
                case 16:
                case 17: return "offhand";
                default: return "";
            }
        }

        public static void Decorate(Transform btn, Texture2D tex, float left = 0.12f)
        {
            if (tex == null || btn == null)
            {
                return;
            }

            var go = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(btn, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.01f, 0.08f);
            rt.anchorMax = new Vector2(left, 0.92f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var raw = go.GetComponent<RawImage>();
            raw.texture = tex;
            raw.raycastTarget = false;
            var cap = btn.Find("Caption") as RectTransform;
            if (cap != null)
            {
                cap.offsetMin = new Vector2(72f, 0f);
            }
        }

        static bool IsBodySlot(string slot)
        {
            return slot == "head" || slot == "glass" || slot == "hair" || slot == "eff" ||
                   slot == "cloth" || slot == "face" || slot == "suits";
        }

        static IEnumerable<string> PicKeys(string pic)
        {
            string raw = (pic ?? "").Replace('\\', '/').Trim();
            if (string.IsNullOrEmpty(raw))
            {
                yield break;
            }

            yield return raw;
            string low = raw.ToLowerInvariant();
            if (low != raw)
            {
                yield return low;
            }

            if ((raw.StartsWith("S") || raw.StartsWith("s")) && raw.Length > 1)
            {
                string rest = raw.Substring(1);
                yield return rest;
                string restLow = rest.ToLowerInvariant();
                if (restLow != rest)
                {
                    yield return restLow;
                }
            }
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
