using System;
using Blackfeather.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blackfeather_35.Tests
{
    [TestClass]
    public class ManagedMemoryTests
    {
        [TestMethod]
        public void ReadAndWrite()
        {
            using (var memory = new ManagedMemory())
            {
                memory.Write("Test", "TestKey", "TestValue!");
                var read = memory.Read<string>("Test", "TestKey");

                Assert.IsNotNull(read);
                Assert.AreEqual(read, "TestValue!");
            }
        }

        [TestMethod]
        public void ReadManagedMemorySpace()
        {
            using (var memory = new ManagedMemory())
            {
                memory.Write("Test", "TestKey", "TestValue!");
                var read = memory.Read<ManagedMemorySpace>("Test", "TestKey");

                Assert.IsNotNull(read);
                Assert.AreEqual(read.Value, "TestValue!");
            }
        }

        [TestMethod]
        public void ReadAndWriteAll()
        {
            using (var memory = new ManagedMemory())
            {
                memory.Write("Test", "TestKey1", "TestValue1!");
                memory.Write("Test", "TestKey2", "TestValue2!");
                var read = memory.ReadAll<string>("Test");

                Assert.IsNotNull(read);
                Assert.AreEqual(read[0], "TestValue1!");
                Assert.AreEqual(read[1], "TestValue2!");
            }
        }

        [TestMethod]
        public void ReadAllManagedMemorySpace()
        {
            using (var memory = new ManagedMemory())
            {
                memory.Write("Test", "TestKey1", "TestValue1!");
                memory.Write("Test", "TestKey2", "TestValue2!");
                var read = memory.ReadAll<ManagedMemorySpace>("Test");

                Assert.IsNotNull(read);
                Assert.AreEqual(read[0].Value, "TestValue1!");
                Assert.AreEqual(read[1].Value, "TestValue2!");
            }
        }

        [TestMethod]
        public void Delete()
        {
            using (var memory = new ManagedMemory())
            {
                memory.Write("Test", "TestKey", "TestValue!");
                var read = memory.Read<string>("Test", "TestKey");

                Assert.IsNotNull(read);
                Assert.AreEqual(read, "TestValue!");

                memory.Delete("Test", "TestKey");
                read = memory.Read<string>("Test", "TestKey");

                Assert.IsNull(read);
            }
        }
    }
}
