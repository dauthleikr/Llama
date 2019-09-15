namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class UnaryOperatorParser : IParse<UnaryOperator>
    {
        public UnaryOperator Read(IParseContext context)
        {
            switch (context.NextCodeToken.Kind)
            {
                case TokenKind.AddressOf:
                case TokenKind.Pointer:
                    return new UnaryOperator(context.ReadCodeToken());
                default:
                    context.Panic($"Expected {nameof(UnaryOperatorParser)}");
                    return null;
            }
        }
    }
}