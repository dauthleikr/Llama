namespace Llama.PE.Packaging.PE32Plus.PEHeader
{
    using System;
    using System.Reflection.PortableExecutable;

    internal interface IPEInfo
    {
        Characteristics Characteristics { get; }
        Machine Architecture { get; }
        DateTime TimeStamp { get; }
        ushort NumberOfSections { get; }
    }
}