namespace Llama.Compiler.StatementCompilers
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class IfCompiler : ICompileStatements<If>
    {
        public void Compile(
            If statement,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var preferredRegisterCondition = new PreferredRegister(Register64.RAX);
            var ifConditionResult = context.CompileExpression(statement.Condition, codeGen, storageManager, preferredRegisterCondition, scope);
            ifConditionResult.GenerateMoveTo(Register64.RAX, Constants.BoolType, codeGen, addressFixer);
            codeGen.Test(Register32.EAX, Register32.EAX);

            var childContext = context.CreateChildContext();
            var bodyCodeGen = new CodeGen();
            childContext.CompileStatement(statement.Instruction.StatementAsBlock(), bodyCodeGen, storageManager, scope);
            var bodySpan = bodyCodeGen.GetDataSpan();

            if (bodySpan.Length <= sbyte.MaxValue)
                codeGen.Je((sbyte)bodySpan.Length);
            else
                codeGen.Je(bodySpan.Length);

            childContext.AddressLinker.CopyTo(context.AddressLinker, codeGen.StreamPosition);
            codeGen.Write(bodySpan);

            if (statement.ElseInstruction != null)
                context.CompileStatement(statement.ElseInstruction.StatementAsBlock(), codeGen, storageManager, scope);
        }
    }
}