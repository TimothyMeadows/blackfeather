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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Blackfeather.Data.Serialization
{
    public static class ManagedMemorySerialization
    {
        /// <summary>
        /// Serialize data to text, or, basic CSV format.
        /// </summary>
        /// <param name="memory">Managed memory object.</param>
        /// <param name="pointer">Optional, pointer you wish to serialize from.</param>
        /// <returns></returns>
        public static string[] ToText(this ManagedMemory memory, string pointer = null)
        {
            var memoryPointer = new List<string> { "Pointer,Name,Value,Created,Updated,Accessed" };
            var memoryFragment = string.IsNullOrEmpty(pointer) ? memory.Export() : memory.ExportAll(pointer);
            memoryPointer.AddRange(memoryFragment.Select(entry => $"\"{entry.Pointer}\",\"{entry.Name}\",\"{entry.Value}\",{entry.Created},{entry.Updated},{entry.Accessed}"));

            return memoryPointer.ToArray();
        }

        /// <summary>
        /// Serialize data from text, or, basic CSV format.
        /// </summary>
        /// <param name="memory">Managed memory object.</param>
        /// <param name="value">String data, or, CSV data.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public static void FromText(this ManagedMemory memory, string value, bool append = false)
        {
            if (!append)
            {
                memory.Clear();
            }

            var text = !value.Contains("\r\n") ? value.Split('\n') : value.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            foreach (var textLine in text.Select(payloadLine => payloadLine.Split(',')).Where(textLine => textLine.Length == 6).Where(textLine => textLine[5].ToLower() != "accessed"))
            {
                memory.Write(textLine[0], textLine[1], textLine[2], Convert.ToInt64(textLine[3]), Convert.ToInt64(textLine[4]), Convert.ToInt64(textLine[5]));
            }
        }

        /// <summary>
        /// Serailize to xml format data.
        /// </summary>
        /// <param name="memory">Managed memory object.</param>
        /// <param name="pointer">Optional, pointer you wish to serialize from.</param>
        /// <returns>String data.</returns>
        public static string ToXml(this ManagedMemory memory, string pointer = null, System.Text.Encoding encoding = null)
        {
            var memorySpace = new MemoryStream();
            var memoryFragment = string.IsNullOrEmpty(pointer) ? memory.Export() : memory.ExportAll(pointer);
            var contract = new DataContractSerializer(typeof(ManagedMemorySpace[]));
            contract.WriteObject(memorySpace, memoryFragment.ToArray());

            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            var xml = encoding.GetString(memorySpace.ToArray());
            memorySpace.Dispose();

            return xml;
        }

        /// <summary>
        /// Serialize from xml format data.
        /// </summary>
        /// <param name="memory">Managed memory object.</param>
        /// <param name="value">String data, or, CSV data.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public static void FromXml(this ManagedMemory memory, string value, bool append = false, System.Text.Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            var memorySpace = new MemoryStream(encoding.GetBytes(value));
            var contract = new DataContractSerializer(typeof(ManagedMemorySpace[]));
            var memorySpaces = (ManagedMemorySpace[])contract.ReadObject(memorySpace);
            memorySpace.Dispose();

            memory.Import(memorySpaces, append);
        }

        /// <summary>
        /// Serialize to json format data.
        /// </summary>
        /// <param name="memory">Managed memory object.</param>
        /// <param name="pointer">Optional, pointer you wish to serialize from.</param>
        /// <returns>String data</returns>
        public static string ToJson(this ManagedMemory memory, string pointer = null, System.Text.Encoding encoding = null)
        {
            var memorySpace = new MemoryStream();
            var memoryFilter = string.IsNullOrEmpty(pointer) ? memory.Export() : memory.ExportAll(pointer);
            var contract = new DataContractJsonSerializer(typeof(ManagedMemorySpace[]));
            contract.WriteObject(memorySpace, memoryFilter.ToArray());

            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            var json = encoding.GetString(memorySpace.ToArray());
            memorySpace.Dispose();

            return json;
        }

        /// <summary>
        /// Serialize from json format data.
        /// </summary>
        /// <param name="memory">Managed memory object.</param>
        /// <param name="value">String data, or, CSV data.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public static void FromJson(this ManagedMemory memory, string value, bool append = false, System.Text.Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            var memorySpace = new MemoryStream(encoding.GetBytes(value));
            var contract = new DataContractJsonSerializer(typeof(ManagedMemorySpace[]));
            var memorySpaces = (ManagedMemorySpace[])contract.ReadObject(memorySpace);
            memorySpace.Dispose();

            memory.Import(memorySpaces, append);
        }
    }
}
