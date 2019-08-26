namespace Llama.PE.Packaging.PE32Plus.MZHeader
{
    using System;
    using BinaryUtils;
    using Structures.Header;

    internal class MZResult : IMZResult
    {
        public byte[] RawData { get; }
        public uint NewHeaderOffset { get; }

        public MZResult(MZHeader header)
        {
            RawData = StructConverter.GetBytes(header);
            NewHeaderOffset = header.NewHeaderRVA;
        }
    }
}