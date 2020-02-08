namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class ReturnCompiler : ICompileStatements<Return>
    {
        public void Compile(
            Return statement,
            ICompilationContext context
        )
        {
            if (statement.ReturnValue != null)
            {
                var returnRegisters = new PreferredRegister(Register64.RAX, XmmRegister.XMM0);
                var returnResult = context.CompileExpression(statement.ReturnValue, returnRegisters);
                var myFunction = context.Symbols.GetFunctionDeclaration(context.Symbols.CurrentFunctionIdentifier);

                returnResult.GenerateMoveTo(returnRegisters.MakeFor(myFunction.ReturnType), myFunction.ReturnType, context.Generator, context.Linking);
            }

            context.Generator.Jmp(Constants.DummyOffsetInt);
            context.Linking.FixFunctionEpilogueOffset(context.Generator.StreamPosition, context.Symbols.CurrentFunctionIdentifier);
        }
    }
}