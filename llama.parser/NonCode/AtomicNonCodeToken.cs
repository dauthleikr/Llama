namespace Llama.Parser.NonCode
{
    using Framework;

    public abstract class AtomicNonCodeToken : INonCode
    {
        public void WalkRecursive(ISourceWalker walker) => walker.Walk(this);
    }
}