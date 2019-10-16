namespace Llama.Compiler
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    public class Storage
    {
        private const Register64 TempIntRegister = Register64.RAX;
        private const XmmRegister TempFloatRegister = XmmRegister.XMM0;
        public readonly bool IsIntegerType;
        public readonly bool IsRegister;
        public readonly Register Register;
        public readonly int StackOffset;

        public Storage(Register register)
        {
            IsRegister = true;
            Register = register;
            IsIntegerType = !register.FloatingPoint;
        }

        public Storage(int stackOffset, bool isIntegerType)
        {
            IsRegister = false;
            StackOffset = stackOffset;
            IsIntegerType = isIntegerType;
        }

        public void Store(ExpressionResult expressionResult, CodeGen codeGen, IAddressFixer addressFixer)
        {
            if (!IsRegister)
            {
                var tempRegister = IsIntegerType ? (Register)TempIntRegister : TempFloatRegister;
                expressionResult.GenerateMoveTo(tempRegister, expressionResult.ValueType, codeGen, addressFixer);

                if (IsIntegerType)
                    codeGen.MovToDereferenced(Register64.RSP, tempRegister, StackOffset, Segment.SS);
                else
                    codeGen.MovqToDereferenced(Register64.RSP, tempRegister, StackOffset, Segment.SS);
            }
            else
            {
                expressionResult.GenerateMoveTo(
                    expressionResult.ValueType.MakeRegisterWithCorrectSize(Register),
                    expressionResult.ValueType,
                    codeGen,
                    addressFixer
                );
            }
        }

        public ExpressionResult AsExpressionResult(Type typeUnsafe)
        {
            if (IsRegister)
                return new ExpressionResult(typeUnsafe, typeUnsafe.MakeRegisterWithCorrectSize(Register));
            return new ExpressionResult(typeUnsafe, Register64.RSP, StackOffset, Segment.SS);
        }
    }
}