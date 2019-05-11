using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Framework
{
    public interface ITokenizationResult<out T> where T : class, IToken
    {
        bool Successful { get; }
        IErrorWithConfidence ResultError { get; }
        T ResultSuccess { get;}
    }
}
