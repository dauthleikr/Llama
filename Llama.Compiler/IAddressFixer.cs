namespace Llama.Compiler
{
    using spit;

    public interface IAddressFixer
    {
        void FixIATEntryOffset(CodeGen generator, string library, string function);
        void FixConstantDataOffset(CodeGen generator, string identifier, byte[] data);
        void FixConstantDataOffset(CodeGen generator, string identifier);
        void FixFunctionOffset(CodeGen generator, string identifier);
        void FixFunctionAddress(CodeGen generator, string identifier);
    }
}