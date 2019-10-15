namespace Llama.Compiler
{
    using System;
    using Extensions;
    using spit;
    using RegisterExtensions = Extensions.RegisterExtensions;
    using Type = Parser.Nodes.Type;

    public partial class ExpressionResult
    {
        private delegate Register GetRegisterFunction(params Register[] not);

        public enum ResultKind
        {
            Invalid,
            Value, // RAX
            Pointer, // [RAX + c]
            Pointer2, // [RAX + RCX * cPtr + c]
            Pointer3, // [RAX * cPtr + c]
            Offset // [12345678]
        }

        public delegate void GenericBrotherAction(Register target, Register source);

        public Type ValueType { get; }
        public Register Value { get; }
        public bool IsReference => Kind != ResultKind.Value;

        public readonly ResultKind Kind;
        public readonly int Offset;
        public readonly Action<IAddressFixer, CodeGen> OffsetFixup;
        public readonly byte OffsetMul = 1;
        public readonly Register64 Ptr;
        public readonly Segment Segment;
        public readonly Register64 StructOffset;

        public ExpressionResult(Type valueType, Action<IAddressFixer, CodeGen> offsetFixup)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            OffsetFixup = offsetFixup ?? throw new ArgumentNullException(nameof(offsetFixup));
            Kind = ResultKind.Offset;
        }

        public ExpressionResult(Type valueType, Register valueRegister)
        {
            if (valueType.IsIntegerRegisterType() == valueRegister.FloatingPoint)
                throw new ArgumentException("Type does not match register");

            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Value = valueRegister;
            Kind = ResultKind.Value;
        }

        public ExpressionResult(Type valueType, Register64 ptr, int offset, Segment segment = Segment.DS)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Ptr = ptr;
            Offset = offset;
            Segment = segment;
            Kind = ResultKind.Pointer;
        }

        public ExpressionResult(
            Type valueType,
            Register64 ptr,
            Register64 structOffset,
            byte offsetMul,
            int offsetFlat = 0,
            Segment segment = Segment.DS
        )
        {
            if (offsetMul != 1 && offsetMul != 2 && offsetMul != 4 && offsetMul != 8)
                throw new ArgumentOutOfRangeException("The multiplicative offset may only be 1, 2, 4 or 8");

            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Ptr = ptr;
            StructOffset = structOffset;
            OffsetMul = offsetMul;
            Offset = offsetFlat;
            Segment = segment;
            Kind = ResultKind.Pointer2;
        }

        public ExpressionResult(Type valueType, Register64 ptr, byte ptrMul, int offsetFlat = 0, Segment segment = Segment.DS)
        {
            if (ptrMul != 1 && ptrMul != 2 && ptrMul != 4 && ptrMul != 8)
                throw new ArgumentOutOfRangeException("The multiplicative offset may only be 1, 2, 4 or 8");

            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Ptr = ptr;
            OffsetMul = ptrMul;
            Offset = offsetFlat;
            Segment = segment;
            Kind = ResultKind.Pointer3;
        }

        public Register GetUnoccupiedVolatile(bool integerType)
        {
            var getRegisterAction = integerType ?
                (GetRegisterFunction)RegisterExtensions.OtherVolatileIntRegister :
                RegisterExtensions.OtherVolatileFloatRegister;

            return Kind switch
            {
                ResultKind.Value    => getRegisterAction(Value),
                ResultKind.Pointer  => getRegisterAction(Ptr),
                ResultKind.Pointer3 => getRegisterAction(Ptr),
                ResultKind.Pointer2 => getRegisterAction(Ptr, StructOffset),
                ResultKind.Offset   => getRegisterAction(),
                _                   => throw new ArgumentOutOfRangeException()
            };
        }

        public bool IsOccopied(Register register) =>
            Kind switch
            {
                ResultKind.Value    => register.IsSameRegister(Value),
                ResultKind.Pointer  => register.IsSameRegister(Ptr),
                ResultKind.Pointer3 => register.IsSameRegister(Ptr),
                ResultKind.Pointer2 => register.IsSameRegister(Ptr) || register.IsSameRegister(StructOffset),
                ResultKind.Offset   => false,
                _                   => throw new ArgumentOutOfRangeException()
            };
    }
}