namespace Llama.Parser.Parsers
{
    using Framework;
    using Tokens;

    internal class CloseParanthesisParser : AtomicTokenParser<CloseParanthesisToken>
    {
        protected override ITokenizationResult<CloseParanthesisToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new CloseParanthesisToken();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == ')';
    }
}