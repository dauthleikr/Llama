namespace Llama.Parser.NonCode
{
    using Abstractions;
    using Framework;

    public abstract class AtomicNonCodeEntity : INonCode
    {
        public void WalkRecursive(ISourceWalker walker) => walker.Walk(this);
    }
}