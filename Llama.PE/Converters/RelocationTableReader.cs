namespace Llama.PE.Converters
{
    using System.Collections.Generic;
    using BinaryUtils;
    using Structures.Sections.Reloc;

    internal class RelocationTableReader : IPEReader<RelocationTable>
    {
        public RelocationTable Read(IStructReader reader, IPE32PlusContext image)
        {
            var relocationTableSize = image.OptionalHeader.DataDirectories.BaseRelocationTable.Size;
            var relocationTableEnd = reader.Offset + relocationTableSize;
            var relocationTable = new Dictionary<BaseRelocationBlockHeader, BaseRelocationBlockEntry[]>();

            while (reader.Offset < relocationTableEnd)
            {
                var header = reader.Read<BaseRelocationBlockHeader>();
                var noEntries = (header.BlockSize - 8) / 2; // 8 bytes header, 2 bytes for each entry
                var entries = new BaseRelocationBlockEntry[noEntries];
                for (var i = 0; i < noEntries; i++)
                    entries[i] = reader.Read<BaseRelocationBlockEntry>();
                relocationTable[header] = entries;
            }

            return new RelocationTable(relocationTable);
        }
    }
}