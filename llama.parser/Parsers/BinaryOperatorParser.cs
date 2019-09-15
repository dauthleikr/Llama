namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class BinaryOperatorParser : IParse<BinaryOperator>
    {
        public BinaryOperator Read(IParseContext context)
        {
            switch (context.NextCodeToken.Kind)
            {
                case TokenKind.Plus:
                case TokenKind.Minus:
                    return new BinaryOperator(context.ReadCodeToken());
                default:
                    context.Panic($"Expected {nameof(BinaryOperator)}");
                    return null;
            }
        }
    }
}