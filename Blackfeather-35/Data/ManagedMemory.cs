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
using System.Linq;

namespace Blackfeather.Data
{
    /// <summary>
    /// Managed memory class with serialization support.
    /// </summary>
    public sealed class ManagedMemory : IDisposable
    {
        private bool _disposed = false;
        private List<ManagedMemorySpace> _memory = new List<ManagedMemorySpace>();

        /// <summary>
        /// Managed memory read.
        /// </summary>
        /// <typeparam name="T">Try and make sure your types are serializable.</typeparam>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        /// <param name="name">A name pointer to the memory entry.</param>
        /// <returns>A memory value with type T.</returns>
        public T Read<T>(string pointer, string name)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Memory");
            }

            if (!_memory.Any())
            {
                return default(T);
            }

            var memorySet = _memory.Where(entry => entry.Pointer == pointer).Where(entry => entry.Name == name);
            if (!_memory.Any())
            {
                return default(T);
            }

            var accessedEntry = memorySet.First();
            accessedEntry.Accessed = DateTime.UtcNow.ToBinary();

            return typeof (T) == typeof (ManagedMemorySpace)
                ? (T)Convert.ChangeType(accessedEntry, typeof(ManagedMemorySpace))
                : (T)Convert.ChangeType(accessedEntry.Value, typeof(T));
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
                    ? (T)Convert.ChangeType(entry, typeof(ManagedMemorySpace))
                    : (T)Convert.ChangeType(entry.Value, typeof(T)));

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
        public void Write(string pointer, string name, object value, long created = 0, long updated = 0, long accessed = 0)
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
            spaces.ToList().ForEach(entry => Write(entry.Pointer, entry.Name, entry.Value, entry.Created, entry.Updated, entry.Accessed));
        }

        public void WriteAll(List<ManagedMemorySpace> spaces)
        {
            spaces.ForEach(entry => Write(entry.Pointer, entry.Name, entry.Value, entry.Created, entry.Updated, entry.Accessed));
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

            if (!_memory.Any())
            {
                return;
            }

            var memorySet = _memory.Where(entry => entry.Pointer == pointer).Where(entry => entry.Name == name);
            var managedMemorySpaces = memorySet as ManagedMemorySpace[] ?? memorySet.ToArray();
            if (!managedMemorySpaces.Any())
            {
                return;
            }

            _memory.Remove(managedMemorySpaces.First());
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
                _memory = new List<ManagedMemorySpace>(spaces);
            }
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