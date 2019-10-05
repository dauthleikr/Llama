namespace Llama.Compiler
{
    using System;
    using System.Linq;
    using spit;

    public readonly struct Register : IEquatable<Register>
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

        public void AssertCanUseInteger()
        {
            if (!CanUseIntegerRegister)
                throw new TypeMismatchException("integer", "floating point");
        }

        public void AssertCanUseFloat()
        {
            if (!CanUseFloatRegister)
                throw new TypeMismatchException("floating point", "integer");
        }

        public override string ToString()
        {
            if (EitherRegisterIsFine)
                return $"{IntegerRegister} or {FloatRegister}";
            if (IsIntegerRegister)
                return IntegerRegister.ToString();
            if (!IsIntegerRegister)
                return FloatRegister.ToString();
            return base.ToString();
        }

        public bool Equals(Register other)
        {
            if (CanUseIntegerRegister && other.CanUseIntegerRegister && IntegerRegister != other.IntegerRegister)
                return false;
            if (CanUseFloatRegister && other.CanUseFloatRegister && FloatRegister != other.FloatRegister)
                return false;
            return true;
        }

        public override bool Equals(object obj) => obj is Register other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)IntegerRegister;
                hashCode = (hashCode * 397) ^ (int)FloatRegister;
                hashCode = (hashCode * 397) ^ IsIntegerRegister.GetHashCode();
                hashCode = (hashCode * 397) ^ EitherRegisterIsFine.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Register left, Register right) => left.Equals(right);

        public static bool operator !=(Register left, Register right) => !left.Equals(right);
    }
}