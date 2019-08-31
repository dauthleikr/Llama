namespace Llama.PE.Packaging.PE32Plus.Reloc
{
    using Structures.Sections.Reloc;

    internal interface IRelocInfo
    {
        RelocationTable Relocations { get; }
    }
}