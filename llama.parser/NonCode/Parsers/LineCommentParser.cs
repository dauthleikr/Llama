namespace Llama.Parser.NonCode.Parsers
{
    using System.Text;
    using Framework;
    using Tokens;

    public class LineCommentParser : NonCodeTokenParserBase<LineCommentToken>
    {
        public override bool TryParse(ISourceReader reader, out LineCommentToken nonCode)
        {
            if (reader.ReadChar() != '/' || reader.ReadChar() != '/')
            {
                nonCode = null;
                return false;
            }

            var builder = new StringBuilder();
            while (!reader.IsAtEnd)
            {
                var readChar = reader.ReadChar();
                builder.Append(readChar);
                if (readChar == '\n')
                    break;
            }

            nonCode = new LineCommentToken(builder.ToString());
            return true;
        }
    }
}