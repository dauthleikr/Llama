namespace Llama.Parser.Nodes
{
    using Lexer;

    public class UnaryOperator : IExpression
    {
        public Token Operator { get; }

        public UnaryOperator(Token @operator) => Operator = @operator;

        public static bool IsTokenKindValid(TokenKind kind) =>
            kind == TokenKind.AddressOf || kind == TokenKind.Pointer || kind == TokenKind.Minus || kind == TokenKind.Not;
    }
}