namespace Llama.PE.Builder
{
    using System;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using Packaging.PE32Plus.Sections;

    internal class SectionInfo : ISectionInfo
    {
        public byte[] RawSectionData { get; }
        public ulong Name { get; }
        public uint VirtualSize { get; }
        public SectionCharacteristics Characteristics { get; }

        public SectionInfo(byte[] rawSectionData, string name, uint virtualSize, SectionCharacteristics characteristics)
        {
            RawSectionData = rawSectionData;
            VirtualSize = virtualSize;
            Characteristics = characteristics;

            var sectionName = SectionsPackager.StringToSectionName(name);
            if (string.IsNullOrWhiteSpace(sectionName))
                throw new BadSectionNameException(name);
            Name = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(sectionName));
        }
    }
}