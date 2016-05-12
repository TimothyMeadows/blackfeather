using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Blackfeather.Data.Compression
{
    /// <summary>
    /// Compress, and, decompress string data, and, binary data using GZip
    /// </summary>
    public class GZipCompression
    {
        /// <summary>
        /// Compress string data using the supplied encoding
        /// </summary>
        /// <param name="value">String to compress</param>
        /// <param name="encoding">Encoding to compress with</param>
        /// <returns></returns>
        public static byte[] Compress(string value, System.Text.Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            return Compress(encoding.GetBytes(value));
        }

        /// <summary>
        /// Compress binary data
        /// </summary>
        /// <param name="value">Binary data to compress</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] value)
        {
            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Compress))
                {
                    gzip.Write(value, 0, value.Length);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Decompress string data using the supplied encoding
        /// </summary>
        /// <param name="value">Binary compressed data</param>
        /// <param name="encoding">Encoding to decompress with</param>
        /// <returns></returns>
        public static string Decompress(byte[] value, System.Text.Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            return encoding.GetString(Decompress(value));
        }

        /// <summary>
        /// Decompress binary data
        /// </summary>
        /// <param name="value">Binary data to compress</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] value)
        {
            using (var memory = new MemoryStream(value))
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Decompress))
                {
                    gzip.Read(value, 0, value.Length);
                    return memory.ToArray();
                }
            }
        }
    }
}
