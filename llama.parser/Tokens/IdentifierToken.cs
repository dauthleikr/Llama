namespace Llama.Parser.Tokens
{
    public class IdentifierToken : AtomicToken<IdentifierToken>, IExpressionToken
    {
        public readonly string Identifier;

        public IdentifierToken(string identifier) => Identifier = identifier;

        protected override string ToStringInternal() => Identifier;
    }
}