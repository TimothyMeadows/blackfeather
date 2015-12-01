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

using System.Security.Cryptography;
using System.Text;
using CryptSharp.Utility;

namespace Blackfeather.Security.Cryptography
{
    public static class KeyDevination
    {
        /// <summary>
        /// Supported devination types.
        /// </summary>
        public enum DevinationType
        {
            Pbkdf2 = 1,
            Scrypt = 2
        }

        /// <summary>
        /// PBKDF2 Iteration Count.
        /// </summary>
        public static int PBKDF2_ITERATIONS = 10000;
        /// <summary>
        /// Scrypt CPU Cost. (More = More Secure, Less = Less Secure)
        /// </summary>
        public static int SCRYPT_COST = 262144;
        /// <summary>
        /// Scrypt blocksize used.
        /// </summary>
        public static int SCRYPT_BLOCKSIZE = 8;
        /// <summary>
        /// Scrypt parallel threading
        /// </summary>
        public static int SCRYPT_PARALLEL = 1;
        /// <summary>
        /// Scrypt max thread count
        /// </summary>
        public static int? SCRYPT_MAXTHREADS = null;

        /// <summary>
        /// Devinate string data based on supported types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="type">Supported devination types.</param>
        /// <param name="salt">Devination salt.</param>
        /// <param name="length">Devination returned byte size.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToKeyDevination(this string data, DevinationType type, byte[] salt = null, int length = 128)
        {
            var saltedData = default(SaltedData);

            switch (type)
            {
                case DevinationType.Pbkdf2:
                    saltedData = ToKeyDevination_Pbkdf2(data, type, salt, length);
                    break;
                case DevinationType.Scrypt:
                    saltedData = ToKeyDevination_Scrypt(data, type, salt, length);
                    break;
            }

            return saltedData;
        }

        /// <summary>
        /// Devinate byte data based on supported types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="type">Supported devination types.</param>
        /// <param name="salt">Devination salt.</param>
        /// <param name="length">Devination returned byte size.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToKeyDevination(this byte[] data, DevinationType type, byte[] salt = null, int length = 128)
        {
            var saltedData = default(SaltedData);

            switch (type)
            {
                case DevinationType.Pbkdf2:
                    saltedData = ToKeyDevination_Pbkdf2(data, type, salt, length);
                    break;
                case DevinationType.Scrypt:
                    saltedData = ToKeyDevination_Scrypt(data, type, salt, length);
                    break;
            }

            return saltedData;
        }

        private static SaltedData ToKeyDevination_Pbkdf2(string data, DevinationType type, byte[] salt = null, int length = 128)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var pbkdf2 = new Rfc2898DeriveBytes(data, salting, PBKDF2_ITERATIONS);
            var derived = pbkdf2.GetBytes(length);
            pbkdf2.Reset();

            return new SaltedData() { Salt = salting, Data = derived };
        }

        private static SaltedData ToKeyDevination_Pbkdf2(byte[] data, DevinationType type, byte[] salt = null, int length = 128)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var pbkdf2 = new Rfc2898DeriveBytes(data, salting, PBKDF2_ITERATIONS);
            var derived = pbkdf2.GetBytes(length);
            pbkdf2.Reset();

            return new SaltedData() { Salt = salting, Data = derived };
        }

        private static SaltedData ToKeyDevination_Scrypt(string data, DevinationType type, byte[] salt = null, int length = 128)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = SCrypt.ComputeDerivedKey(new UTF8Encoding().GetBytes(data), salting, SCRYPT_COST, SCRYPT_BLOCKSIZE, SCRYPT_PARALLEL, SCRYPT_MAXTHREADS, length);

            return new SaltedData() { Salt = salting, Data = derived };
        }

        private static SaltedData ToKeyDevination_Scrypt(byte[] data, DevinationType type, byte[] salt = null, int length = 128)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var derived = SCrypt.ComputeDerivedKey(data, salting, SCRYPT_COST, SCRYPT_BLOCKSIZE, SCRYPT_PARALLEL, SCRYPT_MAXTHREADS, length);

            return new SaltedData() { Salt = salting, Data = derived };
        }
    }
}