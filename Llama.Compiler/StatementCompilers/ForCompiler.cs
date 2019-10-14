namespace Llama.Compiler.StatementCompilers
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class ForCompiler : ICompileStatements<For>
    {
        public void Compile(
            For statement,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            context.CompileStatement(statement.Variable, codeGen, storageManager, scope);

            var conditionStart = codeGen.StreamPosition;
            var preferredRegisterCondition = new PreferredRegister(Register64.RAX);
            var forConditionResult = context.CompileExpression(statement.Condition, codeGen, storageManager, preferredRegisterCondition, scope);

            forConditionResult.GenerateMoveTo(Register64.RAX, Constants.BoolType, codeGen, addressFixer);
            codeGen.Test(Register32.EAX, Register32.EAX);

            var childContext = context.CreateChildContext();
            var bodyCodeGen = new CodeGen();

            childContext.CompileStatement(statement.Instruction.StatementAsBlock(), bodyCodeGen, storageManager, scope);
            childContext.CompileExpression(statement.Increment, bodyCodeGen, storageManager, new PreferredRegister(Register64.RAX), scope);

            var offsetToStart = conditionStart - codeGen.StreamPosition - bodyCodeGen.GetDataSpan().Length;
            if (offsetToStart >= sbyte.MinValue)
                bodyCodeGen.Jmp((sbyte)offsetToStart);
            else
                bodyCodeGen.Jmp((int)offsetToStart);

            var bodySpan = bodyCodeGen.GetDataSpan();
            if (bodySpan.Length <= sbyte.MaxValue)
                codeGen.Je((sbyte)bodySpan.Length);
            else
                codeGen.Je(bodySpan.Length);

            childContext.AddressLinker.CopyTo(context.AddressLinker, codeGen.StreamPosition);
            codeGen.Write(bodySpan);
        }
    }
}