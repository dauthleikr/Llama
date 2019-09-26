namespace Llama.Compiler
{
    using spit;

    public readonly struct Register
    {
        public readonly Register64 IntegerRegister;
        public readonly XmmRegister FloatRegister;
        public readonly bool IsIntegerRegister;
        public readonly bool EitherRegisterIsFine;

        public Register(Register64 integerRegister)
        {
            IntegerRegister = integerRegister;
            FloatRegister = XmmRegister.XMM0;
            IsIntegerRegister = true;
            EitherRegisterIsFine = false;
        }

        public Register(XmmRegister floatRegister)
        {
            FloatRegister = floatRegister;
            IntegerRegister = Register64.RAX;
            IsIntegerRegister = false;
            EitherRegisterIsFine = false;
        }

        public Register(Register64 integerRegister, XmmRegister floatRegister)
        {
            IntegerRegister = integerRegister;
            FloatRegister = floatRegister;
            IsIntegerRegister = false;
            EitherRegisterIsFine = true;
        }

        public void AssertIsInteger()
        {
            if (!EitherRegisterIsFine && !IsIntegerRegister)
                throw new TypeMismatchException("integer", "floating point");
        }

        public void AssertIsFloat()
        {
            if (!EitherRegisterIsFine && IsIntegerRegister)
                throw new TypeMismatchException("floating point", "integer");
        }
    }
}