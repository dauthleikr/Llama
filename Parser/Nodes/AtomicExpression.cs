namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class AtomicExpression : IExpression
    {
        public Token Token { get; }

        public AtomicExpression(Token token) => Token = token;

        public static bool IsTokenKindValid(TokenKind kind) =>
            kind == TokenKind.FloatLiteral || kind == TokenKind.IntegerLiteral || kind == TokenKind.StringLiteral || kind == TokenKind.Identifier;
    }
}