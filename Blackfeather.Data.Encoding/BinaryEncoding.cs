using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackfeather.Data.Encoding
{
    public static class BinaryEncoding
    {
        public enum EncodingType
        {
            Hex = 1,
            Base32 = 2,
            Base64 = 3
        }

        public static string ToString(byte[] source, EncodingType type)
        {
            var content = string.Empty;
            switch (type)
            {
                case EncodingType.Hex:
                    content = source.ToHex();
                    break;
                case EncodingType.Base32:
                    content = source.ToBase32();
                    break;
                case EncodingType.Base64:
                    content = source.ToBase64();
                    break;
            }

            return content;
        }

        public static byte[] FromString(string source, EncodingType type)
        {
            byte[] content = null;
            switch (type)
            {
                case EncodingType.Hex:
                    content = source.FromHex();
                    break;
                case EncodingType.Base32:
                    content = source.FromBase32();
                    break;
                case EncodingType.Base64:
                    content = source.FromBase64();
                    break;
            }

            return content;
        }

        public static string ToHex(this byte[] source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            var hex = new StringBuilder(source.Length * 2);

            foreach (var currentByte in source)
            {
                hex.AppendFormat("{0:X2}", currentByte);
            }

            return hex.ToString();
        }

        public static byte[] FromHex(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            var bytes = new byte[source.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(source.Substring(i * 2, 2));
            }

            return bytes;
        }

        public static string ToBase32(this byte[] source)
        {
            return source == null ? string.Empty : Base32.ToBase32String(source);
        }

        public static byte[] FromBase32(this string source)
        {
            return string.IsNullOrEmpty(source) ? null : Base32.FromBase32String(source);
        }

        public static string ToBase64(this byte[] source)
        {
            return source == null ? string.Empty : Convert.ToBase64String(source);
        }

        public static byte[] FromBase64(this string source)
        {
            return string.IsNullOrEmpty(source) ? null : Convert.FromBase64String(source);
        }
    }
}
