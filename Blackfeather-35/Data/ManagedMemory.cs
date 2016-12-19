// Copyright 2013 Timothy D Meadows II
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Blackfeather.Data
{
    /// <summary>
    /// Managed memory entry holder.
    /// </summary>
    [Serializable]
    public struct ManagedMemorySpace
    {
        public long Created;
        public long Accessed;
        public long Updated;
        public string Pointer;
        public string Name;
        public object Value;
    }

    public enum ContentDataType
    {
        Text = 1,
        Json = 2
    }

    /// <summary>
    /// Managed memory class with serialization support.
    /// </summary>
    public sealed class ManagedMemory : IDisposable
    {
        private bool _disposed = false;
        private SynchronizedCollection<ManagedMemorySpace> _memory = new SynchronizedCollection<ManagedMemorySpace>();

        /// <summary>
        /// Managed memory read.
        /// </summary>
        /// <typeparam name="T">Try and make sure your types are serializable.</typeparam>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        /// <param name="name">A name pointer to the memory entry.</param>
        /// <returns>A memory value with type T.</returns>
        public T Read<T>(string pointer, string name)
        {
            lock (_memory)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("Memory");
                }

                SynchronizedCollection<ManagedMemorySpace> memoryPointer;
                lock (_memory.SyncRoot)
                {
                    memoryPointer = new SynchronizedCollection<ManagedMemorySpace>(new object(), _memory.ToArray());
                }

                if (!memoryPointer.Any())
                {
                    return default(T);
                }

                var memorySet = memoryPointer.Where(entry => entry.Pointer == pointer)
                    .Where(entry => entry.Name == name);
                if (!_memory.Any())
                {
                    return default(T);
                }

                var accessedEntry = memorySet.FirstOrDefault();
                accessedEntry.Accessed = DateTime.UtcNow.ToBinary();

                return typeof(T) == typeof(ManagedMemorySpace)
                    ? (T) Convert.ChangeType(accessedEntry, typeof(ManagedMemorySpace))
                    : (T) Convert.ChangeType(accessedEntry.Value, typeof(T));
            }
        }

        /// <summary>
        /// Managed memory bulk read.
        /// </summary>
        /// <typeparam name="T">Try and make sure your types are serializable.</typeparam>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        /// <returns>A object array of memory value with type T.</returns>
        public T[] ReadAll<T>(string pointer)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Memory");
            }

            if (!_memory.Any())
            {
                return null;
            }

            var memorySet = _memory.Where(entry => entry.Pointer == pointer);
            if (!_memory.Any())
            {
                return null;
            }

            var memoryFragment = new List<T>();
            memorySet.All(entry =>
            {
                entry.Accessed = DateTime.UtcNow.ToBinary();

                memoryFragment.Add(typeof(T) == typeof(ManagedMemorySpace)
                    ? (T) Convert.ChangeType(entry, typeof(ManagedMemorySpace))
                    : (T) Convert.ChangeType(entry.Value, typeof(T)));

                return true;
            });

            return memoryFragment.ToArray();
        }

        /// <summary>
        /// Managed memory write.
        /// </summary>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        /// <param name="name">A name pointer to the memory entry.</param>
        /// <param name="value">The object you wish to store in managed memory.</param>
        /// <param name="created">Binary time the object was created. (Optional)</param>
        /// <param name="updated">Binary time the object was updated. (Optional)</param>
        /// <param name="accessed">Binary time the object was last accessed. (Optional)</param>
        public void Write(string pointer, string name, object value, long created = 0, long updated = 0,
            long accessed = 0)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Memory");
            }

            Delete(pointer, name);
            var createStamp = created == 0 ? DateTime.UtcNow.ToBinary() : created;
            var updateStamp = updated == 0 ? createStamp : updated;
            var accessedStamp = accessed == 0 ? createStamp : accessed;

            _memory.Add(new ManagedMemorySpace()
            {
                Created = createStamp,
                Updated = updateStamp,
                Accessed = accessedStamp,
                Pointer = pointer,
                Name = name,
                Value = value
            });
        }

        public void WriteAll(ManagedMemorySpace[] spaces)
        {
            spaces.ToList()
                .ForEach(
                    entry => Write(entry.Pointer, entry.Name, entry.Value, entry.Created, entry.Updated, entry.Accessed));
        }

        public void WriteAll(List<ManagedMemorySpace> spaces)
        {
            spaces.ForEach(
                entry => Write(entry.Pointer, entry.Name, entry.Value, entry.Created, entry.Updated, entry.Accessed));
        }

        public void WriteAll(string pointer, Dictionary<string, object> spaces)
        {
            foreach (var space in spaces)
            {
                Write(pointer, space.Key, space.Value);
            }
        }

        /// <summary>
        /// Managed memory delete.
        /// </summary>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        /// <param name="name">A name pointer to the memory entry.</param>
        public void Delete(string pointer, string name)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Memory");
            }

            SynchronizedCollection<ManagedMemorySpace> memoryPointer;
            lock (_memory.SyncRoot)
            {
                memoryPointer = new SynchronizedCollection<ManagedMemorySpace>(new object(), _memory.ToArray());
            }

            if (!memoryPointer.Any())
            {
                return;
            }

            var memorySet = memoryPointer.Where(entry => entry.Pointer == pointer).Where(entry => entry.Name == name);
            if (!memorySet.Any())
            {
                return;
            }

            _memory.Remove(memorySet.First());
        }


        /// <summary>
        /// Managed memory bulk delete.
        /// </summary>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        public void DeleteAll(string pointer)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Memory");
            }

            if (!_memory.Any())
            {
                return;
            }

            var memorySet = _memory.Where(entry => entry.Pointer == pointer);
            memorySet.ToList().ForEach(entry => _memory.Remove(entry));
        }

        public void Clear()
        {
            _memory.Clear();
        }

        public ManagedMemorySpace[] Export()
        {
            return _memory.ToArray();
        }

        public ManagedMemorySpace[] ExportAll(string pointer)
        {
            return _memory.Where(entry => entry.Pointer == pointer).ToArray();
        }

        public void Import(ManagedMemorySpace[] spaces, bool append = false)
        {
            if (append)
            {
                foreach (var space in spaces)
                {
                    _memory.Add(space);
                }
            }
            else
            {
                _memory = new SynchronizedCollection<ManagedMemorySpace>(spaces);
            }
        }

        /// <summary>
        /// Load a managed memory object from disk.
        /// </summary>
        /// <param name="type">Supported content type.</param>
        /// <param name="path">Path to memory object on disk.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public void Load(ContentDataType type, string path, bool append = false, params Type[] knownTypes)
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
                    FromText(encodedFile, append);
                    break;
                case ContentDataType.Json:
                    encodedFile = new UTF8Encoding().GetString(file);
                    FromJson(encodedFile, append, knownTypes);
                    break;
            }
        }

        /// <summary>
        /// Save a managed memory object from disk.
        /// </summary>
        /// <param name="type">Supported content type.</param>
        /// <param name="path">Path to memory object on disk.</param>
        /// <param name="pointer">Optional, pointer you wish to write from.</param>
        public void Save(ContentDataType type, string path, string pointer = null, params Type[] knownTypes)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            object content = null;
            switch (type)
            {
                case ContentDataType.Text:
                    content = string.Join("\n", ToText(pointer));
                    File.WriteAllText(path, content.ToString());
                    break;
                case ContentDataType.Json:
                    content = ToJson(pointer, knownTypes);
                    if (content == null)
                    {
                        return;
                    }

                    File.WriteAllText(path, content.ToString());
                    break;
            }
        }

        /// <summary>
        /// Serialize data to text, or, basic CSV format.
        /// </summary>
        /// <param name="pointer">Optional, pointer you wish to serialize from.</param>
        /// <returns></returns>
        public string[] ToText(string pointer = null)
        {
            var memoryPointer = new List<string> {"Pointer,Name,Value,Created,Updated,Accessed"};
            var memoryFragment = string.IsNullOrEmpty(pointer) ? Export() : ExportAll(pointer);
            memoryPointer.AddRange(
                memoryFragment.Select(
                    entry =>
                            $"\"{entry.Pointer}\",\"{entry.Name}\",\"{entry.Value}\",{entry.Created},{entry.Updated},{entry.Accessed}"));

            return memoryPointer.ToArray();
        }

        /// <summary>
        /// Serialize data from text, or, basic CSV format.
        /// </summary>
        /// <param name="value">String data, or, CSV data.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public void FromText(string value, bool append = false)
        {
            if (!append)
            {
                Clear();
            }

            var payload = !value.Contains("\r\n")
                ? value.Split('\n')
                : value.Split(new[] {'\r', '\n'}, StringSplitOptions.None);
            foreach (
                var csv in
                payload.Select(payloadLine => payloadLine.Split(','))
                    .Where(csv => csv.Length == 6)
                    .Where(csv => csv[5].ToLower() != "accessed"))
            {
                Write(csv[0], csv[1], csv[2], Convert.ToInt64(csv[3]), Convert.ToInt64(csv[4]), Convert.ToInt64(csv[5]));
            }
        }

        /// <summary>
        /// Serialize to json format data.
        /// </summary>
        /// <param name="pointer">Optional, pointer you wish to serialize from.</param>
        /// <returns>String data</returns>
        public string ToJson(string pointer = null, params Type[] knownTypes)
        {
            var memorySpace = new MemoryStream();
            var memoryFilter = string.IsNullOrEmpty(pointer) ? Export() : ExportAll(pointer);
            var contract = new DataContractJsonSerializer(typeof(ManagedMemorySpace[]), knownTypes);
            contract.WriteObject(memorySpace, memoryFilter.ToArray());

            var json = System.Text.Encoding.UTF8.GetString(memorySpace.ToArray());
            memorySpace.Dispose();

            return json;
        }

        /// <summary>
        /// Serialize from json format data.
        /// </summary>
        /// <param name="value">String data, or, CSV data.</param>
        /// <param name="append">Should memory be cleared or left intact before loading?</param>
        public void FromJson(string value, bool append = false, params Type[] knownTypes)
        {
            var memorySpace = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value));
            var contract = new DataContractJsonSerializer(typeof(ManagedMemorySpace[]), knownTypes);
            var memorySpaces = (ManagedMemorySpace[]) contract.ReadObject(memorySpace);

            memorySpace.Dispose();
            Import(memorySpaces, append);
        }

        /// <summary>
        /// Dispose of all current memory. Object must be re-created before it can be used again.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _memory.Clear();
            _memory = null;

            _disposed = true;
        }
    }
}