namespace Llama.PE.Packaging.PE32Plus.Sections
{
    using System;
    using System.Collections.Generic;
    using Structures.Header;

    internal class SectionsResult : ISectionsResult
    {
        public ReadOnlySpan<byte> RawData => _rawData;
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }
        public uint EntryPointRVA { get; }
        private readonly byte[] _rawData;

        public SectionsResult(byte[] rawData, IReadOnlyList<SectionHeader> headers, uint entryPointRVA)
        {
            _rawData = rawData;
            SectionHeaders = headers;
            EntryPointRVA = entryPointRVA;
        }
    }
}