namespace Llama.PE.Builder
{
    using System.IO;

    public interface IPE32PlusBuildResult
    {
        byte[] GetSectionBuffer(string sectionName);
        ulong GetIATEntryOffsetToStartOfCode(string library, string function);
        ulong GetSectionOffsetToStartOfCode(string sectionName);

        void Finish(Stream output);
    }
}