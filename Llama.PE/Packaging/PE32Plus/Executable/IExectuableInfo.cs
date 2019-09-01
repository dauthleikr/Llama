namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System.Collections.Generic;
    using System.Reflection.PortableExecutable;
    using Sections;

    internal interface IExectuableInfo
    {
        byte MajorLinkerVersion { get; }
        byte MinorLinkerVersion { get; }
        ushort MajorOperatingSystemVersion { get; }
        ushort MinorOperatingSystemVersion { get; }
        ushort MajorSubSystemVersion { get; }
        ushort MinorSubSystemVersion { get; }
        ushort MajorImageVersion { get; }
        ushort MinorImageVersion { get; }
        Subsystem Subsystem { get; }
        uint StackSizeReserve { get; }
        uint StackSizeCommit { get; }
        uint HeapSizeReserve { get; }
        uint HeapSizeCommit { get; }
        ulong ImageBase { get; }
        uint FileAlignment { get; }
        uint SectionAlignment { get; }
        bool MayRunFromRemoveableDrive { get; }
        bool MayRunFromNetwork { get; }
        IEnumerable<(string library, string function)> Imports { get; }
        IEnumerable<(string section, uint offset)> Relocations64 { get; }
        ICodeInfo TextSection { get; }
        IEnumerable<ISectionInfo> OtherSections { get; }
    }
}