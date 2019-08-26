namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.PortableExecutable;
    using System.Text;

    internal class IdataResult : IIdataResult
    {
        public ReadOnlySpan<byte> RawSectionData => _rawData.AsSpan();
        public ulong Name => BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".idata\0\0"));
        public uint VirtualSize => (uint)_rawData.Length;
        public SectionCharacteristics Characteristics => SectionCharacteristics.MemRead | SectionCharacteristics.ContainsInitializedData;
        public uint ImportDirectoryTableRVA { get; }
        public uint IAT_RVA { get; }
        public uint IdataRVA { get; }

        public ReadOnlySpan<byte> RawData => RawSectionData;
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