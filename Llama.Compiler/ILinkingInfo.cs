namespace Llama.Compiler
{
    public interface ILinkingInfo
    {
        void FixIATEntryOffset(long position, string library, string function);
        void FixConstantDataOffset(long position, byte[] data);
        void FixConstantDataAddress(long position, byte[] data);
        void FixDataOffset(long position, string identifier, int length = 8);
        void FixFunctionOffset(long position, string identifier);
        void FixFunctionAddress(long position, string identifier);
        void FixFunctionEpilogueOffset(long position, string identifier);
        void ResolveFunctionFixes(string identifier, long position);
        void ResolveFunctionEpilogueFixes(string identifier, long position);
        void Insert(long position, int count);
        void CopyTo(ILinkingInfo other, long offset);
    }
}