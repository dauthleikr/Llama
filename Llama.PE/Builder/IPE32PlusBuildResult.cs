namespace Llama.PE.Builder
{
    using System;
    using System.IO;

    public interface IPE32PlusBuildResult
    {
        Span<byte> GetSectionBuffer(string sectionName);
        ulong GetIATEntryOffsetToStartOfCode(string library, string function);
        ulong GetSectionOffsetToStartOfCode(string sectionName);

        void Finish(Stream output);
    }
}