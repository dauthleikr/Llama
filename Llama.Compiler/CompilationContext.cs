namespace Llama.Compiler
{
    using Parser.Nodes;
    using spit;

    public class CompilationContext : ICompilationContext
    {
        public IAddressFixer AddressLinker { get; }
        private readonly IFactory<IAddressFixer> _linkerFactory;
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store, IFactory<IAddressFixer> linkerFactory)
        {
            _store = store;
            _linkerFactory = linkerFactory;
            AddressLinker = linkerFactory.Create();
        }

        public void CompileStatement<T>(T statement, CodeGen codeGen, StorageManager storageManager, IScopeContext scope) where T : IStatement =>
            _store.GetStatementCompiler<T>().Compile(statement, codeGen, storageManager, scope, AddressLinker, this);

        public ICompilationContext CreateChildContext() => new CompilationContext(_store, _linkerFactory);

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