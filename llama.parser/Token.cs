namespace Llama.Parser
{
    using Abstractions;

    public readonly struct Token : IToken<TokenKind>
    {
        public readonly string RawText;

        public TokenKind Kind { get; }
        public bool IsTrivia => Kind == TokenKind.WhitespaceOrControl || Kind == TokenKind.LineComment || Kind == TokenKind.BlockComment;

        public Token(TokenKind kind, string rawText)
        {
            Kind = kind;
            RawText = rawText;
        }
    }
}