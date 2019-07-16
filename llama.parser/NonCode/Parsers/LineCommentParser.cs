namespace Llama.Parser.NonCode.Parsers
{
    using System.Text;
    using Abstractions;
    using Entities;

    public class LineCommentParser : NonCodeEntityParserBase<LineCommentEntity>
    {
        public override bool TryParse(ISourceReader reader, out LineCommentEntity nonCode)
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

            nonCode = new LineCommentEntity(builder.ToString());
            return true;
        }
    }
}