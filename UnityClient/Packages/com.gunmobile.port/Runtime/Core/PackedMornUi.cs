using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GunMobile.Core
{
    public sealed class MornView
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public XElement Root { get; set; }
    }

    /// <summary>
    /// Reader for Morn <c>.ui</c> layout bundles.
    /// </summary>
    /// <remarks>
    /// A bundle is a zlib stream wrapping one AMF3 dynamic object: the keys are view
    /// paths (<c>bank/BankSeeItem.xml</c>) and the values are XML markup describing
    /// the widget tree. The 95 shipped bundles hold 950 views between them.
    /// <para>
    /// This used to scan the decoded text for <c>&lt;View</c> … <c>&lt;/View&gt;</c>
    /// pairs instead of reading the container, which broke three ways on the real
    /// data. A self-closing <c>&lt;View …/&gt;</c> has no closing tag, so the scan ran
    /// past it and returned one chunk holding two roots with the AMF3 length bytes
    /// still between them — not well-formed, so <see cref="XElement.Parse"/> threw and
    /// took the whole screen with it (magicStone, magicStone_bk, dreamlandChallenge).
    /// Names were recovered by walking backwards from <c>.xml</c> over
    /// letters/digits/<c>_/-.</c>, which swallows the AMF3 length prefix whenever that
    /// byte happens to be one of those characters — 580 of the 950 view names came out
    /// with a stray leading character. And a view containing a nested
    /// <c>&lt;View&gt;</c> closed early.
    /// </para>
    /// Reading the container fixes all three at once, so the scan is gone.
    /// </remarks>
    public static class PackedMornUi
    {
        public static List<MornView> Parse(byte[] data)
        {
            var views = new List<MornView>();
            if (data == null || data.Length == 0)
            {
                return views;
            }

            byte[] decoded = ZlibXml.DecodeBytes(data);
            foreach (KeyValuePair<string, string> entry in new Amf3Bundle(decoded).ReadViews())
            {
                XElement root;
                try
                {
                    root = XElement.Parse(entry.Value);
                }
                catch (System.Xml.XmlException)
                {
                    // One unreadable view must not cost the caller the whole screen.
                    continue;
                }

                views.Add(new MornView
                {
                    Name = entry.Key,
                    Width = IntAttr(root, "width"),
                    Height = IntAttr(root, "height"),
                    Root = root
                });
            }

            return views;
        }

        private static int IntAttr(XElement el, string name)
        {
            XAttribute attr = el.Attribute(name);
            if (attr == null)
            {
                return 0;
            }

            int.TryParse(attr.Value, out int value);
            return value;
        }
    }

    /// <summary>
    /// The slice of AMF3 a <c>.ui</c> bundle uses: one dynamic object of string keys
    /// to XML values. Anything else in the format is out of scope and rejected rather
    /// than guessed at.
    /// </summary>
    internal sealed class Amf3Bundle
    {
        private const int Undefined = 0, Null = 1, False = 2, True = 3, Integer = 4;
        private const int Double = 5, StringMarker = 6, XmlDoc = 7, ArrayMarker = 9;
        private const int ObjectMarker = 10, XmlMarker = 11;

        private readonly byte[] _data;
        private readonly List<string> _strings = new List<string>();
        private int _position;

        public Amf3Bundle(byte[] data)
        {
            _data = data ?? Array.Empty<byte>();
        }

        /// <summary>Views in the order the bundle stores them; empty if it is not a bundle.</summary>
        public List<KeyValuePair<string, string>> ReadViews()
        {
            var views = new List<KeyValuePair<string, string>>();
            try
            {
                if (ReadByte() != ObjectMarker)
                {
                    return views;
                }

                int traits = ReadU29();
                if ((traits & 1) == 0 || (traits & 2) == 0 || (traits & 4) != 0)
                {
                    return views; // reference or externalizable: not what a bundle is
                }

                bool dynamic = (traits & 8) != 0;
                int sealedCount = traits >> 4;

                ReadString(); // class name, always empty here
                var names = new string[sealedCount];
                for (int i = 0; i < sealedCount; i++)
                {
                    names[i] = ReadString();
                }

                for (int i = 0; i < sealedCount; i++)
                {
                    Collect(views, names[i], ReadValue());
                }

                if (dynamic)
                {
                    while (true)
                    {
                        string key = ReadString();
                        if (key.Length == 0)
                        {
                            break;
                        }

                        Collect(views, key, ReadValue());
                    }
                }
            }
            catch (Exception e) when (e is IndexOutOfRangeException ||
                                      e is ArgumentOutOfRangeException ||
                                      e is FormatException)
            {
                // Truncated or unexpected: return whatever was read cleanly.
            }

            return views;
        }

        private static void Collect(List<KeyValuePair<string, string>> views, string key, object value)
        {
            if (value is string markup)
            {
                views.Add(new KeyValuePair<string, string>(key, markup));
            }
        }

        private object ReadValue()
        {
            int marker = ReadByte();
            switch (marker)
            {
                case Undefined:
                case Null:
                    return null;
                case False:
                    return false;
                case True:
                    return true;
                case Integer:
                {
                    int value = ReadU29();
                    // U29 integers are 29-bit two's complement.
                    return (value & 0x10000000) != 0 ? value - 0x20000000 : value;
                }
                case Double:
                    return ReadDouble();
                case StringMarker:
                    return ReadString();
                case XmlMarker:
                case XmlDoc:
                    return ReadXml();
                case ObjectMarker:
                    throw new FormatException("nested AMF3 objects are not part of a .ui bundle");
                case ArrayMarker:
                    throw new FormatException("AMF3 arrays are not part of a .ui bundle");
                default:
                    throw new FormatException($"unsupported AMF3 marker 0x{marker:x2}");
            }
        }

        private string ReadString()
        {
            int head = ReadU29();
            if ((head & 1) == 0)
            {
                return _strings[head >> 1];
            }

            int length = head >> 1;
            string text = Encoding.UTF8.GetString(_data, _position, length);
            _position += length;

            // The empty string is never entered into the table; adding it would shift
            // every later reference by one.
            if (text.Length > 0)
            {
                _strings.Add(text);
            }

            return text;
        }

        private string ReadXml()
        {
            int head = ReadU29();
            if ((head & 1) == 0)
            {
                throw new FormatException("xml references are not supported");
            }

            int length = head >> 1;
            string text = Encoding.UTF8.GetString(_data, _position, length);
            _position += length;
            return text;
        }

        private byte ReadByte()
        {
            if (_position >= _data.Length)
            {
                throw new IndexOutOfRangeException("AMF3 stream ended early");
            }

            return _data[_position++];
        }

        /// <summary>Variable-length integer: up to three continuation bytes, then a full byte.</summary>
        private int ReadU29()
        {
            int value = 0;
            for (int i = 0; i < 3; i++)
            {
                byte b = ReadByte();
                value = (value << 7) | (b & 0x7F);
                if ((b & 0x80) == 0)
                {
                    return value;
                }
            }

            return (value << 8) | ReadByte();
        }

        private double ReadDouble()
        {
            // AMF3 doubles are big endian; BitConverter follows the host.
            var scratch = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                scratch[7 - i] = _data[_position + i];
            }

            _position += 8;
            return BitConverter.ToDouble(scratch, 0);
        }
    }
}
