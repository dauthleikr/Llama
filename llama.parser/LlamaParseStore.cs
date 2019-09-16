namespace Llama.Parser
{
    using Nodes;
    using Parsers;

    public sealed class LlamaParseStore : IParseStore
    {
        private struct Store<T>
        {
            public static IParse<T> Parser;
        }

        public static LlamaParseStore Instance { get; } = new LlamaParseStore();

        static LlamaParseStore()
        {
            Store<IExpression>.Parser = new ExpressionParser();
            Store<IStatement>.Parser = new StatementParser();
            Store<BinaryOperator>.Parser = new BinaryOperatorParser();
            Store<FunctionDeclaration>.Parser = new FunctionDeclarationParser();
            Store<FunctionImplementation>.Parser = new FunctionImplementationParser();
            Store<FunctionImport>.Parser = new FunctionImportParser();
            Store<LlamaDocument>.Parser = new LlamaDocumentParser();
            Store<Type>.Parser = new TypeParser();
            Store<UnaryOperator>.Parser = new UnaryOperatorParser();
            Store<Declaration>.Parser = new DeclarationParser();
        }

        private LlamaParseStore() { }

        public IParse<T> GetStrategyFor<T>() => Store<T>.Parser;
    }
}