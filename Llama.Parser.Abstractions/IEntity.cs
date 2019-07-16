namespace Llama.Parser.Abstractions
{
    public interface IEntity
    {
        void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true);
    }
}