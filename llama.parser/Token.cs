namespace Llama.Parser
{
    public readonly struct Token
    {
        public readonly TokenKind Kind;
        public readonly string RawText;

        public bool IsTrivia => Kind == TokenKind.WhitespaceOrControl || Kind == TokenKind.LineComment || Kind == TokenKind.BlockComment;

        public Token(TokenKind kind, string rawText)
        {
            Kind = kind;
            RawText = rawText;
        }
    }
}