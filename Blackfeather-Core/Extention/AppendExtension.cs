/* 
 The MIT License (MIT)

 Copyright (c) 2013 - 2015 Timothy D Meadows II

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

namespace Blackfeather.Extention
{
    /// <summary>
    /// Append an element to an array
    /// </summary>
    public static class AppendExtension
    {
        /// <summary>
        /// Append an element to an array
        /// </summary>
        /// <typeparam name="T">Any matching type</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="destination">Destination array</param>
        /// <param name="sourceLength">Length of source array, if not supplied source.Length is used.</param>
        /// <param name="destinationLength">Length of destination array, if not supplied destination.Length is used.</param>
        /// <returns>Transformed array</returns>
        public static T[] Append<T>(this T[] source, T[] destination, int sourceLength = 0, int destinationLength = 0)
        {
            var expandSourceLength = sourceLength > 0 ? sourceLength : source.Length;
            var expandDestinationLength = destinationLength > 0 ? destinationLength : destination.Length;
            var expanded = new T[expandSourceLength + expandDestinationLength];

            Array.Copy(source, 0, expanded, 0, expandSourceLength);
            Array.Copy(destination, 0, expanded, expandSourceLength, expandDestinationLength);

            return expanded;
        }
    }
}