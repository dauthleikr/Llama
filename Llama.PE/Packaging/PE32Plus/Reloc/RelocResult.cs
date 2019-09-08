namespace Llama.PE.Packaging.PE32Plus.Reloc
{
    using System;
    using System.Reflection.PortableExecutable;
    using System.Text;

    internal class RelocResult : IRelocResult
    {
        public byte[] RawData { get; }
        public byte[] RawSectionData { get; }
        public ulong Name => BitConverter.ToUInt64(Encoding.ASCII.GetBytes(".reloc\0\0"));
        public uint VirtualSize { get; }
        public uint RelocationDirectorySize { get; }

        public SectionCharacteristics Characteristics =>
            SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemDiscardable | SectionCharacteristics.MemRead;

        public RelocResult(byte[] rawData, uint virtualSize, uint relocationDirectorySize)
        {
            RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
            RawSectionData = rawData;
            VirtualSize = virtualSize;
            RelocationDirectorySize = relocationDirectorySize;
        }
    }
}