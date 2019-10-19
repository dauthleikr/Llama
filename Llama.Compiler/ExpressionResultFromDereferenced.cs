namespace Llama.Compiler
{
    using System;
    using Extensions;
    using spit;
    using Type = Parser.Nodes.Type;

    public partial class ExpressionResult
    {
        public delegate void GenericFromDref2Action(
            Register target,
            Register64 ptr,
            Register64 structOffset,
            byte offsetMul = 1,
            int offsetFlat = 0,
            Segment segment = Segment.DS
        );

        public delegate void GenericFromDref3Action(
            Register target,
            Register64 ptr,
            byte offsetMul = 1,
            int offsetFlat = 0,
            Segment segment = Segment.DS
        );

        public delegate void GenericFromDref4Action(Register target, int sourceOffset, Segment segment = Segment.DS);

        public delegate void GenericFromDrefAction(Register target, Register64 ptr, int offsetFlat = 0, Segment segment = Segment.DS);

        public void GenerateMoveTo(Register target, Type targetType, CodeGen codeGen, IAddressFixer fixer)
        {
            if (!targetType.CanAssign(ValueType))
                throw new TypeMismatchException(targetType.ToString(), ValueType.ToString());
            if (target.BitSize / 8 != targetType.SizeOf())
                throw new ArgumentException($"Value of type {targetType} can not be moved to register {target}");

            if (targetType == ValueType)
            {
                GenerateMoveTo(target, codeGen, fixer);
                return;
            }

            if (targetType.SizeOf() < ValueType.SizeOf())
                throw new ArgumentException($"Cannot narrow type from {ValueType} to {targetType} without a cast", nameof(targetType));

            var signed = targetType.IsSignedInteger();
            switch (Kind)
            {
                case ResultKind.Value:
                    GenerateUpgradeToValue(target, codeGen, signed);
                    return;
                case ResultKind.Pointer:
                    GenerateUpgradeToPointer(target, codeGen, signed);
                    return;
                case ResultKind.Pointer2:
                    GenerateUpgradeToPointer2(target, codeGen, signed);
                    return;
                case ResultKind.Pointer3:
                    GenerateUpgradeToPointer3(target, codeGen, signed);
                    return;
                case ResultKind.Offset:
                    GenerateUpgradeToOffset(target, codeGen, fixer, signed);
                    return;
                default:
                    throw new NotImplementedException($"{nameof(ExpressionResult)}: {nameof(GenerateMoveTo)} has not implemented kind: {Kind}");
            }
        }

        //todo: bug: rework this redundant shit
        private void GenerateUpgradeToOffset(Register target, CodeGen codeGen, IAddressFixer fixer, bool signed)
        {
            if (!target.FloatingPoint && ValueType.IsIntegerRegisterType())
            {
                var bitize = Math.Min(target.BitSize, ValueType.SizeOf() * 8);
                if (bitize == 32)
                {
                    if (signed)
                        codeGen.MovsxdFromDereferenced4(target.AsR64(), Offset, Segment);
                    else
                        codeGen.MovFromDereferenced4(target, Offset, Segment);
                }
                else
                {
                    if (signed)
                        codeGen.MovsxFromDereferenced4(target, ValueType.SizeOf() == 1, Offset);
                    else
                        codeGen.MovzxFromDereferenced4(target, ValueType.SizeOf() == 1, Offset);
                }
            }
            else if (target.FloatingPoint && Value.FloatingPoint)
                codeGen.CvtSs2SdFromDereferenced4(target, Offset, Segment);
            else
                throw new NotImplementedException();

            OffsetFixup(fixer, codeGen);
        }

        private void GenerateUpgradeToPointer3(Register target, CodeGen codeGen, bool signed)
        {
            if (!target.FloatingPoint && ValueType.IsIntegerRegisterType())
            {
                var bitize = Math.Min(target.BitSize, ValueType.SizeOf() * 8);
                if (bitize == 32)
                {
                    if (signed)
                        codeGen.MovsxdFromDereferenced3(target.AsR64(), Ptr, OffsetMul, Offset, Segment);
                    else
                        codeGen.MovFromDereferenced3(target, Ptr, OffsetMul, Offset, Segment);
                }
                else
                {
                    if (signed)
                        codeGen.MovsxFromDereferenced3(target, Ptr, OffsetMul, ValueType.SizeOf() == 1, Offset);
                    else
                        codeGen.MovzxFromDereferenced3(target, Ptr, OffsetMul, ValueType.SizeOf() == 1, Offset);
                }
            }
            else if (target.FloatingPoint && !ValueType.IsIntegerRegisterType())
                codeGen.CvtSs2SdFromDereferenced3(target, Ptr, OffsetMul, Offset, Segment);
            else
                throw new NotImplementedException();
        }

        private void GenerateUpgradeToPointer2(Register target, CodeGen codeGen, bool signed)
        {
            if (!target.FloatingPoint && ValueType.IsIntegerRegisterType())
            {
                var bitize = Math.Min(target.BitSize, ValueType.SizeOf() * 8);
                if (bitize == 32)
                {
                    if (signed)
                        codeGen.MovsxdFromDereferenced2(target.AsR64(), Ptr, StructOffset, OffsetMul, Offset, Segment);
                    else
                        codeGen.MovFromDereferenced2(target, Ptr, StructOffset, OffsetMul, Offset, Segment);
                }
                else
                {
                    if (signed)
                        codeGen.MovsxFromDereferenced2(target, Ptr, StructOffset, ValueType.SizeOf() == 1, OffsetMul, Offset);
                    else
                        codeGen.MovzxFromDereferenced2(target, Ptr, StructOffset, ValueType.SizeOf() == 1, OffsetMul, Offset);
                }
            }
            else if (target.FloatingPoint && Value.FloatingPoint)
                codeGen.CvtSs2SdFromDereferenced2(target, Ptr, StructOffset, OffsetMul, Offset, Segment);
            else
                throw new NotImplementedException();
        }

        private void GenerateUpgradeToPointer(Register target, CodeGen codeGen, bool signed)
        {
            if (!target.FloatingPoint && ValueType.IsIntegerRegisterType())
            {
                var bitize = Math.Min(target.BitSize, ValueType.SizeOf() * 8);
                if (bitize == 32)
                {
                    if (signed)
                        codeGen.MovsxdFromDereferenced(target.AsR64(), Ptr, Offset, Segment);
                    else
                        codeGen.MovFromDereferenced(target, Ptr, Offset, Segment);
                }
                else
                {
                    if (signed)
                        codeGen.MovsxFromDereferenced(target, Ptr, ValueType.SizeOf() == 1, Offset);
                    else
                        codeGen.MovzxFromDereferenced(target, Ptr, ValueType.SizeOf() == 1, Offset);
                }
            }
            else if (target.FloatingPoint && Value.FloatingPoint)
                codeGen.CvtSs2SdFromDereferenced(target, Ptr, Offset, Segment);
            else
                throw new NotImplementedException();
        }

        private void GenerateUpgradeToValue(Register target, CodeGen codeGen, bool signed)
        {
            if (!target.FloatingPoint && ValueType.IsIntegerRegisterType())
            {
                var bitize = Value.BitSize;
                switch (bitize)
                {
                    case 32:
                        if (signed)
                            codeGen.Movsxd(target.AsR64(), Value.AsR32());
                        else
                            codeGen.Mov(target.AsR32(), Value.AsR32());
                        break;
                    case 16:
                        if (signed)
                            codeGen.Movsx(target.AsR64(), Value.AsR16());
                        else
                            codeGen.Movzx(target.AsR64(), Value.AsR16());
                        break;
                    case 8:
                        codeGen.Movzx(target.AsR64(), Value.AsR8());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (target.FloatingPoint && Value.FloatingPoint)
                codeGen.CvtSs2Sd(target.AsFloat(), Value.AsFloat());
            else
                throw new NotImplementedException();
        }

        public void GenerateMoveTo(Register target, CodeGen codeGen, IAddressFixer fixer)
        {
            switch (Kind)
            {
                case ResultKind.Value:
                    GenerateMoveToValue(target, codeGen);
                    return;
                case ResultKind.Pointer:
                    GenerateMoveToPointer(target, codeGen);
                    return;
                case ResultKind.Pointer2:
                    GenerateMoveToPointer2(target, codeGen);
                    return;
                case ResultKind.Pointer3:
                    GenerateMoveToPointer3(target, codeGen);
                    return;
                case ResultKind.Offset:
                    GenerateMoveToOffset(target, codeGen, fixer);
                    return;
                default:
                    throw new NotImplementedException($"{nameof(ExpressionResult)}: {nameof(GenerateMoveTo)} has not implemented kind: {Kind}");
            }
        }

        public void DereferenceToRegister(
            Register target,
            CodeGen codeGen,
            IAddressFixer fixer,
            GenericBrotherAction brotherAction,
            GenericFromDrefAction drefAction,
            GenericFromDref2Action dref2Action,
            GenericFromDref3Action dref3Action,
            GenericFromDref4Action dref4Action
        )
        {
            switch (Kind)
            {
                case ResultKind.Value:
                    DereferenceToRegister(target, brotherAction);
                    return;
                case ResultKind.Pointer:
                    DereferenceToRegister(target, drefAction);
                    return;
                case ResultKind.Pointer2:
                    DereferenceToRegister(target, dref2Action);
                    return;
                case ResultKind.Pointer3:
                    DereferenceToRegister(target, dref3Action);
                    return;
                case ResultKind.Offset:
                    DereferenceToRegister(target, codeGen, dref4Action, fixer);
                    return;
                default:
                    throw new NotImplementedException($"{nameof(ExpressionResult)}: {nameof(GenerateMoveTo)} has not implemented kind: {Kind}");
            }
        }

        private void DereferenceToRegister(Register target, CodeGen codeGen, GenericFromDref4Action action, IAddressFixer fixer)
        {
            action(target, Offset);
            OffsetFixup(fixer, codeGen);
        }

        private void DereferenceToRegister(Register target, GenericFromDref3Action action)
        {
            action(target, Ptr, OffsetMul, Offset, Ptr == Register64.RSP || Ptr == Register64.RBP ? Segment.SS : Segment.DS);
        }

        private void DereferenceToRegister(Register target, GenericFromDref2Action action)
        {
            action(target, Ptr, StructOffset, OffsetMul, Offset, Ptr == Register64.RSP || Ptr == Register64.RBP ? Segment.SS : Segment.DS);
        }

        private void DereferenceToRegister(Register target, GenericFromDrefAction action)
        {
            action(target, Ptr, Offset, Ptr == Register64.RSP || Ptr == Register64.RBP ? Segment.SS : Segment.DS);
        }

        private void DereferenceToRegister(Register target, GenericBrotherAction action)
        {
            if (target.BitSize != Value.BitSize)
                throw new ArgumentException($"Action only allowed with registers of identical size; {target} does not match {Value}");

            action(target, Value);
        }

        private void GenerateMoveToOffset(Register register, CodeGen codeGen, IAddressFixer fixer)
        {
            if (!register.FloatingPoint)
            {
                // mov target, [12345678]
                codeGen.MovFromDereferenced4(register, Constants.DummyOffsetInt);
            }
            else
            {
                // movq target, [12345678]
                codeGen.MovqFromDereferenced4(register.AsFloat(), Constants.DummyOffsetInt);
            }

            OffsetFixup(fixer, codeGen);
        }

        private void GenerateMoveToPointer3(Register register, CodeGen codeGen)
        {
            if (!register.FloatingPoint)
            {
                // mov target, [Ptr * OffsetMul + Offset]
                codeGen.MovFromDereferenced3(register, Ptr, OffsetMul, Offset, Segment);
            }
            else
            {
                // movq target, [Ptr * OffsetMul + Offset]
                codeGen.MovqFromDereferenced3(register.AsFloat(), Ptr, OffsetMul, Offset, Segment);
            }
        }

        private void GenerateMoveToPointer2(Register register, CodeGen codeGen)
        {
            if (!register.FloatingPoint)
            {
                // mov target, [Ptr + StructOffset * OffsetMul + Offset]
                codeGen.MovFromDereferenced2(register, Ptr, StructOffset, OffsetMul, Offset, Segment);
            }
            else
            {
                // movq target, [Ptr + StructOffset * OffsetMul + Offset]
                codeGen.MovqFromDereferenced2(register.AsFloat(), Ptr, StructOffset, OffsetMul, Offset, Segment);
            }
        }

        private void GenerateMoveToPointer(Register register, CodeGen codeGen)
        {
            if (!register.FloatingPoint)
            {
                // mov target, [Ptr + Offset]
                codeGen.MovFromDereferenced(register, Ptr, Offset, Segment);
            }
            else
            {
                // movq target, [Ptr + Offset]
                codeGen.MovqFromDereferenced(register.AsFloat(), Ptr, Offset, Segment);
            }
        }

        private void GenerateMoveToValue(Register register, CodeGen codeGen)
        {
            if (register.IsSameRegister(Value))
                return;

            if (!register.FloatingPoint && !Value.FloatingPoint)
                codeGen.Mov(register.AsR64(), Value.AsR64());
            else if (register.FloatingPoint && Value.FloatingPoint)
                codeGen.Movsd(register.AsFloat(), Value.AsFloat());
            else if (!register.FloatingPoint && Value.FloatingPoint)
                codeGen.Movq(register.AsR64(), Value.AsFloat());
            else
                codeGen.Movq(register.AsFloat(), Value.AsR64());
        }
    }
}