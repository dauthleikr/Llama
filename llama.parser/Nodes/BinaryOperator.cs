namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class BinaryOperator
    {
        public Token Operator { get; }

        public BinaryOperator(Token @operator) => Operator = @operator;

        public static bool IsTokenKindValid(TokenKind kind) => kind == TokenKind.Plus || kind == TokenKind.Minus;
    }
}