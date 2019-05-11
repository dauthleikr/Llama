using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class LineCommentToken : INonCodeToken
    {
        public readonly string CommentText;

        public LineCommentToken(string commentText) => CommentText = commentText;

        public void WalkRecursive(ISourceWalker walker) => walker.Walk(this);

        public override string ToString() => $"//{CommentText}";

        public static bool TryParse(ISourceReader reader, out LineCommentToken result)
        {
            if (reader.Peek() != '/' || reader.PeekFurther(1) != '/')
            {
                result = null;
                return false;
            }
            var text = new StringBuilder();
            char readChar;
            while (!reader.IsAtEnd && (readChar = reader.ReadChar()) != '\n')
                text.Append(readChar);
            result = new LineCommentToken(text.ToString());
            return true;
        }
    }
}
