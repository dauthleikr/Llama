namespace Llama.Compiler
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    public class PreferredRegister
    {
        public static readonly PreferredRegister DefaultVolatile = new PreferredRegister(Register64.RAX, XmmRegister.XMM0);

        public readonly XmmRegister FloatRegister;
        public readonly Register64 IntRegister;

        public PreferredRegister(Register64 intRegister, XmmRegister floatRegister)
        {
            IntRegister = intRegister;
            FloatRegister = floatRegister;
        }

        public PreferredRegister(Register64 intRegister) => IntRegister = intRegister;

        public PreferredRegister(XmmRegister register) => FloatRegister = register;

        public static implicit operator PreferredRegister(Register register)
        {
            if (register.FloatingPoint)
                return new PreferredRegister(register.AsFloat());
            return new PreferredRegister(register.AsR64());
        }

        public Register MakeFor(Type type)
        {
            if (type.IsIntegerRegisterType())
                return Register.IntRegisterFromSize(type.SizeOf(), (int)IntRegister);
            return FloatRegister;
        }
    }
}