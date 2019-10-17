namespace Llama.Parser.Parsers
{
    using Nodes;

    internal class BinaryOperatorParser : IParse<BinaryOperator>
    {
        public BinaryOperator Read(IParseContext context)
        {
            if (BinaryOperator.IsTokenKindValid(context.NextCodeToken.Kind))
                return new BinaryOperator(context.ReadCodeToken());

            context.Panic($"Expected {nameof(BinaryOperator)}, but got: {context.NextCodeToken}");
            return null;
        }
    }
}