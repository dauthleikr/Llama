namespace Llama.Parser.NonCode.Tokens
{
    public class BlockCommentToken : AtomicNonCodeToken
    {
        public readonly string CommentText;

        public BlockCommentToken(string commentText) => CommentText = commentText;

        public override string ToString() => $"/*{CommentText}*/";
    }
}