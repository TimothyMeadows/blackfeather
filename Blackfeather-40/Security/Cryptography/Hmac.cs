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
using Blackfeather.Data.Encoding;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace Blackfeather.Security.Cryptography
{
    public static class Hmac
    {
        public enum DigestType
        {
            /// <summary>
            /// 256 bit hash
            /// </summary>
            Sha256 = 1,
            /// <summary>
            /// 384 bit hash
            /// </summary>
            Sha384 = 2,
            /// <summary>
            /// 512 bit hash
            /// </summary>
            Sha512 = 3,
            /// <summary>
            /// 512 bit hash
            /// </summary>
            Whirlpool = 4,
            /// <summary>
            /// 256 bit hash
            /// </summary>
            Gost3411 = 5,
            /// <summary>
            /// 192 bit hash
            /// </summary>
            Tiger = 6
        }

        /// <summary>
        /// Hash+Mac string data based on the supported digest types, also, devinates the key data based on supported devination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be devinated using the supplied devination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this string data, DigestType digestType, KeyDevination.DevinationType devinationType, string key, byte[] salt = null)
        {
            var saltedData = default(SaltedData);

            switch (digestType)
            {
                case DigestType.Sha256:
                    saltedData = ToHash_Sha256(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha384:
                    saltedData = ToHash_Sha384(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha512:
                    saltedData = ToHash_Sha512(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Gost3411:
                    saltedData = ToHash_Gost3411(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Whirlpool:
                    saltedData = ToHash_Whirlpool(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Tiger:
                    saltedData = ToHash_Tiger(data, digestType, devinationType, key, salt);
                    break;
            }

            return saltedData;
        }

        /// <summary>
        /// Hash+Mac string data based on the supported digest types, also, devinates the key data based on supported devination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be devinated using the supplied devination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this string data, DigestType digestType, KeyDevination.DevinationType devinationType, byte[] key, byte[] salt = null)
        {
            var saltedData = default(SaltedData);

            switch (digestType)
            {
                case DigestType.Sha256:
                    saltedData = ToHash_Sha256(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha384:
                    saltedData = ToHash_Sha384(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha512:
                    saltedData = ToHash_Sha512(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Gost3411:
                    saltedData = ToHash_Gost3411(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Whirlpool:
                    saltedData = ToHash_Whirlpool(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Tiger:
                    saltedData = ToHash_Tiger(data, digestType, devinationType, key, salt);
                    break;
            }

            return saltedData;
        }

        /// <summary>
        /// Hash+Mac byte data based on the supported digest types, also, devinates the key data based on supported devination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be devinated using the supplied devination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this byte[] data, DigestType digestType, KeyDevination.DevinationType devinationType, string key, byte[] salt = null)
        {
            var saltedData = default(SaltedData);

            switch (digestType)
            {
                case DigestType.Sha256:
                    saltedData = ToHash_Sha256(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha384:
                    saltedData = ToHash_Sha384(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha512:
                    saltedData = ToHash_Sha512(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Gost3411:
                    saltedData = ToHash_Gost3411(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Whirlpool:
                    saltedData = ToHash_Whirlpool(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Tiger:
                    saltedData = ToHash_Tiger(data, digestType, devinationType, key, salt);
                    break;
            }

            return saltedData;
        }

        /// <summary>
        /// Hash+Mac byte data based on the supported digest types, also, devinates the key data based on supported devination types.
        /// </summary>
        /// <param name="data">Data of any encoding type</param>
        /// <param name="key">Hash password. Will be devinated using the supplied devination type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHmac(this byte[] data, DigestType digestType, KeyDevination.DevinationType devinationType, byte[] key, byte[] salt = null)
        {
            var saltedData = default(SaltedData);

            switch (digestType)
            {
                case DigestType.Sha256:
                    saltedData = ToHash_Sha256(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha384:
                    saltedData = ToHash_Sha384(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Sha512:
                    saltedData = ToHash_Sha512(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Gost3411:
                    saltedData = ToHash_Gost3411(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Whirlpool:
                    saltedData = ToHash_Whirlpool(data, digestType, devinationType, key, salt);
                    break;
                case DigestType.Tiger:
                    saltedData = ToHash_Tiger(data, digestType, devinationType, key, salt);
                    break;
            }

            return saltedData;
        }

        private static SaltedData ToHash_Sha256(string data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha256(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha256(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha256(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha384(string data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha384Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha384(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha384Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha384(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha384Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha384(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha384Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha512(string data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha512Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha512(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha512Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha512(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha512Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha512(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Sha512Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Gost3411(string data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Gost3411Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Gost3411(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Gost3411Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Gost3411(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Gost3411Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Gost3411(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new Gost3411Digest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Whirlpool(string data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new WhirlpoolDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Whirlpool(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new WhirlpoolDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Whirlpool(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new WhirlpoolDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Whirlpool(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new WhirlpoolDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Tiger(string data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new TigerDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Tiger(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var dataBytes = new UTF8Encoding().GetBytes(data);
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new TigerDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(dataBytes, 0, dataBytes.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Tiger(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, string key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new TigerDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Tiger(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] key, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derivedKey = key.ToKeyDevination(divinationType, salting).Data;
            var digest = new HMac(new TigerDigest());

            digest.Init(new KeyParameter(derivedKey));
            digest.BlockUpdate(data, 0, data.Length);

            var output = new byte[digest.GetMacSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }
    }
}
