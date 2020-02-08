namespace Llama.Compiler.StatementCompilers
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class IfCompiler : ICompileStatements<If>
    {
        public void Compile(
            If statement,
            ICompilationContext context
        )
        {
            var preferredRegisterCondition = new PreferredRegister(Register64.RAX);
            var ifConditionResult = context.CompileExpression(statement.Condition, preferredRegisterCondition);
            ifConditionResult.GenerateMoveTo(preferredRegisterCondition.MakeFor(Constants.BoolType), context.Generator, context.Linking);
            context.Generator.Test(Register8.AL, Register8.AL);

            var ifBody = context.CreateChildContext();
            var elseBody = context.CreateChildContext();

            if (statement.ElseInstruction != null)
                elseBody.CompileStatement(statement.ElseInstruction.StatementAsBlock());

            ifBody.CompileStatement(statement.Instruction.StatementAsBlock());

            var elseBodySpan = elseBody.Generator.GetBufferSpan();
            if (elseBodySpan.Length > 0)
            {
                if (elseBodySpan.Length <= sbyte.MaxValue)
                    ifBody.Generator.Jmp((sbyte)elseBodySpan.Length);
                else
                    ifBody.Generator.Jmp(elseBodySpan.Length);
            }

            var ifBodySpan = ifBody.Generator.GetBufferSpan();
            if (ifBodySpan.Length <= sbyte.MaxValue)
                context.Generator.Je((sbyte)ifBodySpan.Length);
            else
                context.Generator.Je(ifBodySpan.Length);

            ifBody.CopyToContext(context);
            elseBody.CopyToContext(context);
        }
    }
}