namespace Llama.Parser.NonCode.Entities
{
    public class BlockCommentEntity : AtomicNonCodeEntity
    {
        public readonly string CommentText;

        public BlockCommentEntity(string commentText) => CommentText = commentText;

        public override string ToString() => $"/*{CommentText}*/";
    }
}