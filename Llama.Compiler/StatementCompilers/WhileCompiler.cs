namespace Llama.Compiler.StatementCompilers
{
    using System;
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class WhileCompiler : ICompileStatements<While>
    {
        private static readonly int JmpIntSize = InstructionLength(gen => gen.Jmp(Constants.DummyOffsetInt));
        private static readonly int JmpSbyteSize = InstructionLength(gen => gen.Jmp((sbyte)0));

        public void Compile(
            While statement,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var conditionStart = codeGen.StreamPosition;
            var preferredRegisterCondition = new PreferredRegister(Register64.RAX);
            var whileConditionResult = context.CompileExpression(statement.Condition, codeGen, storageManager, preferredRegisterCondition, scope);
            Constants.BoolType.AssertCanAssign(whileConditionResult.ValueType);

            whileConditionResult.GenerateMoveTo(Register8.AL, Constants.BoolType, codeGen, addressFixer);

            codeGen.TestAlWith0();

            var childContext = context.CreateChildContext();
            var bodyCodeGen = new CodeGen();

            childContext.CompileStatement(statement.Instruction.StatementAsBlock(), bodyCodeGen, storageManager, scope);

            var bodySpan = bodyCodeGen.GetBufferSpan();
            var bodyLength = bodySpan.Length + JmpIntSize; // assume far jmp will be generated
            if (bodyLength <= sbyte.MaxValue)
                codeGen.Jne((sbyte)bodyLength);
            else
                codeGen.Jne(bodyLength);
            var farJmpGuessPos = codeGen.StreamPosition;

            childContext.AddressLinker.CopyTo(context.AddressLinker, codeGen.StreamPosition);
            codeGen.Write(bodySpan);

            var offsetToStart = conditionStart - codeGen.StreamPosition;
            if (offsetToStart >= sbyte.MinValue)
            {
                codeGen.Jmp((sbyte)(offsetToStart - JmpSbyteSize));

                // near jmp has been generated, but we assume a far jmp above
                // if this is a near jmp, the other has to be too
                // so it's safe to say, we just need to edit the byte.
                // The new value has to fit, because the jmp becomes even nearer.
                codeGen.GetBufferSpan().Slice((int)farJmpGuessPos - 1, 1)[0] -= (byte)(JmpIntSize - JmpSbyteSize);
            }
            else
                codeGen.Jmp((int)(offsetToStart - JmpIntSize));
        }

        private static int InstructionLength(Action<CodeGen> genAction)
        {
            var gen = new CodeGen();
            genAction(gen);
            return (int)gen.StreamPosition;
        }
    }
}