using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blackfeather.Data.Compression;
using Blackfeather.Data.Encoding;

namespace Blackfeather.Extention
{
    public static class BinaryCompressionExtention
    {
        /// <summary>
        /// Compress binary data
        /// </summary>
        /// <param name="value">Binary to compress</param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] value)
        {
            return value == null ? null : GZipCompression.Compress(value);
        }

        /// <summary>
        /// Decompress binary data
        /// </summary>
        /// <param name="value">Binary to decompress</param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] value)
        {
            return value == null ? null : GZipCompression.Decompress(value);
        }
    }
}
