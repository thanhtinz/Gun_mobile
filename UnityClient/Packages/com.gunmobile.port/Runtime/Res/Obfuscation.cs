using System;

namespace GunMobile.Res
{
    /// <summary>
    /// Undoes the obfuscation the PC build applies to some shipped resources.
    /// </summary>
    /// <remarks>
    /// 1177 files under <c>Resource/image</c> are obfuscated, and the scheme is two
    /// steps, not one: a five byte prefix (<c>00 03 5E 5F 5E</c>) is put in front of
    /// the payload, and the byte at offset 16 of the payload is bitwise complemented.
    /// Undoing only the prefix leaves a file no decoder accepts.
    /// <para>
    /// Offset 16 is format-agnostic — the packer does not look at what it is wrapping
    /// — so the damage lands somewhere different in each format and looks like a
    /// different bug every time. In PNG (1104 files) it is the high byte of the IHDR
    /// width, so the IHDR CRC fails and <c>Texture2D.LoadImage</c> returns false. In
    /// CWS/SWF (63 files) it is six bytes into the deflate stream, so the body will
    /// not inflate at all. In JPEG (2 files) it is the high byte of the EXIF IFD
    /// offset. In ZIP (8 files) it is a byte of the local header CRC, which readers
    /// ignore in favour of the central directory, so those happened to survive.
    /// </para>
    /// Complementing that one byte back is exact rather than a heuristic: all 1104
    /// PNGs then pass their IHDR CRC and all 63 SWFs then inflate, while none of the
    /// 16929 unobfuscated PNGs in the dump is damaged to begin with. The prefix is
    /// therefore the whole signal, and anything without it is returned untouched.
    /// </remarks>
    public static class Obfuscation
    {
        /// <summary>Offset, within the payload, of the complemented byte.</summary>
        public const int TamperedByte = 16;

        static readonly byte[] Prefix = { 0x00, 0x03, 0x5E, 0x5F, 0x5E };

        public static bool IsObfuscated(byte[] data)
        {
            if (data == null || data.Length < Prefix.Length)
            {
                return false;
            }

            for (int i = 0; i < Prefix.Length; i++)
            {
                if (data[i] != Prefix[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the decodable payload, or <paramref name="data"/> itself when the
        /// file is not obfuscated. Never modifies the caller's array.
        /// </summary>
        public static byte[] Strip(byte[] data)
        {
            if (!IsObfuscated(data))
            {
                return data;
            }

            var body = new byte[data.Length - Prefix.Length];
            Buffer.BlockCopy(data, Prefix.Length, body, 0, body.Length);
            if (body.Length > TamperedByte)
            {
                body[TamperedByte] ^= 0xFF;
            }

            return body;
        }
    }
}
