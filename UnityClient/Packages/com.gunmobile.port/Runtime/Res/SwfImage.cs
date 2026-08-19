using System;
using System.IO;
using GunMobile.Core;
using UnityEngine;

namespace GunMobile.Res
{
    /// <summary>
    /// Pull the largest DefineBitsJPEG3 still from a CWS/FWS SWF so Unity can
    /// show PC living/bomb art without a Flash runtime. Alpha zlib is skipped;
    /// pack-time <c>tools/swf_extract.py</c> writes PNG when lossless tags win.
    /// </summary>
    public static class SwfImage
    {
        public static Texture2D LoadLargest(byte[] swf)
        {
            byte[] jpeg = LargestJpeg(swf);
            if (jpeg == null)
            {
                return null;
            }

            return SpriteSheet.LoadTexture(jpeg, false);
        }

        public static Texture2D TryLoad(ResLoader loader, params string[] paths)
        {
            if (loader == null)
            {
                return null;
            }

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path) || !loader.TryReadBytes(path, out byte[] bytes))
                {
                    continue;
                }

                if (bytes.Length >= 3 && bytes[1] == 0x57 && bytes[2] == 0x53 &&
                    (bytes[0] == 0x43 || bytes[0] == 0x46))
                {
                    Texture2D fromSwf = LoadLargest(bytes);
                    if (fromSwf != null)
                    {
                        return fromSwf;
                    }
                }

                Texture2D tex = SpriteSheet.LoadTexture(SpriteSheet.StripToPng(bytes) ?? bytes, false);
                if (tex != null)
                {
                    return tex;
                }
            }

            return null;
        }

        public static byte[] LargestJpeg(byte[] swf)
        {
            if (swf == null || swf.Length < 10)
            {
                return null;
            }

            byte[] body;
            try
            {
                body = SwfBody(swf);
            }
            catch
            {
                return null;
            }

            byte[] best = null;
            foreach (var tag in Tags(body))
            {
                if (tag.code != 35 && tag.code != 21)
                {
                    continue;
                }

                byte[] jpg = JpegFromTag(tag.code, tag.payload);
                if (jpg != null && (best == null || jpg.Length > best.Length))
                {
                    best = jpg;
                }
            }

            return best;
        }

        static byte[] SwfBody(byte[] swf)
        {
            if (swf[0] == (byte)'F' && swf[1] == (byte)'W' && swf[2] == (byte)'S')
            {
                var body = new byte[swf.Length - 8];
                Buffer.BlockCopy(swf, 8, body, 0, body.Length);
                return body;
            }

            if (swf[0] != (byte)'C' || swf[1] != (byte)'W' || swf[2] != (byte)'S')
            {
                throw new InvalidDataException("not swf");
            }

            // CWS = zlib(body). Skip 2-byte zlib header, Deflate the rest.
            using (var ms = new MemoryStream(swf, 10, swf.Length - 10))
            using (var ds = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress))
            using (var outMs = new MemoryStream())
            {
                ds.CopyTo(outMs);
                return outMs.ToArray();
            }
        }

        struct SwfTag
        {
            public int code;
            public byte[] payload;
        }

        static System.Collections.Generic.IEnumerable<SwfTag> Tags(byte[] body)
        {
            int nbits = body[0] >> 3;
            int i = (5 + nbits * 4 + 7) / 8 + 4;
            int n = body.Length;
            while (i + 2 <= n)
            {
                int rec = body[i] | (body[i + 1] << 8);
                i += 2;
                int code = rec >> 6;
                int ln = rec & 0x3F;
                if (ln == 0x3F)
                {
                    if (i + 4 > n)
                    {
                        yield break;
                    }

                    ln = body[i] | (body[i + 1] << 8) | (body[i + 2] << 16) | (body[i + 3] << 24);
                    i += 4;
                }

                if (ln < 0 || i + ln > n)
                {
                    yield break;
                }

                var payload = new byte[ln];
                Buffer.BlockCopy(body, i, payload, 0, ln);
                i += ln;
                yield return new SwfTag { code = code, payload = payload };
                if (code == 0)
                {
                    yield break;
                }
            }
        }

        static byte[] JpegFromTag(int code, byte[] payload)
        {
            int startSearch = code == 35 ? 6 : 2;
            if (payload.Length <= startSearch + 2)
            {
                return null;
            }

            if (code == 35 && payload.Length >= 6)
            {
                int off = payload[2] | (payload[3] << 8) | (payload[4] << 16) | (payload[5] << 24);
                if (off > 0 && 6 + off <= payload.Length)
                {
                    int soi = IndexOfSoi(payload, 6, 6 + off);
                    if (soi >= 0)
                    {
                        var jpg = new byte[6 + off - soi];
                        Buffer.BlockCopy(payload, soi, jpg, 0, jpg.Length);
                        return jpg;
                    }
                }
            }

            int p = IndexOfSoi(payload, startSearch, payload.Length);
            if (p < 0)
            {
                return null;
            }

            var all = new byte[payload.Length - p];
            Buffer.BlockCopy(payload, p, all, 0, all.Length);
            return all;
        }

        static int IndexOfSoi(byte[] data, int from, int to)
        {
            int end = Math.Min(to, data.Length) - 1;
            for (int i = from; i < end; i++)
            {
                if (data[i] == 0xFF && data[i + 1] == 0xD8)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
