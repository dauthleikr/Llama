namespace Llama.Compiler
{
    using Parser.Nodes;

    public sealed class LlamaCompilerStore : ICompilerStore
    {
        private struct ExpressionCompilerStore<T> where T : IExpression
        {
            public static ICompileExpressions<T> Compiler;
        }

        private struct StatementCompilerStore<T> where T : IStatement
        {
            public static ICompileStatements<T> Compiler;
        }

        public static LlamaCompilerStore Instance { get; } = new LlamaCompilerStore();

        private LlamaCompilerStore()
        {
            // todo
        }

        public ICompileExpressions<T> GetExpressionCompiler<T>() where T : IExpression => ExpressionCompilerStore<T>.Compiler;

        public ICompileStatements<T> GetStatementCompiler<T>() where T : IStatement => StatementCompilerStore<T>.Compiler;
    }
}