namespace Llama.PE.Idata
{
    using BinaryUtils;

    internal class IdataSectionMapper : IPEMapper<IdataSection>
    {
        private readonly IPEMapper<HintNameEntry> _hintNameMapper;

        public IdataSectionMapper(IPEMapper<HintNameEntry> hintNameMapper) => _hintNameMapper = hintNameMapper;

        public void Write(IdataSection representation, IStructWriter writer)
        {
            writer.WriteArray(representation.ImportDirectoryTable);
            writer.Write(default(ImportDirectoryEntry)); // null entry signals end of table
            foreach (var importLookupTable in representation.ImportLookupTables)
            {
                writer.WriteArray(importLookupTable);
                writer.Write(default(ImportLookupEntryPE32Plus)); // null entry signals end of table
            }

            writer.WriteArray(representation.HintOrNameTable, _hintNameMapper);
        }

        public IdataSection Read(IStructReader reader)
        {
            var importDirectoryTable = reader.ReadUntilNull<ImportDirectoryEntry>().ToArray();

            var importCount = importDirectoryTable.Length;
            var importLookupTable = new ImportLookupEntryPE32Plus[importCount][];
            for (var i = 0; i < importCount; i++)
                importLookupTable[i] = reader.ReadUntilNull<ImportLookupEntryPE32Plus>().ToArray();

            var hintOrNameTable = reader.ReadArray(_hintNameMapper, importCount);

            return new IdataSection(importDirectoryTable, importLookupTable, hintOrNameTable);
        }
    }
}