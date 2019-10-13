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

        public void CompileStatement<T>(T statement, CodeGen codeGen, StorageManager storageManager, IScopeContext scope) where T : IStatement =>
            _store.GetStatementCompiler<T>().Compile(statement, codeGen, storageManager, scope, _addressFixer, this);

        public ExpressionResult CompileExpression<T>(
            T expression,
            CodeGen codeGen,
            StorageManager storageManager,
            PreferredRegister target,
            IScopeContext scope
        ) where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, codeGen, storageManager, scope, _addressFixer, this);
    }
}