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
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace Blackfeather.Security.Cryptography
{
    public static class Hmac
    {
        /// <summary>
        /// Hash+Mac string data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <returns>byte[]</returns>
        public static byte[] ToHmac(this string data, string key)
        {
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = new UTF8Encoding().GetBytes(key);
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return output;
        }

        /// <summary>
        /// Hash+Mac string data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this string data, string key, byte[] salt)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() {Data = output, Salt = salting};
        }

        /// <summary>
        /// Hash+Mac string data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <returns>SaltedData</returns>
        public static byte[] ToHmac(this string data, byte[] key)
        {
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(key));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return output;
        }

        /// <summary>
        /// Hash+Mac string data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this string data, byte[] key, byte[] salt)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() {Data = output, Salt = salting};
        }

        /// <summary>
        /// Hash+Mac byte data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <returns>SaltedData</returns>
        public static byte[] ToHmac(this byte[] data, string key)
        {
            var derivedKey = new UTF8Encoding().GetBytes(key);
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return output;
        }

        /// <summary>
        /// Hash+Mac byte data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this byte[] data, string key, byte[] salt)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        /// <summary>
        /// Hash+Mac byte data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <returns>SaltedData</returns>
        public static byte[] ToHmac(this byte[] data, byte[] key)
        {
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(key));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return output;
        }

        /// <summary>
        /// Hash+Mac byte data based on the supported digest types, also, deviates the key data based on supported divination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be deviated using the supplied divination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this byte[] data, byte[] key, byte[] salt)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }
    }
}
