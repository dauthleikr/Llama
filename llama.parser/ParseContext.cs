namespace Llama.Parser
{
    internal class ParseContext : IParseContext<TokenKind>
    {
        public IToken<TokenKind> NextCodeToken { get; private set; }
        private readonly Lexer _lexer;
        private readonly IParseStore _parseStore;
        private readonly string _source;
        private int _sourcePosition;

        public ParseContext(IParseStore parseStore, Lexer lexer, string source)
        {
            _parseStore = parseStore;
            _lexer = lexer;
            _source = source;

            PrepareNextToken();
        }

        public IToken<TokenKind> ReadCodeToken()
        {
            var token = NextCodeToken;
            PrepareNextToken();
            return token;
        }

        public TNode ReadNode<TNode>() => _parseStore.GetStrategyFor<TNode>().Read(this);

        public void Panic(string message) => throw new ParserException(message);

        private void PrepareNextToken() => NextCodeToken = _lexer.NextNonTriviaToken(_source, ref _sourcePosition);
    }
}