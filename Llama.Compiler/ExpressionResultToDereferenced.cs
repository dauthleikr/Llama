namespace Llama.Compiler
{
    using System;
    using spit;

    public partial class ExpressionResult
    {
        public delegate void GenericToDref2Action(
            Register64 ptr,
            Register source,
            Register64 structOffset,
            byte offsetMul = 1,
            int offsetFlat = 0,
            Segment segment = Segment.DS
        );

        public delegate void GenericToDref3Action(
            Register64 ptr,
            Register source,
            byte offsetMul = 1,
            int offsetFlat = 0,
            Segment segment = Segment.DS
        );

        public delegate void GenericToDref4Action(int sourceOffset, Register source, Segment segment = Segment.DS);

        public delegate void GenericToDrefAction(Register64 ptr, Register source, int offsetFlat = 0, Segment segment = Segment.DS);

        public void DereferenceFromRegister(
           CodeGen codeGen,
           Register source,
           IAddressFixer fixer,
           GenericBrotherAction brotherAction,
           GenericToDrefAction drefAction,
           GenericToDref2Action dref2Action,
           GenericToDref3Action dref3Action,
           GenericToDref4Action dref4Action
       )
        {
            switch (Kind)
            {
                case ResultKind.Value:
                    DereferenceFromRegister(brotherAction, source);
                    return;
                case ResultKind.Pointer:
                    DereferenceFromRegister(drefAction, source);
                    return;
                case ResultKind.Pointer2:
                    DereferenceFromRegister(dref2Action, source);
                    return;
                case ResultKind.Pointer3:
                    DereferenceFromRegister(dref3Action, source);
                    return;
                case ResultKind.Offset:
                    DereferenceFromRegister(codeGen, source, dref4Action, fixer);
                    return;
                default:
                    throw new NotImplementedException($"{nameof(ExpressionResult)}: {nameof(GenerateMoveTo)} has not implemented kind: {Kind}");
            }
        }

        private void DereferenceFromRegister(CodeGen codeGen, Register source, GenericToDref4Action action, IAddressFixer fixer)
        {
            action(Offset, source);
            OffsetFixup(fixer, codeGen);
        }

        private void DereferenceFromRegister(GenericToDref3Action action, Register source)
        {
            action(Ptr, source, OffsetMul, Offset, Ptr == Register64.RSP || Ptr == Register64.RBP ? Segment.SS : Segment.DS);
        }

        private void DereferenceFromRegister(GenericToDref2Action action, Register source)
        {
            action(Ptr, source, StructOffset, OffsetMul, Offset, Ptr == Register64.RSP || Ptr == Register64.RBP ? Segment.SS : Segment.DS);
        }

        private void DereferenceFromRegister(GenericToDrefAction action, Register source)
        {
            action(Ptr, source, Offset, Ptr == Register64.RSP || Ptr == Register64.RBP ? Segment.SS : Segment.DS);
        }

        private void DereferenceFromRegister(GenericBrotherAction action, Register source)
        {
            if (source.BitSize != Value.BitSize)
                throw new ArgumentException($"Action only allowed with registers of identical size; {source} does not match {Value}");

            action(source, Value);
        }

        private void GenerateAssignOffset(Register register, CodeGen codeGen, IAddressFixer fixer)
        {
            if (!register.FloatingPoint)
            {
                // mov [12345678], register
                codeGen.MovToDereferenced4(Constants.DummyOffsetInt, register);
            }
            else
            {
                // movq [12345678], register
                codeGen.MovqToDereferenced4(Constants.DummyOffsetInt, register.AsFloat());
            }

            OffsetFixup(fixer, codeGen);
        }

        private void GenerateAssignPointer3(Register register, CodeGen codeGen)
        {
            if (!register.FloatingPoint)
            {
                // mov [Ptr * OffsetMul + Offset], register
                codeGen.MovToDereferenced3(Ptr, register, OffsetMul, Offset, Segment);
            }
            else
            {
                // movq [Ptr * OffsetMul + Offset], register
                codeGen.MovqToDereferenced3(Ptr, register.AsFloat(), OffsetMul, Offset, Segment);
            }
        }

        private void GenerateAssignPointer2(Register register, CodeGen codeGen)
        {
            if (!register.FloatingPoint)
            {
                // mov [Ptr + StructOffset * OffsetMul + Offset], register
                codeGen.MovToDereferenced2(Ptr, register, StructOffset, OffsetMul, Offset, Segment);
            }
            else
            {
                // movq [Ptr + StructOffset * OffsetMul + Offset], register
                codeGen.MovqToDereferenced2(Ptr, register.AsFloat(), StructOffset, OffsetMul, Offset, Segment);
            }
        }

        private void GenerateAssignPointer(Register register, CodeGen codeGen)
        {
            if (!register.FloatingPoint)
            {
                // mov [Ptr + Offset], register
                codeGen.MovToDereferenced(Ptr, register, Offset, Segment);
            }
            else
            {
                // movq [Ptr + Offset], register
                codeGen.MovqToDereferenced(Ptr, register.AsFloat(), Offset, Segment);
            }
        }

        private void GenerateAssignValue(Register register, CodeGen codeGen)
        {
            if (register == Value)
                return;

            if (!register.FloatingPoint && !Value.FloatingPoint)
                codeGen.Mov(Value.AsR64(), register.AsR64());
            else if (register.FloatingPoint && Value.FloatingPoint)
                codeGen.Movsd(Value.AsFloat(), register.AsFloat());
            else if (!register.FloatingPoint && Value.FloatingPoint)
                codeGen.Movq(Value.AsFloat(), register.AsR64());
            else
                codeGen.Movq(Value.AsR64(), register.AsFloat());
        }

        public void GenerateAssign(Register source, CodeGen codeGen, IAddressFixer fixer)
        {
            switch (Kind)
            {
                case ResultKind.Value:
                    GenerateAssignValue(source, codeGen);
                    return;
                case ResultKind.Pointer:
                    GenerateAssignPointer(source, codeGen);
                    return;
                case ResultKind.Pointer2:
                    GenerateAssignPointer2(source, codeGen);
                    return;
                case ResultKind.Pointer3:
                    GenerateAssignPointer3(source, codeGen);
                    return;
                case ResultKind.Offset:
                    GenerateAssignOffset(source, codeGen, fixer);
                    return;
                default:
                    throw new NotImplementedException($"{nameof(ExpressionResult)}: {nameof(GenerateAssign)} has not implemented kind: {Kind}");
            }
        }
    }
}