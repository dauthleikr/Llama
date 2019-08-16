namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public  class PEHeaderTests : TestsUsingHeaders
    {
        [Test]
        public unsafe void HeaderHasExpectedSize()
        {
            Assert.AreEqual(sizeof(PEHeader), 4 + 20, "Header size does not match standard");
        }

        [Test]
        public unsafe void MagicIsPE()
        {
            fixed (byte* ptr = PEHeader.Magic)
                Assert.AreEqual("PE\0\0", Encoding.ASCII.GetString(ptr, 4), "Magic value not matching (headers corrupt?)");
        }

        public PEHeaderTests() : base(File.ReadAllBytes("test.exe")) { }
    }
}