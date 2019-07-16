namespace Llama.Parser.NonCode
{
    using Abstractions;

    public abstract class AtomicNonCodeEntity : INonCode
    {
        public void WalkRecursive(ISourceWalker walker) => walker.Walk(this);
    }
}