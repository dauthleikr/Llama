namespace Llama.Compiler
{
    using Parser.Nodes;

    internal class CompilationContext : ICompilationContext
    {
        private readonly IAddressFixer _addressFixer;
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store, IAddressFixer addressFixer)
        {
            _store = store;
            _addressFixer = addressFixer;
        }

        public void CompileStatement<T>(T statement, IFunctionContext function) where T : IStatement =>
            _store.GetStatementCompiler<T>().Compile(statement, function, _addressFixer, this);

        public Type CompileExpression<T>(T expression, Register target, IFunctionContext function) where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, function, _addressFixer, this);
    }
}