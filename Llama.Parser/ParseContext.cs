namespace Llama.Parser
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using Lexer;

    public class ParseContext : IParseContext
    {
        public Token NextCodeToken { get; private set; }
        private readonly Lexer.Lexer _lexer;
        private readonly IParseStore _parseStore;
        private readonly string _source;
        private int _sourcePosition;
        private readonly ConcurrentQueue<Token> _tokens = new ConcurrentQueue<Token>();
        private readonly Thread _lexerThread;
        private volatile bool _lexingHasEnded;

        public ParseContext(IParseStore parseStore, Lexer.Lexer lexer, string source)
        {
            _parseStore = parseStore;
            _lexer = lexer;
            _source = source;

            _lexerThread = new Thread(LexerThread);
            _lexerThread.Start();

            ReadCodeToken();
        }

        private void LexerThread()
        {
            while (true)
            {
                var token = _lexer.NextToken(_source, ref _sourcePosition);
                if (token.Kind == TokenKind.EndOfStream)
                {
                    _lexingHasEnded = true;
                    _tokens.Enqueue(token);
                    return;
                }

                _tokens.Enqueue(token);
            }
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
            if (_lexingHasEnded && _tokens.IsEmpty)
                return NextCodeToken;

            var token = NextCodeToken;
            Token nextToken;
            var wait = new SpinWait();
            while (!_tokens.TryDequeue(out nextToken))
                wait.SpinOnce();

            NextCodeToken = nextToken;
            return token;
        }
    }
}