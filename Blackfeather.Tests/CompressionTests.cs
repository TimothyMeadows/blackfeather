using System;
using System.Text;
using Blackfeather.Extention;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blackfeather35.Tests
{
    [TestClass]
    public class CompressionTests
    {
        [TestMethod]
        public void CompressTest()
        {
            var compressed = "caw".Compress();

            Assert.IsNotNull(compressed);
            Assert.AreEqual("H4sIAAAAAAAEAO29B2AcSZYlJi9tynt/SvVK1+B0oQiAYBMk2JBAEOzBiM3mkuwdaUcjKasqgcplVmVdZhZAzO2dvPfee++999577733ujudTif33/8/XGZkAWz2zkrayZ4hgKrIHz9+fB8/IqbZ1f8DEhJXBwMAAAA=", compressed);
        }

        [TestMethod]
        public void DecompressTest()
        {
            var decompressed = "H4sIAAAAAAAEAO29B2AcSZYlJi9tynt/SvVK1+B0oQiAYBMk2JBAEOzBiM3mkuwdaUcjKasqgcplVmVdZhZAzO2dvPfee++999577733ujudTif33/8/XGZkAWz2zkrayZ4hgKrIHz9+fB8/IqbZ1f8DEhJXBwMAAAA=".Decompress();

            Assert.IsNotNull(decompressed);
            Assert.AreEqual("caw", decompressed);
        }
    }
}
