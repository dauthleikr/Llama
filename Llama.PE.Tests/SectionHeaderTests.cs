namespace Llama.PE.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using NUnit.Framework;
    using PEHeader = PE.PEHeader;
    using SectionHeader = PE.SectionHeader;

    [TestFixture]
    public unsafe class SectionHeaderTests
    {
        [SetUp]
        public void SetUp()
        {
            var exeBytes = File.ReadAllBytes("test.exe");
            if (sizeof(MZHeader) > exeBytes.Length)
                throw new Exception("Bad test exe file");

            fixed (byte* exePtr = exeBytes)
            {
                var mzheader = *(MZHeader*)exePtr;
                if (sizeof(PEHeader) + mzheader.NewHeaderRVA > exeBytes.Length)
                    throw new Exception("Bad test exe file (cannot fit PE header)");
                var header = *(PEHeader*)(exePtr + mzheader.NewHeaderRVA);
                if (header.OptionalHeaderSize + mzheader.NewHeaderRVA + sizeof(PEHeader) + sizeof(SectionHeader) * header.NumberOfSections > exeBytes.Length)
                    throw new Exception("Bad test exe file (cannot fit section headers)");
                _headers = new SectionHeader[header.NumberOfSections];
                for (var i = 0; i < _headers.Length; i++)
                    _headers[i] = *(SectionHeader*)(exePtr + mzheader.NewHeaderRVA + header.OptionalHeaderSize + sizeof(PEHeader) + i * sizeof(SectionHeader));
            }

            _names = _headers.Select(item => Encoding.ASCII.GetString(item.Name, 8)).ToArray();
        }

        private SectionHeader[] _headers;
        private string[] _names;

        [Test]
        public void HasTextSection()
        {
            Assert.Contains(".text\0\0\0", _names, "Cannot find text section (sections corrupt?)");
        }

        [Test]
        public void TextSectionIsReadableExecutableAndHasCode()
        {
            var textIndex = Array.IndexOf(_names, ".text\0\0\0");
            if (textIndex < 0)
                Assert.Inconclusive("Cannot run this test without .text section");

            Assert.That(_headers[textIndex].Characteristics.HasFlag(SectionCharacteristics.ContainsCode), ".text does not contain code (section flags)");
            Assert.That(_headers[textIndex].Characteristics.HasFlag(SectionCharacteristics.MemExecute), ".text is not executable (section flags)");
            Assert.That(_headers[textIndex].Characteristics.HasFlag(SectionCharacteristics.MemRead), ".text is not readable (section flags)");
        }

        [Test]
        public void RelocationsAndLinenumbersAreZero() // because they are either deprecated or should be 0 for images
        {
            foreach (var sectionHeader in _headers)
            {
                Assert.Zero(sectionHeader.NumberOfLinenumbers);
                Assert.Zero(sectionHeader.PointerToLinenumbers);
                Assert.Zero(sectionHeader.PointerToRelocations);
                Assert.Zero(sectionHeader.NumberOfRelocations);
            }
        }

        [Test]
        public void HasDataSection()
        {
            Assert.Contains(".data\0\0\0", _names, "Cannot find .data section");
        }
    }
}