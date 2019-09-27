namespace Llama.Compiler
{
    using System.Linq;
    using spit;

    public readonly struct Register
    {
        public readonly Register64 IntegerRegister;
        public readonly XmmRegister FloatRegister;
        public readonly bool IsIntegerRegister;
        public readonly bool EitherRegisterIsFine;

        public bool CanUseIntegerRegister => IsIntegerRegister || EitherRegisterIsFine;
        public bool CanUseFloatRegister => !IsIntegerRegister || EitherRegisterIsFine;

        private static readonly Register[] RegistersXmm = Enumerable.Range(0, 16).Select(num => new Register((XmmRegister)num)).ToArray();
        private static readonly Register[] Registers64 = Enumerable.Range(0, 16).Select(num => new Register((Register64)num)).ToArray();

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

        public static implicit operator Register(Register64 reg) => Registers64[(int)reg];
        public static implicit operator Register(XmmRegister reg) => RegistersXmm[(int)reg];

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