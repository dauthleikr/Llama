namespace Llama.PE.Idata
{
    public class ImportDirectory
    {
        #region R#er no reorder
        public readonly ImportDirectoryEntry[] ImportDirectoryTable;
        public readonly ImportLookupEntryPE32Plus[][] ImportLookupTables;
        public readonly HintNameEntry[] HintOrNameTable;

        public ImportDirectory(ImportDirectoryEntry[] importDirectoryTable, ImportLookupEntryPE32Plus[][] importLookupTables, HintNameEntry[] hintOrNameTable)
        {
            ImportDirectoryTable = importDirectoryTable;
            ImportLookupTables = importLookupTables;
            HintOrNameTable = hintOrNameTable;
        }
        #endregion
    }
}