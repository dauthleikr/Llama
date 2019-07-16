namespace Llama.Parser.Parsers
{
    using System.Text;
    using Abstractions;
    using Entities;
    using Framework;

    public class IdentifierParser : AtomicEntityParser<IdentifierEntity>
    {
        protected override IParseResult<IdentifierEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context)
        {
            var readChar = reader.ReadChar();
            var builder = new StringBuilder();
            while (char.IsLetter(readChar) || char.IsDigit(readChar) || readChar == '_')
            {
                builder.Append(readChar);
                readChar = reader.ReadChar();
            }

            reader.Vomit();
            return new IdentifierEntity(builder.ToString());
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context)
        {
            var peekChar = reader.Peek();
            return char.IsLetter(peekChar) || peekChar == '_';
        }
    }
}