using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace GunMobile.Core
{
    /// <summary>
    /// DDTank stores many XML/UI blobs as zlib (RFC 1950). Detects raw XML vs zlib and returns UTF-8 text.
    /// </summary>
    public static class ZlibXml
    {
        public static bool IsZlib(byte[] data)
        {
            if (data == null || data.Length < 2)
            {
                return false;
            }

            return data[0] == 0x78 && (data[1] == 0x01 || data[1] == 0x9C || data[1] == 0xDA);
        }

        public static bool LooksLikeXml(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return false;
            }

            int i = 0;
            while (i < data.Length && (data[i] == 0xEF || data[i] == 0xBB || data[i] == 0xBF ||
                                       data[i] == (byte)' ' || data[i] == (byte)'\t' ||
                                       data[i] == (byte)'\r' || data[i] == (byte)'\n'))
            {
                i++;
            }

            return i < data.Length && data[i] == (byte)'<';
        }

        public static byte[] Inflate(byte[] zlibBytes)
        {
            if (zlibBytes == null || zlibBytes.Length < 2)
            {
                throw new ArgumentException("zlib payload is empty");
            }

            // Skip CMF/FLG header. DeflateStream reads the raw DEFLATE body and stops
            // before the Adler-32 trailer.
            using (var input = new MemoryStream(zlibBytes, 2, zlibBytes.Length - 2, false))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflate.CopyTo(output);
                return output.ToArray();
            }
        }

        public static byte[] DecodeBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (IsZlib(data))
            {
                return Inflate(data);
            }

            return data;
        }

        public static string DecodeText(byte[] data)
        {
            byte[] decoded = DecodeBytes(data);
            if (decoded.Length >= 3 && decoded[0] == 0xEF && decoded[1] == 0xBB && decoded[2] == 0xBF)
            {
                return Encoding.UTF8.GetString(decoded, 3, decoded.Length - 3);
            }

            return Encoding.UTF8.GetString(decoded);
        }

        public static XDocument Load(byte[] data)
        {
            string text = DecodeText(data).TrimStart('\uFEFF', '\0');
            int xmlAt = text.IndexOf('<');
            if (xmlAt < 0)
            {
                throw new InvalidDataException("No XML root in payload");
            }

            if (xmlAt > 0)
            {
                text = text.Substring(xmlAt);
            }

            return XDocument.Parse(text, LoadOptions.SetLineInfo);
        }

        public static XDocument LoadFile(string path)
        {
            return Load(File.ReadAllBytes(path));
        }
    }
}
