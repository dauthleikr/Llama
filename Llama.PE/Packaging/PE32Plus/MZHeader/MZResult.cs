namespace Llama.PE.Packaging.PE32Plus.MZHeader
{
    using System;
    using BinaryUtils;
    using Structures.Header;

    internal class MZResult : IMZResult
    {
        public ReadOnlySpan<byte> RawData => _bytes.AsSpan();
        public uint NewHeaderOffset { get; }
        private readonly byte[] _bytes;

        public MZResult(MZHeader header)
        {
            _bytes = StructConverter.GetBytes(header);
            NewHeaderOffset = header.NewHeaderRVA;
        }
    }
}