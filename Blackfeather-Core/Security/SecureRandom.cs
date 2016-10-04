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
namespace Blackfeather.Security.Cryptography
{
    /// <summary>
    /// Secure random number generator
    /// </summary>
    public static class SecureRandom
    {
        /// <summary>
        /// Generate a secure random number.
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Randomized number</returns>
        public static int ToRandom(this int min, int max)
        {
            return new Org.BouncyCastle.Security.SecureRandom().Next(min, max);
        }

        /// <summary>
        /// Generate a secure random number of bytes.
        /// </summary>
        /// <param name="length">Length of bytes you need randomized</param>
        /// <returns>Randomized number of bytes</returns>
        public static byte[] ToRandomBytes(this int length)
        {
            var result = new byte[length];
            new Org.BouncyCastle.Security.SecureRandom().NextBytes(result, 0, length);

            return result;
        }
    }
}