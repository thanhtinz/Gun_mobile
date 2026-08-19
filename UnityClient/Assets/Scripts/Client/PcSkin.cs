using GunMobile.Core;
using GunMobile.Res;
using UnityEngine;
using UnityEngine.UI;

namespace GunMobile.Client
{
    /// <summary>
    /// PC Flash/Starling bitmaps only — no generated placeholder art.
    /// </summary>
    public static class PcSkin
    {
        public static SpriteSheet Hall { get; private set; }
        public static SpriteSheet Game { get; private set; }
        public static SpriteSheet Default { get; private set; }

        public static void Warm(ResLoader loader)
        {
            if (loader == null)
            {
                return;
            }

            if (Hall == null)
            {
                string star = GamePaths.Starling(GamePaths.DefaultLanguage, "hall_scene");
                Hall = SpriteSheet.TryLoadStarling(
                    loader,
                    GamePaths.PathCombine(star, "hall_scene.png"),
                    GamePaths.PathCombine(star, "hall_scene.xml"));
            }

            if (Game == null)
            {
                string game = GamePaths.Starling(GamePaths.DefaultLanguage, "game");
                Game = SpriteSheet.TryLoadStarling(
                    loader,
                    GamePaths.PathCombine(game, "game.png"),
                    GamePaths.PathCombine(game, "game.xml"));
            }

            if (Default == null)
            {
                string def = GamePaths.Starling(GamePaths.DefaultLanguage, "default");
                Default = SpriteSheet.TryLoadStarling(
                    loader,
                    GamePaths.PathCombine(def, "default_resource.png"),
                    GamePaths.PathCombine(def, "default_resource.xml"));
            }
        }

        public static RawImage Backdrop(Transform parent, ResLoader loader, params string[] files)
        {
            Texture2D tex = PcArt.File(loader, files);
            if (tex == null && Hall != null)
            {
                return Slice(parent, "Backdrop", Hall, "hall_scene_bg_0", true);
            }

            if (tex == null)
            {
                return null;
            }

            var go = new GameObject("Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            go.transform.SetAsFirstSibling();
            UiKit.Stretch(go);
            var raw = go.GetComponent<RawImage>();
            raw.texture = tex;
            raw.color = Color.white;
            raw.raycastTarget = false;
            return raw;
        }

        public static RawImage Slice(Transform parent, string goName, SpriteSheet sheet, string frame, bool stretch)
        {
            if (sheet == null || !sheet.TryGet(frame, out SheetFrame fr) || sheet.Texture == null)
            {
                return null;
            }

            var go = new GameObject(goName, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            var raw = go.GetComponent<RawImage>();
            raw.texture = sheet.Texture;
            raw.uvRect = fr.Uv;
            raw.color = Color.white;
            raw.raycastTarget = false;
            var rt = go.GetComponent<RectTransform>();
            if (stretch)
            {
                go.transform.SetAsFirstSibling();
                UiKit.Stretch(go);
            }
            else
            {
                rt.sizeDelta = new Vector2(Mathf.Max(32f, fr.Size.x), Mathf.Max(32f, fr.Size.y));
            }

            return raw;
        }

        public static void Apply(RawImage raw, SpriteSheet sheet, string frame)
        {
            if (raw == null || sheet == null || !sheet.TryGet(frame, out SheetFrame fr))
            {
                return;
            }

            raw.texture = sheet.Texture;
            raw.uvRect = fr.Uv;
            raw.color = Color.white;
        }

        public static void Chrome(Image img)
        {
            Chrome(img, Game, "game_blood_RBg");
            if (img != null && img.sprite == null)
            {
                Chrome(img, Default, "image_badge_1");
            }

            if (img != null && img.sprite == null)
            {
                Chrome(img, Hall, "hall_scene_build_title_roomList");
            }
        }

        public static void Chrome(Image img, SpriteSheet sheet, string frame)
        {
            if (img == null || sheet == null || !sheet.TryGet(frame, out SheetFrame fr) || sheet.Texture == null)
            {
                return;
            }

            float y = sheet.Texture.height - fr.Pixel.y - fr.Pixel.height;
            var sprite = Sprite.Create(
                sheet.Texture,
                new Rect(fr.Pixel.x, y, fr.Pixel.width, fr.Pixel.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect);
            img.sprite = sprite;
            img.color = Color.white;
            img.type = Image.Type.Sliced;
        }

        public static Texture2D MapThumb(ResLoader loader, int mapId)
        {
            string id = mapId.ToString();
            return PcArt.File(
                loader,
                GamePaths.PathCombine("Resource", "image", "map", id, "samll_map.png"),
                GamePaths.PathCombine("Resource", "image", "map", id, "samll_map_s.jpg"),
                GamePaths.PathCombine("Resource", "image", "map", id, "fore.png"));
        }
    }
}
