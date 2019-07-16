namespace Llama.Parser.NonCode.Parsers
{
    using System.Text;
    using Abstractions;
    using Entities;

    public class WhitespaceParser : NonCodeEntityParserBase<WhitespaceEntity>
    {
        public override bool TryParse(ISourceReader reader, out WhitespaceEntity nonCode)
        {
            var builder = new StringBuilder();
            var peekChar = reader.Peek();
            while ((char.IsWhiteSpace(peekChar) || char.IsControl(peekChar)) && peekChar != '\0')
            {
                builder.Append(peekChar);
                reader.Eat();
                peekChar = reader.Peek();
            }

            if (builder.Length > 0)
            {
                nonCode = new WhitespaceEntity(builder.ToString());
                return true;
            }

            nonCode = null;
            return false;
        }
    }
}