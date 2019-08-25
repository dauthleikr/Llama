namespace Llama.PE.Converters
{
    using System.Collections.Generic;
    using BinaryUtils;
    using Structures.Sections.Idata;

    public class ImportDirectoryReader : IPEReader<ImportDirectory>
    {
        private readonly IPEReader<HintNameEntry> _hintNameReader;

        public ImportDirectoryReader(IPEReader<HintNameEntry> hintNameMapper) => _hintNameReader = hintNameMapper;

        public ImportDirectory Read(IStructReader reader, IPE32PlusContext image)
        {
            var importDirectoryTable = reader.ReadUntilNull<ImportDirectoryEntry>().ToArray();
            var importCount = importDirectoryTable.Length;
            var importLookupTable = new ImportLookupEntryPE32Plus[importCount][];
            var hintOrNameTable = new List<HintNameEntry>();

            for (var i = 0; i < importDirectoryTable.Length; i++)
            {
                reader.Offset = image.GetFileOffset(importDirectoryTable[i].ImportLookupTableRVA);
                importLookupTable[i] = reader.ReadUntilNull<ImportLookupEntryPE32Plus>().ToArray();
                foreach (var lookupEntry in importLookupTable[i])
                {
                    reader.Offset = image.GetFileOffset(lookupEntry.HintNameTableRVA);
                    hintOrNameTable.Add(_hintNameReader.Read(reader, image));
                }
            }

            return new ImportDirectory(importDirectoryTable, importLookupTable, hintOrNameTable.ToArray());
        }
    }
}