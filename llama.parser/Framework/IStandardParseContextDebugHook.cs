namespace Llama.Parser.Framework
{
    using System;

    public interface IStandardParseContextDebugHook
    {
        void IncreaseLevel(Type tokenType);
        void DecreaseLevel();
        void ParsingSkippedNotPlausible(ISourcePeeker source);
        void ParsingEnd<T>(ITokenizationResult<T> result) where T : class, IToken;
        void ParsingStart(ISourcePeeker source);
    }
}