namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using BinaryUtils;
    using Idata;
    using Reloc;
    using Structures.Header;
    using Structures.Sections.Reloc;

    internal class SectionsPackager : IPackage<ISectionsInfo, ISectionsResult>
    {
        private const int SpecialSectionCount = 3;
        private readonly IPackage<IIdataInfo, IIdataResult> _idataPackager;
        private readonly IPackage<IRelocInfo, IRelocResult> _relocPackager;

        public SectionsPackager(IPackage<IIdataInfo, IIdataResult> idataPackager, IPackage<IRelocInfo, IRelocResult> relocPackager)
        {
            _idataPackager = idataPackager;
            _relocPackager = relocPackager;
        }

        public unsafe ISectionsResult Package(ISectionsInfo param)
        {
            var sectionDataStart = param.FileOffsetAtSectionsHeader + sizeof(SectionHeader) * (param.OtherSections.Count() + SpecialSectionCount);
            var peFile = new MemoryStream
            {
                Position = sectionDataStart
            };
            var rva = 0x1000u;
            var idataInfo = new IdataInfo(param.Imports, rva, param.FileAlignment);
            var idataPackage = _idataPackager.Package(idataInfo);
            var idataSection = WriteAndCreateHeader(peFile, idataPackage, param.FileAlignment, param.SectionAlignment, ref rva);
            var textSection = WriteAndCreateHeader(peFile, param.TextSection, param.FileAlignment, param.SectionAlignment, ref rva);
            var sectionHeaders = new List<SectionHeader>
            {
                idataSection,
                textSection
            };

            foreach (var otherSection in param.OtherSections)
                sectionHeaders.Add(WriteAndCreateHeader(peFile, otherSection, param.FileAlignment, param.SectionAlignment, ref rva));

            var relocPackage = _relocPackager.Package(CreateRelocInfo(param, sectionHeaders));
            var relocSection = WriteAndCreateHeader(peFile, relocPackage, param.FileAlignment, param.SectionAlignment, ref rva);
            sectionHeaders.Add(relocSection);

            peFile.Position = param.FileOffsetAtSectionsHeader;
            var structWriter = new StreamStructReaderWriter(peFile);
            structWriter.WriteArray(sectionHeaders.ToArray());
            Debug.Assert(peFile.Position <= sectionDataStart, "Section headers are writing into section data");

            var relocDataDirectory = new ImageDataDirectory
            {
                VirtualAddress = relocSection.VirtualAddress,
                Size = relocSection.SizeOfRawData
            };
            return new SectionsResult(
                peFile.ToArray().Skip((int)param.FileOffsetAtSectionsHeader).ToArray(),
                sectionHeaders,
                param.TextSection.EntryPointOffset + textSection.VirtualAddress,
                idataPackage.IATResolver,
                idataPackage.ImportDirectory,
                idataPackage.IAT,
                default, // todo: debug table
                relocDataDirectory
            );
        }

        private static IRelocInfo CreateRelocInfo(ISectionsInfo info, IReadOnlyList<SectionHeader> otherSections)
        {
            BaseRelocationBlockHeader NewBlockFromRVA(uint rva) =>
                new BaseRelocationBlockHeader
                {
                    PageRVA = (uint)(rva & ~0xFFF)
                };

            var relocationTable = new Dictionary<BaseRelocationBlockHeader, BaseRelocationBlockEntry[]>();
            foreach (var sectionRelocations in info.Relocations64.GroupBy(item => item.section))
            {
                var sectionName = StringToSectionName(sectionRelocations.Key);
                var section = otherSections.FirstOrDefault(sec => sec.NameString == sectionName);
                if (section == default)
                    throw new SectionNotFoundException(sectionName);

                var relocationsInSection = sectionRelocations.Select(item => item.sectionOffset).OrderBy(item => item).ToArray();
                var header = NewBlockFromRVA(relocationsInSection.First());
                var entries = new List<BaseRelocationBlockEntry>();
                foreach (var relocation in relocationsInSection)
                {
                    var page = relocation & ~0xFFF;
                    if (page != header.PageRVA)
                    {
                        header.BlockSize = (uint)(8 + entries.Count * 2);
                        relocationTable[header] = entries.ToArray();
                        entries.Clear();
                        header = NewBlockFromRVA(relocation);
                    }

                    entries.Add(
                        new BaseRelocationBlockEntry
                        {
                            Offset = (int)(relocation - page),
                            Type = BaseRelocationType.ImageRelBasedDir64
                        }
                    );
                }

                header.BlockSize = (uint)(8 + entries.Count * 2);
                relocationTable[header] = entries.ToArray();
            }

            return new RelocInfo(new RelocationTable(relocationTable));
        }

        private static string StringToSectionName(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            if (bytes.Length > 8)
                return null;
            if (bytes.Length == 8)
                return str;
            Array.Resize(ref bytes, 8);
            return Encoding.ASCII.GetString(bytes);
        }

        private SectionHeader WriteAndCreateHeader(Stream target, ISectionInfo sectionInfo, uint fileAlignment, uint sectionAlignment, ref uint va)
        {
            target.Position = Round.Up(target.Position, fileAlignment);
            va = Round.Up(va, sectionAlignment);
            var header = new SectionHeader
            {
                Characteristics = sectionInfo.Characteristics,
                Name = sectionInfo.Name,
                PointerToRawData = (uint)target.Position,
                SizeOfRawData = (uint)Round.Up(sectionInfo.RawSectionData.Length, fileAlignment),
                VirtualSize = (uint)sectionInfo.RawSectionData.Length,
                VirtualAddress = va
            };
            va = Round.Up(va + Math.Max(header.VirtualSize, header.SizeOfRawData), sectionAlignment);
            target.Write(sectionInfo.RawSectionData);
            return header;
        }
    }
}