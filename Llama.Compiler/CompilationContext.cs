namespace Llama.Compiler
{
    using Parser.Nodes;

    internal class CompilationContext : ICompilationContext
    {
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store) => _store = store;

        public void CompileStatement<T>(T statement, IFunctionContext function, IAddressFixer addressFixer) where T : IStatement =>
            _store.GetStatementCompiler<T>().Compile(statement, function, addressFixer, this);

        public Type CompileExpression<T>(T expression, Register target, IFunctionContext function, IAddressFixer addressFixer)
            where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, function, addressFixer, this);
    }
}