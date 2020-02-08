namespace Llama.Compiler
{
    using System;
    using Parser.Nodes;
    using spit;

    public class CompilationContext : ICompilationContext
    {
        public ILinkingInfo Linking { get; }
        public ISymbolResolver Symbols { get; }
        public StorageManager Storage { get; }
        public CodeGen Generator { get; } = new CodeGen();

        private readonly IFactory<ILinkingInfo> _linkingInfoFactory;
        private readonly ICompilerStore _store;

        public CompilationContext(ICompilerStore store, IFactory<ILinkingInfo> linkingInfoFactory, ISymbolResolver symbolResolver, StorageManager storageManager)
        {
            Symbols = symbolResolver;
            Storage = storageManager;
            Linking = linkingInfoFactory.Create();

            _store = store;
            _linkingInfoFactory = linkingInfoFactory;
        }

        public void CopyToContext(ICompilationContext other)
        {
            Linking.CopyTo(other.Linking, other.Generator.StreamPosition);
            other.Generator.Write(Generator.GetBufferSpan());
        }

        public void CompileStatement<T>(T statement) where T : IStatement
        {
            var compiler = _store.GetStatementCompiler<T>();
            if (compiler == null)
                throw new NotImplementedException($"{nameof(CompilationContext)}: I can not get a compiler for: {statement}");
            compiler.Compile(statement, this);
        }

        public ICompilationContext CreateChildContext() => new CompilationContext(_store, _linkingInfoFactory, Symbols, Storage);

        public ExpressionResult CompileExpression<T>(
            T expression,
            PreferredRegister target
        ) where T : IExpression =>
            _store.GetExpressionCompiler<T>().Compile(expression, target, this);
    }
}