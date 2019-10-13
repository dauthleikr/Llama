namespace Llama.Compiler
{
    using System;
    using spit;

    public partial class ExpressionResult
    {
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