namespace Llama.Compiler
{
    using System;
    using System.Diagnostics;
    using Extensions;
    using spit;
    using Type = Parser.Nodes.Type;

    public partial class ExpressionResult
    {
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

        public Type ValueType { get; private set; }
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
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            if (valueRegister == null)
                throw new ArgumentNullException(nameof(valueRegister));
            if (valueType.IsIntegerRegisterType() == valueRegister.FloatingPoint)
                throw new ArgumentException("Type does not match register");
            if (!valueRegister.FloatingPoint && valueType.SizeOf() != valueRegister.BitSize / 8)
                throw new ArgumentException($"Bad register for type: {valueRegister} {valueType}");

            ValueType = valueType;
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

        public void ChangeTypeUnsafe(Type newTypeUnsafe) => ValueType = newTypeUnsafe;

        public Register GetUnoccupiedVolatile(Type type)
        {
            if (type.IsIntegerRegisterType())
            {
                return Kind switch
                {
                    ResultKind.Value => type.OtherVolatileIntRegister(Value),
                    ResultKind.Pointer => type.OtherVolatileIntRegister(Ptr),
                    ResultKind.Pointer3 => type.OtherVolatileIntRegister(Ptr),
                    ResultKind.Pointer2 => type.OtherVolatileIntRegister(Ptr, StructOffset),
                    ResultKind.Offset => type.OtherVolatileIntRegister(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return Kind switch
            {
                ResultKind.Value => type.OtherVolatileFloatRegister(Value),
                ResultKind.Pointer => type.OtherVolatileFloatRegister(Ptr),
                ResultKind.Pointer3 => type.OtherVolatileFloatRegister(Ptr),
                ResultKind.Pointer2 => type.OtherVolatileFloatRegister(Ptr, StructOffset),
                ResultKind.Offset => type.OtherVolatileFloatRegister(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool IsOccopied(Register register) =>
            Kind switch
            {
                ResultKind.Value => register.IsSameRegister(Value),
                ResultKind.Pointer => register.IsSameRegister(Ptr),
                ResultKind.Pointer3 => register.IsSameRegister(Ptr),
                ResultKind.Pointer2 => register.IsSameRegister(Ptr) || register.IsSameRegister(StructOffset),
                ResultKind.Offset => false,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}