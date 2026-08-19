using System;
using System.IO;

namespace GunMobile.Res
{
    /// <summary>
    /// Service/Road/map/{id}/fore.map — 8-byte header (width, height) then 1-bit collision,
    /// rows padded to a full byte. MSB of each byte is the leftmost pixel.
    /// </summary>
    public sealed class MapCollision
    {
        public int Width { get; }
        public int Height { get; }
        public int Stride { get; }

        readonly byte[] _bits;

        public MapCollision(int width, int height, byte[] bits)
        {
            Width = width;
            Height = height;
            Stride = (width + 7) / 8;
            int expected = Stride * height;
            if (bits == null || bits.Length < expected)
            {
                throw new InvalidDataException($"fore.map payload {bits?.Length ?? 0} < {expected}");
            }

            _bits = bits;
        }

        public static MapCollision Load(byte[] data)
        {
            if (data == null || data.Length < 8)
            {
                throw new InvalidDataException("fore.map too small");
            }

            int width = BitConverter.ToInt32(data, 0);
            int height = BitConverter.ToInt32(data, 4);
            if (!BitConverter.IsLittleEndian)
            {
                width = Swap(width);
                height = Swap(height);
            }

            if (width <= 0 || height <= 0 || width > 8192 || height > 8192)
            {
                throw new InvalidDataException($"bad map size {width}x{height}");
            }

            var bits = new byte[data.Length - 8];
            Buffer.BlockCopy(data, 8, bits, 0, bits.Length);
            return new MapCollision(width, height, bits);
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        /// <summary>True when this pixel is solid terrain.</summary>
        public bool IsSolid(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return false;
            }

            int index = y * Stride + (x >> 3);
            int mask = 0x80 >> (x & 7);
            return (_bits[index] & mask) != 0;
        }

        public bool IsEmpty(int x, int y) => !IsSolid(x, y);

        public void SetSolid(int x, int y, bool solid)
        {
            if (!InBounds(x, y))
            {
                return;
            }

            int index = y * Stride + (x >> 3);
            int mask = 0x80 >> (x & 7);
            if (solid)
            {
                _bits[index] |= (byte)mask;
            }
            else
            {
                _bits[index] &= (byte)~mask;
            }
        }

        /// <summary>
        /// Dig a circular crater. Returns the number of pixels cleared.
        /// </summary>
        public int CutCircle(int cx, int cy, int radius)
        {
            int cut = 0;
            int r2 = radius * radius;
            int minX = Math.Max(0, cx - radius);
            int maxX = Math.Min(Width - 1, cx + radius);
            int minY = Math.Max(0, cy - radius);
            int maxY = Math.Min(Height - 1, cy + radius);
            for (int y = minY; y <= maxY; y++)
            {
                int dy = y - cy;
                for (int x = minX; x <= maxX; x++)
                {
                    int dx = x - cx;
                    if (dx * dx + dy * dy > r2)
                    {
                        continue;
                    }

                    if (IsSolid(x, y))
                    {
                        SetSolid(x, y, false);
                        cut++;
                    }
                }
            }

            return cut;
        }

        public int FindStandY(int x, int startY)
        {
            x = Math.Max(0, Math.Min(Width - 1, x));
            for (int y = Math.Max(0, startY); y < Height; y++)
            {
                if (IsSolid(x, y))
                {
                    return y;
                }
            }

            return Height - 1;
        }

        static int Swap(int value)
        {
            unchecked
            {
                return ((value & 0xFF) << 24)
                       | ((value & 0xFF00) << 8)
                       | ((value >> 8) & 0xFF00)
                       | ((value >> 24) & 0xFF);
            }
        }
    }
}
