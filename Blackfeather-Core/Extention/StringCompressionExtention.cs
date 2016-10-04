using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blackfeather.Data.Compression;
using Blackfeather.Data.Encoding;

namespace Blackfeather.Extention
{
    public static class StringCompressionExtention
    {
        /// <summary>
        /// Compress string data using the supplied encoding
        /// </summary>
        /// <param name="value">String to compress</param>
        /// <param name="encoding">Encoding to compress with</param>
        /// <returns></returns>
        public static string Compress(this string value, Encoding encoding = null)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : GZipCompression.Compress(value, encoding).ToBase64();
        }

        /// <summary>
        /// Decompress string data using the supplied encoding
        /// </summary>
        /// <param name="value">Base64 string to decompress</param>
        /// <param name="encoding">Encoding to decompress with</param>
        /// <returns></returns>
        public static string Decompress(this string value, Encoding encoding = null)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : GZipCompression.Decompress(value.FromBase64(), encoding);
        }
    }
}
