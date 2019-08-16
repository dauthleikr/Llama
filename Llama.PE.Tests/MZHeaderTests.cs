namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class MZHeaderTests : TestsUsingHeaders
    {
        [Test]
        public void MagicIsMZ()
        {
            fixed (byte* ptr = MZHeader.Magic)
                Assert.AreEqual("MZ", Encoding.ASCII.GetString(ptr, 2), "Magic value not matching (headers corrupt?)");
        }

        [Test]
        public void HasPlausiblePEHeaderOffset()
        {
            Assert.That(MZHeader.NewHeaderRVA, Is.InRange(sizeof(MZHeader), TestFile.Length));
        }

        public MZHeaderTests() : base(File.ReadAllBytes("test.exe")) { }
    }
}