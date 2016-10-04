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

namespace Blackfeather.Security.Cryptography
{
    public static class KeyDevination
    {
        /// <summary>
        /// PBKDF2 Iteration Count.
        /// </summary>
        public static int PBKDF2_ITERATIONS = 1;

        /// <summary>
        /// Deviate string data based on supported types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="salt">Divination salt.</param>
        /// <param name="length">Divination returned byte size.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToKeyDevination(this string data, byte[] salt = null, int length = 32)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var pbkdf2 = new Rfc2898DeriveBytes(data, salting, PBKDF2_ITERATIONS);
            var derived = pbkdf2.GetBytes(length);
            pbkdf2.Reset();

            return new SaltedData() { Salt = salting, Data = derived };
        }

        /// <summary>
        /// Deviate byte data based on supported types.
        /// </summary>
        /// <param name="data">Data of any encoding type.</param>
        /// <param name="salt">Divination salt.</param>
        /// <param name="length">Divination returned byte size.</param>
        /// <returns>SaltedData</returns>
        public static SaltedData ToKeyDevination(this byte[] data, byte[] salt = null, int length = 32)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var pbkdf2 = new Rfc2898DeriveBytes(data, salting, PBKDF2_ITERATIONS);
            var derived = pbkdf2.GetBytes(length);
            pbkdf2.Reset();

            return new SaltedData() { Salt = salting, Data = derived };
        }
    }
}