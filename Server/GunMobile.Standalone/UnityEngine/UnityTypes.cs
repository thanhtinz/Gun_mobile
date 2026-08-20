namespace UnityEngine
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    public class Texture2D
    {
        public int width;
        public int height;
        public byte[] rawBytes;
    }

    public class Sprite
    {
        public Texture2D texture;
        public Rect rect;
        public Vector2 pivot;
    }

    public class TooltipAttribute : System.Attribute
    {
        public TooltipAttribute(string tooltip) { }
    }
}
