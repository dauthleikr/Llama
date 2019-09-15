namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class UnaryOperator
    {
        public Token Operator { get; }

        public UnaryOperator(Token @operator) => Operator = @operator;
    }
}