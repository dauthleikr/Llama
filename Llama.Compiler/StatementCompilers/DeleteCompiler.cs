namespace Llama.Compiler.StatementCompilers
{
    using System;
    using Parser.Nodes;
    using spit;

    internal class DeleteCompiler : ICompileStatements<Delete>
    {
        public void Compile(
            Delete statement,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        )
        {
            throw new NotImplementedException();
        }
    }
}