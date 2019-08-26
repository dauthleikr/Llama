namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System;
    using System.Reflection.PortableExecutable;

    internal interface ISectionInfo
    {
        byte[] RawSectionData { get; }
        ulong Name { get; }
        uint VirtualSize { get; }
        SectionCharacteristics Characteristics { get; }
    }
}