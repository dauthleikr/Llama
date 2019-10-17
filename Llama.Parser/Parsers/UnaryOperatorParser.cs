namespace Llama.Parser.Parsers
{
    using Nodes;

    internal class UnaryOperatorParser : IParse<UnaryOperator>
    {
        public UnaryOperator Read(IParseContext context)
        {
            if (UnaryOperator.IsTokenKindValid(context.NextCodeToken.Kind))
                return new UnaryOperator(context.ReadCodeToken());

            context.Panic($"Expected {nameof(UnaryOperator)}, but got: {context.NextCodeToken}");
            return null;
        }
    }
}