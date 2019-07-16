namespace Llama.Parser.Parsers
{
    using Framework;
    using Tokens;

    internal class CommaParser : AtomicTokenParser<CommaToken>
    {
        protected override ITokenizationResult<CommaToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new CommaToken();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == ',';
    }
}