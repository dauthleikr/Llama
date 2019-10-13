namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public class PreferredRegister
    {
        private readonly XmmRegister _floatRegister;
        private readonly Register64 _intRegister;

        public PreferredRegister(Register64 intRegister, XmmRegister floatRegister)
        {
            _intRegister = intRegister;
            _floatRegister = floatRegister;
        }

        public PreferredRegister(Register64 intRegister) => _intRegister = intRegister;

        public PreferredRegister(XmmRegister register) => _floatRegister = register;

        public static implicit operator PreferredRegister(Register register)
        {
            if (register.FloatingPoint)
                return new PreferredRegister(register.AsFloat());
            return new PreferredRegister(register.AsR64());
        }

        public Register MakeFor(Type type)
        {
            if (type.IsIntegerRegisterType())
                Register.IntRegisterFromSize(type.SizeOf(), (int)_intRegister);
            return _floatRegister;
        }
    }
}