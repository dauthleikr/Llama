namespace Llama.Compiler.StatementCompilers
{
    using System;
    using Parser.Nodes;

    internal class DeleteCompiler : ICompileStatements<Delete>
    {
        public void Compile(Delete statement, ICompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}