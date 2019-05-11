namespace Llama.Parser.NonCode.Tokens
{
    public class LineCommentToken : AtomicNonCodeToken
    {
        public readonly string CommentTextWithNewline;

        public LineCommentToken(string commentTextWithNewline) => CommentTextWithNewline = commentTextWithNewline;

        public override string ToString() => $"//{CommentTextWithNewline}";
    }
}