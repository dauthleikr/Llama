namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public class CompilationContext : ICompilationContext
    {
        public IAddressFixer AddressLinker { get; }
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store, IAddressFixer addressLinker)
        {
            _store = store;
            AddressLinker = addressLinker;
        }

        public void CompileStatement<T>(T statement, CodeGen codeGen, StorageManager storageManager, IScopeContext scope) where T : IStatement =>
            _store.GetStatementCompiler<T>().Compile(statement, codeGen, storageManager, scope, AddressLinker, this);

        public ExpressionResult CompileExpression<T>(
            T expression,
            CodeGen codeGen,
            StorageManager storageManager,
            PreferredRegister target,
            IScopeContext scope
        ) where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, codeGen, storageManager, scope, AddressLinker, this);
    }
}