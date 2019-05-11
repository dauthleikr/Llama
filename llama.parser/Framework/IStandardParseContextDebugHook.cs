using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Framework
{
    public interface IStandardParseContextDebugHook
    {
        void IncreaseLevel(Type tokenType);
        void DecreaseLevel();
        void ParsingSkippedNotPlausible(ISourcePeeker source);
        void ParsingEnd<T>(ITokenizationResult<T> result) where T : class, IToken;
        void ParsingStart(ISourcePeeker source);
    }
}
