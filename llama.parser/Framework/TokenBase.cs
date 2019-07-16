namespace Llama.Parser.Framework
{
    public abstract class TokenBase<TSelf> : IToken, ITokenizationResult<TSelf> where TSelf : class, IToken
    {
        public bool Successful => true;
        public IErrorWithConfidence ResultError => null;
        public TSelf ResultSuccess => this as TSelf;
        public abstract void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true);
    }
}