namespace Llama.Parser.Parsers
{
    using System.Text;
    using Framework;
    using Tokens;

    internal class IdentifierParser : AtomicTokenParser<IdentifierToken>
    {
        protected override ITokenizationResult<IdentifierToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            var readChar = reader.ReadChar();
            var builder = new StringBuilder();
            while (char.IsLetter(readChar) || char.IsDigit(readChar) || readChar == '_')
            {
                builder.Append(readChar);
                readChar = reader.ReadChar();
            }
            reader.Vomit();
            return new IdentifierToken(builder.ToString());
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context)
        {
            var peekChar = reader.Peek();
            return char.IsLetter(peekChar) || peekChar == '_';
        }
    }
}