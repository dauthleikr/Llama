namespace Llama.PE.Tests
{
    using System.IO;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class MZHeaderTests : TestsUsingHeaders
    {
        public MZHeaderTests() : base(File.ReadAllBytes("test.exe")) { }

        [Test]
        public void HasPlausiblePeHeaderOffset()
        {
            Assert.That(MzHeader.NewHeaderRVA, Is.InRange(sizeof(MZHeader), TestFile.Length));
        }

        [Test]
        public void MagicIsMz()
        {
            fixed (byte* ptr = MzHeader.Magic)
                Assert.AreEqual("MZ", Encoding.ASCII.GetString(ptr, 2), "Magic value not matching (headers corrupt?)");
        }
    }
}