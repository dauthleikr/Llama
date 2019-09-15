namespace Llama.Parser
{
    using System;
    using Lexer;

    public interface IParseContext<out T> where T : Enum
    {
        IToken<T> NextCodeToken { get; }

        IToken<T> ReadCodeToken();
        TNode ReadNode<TNode>();
        void Panic(string message);
    }
}