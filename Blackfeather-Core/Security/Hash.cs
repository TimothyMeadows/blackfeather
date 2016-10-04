/* 
 The MIT License (MIT)

 Copyright (c) 2013 - 2014 Timothy D Meadows II

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

using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace Blackfeather.Security.Cryptography
{
    public static class Hash
    {
        /// <summary>
        /// Hash string data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <returns>SaltedData</returns>
        public static byte[] ToHash(this string data)
        {
            var derived = new UTF8Encoding().GetBytes(data);
            var digest = new Sha256Digest();
            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return output;
        }

        /// <summary>
        /// Hash string data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHash(this string data, byte[] salt)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(salting).Data;
            var digest = new Sha256Digest();
            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        /// <summary>
        /// Hash byte data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <returns>SaltedData</returns>
        public static byte[] ToHash(this byte[] data)
        {
            var digest = new Sha256Digest();

            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return output;
        }

        /// <summary>
        /// Hash byte data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHash(this byte[] data, byte[] salt)
        {
            var derived = salt != null ? data.ToKeyDevination(salt).Data : data;
            var digest = new Sha256Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salt };
        }
    }
}
