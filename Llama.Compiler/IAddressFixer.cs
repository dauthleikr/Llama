namespace Llama.Compiler
{
    using spit;

    public interface IAddressFixer
    {
        void FixIATEntryOffset(CodeGen generator, string library, string function);
        void FixConstantDataOffset(CodeGen generator, byte[] data);
        void FixConstantDataAddress(CodeGen generator, byte[] data);
        void FixDataOffset(CodeGen generator, string identifier, int length = 8);
        void FixFunctionOffset(CodeGen generator, string identifier);
        void FixFunctionAddress(CodeGen generator, string identifier);
    }
}