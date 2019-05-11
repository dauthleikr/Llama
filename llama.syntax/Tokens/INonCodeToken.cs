namespace llama.syntax.Tokens
{
    public interface INonCodeToken
    {
        void WalkRecursive(ISourceWalker walker);
    }
}
