namespace Llama.PE.Tests.Packaging
{
    using System;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using BinaryUtils;
    using Moq;
    using NUnit.Framework;
    using PE.Packaging;
    using PE.Packaging.PE32Plus.Idata;
    using PE.Packaging.PE32Plus.Sections;
    using SectionHeader = PE.Structures.Header.SectionHeader;

    [TestFixture]
    internal class SectionTests
    {
        [SetUp]
        public void SetUp()
        {
            var codeInfo = new Mock<ICodeInfo>();
            codeInfo.Setup(item => item.EntryPointOffset).Returns(10);
            codeInfo.Setup(item => item.RawSectionData).Returns(Enumerable.Repeat(CodeSectionContentByte, CodeSectionSize).ToArray());
            codeInfo.Setup(item => item.VirtualSize).Returns(100);
            codeInfo.Setup(item => item.Name).Returns(BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".text\0\0\0")));
            codeInfo.Setup(item => item.Characteristics)
                .Returns(SectionCharacteristics.MemExecute | SectionCharacteristics.MemRead | SectionCharacteristics.ContainsCode);

            var sectionHeadersInfo = new Mock<ISectionHeadersInfo>();
            sectionHeadersInfo.Setup(info => info.SectionAlignment).Returns(0x1000);
            sectionHeadersInfo.Setup(info => info.FileAlignment).Returns(0x200);
            sectionHeadersInfo.Setup(info => info.TextSection).Returns(codeInfo.Object);
            sectionHeadersInfo.Setup(info => info.FileOffsetAtSectionsHeader).Returns(FileOffsetAtSectionsHeader);
            sectionHeadersInfo.Setup(info => info.OtherSections).Returns(Enumerable.Empty<ISectionInfo>());
            sectionHeadersInfo.Setup(info => info.Imports).Returns(Enumerable.Empty<(string, string)>());

            var idataResult = new Mock<IIdataResult>();
            idataResult.Setup(item => item.Characteristics).Returns(SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemRead);
            idataResult.Setup(item => item.Name).Returns(BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".idata\0\0")));
            idataResult.Setup(item => item.VirtualSize).Returns(100);
            idataResult.Setup(item => item.RawData).Returns(Enumerable.Repeat((byte)1, 99).ToArray());
            idataResult.Setup(item => item.RawSectionData).Returns(Enumerable.Repeat((byte)1, 99).ToArray());

            var idataPackager = new Mock<IPackage<IIdataInfo, IIdataResult>>();
            idataPackager.Setup(item => item.Package(It.IsAny<IIdataInfo>())).Returns(idataResult.Object);

            var packager = new SectionsPackager(idataPackager.Object);
            _packaged = packager.Package(sectionHeadersInfo.Object);
        }

        private ISectionsResult _packaged;

        private const int FileOffsetAtSectionsHeader = 123;
        private const byte CodeSectionContentByte = 0xC3;
        private const byte CodeSectionSize = 0xC3;

        [Test]
        public void CanResolveCodeBytes()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData);
            var sections = reader.ReadArray<SectionHeader>(2);
            var codeSection = sections.First(sec => sec.NameString.StartsWith(".text"));
            var codeRaw = _packaged.RawData.AsSpan()
                .Slice((int)(codeSection.PointerToRawData - FileOffsetAtSectionsHeader), (int)codeSection.SizeOfRawData)
                .ToArray();

            Assert.AreEqual(CodeSectionSize, codeSection.SizeOfRawData);
            Assert.That(codeRaw, Is.All.EqualTo(CodeSectionContentByte));
        }

        [Test]
        public void CanResolveSectionNames()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData);
            var sections = reader.ReadArray<SectionHeader>(2);
            var sectionNames = sections.Select(sec => sec.NameString).ToArray();

            Assert.Contains(".text\0\0\0", sectionNames);
            Assert.Contains(".idata\0\0", sectionNames);
        }

        [Test]
        public void SectionsDoNotOverlap()
        {
            var reader = new ArrayStructReaderWriter(_packaged.RawData);
            var sections = reader.ReadArray<SectionHeader>(2);
            var sectionVirtualRanges = sections.Select(sec => (start: sec.VirtualAddress, end: sec.VirtualAddress + sec.VirtualSize)).ToArray();
            var sectionOffsetRanges = sections.Select(sec => (start: sec.PointerToRawData, end: sec.PointerToRawData + sec.SizeOfRawData)).ToArray();

            foreach (var section1 in sectionVirtualRanges)
            {
                foreach (var section2 in sectionVirtualRanges)
                {
                    if (section1 == section2)
                        continue;

                    Assert.False(
                        section1.start < section2.end && section1.end > section2.start,
                        $"Overlap: {section1.start}-{section1.end} overlaps with {section2.start}-{section2.end}"
                    );
                }
            }

            foreach (var section1 in sectionOffsetRanges)
            {
                foreach (var section2 in sectionOffsetRanges)
                {
                    if (section1 == section2)
                        continue;

                    Assert.False(
                        section1.start < section2.end && section1.end > section2.start,
                        $"Overlap (Offsets): {section1.start}-{section1.end} overlaps with {section2.start}-{section2.end}"
                    );
                }
            }
        }
    }
}