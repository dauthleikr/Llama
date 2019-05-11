using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class BlockCommentToken : INonCodeToken
    {
        public readonly string CommentText;

        public BlockCommentToken(string commentText) => CommentText = commentText;

        public void WalkRecursive(ISourceWalker walker) => walker.Walk(this);

        public static bool TryParse(ISourceReader reader, out BlockCommentToken result)
        {
            if (reader.Peek() != '/' || reader.PeekFurther(1) != '*')
            {
                result = null;
                return false;
            }

            var text = new StringBuilder();
            while (!reader.IsAtEnd)
            {
                var readChar = reader.ReadChar();
                if (readChar == '*' && reader.ReadFurtherIfEqual(1, '/'))
                {
                    result = new BlockCommentToken(text.ToString());
                    return true;
                }
                text.Append(readChar);
            }

            reader.ReportSyntaxError(reader.Position - 2, 2, "Expected end of comment block");
            result = null;
            return false;
        }

        public override string ToString() => $"/*{CommentText}*/";
    }
}
