namespace Llama.PE.Structures.Sections.Reloc
{
    using System.Collections.Generic;
    using System.Linq;

    public class RelocationTable
    {
        public uint Size => (uint)Relocations.Sum(item => item.Key.BlockSize);
        public readonly IReadOnlyDictionary<BaseRelocationBlockHeader, BaseRelocationBlockEntry[]> Relocations;

        public RelocationTable(IReadOnlyDictionary<BaseRelocationBlockHeader, BaseRelocationBlockEntry[]> relocations) => Relocations = relocations;
    }
}