namespace Llama.Parser
{
    using Framework;

    public abstract class AtomicToken<TSelf> : TokenBase<TSelf>, IAtomicToken where TSelf : class, IAtomicToken
    {
        public INonCode PreNonCode { get; set; }
        public INonCode PostNonCode { get; set; }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            if (!codeChildrenOnly && PreNonCode != null)
                walker.Walk(PreNonCode);
            walker.Walk(this);
            if (!codeChildrenOnly && PreNonCode != null)
                walker.Walk(PreNonCode);
        }

        public sealed override string ToString() => $"{PreNonCode}{ToStringInternal()}{PostNonCode}";

        protected abstract string ToStringInternal();
    }
}