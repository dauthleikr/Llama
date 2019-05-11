namespace Llama.Parser.NonCode.Parsers
{
    using System.Text;
    using Framework;
    using Tokens;

    public class BlockCommentParser : NonCodeTokenParserBase<BlockCommentToken>
    {
        public override bool TryParse(ISourceReader reader, out BlockCommentToken nonCode)
        {
            if (reader.ReadChar() != '/' || reader.ReadChar() != '*')
            {
                nonCode = null;
                return false;
            }

            var builder = new StringBuilder();
            while (reader.Peek() != '*' || reader.PeekFurther(1) != '/')
            {
                var readChar = reader.ReadChar();
                builder.Append(readChar);

                if (reader.IsAtEnd)
                {
                    nonCode = null;
                    return false;
                }
            }

            reader.Eat(2);
            nonCode = new BlockCommentToken(builder.ToString());
            return true;
        }
    }
}