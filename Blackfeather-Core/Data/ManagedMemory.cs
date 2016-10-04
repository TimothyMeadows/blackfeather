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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Blackfeather.Extention;

namespace Blackfeather.Data
{
    /// <summary>
    /// Parallel safe managed memory class
    /// </summary>
    public sealed class ManagedMemory : IDisposable
    {
        public static short MAX_MEMORY_SLOTS = 255;
        public static short PREALLOCATED_MEMORY_SLOTS = 255;

        private bool _disposed;
        private ConcurrentDictionary<short, ManagedMemorySpace> _memory;
        private readonly ConcurrentQueue<short> _memoryAddressSpace;

        public ManagedMemory()
        {
            _memory = new ConcurrentDictionary<short, ManagedMemorySpace>();
            _memoryAddressSpace = new ConcurrentQueue<short>();

            for (short i = 0; i <= MAX_MEMORY_SLOTS - 1; i++)
            {
                _memoryAddressSpace.Enqueue(i);
            }

            for (short i = 0; i <= PREALLOCATED_MEMORY_SLOTS - 1; i++)
            {
                if (!_memory.TryAdd(i, default(ManagedMemorySpace)))
                {
                    throw new Exception("Unable to pre-cache item into memory! Check that your max memory slots are large enough to fit your pre-allocated memory slots.");
                }
            }
        }

        /// <summary>
        /// Parallel safe managed memory read
        /// </summary>
        /// <typeparam name="T">Try and make sure your types are serializable.</typeparam>
        /// <param name="pointer">A reference pointer to the memory entry</param>
        /// <param name="name">A name pointer to the memory entry</param>
        /// <returns>A memory value with type T</returns>
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

            var memorySet = _memory.Where(entry => entry.Value.Pointer == pointer)
                .Where(entry => entry.Value.Name == name);

            if (!memorySet.Any())
            {
                return default(T);
            }

            var accessedEntry = memorySet.Last().Value;
            accessedEntry.Accessed = DateTime.UtcNow.ToBinary();

            return typeof(T) == typeof(ManagedMemorySpace)
                ? (T)Convert.ChangeType(accessedEntry, typeof(ManagedMemorySpace))
                : (T)Convert.ChangeType(accessedEntry.Value, typeof(T));
        }

        /// <summary>
        /// Parallel safe managed memory bulk read.
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

            var memorySet = _memory.Where(entry => entry.Value.Pointer == pointer);
            var memoryFragment = new ConcurrentBag<object>();
            foreach (var entry in memorySet)
            {
                var accessedEntry = entry.Value;
                accessedEntry.Accessed = DateTime.UtcNow.ToBinary();

                memoryFragment.Add(typeof(T) == typeof(ManagedMemorySpace)
                    ? (T)Convert.ChangeType(entry, typeof(ManagedMemorySpace))
                    : (T)Convert.ChangeType(entry.Value, typeof(T)));
            }

            return memoryFragment.ToArray().Cast<T>();
        }

        /// <summary>
        /// Parallel safe managed memory write.
        /// </summary>
        /// <param name="pointer">A reference pointer to the memory entry.</param>
        /// <param name="name">A name pointer to the memory entry.</param>
        /// <param name="value">The object you wish to store in managed memory.</param>
        /// <param name="created">Binary time the object was created. (Optional)</param>
        /// <param name="updated">Binary time the object was updated. (Optional)</param>
        /// <param name="accessed">Binary time the object was last accessed. (Optional)</param>
        /// <param name="addressSpace">Location in managed memory to store the entry</param>
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

            var entry = new ManagedMemorySpace()
            {
                Pointer = pointer,
                Name = name,
                Value = value,
                Accessed = accessedStamp,
                Created = createStamp,
                Updated = updateStamp
            };

            short memoryAddress = 0;
            var spaceAvialiable = _memoryAddressSpace.TryDequeue(out memoryAddress);
            if (!spaceAvialiable)
            {
                throw new OutOfMemoryException("Increase max managed memory size, or, clean up memory usage.");
            }

            if (!_memory.TryAdd(memoryAddress, entry))
            {
                throw new Exception("Unable to pre-cache item into memory! Check that your max memory slots are large enough to fit your pre-allocated memory slots.");
            }
        }

        public void WriteAll(ManagedMemorySpace[] spaces)
        {
            foreach (var entry in spaces)
            {
                Write(entry.Pointer, entry.Name, entry.Value, entry.Created, entry.Updated, entry.Accessed);
            }
        }

        /// <summary>
        /// Parallel safe managed memory delete.
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

            var memorySet = _memory.Where(entry => entry.Value.Pointer == pointer).Where(entry => entry.Value.Name == name);
            if (!memorySet.Any())
            {
                return;
            }

            var memoryEntry = memorySet.Last();
            ManagedMemorySpace removedMemoryEntry;
            var entryRemoved = _memory.TryRemove(memoryEntry.Key, out removedMemoryEntry);
            if (!entryRemoved)
            {
                throw new Exception("Unable to remove entry from memory!");
            }

            _memoryAddressSpace.Enqueue(memoryEntry.Key);
        }


        /// <summary>
        /// Parallel safe managed memory bulk delete.
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

            var memorySet = _memory.Where(entry => entry.Value.Pointer == pointer);
            if (!memorySet.Any())
            {
                return;
            }
            foreach (var entry in memorySet)
            {
                ManagedMemorySpace removedMemoryEntry;
                var entryRemoved = _memory.TryRemove(entry.Key, out removedMemoryEntry);
                if (!entryRemoved)
                {
                    throw new Exception("Unable to remove entry from memory!");
                }

                _memoryAddressSpace.Enqueue(entry.Key);
            }
        }

        public void Clear()
        {
            _memory.Clear();
        }

        public KeyValuePair<short, ManagedMemorySpace>[] Export()
        {
            return _memory.ToArray();
        }

        public KeyValuePair<short, ManagedMemorySpace>[] ExportAll(string pointer)
        {
            var memorySet = _memory.Where(entry => entry.Value.Pointer == pointer);
            if (memorySet.Equals(default(IEnumerable<KeyValuePair<short, ManagedMemorySpace>>)) || !memorySet.Any())
            {
                return null;
            }

            return memorySet.ToArray();
        }

        public void Import(KeyValuePair<short, ManagedMemorySpace>[] memorySpace)
        {
            _memory = new ConcurrentDictionary<short, ManagedMemorySpace>();
            foreach (var memoryEntry in memorySpace)
            {
                _memory.TryAdd(memoryEntry.Key, memoryEntry.Value);
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