namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using System;
    using System.Collections.Generic;

    internal class IdataResult : IIdataResult
    {
        public ReadOnlySpan<byte> RawData => _rawData.AsSpan();
        public uint ImportDirectoryTableRVA { get; }
        public uint IAT_RVA { get; }
        public uint IdataRVA { get; }
        private readonly Dictionary<(string lib, string func), uint> _importToRVA;
        private readonly byte[] _rawData;

        public IdataResult(uint iatRVA, uint importDirectoryTableRVA, uint idataRVA, byte[] rawData, Dictionary<(string lib, string func), uint> importToRVA)
        {
            IAT_RVA = iatRVA;
            IdataRVA = idataRVA;
            ImportDirectoryTableRVA = importDirectoryTableRVA;
            _rawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
            _importToRVA = importToRVA ?? throw new ArgumentNullException(nameof(importToRVA));
        }

        public uint GetRVAOfIATEntry(string library, string function)
        {
            if (_importToRVA.TryGetValue((library, function), out var rva))
                return rva;

            throw new ArgumentException($"{nameof(library)}.{function} does not exist");
        }
    }
}