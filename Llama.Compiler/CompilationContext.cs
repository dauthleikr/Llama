namespace Llama.Compiler
{
    using System;
    using Parser.Nodes;
    using spit;

    public class CompilationContext : ICompilationContext
    {
        public ILinkingInfo AddressLinker { get; }
        private readonly IFactory<ILinkingInfo> _linkerFactory;
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store, IFactory<ILinkingInfo> linkerFactory)
        {
            _store = store;
            _linkerFactory = linkerFactory;
            AddressLinker = linkerFactory.Create();
        }

        public void CompileStatement<T>(T statement, CodeGen codeGen, StorageManager storageManager, ISymbolResolver scope) where T : IStatement
        {
            var compiler = _store.GetStatementCompiler<T>();
            if (compiler == null)
                throw new NotImplementedException($"{nameof(CompilationContext)}: I can not get a compiler for: {statement}");
            compiler.Compile(statement, codeGen, storageManager, scope, AddressLinker, this);
        }

        public ICompilationContext CreateChildContext() => new CompilationContext(_store, _linkerFactory);

        public ExpressionResult CompileExpression<T>(
            T expression,
            CodeGen codeGen,
            StorageManager storageManager,
            PreferredRegister target,
            ISymbolResolver scope
        ) where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, codeGen, storageManager, scope, AddressLinker, this);
    }
}