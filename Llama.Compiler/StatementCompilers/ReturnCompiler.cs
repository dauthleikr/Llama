namespace Llama.Compiler.StatementCompilers
{
    using Parser.Nodes;
    using spit;

    internal class ReturnCompiler : ICompileStatements<Return>
    {
        public void Compile(
            Return statement,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        )
        {
            if (statement.ReturnValue != null)
            {
                var returnRegisters = new PreferredRegister(Register64.RAX, XmmRegister.XMM0);
                var returnResult = context.CompileExpression(statement.ReturnValue, codeGen, storageManager, returnRegisters, scope);
                var myFunction = scope.GetFunctionDeclaration(scope.CurrentFunctionIdentifier);

                returnResult.GenerateMoveTo(returnRegisters.MakeFor(myFunction.ReturnType), myFunction.ReturnType, codeGen, linkingInfo);
            }

            codeGen.Jmp(Constants.DummyOffsetInt);
            linkingInfo.FixFunctionEpilogueOffset(codeGen.StreamPosition, scope.CurrentFunctionIdentifier);
        }
    }
}