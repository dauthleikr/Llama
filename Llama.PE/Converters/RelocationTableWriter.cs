namespace Llama.PE.Converters
{
    using BinaryUtils;
    using Structures.Sections.Reloc;

    internal class RelocationTableWriter : IPEWriter<RelocationTable>
    {
        public void Write(IStructWriter writer, RelocationTable item)
        {
            foreach (var (header, entries) in item.Relocations)
            {
                writer.Write(header);
                writer.WriteArray(entries);
            }
        }
    }
}