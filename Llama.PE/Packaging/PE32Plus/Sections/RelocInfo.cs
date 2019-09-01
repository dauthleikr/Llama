namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using Reloc;
    using Structures.Sections.Reloc;

    internal class RelocInfo : IRelocInfo
    {
        public RelocationTable Relocations { get; }

        public RelocInfo(RelocationTable relocations) => Relocations = relocations;
    }
}