namespace Llama.Parser.Lexer
{
    using System;

    public readonly struct Token : IEquatable<Token>
    {
        public readonly string RawText;

        public TokenKind Kind { get; }
        public bool IsTrivia => Kind == TokenKind.WhitespaceOrControl || Kind == TokenKind.LineComment || Kind == TokenKind.BlockComment;

        public Token(TokenKind kind, string rawText)
        {
            Kind = kind;
            RawText = rawText;
        }

        public bool Equals(Token other) => RawText == other.RawText && Kind == other.Kind;

        public override bool Equals(object obj) => obj is Token other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((RawText != null ? RawText.GetHashCode() : 0) * 397) ^ (int)Kind;
            }
        }

        public static bool operator ==(Token left, Token right) => left.Equals(right);

        public static bool operator !=(Token left, Token right) => !left.Equals(right);

        public override string ToString() => $"{{ {nameof(Kind)}: {Kind}, {nameof(RawText)} = {RawText}}}";
    }
}