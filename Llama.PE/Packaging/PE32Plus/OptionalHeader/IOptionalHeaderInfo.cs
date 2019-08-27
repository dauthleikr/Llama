namespace Llama.PE.Packaging.PE32Plus.OptionalHeader
{
    using System.Reflection.PortableExecutable;
    using MZHeader;
    using Sections;

    internal interface IOptionalHeaderInfo
    {
        IMZResult MZHeader { get; }
        ISectionsResult Sections { get; }
        byte MajorLinkerVersion { get; }
        byte MinorLinkerVersion { get; }
        ushort MajorOperatingSystemVersion { get; }
        ushort MinorOperatingSystemVersion { get; }
        ushort MajorSubSystemVersion { get; }
        ushort MinorSubSystemVersion { get; }
        ushort MajorImageVersion { get; }
        ushort MinorImageVersion { get; }
        uint FileAlignment { get; }
        uint SectionAlignment { get; }
        Subsystem Subsystem { get; }
        DllCharacteristics DllCharacteristics { get; }
        uint StackSizeReserve { get; }
        uint StackSizeCommit { get; }
        uint HeapSizeReserve { get; }
        uint HeapSizeCommit { get; }
        ulong ImageBase { get; }
    }
}