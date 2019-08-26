namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using BinaryUtils;
    using Structures.Header;

    internal class SectionsPackager : IPackage<ISectionHeadersInfo, ISectionsResult>
    {
        public unsafe ISectionsResult Package(ISectionHeadersInfo param)
        {
            var sectionDataStart = param.FileOffsetAtSectionsHeader + sizeof(SectionHeader) * (param.OtherSections.Count() + 2);
            var peFile = new MemoryStream { Position = sectionDataStart };
            var va = 0x1000u;
            var textSection = WriteAndCreateHeader(peFile, param.TextSection, param.FileAlignment, param.SectionAlignment, ref va);
            var sectionHeaders = new List<SectionHeader>
            {
                textSection, WriteAndCreateHeader(peFile, param.IdataSection, param.FileAlignment, param.SectionAlignment, ref va)
            };

            foreach (var otherSection in param.OtherSections)
                sectionHeaders.Add(WriteAndCreateHeader(peFile, otherSection, param.FileAlignment, param.SectionAlignment, ref va));

            peFile.Position = param.FileOffsetAtSectionsHeader;
            var structWriter = new StreamStructReaderWriter(peFile);
            structWriter.WriteArray(sectionHeaders.ToArray());
            Debug.Assert(peFile.Position < sectionDataStart, "Section headers are writing into section data");

            return new SectionsResult(
                peFile.ToArray().Skip((int)param.FileOffsetAtSectionsHeader).ToArray(),
                sectionHeaders,
                param.TextSection.EntryPointOffset + textSection.VirtualAddress
            );
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
                SizeOfRawData = (uint)sectionInfo.RawSectionData.Length,
                VirtualSize = (uint)sectionInfo.RawSectionData.Length,
                VirtualAddress = va
            };
            va += header.VirtualSize;
            target.Write(sectionInfo.RawSectionData);
            return header;
        }
    }
}