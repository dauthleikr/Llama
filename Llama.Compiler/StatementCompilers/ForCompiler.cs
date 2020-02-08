namespace Llama.Compiler.StatementCompilers
{
    using System.Linq;
    using Extensions;
    using Parser.Nodes;

    internal class ForCompiler : ICompileStatements<For>
    {
        private readonly ICompileStatements<While> _whileCompiler;

        public ForCompiler(ICompileStatements<While> whileCompiler)
        {
            _whileCompiler = whileCompiler;
        }

        public void Compile(
            For statement,
            ICompilationContext context
        )
        {
            context.Symbols.PushLocalScope();
            context.CompileStatement(statement.Variable);

            var bodyAndIncrement = statement.Instruction.StatementAsBlock().Statements.Concat(new[] { statement.Increment }).ToArray();
            var equalWhile = new While(statement.Condition, new CodeBlock(bodyAndIncrement));
            _whileCompiler.Compile(equalWhile, context);
            context.Symbols.PopLocalScope();
        }
    }
}