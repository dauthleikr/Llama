namespace Llama.Parser.Parsers
{
    using Framework;
    using Tokens;

    internal class OpenParanthesisParser : AtomicTokenParser<OpenParanthesisToken>
    {
        protected override ITokenizationResult<OpenParanthesisToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new OpenParanthesisToken();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == '(';
    }
}