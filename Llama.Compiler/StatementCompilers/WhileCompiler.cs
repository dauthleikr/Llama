namespace Llama.Compiler.StatementCompilers
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class WhileCompiler : ICompileStatements<While>
    {
        private static readonly int JmpIntSize = CodeGenExtensions.InstructionLength(gen => gen.Jmp(Constants.DummyOffsetInt));
        private static readonly int JmpSbyteSize = CodeGenExtensions.InstructionLength(gen => gen.Jmp((sbyte)0));

        public void Compile(
            While statement,
            ICompilationContext context
        )
        {
            var codeGen = context.Generator;
            var conditionStart = codeGen.StreamPosition;
            var preferredRegisterCondition = new PreferredRegister(Register64.RAX);
            var whileConditionResult = context.CompileExpression(statement.Condition, preferredRegisterCondition);
            Constants.BoolType.AssertCanAssignImplicitly(whileConditionResult.ValueType);

            whileConditionResult.GenerateMoveTo(Register8.AL, Constants.BoolType, codeGen, context.Linking);

            codeGen.Test(Register8.AL, Register8.AL);

            var childContext = context.CreateChildContext();

            childContext.CompileStatement(statement.Instruction.StatementAsBlock());

            var bodySpan = childContext.Generator.GetBufferSpan();
            var bodyLength = bodySpan.Length + JmpIntSize; // assume far jmp will be generated
            if (bodyLength <= sbyte.MaxValue)
                codeGen.Je((sbyte)bodyLength);
            else
                codeGen.Je(bodyLength);
            var farJmpGuessPos = codeGen.StreamPosition;

            childContext.CopyToContext(context);

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
    }
}