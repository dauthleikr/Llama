namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class UnaryOperator
    {
        public Token Operator { get; }

        public UnaryOperator(Token @operator) => Operator = @operator;

        public static bool IsTokenKindValid(TokenKind kind) => kind == TokenKind.AddressOf || kind == TokenKind.Pointer;
    }
}