namespace Llama.Parser.NonCode.Entities
{
    public class LineCommentEntity : AtomicNonCodeEntity
    {
        public readonly string CommentTextWithNewline;

        public LineCommentEntity(string commentTextWithNewline) => CommentTextWithNewline = commentTextWithNewline;

        public override string ToString() => $"//{CommentTextWithNewline}";
    }
}