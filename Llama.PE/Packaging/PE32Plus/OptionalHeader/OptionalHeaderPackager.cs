namespace Llama.PE.Packaging.PE32Plus.OptionalHeader
{
    using System.IO;
    using System.Linq;
    using BinaryUtils;
    using Structures.Header;

    internal class OptionalHeaderPackager : IPackage<IOptionalHeaderInfo, IOptionalHeaderResult>
    {
        public unsafe IOptionalHeaderResult Package(IOptionalHeaderInfo param)
        {
            var codeSection = param.Sections.SectionHeaders.First(sec => sec.NameString.StartsWith(".text"));
            var initializedDataSection = param.Sections.SectionHeaders.FirstOrDefault(sec => sec.NameString.StartsWith(".data"));
            var uninitializedDataSection = param.Sections.SectionHeaders.FirstOrDefault(sec => sec.NameString.StartsWith(".bss"));
            var optHeaderStandard = new PE32PlusOptionalHeaderStandard
            {
                ExecutableKind = ExecutableKind.PE32Plus,
                BaseOfCodeRVA = codeSection.VirtualAddress,
                EntryPointRVA = param.Sections.EntryPointRVA,
                MajorLinkerVersion = param.MajorLinkerVersion,
                MinorLinkerVersion = param.MinorLinkerVersion,
                SizeOfCode = codeSection.VirtualSize,
                SizeOfInitializedData = initializedDataSection != default ? initializedDataSection.VirtualSize : 0,
                SizeOfUninitializedData = uninitializedDataSection != default ? uninitializedDataSection.VirtualSize : 0
            };
            var optHeaderWinNT = new PE32PlusOptionalHeaderWinNT
            {
                FileAlignment = param.FileAlignment,
                SectionAlignment = param.SectionAlignment,
                Subsystem = param.Subsystem,
                DllCharacteristics = param.DllCharacteristics,
                HeapSizeReserve = param.HeapSizeReserve,
                HeapSizeCommit = param.HeapSizeCommit,
                StackSizeReserve = param.StackSizeReserve,
                StackSizeCommit = param.StackSizeCommit,
                SizeOfImage = param.Sections.SectionHeaders.Max(sec => Round.Up(sec.VirtualAddress + sec.VirtualSize, param.SectionAlignment)),
                ImageBaseOffset = param.ImageBase,
                NumberOfDataDirectoryEntries = 10,
                SizeOfHeaders = (uint)(param.MZHeader.RawData.Length +
                                       sizeof(PEHeader) +
                                       sizeof(PE32PlusOptionalHeader) +
                                       sizeof(SectionHeader) * param.Sections.SectionHeaders.Count),
                MajorOSVersion = param.MajorOperatingSystemVersion,
                MinorOSVersion = param.MinorOperatingSystemVersion,
                MajorSubsystemVersion = param.MajorSubSystemVersion,
                MinorSubsystemVersion = param.MinorSubSystemVersion,
                MajorImageVersion = param.MajorImageVersion,
                MinorImageVersion = param.MinorImageVersion
            };
            var optHeaderDataDictionary = new PE32PlusOptionalHeaderDataDirectories
            {
                IAT = param.Sections.IAT,
                ImportTable = param.Sections.ImportTable
                //todo
            };
            var rawData = new MemoryStream();
            var structWriter = new StreamStructReaderWriter(rawData);
            structWriter.Write(optHeaderStandard);
            structWriter.Write(optHeaderWinNT);
            structWriter.Write(optHeaderDataDictionary);
            return new OptionalHeadersResult(rawData.ToArray());
        }
    }
}