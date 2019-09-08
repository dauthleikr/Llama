namespace Llama.PE.Builder.PE32Plus
{
    using System;
    using System.IO;

    public interface IExecutableBuilderResult
    {
        Span<byte> GetSectionBuffer(string sectionName);
        long GetIATEntryOffsetToStartOfCode(string library, string function);
        long GetSectionOffsetFromStartOfCode(string sectionName);
        ulong GetSectionRVA(string sectionName);

        void Finish(Stream output);
    }
}