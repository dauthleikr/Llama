namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using BinaryUtils;
    using Idata;
    using Structures.Header;

    internal class SectionsPackager : IPackage<ISectionHeadersInfo, ISectionsResult>
    {
        private readonly IPackage<IIdataInfo, IIdataResult> _idataPackager;

        public SectionsPackager(IPackage<IIdataInfo, IIdataResult> idataPackager) => _idataPackager = idataPackager;

        public unsafe ISectionsResult Package(ISectionHeadersInfo param)
        {
            var sectionDataStart = param.FileOffsetAtSectionsHeader + sizeof(SectionHeader) * (param.OtherSections.Count() + 2);
            var peFile = new MemoryStream { Position = sectionDataStart };
            var rva = 0x1000u;
            var idataInfo = new IdataInfo(param.Imports, rva, param.FileAlignment);
            var idataPackage = _idataPackager.Package(idataInfo);
            var idataSection = WriteAndCreateHeader(peFile, idataPackage, param.FileAlignment, param.SectionAlignment, ref rva);
            var textSection = WriteAndCreateHeader(peFile, param.TextSection, param.FileAlignment, param.SectionAlignment, ref rva);
            var sectionHeaders = new List<SectionHeader> { idataSection, textSection };

            foreach (var otherSection in param.OtherSections)
                sectionHeaders.Add(WriteAndCreateHeader(peFile, otherSection, param.FileAlignment, param.SectionAlignment, ref rva));

            peFile.Position = param.FileOffsetAtSectionsHeader;
            var structWriter = new StreamStructReaderWriter(peFile);
            structWriter.WriteArray(sectionHeaders.ToArray());
            Debug.Assert(peFile.Position <= sectionDataStart, "Section headers are writing into section data");

            return new SectionsResult(
                peFile.ToArray().Skip((int)param.FileOffsetAtSectionsHeader).ToArray(),
                sectionHeaders,
                param.TextSection.EntryPointOffset + textSection.VirtualAddress,
                idataPackage.IATResolver,
                idataPackage.ImportDirectory,
                idataPackage.IAT
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
            va = Round.Up(va + header.VirtualSize, sectionAlignment);
            target.Write(sectionInfo.RawSectionData);
            return header;
        }
    }
}