namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System.Collections.Generic;
    using Idata;
    using Structures.Header;

    internal class SectionsResult : ISectionsResult
    {
        public byte[] RawData { get; }
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }
        public uint EntryPointRVA { get; }
        public ImageDataDirectory IAT { get; }
        public ImageDataDirectory ImportTable { get; }
        public IResolveIATEntries IATResolver { get; }

        public SectionsResult(byte[] rawData, IReadOnlyList<SectionHeader> headers, uint entryPointRVA, IResolveIATEntries iatResolver, ImageDataDirectory importTable, ImageDataDirectory iat)
        {
            RawData = rawData;
            SectionHeaders = headers;
            EntryPointRVA = entryPointRVA;
            IATResolver = iatResolver;
            ImportTable = importTable;
            IAT = iat;
        }
    }
}