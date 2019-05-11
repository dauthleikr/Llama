namespace Llama.Parser.Framework
{
    public interface IToken 
    {
        void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true);
    }
}