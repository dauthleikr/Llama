namespace Llama.Parser.Abstractions
{
    public interface INonCode
    {
        void WalkRecursive(ISourceWalker walker);
    }
}