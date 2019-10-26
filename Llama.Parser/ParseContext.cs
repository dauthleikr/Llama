namespace Llama.Parser
{
    using Lexer;

    public class ParseContext : IParseContext
    {
        public Token NextCodeToken { get; private set; }
        private readonly Lexer.Lexer _lexer;
        private readonly IParseStore _parseStore;
        private readonly string _source;
        private int _sourcePosition;

        public ParseContext(IParseStore parseStore, Lexer.Lexer lexer, string source)
        {
            _parseStore = parseStore;
            _lexer = lexer;
            _source = source;

            PrepareNextToken();
        }

        public TNode ReadNode<TNode>()
        {
            var parser = _parseStore.GetStrategyFor<TNode>();
            if (parser == null)
            {
                Panic($"No parser found for: {typeof(TNode).Name}");
                return default;
            }

            return parser.Read(this);
        }

        public void Panic(string message) => throw new ParserException(message);

        public Token ReadCodeToken()
        {
            var token = NextCodeToken;
            PrepareNextToken();
            return token;
        }

        private void PrepareNextToken() => NextCodeToken = _lexer.NextToken(_source, ref _sourcePosition);
    }
}