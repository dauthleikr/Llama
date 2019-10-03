namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public class CompilationContext : ICompilationContext
    {
        private readonly IAddressFixer _addressFixer;
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store, IAddressFixer addressFixer)
        {
            _store = store;
            _addressFixer = addressFixer;
        }

        public void CompileStatement<T>(T statement, CodeGen codeGen, IScopeContext scope) where T : IStatement =>
            _store.GetStatementCompiler<T>().Compile(statement, codeGen, scope, _addressFixer, this);

        public Type CompileExpression<T>(T expression, CodeGen codeGen, Register target, IScopeContext scope) where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, codeGen, scope, _addressFixer, this);
    }
}