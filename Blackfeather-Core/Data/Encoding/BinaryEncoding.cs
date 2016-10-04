/* 
 The MIT License (MIT)

 Copyright (c) 2013 - 2015 Timothy D Meadows II

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System;
using System.Linq;
using System.Text;

namespace Blackfeather.Data.Encoding
{
    public static class BinaryEncoding
    {
        /// <summary>
        /// Supported encoding types.
        /// </summary>
        public enum EncodingType
        {
            Hex = 1,
            Base32 = 2,
            Base64 = 3
        }

        /// <summary>
        /// Encode a string from bytes using a supported type.
        /// </summary>
        /// <param name="source">Bytes to be encoded.</param>
        /// <param name="type">Supported encoding type.</param>
        /// <returns>String data.</returns>
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

        /// <summary>
        /// Decode from a string using a supported type.
        /// </summary>
        /// <param name="source">Bytes to be encoded.</param>
        /// <param name="type">Supported encoding type.</param>
        /// <returns>Byte data.</returns>
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

        /// <summary>
        /// Decode string data from hex.
        /// </summary>
        /// <param name="source">Byte data to be encoded.</param>
        /// <returns>Byte data.</returns>
        public static byte[] FromHex(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            return Enumerable.Range(0, source.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(source.Substring(x, 2), 16))
                     .ToArray();
        }

        /// <summary>
        /// Encode byte data to Base32.
        /// </summary>
        /// <param name="source">Byte data to be encoded.</param>
        /// <returns>String data.</returns>
        public static string ToBase32(this byte[] source)
        {
            return source == null ? string.Empty : Base32.ToBase32String(source);
        }

        /// <summary>
        /// Decode string data from Base32.
        /// </summary>
        /// <param name="source">String data to be encoded.</param>
        /// <returns>Byte data.</returns>
        public static byte[] FromBase32(this string source)
        {
            return string.IsNullOrEmpty(source) ? null : Base32.FromBase32String(source);
        }

        /// <summary>
        /// Encode byte data to Base64.
        /// </summary>
        /// <param name="source">Byte data to be encoded.</param>
        /// <returns>String data.</returns>
        public static string ToBase64(this byte[] source)
        {
            return source == null ? string.Empty : Convert.ToBase64String(source);
        }

        /// <summary>
        /// Decode string data from Base64.
        /// </summary>
        /// <param name="source">String data to be encoded.</param>
        /// <returns>Byte data.</returns>
        public static byte[] FromBase64(this string source)
        {
            return string.IsNullOrEmpty(source) ? null : Convert.FromBase64String(source);
        }
    }
}
