namespace Llama.PE.Tests
{
    using System.IO;
    using System.Text;
    using Header;
    using NUnit.Framework;

    [TestFixture]
    public class PEHeaderTests : TestsUsingHeaders
    {
        public PEHeaderTests() : base(File.ReadAllBytes("test.exe")) { }

        [Test]
        public unsafe void HeaderHasExpectedSize()
        {
            Assert.AreEqual(sizeof(PEHeader), 4 + 20, "Header size does not match standard");
        }

        [Test]
        public void MagicIsPe()
        {
            Assert.AreEqual("PE\0\0", PEHeader.MagicString, "Magic value not matching (headers corrupt?)");
        }
    }
}