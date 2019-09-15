namespace Llama.Parser.Abstractions
{
    using System;

    public interface IToken<out T> where T:Enum
    {
        T Kind { get; }
        bool IsTrivia { get; }
    }
}