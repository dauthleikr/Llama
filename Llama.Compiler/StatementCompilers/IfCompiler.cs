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
            codeGen.Test(Register8.AL, Register8.AL);

            var ifBodyContext = context.CreateChildContext();
            var ifBodyCodeGen = new CodeGen();
            var elseBodyContext = context.CreateChildContext();
            var elseBodyCodeGen = new CodeGen();

            if (statement.ElseInstruction != null)
                elseBodyContext.CompileStatement(statement.ElseInstruction.StatementAsBlock(), elseBodyCodeGen, storageManager, scope);

            ifBodyContext.CompileStatement(statement.Instruction.StatementAsBlock(), ifBodyCodeGen, storageManager, scope);

            var elseBodySpan = elseBodyCodeGen.GetBufferSpan();
            if (elseBodySpan.Length > 0)
            {
                if (elseBodySpan.Length <= sbyte.MaxValue)
                    ifBodyCodeGen.Jmp((sbyte)elseBodySpan.Length);
                else
                    ifBodyCodeGen.Jmp(elseBodySpan.Length);
            }

            var ifBodySpan = ifBodyCodeGen.GetBufferSpan();
            if (ifBodySpan.Length <= sbyte.MaxValue)
                codeGen.Je((sbyte)ifBodySpan.Length);
            else
                codeGen.Je(ifBodySpan.Length);

            ifBodyContext.AddressLinker.CopyTo(context.AddressLinker, codeGen.StreamPosition);
            codeGen.Write(ifBodySpan);
            elseBodyContext.AddressLinker.CopyTo(context.AddressLinker, codeGen.StreamPosition);
            codeGen.Write(elseBodySpan);
        }
    }
}