namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System.Reflection.PortableExecutable;
    using MZHeader;
    using OptionalHeader;
    using Sections;

    internal class OptionalHeaderInfo : IOptionalHeaderInfo
    {
        public IMZResult MZHeader { get; }
        public ISectionsResult Sections { get; }
        public byte MajorLinkerVersion { get; }
        public byte MinorLinkerVersion { get; }
        public ushort MajorOperatingSystemVersion { get; }
        public ushort MinorOperatingSystemVersion { get; }
        public ushort MajorSubSystemVersion { get; }
        public ushort MinorSubSystemVersion { get; }
        public ushort MajorImageVersion { get; }
        public ushort MinorImageVersion { get; }
        public uint FileAlignment { get; }
        public uint SectionAlignment { get; }
        public Subsystem Subsystem { get; }
        public DllCharacteristics DllCharacteristics { get; }
        public uint StackSizeReserve { get; }
        public uint StackSizeCommit { get; }
        public uint HeapSizeReserve { get; }
        public uint HeapSizeCommit { get; }
        public ulong ImageBase { get; }

        public OptionalHeaderInfo(
            IMZResult mzHeader,
            ISectionsResult sections,
            byte majorLinkerVersion,
            byte minorLinkerVersion,
            ushort majorOperatingSystemVersion,
            ushort minorOperatingSystemVersion,
            ushort majorSubSystemVersion,
            ushort minorSubSystemVersion,
            ushort majorImageVersion,
            ushort minorImageVersion,
            uint fileAlignment,
            uint sectionAlignment,
            Subsystem subsystem,
            DllCharacteristics dllCharacteristics,
            uint stackSizeReserve,
            uint stackSizeCommit,
            uint heapSizeReserve,
            uint heapSizeCommit,
            ulong imageBase
        )
        {
            MZHeader = mzHeader;
            Sections = sections;
            MajorLinkerVersion = majorLinkerVersion;
            MinorLinkerVersion = minorLinkerVersion;
            MajorOperatingSystemVersion = majorOperatingSystemVersion;
            MinorOperatingSystemVersion = minorOperatingSystemVersion;
            MajorSubSystemVersion = majorSubSystemVersion;
            MinorSubSystemVersion = minorSubSystemVersion;
            MajorImageVersion = majorImageVersion;
            MinorImageVersion = minorImageVersion;
            FileAlignment = fileAlignment;
            SectionAlignment = sectionAlignment;
            Subsystem = subsystem;
            DllCharacteristics = dllCharacteristics;
            StackSizeReserve = stackSizeReserve;
            StackSizeCommit = stackSizeCommit;
            HeapSizeReserve = heapSizeReserve;
            HeapSizeCommit = heapSizeCommit;
            ImageBase = imageBase;
        }
    }
}