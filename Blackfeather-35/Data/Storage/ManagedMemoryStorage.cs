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

using System.Text;
using System.IO;
using Blackfeather.Data.Serialization;

namespace Blackfeather.Data.Storage
{
    public static class ManagedMemoryStorage
    {
        /// <summary>
        /// Supported content data types
        /// </summary>
        public enum ContentDataType
        {
            Text = 1,
            Binary = 2,
            Xml = 3,
            Json = 4
        }

        /// <summary>
        /// Load a managed memory object from disk.
        /// </summary>
        /// <param name="memory">Memory object.</param>
        /// <param name="type">Supported content type.</param>
        /// <param name="path">Path to memory object on disk.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public static void Load(this ManagedMemory memory, ContentDataType type, string path, bool append = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var file = File.ReadAllBytes(path);
            var encodedFile = string.Empty;
            switch (type)
            {
                case ContentDataType.Text:
                    encodedFile = new UTF8Encoding().GetString(file);
                    memory.FromText(encodedFile, append);
                    break;
                case ContentDataType.Xml:
                    encodedFile = new UTF8Encoding().GetString(file);
                    memory.FromXml(encodedFile);
                    break;
                case ContentDataType.Json:
                    encodedFile = new UTF8Encoding().GetString(file);
                    memory.FromJson(encodedFile);
                    break;
            }
        }

        /// <summary>
        /// Save a managed memory object from disk.
        /// </summary>
        /// <param name="memory">Memory object.</param>
        /// <param name="type">Supported content type.</param>
        /// <param name="path">Path to memory object on disk.</param>
        /// <param name="pointer">Optional, pointer you wish to write from.</param>
        public static void Save(this ManagedMemory memory, ContentDataType type, string path, string pointer = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            object content = null;
            switch (type)
            {
                case ContentDataType.Text:
                    content = memory.ToText(pointer);
                    if (content == null)
                    {
                        return;
                    }

                    File.WriteAllText(path, content.ToString());
                    break;
                case ContentDataType.Xml:
                    content = memory.ToXml(pointer);
                    if (content == null)
                    {
                        return;
                    }

                    File.WriteAllText(path, content.ToString());
                    break;
                case ContentDataType.Json:
                    content = memory.ToJson(pointer);
                    if (content == null)
                    {
                        return;
                    }

                    File.WriteAllText(path, content.ToString());
                    break;
            }

        }
    }
}
