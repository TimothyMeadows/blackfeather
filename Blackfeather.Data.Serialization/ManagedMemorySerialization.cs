﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Blackfeather.Data.Serialization
{
    public static class ManagedMemorySerialization
    {
        public static string[] ToText(this ManagedMemory memory, string pointer = null)
        {
            var memoryPointer = new List<string> { "Pointer,Name,Value,Created,Updated,Accessed" };
            var memoryFragment = string.IsNullOrEmpty(pointer) ? memory.Export(false) : memory.ExportAll(pointer, false);
            memoryPointer.AddRange(memoryFragment.Select(entry => string.Format("\"{0}\",\"{1}\",\"{2}\",{3},{4},{5}", entry.Pointer, entry.Name, entry.Value, entry.Created, entry.Updated, entry.Accessed)));

            return memoryPointer.ToArray();
        }

        public static void FromText(this ManagedMemory memory, string value, bool append = false)
        {
            if (!append)
            {
                memory.Clear();
            }

            var payload = !value.Contains("\r\n") ? value.Split('\n') : value.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            foreach (var csv in payload.Select(payloadLine => payloadLine.Split(',')).Where(csv => csv.Length == 6).Where(csv => csv[5].ToLower() != "accessed"))
            {
                memory.Write(csv[0], csv[1], csv[2], Convert.ToInt64(csv[3]), Convert.ToInt64(csv[4]), Convert.ToInt64(csv[5]));
            }
        }

        public static byte[] ToBinary(this ManagedMemory memory, string pointer = null)
        {
            var memorySpace = new MemoryStream();
            var memoryFragment = string.IsNullOrEmpty(pointer) ? memory.Export(false) : memory.ExportAll(pointer, false);
            new BinaryFormatter().Serialize(memorySpace, memoryFragment);

            var output = memorySpace.ToArray();
            memorySpace.Dispose();

            return output;
        }

        public static void FromBinary(this ManagedMemory memory, byte[] value, bool append = false)
        {
            var memorySpace = new MemoryStream(value);
            var memorySpaces = (ManagedMemorySpace[])new BinaryFormatter().Deserialize(memorySpace);
            memorySpace.Dispose();

            memory.Import(memorySpaces, append);
        }

        public static string ToXml(this ManagedMemory memory, string pointer = null)
        {
            var memorySpace = new MemoryStream();
            var memoryFragment = string.IsNullOrEmpty(pointer) ? memory.Export(false) : memory.ExportAll(pointer, false);
            var contract = new DataContractSerializer(typeof(ManagedMemorySpace[]));
            contract.WriteObject(memorySpace, memoryFragment.ToArray());

            var xml = Encoding.UTF8.GetString(memorySpace.ToArray());
            memorySpace.Dispose();

            return xml;
        }

        public static void FromXml(this ManagedMemory memory, string value, bool append = false)
        {
            var memorySpace = new MemoryStream(Encoding.UTF8.GetBytes(value));
            var contract = new DataContractSerializer(typeof(ManagedMemorySpace[]));
            var memorySpaces = (ManagedMemorySpace[])contract.ReadObject(memorySpace);
            memorySpace.Dispose();

            memory.Import(memorySpaces, append);
        }

        public static string ToJson(this ManagedMemory memory, string pointer = null)
        {
            var memorySpace = new MemoryStream();
            var memoryFilter = string.IsNullOrEmpty(pointer) ? memory.Export(false) : memory.ExportAll(pointer, false);
            var contract = new DataContractJsonSerializer(typeof(ManagedMemorySpace[]));
            contract.WriteObject(memorySpace, memoryFilter.ToArray());

            var json = Encoding.UTF8.GetString(memorySpace.ToArray());
            memorySpace.Dispose();

            return json;
        }

        public static void FromJson(this ManagedMemory memory, string value, bool append = false)
        {
            var memorySpace = new MemoryStream(Encoding.UTF8.GetBytes(value));
            var contract = new DataContractJsonSerializer(typeof(ManagedMemorySpace[]));
            var memorySpaces = (ManagedMemorySpace[])contract.ReadObject(memorySpace);
            memorySpace.Dispose();

            memory.Import(memorySpaces, append);
        }
    }
}