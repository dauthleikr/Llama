namespace Llama.Compiler.StatementCompilers
{
    using System.Linq;
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class ForCompiler : ICompileStatements<For>
    {
        private readonly ICompileStatements<While> _whileCompiler;

        public ForCompiler(ICompileStatements<While> whileCompiler)
        {
            _whileCompiler = whileCompiler;
        }

        public void Compile(
            For statement,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        )
        {
            scope.PushScope();
            context.CompileStatement(statement.Variable, codeGen, storageManager, scope);

            var bodyAndIncrement = statement.Instruction.StatementAsBlock().Statements.Concat(new[] { statement.Increment }).ToArray();
            var equalWhile = new While(statement.Condition, new CodeBlock(bodyAndIncrement));
            _whileCompiler.Compile(equalWhile, codeGen, storageManager, scope, linkingInfo, context);
            scope.PopScope();
        }
    }
}