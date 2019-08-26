namespace Llama.PE.Tests.Structures
{
    using System.IO;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using NUnit.Framework;
    using SectionHeader = PE.Structures.Header.SectionHeader;

    [TestFixture]
    public class SectionHeaderTests : TestsUsingHeaders
    {
        public SectionHeaderTests() : base(File.ReadAllBytes("test.exe")) { }

        private string[] GetSectionNames() => SectionHeaders.Select(item => item.NameString).ToArray();

        [Test]
        public unsafe void SizeIsCorrect()
        {
            Assert.AreEqual(40, sizeof(SectionHeader));
        }

        [Test]
        public void IdataCharacteristicsArePlausible()
        {
            var characteristics = SectionHeaders.First(sec => sec.NameString.StartsWith(".idata")).Characteristics;
            Assert.That(characteristics.HasFlag(SectionCharacteristics.MemRead), ".idata should be readable");
            Assert.That(characteristics.HasFlag(SectionCharacteristics.ContainsInitializedData), ".idata should contain initialized data");
        }

        [Test]
        public void HasDataSection()
        {
            Assert.Contains(".data\0\0\0", GetSectionNames(), "Cannot find .data section");
        }

        [Test]
        public void HasTextSection()
        {
            Assert.Contains(".text\0\0\0", GetSectionNames(), "Cannot find text section (sections corrupt?)");
        }

        [Test]
        public void RelocationsAndLinenumbersAreZero() // because they are either deprecated or should be 0 for images
        {
            foreach (var sectionHeader in SectionHeaders)
            {
                Assert.Zero(sectionHeader.NumberOfLinenumbers);
                Assert.Zero(sectionHeader.PointerToLinenumbers);
                Assert.Zero(sectionHeader.PointerToRelocations);
                Assert.Zero(sectionHeader.NumberOfRelocations);
            }
        }

        [Test]
        public void TextSectionIsReadableExecutableAndHasCode()
        {
            var textSections = SectionHeaders.Where(item => item.NameString == ".text\0\0\0").ToArray();
            if (textSections.Length < 1)
                Assert.Inconclusive("Cannot run this test without .text section");

            foreach (var sectionHeader in textSections)
            {
                Assert.That(sectionHeader.Characteristics.HasFlag(SectionCharacteristics.ContainsCode), ".text does not contain code (section flags)");
                Assert.That(sectionHeader.Characteristics.HasFlag(SectionCharacteristics.MemExecute), ".text is not executable (section flags)");
                Assert.That(sectionHeader.Characteristics.HasFlag(SectionCharacteristics.MemRead), ".text is not readable (section flags)");
            }
        }
    }
}