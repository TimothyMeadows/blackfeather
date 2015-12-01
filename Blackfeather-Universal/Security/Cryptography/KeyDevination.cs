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

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Security;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Blackfeather.Security.Cryptography
{
    public static class KeyDevination
    {
        /// <summary>
        /// PBKDF2 Iteration Count.
        /// </summary>
        public static uint PBKDF2_ITERATIONS = 10000;

        /// <summary>
        /// Devinate string data based on supported types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="salt">Devination salt.</param>
        /// <param name="length">Devination returned byte size.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToKeyDevination(this string data, byte[] salt = null, uint length = 128)
        {
            var saltedData = ToKeyDevination_Pbkdf2(data, salt, length);

            return saltedData;
        }

        /// <summary>
        /// Devinate byte data based on supported types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="salt">Devination salt.</param>
        /// <param name="length">Devination returned byte size.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToKeyDevination(this byte[] data, byte[] salt = null, uint length = 128)
        {
            var saltedData = ToKeyDevination_Pbkdf2(data, salt, length);

            return saltedData;
        }

        private static SaltedData ToKeyDevination_Pbkdf2(string data, byte[] salt = null, uint length = 128, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            var pbkdf2 = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256);
            var buffSecret = CryptographicBuffer.ConvertStringToBinary(data, encoding);
            var buffSalt = CryptographicBuffer.CreateFromByteArray(salt);
            var pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, PBKDF2_ITERATIONS);
            var keyOriginal = pbkdf2.CreateKey(buffSecret);

            var keyMaterial = CryptographicEngine.DeriveKeyMaterial (
                keyOriginal,
                pbkdf2Params,
                length
            );

            var key = pbkdf2.CreateKey(keyMaterial);
            return new SaltedData() { Salt = buffSalt.ToArray(), Data = key.Export().ToArray() };
        }

        private static SaltedData ToKeyDevination_Pbkdf2(byte[] data, byte[] salt = null, uint length = 128, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            var pbkdf2 = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256);
            var buffSecret = CryptographicBuffer.CreateFromByteArray(data);
            var buffSalt = CryptographicBuffer.CreateFromByteArray(salt);
            var pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, PBKDF2_ITERATIONS);
            var keyOriginal = pbkdf2.CreateKey(buffSecret);

            var keyMaterial = CryptographicEngine.DeriveKeyMaterial(
                keyOriginal,
                pbkdf2Params,
                length
            );

            var key = pbkdf2.CreateKey(keyMaterial);
            return new SaltedData() { Salt = buffSalt.ToArray(), Data = key.Export().ToArray() };
        }
    }
}