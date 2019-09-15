namespace Llama.Parser.Lexer
{
    using System;

    public interface IToken<out T> where T : Enum
    {
        T Kind { get; }
        bool IsTrivia { get; }
    }
}