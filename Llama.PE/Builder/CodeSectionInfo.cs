namespace Llama.PE.Builder
{
    using System.Reflection.PortableExecutable;
    using Packaging.PE32Plus.Sections;

    internal class CodeSectionInfo : SectionInfo, ICodeInfo
    {
        public uint EntryPointOffset { get; }

        public CodeSectionInfo(byte[] rawSectionData, uint virtualSize, uint entryPointOffset) : base(
            rawSectionData,
            ".text",
            virtualSize,
            SectionCharacteristics.MemExecute | SectionCharacteristics.MemRead | SectionCharacteristics.ContainsCode
        ) =>
            EntryPointOffset = entryPointOffset;
    }
}