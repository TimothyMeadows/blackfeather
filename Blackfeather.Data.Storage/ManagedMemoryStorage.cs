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
        public enum ContentDataType
        {
            Text = 1,
            Binary = 2,
            Xml = 3,
            Json = 4
        }

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
