namespace Llama.PE.Builder.PE32Plus
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.PortableExecutable;
    using Packaging.PE32Plus.Executable;
    using Packaging.PE32Plus.Sections;

    /// <summary>
    ///     Contains essential data for packaging
    /// </summary>
    internal class ExecutableInfo : IExecutableInfo
    {
        public byte MajorLinkerVersion => (byte)Assembly.GetExecutingAssembly().GetName().Version.Major;
        public byte MinorLinkerVersion => (byte)Assembly.GetExecutingAssembly().GetName().Version.Minor;
        public ushort MajorOperatingSystemVersion => 6;
        public ushort MinorOperatingSystemVersion => 1;
        public ushort MajorSubSystemVersion => 6;
        public ushort MinorSubSystemVersion => 1;
        public ushort MajorImageVersion { get; }
        public ushort MinorImageVersion { get; }
        public Subsystem Subsystem { get; }
        public uint StackSizeReserve { get; }
        public uint StackSizeCommit { get; }
        public uint HeapSizeReserve => 0x100_000;
        public uint HeapSizeCommit => 0x1_000;
        public ulong ImageBase { get; }
        public uint FileAlignment { get; }
        public uint SectionAlignment { get; }
        public bool MayRunFromRemoveableDrive { get; }
        public bool MayRunFromNetwork { get; }
        public IEnumerable<(string library, string function)> Imports { get; }
        public IEnumerable<(string section, uint offset)> Relocations64 { get; }
        public ICodeInfo TextSection { get; }
        public IEnumerable<ISectionInfo> OtherSections { get; }

        public ExecutableInfo(
            ushort majorImageVersion,
            ushort minorImageVersion,
            Subsystem subsystem,
            uint stackSizeReserve,
            uint stackSizeCommit,
            ulong imageBase,
            uint fileAlignment,
            uint sectionAlignment,
            bool mayRunFromRemoveableDrive,
            bool mayRunFromNetwork,
            IEnumerable<(string library, string function)> imports,
            IEnumerable<(string section, uint offset)> relocations64,
            ICodeInfo textSection,
            IEnumerable<ISectionInfo> otherSections
        )
        {
            MajorImageVersion = majorImageVersion;
            MinorImageVersion = minorImageVersion;
            Subsystem = subsystem;
            StackSizeReserve = stackSizeReserve;
            StackSizeCommit = stackSizeCommit;
            ImageBase = imageBase;
            FileAlignment = fileAlignment;
            SectionAlignment = sectionAlignment;
            MayRunFromRemoveableDrive = mayRunFromRemoveableDrive;
            MayRunFromNetwork = mayRunFromNetwork;
            Imports = imports;
            Relocations64 = relocations64;
            TextSection = textSection;
            OtherSections = otherSections;
        }
    }
}