namespace Llama.Parser.NonCode.Parsers
{
    using System.Text;
    using Framework;
    using Tokens;

    public class WhitespaceParser : NonCodeTokenParserBase<WhitespaceToken>
    {
        public override bool TryParse(ISourceReader reader, out WhitespaceToken nonCode)
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
                nonCode = new WhitespaceToken(builder.ToString());
                return true;
            }

            nonCode = null;
            return false;
        }
    }
}