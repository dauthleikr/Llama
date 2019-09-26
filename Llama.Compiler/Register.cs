namespace Llama.Compiler
{
    using spit;

    internal readonly struct Register
    {
        public readonly Register64 IntegerRegister;
        public readonly XmmRegister FloatRegister;
        public readonly bool IsIntegerRegister;

        public Register(Register64 integerRegister, XmmRegister floatRegister, bool isIntegerRegister)
        {
            IntegerRegister = integerRegister;
            FloatRegister = floatRegister;
            IsIntegerRegister = isIntegerRegister;
        }
    }
}