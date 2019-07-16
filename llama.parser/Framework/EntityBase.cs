namespace Llama.Parser.Framework
{
    using Abstractions;

    public abstract class EntityBase<TSelf> : IEntity, IParseResult<TSelf> where TSelf : class, IEntity
    {
        public bool Successful => true;
        public IErrorWithConfidence ResultError => null;
        public TSelf ResultSuccess => this as TSelf;
        public abstract void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true);
    }
}