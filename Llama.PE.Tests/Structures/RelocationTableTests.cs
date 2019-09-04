namespace Llama.PE.Tests.Structures
{
    using System.IO;
    using System.Linq;
    using BinaryUtils;
    using NUnit.Framework;
    using PE.Structures.Sections.Reloc;

    internal class RelocationTableTests : TestsUsingHeaders
    {
        public RelocationTableTests() : base(File.ReadAllBytes(@"ValidPEs\test.exe")) { }

        [Test]
        public void CanParseHeaderAndEntries()
        {
            var relocRVA = OptionalHeader.DataDirectories.BaseRelocationTable.VirtualAddress;
            var relocSection = SectionHeaders.First(sec => sec.NameString.StartsWith(".reloc"));
            var startFileOffset = relocSection.PointerToRawData + relocRVA - relocSection.VirtualAddress;
            var reader = new ArrayStructReaderWriter(TestFile)
            {
                Offset = startFileOffset
            };

            var blockHeader = reader.Read<BaseRelocationBlockHeader>();
            var noEntries = (blockHeader.BlockSize - 8) / 2;
            var firstBlockEntry = reader.Read<BaseRelocationBlockEntry>();
            var secondBlockEntry = reader.Read<BaseRelocationBlockEntry>();

            Assert.AreEqual(0x19000, blockHeader.PageRVA);
            Assert.AreEqual(12, noEntries);
            Assert.AreEqual(0xA110, firstBlockEntry.Entry);
            Assert.AreEqual(0x110, firstBlockEntry.Offset);
            Assert.AreEqual(BaseRelocationType.ImageRelBasedDir64, firstBlockEntry.Type);
            Assert.AreEqual(0xA440, secondBlockEntry.Entry);
        }
    }
}