namespace Llama.Parser.Framework
{
    using System;
    using Abstractions;

    public interface IStandardParseContextDebugHook
    {
        void IncreaseLevel(Type tokenType);
        void DecreaseLevel();
        void ParsingSkippedNotPlausible(ISourcePeeker source);
        void ParsingEnd<T>(IParseResult<T> result) where T : class, IEntity;
        void ParsingStart(ISourcePeeker source);
    }
}