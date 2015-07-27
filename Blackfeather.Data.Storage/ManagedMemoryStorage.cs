using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                case ContentDataType.Binary:
                    memory.FromBinary(file, append);
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
                case ContentDataType.Binary:
                    content = memory.ToBinary(pointer);
                    if (content == null)
                    {
                        return;
                    }

                    File.WriteAllBytes(path, (byte[]) content);
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
