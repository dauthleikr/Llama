namespace llama.syntax.Tokens
{
    public interface IToken
    {
        void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true);
    }
}
