namespace Llama.PE.Tests
{
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Structures.Header;

    [TestFixture]
    public unsafe class MZHeaderTests : TestsUsingHeaders
    {
        public MZHeaderTests() : base(File.ReadAllBytes("test.exe")) { }

        [Test]
        public void HasPlausiblePeHeaderOffset()
        {
            Assert.That(MZHeader.NewHeaderRVA, Is.InRange(sizeof(MZHeader), TestFile.Length));
        }

        [Test]
        public void MagicIsMz()
        {
            Assert.AreEqual("MZ", MZHeader.MagicString, "Magic value not matching (headers corrupt?)");
        }
    }
}