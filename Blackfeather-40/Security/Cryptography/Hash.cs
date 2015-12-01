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

using Org.BouncyCastle.Crypto.Digests;

namespace Blackfeather.Security.Cryptography
{
    public static class Hash
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
        /// Hash string data based on the supported digest types, also, devinates the key data based on supported devination types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="digestType">Supported digest type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHash(this string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var saltedData = default(SaltedData);

            switch (digestType)
            {
                case DigestType.Sha256:
                    saltedData = ToHash_Sha256(data, digestType, divinationType, salt);
                    break;
                case DigestType.Sha384:
                    saltedData = ToHash_Sha384(data, digestType, divinationType, salt);
                    break;
                case DigestType.Sha512:
                    saltedData = ToHash_Sha512(data, digestType, divinationType, salt);
                    break;
                case DigestType.Whirlpool:
                    saltedData = ToHash_Whirlpool(data, digestType, divinationType, salt);
                    break;
                case DigestType.Gost3411:
                    saltedData = ToHash_Gost3411(data, digestType, divinationType, salt);
                    break;
                case DigestType.Tiger:
                    saltedData = ToHash_Tiger(data, digestType, divinationType, salt);
                    break;
            }

            return saltedData;
        }

        /// <summary>
        /// Hash byte data based on the supported digest types, also, devinates the key data based on supported devination types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="digestType">Supported digest type.</param>
        /// <param name="salt">Optional, supplied 8 byte salt, one will be auto-generated if not supplied.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToHash(this byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var saltedData = default(SaltedData);

            switch (digestType)
            {
                case DigestType.Sha256:
                    saltedData = ToHash_Sha256(data, digestType, divinationType, salt);
                    break;
                case DigestType.Sha384:
                    saltedData = ToHash_Sha384(data, digestType, divinationType, salt);
                    break;
                case DigestType.Sha512:
                    saltedData = ToHash_Sha512(data, digestType, divinationType, salt);
                    break;
                case DigestType.Whirlpool:
                    saltedData = ToHash_Whirlpool(data, digestType, divinationType, salt);
                    break;
                case DigestType.Gost3411:
                    saltedData = ToHash_Gost3411(data, digestType, divinationType, salt);
                    break;
                case DigestType.Tiger:
                    saltedData = ToHash_Tiger(data, digestType, divinationType, salt);
                    break;
            }

            return saltedData;
        }

        private static SaltedData ToHash_Sha256(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Sha256Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha256(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Sha256Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha384(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Sha384Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha384(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Sha384Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha512(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Sha512Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Sha512(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Sha512Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Finish();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Whirlpool(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new WhirlpoolDigest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Whirlpool(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new WhirlpoolDigest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Gost3411(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Gost3411Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Gost3411(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new Gost3411Digest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Tiger(string data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new TigerDigest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }

        private static SaltedData ToHash_Tiger(byte[] data, DigestType digestType, KeyDevination.DevinationType divinationType, byte[] salt = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = data.ToKeyDevination(divinationType, salting).Data;
            var digest = new TigerDigest();

            digest.BlockUpdate(derived, 0, derived.Length);

            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            digest.Reset();

            return new SaltedData() { Data = output, Salt = salting };
        }
    }
}
