using System;

namespace UnityEngine
{
    public static class Mathf
    {
        public const float PI = (float)Math.PI;

        public static int Abs(int v) => v < 0 ? -v : v;
        public static float Abs(float v) => v < 0f ? -v : v;

        public static int Max(int a, int b) => a > b ? a : b;
        public static float Max(float a, float b) => a > b ? a : b;

        public static int Min(int a, int b) => a < b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            return value > max ? max : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            return value > max ? max : value;
        }

        public static float Clamp01(float value) => Clamp(value, 0f, 1f);

        public static int RoundToInt(float v) => (int)Math.Round(v, MidpointRounding.AwayFromZero);
        public static int CeilToInt(float v) => (int)Math.Ceiling(v);
        public static int FloorToInt(float v) => (int)Math.Floor(v);

        public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);

        public const float Deg2Rad = (float)(Math.PI / 180.0);
        public const float Rad2Deg = (float)(180.0 / Math.PI);

        public static float Sin(float f) => (float)Math.Sin(f);
        public static float Cos(float f) => (float)Math.Cos(f);
        public static float Sqrt(float f) => (float)Math.Sqrt(f);
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
    }
}
