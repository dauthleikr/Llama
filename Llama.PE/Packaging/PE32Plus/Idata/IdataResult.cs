namespace Llama.PE.Packaging.PE32Plus.Idata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using Structures.Header;

    internal class IdataResult : IIdataResult, IResolveIATEntries
    {
        public byte[] RawSectionData => RawData;
        public ulong Name => BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".idata\0\0"));
        public uint VirtualSize => (uint)RawData.Length;
        public SectionCharacteristics Characteristics => SectionCharacteristics.MemRead | SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemWrite;
        public ImageDataDirectory ImportDirectory { get; }
        public ImageDataDirectory IAT { get; }
        public uint IdataRVA { get; }
        public IResolveIATEntries IATResolver => this;

        public byte[] RawData { get; }
        private readonly Dictionary<(string lib, string func), uint> _importToRVA;

        public IdataResult(
            uint idataRVA,
            byte[] rawData,
            Dictionary<(string lib, string func), uint> importToRVA,
            ImageDataDirectory importDirectory,
            ImageDataDirectory iat
        )
        {
            IdataRVA = idataRVA;
            RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
            _importToRVA = importToRVA ?? throw new ArgumentNullException(nameof(importToRVA));
            ImportDirectory = importDirectory;
            IAT = iat;
        }

        public uint GetRVAOfIATEntry(string library, string function)
        {
            if (_importToRVA.TryGetValue((library, function), out var rva))
                return rva;

            throw new ArgumentException($"{nameof(library)}.{function} does not exist");
        }
    }
}