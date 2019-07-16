namespace Llama.Parser.NonCode.Parsers
{
    using System.Text;
    using Abstractions;
    using Entities;

    public class BlockCommentParser : NonCodeEntityParserBase<BlockCommentEntity>
    {
        public override bool TryParse(ISourceReader reader, out BlockCommentEntity nonCode)
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
            nonCode = new BlockCommentEntity(builder.ToString());
            return true;
        }
    }
}