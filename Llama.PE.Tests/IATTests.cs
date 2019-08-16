namespace Llama.PE.Tests
{
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class IatTests : TestsUsingHeaders
    {
        public IatTests() : base(File.ReadAllBytes("test.exe")) { }


        [Test]
        public void Abc()
        {
            var ddIaTrva = OptionalHeader.DataDirectories.IAT.StartRVA;
            var idataSectionHeader = SectionHeaders.First(item => item.NameString.StartsWith(".idata"));
            var idataRva = idataSectionHeader.PointerToRawData;
            var idataSize = idataSectionHeader.VirtualSize;
        }
    }
}